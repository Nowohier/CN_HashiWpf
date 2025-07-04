# Hashi.SolvedPuzzles.Test

## Overview
Comprehensive unit tests for the Hashi.SolvedPuzzles project, covering the puzzle loading functionality.

## Test Coverage
- **HashiPuzzleLoaderTests**: Tests for the `HashiPuzzleLoader` class
  - File loading and JSON deserialization
  - Error handling (file not found, invalid format, null content)
  - Edge cases and boundary conditions
  - Private method testing via reflection

- **AutoFacHashiSolvedPuzzlesModuleTests**: Tests for the AutoFac dependency injection module
  - Module registration verification
  - Singleton behavior validation
  - Integration testing with resolved instances

## Test Framework
- **NUnit**: Test framework
- **FluentAssertions**: Assertion library
- **Moq**: Mocking framework (MockBehavior.Strict)
- **Autofac**: Dependency injection container

## Test Patterns
- Follows "Arrange, Act, Assert" pattern
- Naming convention: `[MethodName]_When[TestConditions]_Should[ExpectedResult]`
- Comprehensive error path testing
- File system isolation using temporary files

## Coverage Areas
- ✅ Constructor validation
- ✅ LoadPuzzle method with valid files
- ✅ LoadPuzzle method with invalid/missing files
- ✅ JSON deserialization edge cases
- ✅ Private method testing (GetHashiFileName)
- ✅ Interface implementation verification
- ✅ Integration testing with real files
- ✅ AutoFac module registration