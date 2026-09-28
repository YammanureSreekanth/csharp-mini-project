using System.Diagnostics;

namespace JourneyGen;

/// <summary>
/// The repo's file-scanning source of truth. Untracked and gitignored files — build
/// output, secrets like local.settings.json, a compiled main.json next to its source
/// main.bicep — sit on one machine's disk without ever reaching another, so scanning
/// the raw filesystem makes a detector's result depend on who is running it and what
/// they happen to have built locally. Restricting scans to what `git ls-files` reports
/// keeps every machine and CI in agreement, because it is the one file list every
/// checkout actually shares.
/// </summary>
public static class GitTrackedFiles
{
    static readonly Dictionary<string, HashSet<string>?> Cache = new(StringComparer.Ordinal);

    /// <summary>Null means git was unavailable; callers should fall back to scanning everything.</summary>
    public static HashSet<string>? Get(string root)
    {
        if (Cache.TryGetValue(root, out var cached)) return cached;

        HashSet<string>? result = null;
        try
        {
            var psi = new ProcessStartInfo("git", "ls-files")
            {
                WorkingDirectory = root,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            using var p = Process.Start(psi);
            if (p is not null)
            {
                var output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                if (p.ExitCode == 0)
                {
                    result = new HashSet<string>(StringComparer.Ordinal);
                    foreach (var line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                        result.Add(line.Trim().Replace('\\', '/'));
                }
            }
        }
        catch
        {
            result = null;
        }

        Cache[root] = result;
        return result;
    }

    public static bool IsTracked(string root, string relPath)
    {
        var tracked = Get(root);
        return tracked is null || tracked.Contains(relPath.Replace('\\', '/'));
    }
}
