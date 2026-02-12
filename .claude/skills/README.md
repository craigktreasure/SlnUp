# Claude Code Skills

This directory contains [Claude Code skills](https://docs.anthropic.com/en/docs/claude-code/skills) for the SlnUp project. Each skill is a directory containing a `SKILL.md` file that defines a slash command. Invoke them by typing `/<name>` in a Claude Code session.

## Available Skills

### `/update-vs-versions`

**Purpose:** Regenerates the Visual Studio version database (`src/SlnUp.Core/VersionManager.g.cs`) by scraping Microsoft's official release history documentation.

**When to use:**

- When Microsoft has released new Visual Studio versions or updates
- When the version data in the codebase appears stale or incomplete
- As part of a periodic maintenance workflow to keep SlnUp current

**What it does:**

1. Runs the `VisualStudio.VersionScraper` project against Microsoft's docs for VS 2017, 2019, 2022, and 2026
2. Parses release tables to extract version numbers, build numbers, channels, and release dates
3. Generates updated C# source code in `VersionManager.g.cs`
4. Runs the test suite to verify nothing is broken
5. Reports a summary of changes (new versions added, latest version, etc.)

**Replaces:** `scripts/UpdateVSVersions.ps1`

**Options:** If you suspect cached data is stale, ask Claude to pass `--no-cache` to the scraper.
