#!/bin/bash
set -euo pipefail

# RFC Progress Monitoring Script
# Tracks RFC lifecycle: Created → Assigned → In Progress → PR Created → Merged → Closed

echo "🔍 RFC Progress Monitoring Report"
echo "Generated: $(date -u)"
echo "=================================="

# Analyze RFC files vs Issues vs PRs
echo ""
echo "## 📋 RFC Documentation Status"

RFC_FILES=$(find docs/RFC/ -name "RFC[0-9]*-*.md" 2>/dev/null | wc -l)
echo "Total RFC Documentation Files: $RFC_FILES"

echo ""
echo "### RFC Files Found:"
find docs/RFC/ -name "RFC[0-9]*-*.md" 2>/dev/null | sort | while read rfc_file; do
    RFC_NUM=$(basename "$rfc_file" | grep -o "RFC[0-9]\{3\}" | head -1 || echo "UNKNOWN")
    RFC_TITLE=$(basename "$rfc_file" .md | sed 's/RFC[0-9]\{3\}-//')
    echo "  ✅ $RFC_NUM: $RFC_TITLE"
done

echo ""
echo "## 🎯 RFC Implementation Status"

# Get all RFC-related issues
RFC_ISSUES=$(gh issue list --state all --json number,title,state,labels,createdAt,updatedAt,closedAt --jq '
[.[] | select(.labels[].name | contains("rfc-implementation")) | 
{
    number: .number,
    title: .title,
    state: .state,
    rfc_num: (.title | capture("RFC(?<num>[0-9]+)"; "g").num // "UNKNOWN"),
    labels: [.labels[].name],
    created: .createdAt,
    updated: .updatedAt,
    closed: .closedAt,
    has_agent: (.labels | map(select(. == "agent-assigned")) | length > 0),
    has_copilot: (.labels | map(select(. == "copilot-working")) | length > 0),
    in_progress: (.labels | map(select(. == "in-progress")) | length > 0)
}] | sort_by(.rfc_num)')

# Get all RFC-related PRs
RFC_PRS=$(gh pr list --state all --json number,title,state,createdAt,updatedAt,closedAt,mergedAt --jq '
[.[] | select(.title | contains("RFC")) | 
{
    number: .number,
    title: .title,
    state: .state,
    rfc_num: (.title | capture("RFC(?<num>[0-9]+)"; "g").num // "UNKNOWN"),
    created: .createdAt,
    updated: .updatedAt,
    closed: .closedAt,
    merged: .mergedAt
}] | sort_by(.rfc_num)')

echo "### Implementation Progress by RFC:"

# Create comprehensive status for each RFC
for i in $(seq -w 1 14); do
    RFC_NUM="RFC$(printf "%03d" $i)"
    
    # Check if RFC file exists
    RFC_FILE=$(find docs/RFC/ -name "${RFC_NUM}-*.md" | head -1 || echo "")
    if [ -z "$RFC_FILE" ]; then
        continue
    fi
    
    RFC_TITLE=$(basename "$RFC_FILE" .md | sed "s/${RFC_NUM}-//")
    
    # Find related issue(s)
    ISSUE_INFO=$(echo "$RFC_ISSUES" | jq -r --arg rfc "$i" '
        map(select(.rfc_num == $rfc)) | 
        if length > 0 then 
            .[0] | "\(.number)|\(.state)|\(.has_agent)|\(.has_copilot)|\(.in_progress)|\(.created)"
        else 
            "NONE|NONE|false|false|false|NONE"
        end')
    
    IFS='|' read -r ISSUE_NUM ISSUE_STATE HAS_AGENT HAS_COPILOT IN_PROGRESS ISSUE_CREATED <<< "$ISSUE_INFO"
    
    # Find related PR(s)
    PR_INFO=$(echo "$RFC_PRS" | jq -r --arg rfc "$i" '
        map(select(.rfc_num == $rfc)) | 
        if length > 0 then 
            .[0] | "\(.number)|\(.state)|\(.merged)"
        else 
            "NONE|NONE|NONE"
        end')
    
    IFS='|' read -r PR_NUM PR_STATE PR_MERGED <<< "$PR_INFO"
    
    # Determine overall status
    if [ "$PR_STATE" = "MERGED" ]; then
        STATUS="✅ COMPLETED"
        STAGE="7-MERGED"
    elif [ "$PR_STATE" = "OPEN" ]; then
        STATUS="🔄 IN REVIEW"
        STAGE="6-PR-OPEN"
    elif [ "$PR_STATE" = "CLOSED" ] && [ "$PR_MERGED" = "null" ]; then
        STATUS="❌ PR CLOSED"
        STAGE="6-PR-CLOSED"
    elif [ "$HAS_COPILOT" = "true" ]; then
        STATUS="🤖 COPILOT WORKING"
        STAGE="5-COPILOT-ACTIVE"
    elif [ "$IN_PROGRESS" = "true" ]; then
        STATUS="⚡ IN PROGRESS"
        STAGE="4-IN-PROGRESS"
    elif [ "$HAS_AGENT" = "true" ]; then
        STATUS="👤 AGENT ASSIGNED"
        STAGE="3-AGENT-ASSIGNED"
    elif [ "$ISSUE_STATE" = "OPEN" ]; then
        STATUS="📋 ISSUE OPEN"
        STAGE="2-ISSUE-OPEN"
    elif [ "$ISSUE_NUM" = "NONE" ]; then
        STATUS="📄 RFC ONLY"
        STAGE="1-RFC-ONLY"
    else
        STATUS="❓ UNKNOWN"
        STAGE="0-UNKNOWN"
    fi
    
    echo ""
    echo "**$RFC_NUM: $RFC_TITLE**"
    echo "  Status: $STATUS"
    if [ "$ISSUE_NUM" != "NONE" ]; then
        echo "  Issue: #$ISSUE_NUM ($ISSUE_STATE)"
    fi
    if [ "$PR_NUM" != "NONE" ]; then
        echo "  PR: #$PR_NUM ($PR_STATE)"
        if [ "$PR_MERGED" != "null" ] && [ "$PR_MERGED" != "NONE" ]; then
            echo "  Merged: $(echo "$PR_MERGED" | cut -c1-10)"
        fi
    fi
done

echo ""
echo "## 📊 Overall Progress Summary"

# Calculate progress statistics
TOTAL_RFCS=$RFC_FILES
COMPLETED_COUNT=$(echo "$RFC_PRS" | jq '[.[] | select(.state == "MERGED")] | length')
PR_OPEN_COUNT=$(echo "$RFC_PRS" | jq '[.[] | select(.state == "OPEN")] | length')
COPILOT_WORKING_COUNT=$(echo "$RFC_ISSUES" | jq '[.[] | select(.has_copilot == true)] | length')
IN_PROGRESS_COUNT=$(echo "$RFC_ISSUES" | jq '[.[] | select(.in_progress == true)] | length')
AGENT_ASSIGNED_COUNT=$(echo "$RFC_ISSUES" | jq '[.[] | select(.has_agent == true)] | length')

echo "📈 **Implementation Pipeline:**"
echo "  ✅ Completed (Merged): $COMPLETED_COUNT/$TOTAL_RFCS"
echo "  🔄 In Review (PR Open): $PR_OPEN_COUNT/$TOTAL_RFCS"
echo "  🤖 Copilot Working: $COPILOT_WORKING_COUNT/$TOTAL_RFCS"
echo "  ⚡ In Progress: $IN_PROGRESS_COUNT/$TOTAL_RFCS"
echo "  👤 Agent Assigned: $AGENT_ASSIGNED_COUNT/$TOTAL_RFCS"

COMPLETION_PERCENTAGE=$((COMPLETED_COUNT * 100 / TOTAL_RFCS))
echo ""
echo "🎯 **Overall Completion: $COMPLETION_PERCENTAGE%** ($COMPLETED_COUNT/$TOTAL_RFCS RFCs)"

echo ""
echo "## 🚨 Attention Needed"

echo ""
echo "### RFCs Needing Issue Assignment:"
echo "$RFC_ISSUES" | jq -r '.[] | select(.has_agent == false and .state == "OPEN") | "  ❗ Issue #\(.number): \(.title)"'

echo ""
echo "### Stale Issues (No Update in 48+ Hours):"
CUTOFF_DATE=$(date -u -d '48 hours ago' +%Y-%m-%dT%H:%M:%SZ 2>/dev/null || date -u -j -v-48H +%Y-%m-%dT%H:%M:%SZ)
echo "$RFC_ISSUES" | jq -r --arg cutoff "$CUTOFF_DATE" '
.[] | select(.state == "OPEN" and .updated < $cutoff and .in_progress == true) | 
"  ⏰ Issue #\(.number): \(.title) (Last update: \(.updated | .[0:10]))"'

echo ""
echo "### Failed/Closed PRs Needing Attention:"
echo "$RFC_PRS" | jq -r '.[] | select(.state == "CLOSED" and .merged == null) | "  🔴 PR #\(.number): \(.title)"'

echo ""
echo "## 🔄 Recommended Actions"

# Generate specific recommendations
NEEDS_ISSUES=$(find docs/RFC/ -name "RFC[0-9]*-*.md" | wc -l)
HAS_ISSUES=$(echo "$RFC_ISSUES" | jq 'length')
MISSING_ISSUES=$((NEEDS_ISSUES - HAS_ISSUES))

if [ $MISSING_ISSUES -gt 0 ]; then
    echo "1. **Create Missing Issues**: $MISSING_ISSUES RFCs need implementation issues"
    echo "   Run: \`gh workflow run \"Create RFC Issues for Copilot\"\`"
fi

UNASSIGNED_COUNT=$(echo "$RFC_ISSUES" | jq '[.[] | select(.has_agent == false and .state == "OPEN")] | length')
if [ $UNASSIGNED_COUNT -gt 0 ]; then
    echo "2. **Assign Agents**: $UNASSIGNED_COUNT issues need agent assignment"
    echo "   Add labels: \`agent-assigned\` to ready issues"
fi

READY_FOR_COPILOT=$(echo "$RFC_ISSUES" | jq '[.[] | select(.has_agent == true and .has_copilot == false and .state == "OPEN")] | length')
if [ $READY_FOR_COPILOT -gt 0 ]; then
    echo "3. **Trigger Copilot**: $READY_FOR_COPILOT issues ready for Copilot preparation"
    echo "   Run: \`gh workflow run \"Auto-Spawn Copilot RFC Agents\" --field issue_numbers=all\`"
fi

echo ""
echo "## 🎯 Next Steps for Continuous Progress"
echo ""
echo "**Daily Monitoring:**"
echo "- Run this script: \`./rfc-progress-monitor.sh\`"
echo "- Check for stale issues and blocked PRs"
echo "- Ensure Copilot agents are making progress"
echo ""
echo "**Weekly Reviews:**"
echo "- Merge completed PRs promptly"
echo "- Close completed issues"
echo "- Create new RFC issues for upcoming work"
echo "- Adjust agent assignments based on progress"

echo ""
echo "=================================="
echo "Report completed: $(date -u)"