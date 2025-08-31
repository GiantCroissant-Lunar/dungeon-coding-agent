# Dungeon Coding Agent – Code Review Style Guide

This style guide informs AI code reviewers (e.g., Gemini Code Assist) about our preferences.

- Languages: C#, Python, YAML, Markdown, PowerShell, GitHub Actions
- Goals:
  - Prefer clarity and maintainability over micro-optimizations.
  - Keep CI workflows idempotent, least-privileged, and repo-scoped.
  - Use descriptive naming and small, focused functions.
  - Favor explicit error handling and actionable log messages.
- Testing:
  - Recommend adding/adjusting tests when logic changes or new paths are introduced.
  - Suggest lightweight checks where full unit tests aren’t available.
- Security:
  - Avoid hardcoding secrets; use GitHub Secrets and environment variables.
  - Principle of least privilege for tokens and apps.
- Documentation:
  - Update relevant README or workflow comments when behavior changes.
  - Prefer concise, example-driven explanations.

## Language-specific

- C#
  - Follow .NET naming conventions, null-safety, and guard clauses.
  - Keep public APIs documented with XML comments when feasible.
- Python
  - PEP 8 style; type hints preferred for new/modified functions.
  - Clear exceptions with context; avoid bare except.
- YAML (GitHub Actions)
  - Minimize duplication; use reusable steps.
  - Explicit permissions and `gh --repo` where practical.
- PowerShell
  - Avoid parsing plain text when `ConvertFrom-Json` is available.
  - Check `$LASTEXITCODE` and provide helpful error messages.
- Markdown
  - Headings consistent; short sections; code blocks with language tags.
