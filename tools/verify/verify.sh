#!/bin/bash
# Verifies the app without device toolchains (no Android SDK / Xcode needed):
#   1. Builds the platform-neutral net10.0 head (C# + XAML compilation).
#   2. Static XAML checks (resource keys, binding paths, image references).
#   3. Smoke test: inflates every page/popup and its item templates with sample data.
#   4. Unit tests (offline ones; Category=Network is filtered out).
#   5. Runs MAUI Resizetizer for Android so app icon, splash and images are rendered.
#      Output lands in obj/Debug/net10.0-android/resizetizer for visual inspection.
# Usage: tools/verify/verify.sh            (from anywhere)
set -uo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
# Only net10.0(-android) are buildable on Linux; TsundokuNeutralOnly drops the
# iOS/MacCatalyst heads that would otherwise fail restore.
NEUTRAL="-p:TsundokuNeutralOnly=true"

step() { printf '\n== %s\n' "$1"; }
fail() { echo "FAILED: $1"; exit 1; }

step "Build (net10.0)"
out=$(dotnet build Tsundoku.csproj $NEUTRAL -f net10.0 -v:q -nologo 2>&1)
echo "$out" | grep -E " (error|warning) " | sort -u
echo "$out" | grep -q "Build succeeded" || fail "build"
echo "$out" | grep -qE " error " && fail "build"
echo "Build succeeded"

step "Static XAML checks"
python3 tools/verify/check_xaml.py || fail "XAML checks"

step "Smoke test"
out=$(dotnet build tools/verify/Smoke/Smoke.csproj $NEUTRAL -v:q -nologo 2>&1)
echo "$out" | grep -q "Build succeeded" || { echo "$out" | grep -E " error " | sort -u; fail "smoke build"; }
dotnet tools/verify/Smoke/bin/Debug/net10.0/Smoke.dll || fail "smoke test"

step "Unit tests"
# Network-dependent tests (Category=Network) need outbound access to Amazon and are skipped here.
dotnet test Tsundoku.xUnitTest/Tsundoku.xUnitTest/Tsundoku.xUnitTest.csproj $NEUTRAL --filter "Category!=Network" -v:q -nologo || fail "unit tests"

step "Resizetizer (Android icons, splash, images)"
fake_sdk=$(mktemp -d)
rm -rf obj/Debug/net10.0-android/resizetizer
dotnet restore Tsundoku.csproj -p:TargetFrameworks=net10.0-android -v:q >/dev/null || fail "android restore"
out=$(dotnet msbuild Tsundoku.csproj -p:TargetFrameworks=net10.0-android -p:TargetFramework=net10.0-android \
    -p:AndroidSdkDirectory="$fake_sdk" -t:ResizetizeImages -v:q -nologo 2>&1)
rmdir "$fake_sdk"
echo "$out" | grep -iE "error|warn" && fail "resizetizer"
echo "Resizetizer OK"
# Leave the assets file pointing at net10.0 so IDE/test builds keep working.
dotnet restore Tsundoku.csproj $NEUTRAL -v:q >/dev/null

printf '\nAll checks passed.\n'
