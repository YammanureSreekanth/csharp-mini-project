namespace JourneyGen;

/// <summary>
/// Maps a source-relative path to the project it belongs to: the nearest ancestor
/// directory that itself contains a .csproj. A plain `path.Split('/')[0]` breaks once
/// a solution's projects live a level or two below the repo root (a monorepo folder,
/// or a `src/` layout) — this walks up from the file instead of assuming the root is
/// one level above every project.
/// </summary>
public static class ProjectResolver
{
    static readonly Dictionary<string, List<string>> Cache = new(StringComparer.OrdinalIgnoreCase);

    public static (string Dir, string Name) Resolve(string root, string rel)
    {
        var dirs = DirsWithCsproj(root);
        var cur = Path.GetDirectoryName(rel)?.Replace('\\', '/') ?? "";

        while (true)
        {
            if (dirs.Contains(cur))
                return (cur, cur.Length == 0 ? "(root)" : Path.GetFileName(cur));

            if (cur.Length == 0) break;
            var i = cur.LastIndexOf('/');
            cur = i < 0 ? "" : cur[..i];
        }

        // No .csproj above it (e.g. a solution-level file) — fall back to the old behaviour.
        var top = rel.Contains('/') ? rel[..rel.IndexOf('/')] : "(root)";
        return (top, top);
    }

    static List<string> DirsWithCsproj(string root)
    {
        if (Cache.TryGetValue(root, out var cached)) return cached;

        var dirs = Directory.EnumerateFiles(root, "*.csproj", SearchOption.AllDirectories)
            .Select(f => f.Replace('\\', '/'))
            .Where(f => !f.Contains("/bin/", StringComparison.OrdinalIgnoreCase)
                     && !f.Contains("/obj/", StringComparison.OrdinalIgnoreCase))
            .Select(f => Path.GetRelativePath(root, Path.GetDirectoryName(f)!).Replace('\\', '/'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        Cache[root] = dirs;
        return dirs;
    }
}
