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

## Files

| File | Role |
|---|---|
| `CodeAnalyzer.cs` | walks every `.cs` file, collects types and per-project stats |
| `FeatureWalker.cs` | the syntax-level feature detectors |
| `GitHistory.cs` | weekly commit/churn timeline from `git log` |
| `Config.cs` | loads `journey.json`, evaluates each concept |
| `MermaidRenderer.cs` | class diagram from the parsed types |
| `ReadmeWriter.cs` | the README block |
| `SiteWriter.cs` | the dashboard, from `assets/index.template.html` |

The project is deliberately **not** in `csharp-ecom-mini-app.slnx`, so the Roslyn
dependency stays out of the solution build.
