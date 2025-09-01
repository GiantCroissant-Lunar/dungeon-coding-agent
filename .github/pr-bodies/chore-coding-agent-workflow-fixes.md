This PR improves the end-to-end coding-agent workflow reliability.

Changes
- Ensure labels: copilot-prepared, copilot-working, agent-work (labels-sync)
- Reliable activation: add a single @copilot block into issue bodies on create/prepare (idempotent markers)
- Preparation -> Working semantics: mark copilot-working when activation is present
- PR auto-link: append Closes #<issue> when RFC can be inferred from PR title or branch
- Stale/conflict coordinator: scheduled workflow to label/nudge conflicted/behind PRs and mark stale issues
- Merge Guide: added docs/MERGE-GUIDE.md with quick rebase and auto-merge criteria

Why
- Body-based @copilot activation is the most reliable trigger for new agent work
- Enforcing PR to issue linking closes issues on merge without manual steps

Notes
- AI review remains advisory-only
- Safe re-runs: idempotent activation markers prevent duplication

Follow-ups (separate PRs)
- RFC change sync to issue lifecycle (update/close)
