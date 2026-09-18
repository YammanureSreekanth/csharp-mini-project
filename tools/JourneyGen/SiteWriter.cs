using System.Text.Json;
using System.Text.Json.Serialization;

namespace JourneyGen;

/// <summary>Builds the GitHub Pages dashboard from the analysis results.</summary>
public static class SiteWriter
{
    static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        // Default encoder escapes < > &, so the payload is safe inside <script>.
    };

    public static object BuildPayload(JourneyConfig cfg, CodeAnalyzer analyzer, GitSummary git, Provenance prov, string repoUrl = "")
    {
        var kinds = analyzer.Types
            .GroupBy(t => t.Kind)
            .Select(g => new { kind = Pluralize(g.Key, g.Count()), count = g.Count() })
            .OrderByDescending(k => k.count)
            .ToList();

        var areas = cfg.Concepts
            .GroupBy(c => c.Area)
            .Select(g => new
            {
                name = g.Key,
                done = g.Count(c => c.Done),
                concepts = g.Select(c => new
                {
                    id = c.Id, label = c.Label, done = c.Done, planned = c.Planned,
                    notes = c.Notes, evidence = c.Evidence,
                }).ToList(),
            })
            .ToList();

        return new
        {
            title = cfg.Title,
            repoUrl,
            assignment = cfg.Assignment.Items.Count == 0 ? null : new
            {
                title = cfg.Assignment.Title,
                source = cfg.Assignment.Source,
                assignedOn = cfg.Assignment.AssignedOn,
                reviewOn = cfg.Assignment.ReviewOn,
                note = cfg.Assignment.Note,
                done = cfg.Assignment.Done,
                inProgress = cfg.Assignment.InProgress,
                notStarted = cfg.Assignment.NotStarted,
                total = cfg.Assignment.Items.Count,
                donePercent = cfg.Assignment.DonePercent,
                daysToReview = cfg.Assignment.DaysToReview,
                items = cfg.Assignment.Items.Select(i => new
                {
                    label = i.Label,
                    status = i.Status,
                    notes = i.Notes,
                    detected = i.Detected,
                    evidence = i.Evidence,
                    unevidenced = i.Unevidenced,
                    partiallyEvidenced = i.PartiallyEvidenced,
                    parts = i.Parts.Select(pt => new
                    {
                        label = pt.Label, detected = pt.Detected, evidence = pt.Evidence,
                    }),
                }),
            },
            code = new
            {
                files = analyzer.SourceFiles.Count,
                lines = analyzer.TotalLines,
                types = analyzer.Types.Count,
                projects = analyzer.Projects.Select(p => new { name = p.Name, files = p.Files, lines = p.Lines, types = p.Types }),
                kinds,
            },
            git = new
            {
                totalCommits = git.TotalCommits,
                firstCommit = git.FirstCommit,
                lastCommit = git.LastCommit,
                activeDays = git.ActiveDays,
                weeks = git.Weeks.Select(w => new { week = w.Week, start = w.Start, commits = w.Commits, added = w.Added, removed = w.Removed }),
            },
            concepts = new
            {
                done = cfg.Concepts.Count(c => c.Done),
                total = cfg.Concepts.Count,
                areas,
            },
            provenance = !prov.Available ? null : new
            {
                available = true,
                app = Row(prov.App),
                tooling = Row(prov.Tooling),
                projects = prov.Projects.Select(p => new
                {
                    project = p.Project, isTooling = p.IsTooling,
                    you = p.You, ai = p.Ai, scaffold = p.Scaffold,
                    authored = p.Authored, youPercent = p.YouPercent, aiPercent = p.AiPercent,
                }),
                declared = prov.Declared.Select(d => new
                {
                    sha = d.Sha, subject = d.Subject,
                    source = d.Source == Provenance.Source.Ai ? "AI" : "scaffold",
                }),
            },
            mermaid = MermaidRenderer.ClassDiagram(analyzer.Types, cfg.Diagram),
        };
    }

    public static void Write(string outDir, JourneyConfig cfg, CodeAnalyzer analyzer, GitSummary git, Provenance prov, string repoUrl)
    {
        Directory.CreateDirectory(outDir);

        var payload = BuildPayload(cfg, analyzer, git, prov, repoUrl);
        var json = JsonSerializer.Serialize(payload, Json);

        // Raw data next to the page, so the numbers are consumable without scraping HTML.
        File.WriteAllText(Path.Combine(outDir, "journey.json"),
            JsonSerializer.Serialize(payload, new JsonSerializerOptions(Json) { WriteIndented = true }));

        var templatePath = Path.Combine(AppContext.BaseDirectory, "assets", "index.template.html");
        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"template missing at {templatePath}");

        var repoName = repoUrl.TrimEnd('/');
        repoName = repoName.Length > 0 ? string.Join('/', repoName.Split('/').TakeLast(2)) : "repository";

        var html = File.ReadAllText(templatePath)
            .Replace("__TITLE__", HtmlEscape(cfg.Title))
            .Replace("__TAGLINE__", HtmlEscape(cfg.Tagline))
            .Replace("__REPO_NAME__", HtmlEscape(repoName))
            .Replace("__REPO__", HtmlEscape(repoUrl))
            .Replace("__UPDATED__", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm") + " UTC")
            .Replace("__DATA__", json);

        File.WriteAllText(Path.Combine(outDir, "index.html"), html);

        // Pages otherwise runs the output through Jekyll, which drops files it doesn't like.
        File.WriteAllText(Path.Combine(outDir, ".nojekyll"), "");
    }

    static object Row(ProvenanceStats s) => new
    {
        you = s.You, ai = s.Ai, scaffold = s.Scaffold, total = s.Total,
        authored = s.Authored, youPercent = s.YouPercent, aiPercent = s.AiPercent,
    };

    static string Pluralize(string kind, int count) => count == 1 ? kind : kind switch
    {
        "class" => "classes",
        "struct" => "structs",
        "interface" => "interfaces",
        "enum" => "enums",
        "record" => "records",
        _ => kind + "s",
    };

    static string HtmlEscape(string s) => s
        .Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
}
