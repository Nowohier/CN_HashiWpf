# Coding Standards

## Principles

- Follow **SOLID** principles and **Clean Code** practices
- Write self-documenting code with meaningful names

## Documentation

- Document all public classes, methods, properties, and interfaces with XML comments
- Use `<inheritdoc/>` for interface method implementations

## Unit Testing

- **Framework:** NUnit + FluentAssertions + Moq
- **Mocking:** Always use `MockBehavior.Strict`
- **Pattern:** Arrange, Act, Assert
- **Naming:** `[MethodName]_When[Condition]_Should[ExpectedBehavior]`
- **Coverage:** Test all business logic, edge cases, and complex algorithms. Skip simple properties and trivial pass-throughs.

## Organization

- One class per file (file name = class name)
- Use dependency injection with interfaces