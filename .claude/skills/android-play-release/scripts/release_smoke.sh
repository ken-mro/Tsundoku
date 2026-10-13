#!/usr/bin/env bash
# Release smoke test on the Android emulator: a clean Release build, installed over the current
# app (data kept), launched, then logcat is searched for crashes.
#
#   bash .claude/skills/android-play-release/scripts/release_smoke.sh <out-dir> [--no-build]
#
# Run from the app's repository root (the folder with its one .csproj). Exit code 0 = launched
# and no fatal line found, 1 = something to look at (details in <out-dir>/report.txt). A launch
# screenshot goes to <out-dir>/01_launch.png; the app is left running so Claude can go on to open
# every tab or page by hand (adb shell input tap X Y, then adb exec-out screencap -p > file).
# FlagsRally 1.1.20 passed every Debug check and still crashed on launch from Google Play: only a
# Release build shows what R8 / trimming / AOT remove, so this always builds Release from clean.
set -uo pipefail
export MSYS_NO_PATHCONV=1

OUT=${1:?usage: release_smoke.sh <out-dir> [--no-build]}
BUILD=1; [ "${2:-}" = "--no-build" ] && BUILD=0
mkdir -p "$OUT"
REPORT="$OUT/report.txt"; : > "$REPORT"
log() { echo "$*" | tee -a "$REPORT"; }

shopt -s nullglob
PROJECTS=(*.csproj)
[ ${#PROJECTS[@]} -eq 1 ] || { log "FAIL: run from the app's repository root (found ${#PROJECTS[@]} .csproj here)"; exit 1; }
PROJECT=${PROJECTS[0]}
PKG=$(grep -oE '<ApplicationId>[^<]+' "$PROJECT" | head -1 | sed 's/<ApplicationId>//')
TFM=$(grep -oE 'net[0-9.]+-android' "$PROJECT" | head -1)
[ -n "$PKG" ] && [ -n "$TFM" ] || { log "FAIL: ApplicationId or netX-android target not found in $PROJECT"; exit 1; }

if [ -z "${ANDROID_SERIAL:-}" ]; then
  ANDROID_SERIAL=$(adb devices | awk '/^emulator-[0-9]+	device$/ { print $1; exit }')
  [ -n "$ANDROID_SERIAL" ] || { log "FAIL: no emulator running"; exit 1; }
fi
export ANDROID_SERIAL

if [ "$BUILD" = 1 ]; then
  log "== Clean Release build ($PROJECT, $TFM)"
  if ! dotnet build "$PROJECT" -f "$TFM" -c Release -t:Rebuild > "$OUT/build.log" 2>&1; then
    grep -E " error " "$OUT/build.log" | sort -u | head -20 | tee -a "$REPORT"
    log "FAIL: Release build failed (see build.log)"; exit 1
  fi
  log "build succeeded"
fi

# By name: the APK's timestamps are fixed (1981) for reproducible builds, so "newest" means nothing.
APK=bin/Release/$TFM/$PKG-Signed.apk
[ -f "$APK" ] || { log "FAIL: $APK not found"; exit 1; }
log "== Install $APK"
adb install -r "$APK" >> "$REPORT" 2>&1 || { log "FAIL: install failed (a different signature? see report; uninstalling loses the app's data, so ask the user first)"; exit 1; }

in_front() { adb shell dumpsys window | grep -q "mCurrentFocus.*$PKG"; }
alive() { [ -n "$(adb shell pidof "$PKG" | tr -d '\r')" ]; }

log "== Launch $PKG"
adb shell setprop debug.mono.extra "''"
adb shell am force-stop "$PKG"
adb logcat -c
adb shell am start -n "$(adb shell cmd package resolve-activity --brief -c android.intent.category.LAUNCHER "$PKG" | tail -1 | tr -d '\r')" > /dev/null
for _ in $(seq 1 12); do sleep 5; in_front && break; done
sleep 8   # startup work (subscriptions, database) runs after the first frame
adb exec-out screencap -p > "$OUT/01_launch.png"

adb logcat -d > "$OUT/logcat.txt"
PROBLEMS=0
if ! alive; then log "FAIL: the app is no longer running"; PROBLEMS=1; fi
if ! in_front; then log "FAIL: the app is not in front (crashed, or a system dialog such as 'isn't responding')"; PROBLEMS=1; fi
APP_PIDS=$(grep -oE "Start proc [0-9]+:$PKG" "$OUT/logcat.txt" | grep -oE "[0-9]+" | sort -u | tr '\n' '|' | sed 's/|$//')
# A line counts when it comes from the app's process (logcat's third column) or names the
# package (ANRs are reported by the system process).
grep -E "FATAL EXCEPTION|AndroidRuntime: Process: $PKG|UNHANDLED EXCEPTION|ANR in $PKG|Main dispatcher is missing|Unable to start activity|java.lang.(NoClassDefFound|ClassNotFound|UnsatisfiedLink)Error|System.(TypeLoad|MissingMethod|EntryPointNotFound)Exception" "$OUT/logcat.txt" \
  | awk -v pids="|${APP_PIDS}|" -v pkg="$PKG" 'index(pids, "|" $3 "|") || index($0, pkg)' > "$OUT/fatal.txt"
if [ -s "$OUT/fatal.txt" ]; then
  log "FAIL: fatal lines in logcat (first 15, all in fatal.txt):"; head -15 "$OUT/fatal.txt" | tee -a "$REPORT"; PROBLEMS=1
fi
[ -n "$APP_PIDS" ] && log "app process ids: $APP_PIDS"

if [ "$PROBLEMS" = 0 ]; then log "OK: launched, no fatal lines in logcat; now open every tab and the changed screens"; fi
log "screenshot and logcat.txt: $OUT"
exit $PROBLEMS
