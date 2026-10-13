---
name: ship-branch
description: Verify, push, open and merge a pull request for the current branch into master, merging any open PR it is built on first. Use only when the user asks to push, open a PR, merge, or "PR して Merge して".
---

# Ship a branch

Pushing, opening and merging are done only when the user asks in this conversation. Committing to the default branch directly is not done: branch first (`git checkout -b <type>/<short-name>`, e.g. `fix/scanner-reopen`, `chore/bump-android-1.0.8`).

**On Windows**, network git commands use `git -c http.sslBackend=schannel ...` (push, fetch, pull); without it they can fail on certificate checks.

1. **Project rules first.** Read the repository's CLAUDE.md: its verification command (e.g. `tools/verify/verify.sh`), commit rules (one change per commit, English imperative subject, reason in the body), and any changelog or secret-file rule. Those override the defaults below.
2. **Open PRs:** `gh pr list --state open --json number,title,headRefName,baseRefName,mergeable`. If this branch is built on another open PR's branch (`git merge-base --is-ancestor origin/<that-branch> HEAD`), merge that PR first.
3. **Commits:** `git status --short` must not leave the change half-committed. Do not commit build output (`bin/`, `obj/`, `.apk`, `.aab`, `.ipa`) or local-only files (`*.user`, `settings.local.json`); leave untracked artifacts alone and mention them.
4. **Nothing secret goes out:** `git diff origin/<default>...HEAD` must not contain keystores, passwords, service-account JSON or API keys that are not meant to be public. A secrets file the project keeps uncommitted (for example a `Constants.cs` with real keys in the working tree) must not appear in `git diff --name-only origin/<default>...HEAD`.
5. **Verify:** run the CLAUDE.md verification command and read its result. No command named → `dotnet build` and `dotnet test` of the test project. A failure stops the ship; fix it in its own commit or report it.
6. **Changelog:** only if the project has `CHANGELOG.md` (or CLAUDE.md asks for one): the branch must update it.
7. **Conflicts:** `git -c http.sslBackend=schannel fetch origin`, then `git merge-tree --write-tree origin/<default> HEAD` must report no conflict.
8. **Push:** `git -c http.sslBackend=schannel push -u origin <branch>`.
9. **PR:** write the body to a scratchpad file, then `gh pr create --base <default> --head <branch> --title "<imperative summary>" --body-file <file>`. Body sections:
   - what changed, grouped by area
   - security / performance notes, if any
   - testing: what the verification ran and its result, what was checked on a device, what was not (e.g. iOS)
   - the attribution line the session asks for (e.g. `🤖 Generated with [Claude Code](https://claude.com/claude-code)`)
10. **Merge:** `gh pr view <n> --json mergeable,mergeStateStatus` should be `MERGEABLE` / `CLEAN`. Then `gh pr merge <n> --merge` (merge commits, as in earlier PRs). Then `git checkout <default> && git -c http.sslBackend=schannel pull --ff-only`.
11. **Report:** PR URL(s), what was merged in which order, the verification result, and anything left (untracked files, unchecked platforms).
