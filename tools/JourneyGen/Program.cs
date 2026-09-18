using JourneyGen;

// journeygen [--root <dir>] [--config <file>] [--out <dir>] [--readme <file>]
//            [--repo-url <url>] [--site-url <url>] [--check]
//
// Reads the repo's own C# source plus its git history and regenerates:
//   * the JOURNEY block inside the README
//   * a static dashboard (index.html + journey.json) for GitHub Pages
// --check makes no writes and exits 1 if the README block is stale (useful in CI).

var root = Arg("--root") ?? Directory.GetCurrentDirectory();
root = Path.GetFullPath(root);

var configPath = Arg("--config") ?? Path.Combine(root, "journey.json");
var outDir = Arg("--out") ?? Path.Combine(root, "artifacts", "site");
var readmePath = Arg("--readme") ?? FindReadme(root);
var repoUrl = Arg("--repo-url") ?? GuessRepoUrl(root);
var siteUrl = Arg("--site-url") ?? GuessPagesUrl(repoUrl);
var check = Flag("--check");

Console.WriteLine($"journeygen: analysing {root}");

var config = JourneyConfig.Load(configPath);

var analyzer = new CodeAnalyzer(root);
analyzer.Run();
Console.WriteLine($"journeygen: {analyzer.SourceFiles.Count} files, {analyzer.TotalLines} lines, " +
                  $"{analyzer.Types.Count} types, {analyzer.Features.Count} features detected");

var git = GitHistory.Read(root);
Console.WriteLine($"journeygen: {git.TotalCommits} commits across {git.Weeks.Count} weeks");

var provenance = new Provenance(root, config.Provenance);
if (config.Provenance.Enabled)
{
    // The code stats skip tools/, but hiding AI-written tooling from an AI-share
    // number would defeat the point, so measure the extra paths too.
    var extra = config.Provenance.ExtraPaths
        .Select(p => Path.Combine(root, p))
        .Where(Directory.Exists)
        .SelectMany(dir => Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories))
        .Select(f => Path.GetRelativePath(root, f).Replace('\\', '/'))
        .Where(f => !f.Contains("/bin/") && !f.Contains("/obj/"));

    provenance.Run(analyzer.SourceFiles.Concat(extra).Distinct(StringComparer.Ordinal));

    if (provenance.Available)
        Console.WriteLine($"journeygen: app code {provenance.App.YouPercent}% yours " +
                          $"({provenance.App.You} yours / {provenance.App.Ai} AI / {provenance.App.Scaffold} scaffold); " +
                          $"tooling {provenance.Tooling.Total} lines ({provenance.Tooling.Ai} AI)");
    else
        Console.Error.WriteLine("journeygen: provenance unavailable (no git history?)");
}

config.Evaluate(analyzer, root);
var done = config.Concepts.Count(c => c.Done);
Console.WriteLine($"journeygen: {done}/{config.Concepts.Count} concepts covered");

var block = ReadmeWriter.Render(config, analyzer, git, provenance, siteUrl);

if (readmePath is null)
{
    Console.Error.WriteLine("journeygen: no README found, skipping README update");
}
else
{
    var existing = File.Exists(readmePath) ? File.ReadAllText(readmePath) : "";
    var updated = ReadmeWriter.Inject(existing, block);

    if (check)
    {
        if (Normalize(existing) != Normalize(updated))
        {
            Console.Error.WriteLine($"journeygen: {Path.GetFileName(readmePath)} is out of date. Run `dotnet run --project tools/JourneyGen -- --root .`");
            return 1;
        }
        Console.WriteLine("journeygen: README is up to date");
    }
    else
    {
        File.WriteAllText(readmePath, updated);
        Console.WriteLine($"journeygen: wrote {Path.GetRelativePath(root, readmePath)}");
    }
}

if (!check)
{
    SiteWriter.Write(outDir, config, analyzer, git, provenance, repoUrl);
    Console.WriteLine($"journeygen: wrote {Path.GetRelativePath(root, outDir)}/index.html");
}

return 0;

// The generated block carries a timestamp, so ignore it when deciding staleness.
static string Normalize(string s)
{
    var i = s.IndexOf("<sub>Last updated", StringComparison.Ordinal);
    while (i >= 0)
    {
        var end = s.IndexOf("</sub>", i, StringComparison.Ordinal);
        if (end < 0) break;
        s = s.Remove(i, end - i + "</sub>".Length);
        i = s.IndexOf("<sub>Last updated", StringComparison.Ordinal);
    }
    return s.Replace("\r\n", "\n").Trim();
}

static string? FindReadme(string root) =>
    new[] { "README.MD", "README.md", "Readme.md", "readme.md" }
        .Select(n => Path.Combine(root, n))
        .FirstOrDefault(File.Exists);

static string GuessRepoUrl(string root)
{
    var env = Environment.GetEnvironmentVariable("GITHUB_SERVER_URL");
    var repo = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY");
    if (!string.IsNullOrEmpty(env) && !string.IsNullOrEmpty(repo)) return $"{env}/{repo}";

    try
    {
        var psi = new System.Diagnostics.ProcessStartInfo("git", "remote get-url origin")
        {
            WorkingDirectory = root, RedirectStandardOutput = true, UseShellExecute = false,
        };
        using var p = System.Diagnostics.Process.Start(psi);
        var url = p?.StandardOutput.ReadToEnd().Trim() ?? "";
        p?.WaitForExit();
        if (url.StartsWith("git@github.com:", StringComparison.Ordinal))
            url = "https://github.com/" + url["git@github.com:".Length..];
        return url.EndsWith(".git", StringComparison.Ordinal) ? url[..^4] : url;
    }
    catch { return ""; }
}

static string GuessPagesUrl(string repoUrl)
{
    if (!repoUrl.StartsWith("https://github.com/", StringComparison.Ordinal)) return "";
    var parts = repoUrl["https://github.com/".Length..].Split('/');
    return parts.Length < 2 ? "" : $"https://{parts[0].ToLowerInvariant()}.github.io/{parts[1]}/";
}

string? Arg(string name)
{
    var argv = Environment.GetCommandLineArgs();
    for (var i = 0; i < argv.Length - 1; i++)
        if (argv[i] == name) return argv[i + 1];
    return null;
}

bool Flag(string name) => Environment.GetCommandLineArgs().Contains(name);
