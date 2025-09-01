# PowerShell script to manually prepare GitHub issues for Copilot RFC implementation

param(
    [Parameter(Mandatory=$true)]
    [int]$IssueNumber,
    
    [Parameter(Mandatory=$false)]
    [switch]$Force,
    
    [Parameter(Mandatory=$false)]
    [string]$Repository = ""
)

# Check if GitHub CLI is installed
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error "GitHub CLI (gh) is not installed. Please install it first: https://cli.github.com/"
    exit 1
}

# Resolve repository if not provided
if ([string]::IsNullOrWhiteSpace($Repository)) {
    if ($env:GH_REPOSITORY) {
        $Repository = $env:GH_REPOSITORY
    } else {
        try {
            $originUrl = git remote get-url origin 2>$null
        } catch { $originUrl = $null }
        if ($originUrl) {
            # Support SSH and HTTPS formats
            if ($originUrl -match 'git@[^:]+:([^/]+/[^/\.]+)') { $Repository = $Matches[1] }
            elseif ($originUrl -match 'https?://[^/]+/([^/]+/[^/\.]+)(?:\.git)?') { $Repository = $Matches[1] }
        }
    }
}

if ([string]::IsNullOrWhiteSpace($Repository)) {
    Write-Error "Repository not specified and could not be auto-detected. Provide -Repository or set GH_REPOSITORY."
    exit 1
}

Write-Host "🚀 Preparing GitHub issue for Copilot RFC implementation..." -ForegroundColor Cyan

try {
    # Get issue details
    $issueData = gh issue view $IssueNumber --repo $Repository --json labels,title,body,assignees | ConvertFrom-Json
    
    if (-not $issueData) {
        Write-Error "Could not find issue #$IssueNumber in repository $Repository"
        exit 1
    }
    
    Write-Host "📋 Issue: $($issueData.title)" -ForegroundColor Green
    
    # Check if it's an RFC implementation issue
    $hasRfcLabel = $issueData.labels | Where-Object { $_.name -eq "rfc-implementation" }
    $hasAgentLabel = $issueData.labels | Where-Object { $_.name -eq "agent-assigned" }
    $hasWorkingLabel = $issueData.labels | Where-Object { $_.name -eq "copilot-working" }
    
    if (-not $hasRfcLabel) {
        Write-Warning "Issue #$IssueNumber does not have 'rfc-implementation' label"
    }
    
    if (-not $hasAgentLabel) {
        Write-Warning "Issue #$IssueNumber does not have 'agent-assigned' label"
    }
    
    if ($hasWorkingLabel -and -not $Force) {
        Write-Warning "Issue #$IssueNumber already has 'copilot-working' label. Use -Force to override."
        exit 1
    }
    
    # Extract RFC number
    $rfcMatch = [regex]::Match($issueData.title, 'RFC(\d{3})')
    if (-not $rfcMatch.Success) {
        Write-Error "Could not extract RFC number from issue title: $($issueData.title)"
        exit 1
    }
    
    $rfcNumber = $rfcMatch.Groups[1].Value
    Write-Host "🎯 RFC Number: $rfcNumber" -ForegroundColor Yellow
    
    # Add copilot-working label
    Write-Host "🏷️ Adding 'copilot-working' label..." -ForegroundColor Blue
    gh issue edit $IssueNumber --repo $Repository --add-label "copilot-working"
    
    # Create comprehensive preparation comment for Copilot
    $copilotComment = @"
# 🤖 Issue Prepared for GitHub Copilot Coding Agent - RFC$rfcNumber

This issue is now prepared for GitHub Copilot coding agent implementation with comprehensive guidance.

## ⚠️ Manual Step Required
**GitHub Copilot coding agents cannot be started programmatically.** To begin implementation:

1. **Via GitHub UI**: Click "Let Copilot work on this" button (if available in your organization)
2. **Via Copilot Chat**: Mention this issue in a GitHub Copilot chat session
3. **Ensure Settings**: Verify Copilot coding agents are enabled in your GitHub organization

## 🎯 Implementation Instructions for Copilot Agent

When a coding agent is assigned to this issue:

1. **Read the RFC**: Study the complete RFC specification in ``docs/RFC/RFC$rfcNumber-*.md``
2. **Follow Architecture**: Use Arch ECS patterns - Components are data, Systems are logic  
3. **Use Terminal.Gui**: All UI must use Terminal.Gui v2 framework
4. **Write Tests**: Achieve >80% test coverage with unit and integration tests
5. **Check Acceptance Criteria**: Complete ALL checkboxes in the RFC
6. **Create Feature Branch**: Use ``feature/rfc$rfcNumber-[description]`` naming convention

## 📋 Technical Requirements

- **ECS Patterns**: Components as structs, Systems inherit from ``SystemBase<World, float>``
- **Event System**: Use ``GameEvents.RaiseXXX()`` for inter-system communication
- **File Organization**: Follow ``src/DungeonCodingAgent.Game/[Components|Systems|UI|Core]/`` structure
- **Testing**: Write comprehensive tests in ``tests/DungeonCodingAgent.Tests/``
- **Code Style**: Follow existing C# conventions in the codebase

## 🔗 Key Resources

- **Architecture Guidelines**: See ``AGENTS.md`` and ``.github/copilot-instructions.md``
- **RFC Document**: Complete specification is in ``docs/RFC/RFC$rfcNumber-*.md``
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
"@

    # Post the comment
    Write-Host "💬 Posting preparation comment..." -ForegroundColor Magenta
    $copilotComment | gh issue comment $IssueNumber --repo $Repository --body-file -
    
    Write-Host "✅ Successfully prepared issue #$IssueNumber (RFC$rfcNumber) for Copilot coding agent" -ForegroundColor Green
    Write-Host "🔗 View issue: https://github.com/$Repository/issues/$IssueNumber" -ForegroundColor Cyan
    Write-Host "🎯 Next step: Start Copilot coding agent via GitHub UI or Copilot Chat" -ForegroundColor Yellow
    
} catch {
    Write-Error "Failed to prepare issue: $($_.Exception.Message)"
    exit 1
}