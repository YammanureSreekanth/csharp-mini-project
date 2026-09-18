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
