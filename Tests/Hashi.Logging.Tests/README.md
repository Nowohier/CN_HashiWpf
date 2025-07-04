# Hashi.Logging.Tests

## Overview
Comprehensive unit tests for the Hashi.Logging project, covering the NLog-based logging infrastructure.

## Test Coverage
- **LoggerTests**: Tests for the `Logger` class
  - All logging levels (Trace, Debug, Info, Warn, Error, Fatal)
  - Exception logging scenarios
  - Edge cases with null/empty messages
  - Integration with real NLog instances

- **LoggerFactoryTests**: Tests for the `LoggerFactory` class
  - Logger creation with generic types
  - Logger creation with string names
  - NLog configuration validation
  - Edge cases and Unicode character handling

- **AutoFacLoggingModuleTests**: Tests for the AutoFac dependency injection module
  - Module registration verification
  - Singleton behavior validation
  - Integration testing with resolved instances

## Test Framework
- **NUnit**: Test framework
- **FluentAssertions**: Assertion library
- **Moq**: Mocking framework (MockBehavior.Strict)
- **Autofac**: Dependency injection container
- **NLog**: Real logging instances for integration testing

## Test Patterns
- Follows "Arrange, Act, Assert" pattern
- Naming convention: `[MethodName]_When[TestConditions]_Should[ExpectedResult]`
- Integration testing with real NLog instances
- No mocking of sealed NLog.Logger class - uses real instances

## Coverage Areas
- ✅ Logger constructor validation
- ✅ All logging methods (Trace, Debug, Info, Warn, Error, Fatal)
- ✅ Error and Fatal methods with exception parameters
- ✅ Edge cases (null, empty messages)
- ✅ LoggerFactory constructor and configuration
- ✅ Generic and string-based logger creation
- ✅ NLog configuration validation
- ✅ Interface implementation verification
- ✅ Integration testing with AutoFac
- ✅ Thread safety and singleton behavior
- ✅ Unicode and special character handling

## Special Notes
- Uses real NLog instances instead of mocks due to sealed class limitations
- Tests focus on behavior validation rather than mock verification
- Includes reflection-based testing for private methods
- File system integration testing for log configuration