# Contributing to SpecMe

Thank you for your interest in contributing to SpecMe! Contributions from the community help make this project better for game server operators and players alike.

## Code of Conduct

Please maintain a respectful, welcoming, and professional environment across all issues, pull requests, and discussions.

## How Can I Contribute?

### Reporting Bugs
Before submitting an issue, please search the issue tracker to ensure the problem has not already been reported:
- Clearly describe the bug and steps to reproduce.
- Include your server OS, SwiftlyS2 version, and relevant console log output.
- State whether the issue happens on dedicated or local testing servers.

### Suggesting Enhancements
Feature requests and architectural improvements are welcome:
- Explain why this feature is useful to most users.
- Outline the expected command behavior or configuration schema.

### Pull Requests
1. Fork the repository and create your branch from `main`.
2. Ensure you have the .NET 10 SDK installed.
3. Keep code modular and clean, adhering to established conventions.
4. Ensure comments are textual and follow C# block style `/* ... */` without decorative banners.
5. Verify the project builds cleanly without errors:
   ```bash
   dotnet build -c Release
   ```
6. Submit a pull request detailing your changes.

## Commit Guidelines
- Use conventional commits format (e.g., `feat: ...`, `fix: ...`, `docs: ...`, `refactor: ...`).
- Write clear imperative subject lines with explanations in the body where relevant.

