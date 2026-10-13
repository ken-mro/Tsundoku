#!/usr/bin/env bash
# Build the signed Android App Bundle (.aab) for Google Play, from a clean Release build.
#
#   bash .claude/skills/android-play-release/scripts/build_aab.sh [--secrets-file <path>]
#
# Run from the app's repository root (the folder with its one .csproj), on the merged default
# branch. Prints the path of the .aab on its last line.
#
# --secrets-file: a file that holds dummy keys when committed and the real ones only in the working
# tree (FlagsRally: Repository/Constants.cs). The build stops if that file is unchanged, since a
# store build with the dummy keys would ship broken. Leave it out when the project has no such file.
#
# The upload key is set up once by the user (see the skill's "One-time setup") in environment
# variables, per project in .claude/settings.local.json. The passwords are passed to MSBuild as
# env:NAME, so MSBuild reads them itself and they never appear in a command line, a log or here:
#   ANDROID_UPLOAD_KEYSTORE     path to the upload keystore (.keystore / .jks), outside the repository
#   ANDROID_UPLOAD_KEY_ALIAS    key alias in it
#   ANDROID_UPLOAD_STORE_PASS   keystore password
#   ANDROID_UPLOAD_KEY_PASS     key password (often the same as the keystore password)
set -euo pipefail

SECRETS_FILE=
while [ $# -gt 0 ]; do
  case "$1" in
    --secrets-file) SECRETS_FILE=${2:?--secrets-file needs a path}; shift 2 ;;
    *) echo "usage: build_aab.sh [--secrets-file <path>]"; exit 2 ;;
  esac
done

shopt -s nullglob
PROJECTS=(*.csproj)
[ ${#PROJECTS[@]} -eq 1 ] || { echo "FAIL: run from the app's repository root (found ${#PROJECTS[@]} .csproj here)"; exit 1; }
PROJECT=${PROJECTS[0]}

# The store build must be the merged code.
DEFAULT=$(git symbolic-ref --short refs/remotes/origin/HEAD 2>/dev/null | sed 's#^origin/##' || true)
DEFAULT=${DEFAULT:-master}
[ "$(git rev-parse --abbrev-ref HEAD)" = "$DEFAULT" ] || { echo "FAIL: not on $DEFAULT; build the bundle from the merged $DEFAULT"; exit 1; }
if [ -n "$SECRETS_FILE" ] && git diff --quiet HEAD -- "$SECRETS_FILE"; then
  echo "FAIL: $SECRETS_FILE is the committed version with dummy keys; the store build needs the real keys in the working tree"; exit 1
fi

missing=()
for name in ANDROID_UPLOAD_KEYSTORE ANDROID_UPLOAD_KEY_ALIAS ANDROID_UPLOAD_STORE_PASS ANDROID_UPLOAD_KEY_PASS; do
  [ -n "${!name:-}" ] || missing+=("$name")
done
if [ ${#missing[@]} -gt 0 ]; then
  echo "FAIL: not set: ${missing[*]} (the user sets these up once; see the skill's One-time setup)"; exit 1
fi
# On Windows (Git Bash) the path may be written C:\...; check it in POSIX form, and give MSBuild,
# a Windows program, the Windows form.
KEYSTORE_POSIX=$ANDROID_UPLOAD_KEYSTORE; KEYSTORE_ARG=$ANDROID_UPLOAD_KEYSTORE
if command -v cygpath > /dev/null; then
  KEYSTORE_POSIX=$(cygpath -u "$ANDROID_UPLOAD_KEYSTORE"); KEYSTORE_ARG=$(cygpath -w "$ANDROID_UPLOAD_KEYSTORE")
fi
[ -f "$KEYSTORE_POSIX" ] || { echo "FAIL: ANDROID_UPLOAD_KEYSTORE does not point to a file"; exit 1; }
case "$(cd "$(dirname "$KEYSTORE_POSIX")" && pwd)/" in
  "$(pwd)/"*) echo "FAIL: the keystore is inside the repository; keep it outside so it can never be committed"; exit 1 ;;
esac

TFM=$(grep -oE 'net[0-9.]+-android' "$PROJECT" | head -1)
[ -n "$TFM" ] || { echo "FAIL: no netX-android target in $PROJECT"; exit 1; }
OUT=bin/Release/$TFM/publish
rm -rf "$OUT"
LOG=$(mktemp -t android-aab.XXXXXX)
if ! dotnet publish "$PROJECT" -f "$TFM" -c Release \
    -p:AndroidPackageFormat=aab \
    -p:AndroidKeyStore=true \
    -p:AndroidSigningKeyStore="$KEYSTORE_ARG" \
    -p:AndroidSigningKeyAlias="$ANDROID_UPLOAD_KEY_ALIAS" \
    -p:AndroidSigningStorePass=env:ANDROID_UPLOAD_STORE_PASS \
    -p:AndroidSigningKeyPass=env:ANDROID_UPLOAD_KEY_PASS \
    > "$LOG" 2>&1; then
  grep -E " error " "$LOG" | sort -u | head -20
  echo "FAIL: the bundle build failed (full log: $LOG)"; exit 1
fi

AAB=$(ls "$OUT"/*-Signed.aab 2>/dev/null | head -1)
[ -n "$AAB" ] || { echo "FAIL: no signed .aab in $OUT (log: $LOG)"; exit 1; }
# The upload key, not the debug key: Google Play rejects a bundle signed with the debug keystore.
# A JDK's jarsigner that actually runs. macOS's /usr/bin/jarsigner is a stub that fails without a
# system Java, which would make a correctly signed bundle look unsigned, so known JDKs come first:
# the Android build's own JDK (Mac), JAVA_HOME, then the usual Windows places.
JARSIGNER=
for candidate in "$HOME"/Library/Developer/Android/jdk/*/bin/jarsigner \
                 "${JAVA_HOME:-/nonexistent}"/bin/jarsigner "${JAVA_HOME:-/nonexistent}"/bin/jarsigner.exe \
                 "/c/Program Files/Microsoft"/jdk-*/bin/jarsigner.exe \
                 "/c/Program Files/Android/Android Studio/jbr/bin/jarsigner.exe" \
                 "/c/Program Files/Eclipse Adoptium"/jdk-*/bin/jarsigner.exe \
                 jarsigner; do
  if "$candidate" -help > /dev/null 2>&1; then JARSIGNER=$candidate; break; fi
done
[ -n "$JARSIGNER" ] || { echo "FAIL: no working jarsigner, so the signature cannot be checked"; exit 1; }
SIGNER=$("$JARSIGNER" -verify -verbose -certs "$AAB" 2>/dev/null | grep -m1 -o "CN=[^,]*" || true)
[ -n "$SIGNER" ] || { echo "FAIL: $AAB is not signed"; exit 1; }
if [ "$SIGNER" = "CN=Android Debug" ]; then
  echo "FAIL: $AAB is signed with the debug key"; exit 1
fi
echo "bundle built: $(du -h "$AAB" | cut -f1), signed by $SIGNER"
echo "$AAB"
