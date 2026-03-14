# Contributing to Hashiwokakero

Thank you for your interest in contributing! Here's how you can help.

## Getting Started

1. Fork the repository
2. Clone your fork: `git clone https://github.com/<your-username>/CN_HashiWpf.git`
3. Create a feature branch: `git checkout -b feature/your-feature-name`
4. Make your changes
5. Push and open a Pull Request against `master`

## Development Setup

- .NET 8.0 SDK
- Windows 10/11 (WPF requires Windows)
- Visual Studio 2022 or JetBrains Rider (recommended)

```bash
dotnet restore
dotnet build
dotnet test
```

## Code Standards

- Follow **SOLID** principles and **Clean Code** practices
- Document all public classes, methods, and interfaces with XML comments
- Use `<inheritdoc/>` for interface implementations
- One class per file (file name = class name)
- Use braces for all control flow statements
- Use camelCase for private fields

## Testing

- Framework: **NUnit** + **FluentAssertions** + **Moq**
- Always use `MockBehavior.Strict`
- Follow **Arrange, Act, Assert** pattern
- Naming: `[MethodName]_When[Condition]_Should[ExpectedBehavior]`
- Test all business logic and edge cases

## Pull Requests

- Keep PRs focused on a single change
- Include tests for new functionality
- Ensure all existing tests pass (`dotnet test`)
- Write a clear PR description explaining the "why"

## Reporting Issues

- Use GitHub Issues to report bugs or suggest features
- Include steps to reproduce for bug reports
- Check existing issues before creating a new one
