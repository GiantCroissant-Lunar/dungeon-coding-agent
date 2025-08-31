# RFC013: Build/CI Quality Gates & Auto-merge Policy

## 📋 Metadata
- Status: 📝 Draft
- Author: Architecture Team
- Created: 2025-08-31
- Dependencies: None (Tooling)

## 🎯 Objective
Establish reliable CI quality gates and an auto-merge policy so PRs from coding agents are merged automatically once checks, AI review, and labels meet policy.

## 📖 Problem Statement
Agent-driven PRs need consistent, automated enforcement for build, tests, coverage, linting, labels, and reviews. Manual merges slow the pipeline and introduce inconsistency.

## 🏗️ Technical Specification
- CI Jobs
  - `dotnet build` + `dotnet test` with code coverage (coverlet or built-in collectors)
  - Lint/format check (dotnet format or analyzers) – non-blocking warning phase at first
- Quality Gates
  - All checks green
  - Minimum line coverage threshold: 70% (raise later)
  - Label enforcement: `ai-review` or `ai-review-requested` must be present; `do-not-merge` absent
- Auto-merge
  - Enable GitHub repo auto-merge; PRs merge via squash once gates pass
  - Delete branch on merge
- Visibility
  - Add CI status badge(s) and a short policy section to top-level `README.md`

## 🔗 Integration Points
- `.github/workflows/auto-merge-coordinator.yml` coordinates merge windows and label checks
- `.github/workflows/pr-agent-commands.yml` supports slash-triggered actions
- `.github/workflows/labels-sync.yml` maintains standard labels

## ✅ Acceptance Criteria
- [ ] CI runs on PR and main; build + test + coverage reported
- [ ] PR blocked if coverage < threshold or tests fail
- [ ] Auto-merge occurs when labels and checks are satisfied; branch deleted
- [ ] README shows badges and short policy statement

## 🧪 Testing Strategy
- Create a dummy PR that intentionally fails coverage → verify block
- Adjust test to meet threshold → verify auto-merge after labels present

## 📊 Performance Considerations
- Keep CI under 5 minutes on typical agent PRs; parallelize test projects when possible
