#!/bin/bash
# Bash script to manually prepare GitHub issues for Copilot RFC implementation

set -euo pipefail

# Default values
REPOSITORY=""
FORCE=false

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --issue|-i)
            ISSUE_NUMBER="$2"
            shift 2
            ;;
        --repo|-r)
            REPOSITORY="$2"
            shift 2
            ;;
        --force|-f)
            FORCE=true
            shift
            ;;
        --help|-h)
            echo "Usage: $0 --issue <issue_number> [--repo <repository>] [--force]"
            echo ""
            echo "Options:"
            echo "  --issue, -i    Issue number to prepare for Copilot (required)"
            echo "  --repo, -r     Repository (default: auto-detect)"
            echo "  --force, -f    Force preparation even if already prepared"
            echo "  --help, -h     Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

# Resolve repository if not provided
if [[ -z "${REPOSITORY}" ]]; then
    if [[ -n "${GH_REPOSITORY:-}" ]]; then
        REPOSITORY="$GH_REPOSITORY"
    else
        originUrl=$(git remote get-url origin 2>/dev/null || true)
        if [[ -n "$originUrl" ]]; then
            if [[ "$originUrl" =~ git@[^:]+:([^/]+/[^/\.]+)(\.git)?$ ]]; then
                REPOSITORY="${BASH_REMATCH[1]}"
            elif [[ "$originUrl" =~ https?://[^/]+/([^/]+/[^/\.]+)(\.git)?$ ]]; then
                REPOSITORY="${BASH_REMATCH[1]}"
            fi
        fi
    fi
fi

# Validate required arguments
if [[ -z "${ISSUE_NUMBER:-}" ]]; then
    echo "❌ Error: Issue number is required"
    echo "Usage: $0 --issue <issue_number> [--repo <repository>] [--force]"
    exit 1
fi

if [[ -z "${REPOSITORY}" ]]; then
    echo "❌ Error: Repository not specified and could not be auto-detected. Provide --repo or set GH_REPOSITORY."
    exit 1
fi

# Check if GitHub CLI is installed
if ! command -v gh &> /dev/null; then
    echo "❌ Error: GitHub CLI (gh) is not installed"
    echo "Please install it first: https://cli.github.com/"
    exit 1
fi

# Check if jq is installed (required for JSON parsing)
if ! command -v jq &> /dev/null; then
    echo "❌ Error: 'jq' is required but not installed"
    echo "Please install jq: https://stedolan.github.io/jq/download/"
    exit 1
fi

echo "🚀 Preparing GitHub issue for Copilot RFC implementation..."

# Get issue details
echo "📋 Fetching issue #$ISSUE_NUMBER details..."
ISSUE_DATA=$(gh issue view "$ISSUE_NUMBER" --repo "$REPOSITORY" --json labels,title,body,assignees)

if [[ -z "$ISSUE_DATA" ]]; then
    echo "❌ Error: Could not find issue #$ISSUE_NUMBER in repository $REPOSITORY"
    exit 1
fi

ISSUE_TITLE=$(echo "$ISSUE_DATA" | jq -r '.title')
echo "📋 Issue: $ISSUE_TITLE"

# Check labels
HAS_RFC_LABEL=$(echo "$ISSUE_DATA" | jq -r '.labels[].name' | grep -q "rfc-implementation" && echo "true" || echo "false")
HAS_AGENT_LABEL=$(echo "$ISSUE_DATA" | jq -r '.labels[].name' | grep -q "agent-assigned" && echo "true" || echo "false")
HAS_WORKING_LABEL=$(echo "$ISSUE_DATA" | jq -r '.labels[].name' | grep -q "copilot-working" && echo "true" || echo "false")

if [[ "$HAS_RFC_LABEL" != "true" ]]; then
    echo "⚠️ Warning: Issue #$ISSUE_NUMBER does not have 'rfc-implementation' label"
fi

if [[ "$HAS_AGENT_LABEL" != "true" ]]; then
    echo "⚠️ Warning: Issue #$ISSUE_NUMBER does not have 'agent-assigned' label"
fi

if [[ "$HAS_WORKING_LABEL" == "true" && "$FORCE" != "true" ]]; then
    echo "⚠️ Warning: Issue #$ISSUE_NUMBER already has 'copilot-working' label"
    echo "Use --force to override"
    exit 1
fi

# Extract RFC number
RFC_NUMBER=$(echo "$ISSUE_TITLE" | grep -o 'RFC[0-9]\{3\}' | head -1 | sed 's/RFC//' || echo "")

if [[ -z "$RFC_NUMBER" ]]; then
    echo "❌ Error: Could not extract RFC number from issue title: $ISSUE_TITLE"
    exit 1
fi

echo "🎯 RFC Number: $RFC_NUMBER"

# Add copilot-working label
echo "🏷️ Adding 'copilot-working' label..."
gh issue edit "$ISSUE_NUMBER" --repo "$REPOSITORY" --add-label "copilot-working"

# Create comprehensive preparation comment for Copilot
echo "💬 Posting preparation comment..."

COPILOT_COMMENT=$(cat << EOF
# 🤖 Issue Prepared for GitHub Copilot Coding Agent - RFC$RFC_NUMBER

This issue is now prepared for GitHub Copilot coding agent implementation with comprehensive guidance.

## ⚠️ Manual Step Required
**GitHub Copilot coding agents cannot be started programmatically.** To begin implementation:

1. **Via GitHub UI**: Click "Let Copilot work on this" button (if available in your organization)
2. **Via Copilot Chat**: Mention this issue in a GitHub Copilot chat session
3. **Ensure Settings**: Verify Copilot coding agents are enabled in your GitHub organization

## 🎯 Implementation Instructions for Copilot Agent

When a coding agent is assigned to this issue:

1. **Read the RFC**: Study the complete RFC specification in \`docs/RFC/RFC$RFC_NUMBER-*.md\`
2. **Follow Architecture**: Use Arch ECS patterns - Components are data, Systems are logic  
3. **Use Terminal.Gui**: All UI must use Terminal.Gui v2 framework
4. **Write Tests**: Achieve >80% test coverage with unit and integration tests
5. **Check Acceptance Criteria**: Complete ALL checkboxes in the RFC
6. **Create Feature Branch**: Use \`feature/rfc$RFC_NUMBER-[description]\` naming convention

## 📋 Technical Requirements

- **ECS Patterns**: Components as structs, Systems inherit from \`SystemBase<World, float>\`
- **Event System**: Use \`GameEvents.RaiseXXX()\` for inter-system communication
- **File Organization**: Follow \`src/DungeonCodingAgent.Game/[Components|Systems|UI|Core]/\` structure
- **Testing**: Write comprehensive tests in \`tests/DungeonCodingAgent.Tests/\`
- **Code Style**: Follow existing C# conventions in the codebase

## 🔗 Key Resources

- **Architecture Guidelines**: See \`AGENTS.md\` and \`.github/copilot-instructions.md\`
- **RFC Document**: Complete specification is in \`docs/RFC/RFC$RFC_NUMBER-*.md\`
- **Existing Code**: Study current patterns in the codebase
- **Dependencies**: Terminal.Gui 2.0.0, Arch 2.0.0, xUnit for tests

## ✅ Definition of Done

- [ ] All RFC acceptance criteria checkboxes completed
- [ ] Unit tests written and passing (>80% coverage)
- [ ] Integration tests verify system works with existing code
- [ ] Code follows project architectural patterns
- [ ] Feature works as demonstrated in RFC
- [ ] Pull request created with detailed description

This issue is fully prepared with comprehensive context. A Copilot coding agent can now implement the RFC following all specified patterns and requirements.

---
*Prepared by manual trigger script*
EOF
)

# Post the comment
echo "$COPILOT_COMMENT" | gh issue comment "$ISSUE_NUMBER" --repo "$REPOSITORY" --body-file -

echo "✅ Successfully prepared issue #$ISSUE_NUMBER (RFC$RFC_NUMBER) for Copilot coding agent"
echo "🔗 View issue: https://github.com/$REPOSITORY/issues/$ISSUE_NUMBER"
echo "🎯 Next step: Start Copilot coding agent via GitHub UI or Copilot Chat"