---
name: android-play-release
description: Release a new Android version of this .NET MAUI app end to end - check a clean Release build on the emulator for launch crashes (fixing and re-checking until clean), bump versionName/versionCode in Platforms/Android/AndroidManifest.xml (patch by default), open and merge the PR, write the store release notes, then build the signed .aab and upload it to Google Play after the user confirms. Use whenever the user wants to release, ship or put out a new Android version ("Android をリリースして", "リリース準備", "バージョンを上げて PR", "Play に出す", "Google Play にアップロード", "1.2.0 を出したい").
---

# Release an Android version

All commands run from the app's repository root (the folder with its one `.csproj`), in Git Bash on Windows. `S=.claude/skills/android-play-release/scripts` below. `<scratch>` is Claude's scratchpad directory.

After a short start (stage 0), five stages run in order, each finished before the next, then a report (stage 6):

1. **Release check**: a clean Release build must launch and run without a fatal bug. Fix and re-check until it does.
2. **Version bump** in `Platforms/Android/AndroidManifest.xml`.
3. **PR and merge** through `/ship-branch`.
4. **Release notes** through `/store-release-notes`, saved to a file.
5. **Upload to Google Play**: the signed bundle and exactly those notes, after the user says yes.

The check comes first because a versionCode is a promise to Google Play: once uploaded it can never be reused, so bump only once the app is known to start.

**The upload is the one step that cannot be taken back.** A committed bundle uses up its versionCode for good, and a published release reaches real users. Stage 5 always stops right before the upload, shows what will be sent, and goes on only after a clear yes in this conversation; "release it" at the start does not cover it. By default the release is a **draft**: Google Play holds the bundle and notes, and the user starts the rollout in the Play Console.

**On Windows:** network git commands use `git -c http.sslBackend=schannel ...`. Paths in the variables below may be `C:\...`; `build_aab.sh` converts them. `jarsigner` is looked for in `JAVA_HOME`, Microsoft OpenJDK and Android Studio under `C:\Program Files`. iOS cannot be checked on Windows: if a fix touches shared code or layout, say so.

## One-time setup (per app, by the user, before the first upload)

The upload key and Google Play access are the user's secrets. Claude never asks for them in chat, types them, prints them or puts them in the repository; key files live outside it. The variables go in the `env` of the app's `.claude/settings.local.json` (git-ignored, per project, so each app can have its own key). Claude Code reads it at start, so restart after a change.

| Variable | What |
|---|---|
| `ANDROID_UPLOAD_KEYSTORE` | path to the app's upload keystore (`.keystore` / `.jks`), outside the repository |
| `ANDROID_UPLOAD_KEY_ALIAS` | the key's alias |
| `ANDROID_UPLOAD_STORE_PASS`, `ANDROID_UPLOAD_KEY_PASS` | its passwords (MSBuild reads them through `env:`; they never appear in a command or log) |
| `GOOGLE_PLAY_SERVICE_ACCOUNT_JSON` | path to a Google Cloud service account key file, outside the repository. In the Play Console (Users and permissions) it needs access to the app with "Release apps to testing tracks" and "Release to production". |

Before relying on `settings.local.json`, check it is git-ignored (`git check-ignore -v .claude/settings.local.json`); if not, tell the user and stop.

The Python scripts run with [uv](https://docs.astral.sh/uv/). `upload_to_play.py` lists its packages in its header and their versions and hashes are pinned in `upload_to_play.py.lock`; `uv run --locked` refuses anything else, since this script holds the Play credentials. To update them on purpose: `uv lock --script $S/upload_to_play.py --upgrade`, then review the lock diff. If `uv` is missing, ask the user to install it.

If anything is missing when stage 5 starts, stop there and say exactly what. Do not create keys, service accounts or Play Console access. Stages 1–4 need none of it.

## 0. Start

- Read the project's CLAUDE.md: its verification command, commit rules, and whether it keeps a file with real keys only in the working tree (FlagsRally: `Repository/Constants.cs`; Tsundoku commits its `Constants.cs`, so it has none). Never stage such a file.
- Work from the latest default branch: `git checkout master && git -c http.sslBackend=schannel pull --ff-only`. Open PRs meant for this release are merged first (`gh pr list --state open`); ask about any whose status is unclear.
- Find the last version bump (`-G`, since `-S` misses value-only changes):
  ```bash
  git log -1 --format=%h -G'android:versionName' -- Platforms/Android/AndroidManifest.xml
  ```
- **Was the current version already uploaded?** The repository cannot tell. If the bump is the latest change, or the changelog's top section is that version, ask the user once. If it was not uploaded, release it under the current number and skip the bump in stage 2.
- Pick the version (the user's wins: `1.2.0`, "マイナーを上げて"; otherwise the patch) and preview it:
  ```bash
  uv run $S/bump_android_version.py --dry-run   # --minor / --major / --version X.Y.Z when asked
  ```
- Create the release branch: `git checkout -b chore/bump-android-<version>`. Fixes from stage 1 go on it too, so the release is one PR.

## 1. Release check (repeat until clean)

FlagsRally 1.1.20 passed every Debug check and still crashed the moment it opened from Google Play: R8 had removed a class the subscription library loads at startup. Only a Release build shows what shrinking, trimming and AOT remove, so this check is never skipped, even for a bump with no code changes.

1. The project's verification (CLAUDE.md, e.g. `tools/verify/verify.sh`), or `dotnet test` of its test project. Everything must pass.
2. An emulator must be running (`adb devices`). If none is, ask the user to start one; do not skip the launch check silently.
3. If the project has its own emulator tooling (e.g. `tools/emu.sh` with `db-backup`), back up the app's data with it first. Otherwise note that the Release install keeps data (`adb install -r`) and do nothing that changes data.
4. Release smoke test (clean Release build, install over the current app, launch, logcat):
   ```bash
   bash $S/release_smoke.sh <scratch>/release-check-1
   ```
   Exit 0 means it launched and logcat has no fatal line from the app. Then, with the app still running, open every tab and page (`adb shell input tap X Y`, wait 2–3 s, `adb exec-out screencap -p > <scratch>/x.png`) and read the screenshots yourself: a blank page or endless spinner is fatal even without a crash. Check `adb logcat -d` again afterwards.
5. Check what changed since the last release (`git log <last bump>..HEAD`): exercise those features on the Release build, in English and Japanese if text changed. Subscription screens: the plans may not load without a Play license tester account; say so instead of guessing.
6. **Something fatal?** (crash, freeze, "isn't responding", a page that does not load, data loss.) Find the cause first (the exception and stack in logcat). Fix it on the release branch, add a test when possible, one concern per commit, add a changelog entry if the project keeps one. Then **go back to step 1 with a new output folder**: only a full clean run counts.
7. When clean, put the Debug build back if the project's tooling needs it (a Release build is not debuggable, so `run-as`-based restore fails on it), and restore any backup.

Non-fatal problems found on the way do not hold the release: list them, and fix them here only if small and clearly wanted.

## 2. Version bump

1. `uv run $S/bump_android_version.py` (same flags as the dry run). It writes `android:versionName` and `android:versionCode` (`MMmmpp`, zero-padded: 1.0.8 → `010008`), keeps the BOM and refuses a code that does not go up.
2. `git diff Platforms/Android/AndroidManifest.xml` must show only those two values. Commit just that file: `Bump the Android version to <version> (<code>)`, with the reason in the body.
3. If the project has `CHANGELOG.md`: rename its `## [Unreleased]` section to `## [<version>] - Unreleased` (or add a short entry when nothing user-facing changed) and commit it on its own.

## 3. PR and merge

Run `/ship-branch`. In the PR body's testing section, say which Release checks ran and their result, and any fixes and re-checks.

## 4. Release notes

After the merge, switch to the merged default branch (`git checkout master && git -c http.sslBackend=schannel pull --ff-only`) and run `/store-release-notes` for the new version, so the notes describe what was actually merged. Save the block exactly as shown, tags included, to `<scratch>/release-notes-<version>.txt`; stage 5 uploads this file. If the user asks for changes, change the file and show it again.

## 5. Upload to Google Play

1. Check the setup: the variables are set and the paths point to files (print only set/unset and exists, never values; check inside `bash -c`), and `uv` is on the PATH. If not, stop and report what is missing. Then check access, read-only: `uv run --locked $S/upload_to_play.py --show-track`. "API disabled" or a 403 is the user's to fix in Google Cloud or the Play Console.
2. Build the signed bundle on the merged default branch:
   ```bash
   bash $S/build_aab.sh                                   # add --secrets-file <path> if the project keeps real keys only locally
   ```
   It prints the bundle's path on its last line, and stops when not on the default branch, when the secrets file is the committed dummy version, or when the bundle is signed with the debug key. A build error here means the store build differs from what passed stage 1: go back to stage 1.
3. Let Google Play check everything without publishing:
   ```bash
   uv run --locked $S/upload_to_play.py --aab <bundle> --notes <scratch>/release-notes-<version>.txt --version <version> --validate-only
   ```
   It checks the notes locally (en-US and ja-JP present, ≤ 500 characters each) and that the manifest says `<version>`, then has Google Play validate an edit and throws it away. Nothing is published. Fix whatever it reports.
4. **Stop and ask the user.** Show the version and versionCode, the track (`production`), the release type (draft unless asked otherwise), the bundle size and the notes block. Say that the upload uses up this versionCode for good. Wait for a clear yes in this conversation; anything else means no upload.
5. Upload with the same arguments minus `--validate-only`. Add `--status completed` (everyone) or `--status inProgress --fraction 0.2` (staged) only when the user asked. It prints `UPLOADED: ...` on success.
   - "changesNotSentForReview" (managed publishing): nothing was published; tell the user and re-run with `--not-sent-for-review` only after they say yes.
6. If it fails, the script deletes its edit. If the connection dropped during the final commit, Google Play may have committed anyway: run `--show-track` before any retry; if the version is there, do not upload again. Retry only when the user says so.

## 6. Report

- PR link, version and code, and that the default branch has them.
- The Release check: number of runs, what was fixed between them, the folder with the final run's screenshots, what was not checked (iOS, subscriptions).
- Non-fatal problems left for later.
- The release notes block.
- The upload: track, release type and the `UPLOADED` line. For a draft, say the rollout is the user's to start in the Play Console (Release > Production). If no upload happened, say why and what is left.
