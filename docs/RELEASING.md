# Releasing CyberCalc.NET9

This guide documents the complete process for preparing and publishing a CyberCalc.NET9 release.

## 1. Development (`dev`)

1. Switch to the development branch. The repository currently has `main`; create `dev` before the first development cycle:

```bash
git switch -c dev
git push -u origin dev
```

For later development cycles:

```bash
git switch dev
git pull origin dev
```

2. Implement and test features, improvements, or fixes. Do not change calculator logic or WPF behavior unless the release requires it.
3. Test the desktop application:

```bash
dotnet restore WPF-CALC-NET-9.sln --runtime win-x64
dotnet build WPF-CALC-NET-9.sln --configuration Release --no-restore
```

4. Test the website:

```bash
cd website
npm ci
npm run sync-release
npm run build
npm run dev
cd ..
```

5. Commit the development changes:

```bash
git add -A
git commit -m "Describe the change"
```

Push the development branch when ready:

```bash
git push origin dev
```

## 2. Prepare the release

1. Choose the next stable version using `MAJOR.MINOR.PATCH`, for example `2.1.8`.
2. Update `version.txt` so it contains only the version:

```text
2.1.8
```

3. Review release-related files if applicable:

- `CyberCalc.NET9/WPF-CALC-NET-9.csproj` reads `version.txt`; normally it does not need manual version edits.
- `installer/CyberCalc.iss` receives the version from the build pipeline.
- `README.md` and the website do not require a manually entered release number.

4. Run the local desktop release script:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File scripts/build-release.ps1
```

This creates:

- `artifacts/CyberCalc-Setup-X.Y.Z-x64.exe`
- `artifacts/CyberCalc-X.Y.Z-win-x64.zip`
- `artifacts/CyberCalc-X.Y.Z-SHA256SUMS.txt`

5. Build the website as described above.
6. Commit release preparation:

```bash
git add -A
git commit -m "Prepare release 2.1.8"
```

## 3. Merge into `main`

1. Switch to `main` and update it:

```bash
git switch main
git pull origin main
```

2. Merge the tested development branch:

```bash
git merge dev
```

Resolve merge conflicts if they occur, verify the project again, and push:

```bash
git push origin main
```

The tag must point to the resulting release commit on `main`.

## 4. Create and push the version tag

Release tags use the exact format:

```text
vMAJOR.MINOR.PATCH
```

For version `2.1.8`:

```bash
git switch main
git pull origin main
git tag -a v2.1.8 -m "CyberCalc.NET9 2.1.8"
git push origin v2.1.8
```

The tag must match the value in `version.txt`. The workflow rejects mismatched stable versions and tags that are not in `vX.Y.Z` format.

Do not create a tag before the release commit is pushed to `main`.

## 5. Automated release

Pushing `vX.Y.Z` starts `.github/workflows/release.yml`. The workflow:

1. Checks out the repository.
2. Validates that the tag matches `vX.Y.Z`.
3. Reads `version.txt` and verifies that it matches the tag.
4. Installs .NET 9.
5. Restores and builds the WPF application in Release mode.
6. Creates a clean `win-x64` publication using:
   - `SelfContained=true`
   - `PublishReadyToRun=true`
   - `PublishTrimmed=false`
   - `PublishAot=false`
7. Removes developer-only files from the publication.
8. Installs Inno Setup and creates the installer; code signing is not currently configured:
   - `CyberCalc-Setup-X.Y.Z-x64.exe`
9. Creates:
   - `CyberCalc-X.Y.Z-win-x64.zip`
10. Generates `CyberCalc-X.Y.Z-SHA256SUMS.txt`.
11. Verifies that all release files exist and that every checksum is correct.
12. Prevents overwriting an existing GitHub Release.
13. Creates the GitHub Release and uploads the installer, ZIP, and checksum file.
14. Builds the Astro website and synchronizes release metadata from GitHub.
15. Deploys the website to GitHub Pages through the `github-pages` environment.

No GitHub Release or Pages deployment is performed locally by this guide.

## 6. Verify the release

- Open **Actions** and confirm that both the `Release` job and `Build and deploy website` job completed successfully.
- Open the GitHub **Releases** page.
- Confirm these files are available:
  - `CyberCalc-Setup-X.Y.Z-x64.exe`
  - `CyberCalc-X.Y.Z-win-x64.zip`
  - `CyberCalc-X.Y.Z-SHA256SUMS.txt`
- Open `https://zerodayz7.github.io/CyberCalc.NET9/` and confirm that it shows the new version and working download links.
- If practical, install the installer on a clean Windows 10 or Windows 11 x64 system.
- Confirm that the application launches and existing user history remains in `%LOCALAPPDATA%\CyberCalc`.

## 7. Command reference

```bash
# Development
git switch dev
git pull origin dev
# make changes
git add -A
git commit -m "Describe the change"
git push origin dev

# Release preparation
git switch dev
# set the new version in version.txt
git add version.txt
git commit -m "Prepare release 2.1.8"

# Merge and release
git switch main
git pull origin main
git merge dev
git push origin main

git tag -a v2.1.8 -m "CyberCalc.NET9 2.1.8"
git push origin v2.1.8
```

## 8. Troubleshooting

### Tag does not match `version.txt`

Check `version.txt`, the pushed tag, and the workflow log. They must describe the same `X.Y.Z` version.

Delete an unused local or remote tag only when intentional:

```bash
git tag -d v2.1.8
git push origin :refs/tags/v2.1.8
```

Do not move or reuse a tag for an existing release.

### Release workflow fails

Open the failed workflow run under **Actions** and inspect the earliest failing step. Common areas are WPF restore/build, Inno Setup compilation, artifact verification, or GitHub Release creation.

### Installer or ZIP is missing

Verify the `Create GitHub Release` step and the `Verify release artifacts` step. Confirm that the exact three expected filenames were generated.

### GitHub Pages deployment fails

Check the `website` job for dependency, metadata, Astro build, Pages configuration, or permissions errors. Confirm that **Settings → Pages → Source** is set to **GitHub Actions**.

### Website still displays an older version

Wait for the Pages deployment to finish. Check the workflow's `Sync release metadata and build` step and confirm that the tag asset names exactly match the names used by `sync-release.mjs`. The website does not query release downloads from the browser.

## 9. GitHub repository configuration

GitHub Pages must be configured as follows:

- Repository **Settings → Pages**
- **Source: GitHub Actions**

No secret token is required for the existing workflow. The workflow uses the provided `GITHUB_TOKEN` and repository permissions declared in the workflow files.
