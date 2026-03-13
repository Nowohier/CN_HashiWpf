Perform a thorough code quality analysis of the complete solution. Examine every source file and generate a detailed refactoring plan.

## Analysis Areas

### 1. SOLID Principles
- **Single Responsibility:** Identify classes/methods doing too much
- **Open/Closed:** Find code that requires modification instead of extension
- **Liskov Substitution:** Check for inheritance violations
- **Interface Segregation:** Find bloated interfaces forcing unused implementations
- **Dependency Inversion:** Identify concrete dependencies where abstractions should be used

### 2. Clean Code
- Unclear or misleading naming (classes, methods, variables, parameters)
- Methods that are too long or have too many parameters
- Magic numbers or hardcoded strings that should be constants
- Code duplication across files
- Dead code or unused imports
- Overly complex conditionals or deeply nested logic

### 3. XML Documentation
- Missing XML doc comments on public classes, interfaces, methods, and properties
- Incomplete or low-quality existing documentation (e.g., missing `<param>`, `<returns>`, `<summary>`)
- Missing `<inheritdoc/>` on interface implementations

### 4. Inversion of Control & Dependency Injection
- Classes using `new` to create dependencies instead of injecting them
- Missing interface abstractions for services
- Service locator anti-patterns
- Tight coupling between components
- Registration issues or missing DI wiring

### 5. Unit Test Coverage
- Identify public methods with business logic, edge cases, or complex algorithms that lack unit tests
- Do NOT flag simple properties or trivial pass-throughs
- Check existing tests for correctness and completeness

### 6. Unit Test Quality (for existing tests)
- Tests must follow **Arrange, Act, Assert** pattern
- Tests must use **NUnit** as the framework
- Tests must use **FluentAssertions** for assertions
- Tests must use **Moq** with `MockBehavior.Strict` for mocking
- Test naming must follow: `[MethodName]_When[Condition]_Should[ExpectedResult]`
- Flag any deviations from the above

## Output Format

Generate a structured plan organized by file/class. For each finding include:

- **File:** path to the affected file
- **Issue:** clear description of the problem
- **Category:** which analysis area it falls under (SOLID, Clean Code, Documentation, IoC/DI, Test Coverage, Test Quality)
- **Severity:** Critical / High / Medium / Low
  - **Critical:** Architectural flaws, serious DI violations, major SOLID breaches
  - **High:** Missing tests for complex logic, significant clean code violations, missing abstractions
  - **Medium:** Documentation gaps, minor SOLID issues, test naming inconsistencies
  - **Low:** Minor style issues, cosmetic improvements
- **Fix:** concrete, actionable steps to resolve the issue

Summarize totals at the end grouped by severity and category.
