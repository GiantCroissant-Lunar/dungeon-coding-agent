# PR Merge Guide (PowerShell) — Dungeon Coding Agent

This guide documents reliable ways to merge PRs in this repo without using non-existent flags. It covers immediate merges, auto-merge, updating branches, and a REST fallback.

## Quick rules
- There is no `--merge-queue` flag in `gh`.
- `gh pr merge` is interactive; to auto-confirm in pwsh, pipe `y`.
- Rulesets and classic branch protection can both enforce checks.

## 1) Inspect PR status
```powershell
# Snapshot: state, mergeability, labels
gh pr view <pr> --json number,state,mergeStateStatus,mergeable,labels,url `
  --jq '{number,state,mergeStateStatus,mergeable,labels: (.labels|map(.name)),url}'

# Optional: checks (may not render in some shells)
gh pr checks <pr>
```

## 2) Immediate squash merge (with prompt auto-confirm)
```powershell
Write-Output 'y' | gh pr merge <pr> --squash --delete-branch
```

## 3) Enable auto-merge (queues when ready)
```powershell
# Auto-merge once gates pass (if merge queue is required, GitHub queues it automatically)
gh pr merge <pr> --squash --auto
```

## 4) If branch is BEHIND: update branch
```powershell
# Equivalent to the “Update branch” button
gh api -X PUT repos/GiantCroissant-Lunar/dungeon-coding-agent/pulls/<pr>/update-branch
```

## 5) Approvals and conversations
- If approvals are required, add one approval from an eligible reviewer.
- Resolve all open conversations if the rules require it.

## 6) Admin override (last resort)
```powershell
Write-Output 'y' | gh pr merge <pr> --squash --delete-branch --admin
```

## 7) REST fallback (non-interactive)
```powershell
# Immediate squash via REST API (no interactive prompt)
gh api -X PUT repos/GiantCroissant-Lunar/dungeon-coding-agent/pulls/<pr>/merge -f merge_method=squash
```

## 8) Refresh stuck “Expected” checks
```powershell
# Toggle labels to refire events
gh pr edit <pr> --add-label status
Start-Sleep -Seconds 2
gh pr edit <pr> --remove-label status

# Or close/reopen to force reevaluation
gh pr close <pr> --comment "Refreshing checks"; gh pr reopen <pr>
```

## 9) Branch protection vs rulesets
- Classic Branch Protection: Settings → Branches → main → Required status checks
- Rulesets: Settings → Rules → Repository rules
- Remove stale required checks (e.g., “Require Gemini AI Review / Check for 'ai-review' label”) from classic protection if already disabled in ruleset.

---

Tips
- Use `--squash` for clean history and set “Delete branch on merge” in repo settings.
- If a merge remains blocked without reason, grab the PR status JSON and post it in the PR for quick triage:
```powershell
gh pr view <pr> --json number,title,state,mergeStateStatus,mergeable,isDraft,reviewDecision,statusCheckRollup `
  --jq '{number,title,state,mergeStateStatus,mergeable,isDraft,reviewDecision,checks: (.statusCheckRollup|map({name,status,conclusion}))}'
```
