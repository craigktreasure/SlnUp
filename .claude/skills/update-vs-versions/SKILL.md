---
name: update-vs-versions
description: Update the Visual Studio version database by scraping Microsoft's documentation
allowed-tools: Bash, Read, Grep, Glob
---

Update the Visual Studio version database by scraping Microsoft's documentation.

This runs the VisualStudio.VersionScraper project, which:
1. Scrapes Visual Studio release history pages from Microsoft docs (VS 2017, 2019, 2022, 2026)
2. Parses HTML tables to extract version, release date, build number, and channel info
3. Regenerates `src/SlnUp.Core/VersionManager.g.cs` with the latest version data

## Steps

1. Run the version scraper from the repository root:
   ```
   dotnet run --project src/VisualStudio.VersionScraper --no-launch-profile -- src/SlnUp.Core/VersionManager.g.cs --format CSharp
   ```
2. After the scraper completes, read `src/SlnUp.Core/VersionManager.g.cs` and verify the file was updated (check that it contains recent Visual Studio versions and looks well-formed).
3. Run the project tests to make sure nothing is broken:
   ```
   dotnet test
   ```
4. Report a summary of what changed: any new VS versions added, any versions removed, and the current latest version listed in the file.

## Notes

- This requires an internet connection to reach Microsoft's documentation pages.
- The scraper caches HTTP responses by default. Pass `--no-cache` to bypass the cache if you suspect stale data.
- Preview versions are filtered out automatically.
- The generated file uses `[GeneratedCodeAttribute]` and should not be hand-edited.
