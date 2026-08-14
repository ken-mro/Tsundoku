---
name: upgrading-dotnet-maui
description: Use when upgrading this app to a new .NET/MAUI version or bumping any NuGet package across a major version (e.g. net10→net11, CommunityToolkit.Maui, RevenueCat wrapper, ZXing). Also use when a UI or billing regression appears right after a version bump.
---

# Upgrading .NET / MAUI and Major Package Versions

## Overview

**A clean build is not a passing test for an upgrade.** Major package bumps change *runtime behavior* without breaking compilation. The .NET 10 upgrade compiled with 0 warnings, yet CommunityToolkit.Maui's Popup rewrite silently turned the paywall bottom sheet into a full-screen page. Treat every major bump as a behavioral migration, not a version edit.

## Process

1. **Capture a pre-upgrade baseline.** Before touching versions, run the app and screenshot every screen that uses the packages being bumped (popups, camera, paywall). You cannot spot a behavioral regression without a "before".
2. **Inventory the bumps.** List every package old→new version; flag major bumps. Each major bump gets steps 3–4 individually.
3. **Enumerate breaking changes — including behavioral ones.** Read release notes and migration guides. When docs are thin (wrapper packages like Kebechet.Maui.RevenueCat), diff the API surface yourself: reflect over the old and new assemblies in `~/.nuget/packages/<id>/<ver>/lib/` (PowerShell `Assembly.LoadFile` + `GetTypes`), or decompile with `ilspycmd`. "No docs" never means "no breaking changes".
4. **Audit every usage site.** Grep the repo for the package's namespaces/types and review each site against the breaking-change list — especially code that still compiles but behaves differently.
5. **Build each TFM** (`net10.0`, `net10.0-android`, …). Keep MAUI TFMs **unversioned** (`net10.0-android`, never `net10.0-android36.0`) — versioned TFMs break Visual Studio deploy/debug, and targetSdk comes from the SDK anyway.
6. **Verify at runtime against the step-1 baseline, screen by screen.** "The popup opens" is not verification — compare position, size, and data (is the price text populated?) to the before-screenshots.
7. **Check store requirements from the build output**, not the source: the merged manifest at `obj/Debug/<android-tfm>/android/AndroidManifest.xml` shows the real `targetSdkVersion`/`minSdkVersion`/`versionCode`.

## Known Gotchas in This Repo

| Symptom | Cause / Fix |
|---|---|
| Popup fills screen | CTK.Maui Popup v2: `ScrollView` content expands popup to full screen — use auto-sized layout or explicit `HeightRequest` |
| Popup not flush with screen edge | Popup v2 treats `Margin` of all-zero `Thickness` as *unset* → default 30. Use e.g. `new Thickness(0, 1, 0, 0)` |
| Popup position ignored | Popup v2 positions via `HorizontalOptions`/`VerticalOptions` on the Popup + `PopupOptions` in `ShowPopupAsync`; `Size` became `HeightRequest`/`WidthRequest` |
| F5 Android debug fails | Versioned Android TFM in `TargetFrameworks` — keep it unversioned |
| XAML error on ZXing view | `xmlns:zxing` assembly name/casing must match the package exactly |
| "Not configured for billing" + missing prices | Sideloaded build unknown to Google Play — add device account as license tester in Play Console; not a code bug |
| Play target-API warning | `SupportedOSPlatformVersion` is **minSdk**, not targetSdk; targetSdk = SDK's bound API level — verify in merged manifest |

## Red Flags — the upgrade is NOT done if you're thinking:

- "Build passed with 0 warnings, so the upgrade is complete"
- "The screen opens, so that feature works"
- "This package has no migration guide, so nothing to migrate"
- "That package is minor, skip its release notes"

All of these mean: return to steps 3–6.
