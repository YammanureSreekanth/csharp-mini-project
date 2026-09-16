using System.Diagnostics;
using System.Globalization;

namespace JourneyGen;

/// <summary>Mines `git log` for the shape of the journey over time.</summary>
public static class GitHistory
{
    const string Marker = "@@C@@";

    /// <summary>Vendored, generated and binary files would swamp the churn chart.</summary>
    static readonly string[] IgnoredPaths =
        [ "/bin/", "/obj/", "wwwroot/lib/", "package-lock.json", ".min.js", ".min.css",
          ".png", ".jpg", ".jpeg", ".gif", ".ico", ".woff", ".woff2", ".ttf", ".eot", ".map" ];

    static bool Vendored(string path) =>
        IgnoredPaths.Any(p => path.Contains(p, StringComparison.OrdinalIgnoreCase));

    public static GitSummary Read(string root)
    {
        var summary = new GitSummary();
        var raw = Run(root, $"log --no-merges --date=short --reverse --pretty=format:\"{Marker}|%H|%ad\" --numstat");
        if (string.IsNullOrWhiteSpace(raw)) return summary;

        var weeks = new Dictionary<string, WeekPoint>(StringComparer.Ordinal);
        var days = new HashSet<string>(StringComparer.Ordinal);
        WeekPoint? current = null;
        var seenProjectsThisCommit = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var line in raw.Split('\n'))
        {
            if (line.StartsWith(Marker, StringComparison.Ordinal))
            {
                var parts = line.Split('|');
                if (parts.Length < 3) continue;
                var date = parts[2].Trim();

                summary.TotalCommits++;
                days.Add(date);
                if (summary.FirstCommit.Length == 0) summary.FirstCommit = date;
                summary.LastCommit = date;

                current = Bucket(weeks, date);
                current.Commits++;
                seenProjectsThisCommit.Clear();
                continue;
            }

            if (current is null || line.Length == 0) continue;

            var cols = line.Split('\t');
            if (cols.Length < 3) continue;

            var path = cols[2].Replace('\\', '/');
            if (Vendored(path)) continue;

            if (int.TryParse(cols[0], out var added)) current.Added += added;
            if (int.TryParse(cols[1], out var removed)) current.Removed += removed;

            // Attribute the commit to every top-level folder it touched, once each.
            var project = path.Contains('/') ? path[..path.IndexOf('/')] : "(root)";
            if (seenProjectsThisCommit.Add(project))
                summary.CommitsByProject[project] = summary.CommitsByProject.GetValueOrDefault(project) + 1;
        }

        summary.ActiveDays = days.Count;
        summary.Weeks = FillGaps(weeks.Values.OrderBy(w => w.Start, StringComparer.Ordinal).ToList());
        return summary;
    }

    static WeekPoint Bucket(Dictionary<string, WeekPoint> weeks, string isoDate)
    {
        var d = DateTime.ParseExact(isoDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var monday = d.AddDays(-(((int)d.DayOfWeek + 6) % 7));
        var key = monday.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        if (!weeks.TryGetValue(key, out var w))
            weeks[key] = w = new WeekPoint
            {
                Start = key,
                Week = $"{ISOWeek.GetYear(d)}-W{ISOWeek.GetWeekOfYear(d):00}",
            };
        return w;
    }

    /// <summary>Insert empty weeks so the timeline chart shows real gaps rather than compressing them.</summary>
    static List<WeekPoint> FillGaps(List<WeekPoint> points)
    {
        if (points.Count < 2) return points;
        var filled = new List<WeekPoint>();
        var cursor = DateTime.ParseExact(points[0].Start, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var end = DateTime.ParseExact(points[^1].Start, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var byStart = points.ToDictionary(p => p.Start, StringComparer.Ordinal);

        while (cursor <= end)
        {
            var key = cursor.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            filled.Add(byStart.TryGetValue(key, out var hit)
                ? hit
                : new WeekPoint { Start = key, Week = $"{ISOWeek.GetYear(cursor)}-W{ISOWeek.GetWeekOfYear(cursor):00}" });
            cursor = cursor.AddDays(7);
        }
        return filled;
    }

    static string Run(string root, string args)
    {
        try
        {
            var psi = new ProcessStartInfo("git", args)
            {
                WorkingDirectory = root,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            using var p = Process.Start(psi);
            if (p is null) return "";
            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return p.ExitCode == 0 ? output : "";
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"journeygen: git history unavailable ({ex.Message})");
            return "";
        }
    }
}
