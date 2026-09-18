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
    public Assignment Assignment = new();
    public Roadmap Roadmap = new();
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

        if (root.TryGetProperty("assignment", out var asg))
        {
            var a = cfg.Assignment;
            if (asg.TryGetProperty("title", out var at)) a.Title = at.GetString() ?? a.Title;
            if (asg.TryGetProperty("source", out var asrc)) a.Source = asrc.GetString() ?? "";
            if (asg.TryGetProperty("assignedOn", out var ao)) a.AssignedOn = ao.GetString() ?? "";
            if (asg.TryGetProperty("reviewOn", out var ro)) a.ReviewOn = ro.GetString() ?? "";
            if (asg.TryGetProperty("note", out var an)) a.Note = an.GetString() ?? "";

            if (asg.TryGetProperty("items", out var items) && items.ValueKind == JsonValueKind.Array)
            {
                foreach (var it in items.EnumerateArray())
                {
                    var item = new AssignmentItem
                    {
                        Label = it.TryGetProperty("label", out var il) ? il.GetString() ?? "" : "",
                        Status = it.TryGetProperty("status", out var ist) ? (ist.GetString() ?? "not-started") : "not-started",
                        StatusDeclared = it.TryGetProperty("status", out _),
                        Notes = it.TryGetProperty("notes", out var inn) ? inn.GetString() ?? "" : "",
                    };
                    item.Detectors.AddRange(ReadDetectors(it));

                    if (it.TryGetProperty("parts", out var parts) && parts.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var pt in parts.EnumerateArray())
                        {
                            var part = new AssignmentPart
                            {
                                Label = pt.TryGetProperty("label", out var pl) ? pl.GetString() ?? "" : "",
                            };
                            part.Detectors.AddRange(ReadDetectors(pt));
                            item.Parts.Add(part);
                        }
                    }

                    a.Items.Add(item);
                }
            }
        }

        if (root.TryGetProperty("roadmap", out var rm))
        {
            var r = cfg.Roadmap;
            if (rm.TryGetProperty("title", out var rt)) r.Title = rt.GetString() ?? r.Title;
            if (rm.TryGetProperty("source", out var rs)) r.Source = rs.GetString() ?? "";
            if (rm.TryGetProperty("note", out var rn)) r.Note = rn.GetString() ?? "";

            if (rm.TryGetProperty("areas", out var rAreas) && rAreas.ValueKind == JsonValueKind.Array)
            {
                foreach (var ra in rAreas.EnumerateArray())
                {
                    var area = new RoadmapArea
                    {
                        Name = ra.TryGetProperty("name", out var rnm) ? rnm.GetString() ?? "" : "",
                        Summary = ra.TryGetProperty("summary", out var rsm) ? rsm.GetString() ?? "" : "",
                    };
                    if (ra.TryGetProperty("items", out var rItems) && rItems.ValueKind == JsonValueKind.Array)
                        foreach (var ri in rItems.EnumerateArray())
                            area.Items.Add(ReadItem(ri));
                    r.Areas.Add(area);
                }
            }
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

    static AssignmentItem ReadItem(JsonElement it)
    {
        var item = new AssignmentItem
        {
            Label = it.TryGetProperty("label", out var il) ? il.GetString() ?? "" : "",
            Status = it.TryGetProperty("status", out var ist) ? (ist.GetString() ?? "not-started") : "not-started",
            StatusDeclared = it.TryGetProperty("status", out _),
            Notes = it.TryGetProperty("notes", out var inn) ? inn.GetString() ?? "" : "",
        };
        item.Detectors.AddRange(ReadDetectors(it));

        if (it.TryGetProperty("parts", out var parts) && parts.ValueKind == JsonValueKind.Array)
        {
            foreach (var pt in parts.EnumerateArray())
            {
                var part = new AssignmentPart
                {
                    Label = pt.TryGetProperty("label", out var pl) ? pl.GetString() ?? "" : "",
                };
                part.Detectors.AddRange(ReadDetectors(pt));
                item.Parts.Add(part);
            }
        }
        return item;
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
    /// <summary>
    /// Decide, per concept, whether the repo actually demonstrates it yet, then do the
    /// same for the assignment items. A concept with no detectors (or `manual`) stays
    /// on the roadmap; an assignment item keeps its declared status either way, and only
    /// gains or loses its supporting evidence.
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
            if (concept.Detectors.Count == 0 ||
                concept.Detectors.All(d => d.Equals("manual", StringComparison.OrdinalIgnoreCase)))
            {
                concept.Planned = true;
                continue;
            }

            var (matched, evidence) = RunDetectors(concept.Detectors, analyzer, root, fileText);
            concept.Done = matched;
            concept.Evidence = evidence;
        }

        foreach (var item in Assignment.Items)
        {
            var (matched, evidence) = RunDetectors(item.Detectors, analyzer, root, fileText);
            item.Detected = matched;
            item.Evidence = evidence;

            foreach (var part in item.Parts)
            {
                var (pm, pe) = RunDetectors(part.Detectors, analyzer, root, fileText);
                part.Detected = pm;
                part.Evidence = pe;
                if (pm)
                {
                    item.Detected = true;
                    foreach (var f in pe) if (!item.Evidence.Contains(f)) item.Evidence.Add(f);
                }
            }

            item.Evidence = item.Evidence.Take(6).ToList();
        }

        foreach (var item in Roadmap.Areas.SelectMany(a => a.Items))
        {
            var (matched, evidence) = RunDetectors(item.Detectors, analyzer, root, fileText);
            item.Detected = matched;
            item.Evidence = evidence.Take(4).ToList();
        }
    }

    /// <summary>
    /// Run a detector list against the repo. Any single match counts.
    /// `auto:` consults the Roslyn walker, `regex:` the text of every .cs file,
    /// `path:` every file path; `manual` never matches.
    /// </summary>
    static (bool matched, List<string> evidence) RunDetectors(
        IEnumerable<string> detectors, CodeAnalyzer analyzer, string root,
        Lazy<Dictionary<string, string>> fileText)
    {
        var matched = false;
        var evidence = new List<string>();

        foreach (var detector in detectors)
        {
            var (scheme, arg) = Split(detector);
            switch (scheme)
            {
                case "auto":
                    if (analyzer.Features.TryGetValue(arg, out var hits))
                    {
                        matched = true;
                        evidence.AddRange(hits);
                    }
                    break;

                case "regex":
                    var rx = new Regex(arg, RegexOptions.Multiline | RegexOptions.CultureInvariant);
                    foreach (var (file, text) in fileText.Value)
                        if (rx.IsMatch(text)) { matched = true; evidence.Add(file); }
                    break;

                case "grep":
                    var grx = new Regex(arg, RegexOptions.Multiline | RegexOptions.CultureInvariant);
                    foreach (var (rel, text) in AllTextFiles(root).Value)
                        if (grx.IsMatch(text)) { matched = true; evidence.Add(rel); }
                    break;

                case "path":
                    foreach (var f in analyzer.SourceFiles)
                        if (f.Contains(arg, StringComparison.OrdinalIgnoreCase)) { matched = true; evidence.Add(f); }
                    foreach (var f in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
                                               .OrderBy(x => x, StringComparer.Ordinal))
                    {
                        var rel = Path.GetRelativePath(root, f).Replace('\\', '/');
                        if (IsExcluded(rel)) continue;
                        if (rel.Contains(arg, StringComparison.OrdinalIgnoreCase)) { matched = true; evidence.Add(rel); }
                    }
                    break;
            }
        }

        // Ordered so the same commit renders identically on any machine.
        return (matched, evidence.Distinct(StringComparer.Ordinal)
                                 .OrderBy(e => e, StringComparer.Ordinal)
                                 .Take(6).ToList());
    }

    /// <summary>
    /// Paths no detector may look at. `artifacts/` matters most: it is this tool's own
    /// output directory, so leaving it in made results depend on whether the generator
    /// had been run before — the same commit produced different output on a machine
    /// with a previous build than on a fresh CI checkout.
    /// </summary>
    static bool IsExcluded(string rel) =>
        rel.StartsWith(".git/", StringComparison.Ordinal) ||
        rel.StartsWith("artifacts/", StringComparison.OrdinalIgnoreCase) ||
        rel.Contains("/bin/", StringComparison.Ordinal) ||
        rel.Contains("/obj/", StringComparison.Ordinal) ||
        rel.Contains("wwwroot/lib/", StringComparison.Ordinal);

    static readonly string[] TextExtensions =
    [
        ".cs", ".csproj", ".slnx", ".sln", ".json", ".yml", ".yaml", ".bicep", ".tf",
        ".sql", ".cshtml", ".razor", ".props", ".targets", ".config", ".xml",
        ".http", ".ps1", ".sh", ".dockerfile",
    ];

    static Lazy<SortedDictionary<string, string>>? _allText;

    /// <summary>
    /// Every text file in the repo, read once. `grep:` needs this because a lot of
    /// what a roadmap asks about lives outside .cs — a PackageReference, a bicep
    /// template, a workflow step, a connection string.
    /// </summary>
    static Lazy<SortedDictionary<string, string>> AllTextFiles(string root) =>
        _allText ??= new Lazy<SortedDictionary<string, string>>(() =>
        {
            // Sorted, not filesystem order: APFS and ext4 enumerate differently, so an
            // unsorted walk made the generated evidence differ between a mac and a Linux
            // CI runner for the same commit.
            var map = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var full in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
                                          .OrderBy(f => f, StringComparer.Ordinal))
            {
                var rel = Path.GetRelativePath(root, full).Replace('\\', '/');
                if (IsExcluded(rel)) continue;

                // Never let the search find the thing that describes the search.
                // journey.json holds every detector pattern, the generated README
                // echoes every label back, and tools/ mentions half of .NET by name —
                // searching any of them makes every detector prove itself.
                if (rel.Equals("journey.json", StringComparison.OrdinalIgnoreCase) ||
                    rel.StartsWith("tools/", StringComparison.OrdinalIgnoreCase) ||
                    rel.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ||
                    rel.EndsWith(".MD", StringComparison.Ordinal)) continue;

                var ext = Path.GetExtension(rel);
                var named = Path.GetFileName(rel);
                if (ext.Length > 0 && !TextExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase)
                    && !named.Equals("Dockerfile", StringComparison.OrdinalIgnoreCase)) continue;

                try
                {
                    var info = new FileInfo(full);
                    if (info.Length > 2_000_000) continue;   // don't slurp anything huge
                    map[rel] = File.ReadAllText(full);
                }
                catch { /* unreadable file is simply not evidence */ }
            }
            return map;
        });

    static (string scheme, string arg) Split(string detector)
    {
        var i = detector.IndexOf(':');
        return i < 0 ? ("auto", detector) : (detector[..i].Trim().ToLowerInvariant(), detector[(i + 1)..].Trim());
    }
}
