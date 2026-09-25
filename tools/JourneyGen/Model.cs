namespace JourneyGen;

/// <summary>A type declared somewhere in the repo's own source.</summary>
public sealed class TypeDecl
{
    public string Name = "";
    public string Kind = "class";          // class | interface | struct | record | enum
    public string Namespace = "";
    public string Project = "";            // top-level folder, e.g. Catalog.ConsoleApp
    public string File = "";               // repo-relative path
    public int Lines;
    public bool IsAbstract;
    public bool IsStatic;
    public bool IsSealed;
    public List<string> BaseTypes = new();
    public List<MemberDecl> Members = new();

    public string DisplayName => Name;
}

public sealed class MemberDecl
{
    public string Name = "";
    public string Kind = "property";       // property | method | field | ctor | enumMember
    public string Type = "";
    public string Accessibility = "public";
    public bool IsStatic;
    public bool IsAbstract;
    public bool IsOverride;
}

public sealed class ProjectStats
{
    public string Name = "";
    public string Dir = "";
    public int Files;
    public int Lines;
    public int Types;
}

public sealed class Concept
{
    public string Id = "";
    public string Label = "";
    public string Area = "";
    public string Notes = "";
    public bool Done;
    public bool Planned;                   // explicitly on the roadmap, never auto-detected

    /// <summary>
    /// Set by hand in journey.json: "done", "not-started" or "in-progress". Overrides
    /// what the code says in both directions — a judgement topic can be ticked without
    /// evidence, and something the scaffold happens to contain can be held back until
    /// I have actually learned it.
    /// </summary>
    public string? DeclaredStatus;

    /// <summary>Whether the detectors found it, regardless of what I declared.</summary>
    public bool Detected;
    public List<string> Detectors = new(); // auto:<feature> | regex:<pattern> | path:<fragment> | manual
    public List<string> Evidence = new();  // repo-relative files that prove it
}

public sealed class WeekPoint
{
    public string Week = "";               // ISO-ish yyyy-Www
    public string Start = "";              // yyyy-MM-dd
    public int Commits;
    public int Added;
    public int Removed;
}

public sealed class GitSummary
{
    public int TotalCommits;
    public string FirstCommit = "";
    public string LastCommit = "";
    public int ActiveDays;
    public List<WeekPoint> Weeks = new();
    public Dictionary<string, int> CommitsByProject = new();
}

public sealed class ProvenanceStats
{
    public string Project = "";
    public bool IsTooling;
    public int You;
    public int Ai;
    public int Scaffold;

    public int Total => You + Ai + Scaffold;

    /// <summary>Share of lines you wrote, counting only lines somebody actually authored.</summary>
    public int YouPercent => Authored == 0 ? 0 : (int)Math.Round(100.0 * You / Authored);
    public int AiPercent => Authored == 0 ? 0 : (int)Math.Round(100.0 * Ai / Authored);

    /// <summary>Human + AI lines: template output is excluded, since neither of you wrote it.</summary>
    public int Authored => You + Ai;

    public void Add(Provenance.Source source)
    {
        switch (source)
        {
            case Provenance.Source.You: You++; break;
            case Provenance.Source.Ai: Ai++; break;
            case Provenance.Source.Scaffold: Scaffold++; break;
        }
    }
}

public sealed class ProvenanceConfig
{
    public bool Enabled = true;
    public List<string> AiMarkers = new();
    public List<string> AiCommits = new();
    public List<string> AiPaths = new();
    public List<string> ScaffoldCommits = new();
    public List<string> ScaffoldPaths = new();
    public List<string> BotAuthors = new();
    /// <summary>Extra directories to measure that the code stats skip, e.g. tools/.</summary>
    public List<string> ExtraPaths = new();
}

/// <summary>A learning plan handed down by someone else, with dates and declared status.</summary>
public sealed class Assignment
{
    public string Title = "Learning plan";
    public string Source = "";              // who set it
    public string AssignedOn = "";           // ISO date
    public string ReviewOn = "";             // ISO date of the next check-in
    public string Note = "";
    public List<AssignmentItem> Items = new();

    public int Done => Items.Count(i => i.Status == "done");
    public int InProgress => Items.Count(i => i.Status == "in-progress");
    public int NotStarted => Items.Count(i => i.Status == "not-started");

    public int DonePercent => Items.Count == 0 ? 0 : (int)Math.Round(100.0 * Done / Items.Count);

    /// <summary>Whole days until the review, or null when no date is set or it has passed.</summary>
    public int? DaysToReview
    {
        get
        {
            if (!DateOnly.TryParse(ReviewOn, out var d)) return null;
            var days = d.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;
            return days < 0 ? null : days;
        }
    }
}

public sealed class AssignmentItem
{
    public string Label = "";
    /// <summary>Declared by the learner, not inferred: done | in-progress | not-started.</summary>
    public string Status = "not-started";

    /// <summary>False when the config said nothing, so the code decides instead.</summary>
    public bool StatusDeclared;

    /// <summary>What to show: a declared status wins; otherwise the code speaks.</summary>
    public string EffectiveStatus =>
        StatusDeclared ? Status : (Detected ? "done" : "not-started");

    /// <summary>
    /// False when nothing but `manual` was given — judgement topics like SOLID or
    /// "when not to split into microservices" that no detector can honestly prove.
    /// </summary>
    public bool CodeDetectable =>
        Detectors.Any(d => !d.Equals("manual", StringComparison.OrdinalIgnoreCase)) || Parts.Count > 0;
    public string Notes = "";
    public List<string> Detectors = new();
    public List<AssignmentPart> Parts = new();

    /// <summary>Whether the code actually shows this, independent of the declared status.</summary>
    public bool Detected;
    public List<string> Evidence = new();

    /// <summary>Declared complete but nothing in the code backs it up.</summary>
    public bool Unevidenced => Status == "done" && !Detected && Parts.Count == 0;

    /// <summary>
    /// Not declared complete, yet the code now shows it. The prompt to go update the
    /// status — the plan's status is declared by hand, so without this the page would
    /// quietly understate progress until someone remembered to edit the config.
    /// </summary>
    public bool ReadyToAdvance =>
        Status != "done" && Detected && (Parts.Count == 0 || Parts.All(p => p.Detected));

    /// <summary>Declared complete but only some named parts are present.</summary>
    public bool PartiallyEvidenced =>
        Status == "done" && Parts.Count > 0 && Parts.Any(p => !p.Detected);
}

public sealed class AssignmentPart
{
    public string Label = "";
    public List<string> Detectors = new();
    public bool Detected;
    public List<string> Evidence = new();
}


/// <summary>The long-range curriculum: many areas, mostly not started yet, and that is fine.</summary>
public sealed class Roadmap
{
    public string Title = "Roadmap";
    public string Source = "";
    public string Note = "";
    public List<RoadmapArea> Areas = new();

    public int Total => Areas.Sum(a => a.Items.Count);
    public int Done => Areas.Sum(a => a.Done);
    public int Percent => Total == 0 ? 0 : (int)Math.Round(100.0 * Done / Total);
}

public sealed class RoadmapArea
{
    public string Name = "";
    public string Summary = "";
    public List<AssignmentItem> Items = new();

    public int Done => Items.Count(i => i.EffectiveStatus == "done");
    public int InProgress => Items.Count(i => i.EffectiveStatus == "in-progress");
    public int Percent => Items.Count == 0 ? 0 : (int)Math.Round(100.0 * Done / Items.Count);
}
