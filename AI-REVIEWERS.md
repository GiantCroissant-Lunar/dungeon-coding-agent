# AI Reviewers: Gemini Code Assist

This repository integrates Gemini Code Assist (GitHub App) to review pull requests.

- Bot account: `gemini-code-assist[bot]`
- Triggers:
  - On PR open: posts a summary and an initial review (configurable).
  - On demand: add a PR comment with one of:
    - `/gemini review`
    - `/gemini summary`
    - `/gemini help`

## Labels

- `ai-review-requested` — added when someone comments `/gemini ...` on a PR.
- `ai-review` — added when Gemini posts a review on the PR.
- `agent-work` — existing label applied to PRs created by agents or bots.

Configuration lives in `.gemini/`:
- `.gemini/config.yaml` — tuning (ignore patterns, severity, max comments, summary/review on PR open).
- `.gemini/styleguide.md` — repository style guidance for reviews.

## Disabling Copilot code review

To avoid using Copilot code review quota, we only use Gemini for PR reviews.
- Ensure any Copilot review workflows or bots are disabled, or gated behind a label you do not apply.
- If you re-enable Copilot, consider adding a manual label trigger to control usage.

## Notes

- Reviews typically appear within a few minutes of the trigger.
- You can react 👍/👎 to Gemini comments to give feedback.
- Adjust `.gemini/config.yaml` to manage noise:
  - `max_review_comments: 20`
  - `comment_severity_threshold: MEDIUM`
  - `ignore_patterns` to skip generated or vendor files.
