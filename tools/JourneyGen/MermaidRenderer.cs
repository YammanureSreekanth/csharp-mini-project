using System.Text;

namespace JourneyGen;

/// <summary>Turns the parsed types into a Mermaid class diagram GitHub renders natively.</summary>
public static class MermaidRenderer
{
    static readonly string[] CollectionWrappers =
        [ "List", "IList", "IReadOnlyList", "ICollection", "IEnumerable", "HashSet", "Queue", "Stack", "IReadOnlyCollection" ];

    public static string ClassDiagram(IReadOnlyList<TypeDecl> allTypes, DiagramOptions opts)
    {
        var scope = allTypes
            .Where(t => opts.NamespaceContains.Length == 0
                     || t.Namespace.Contains(opts.NamespaceContains, StringComparison.OrdinalIgnoreCase))
            .OrderBy(t => t.Kind == "interface" ? 0 : t.Kind == "enum" ? 2 : 1)
            .ThenBy(t => t.Name, StringComparer.Ordinal)
            .ToList();

        if (scope.Count == 0) scope = allTypes.ToList();

        var inScope = scope.ToDictionary(t => t.Name, StringComparer.Ordinal);
        var showMembers = opts.ShowMembers && scope.Count <= 40;

        var sb = new StringBuilder();
        sb.AppendLine("classDiagram");
        sb.AppendLine("    direction LR");

        foreach (var t in scope)
        {
            sb.AppendLine($"    class {t.Name} {{");

            var stereotype = t.Kind switch
            {
                "interface" => "<<interface>>",
                "enum" => "<<enumeration>>",
                "struct" => "<<struct>>",
                "record" => "<<record>>",
                _ => t.IsAbstract ? "<<abstract>>" : t.IsStatic ? "<<static>>" : null,
            };
            if (stereotype is not null) sb.AppendLine($"        {stereotype}");

            if (showMembers)
            {
                foreach (var line in MemberLines(t, opts.MaxMembers))
                    sb.AppendLine($"        {line}");
            }

            sb.AppendLine("    }");
        }

        var edges = new List<string>();
        void Edge(string s) { if (!edges.Contains(s)) edges.Add(s); }

        foreach (var t in scope)
        {
            foreach (var raw in t.BaseTypes)
            {
                var b = CodeAnalyzer.StripGeneric(raw).Trim();
                if (!inScope.TryGetValue(b, out var baseType)) continue;
                Edge(baseType.Kind == "interface"
                    ? $"    {b} <|.. {t.Name} : implements"
                    : $"    {b} <|-- {t.Name}");
            }

            // Associations: a member whose type is another type in the diagram.
            foreach (var m in t.Members)
            {
                if (m.Kind is not ("property" or "field")) continue;
                var (target, many) = Unwrap(m.Type);
                if (target.Length == 0 || target == t.Name) continue;
                if (!inScope.TryGetValue(target, out var targetType)) continue;
                if (targetType.Kind == "enum") continue;  // already legible as the property's type
                if (t.BaseTypes.Any(b => CodeAnalyzer.StripGeneric(b).Trim() == target)) continue;
                Edge($"    {t.Name} {(many ? "\"1\" --> \"*\"" : "-->")} {target} : {m.Name}");
            }
        }

        // Self-referencing types (Category tree, product variations) read best as an explicit note.
        foreach (var t in scope)
        {
            var selfRef = t.Members.FirstOrDefault(m =>
                m.Kind is "property" or "field" && Unwrap(m.Type).target == t.Name);
            if (selfRef is not null)
                Edge($"    {t.Name} --> {t.Name} : {selfRef.Name}");
        }

        foreach (var e in edges) sb.AppendLine(e);
        return sb.ToString().TrimEnd();
    }

    static IEnumerable<string> MemberLines(TypeDecl t, int max)
    {
        if (t.Kind == "enum")
        {
            foreach (var m in t.Members.Take(max)) yield return m.Name;
            if (t.Members.Count > max) yield return $"...{t.Members.Count - max} more";
            yield break;
        }

        var props = t.Members.Where(m => m.Kind is "property" or "field").ToList();
        var methods = t.Members.Where(m => m.Kind == "method").ToList();
        var budget = max;

        foreach (var p in props.Take(budget))
        {
            yield return $"{Vis(p.Accessibility)}{Mermaid(p.Type)} {p.Name}";
            budget--;
        }

        foreach (var m in methods.Take(Math.Max(budget, 2)))
        {
            var suffix = m.IsAbstract ? "*" : "";
            yield return $"{Vis(m.Accessibility)}{m.Name}(){suffix} {Mermaid(m.Type)}";
        }

        var shown = Math.Min(props.Count, max) + Math.Min(methods.Count, Math.Max(budget, 2));
        var total = props.Count + methods.Count;
        if (total > shown) yield return $"...{total - shown} more";
    }

    static string Vis(string accessibility) => accessibility switch
    {
        "public" => "+",
        "protected" => "#",
        "internal" => "~",
        _ => "-",
    };

    /// <summary>Mermaid uses ~ for generics and chokes on ? and whitespace.</summary>
    static string Mermaid(string type)
    {
        if (string.IsNullOrWhiteSpace(type)) return "void";
        return type.Replace("<", "~").Replace(">", "~").Replace("?", "").Replace(" ", "").Trim();
    }

    /// <summary>Strip nullability and one collection wrapper: IReadOnlyList&lt;ProductImage&gt; -> (ProductImage, true).</summary>
    static (string target, bool many) Unwrap(string type)
    {
        var t = (type ?? "").Replace("?", "").Trim();
        if (t.Length == 0) return ("", false);

        var open = t.IndexOf('<');
        if (open > 0 && t.EndsWith(">", StringComparison.Ordinal))
        {
            var outer = t[..open];
            var inner = t[(open + 1)..^1].Trim();
            if (CollectionWrappers.Contains(outer, StringComparer.Ordinal) && !inner.Contains(','))
                return (inner, true);
            return ("", false);
        }

        if (t.EndsWith("[]", StringComparison.Ordinal)) return (t[..^2], true);
        return (t, false);
    }
}
