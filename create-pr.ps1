param(
    [Parameter(Mandatory=$false)] [string]$Base = "main",
    [Parameter(Mandatory=$false)] [string]$Head = "",
    [Parameter(Mandatory=$true)]  [string]$Title,
    [Parameter(Mandatory=$false)] [string]$BodyFile,
    [Parameter(Mandatory=$false)] [string]$BodyText,
    [Parameter(Mandatory=$false)] [string[]]$Labels,
    [Parameter(Mandatory=$false)] [string[]]$Reviewers,
    [Parameter(Mandatory=$false)] [switch]$Draft
)

$ErrorActionPreference = 'Stop'

function Fail($msg) { Write-Error $msg; exit 1 }

# Check gh
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) { Fail "GitHub CLI (gh) not found. Install from https://cli.github.com/" }

# Determine repo
$Repository = $env:GH_REPOSITORY
if (-not $Repository) {
    try { $originUrl = git remote get-url origin 2>$null } catch { $originUrl = $null }
    if ($originUrl -match 'git@[^:]+:([^/]+/[^/\\.]+)') { $Repository = $Matches[1] }
    elseif ($originUrl -match 'https?://[^/]+/([^/]+/[^/\\.]+)(?:\.git)?') { $Repository = $Matches[1] }
}
if (-not $Repository) { Fail "Could not determine repository. Set GH_REPOSITORY=owner/repo or run inside a git repo with 'origin'." }

# Determine head branch
if ([string]::IsNullOrWhiteSpace($Head)) {
    $Head = (git rev-parse --abbrev-ref HEAD).Trim()
}
if (-not $Head) { Fail "Could not determine head branch." }

# Ensure branch pushed
$remoteBranch = (git ls-remote --heads origin $Head) 2>$null
if (-not $remoteBranch) {
    Write-Host "Pushing branch '$Head' to origin..." -ForegroundColor Cyan
    git push -u origin $Head | Out-Null
}

# Prepare body file
$tempBody = $null
try {
    if ($BodyFile) {
        if (-not (Test-Path -LiteralPath $BodyFile)) { Fail "BodyFile not found: $BodyFile" }
        $bodyPath = (Resolve-Path -LiteralPath $BodyFile).Path
    } elseif ($BodyText) {
        $tempBody = New-TemporaryFile
        Set-Content -LiteralPath $tempBody -Value $BodyText -Encoding UTF8
        $bodyPath = $tempBody
    } else {
        $tempBody = New-TemporaryFile
        Set-Content -LiteralPath $tempBody -Value "Automated PR" -Encoding UTF8
        $bodyPath = $tempBody
    }

    # Build gh pr create args safely
    $ghArgs = @('pr','create','--base', $Base,'--head', $Head,'--title', $Title,'--body-file', $bodyPath)
    if ($Draft) { $ghArgs += '--draft' }
    if ($Labels) { foreach($l in $Labels){ if($l){ $ghArgs += @('--label',$l) } } }
    if ($Reviewers) { foreach($r in $Reviewers){ if($r){ $ghArgs += @('--reviewer',$r) } } }

    Write-Host "Creating PR: $Title" -ForegroundColor Green
    $out = & gh @ghArgs 2>&1
    if ($LASTEXITCODE -ne 0) { $out | Write-Host; Fail "gh pr create failed." }
    $url = ($out | Select-String -Pattern 'https?://\S+' -AllMatches).Matches.Value | Select-Object -First 1
    if ($url) { Write-Host "PR URL: $url" -ForegroundColor Yellow } else { Write-Host $out }
}
finally {
    if ($tempBody -and (Test-Path -LiteralPath $tempBody)) { Remove-Item -LiteralPath $tempBody -Force -ErrorAction SilentlyContinue }
}
