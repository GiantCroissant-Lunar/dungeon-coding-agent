#!/bin/bash

echo "🤖 Checking GitHub Copilot Agent Progress..."
echo "============================================="
echo

# Check for RFC issues
echo "📋 RFC Issues Status:"
gh issue list --state=open --json number,title,assignees,labels | jq -r '.[] | select(.title | contains("RFC")) | "Issue #\(.number): \(.title) - Assignees: \(.assignees | length)"'
echo

# Check for feature branches
echo "🌿 Feature Branches:"
git branch -r | grep -E "(feature/rfc|rfc)" | head -10 || echo "No RFC feature branches found yet"
echo

# Check for pull requests
echo "🔀 Active Pull Requests:"
gh pr list --json number,title,author,isDraft | jq -r '.[] | "PR #\(.number): \(.title) - Author: \(.author.login) - Draft: \(.isDraft)"' | head -5 || echo "No pull requests found yet"
echo

# Check recent commits on feature branches
echo "📝 Recent Commits on Feature Branches:"
for branch in $(git branch -r | grep -E "(feature/rfc|rfc)" | head -5); do
    if [ ! -z "$branch" ]; then
        echo "Branch: $branch"
        git log --oneline -3 $branch 2>/dev/null || echo "  No commits yet"
        echo
    fi
done

# Check workflow runs
echo "⚙️ Recent Workflow Runs:"
gh run list --limit 3 --json conclusion,name,headBranch | jq -r '.[] | "\(.name) on \(.headBranch): \(.conclusion // "running")"'
echo

echo "🎯 Next Steps:"
echo "- If no feature branches: Assign RFC issues to @copilot"
echo "- If branches exist but no commits: Wait or check Copilot assignment"
echo "- If commits exist: Monitor PR creation"
echo "- If PRs exist: Let automated review process handle merging"