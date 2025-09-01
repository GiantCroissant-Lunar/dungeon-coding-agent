# GitHub Coding Agent Workflow Roadmap (2025-09-01)

This roadmap captures current state, gaps, and a minimal set of fixes to achieve a mostly hands-free RFC→Issue→Agent→PR→Merge loop.

## 🎯 Target End-to-End Flow

1) RFC added/updated/removed → issues are created/updated/closed and always contain a reliable activation block.
2) Issues link the RFC clearly and carry correct labels. Preparation is tracked separately from active work.
3) Agent activates automatically (best-effort) from issue body; comments are only a fallback.
4) PRs are auto-linked to issues (Closes #N). Review is advisory; merges proceed when checks pass.
5) Stale/conflicting PRs/issues are reactivated or escalated by a “Coordinator” loop.

## ✅ What Works Today

- RFC definitions and templates are clear and complete.
- Advisory AI review (non-blocking) is in place.
- Issue creation (manual workflow) exists and is functional.
- Auto-prep workflow and script can discover “ready” RFC issues and add prep context.

## ❌ Gaps Blocking Reliability

- Activation reliability: issue bodies often lack a direct “@copilot” activation block (comments alone are inconsistent).
- Labels: some labels referenced in docs aren’t ensured (copilot-prepared, copilot-working, agent-work).
- Two-phase state: docs vs workflows differ on whether preparation uses copilot-prepared vs copilot-working.
- PR↔Issue linkage: not enforced; missing “Closes #N” leads to open issues after merges.
- Stale/conflict handling: monitors/coordinator steps are present but disabled or not tuned.
- RFC change sync: on-push updates/removals don’t consistently update/close corresponding issues.

## 🛠️ Minimal Fix Plan (do these in order)

1) Ensure missing labels exist
- File: .github/workflows/labels-sync.yml
- Add: copilot-prepared, copilot-working, agent-work
- DoD: labels appear in repo; no-op on re-run
- Validation: run labels sync; check GitHub labels list

2) Make activation reliable (issue body, idempotent)
- Files: .github/workflows/create-rfc-issues.yml, .github/workflows/auto-spawn-copilot-agents.yml
- On create and on prepare: inject/merge an “Activation” section in the issue body containing an @copilot request (only add once)
- Prefer a distinct delimiter (e.g., <!-- copilot-activation:start --> … <!-- copilot-activation:end -->) to avoid duplication
- Keep preparation label as copilot-prepared; set copilot-working only when activation block is present
- DoD: newly created/prepared issues contain @copilot in body exactly once
- Validation: create a test issue; verify body contains activation section

3) Align “prepared → working” semantics across docs and workflows
- Files: COPILOT-RFC-AUTOMATION.md, ASSIGN-COPILOT-AGENTS.md, README.md (short caveat)
- Clarify: preparation adds copilot-prepared; active work uses copilot-working
- Keep capacity default small (3); capacity is not the blocker
- DoD: docs and labels match runtime behavior

4) Enforce PR↔Issue linkage (auto “Closes #N”)
- File: .github/workflows/pr-auto-triage.yml (or a small new workflow)
- On opened/synchronize: infer related issue via branch name (feature/rfcXYZ…) or PR title; if missing, append “Closes #<issue>” to PR body
- DoD: merged PRs close their issue; manual override possible
- Validation: open a PR from a feature/rfc branch and confirm issue closes on merge

5) Enable a Coordinator loop for stale/conflicts
- Files: .github/workflows/workflow-cleanup-manager.yml.disabled, workflow-failure-monitor.yml.disabled, conflict-resolution-agent.yml (enable/tune)
- Behavior:
  - PR stale: comment with a precise @copilot reactivation template; try update-branch; apply merge-conflict label if still blocked
  - Issue stale with copilot-working but no PR: re-edit body with activation block and @copilot ping; optionally revert to copilot-prepared
- DoD: stale PRs/issues get actionable nudges automatically
- Validation: simulate stale by labels/timestamps; observe comments/labels

6) Sync issues on RFC changes (create/update/close)
- File: .github/workflows/auto-create-rfc-issues-on-push.yml
- Update issue titles/body when RFC title/status changes; close the issue when RFC is removed or marked Complete
- Ensure the activation block remains present after updates
- DoD: RFC file lifecycle mirrors issue lifecycle
- Validation: edit an RFC title/status and verify matching issue updates

## 📌 Implementation Notes

- Activation section template: keep concise and specific, include the RFC file path, and an explicit ask to start implementation.
- Idempotency: guard on an HTML comment marker to avoid duplicating activation text.
- Safety: never clobber existing issue body—append or merge the activation section.
- PR link inference: prefer branch naming hints (feature/rfc001-…), fallback to PR title or labels.

## 🔍 Verification & Quality Gates

- After each change: run related workflows via workflow_dispatch and confirm summaries.
- Track three metrics:
  1) Issues created with activation in body (%)
  2) PRs merged that auto-close issues (%)
  3) Stale items auto-nudged and resumed (% resumed within 48h)

## 🧭 Status Snapshot (today)

- Advisory AI review: ✅
- Labels complete/consistent: ❌ (missing 3 labels)
- Issue body activation: ❌ (not consistent)
- Prepared→Working semantics: ⚠️ (documented; not fully aligned)
- PR auto-closing issues: ❌ (not enforced)
- Coordinator for stale/conflicts: ⚠️ (disabled / needs tuning)
- RFC change sync to issues: ⚠️ (partial)

---

When this checklist is green, the GitHub coding agent should handle the majority of the loop with minimal human intervention.
