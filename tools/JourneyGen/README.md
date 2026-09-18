# JourneyGen

Reads this repository's own C# source and git history, then regenerates:

* the block between `<!-- JOURNEY:START -->` and `<!-- JOURNEY:END -->` in `README.MD`
* `artifacts/site/` — the GitHub Pages dashboard (`index.html` + `journey.json`)

It parses with Roslyn (syntax only, no compilation), so it runs in seconds and
never needs the projects to build.

## Run it locally

```bash
dotnet run --project tools/JourneyGen -- --root .
open artifacts/site/index.html
```

`--check` writes nothing and exits 1 when the README block is out of date — that
is what CI uses to decide whether a commit is needed, which is also what stops
the workflow from triggering itself in a loop.

| Flag | Default | Meaning |
|---|---|---|
| `--root <dir>` | current directory | repository root to analyse |
| `--config <file>` | `<root>/journey.json` | the concept checklist |
| `--out <dir>` | `<root>/artifacts/site` | where the dashboard is written |
| `--readme <file>` | `<root>/README.MD` | file containing the journey markers |
| `--site-url <url>` | derived from `origin` | link used in the README block |
| `--check` | off | report staleness, write nothing |

## Adding a concept

Edit `journey.json`. Each concept says how it is proven from the code:

```jsonc
{ "id": "records", "label": "Records", "detect": "auto:records" }
```

| Detector | Matches |
|---|---|
| `auto:<feature>` | a feature the Roslyn walker recognises |
| `regex:<pattern>` | .NET regex against the text of every `.cs` file |
| `path:<fragment>` | any file path in the repo containing the fragment |
| `manual` | never ticks itself — roadmap only |

List several and any one match counts. The `auto:` ids are the strings passed to
`Mark(...)` in `FeatureWalker.cs` plus the layout-level ones in
`CodeAnalyzer.DetectRepoShapeFeatures`. To teach it something new, add a
`Mark("my-feature")` to the walker and reference `auto:my-feature` in the config.

## Code provenance

The page reports how much of the code each party actually wrote, split three ways:
**mine**, **AI-written**, and **`dotnet new` scaffold**. Application projects carry the
headline figure; `tools/` is reported separately so AI-written tooling cannot skew it.

Every non-blank line is attributed with `git blame -w -M` to the commit that last touched
it, and each commit is classified once. Lines count as yours by default — a line is only
AI or scaffold because a rule in `journey.json` says so. `-w` ignores whitespace-only
changes and `-M` follows code moved within a file, so a reformat does not transfer
authorship.

### Keeping it honest going forward

**When AI writes code for you, put a trailer in the commit message.** That is the whole
convention, and it is what keeps the number true without any bookkeeping:

```
Add product search with filters

Co-Authored-By: Claude Opus 5 <noreply@anthropic.com>
```

Any commit whose message matches an entry in `provenance.aiMarkers` has its lines counted
as AI-written. Commits without a marker count as yours. If you forget on a commit, add its
sha to `provenance.aiCommits` instead — that is how `00e0f1f` (which added this tool) is
declared, since it predates the convention.

### The declarations in place

| Rule | Value | Why |
|---|---|---|
| `aiPaths` | `tools/` | this generator is AI-written in full |
| `aiCommits` | `00e0f1f` | added `tools/JourneyGen` before the trailer convention existed |
| `scaffoldCommits` | `d56cc61` | `dotnet new mvc` + `dotnet new webapi`; every `.cs` file it added is still byte-identical to the template |
| `botAuthors` | `github-actions[bot]` | the workflow's own README commits |

Edit a scaffold line and blame reassigns it to you automatically — the split tracks the
code rather than the declaration going stale.

### Verifying a scaffold claim

To re-check that a file really is untouched template output:

```bash
dotnet new mvc -n Ecom.MvcWebApp -o /tmp/tpl --no-restore
diff /tmp/tpl/Program.cs Ecom.MvcWebApp/Program.cs
```

## Files

| File | Role |
|---|---|
| `CodeAnalyzer.cs` | walks every `.cs` file, collects types and per-project stats |
| `FeatureWalker.cs` | the syntax-level feature detectors |
| `GitHistory.cs` | weekly commit/churn timeline from `git log` |
| `Provenance.cs` | line-level authorship split via `git blame` |
| `Config.cs` | loads `journey.json`, evaluates each concept |
| `MermaidRenderer.cs` | class diagram from the parsed types |
| `ReadmeWriter.cs` | the README block |
| `SiteWriter.cs` | the dashboard, from `assets/index.template.html` |

The project is deliberately **not** in `csharp-ecom-mini-app.slnx`, so the Roslyn
dependency stays out of the solution build.
