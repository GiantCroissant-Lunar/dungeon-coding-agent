#!/usr/bin/env pwsh

Write-Host "🤖 Checking GitHub Copilot Agent Progress..." -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host

# Check for RFC issues
Write-Host "📋 RFC Issues Status:" -ForegroundColor Yellow
$issues = gh issue list --state=open --json number,title,assignees | ConvertFrom-Json
$rfcIssues = $issues | Where-Object { $_.title -like "*RFC*" }
foreach ($issue in $rfcIssues) {
    $assigneeCount = if ($issue.assignees) { $issue.assignees.Count } else { 0 }
    Write-Host "Issue #$($issue.number): $($issue.title) - Assignees: $assigneeCount"
}
Write-Host

# Check for feature branches
Write-Host "🌿 Feature Branches:" -ForegroundColor Green
$branches = git branch -r | Select-String "(feature/rfc|rfc)" | Select-Object -First 10
if ($branches) {
    $branches | ForEach-Object { Write-Host $_.Line.Trim() }
} else {
    Write-Host "No RFC feature branches found yet"
}
Write-Host

# Check for pull requests
Write-Host "🔀 Active Pull Requests:" -ForegroundColor Magenta
$prs = gh pr list --json number,title,author,isDraft | ConvertFrom-Json
if ($prs) {
    $prs | Select-Object -First 5 | ForEach-Object {
        Write-Host "PR #$($_.number): $($_.title) - Author: $($_.author.login) - Draft: $($_.isDraft)"
    }
} else {
    Write-Host "No pull requests found yet"
}
Write-Host

# Check recent commits on feature branches
Write-Host "📝 Recent Commits on Feature Branches:" -ForegroundColor Blue
$featureBranches = git branch -r | Select-String "(feature/rfc|rfc)" | Select-Object -First 5
foreach ($branch in $featureBranches) {
    if ($branch) {
        $branchName = $branch.Line.Trim()
        Write-Host "Branch: $branchName"
        try {
            $commits = git log --oneline -3 $branchName 2>$null
            if ($commits) {
                $commits | ForEach-Object { Write-Host "  $_" }
            } else {
                Write-Host "  No commits yet"
            }
        } catch {
            Write-Host "  No commits yet"
        }
        Write-Host
    }
}

# Check workflow runs
Write-Host "⚙️ Recent Workflow Runs:" -ForegroundColor DarkYellow
$runs = gh run list --limit 3 --json conclusion,name,headBranch | ConvertFrom-Json
$runs | ForEach-Object {
    $status = if ($_.conclusion) { $_.conclusion } else { "running" }
    Write-Host "$($_.name) on $($_.headBranch): $status"
}
Write-Host

Write-Host "🎯 Next Steps:" -ForegroundColor White
Write-Host "- If no feature branches: Assign RFC issues to @copilot"
Write-Host "- If branches exist but no commits: Wait or check Copilot assignment"  
Write-Host "- If commits exist: Monitor PR creation"
Write-Host "- If PRs exist: Let automated review process handle merging"