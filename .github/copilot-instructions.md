## Code Standards
- SOLID principles
- Clean coding

## Repository Structure
- Generator folder: Contains the hashi generator project as well as the interface definitions project
- Gui folder: Contains the hashi WPF project as well as it´s interface definitions. Additionally, it contains projects for language and translation
- PuzzleLoader folder: A projekt library that loads puzzles from a file and provides them to the generator. The interface definitions are also included here.
- Tests: Contains unit test projects for the associated libraries and projects
- Solver folder: Contains the linear solver project for hashi fields as well as the interface definitions project. Also contains the rules project which defines the rules for the hashi game.
- Enums: Contains all enums used in the project.

## Unit Tests
- Write unit tests for new functionality. 
- Use FluentAssertions and Moq for testing. 
- Use MockBehavior.Strict for all Mocks.
- UnitTest class name: [NameOfClassToTest]Tests.cs
- Unit Test naming: [MethodName]_When[TestConditions]_Should[ExpectedResult]
- Use pattern "Arrange, Act, Assert" for structuring tests

## Key Guidelines
1. Follow C# best practices and idiomatic patterns
2. Maintain existing code structure and organization
3. Document everything. InheritDoc for all public methods and classes. Summary, etc. belongs in the interface.
4. New classes require new interfaces
5. Use autofac for dependency injection in constructors
4. Write unit tests for new functionality. 
5. Use camel case for private fields
6. Use braces for if, for, foreach
7. Use namespace declaration