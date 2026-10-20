#!/usr/bin/env bash
set -Eeuo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
UNITY_VERSION="${UNITY_VERSION:-6000.6.5f1}"
[[ "$UNITY_VERSION" == "6000.6.5f1" ]] || { echo "Use Unity 6000.6.5f1" >&2; exit 2; }
# Supply an existing image explicitly; do not invent or silently download a tag.
: "${UNITY_IMAGE:?Set UNITY_IMAGE to a locally available Unity 6000.6.5f1 image}"
OUTPUT_DIR="${OUTPUT_DIR:-${ROOT}/artifacts/unity-ci}"
mkdir -p "${OUTPUT_DIR}"
OUTPUT_DIR="$(cd "${OUTPUT_DIR}" && pwd)"
if [[ "$(uname -s)" != Linux ]]; then
  echo "Run on a Linux x86_64 Docker host or WSL2 Linux; not Windows containers." >&2
  exit 2
fi
if [[ "$(uname -m)" != x86_64 ]]; then
  echo "Unity Linux Editor image requires linux/amd64 (x86_64)." >&2
  exit 2
fi
command -v docker >/dev/null || { echo "docker is required" >&2; exit 2; }
command -v python3 >/dev/null || { echo "python3 is required" >&2; exit 2; }
docker image inspect "$UNITY_IMAGE" >/dev/null
PROJECT="$(mktemp -d)"
trap 'rm -rf "$PROJECT"' EXIT
python3 "${ROOT}/tools/unity-ci/prepare_project.py" --package "$ROOT" --project "$PROJECT" --unity "$UNITY_VERSION" --container-package-path /workspace/package --stage-package
# Never print or store the Unity license content in logs or artifacts.
LICENSE_ARGS=()
if [[ -n "${UNITY_LICENSE_FILE:-}" ]]; then
  [[ -f "$UNITY_LICENSE_FILE" ]] || { echo "UNITY_LICENSE_FILE not found" >&2; exit 2; }
  LICENSE_ARGS=(-v "${UNITY_LICENSE_FILE}:/root/.local/share/unity3d/Unity/Unity_lic.ulf:ro")
elif [[ -n "${UNITY_LICENSE:-}" ]]; then
  echo "Use UNITY_LICENSE_FILE instead of an environment literal to avoid accidental disclosure." >&2
  exit 2
else
  echo "Unity Editor needs a valid, properly licensed activation. Set UNITY_LICENSE_FILE." >&2
  exit 2
fi
STATUS=0
for mode in EditMode PlayMode; do
  echo "Running Unity $mode suite"
  rm -f "${OUTPUT_DIR}/${mode}.xml" "${OUTPUT_DIR}/${mode}.log"
  set +e
  docker run --rm --init --platform linux/amd64 \
    --pull never \
    --network bridge \
    --mount "type=bind,src=${ROOT},dst=/workspace/package,readonly" \
    --mount "type=bind,src=${PROJECT},dst=/workspace/project" \
    --mount "type=bind,src=${OUTPUT_DIR},dst=/workspace/results" \
    "${LICENSE_ARGS[@]}" \
    --entrypoint /opt/unity/Editor/Unity "$UNITY_IMAGE" \
      -batchmode -nographics -projectPath /workspace/project \
      -runTests -testPlatform "$mode" \
      -testResults "/workspace/results/${mode}.xml" \
      -logFile "/workspace/results/${mode}.log"
  code=$?
  set -e
  if [[ $code -ne 0 || ! -s "${OUTPUT_DIR}/${mode}.xml" ]]; then
    echo "Unity $mode failed: exit=$code or missing NUnit XML" >&2
    STATUS=1
  else
    if ! python3 "${ROOT}/tools/unity-ci/validate_nunit.py" "$OUTPUT_DIR/$mode.xml" --suite "$mode"
    then
      STATUS=1
    fi
  fi
done
exit "$STATUS"
