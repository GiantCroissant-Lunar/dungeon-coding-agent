#!/bin/bash
set -euo pipefail

# GitHub Copilot RFC Agent Preparation Script
# Usage: prepare-copilot-agents.sh [--force] [--issues "1,2,3"] [--max-capacity N]

FORCE_SPAWN="${1:-false}"
ISSUE_LIST="${2:-auto}"
MAX_CAPACITY="${3:-3}"

echo "🤖 Starting Copilot agent preparation..."
echo "Force spawn: $FORCE_SPAWN"
echo "Issue list: $ISSUE_LIST" 
echo "Max capacity: $MAX_CAPACITY"

# Check global capacity (max active Copilot agents)
ACTIVE_COUNT=$(gh issue list --state open --label "copilot-working" --json number --jq 'length')
AVAILABLE_SLOTS=$((MAX_CAPACITY - ACTIVE_COUNT))

echo "Active Copilot agents: $ACTIVE_COUNT/$MAX_CAPACITY"
echo "Available slots: $AVAILABLE_SLOTS"

if [ "$AVAILABLE_SLOTS" -le 0 ]; then
  echo "🚫 No available capacity for new Copilot agents"
  exit 0
fi

# Determine which issues to process
if [ "$ISSUE_LIST" == "auto" ]; then
  # Auto-discovery: find RFC issues ready for implementation
  CANDIDATE_ISSUES=$(gh issue list \
    --state open \
    --label "rfc-implementation" \
    --label "agent-assigned" \
    --json number,labels --jq '
      map(select(
        (.labels | map(.name) | contains(["rfc-implementation", "agent-assigned"])) and
        (.labels | map(.name) | contains(["copilot-working"]) | not)
      )) | .[].number
    ' | tr '\n' ',' | sed 's/,$//')
else
  CANDIDATE_ISSUES="$ISSUE_LIST"
fi

echo "Candidate issues: $CANDIDATE_ISSUES"

if [ -z "$CANDIDATE_ISSUES" ]; then
  echo "ℹ️ No RFC issues ready for agent preparation"
  exit 0
fi

IFS=',' read -ra ISSUE_ARRAY <<< "$CANDIDATE_ISSUES"
SPAWNED_COUNT=0

for issue_num in "${ISSUE_ARRAY[@]}"; do
  issue_num=$(echo "$issue_num" | xargs)  # trim whitespace
  [ -z "$issue_num" ] && continue
  
  if [ "$SPAWNED_COUNT" -ge "$AVAILABLE_SLOTS" ]; then
    echo "🛑 Capacity limit reached. Stopping agent spawn."
    break
  fi
  
  echo "🎯 Processing issue #$issue_num"
  
  # Get issue details
  ISSUE_DATA=$(gh issue view "$issue_num" --json title,labels,assignees)
  ISSUE_TITLE=$(echo "$ISSUE_DATA" | jq -r '.title')
  
  # Extract RFC number
  RFC_NUM=$(echo "$ISSUE_TITLE" | grep -o 'RFC[0-9]\{3\}' | head -1 || echo "")
  if [ -z "$RFC_NUM" ]; then
    echo "⚠️ Could not extract RFC number from: $ISSUE_TITLE"
    continue
  fi
  
  # Check if already working (unless forced)
  HAS_WORKING_LABEL=$(echo "$ISSUE_DATA" | jq -r '.labels[].name' | grep -q "copilot-working" && echo "true" || echo "false")
  if [ "$HAS_WORKING_LABEL" == "true" ] && [ "$FORCE_SPAWN" != "true" ]; then
    echo "⏭️ Skipping #$issue_num - already has copilot-working label"
    continue
  fi
  
  # Find RFC file
  RFC_FILE=$(find docs/RFC/ -name "$RFC_NUM-*.md" | head -1 || echo "")
  if [ -z "$RFC_FILE" ]; then
    echo "⚠️ RFC file not found for $RFC_NUM"
    continue
  fi
  
  echo "🚀 Preparing issue for Copilot agent: #$issue_num ($RFC_NUM)"
  
  # Add copilot-working label
  gh issue edit "$issue_num" --add-label "copilot-working"
  
  # Create enhanced preparation comment with RFC context
  RFC_SUMMARY=$(head -20 "$RFC_FILE" | grep -E "^## " -A 3 | head -10 || echo "See RFC for full details")
  
  cat > /tmp/copilot_preparation_comment.md << 'EOF'
# GitHub Copilot Agent Ready - $RFC_NUM Implementation

This issue is now prepared for GitHub Copilot coding agent implementation.

## RFC Overview
$RFC_SUMMARY

**Full RFC Specification**: `$RFC_FILE`

## Implementation Mission
When a coding agent is assigned to this issue, please:

1. **Study the complete RFC** - Read $RFC_FILE thoroughly
2. **Create feature branch** - Use naming: feature/$RFC_NUM-implementation  
3. **Follow architecture patterns** - Arch ECS + Terminal.Gui v2 as specified in AGENTS.md
4. **Implement ALL acceptance criteria** - Complete every checkbox in the RFC
5. **Write comprehensive tests** - Achieve >80% coverage
6. **Create pull request** - With detailed implementation description

## Technical Architecture (Critical)

### ECS Pattern Requirements:
- **Components**: Pure data structs only (no methods)
- **Systems**: Logic classes inheriting `SystemBase<World, float>`
- **Event Communication**: Use `GameEvents.RaiseXXX()` for inter-system messaging

### File Organization:
```
src/DungeonCodingAgent.Game/
|-- Components/     # Data structures ($RFC_NUM components)
|-- Systems/        # Game logic ($RFC_NUM systems) 
|-- UI/            # Terminal.Gui views ($RFC_NUM UI)
|-- Core/          # Events, utilities ($RFC_NUM events)
\-- Generation/    # Content generation (if applicable)
```

## Definition of Done
- [ ] All RFC acceptance criteria checkboxes completed
- [ ] Unit tests written and passing (>80% coverage)
- [ ] Integration tests verify compatibility with existing systems
- [ ] Code follows established architectural patterns  
- [ ] Terminal.Gui integration working (if UI components)
- [ ] Event system integration complete (if applicable)
- [ ] Pull request created with comprehensive description
- [ ] No build warnings or errors

## Essential Resources
- **Architecture Guidelines**: AGENTS.md and .github/copilot-instructions.md
- **RFC Specification**: $RFC_FILE (complete implementation spec)
- **Existing Patterns**: Study current codebase for established conventions
- **Dependencies**: Terminal.Gui 2.0.0, Arch 2.0.0, xUnit testing

## Ready for Copilot Agent Assignment

This issue is fully prepared with comprehensive guidance. A Copilot coding agent can now be assigned via the GitHub UI or Copilot Chat to implement the RFC following all specified patterns and requirements.

---
*Auto-prepared by RFC automation system*
EOF

  # Substitute variables in the template
  sed -i "s/\$RFC_NUM/$RFC_NUM/g" /tmp/copilot_preparation_comment.md
  sed -i "s|\$RFC_FILE|$RFC_FILE|g" /tmp/copilot_preparation_comment.md
  sed -i "s/\$RFC_SUMMARY/$RFC_SUMMARY/g" /tmp/copilot_preparation_comment.md
  
  # Post the preparation comment
  gh issue comment "$issue_num" --body-file /tmp/copilot_preparation_comment.md
  
  echo "✅ Prepared issue #$issue_num ($RFC_NUM) for Copilot agent"
  SPAWNED_COUNT=$((SPAWNED_COUNT + 1))
  
  # Brief delay to avoid rate limiting
  sleep 2
done

echo "🎉 Successfully prepared $SPAWNED_COUNT issue(s) for Copilot agents"