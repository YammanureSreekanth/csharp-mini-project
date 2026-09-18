using System.Diagnostics;
using System.Text.RegularExpressions;

namespace JourneyGen;

/// <summary>
/// Attributes every surviving line of code to whoever actually wrote it.
///
/// Line-level, via `git blame`: each line belongs to the commit that last touched it,
/// and each commit is classified once. Edit a scaffold line tomorrow and blame moves
/// that line to your commit on its own — the numbers cannot go stale.
/// </summary>
public sealed class Provenance
{
    public enum Source { You, Ai, Scaffold }

    readonly string _root;
    readonly ProvenanceConfig _cfg;

    /// <summary>sha -> how that commit's lines are counted.</summary>
    readonly Dictionary<string, Source> _commitSource = new(StringComparer.OrdinalIgnoreCase);

    public List<ProvenanceStats> Projects { get; } = new();

    /// <summary>The application projects — the headline number.</summary>
    public ProvenanceStats App { get; } = new() { Project = "Application code" };

    /// <summary>Supporting tooling, reported separately so it cannot skew the headline.</summary>
    public ProvenanceStats Tooling { get; } = new() { Project = "Tooling" };

    public bool Available { get; private set; }

    /// <summary>Commits classified as something other than human work, for the audit trail.</summary>
    public List<(string Sha, string Subject, Source Source)> Declared { get; } = new();

    /// <summary>Shas that actually contributed surviving lines, so the audit list omits no-ops.</summary>
    readonly Dictionary<string, int> _linesBySha = new(StringComparer.OrdinalIgnoreCase);

    static readonly Regex BlameHeader = new(@"^([0-9a-f]{40}) \d+ \d+", RegexOptions.Compiled);

    public Provenance(string root, ProvenanceConfig cfg)
    {
        _root = root;
        _cfg = cfg;
    }

    public void Run(IEnumerable<string> files)
    {
        if (!ClassifyCommits()) return;

        var perProject = new Dictionary<string, ProvenanceStats>(StringComparer.OrdinalIgnoreCase);

        foreach (var rel in files)
        {
            var project = rel.Split('/')[0];
            var isTooling = _cfg.ExtraPaths.Any(t =>
                rel.StartsWith(t.TrimEnd('/') + "/", StringComparison.OrdinalIgnoreCase));

            if (!perProject.TryGetValue(project, out var stats))
                perProject[project] = stats = new ProvenanceStats { Project = project, IsTooling = isTooling };

            // A path declared wholesale wins over per-commit classification.
            var forced = ForcedForPath(rel);

            foreach (var (sha, content) in Blame(rel))
            {
                if (content.Trim().Length == 0) continue;   // blank lines are nobody's work

                var source = forced ?? (_commitSource.TryGetValue(sha, out var s) ? s : Source.You);
                stats.Add(source);
                (isTooling ? Tooling : App).Add(source);
                _linesBySha[sha] = _linesBySha.GetValueOrDefault(sha) + 1;
            }
        }

        Projects.AddRange(perProject.Values
            .OrderBy(p => p.IsTooling)
            .ThenByDescending(p => p.Total));

        // Drop declared commits whose lines no longer survive — they only add noise.
        Declared.RemoveAll(d => !_linesBySha.Keys.Any(
            sha => sha.StartsWith(d.Sha, StringComparison.OrdinalIgnoreCase)));

        Available = App.Total + Tooling.Total > 0;
    }

    Source? ForcedForPath(string rel)
    {
        if (_cfg.AiPaths.Any(p => rel.StartsWith(p, StringComparison.OrdinalIgnoreCase))) return Source.Ai;
        if (_cfg.ScaffoldPaths.Any(p => rel.StartsWith(p, StringComparison.OrdinalIgnoreCase))) return Source.Scaffold;
        return null;
    }

    /// <summary>
    /// Read every commit once and decide what it counts as. Precedence:
    /// explicit sha lists, then an AI trailer in the message, then bot authors, else human.
    /// </summary>
    bool ClassifyCommits()
    {
        // %x00 separates fields, %x01 separates commits — neither can appear in a commit message.
        var raw = Run($"log --no-merges --pretty=format:%H%x00%an%x00%s%x00%b%x01");
        if (raw.Length == 0) return false;

        foreach (var chunk in raw.Split('\x01', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = chunk.TrimStart('\n').Split('\x00');
            if (parts.Length < 4) continue;
            var (sha, author, subject, body) = (parts[0].Trim(), parts[1], parts[2], parts[3]);
            if (sha.Length < 40) continue;

            var source = Classify(sha, author, subject + "\n" + body);
            _commitSource[sha] = source;
            if (source != Source.You) Declared.Add((sha[..7], subject, source));
        }

        return _commitSource.Count > 0;
    }

    Source Classify(string sha, string author, string message)
    {
        bool Listed(IEnumerable<string> shas) =>
            shas.Any(s => s.Length >= 7 && sha.StartsWith(s.Trim(), StringComparison.OrdinalIgnoreCase));

        if (Listed(_cfg.ScaffoldCommits)) return Source.Scaffold;
        if (Listed(_cfg.AiCommits)) return Source.Ai;

        // The going-forward mechanism: a trailer in the commit message.
        if (_cfg.AiMarkers.Any(m => message.Contains(m, StringComparison.OrdinalIgnoreCase))) return Source.Ai;

        if (_cfg.BotAuthors.Any(b => author.Contains(b, StringComparison.OrdinalIgnoreCase))) return Source.Scaffold;

        return Source.You;
    }

    IEnumerable<(string Sha, string Content)> Blame(string rel)
    {
        // -w ignores whitespace-only changes and -M follows lines moved within the file,
        // so a reformat or a code move does not silently transfer authorship.
        var raw = Run($"blame --line-porcelain -w -M -- \"{rel}\"");
        if (raw.Length == 0) yield break;

        var sha = "";
        foreach (var line in raw.Split('\n'))
        {
            var m = BlameHeader.Match(line);
            if (m.Success) { sha = m.Groups[1].Value; continue; }
            if (line.StartsWith('\t')) yield return (sha, line[1..]);
        }
    }

    string Run(string args)
    {
        try
        {
            var psi = new ProcessStartInfo("git", args)
            {
                WorkingDirectory = _root,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            using var p = Process.Start(psi);
            if (p is null) return "";
            var output = p.StandardOutput.ReadToEnd();
            p.StandardError.ReadToEnd();
            p.WaitForExit();
            return p.ExitCode == 0 ? output : "";
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"journeygen: provenance unavailable ({ex.Message})");
            return "";
        }
    }
}
