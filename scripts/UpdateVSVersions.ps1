#Requires -PSEdition Core

$repoRoot = Join-Path $PSScriptRoot '..'

Push-Location $repoRoot

try {
    & dotnet run --project .\src\VisualStudio.VersionScraper\ -- .\src\SlnUp\VersionManager.g.cs --format CSharp
}
finally {
    Pop-Location
}
