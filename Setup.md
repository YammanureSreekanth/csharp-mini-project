# Solution Setup Cheatsheet

Personal reference for managing the `csharp-ecom-mini-app` solution.
Run all of these from the repo root (same folder as the `.sln`), unless noted.

---

## Create the solution (one-time, already done)

```
dotnet new sln -n csharp-ecom-mini-app
```

Creates `csharp-ecom-mini-app.sln` (or `.slnx` on newer SDKs using the XML
solution format) in the current directory.

---

## Add an existing project to the solution

Always point at the actual `.csproj`, not just the folder:

```
dotnet sln add Catalog.ConsoleApp/Catalog.ConsoleApp.csproj
```

### Bulk-add every .csproj under a folder

```
find src -name "*.csproj" | xargs -I {} dotnet sln add {}
```

Adjust `src` to whatever your actual projects folder is called (or `.` if
projects sit flat next to the `.sln`).

---

## Create a brand-new project directly into the solution

```
dotnet new console -n NewProjectName -o src/NewProjectName
dotnet sln add src/NewProjectName/NewProjectName.csproj
```

Swap `console` for `classlib` for a class library project.

---

## Wire up project references

Reminder: only ever reference "inward" toward `Catalog.Domain` — never the
other direction. See `README.md` for the layering rules.

```
dotnet add Catalog.Application/Catalog.Application.csproj reference Catalog.Domain/Catalog.Domain.csproj
```

---

## Verify

```
dotnet sln list
```

---

## Build

```
dotnet build
```

---

## Run

Once there's more than one project with a `Main` method in the solution,
`dotnet run` alone is ambiguous — always specify which project:

```
dotnet run --project Catalog.ConsoleApp/Catalog.ConsoleApp.csproj
```

Or `cd` into that project's folder first and run `dotnet run` with no args.

---

## Notes

-