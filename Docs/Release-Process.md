# Release Process

This document describes how to release SlnUp to NuGet.org using the new manual release workflow.

## Overview

The release process has been separated from the CI build process to provide better control over when releases are performed. This eliminates the need to remember to include "do release" in commit messages.

## How It Works

1. **Continuous Integration**: Every push to the `main` branch triggers a CI build that compiles, tests, and creates package artifacts.
2. **Manual Release**: When you're ready to release, you manually trigger the release workflow which downloads artifacts from a successful CI build and publishes to NuGet.org.

## Release Steps

### 1. Ensure CI Build is Successful

- Push your changes to the `main` branch
- Wait for the CI workflow to complete successfully
- Verify the build created the expected package artifacts

### 2. Trigger Manual Release

1. Go to your repository on GitHub
2. Navigate to **Actions** tab
3. Select **Manual Release** workflow from the left sidebar
4. Click **Run workflow** button
5. Fill in the workflow inputs:
   - **CI Run ID**: Leave empty to use the latest successful build, or specify a specific run ID
   - **Confirm release**: ✅ **Must check this box** to proceed with the release
   - **Release notes**: Optional notes about this release

### 3. Monitor Release Progress

- The workflow will show you which build it's releasing
- It will download the package artifacts from the specified CI run
- It will display the package version being released
- Finally, it will push the package to NuGet.org

### 4. Verify Release

- Check the workflow summary for the NuGet.org link
- Verify the package appears on NuGet.org
- Test installing the new version

## Workflow Features

### Safety Features

- **Confirmation Required**: You must explicitly check the confirmation box
- **Build Validation**: Ensures the source build was successful
- **Clear Logging**: Shows exactly what's being released and from which build

### Flexibility

- **Release Any Build**: Can release any successful CI build, not just the latest
- **Release Notes**: Optional field for documenting what's in the release
- **Detailed Summary**: Provides links and information about the released package

### Error Handling

- **No Confirmation**: If you forget to check the confirmation box, the workflow will fail safely
- **Missing Artifacts**: If the specified build doesn't have artifacts, the workflow will fail
- **Clear Messages**: All failures include helpful error messages

## Troubleshooting

### "No successful CI runs found"

- Ensure you have at least one successful CI build on the main branch
- Check that the CI workflow completed without errors

### "Could not find build job in the specified run"

- Verify the run ID you specified is correct
- Ensure the specified run was a CI workflow run (not a different workflow)

### "No package files found"

- The specified CI run may not have completed successfully
- Check that the build job in the CI run produced package artifacts

### Release not confirmed

- You must check the "Confirm you want to release to NuGet.org" checkbox
- Re-run the workflow with the confirmation checked

## Migration from Old Process

The old process required including "do release" in commit messages and used a separate `workflow_release.yml` file. This is no longer needed:

- ❌ **Old**: Commit with "do release" in message → automatic release via `workflow_release.yml`
- ✅ **New**: Commit normally → manual release when ready via the consolidated `Release.yml` workflow

The `workflow_release.yml` file has been removed as all release logic is now consolidated into the manual release workflow.

## Benefits

- **No Forgotten Releases**: No need to remember special commit message keywords
- **Better Control**: Release exactly when you want to
- **Release Any Build**: Can release any successful build, not just the latest
- **Clear Audit Trail**: Easy to see who released what and when
- **Safer Process**: Explicit confirmation required before releasing
