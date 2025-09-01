# Contributing

Thanks for your interest in contributing! This project is designed for RFC-driven, parallel AI + human development.

## How Work Is Organized

- RFC specs live in `docs/RFC/`.
- Each implementation task is tracked as an issue titled `RFC###: [Title] - Implementation` with label `rfc-implementation`.
- Automation assigns/labels issues and posts detailed guidance for GitHub Copilot Coding Agents.

## Getting Started

1. Read the project overview in `README.md` and `AGENTS.md`.
2. Browse RFCs in `docs/RFC/README.md`.
3. Pick an RFC that is `📝 Draft` and has dependencies satisfied.
4. Create a feature branch: `feature/rfc###-short-description`.
5. Implement per RFC acceptance criteria with tests (>80% preferred for new code).

## Development

- Runtime: .NET 8
- UI: Terminal.Gui v2
- ECS: Arch
- Tests: xUnit

Follow the architecture and patterns described in `AGENTS.md` and `.github/copilot-instructions.md`.

## Submitting Changes

1. Ensure all tests pass locally (`dotnet test`).
2. Open a Pull Request with:
   - Clear description referencing the RFC and acceptance criteria
   - Notes on tests and coverage
   - Any migration or compatibility concerns
3. Request review; do not merge directly to `main`.

## Code of Conduct

This project adheres to the [Code of Conduct](./CODE_OF_CONDUCT.md). By participating, you agree to abide by its terms.

## Security Issues

Please do not open public issues for security vulnerabilities. See [SECURITY.md](./SECURITY.md) for responsible disclosure.
