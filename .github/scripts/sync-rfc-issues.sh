#!/usr/bin/env bash
set -euo pipefail

REPO="${REPO:-${GITHUB_REPOSITORY:-}}"
if [[ -z "$REPO" ]]; then
  echo "REPO is not set; defaulting to current repo from GH context" >&2
  REPO='${{ github.repository }}'
fi

if ! command -v gh >/dev/null 2>&1; then
  echo "gh CLI not available" >&2
  exit 1
fi

if [[ -z "${GH_TOKEN:-}" ]]; then
  echo "GH_TOKEN not set; requires GitHub App/Action token" >&2
  exit 1
fi

RFC_DIR="docs/RFC"
if [[ ! -d "$RFC_DIR" ]]; then
  echo "No RFC directory found at $RFC_DIR; nothing to do" >&2
  exit 0
fi

trim() { sed -e 's/^\s\+//' -e 's/\s\+$//' ; }

# Collect RFC metadata
mapfile -t RFC_FILES < <(find "$RFC_DIR" -maxdepth 1 -type f -name 'RFC*.md' | sort)

get_rfc_number() { # file -> ###
  local base; base="$(basename "$1")"
  [[ "$base" =~ RFC([0-9]{3,}) ]] && echo "${BASH_REMATCH[1]}" || echo ""
}

get_title() { # file -> title
  local t; t="$(grep -m1 '^# ' "$1" | sed 's/^# \s*//')"
  [[ -n "$t" ]] && echo "$t" || echo "$(basename "$1" .md)"
}

get_status() { # file -> status value
  grep -m1 -E '^- \*\*Status\*\*:' "$1" | cut -d':' -f2- | trim || true
}

ensure_activation_once() { # existingBody, message -> newBody
  local existing="$1"; shift
  local msg="$1"
  local start='<!-- copilot-activation:start -->'
  local end='<!-- copilot-activation:end -->'
  # Remove any existing activation block (idempotent)
  existing="$(awk -v RS= -v ORS= '{gsub(/<!-- copilot-activation:start -->[\s\S]*?<!-- copilot-activation:end -->/, ""); print}' <<< "$existing")"
  printf "%s\n%s\n%s\n%s\n" "$existing" "$start" "$msg" "$end"
}

in_array() { local n=$1; shift; for e in "$@"; do [[ "$e" == "$n" ]] && return 0; done; return 1; }

create_or_update_issue() {
  local num="$1" title="$2" status="$3"
  local issue_title="RFC${num}: ${title} — Implementation"
  local marker="<!-- rfc-id: ${num} -->"
  local searchQ="label:rfc-implementation in:body \"${marker}\" repo:${REPO} state:open"
  local foundJson number existing curTitle curBody
  foundJson="$(gh search issues --json number -q 'items[0]' -- "$searchQ" || true)"
  number="$(jq -r '.number // empty' <<< "$foundJson" 2>/dev/null || true)"

  local activation_msg="@copilot Please begin implementing RFC${num}. Follow the RFC acceptance criteria and open a PR linked to this issue."

  # If RFC is marked Complete, close existing issue (if any) and skip creation
  if [[ "$status" =~ [Cc]omplete ]]; then
    if [[ -n "$number" ]]; then
      gh issue close "$number" --repo "$REPO" --comment "RFC marked Complete; closing implementation issue." >/dev/null || true
      CLOSED=$((CLOSED+1))
    fi
    return 0
  fi

  if [[ -z "$number" ]]; then
    local body
    body=$(cat <<EOF
${marker}
### RFC
- File: docs/RFC/RFC${num}*.md
- Title: ${title}
- Status: ${status}

### Implementation Notes
- This issue tracks implementation work for RFC${num}.
- Keep PRs titled with "RFC${num}" to enable auto-linking.

<!-- copilot-activation:start -->
${activation_msg}
<!-- copilot-activation:end -->
EOF
)
    gh issue create --repo "$REPO" --title "$issue_title" --label rfc-implementation --body "$body" >/dev/null
    CREATED=$((CREATED+1))
  else
    existing="$(gh issue view "$number" --repo "$REPO" --json title,body -q '{t:.title,b:.body}')"
    curTitle="$(jq -r '.t' <<< "$existing")"
    curBody="$(jq -r '.b' <<< "$existing")"

    # Ensure marker present
    if ! grep -q "${marker}" <<< "$curBody"; then
      curBody="$marker
$curBody"
    fi

    # Replace or append activation block
    local newBody
    newBody="$(ensure_activation_once "$curBody" "$activation_msg")"

    # Update title if changed
    if [[ "$curTitle" != "$issue_title" ]]; then
      gh issue edit "$number" --repo "$REPO" --title "$issue_title" >/dev/null || true
      UPDATED=$((UPDATED+1))
    fi

    # Update body if changed
    if [[ "$newBody" != "$curBody" ]]; then
      gh issue edit "$number" --repo "$REPO" --body "$newBody" >/dev/null || true
      UPDATED=$((UPDATED+1))
    fi
  fi
}

CREATED=0; UPDATED=0; CLOSED=0; REMOVED_CLOSED=0

# Build a set of active RFC numbers from files
mapfile -t RFC_NUMS < <(for f in "${RFC_FILES[@]}"; do n=$(get_rfc_number "$f"); [[ -n "$n" ]] && echo "$n"; done)

# Process each RFC file
for f in "${RFC_FILES[@]}"; do
  n="$(get_rfc_number "$f")"; [[ -z "$n" ]] && continue
  t="$(get_title "$f")"
  s="$(get_status "$f")"
  create_or_update_issue "$n" "$t" "$s"
done

# Close issues for RFCs that no longer exist
openIssuesJson="$(gh issue list --repo "$REPO" --label rfc-implementation --state open --limit 200 --json number,body,title)"
count=$(jq 'length' <<< "$openIssuesJson")
for i in $(seq 0 $((count-1))); do
  body="$(jq -r ".[${i}].body" <<< "$openIssuesJson")"
  if [[ "$body" =~ <!--[[:space:]]rfc-id:[[:space:]]([0-9]{3,})[[:space:]]--> ]]; then
    id="${BASH_REMATCH[1]}"
    if ! in_array "$id" "${RFC_NUMS[@]}"; then
      num="$(jq -r ".[${i}].number" <<< "$openIssuesJson")"
      gh issue close "$num" --repo "$REPO" --comment "RFC${id} file removed; closing implementation issue." >/dev/null || true
      REMOVED_CLOSED=$((REMOVED_CLOSED+1))
    fi
  fi
done

# Summary
if [[ -n "${GITHUB_STEP_SUMMARY:-}" ]]; then
  {
    echo "## RFC Issue Sync"
    echo "- Created: $CREATED"
    echo "- Updated: $UPDATED"
    echo "- Closed (Complete): $CLOSED"
    echo "- Closed (Removed): $REMOVED_CLOSED"
  } >> "$GITHUB_STEP_SUMMARY"
else
  echo "RFC Issue Sync — Created:$CREATED Updated:$UPDATED Closed(Complete):$CLOSED Closed(Removed):$REMOVED_CLOSED" >&2
fi