using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JourneyGen;

/// <summary>
/// Parses every .cs file the repo owns and reports (a) the types declared and
/// (b) which C#/.NET features actually show up in the source.
/// Syntax-only: no compilation, no MSBuild, so it runs in a couple of seconds on CI.
/// </summary>
public sealed class CodeAnalyzer
{
    static readonly string[] SkipSegments =
        [ "/bin/", "/obj/", "/wwwroot/lib/", "/tools/", "/.git/", "/artifacts/" ];

    public List<TypeDecl> Types { get; } = new();
    public List<ProjectStats> Projects { get; } = new();

    /// <summary>feature id -> repo-relative files where it was seen.</summary>
    public Dictionary<string, List<string>> Features { get; } = new(StringComparer.OrdinalIgnoreCase);

    public List<string> SourceFiles { get; } = new();
    public int TotalLines { get; private set; }

    readonly string _root;

    public CodeAnalyzer(string root) => _root = root;

    public void Run()
    {
        var files = Directory.EnumerateFiles(_root, "*.cs", SearchOption.AllDirectories)
            .Where(f => !SkipSegments.Any(s => Norm(f).Contains(s, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(f => f, StringComparer.Ordinal)
            .ToList();

        var perProject = new Dictionary<string, ProjectStats>(StringComparer.OrdinalIgnoreCase);
        var opts = new CSharpParseOptions(LanguageVersion.Preview);

        foreach (var file in files)
        {
            var rel = Rel(file);
            var (projectDir, project) = ProjectResolver.Resolve(_root, rel);
            var text = File.ReadAllText(file);
            var lines = CountLines(text);

            SourceFiles.Add(rel);
            TotalLines += lines;

            if (!perProject.TryGetValue(project, out var stats))
                perProject[project] = stats = new ProjectStats { Name = project, Dir = projectDir };
            stats.Files++;
            stats.Lines += lines;

            var tree = CSharpSyntaxTree.ParseText(text, opts, path: file);
            var root = tree.GetCompilationUnitRoot();

            var before = Types.Count;
            CollectTypes(root, rel, project);
            stats.Types += Types.Count - before;

            var walker = new FeatureWalker();
            walker.Visit(root);
            foreach (var feature in walker.Found)
                Add(feature, rel);
        }

        Projects.AddRange(perProject.Values.OrderByDescending(p => p.Lines));
        DetectRepoShapeFeatures();
    }

    void CollectTypes(SyntaxNode root, string rel, string project)
    {
        foreach (var node in root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
        {
            var decl = new TypeDecl
            {
                Name = node.Identifier.Text,
                Kind = KindOf(node),
                Namespace = NamespaceOf(node),
                Project = project,
                File = rel,
                Lines = node.GetLocation().GetLineSpan().EndLinePosition.Line
                      - node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                IsAbstract = node.Modifiers.Any(SyntaxKind.AbstractKeyword),
                IsStatic = node.Modifiers.Any(SyntaxKind.StaticKeyword),
                IsSealed = node.Modifiers.Any(SyntaxKind.SealedKeyword),
            };

            if (node.BaseList is not null)
                foreach (var b in node.BaseList.Types)
                    decl.BaseTypes.Add(b.Type.ToString());

            // Records can declare their state positionally.
            if (node is RecordDeclarationSyntax { ParameterList: { } recordParams })
                foreach (var p in recordParams.Parameters)
                    decl.Members.Add(new MemberDecl
                    {
                        Name = p.Identifier.Text, Kind = "property", Type = p.Type?.ToString() ?? "",
                    });

            if (node is TypeDeclarationSyntax typeDecl)
                foreach (var m in typeDecl.Members)
                    AddMember(decl, m, decl.Kind == "interface");
            else if (node is EnumDeclarationSyntax enumDecl)
                foreach (var m in enumDecl.Members)
                    decl.Members.Add(new MemberDecl { Name = m.Identifier.Text, Kind = "enumMember" });

            Types.Add(decl);
        }
    }

    static void AddMember(TypeDecl decl, MemberDeclarationSyntax m, bool implicitlyPublic = false)
    {
        string Access(SyntaxTokenList mods) =>
            implicitlyPublic && !mods.Any(SyntaxKind.PrivateKeyword) ? "public" : AccessOf(mods);

        switch (m)
        {
            case PropertyDeclarationSyntax p:
                decl.Members.Add(new MemberDecl
                {
                    Name = p.Identifier.Text, Kind = "property", Type = p.Type.ToString(),
                    Accessibility = Access(p.Modifiers),
                    IsStatic = p.Modifiers.Any(SyntaxKind.StaticKeyword),
                    IsAbstract = p.Modifiers.Any(SyntaxKind.AbstractKeyword),
                    IsOverride = p.Modifiers.Any(SyntaxKind.OverrideKeyword),
                });
                break;
            case MethodDeclarationSyntax me:
                decl.Members.Add(new MemberDecl
                {
                    Name = me.Identifier.Text, Kind = "method", Type = me.ReturnType.ToString(),
                    Accessibility = Access(me.Modifiers),
                    IsStatic = me.Modifiers.Any(SyntaxKind.StaticKeyword),
                    IsAbstract = me.Modifiers.Any(SyntaxKind.AbstractKeyword),
                    IsOverride = me.Modifiers.Any(SyntaxKind.OverrideKeyword),
                });
                break;
            case FieldDeclarationSyntax f:
                foreach (var v in f.Declaration.Variables)
                    decl.Members.Add(new MemberDecl
                    {
                        Name = v.Identifier.Text, Kind = "field", Type = f.Declaration.Type.ToString(),
                        Accessibility = Access(f.Modifiers),
                        IsStatic = f.Modifiers.Any(SyntaxKind.StaticKeyword),
                    });
                break;
            case ConstructorDeclarationSyntax c:
                decl.Members.Add(new MemberDecl
                {
                    Name = c.Identifier.Text, Kind = "ctor", Accessibility = Access(c.Modifiers),
                });
                break;
        }
    }

    /// <summary>Features that are about repo layout rather than syntax.</summary>
    void DetectRepoShapeFeatures()
    {
        foreach (var t in Types)
        {
            if (t.BaseTypes.Any(b => b.EndsWith("Exception", StringComparison.Ordinal)))
                Add("custom-exceptions", t.File);
            if (t.BaseTypes.Any(b => b is "Attribute" or "System.Attribute"))
                Add("custom-attributes", t.File);
            if (t.BaseTypes.Any(b => b.EndsWith("Controller", StringComparison.Ordinal)))
                Add("mvc-controllers", t.File);
            if (t.Kind == "interface")
                Add("interfaces", t.File);
            if (t.IsAbstract && t.Kind == "class")
                Add("abstract-classes", t.File);
            if (t.IsStatic)
                Add("static-classes", t.File);
            if (t.IsSealed)
                Add("sealed-classes", t.File);

            // Inheriting from a class we declare ourselves, not just implementing interfaces.
            var ownClassNames = Types.Where(x => x.Kind is "class" or "record").Select(x => x.Name).ToHashSet(StringComparer.Ordinal);
            if (t.BaseTypes.Any(b => ownClassNames.Contains(StripGeneric(b))))
                Add("inheritance", t.File);

            // Naming-convention signals for common layering patterns.
            if (t.Name.EndsWith("Repository", StringComparison.Ordinal) || t.Name.StartsWith("IRepository", StringComparison.Ordinal))
                Add("repository-pattern", t.File);
            if (t.Name.EndsWith("Factory", StringComparison.Ordinal))
                Add("factory-pattern", t.File);
            if (t.Name.EndsWith("Service", StringComparison.Ordinal))
                Add("service-layer", t.File);
        }

        foreach (var f in SourceFiles)
        {
            if (f.Contains("Test", StringComparison.OrdinalIgnoreCase) && f.EndsWith(".cs", StringComparison.Ordinal))
                Add("unit-tests", f);
            if (f.Contains("/Views/", StringComparison.Ordinal))
                Add("razor-views", f);
        }

        foreach (var p in Projects)
        {
            var dir = Path.Combine(_root, p.Dir);
            if (Directory.Exists(Path.Combine(dir, "Views"))) Add("razor-views", p.Dir + "/Views");
            if (Directory.Exists(Path.Combine(dir, "wwwroot"))) Add("static-files", p.Dir + "/wwwroot");
        }
    }

    public void Add(string feature, string evidence)
    {
        if (!Features.TryGetValue(feature, out var list))
            Features[feature] = list = new List<string>();
        if (!list.Contains(evidence)) list.Add(evidence);
    }

    public static string StripGeneric(string s)
    {
        var i = s.IndexOf('<');
        return i < 0 ? s : s[..i];
    }

    static string KindOf(BaseTypeDeclarationSyntax n) => n switch
    {
        RecordDeclarationSyntax => "record",
        ClassDeclarationSyntax => "class",
        StructDeclarationSyntax => "struct",
        InterfaceDeclarationSyntax => "interface",
        EnumDeclarationSyntax => "enum",
        _ => "class",
    };

    static string AccessOf(SyntaxTokenList mods)
    {
        if (mods.Any(SyntaxKind.PublicKeyword)) return "public";
        if (mods.Any(SyntaxKind.ProtectedKeyword)) return "protected";
        if (mods.Any(SyntaxKind.InternalKeyword)) return "internal";
        return "private";
    }

    static string NamespaceOf(SyntaxNode node)
    {
        for (var n = node.Parent; n is not null; n = n.Parent)
        {
            if (n is BaseNamespaceDeclarationSyntax ns) return ns.Name.ToString();
        }
        return "";
    }

    static int CountLines(string text)
    {
        if (text.Length == 0) return 0;
        var n = 1;
        foreach (var c in text) if (c == '\n') n++;
        return n;
    }

    static string Norm(string p) => p.Replace('\\', '/');

    string Rel(string full) => Norm(Path.GetRelativePath(_root, full));
}
