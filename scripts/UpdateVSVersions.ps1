#Requires -PSEdition Core

$repoRoot = Join-Path $PSScriptRoot '..'

Push-Location $repoRoot

try {
    & dotnet run --project .\src\VisualStudio.VersionScraper\ --no-launch-profile -- .\src\SlnUp.Core\VersionManager.g.cs --format CSharp
}
finally {
    Pop-Location
}
