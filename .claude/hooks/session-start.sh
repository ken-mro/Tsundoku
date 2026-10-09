#!/bin/bash
# Claude Code cloud sessions: install the .NET 10 SDK and the MAUI workloads that
# tools/verify/verify.sh needs. Idempotent; skipped on local machines.
set -euo pipefail

if [ "${CLAUDE_CODE_REMOTE:-}" != "true" ]; then
  exit 0
fi

if ! command -v dotnet >/dev/null 2>&1 || ! dotnet --list-sdks | grep -q '^10\.'; then
  # dot.net install scripts are not reachable from the sandbox; Ubuntu's archive is.
  export DEBIAN_FRONTEND=noninteractive
  apt-get update -qq
  apt-get install -y -qq dotnet-sdk-10.0 >/dev/null
fi

# net10.0 builds of a UseMaui project need the MAUI workload manifests;
# maui-android also provides Resizetizer for the icon/splash checks.
installed=$(dotnet workload list 2>/dev/null || true)
for w in maui-android maui-tizen; do
  if ! grep -q "^$w " <<<"$installed"; then
    dotnet workload install "$w" >/dev/null
  fi
done

cd "$CLAUDE_PROJECT_DIR"
dotnet restore Tsundoku.csproj -p:TsundokuNeutralOnly=true -v:q >/dev/null
dotnet restore tools/verify/Smoke/Smoke.csproj -p:TsundokuNeutralOnly=true -v:q >/dev/null
