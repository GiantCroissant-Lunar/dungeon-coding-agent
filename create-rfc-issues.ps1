# PowerShell script to create GitHub issues for all RFCs using GitHub CLI
# Usage: .\create-rfc-issues.ps1
# Prerequisites: 
# - Install GitHub CLI: https://cli.github.com/
# - Login: gh auth login
# - Push repo to GitHub first

Write-Host "🐉 Creating RFC implementation issues for Dungeon Coding Agent..." -ForegroundColor Cyan

# Check if gh is installed and authenticated
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Host "❌ GitHub CLI not found. Install from https://cli.github.com/" -ForegroundColor Red
    exit 1
}

try {
    gh auth status 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Not authenticated with GitHub CLI. Run: gh auth login" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "❌ Not authenticated with GitHub CLI. Run: gh auth login" -ForegroundColor Red
    exit 1
}

# Array of RFCs with title, file, effort, and dependencies
$rfcs = @(
    "RFC001:Core Game Loop & State Management:RFC001-Core-Game-Loop.md:2-3 days:None (foundational system)",
    "RFC002:Terminal.Gui Application Shell:RFC002-Terminal-Application-Shell.md:3-4 days:RFC001",
    "RFC003:Map Generation & Rendering System:RFC003-Map-Generation-System.md:4-5 days:RFC001, RFC002",
    "RFC004:Player Entity & Movement System:RFC004-Player-Movement-System.md:3-4 days:RFC001, RFC003",
    "RFC006:Basic Inventory System:RFC006-Basic-Inventory.md:1-2 days:RFC001, RFC004",
    "RFC007:Simple Message Log UI:RFC007-Simple-UI-Messages.md:1 day:RFC002",
    "RFC008:Save Game Data System:RFC008-Save-Game-Data.md:2 days:RFC001",
    "RFC009:Simple Enemy AI:RFC009-Simple-Enemy-AI.md:2-3 days:RFC001, RFC003, RFC004",
    "RFC010:Health and Status Bar UI:RFC010-Health-Status-Bar.md:1 day:RFC002, RFC004"
)

# Create issues for each RFC
foreach ($rfc in $rfcs) {
    $parts = $rfc -split ':'
    $rfc_num = $parts[0]
    $title = $parts[1]
    $filename = $parts[2]
    $effort = $parts[3]
    $deps = $parts[4]
    
    Write-Host "📋 Creating issue for $rfc_num`: $title" -ForegroundColor Yellow
    
    # Create issue body
    $issue_body = @"
## 🤖 RFC Implementation Request

**RFC**: $rfc_num`: $title
**File**: ``docs/RFC/$filename``
**Estimated Effort**: $effort
**Dependencies**: $deps

## 📋 Task Description

Implement the system specified in the linked RFC according to its acceptance criteria and definition of done.

## ✅ Definition of Done

Please complete ALL checkboxes in the RFC specification before marking this issue as complete:

**Review the complete RFC specification**: ``docs/RFC/$filename``

The RFC contains detailed:
- Technical specifications with code examples
- Complete acceptance criteria (checkboxes to complete)
- Integration requirements and event definitions
- Testing strategy and performance requirements

## 🎯 Success Criteria

- [ ] All RFC acceptance criteria checkboxes completed
- [ ] Unit tests written and passing (>80% coverage)
- [ ] Code follows project standards from ``AGENTS.md``
- [ ] Integration events working with existing systems
- [ ] RFC status updated to ``✅ Complete``

## 📚 Resources

- **Agent Guidelines**: ``AGENTS.md`` - Coding standards and architecture patterns
- **Project Context**: ``CLAUDE.md`` - Overall project architecture
- **RFC Directory**: ``docs/RFC/README.md`` - All RFC specifications

## 🔧 Development Notes

When implementing this RFC:

1. **Read the complete RFC specification** - Don't start coding until you understand all requirements
2. **Follow ECS architecture patterns** - Use Arch ECS for all game state and logic
3. **Write tests as you go** - Each system needs corresponding unit tests
4. **Fire integration events** - Systems communicate through the GameEvents class
5. **Update RFC status** - Change from ``📝 Draft`` to ``🔄 In Progress`` to ``✅ Complete``

---

**@copilot This issue is ready for implementation. Please review the linked RFC specification and implement according to the acceptance criteria.**
"@

    # Create the issue using GitHub CLI
    try {
        gh issue create --title "Implement $rfc_num`: $title" --body $issue_body --label "rfc-implementation,copilot" 2>$null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Created issue for $rfc_num" -ForegroundColor Green
        } else {
            Write-Host "❌ Failed to create issue for $rfc_num" -ForegroundColor Red
        }
    } catch {
        Write-Host "❌ Failed to create issue for $rfc_num`: $_" -ForegroundColor Red
    }
    
    # Small delay to avoid rate limiting
    Start-Sleep -Seconds 1
}

Write-Host ""
Write-Host "🎉 Finished creating RFC issues!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Go to your GitHub repository issues page"
Write-Host "2. Assign specific issues to @copilot"
Write-Host "3. Start with RFC001 and RFC002 (foundational systems)"
Write-Host "4. Monitor pull requests created by Copilot"
Write-Host ""
Write-Host "📖 See GETTING-STARTED-WITH-COPILOT.md for detailed instructions" -ForegroundColor Yellow