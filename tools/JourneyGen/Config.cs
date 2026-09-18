using System.Text.Json;
using System.Text.RegularExpressions;

namespace JourneyGen;

public sealed class DiagramOptions
{
    public string NamespaceContains = "Domain";
    public bool ShowMembers = true;
    public int MaxMembers = 10;
}

public sealed class JourneyConfig
{
    public string Title = "My .NET Journey";
    public string Tagline = "";
    public DiagramOptions Diagram = new();
    public ProvenanceConfig Provenance = new();
    public List<Concept> Concepts = new();

    static readonly JsonDocumentOptions ReadOpts = new()
    {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public static JourneyConfig Load(string path)
    {
        var cfg = new JourneyConfig();
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"journeygen: no config at {path}, using defaults");
            return cfg;
        }

        using var doc = JsonDocument.Parse(File.ReadAllText(path), ReadOpts);
        var root = doc.RootElement;

        if (root.TryGetProperty("title", out var t)) cfg.Title = t.GetString() ?? cfg.Title;
        if (root.TryGetProperty("tagline", out var tl)) cfg.Tagline = tl.GetString() ?? "";

        if (root.TryGetProperty("diagram", out var d))
        {
            if (d.TryGetProperty("namespaceContains", out var nc)) cfg.Diagram.NamespaceContains = nc.GetString() ?? "";
            if (d.TryGetProperty("showMembers", out var sm)) cfg.Diagram.ShowMembers = sm.GetBoolean();
            if (d.TryGetProperty("maxMembers", out var mm)) cfg.Diagram.MaxMembers = mm.GetInt32();
        }

        if (root.TryGetProperty("provenance", out var pv))
        {
            var c = cfg.Provenance;
            if (pv.TryGetProperty("enabled", out var en)) c.Enabled = en.GetBoolean();
            ReadStrings(pv, "aiMarkers", c.AiMarkers);
            ReadStrings(pv, "aiCommits", c.AiCommits);
            ReadStrings(pv, "aiPaths", c.AiPaths);
            ReadStrings(pv, "scaffoldCommits", c.ScaffoldCommits);
            ReadStrings(pv, "scaffoldPaths", c.ScaffoldPaths);
            ReadStrings(pv, "botAuthors", c.BotAuthors);
            ReadStrings(pv, "extraPaths", c.ExtraPaths);
        }

        if (root.TryGetProperty("areas", out var areas))
        {
            foreach (var area in areas.EnumerateArray())
            {
                var areaName = area.TryGetProperty("name", out var an) ? an.GetString() ?? "" : "";
                if (!area.TryGetProperty("concepts", out var concepts)) continue;

                foreach (var c in concepts.EnumerateArray())
                {
                    var concept = new Concept
                    {
                        Area = areaName,
                        Id = c.TryGetProperty("id", out var id) ? id.GetString() ?? "" : "",
                        Label = c.TryGetProperty("label", out var lb) ? lb.GetString() ?? "" : "",
                        Notes = c.TryGetProperty("notes", out var nt) ? nt.GetString() ?? "" : "",
                    };
                    if (concept.Label.Length == 0) concept.Label = concept.Id;
                    concept.Detectors.AddRange(ReadDetectors(c));
                    cfg.Concepts.Add(concept);
                }
            }
        }

        return cfg;
    }

    static void ReadStrings(JsonElement parent, string name, List<string> into)
    {
        if (!parent.TryGetProperty(name, out var arr) || arr.ValueKind != JsonValueKind.Array) return;
        foreach (var e in arr.EnumerateArray())
            if (e.ValueKind == JsonValueKind.String && e.GetString() is { Length: > 0 } v)
                into.Add(v);
    }

    static IEnumerable<string> ReadDetectors(JsonElement c)
    {
        if (!c.TryGetProperty("detect", out var det)) yield break;
        if (det.ValueKind == JsonValueKind.String) { yield return det.GetString() ?? ""; yield break; }
        if (det.ValueKind != JsonValueKind.Array) yield break;
        foreach (var e in det.EnumerateArray())
            if (e.ValueKind == JsonValueKind.String) yield return e.GetString() ?? "";
    }

    /// <summary>
    /// Decide, per concept, whether the repo actually demonstrates it yet.
    /// A concept with no detectors (or `manual`) stays on the roadmap.
    /// </summary>
    public void Evaluate(CodeAnalyzer analyzer, string root)
    {
        var fileText = new Lazy<Dictionary<string, string>>(() =>
            analyzer.SourceFiles.ToDictionary(
                f => f,
                f => File.ReadAllText(Path.Combine(root, f)),
                StringComparer.Ordinal));

        foreach (var concept in Concepts)
        {
            if (concept.Detectors.Count == 0 || concept.Detectors.All(d => d.Equals("manual", StringComparison.OrdinalIgnoreCase)))
            {
                concept.Planned = true;
                continue;
            }

            foreach (var detector in concept.Detectors)
            {
                var (scheme, arg) = Split(detector);
                switch (scheme)
                {
                    case "auto":
                        if (analyzer.Features.TryGetValue(arg, out var hits))
                        {
                            concept.Done = true;
                            concept.Evidence.AddRange(hits);
                        }
                        break;

                    case "regex":
                        var rx = new Regex(arg, RegexOptions.Multiline | RegexOptions.CultureInvariant);
                        foreach (var (file, text) in fileText.Value)
                            if (rx.IsMatch(text)) { concept.Done = true; concept.Evidence.Add(file); }
                        break;

                    case "path":
                        foreach (var f in analyzer.SourceFiles)
                            if (f.Contains(arg, StringComparison.OrdinalIgnoreCase)) { concept.Done = true; concept.Evidence.Add(f); }
                        // Also allow matching non-.cs assets (Views, wwwroot, scripts).
                        foreach (var f in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
                        {
                            var rel = Path.GetRelativePath(root, f).Replace('\\', '/');
                            if (rel.StartsWith(".git/", StringComparison.Ordinal) || rel.Contains("/obj/") || rel.Contains("/bin/")) continue;
                            if (rel.Contains(arg, StringComparison.OrdinalIgnoreCase)) { concept.Done = true; concept.Evidence.Add(rel); }
                        }
                        break;
                }
            }

            concept.Evidence = concept.Evidence.Distinct(StringComparer.Ordinal).Take(6).ToList();
        }
    }

    static (string scheme, string arg) Split(string detector)
    {
        var i = detector.IndexOf(':');
        return i < 0 ? ("auto", detector) : (detector[..i].Trim().ToLowerInvariant(), detector[(i + 1)..].Trim());
    }
}
