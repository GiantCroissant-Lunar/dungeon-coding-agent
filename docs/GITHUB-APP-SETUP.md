# GitHub App Setup for RFC Automation

This repo uses a GitHub App to authenticate automation workflows (creating issues, assigning labels, posting comments, etc.). Follow these steps to set it up for a public repository.

## 1) Create a GitHub App

1. Go to https://github.com/settings/apps (or your org’s Developer settings) and click "New GitHub App".
2. Fill out basic info (name, homepage URL, etc.).
3. Under Repository permissions, set:
   - Issues: Read and write
   - Pull requests: Read and write
   - Contents: Read-only
4. Webhooks are not required for these workflows; you can leave webhook settings as default (disabled) unless you need them.
5. Save the App and generate a Private Key (PEM). Download it.

## 2) Install the App on the Repository

1. From the App page, click "Install App".
2. Choose the target organization or your user account.
3. Select the repository where you want to enable automation (this repo), or "All repositories" if desired.

## 3) Add Repository Secrets

Create these secrets in the repository (Settings → Secrets and variables → Actions → New repository secret):

- APP_ID: The numeric ID of your GitHub App (found on the App page)
- APP_PRIVATE_KEY: Paste the contents of the PEM private key you downloaded

Note: Keep the PEM content exactly as downloaded, including BEGIN/END headers. Multi-line secrets are supported.

## 4) Validate Workflows

These workflows rely on the GitHub App token via `actions/create-github-app-token@v1`:

- `.github/workflows/auto-create-rfc-issues-on-push.yml`
- `.github/workflows/assign-rfc-issues-to-agent.yml`
- `.github/workflows/auto-spawn-copilot-agents.yml`
- `.github/workflows/labels-sync.yml`
- (Legacy) `.github/workflows/trigger-copilot-implementation.yml`

Ensure the above workflows run in your environment with the secrets properly configured.

## 5) Test the Setup

- Trigger a label sync to create standard labels:
  - Actions → "Sync Repository Labels" → Run workflow
- Create or update an RFC doc in `docs/RFC/` on `main`:
  - Verify an issue is created and labeled `rfc-implementation`.
- Manually assign and spawn Copilot for a test issue:
  - Actions → "Assign RFC Issues to Agent" → issue_numbers: the test issue number, add labels
  - Actions → "Auto-Spawn Copilot RFC Agents" → Run workflow

## 6) Copilot Coding Agent Enablement

- Ensure your organization/repository has GitHub Copilot Coding Agent enabled.
- The system cannot programmatically start a Copilot session; it prepares issues and posts detailed instructions.
- Use the GitHub UI ("Let Copilot work on this") or Copilot Chat to start the agent if available.

## Troubleshooting

- If workflows fail with authentication errors, re-check APP_ID and APP_PRIVATE_KEY secrets.
- Make sure the App is installed on the correct repository and has the listed permissions.
- Ensure labels exist via the label sync workflow.
- For rate-limit issues, try again after a short delay.
