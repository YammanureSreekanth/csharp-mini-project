Creating a solution in VS Code (Mac) via terminal

You're not doing this through a GUI wizard like Visual Studio on Windows — on Mac with VS Code, it's all dotnet CLI, and VS Code just edits the files.

# 1. Navigate to your repo root (or wherever you want the solution)
cd ~/path/to/csharp-mini-project

# 2. Create the solution file
dotnet new sln -n csharp-ecom-mini-app

# This creates CSharpLearningcsharp-ecom-mini-apps.slnx in the current directory
Adding existing projects to the solution

# Since you already have multiple console projects, you add each one:

dotnet sln add Catalog.ConsoleApp

# Or, if you want to add all .csproj files under csharp-mini-project

find src -name "*.csproj" | xargs -I {} dotnet sln add {}

# Adding a brand-new project directly into the solution

dotnet new console -n NewProjectName -o src/NewProjectName
dotnet sln add src/NewProjectName/NewProjectName.csproj

# Verify

dotnet sln list