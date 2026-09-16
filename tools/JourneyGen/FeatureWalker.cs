using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JourneyGen;

/// <summary>
/// Walks one file's syntax tree and records which language/framework features it uses.
/// Deliberately conservative: a feature is only reported when the syntax for it is
/// unambiguously present, so the journey checklist never ticks something you haven't written.
/// </summary>
public sealed class FeatureWalker : CSharpSyntaxWalker
{
    public readonly HashSet<string> Found = new(StringComparer.OrdinalIgnoreCase);

    static readonly HashSet<string> LinqMethods = new(StringComparer.Ordinal)
    {
        "Where", "Select", "SelectMany", "Any", "All", "First", "FirstOrDefault",
        "Single", "SingleOrDefault", "Last", "LastOrDefault", "OrderBy", "OrderByDescending",
        "ThenBy", "GroupBy", "ToList", "ToArray", "ToDictionary", "ToHashSet", "Sum",
        "Average", "Aggregate", "Distinct", "Skip", "Take", "Zip", "Join", "Except", "Union",
    };

    static readonly HashSet<string> DiMethods = new(StringComparer.Ordinal)
    {
        "AddScoped", "AddTransient", "AddSingleton", "AddDbContext", "AddHttpClient",
        "AddControllers", "AddControllersWithViews", "AddRazorPages", "AddEndpointsApiExplorer",
    };

    static readonly HashSet<string> MinimalApiMethods = new(StringComparer.Ordinal)
        { "MapGet", "MapPost", "MapPut", "MapDelete", "MapPatch" };

    static readonly HashSet<string> AdoTypes = new(StringComparer.Ordinal)
    {
        "SqlConnection", "SqlCommand", "SqlDataReader", "SqlParameter",
        "MySqlConnection", "MySqlCommand", "DbConnection", "DbCommand",
    };

    void Mark(string id) => Found.Add(id);

    public override void VisitCompilationUnit(CompilationUnitSyntax node)
    {
        if (node.Members.OfType<GlobalStatementSyntax>().Any()) Mark("top-level-statements");
        if (node.Members.OfType<FileScopedNamespaceDeclarationSyntax>().Any()) Mark("file-scoped-namespaces");
        if (node.Members.OfType<NamespaceDeclarationSyntax>().Any()) Mark("namespaces");
        base.VisitCompilationUnit(node);
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        Mark("classes");
        if (node.TypeParameterList is not null) Mark("generic-types");
        if (node.ParameterList is not null) Mark("primary-constructors");
        if (node.Modifiers.Any(SyntaxKind.PartialKeyword)) Mark("partial-types");
        if (node.Modifiers.Any(SyntaxKind.StaticKeyword) && HasExtensionMethod(node)) Mark("extension-methods");
        base.VisitClassDeclaration(node);
    }

    public override void VisitStructDeclaration(StructDeclarationSyntax node)
    {
        Mark("structs");
        if (node.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)) Mark("readonly-structs");
        base.VisitStructDeclaration(node);
    }

    public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        Mark("interfaces");
        if (node.TypeParameterList is not null) Mark("generic-types");
        // A body on an interface member means a default implementation.
        if (node.Members.OfType<MethodDeclarationSyntax>().Any(m => m.Body is not null || m.ExpressionBody is not null))
            Mark("default-interface-methods");
        base.VisitInterfaceDeclaration(node);
    }

    public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
    {
        Mark("records");
        if (node.ParameterList is not null) Mark("positional-records");
        base.VisitRecordDeclaration(node);
    }

    public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
    {
        Mark("enums");
        base.VisitEnumDeclaration(node);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        if (node.Modifiers.Any(SyntaxKind.AsyncKeyword)) Mark("async-await");
        if (node.Modifiers.Any(SyntaxKind.OverrideKeyword)) Mark("method-overriding");
        if (node.Modifiers.Any(SyntaxKind.VirtualKeyword)) Mark("virtual-members");
        if (node.Modifiers.Any(SyntaxKind.AbstractKeyword)) Mark("abstract-members");
        if (node.TypeParameterList is not null) Mark("generic-methods");
        if (node.ExpressionBody is not null) Mark("expression-bodied-members");
        if (node.ParameterList.Parameters.Any(p => p.Modifiers.Any(SyntaxKind.ThisKeyword))) Mark("extension-methods");
        if (node.ParameterList.Parameters.Any(p => p.Modifiers.Any(SyntaxKind.OutKeyword))) Mark("out-parameters");
        if (node.ParameterList.Parameters.Any(p => p.Modifiers.Any(SyntaxKind.ParamsKeyword))) Mark("params-arrays");
        if (node.ParameterList.Parameters.Any(p => p.Default is not null)) Mark("optional-parameters");
        base.VisitMethodDeclaration(node);
    }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        Mark("properties");
        var accessors = node.AccessorList?.Accessors ?? default;
        foreach (var a in accessors)
        {
            if (a.IsKind(SyntaxKind.InitAccessorDeclaration)) Mark("init-only-properties");
            if (a.Body is not null || a.ExpressionBody is not null) Mark("custom-accessors");
        }
        if (node.Modifiers.Any(SyntaxKind.RequiredKeyword)) Mark("required-members");
        if (node.ExpressionBody is not null) Mark("expression-bodied-members");
        base.VisitPropertyDeclaration(node);
    }

    public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
    {
        Mark("indexers");
        base.VisitIndexerDeclaration(node);
    }

    public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
    {
        Mark("operator-overloading");
        base.VisitOperatorDeclaration(node);
    }

    public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
    {
        Mark("delegates");
        base.VisitDelegateDeclaration(node);
    }

    public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
    {
        Mark("events");
        base.VisitEventFieldDeclaration(node);
    }

    public override void VisitAttributeList(AttributeListSyntax node)
    {
        Mark("attribute-usage");
        base.VisitAttributeList(node);
    }

    public override void VisitAwaitExpression(AwaitExpressionSyntax node)
    {
        Mark("async-await");
        base.VisitAwaitExpression(node);
    }

    public override void VisitQueryExpression(QueryExpressionSyntax node)
    {
        Mark("linq-query-syntax");
        Mark("linq");
        base.VisitQueryExpression(node);
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        var name = node.Expression switch
        {
            MemberAccessExpressionSyntax m => m.Name.Identifier.Text,
            IdentifierNameSyntax i => i.Identifier.Text,
            GenericNameSyntax g => g.Identifier.Text,
            MemberBindingExpressionSyntax b => b.Name.Identifier.Text,
            _ => "",
        };

        if (LinqMethods.Contains(name)) { Mark("linq"); Mark("linq-method-syntax"); }
        if (DiMethods.Contains(name)) Mark("dependency-injection");
        if (MinimalApiMethods.Contains(name)) Mark("minimal-apis");
        if (name.StartsWith("Use", StringComparison.Ordinal) && name.Length > 3 && char.IsUpper(name[3]))
            Mark("middleware-pipeline");
        if (name is "MapControllerRoute" or "MapControllers" or "MapDefaultControllerRoute") Mark("mvc-routing");
        if (name is "ExecuteReader" or "ExecuteNonQuery" or "ExecuteScalar" or "OpenAsync" or "Open") Mark("ado-net");
        if (name is "WriteLine" or "ReadLine" && node.Expression.ToString().StartsWith("Console", StringComparison.Ordinal))
            Mark("console-io");

        base.VisitInvocationExpression(node);
    }

    public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
    {
        if (AdoTypes.Contains(CodeAnalyzer.StripGeneric(node.Type.ToString()))) Mark("ado-net");
        if (node.Initializer is not null) Mark("object-initializers");
        base.VisitObjectCreationExpression(node);
    }

    public override void VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node)
    {
        Mark("target-typed-new");
        base.VisitImplicitObjectCreationExpression(node);
    }

    public override void VisitCollectionExpression(CollectionExpressionSyntax node)
    {
        Mark("collection-expressions");
        base.VisitCollectionExpression(node);
    }

    public override void VisitIsPatternExpression(IsPatternExpressionSyntax node)
    {
        Mark("pattern-matching");
        base.VisitIsPatternExpression(node);
    }

    public override void VisitSwitchExpression(SwitchExpressionSyntax node)
    {
        Mark("switch-expressions");
        Mark("pattern-matching");
        base.VisitSwitchExpression(node);
    }

    public override void VisitSwitchStatement(SwitchStatementSyntax node)
    {
        Mark("switch-statements");
        base.VisitSwitchStatement(node);
    }

    public override void VisitTryStatement(TryStatementSyntax node)
    {
        Mark("exception-handling");
        if (node.Catches.Any(c => c.Filter is not null)) Mark("exception-filters");
        if (node.Finally is not null) Mark("try-finally");
        base.VisitTryStatement(node);
    }

    public override void VisitThrowStatement(ThrowStatementSyntax node)
    {
        Mark("throwing-exceptions");
        base.VisitThrowStatement(node);
    }

    public override void VisitUsingStatement(UsingStatementSyntax node)
    {
        Mark("using-idisposable");
        base.VisitUsingStatement(node);
    }

    public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
    {
        if (node.UsingKeyword != default) Mark("using-idisposable");
        if (node.Declaration.Type is IdentifierNameSyntax { Identifier.Text: "var" }) Mark("implicit-typing");
        base.VisitLocalDeclarationStatement(node);
    }

    public override void VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
    {
        Mark("string-interpolation");
        base.VisitInterpolatedStringExpression(node);
    }

    public override void VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
    {
        Mark("null-conditional-operators");
        base.VisitConditionalAccessExpression(node);
    }

    public override void VisitBinaryExpression(BinaryExpressionSyntax node)
    {
        if (node.IsKind(SyntaxKind.CoalesceExpression)) Mark("null-coalescing");
        if (node.IsKind(SyntaxKind.AsExpression) || node.IsKind(SyntaxKind.IsExpression)) Mark("type-checking-casts");
        base.VisitBinaryExpression(node);
    }

    public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
    {
        if (node.IsKind(SyntaxKind.CoalesceAssignmentExpression)) Mark("null-coalescing");
        base.VisitAssignmentExpression(node);
    }

    public override void VisitNullableType(NullableTypeSyntax node)
    {
        Mark("nullable-types");
        base.VisitNullableType(node);
    }

    public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
    {
        Mark("lambdas");
        base.VisitSimpleLambdaExpression(node);
    }

    public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
    {
        Mark("lambdas");
        base.VisitParenthesizedLambdaExpression(node);
    }

    public override void VisitYieldStatement(YieldStatementSyntax node)
    {
        Mark("iterators-yield");
        base.VisitYieldStatement(node);
    }

    public override void VisitForEachStatement(ForEachStatementSyntax node)
    {
        Mark("foreach-iteration");
        base.VisitForEachStatement(node);
    }

    public override void VisitTupleType(TupleTypeSyntax node)
    {
        Mark("tuples");
        base.VisitTupleType(node);
    }

    public override void VisitTupleExpression(TupleExpressionSyntax node)
    {
        Mark("tuples");
        base.VisitTupleExpression(node);
    }

    public override void VisitGenericName(GenericNameSyntax node)
    {
        Mark("generics");
        var name = node.Identifier.Text;
        if (name is "List" or "Dictionary" or "HashSet" or "IEnumerable" or "IReadOnlyList"
                 or "IList" or "ICollection" or "Queue" or "Stack" or "IReadOnlyDictionary")
            Mark("collections");
        if (name is "Task" or "ValueTask") Mark("task-based-async");
        base.VisitGenericName(node);
    }

    static bool HasExtensionMethod(ClassDeclarationSyntax node) =>
        node.Members.OfType<MethodDeclarationSyntax>()
            .Any(m => m.ParameterList.Parameters.Any(p => p.Modifiers.Any(SyntaxKind.ThisKeyword)));
}
