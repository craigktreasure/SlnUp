#Requires -PSEdition Core

$repoRoot = Join-Path $PSScriptRoot '..'

Push-Location $repoRoot

try {
    & dotnet run --project .\src\VisualStudio.VersionScraper\ -- .\src\SlnUp\Versions.json
}
finally {
    Pop-Location
}
