# Testing Strategy

This document outlines the comprehensive testing strategy for the Hashiwokakero WPF application, including patterns, conventions, and best practices.

## Testing Framework Stack

### Core Testing Tools
- **NUnit**: Primary testing framework for all test projects
- **FluentAssertions**: Readable and expressive assertions
- **Moq**: Mocking framework with strict behavior verification
- **Autofac**: Dependency injection container testing
- **Coverage**: Code coverage analysis and reporting

### Test Project Structure
```
Tests/
├── Hashi.Generator.Test/        # Generator component tests
├── Hashi.Gui.Test/              # GUI component tests
├── Hashi.LinearSolver.Test/     # Solver component tests
├── Hashi.Logging.Tests/         # Logging infrastructure tests
├── Hashi.Rules.Test/            # Rules engine tests
├── Hashi.SolvedPuzzles.Test/    # Puzzle I/O tests
└── Hashi.TestCoverageResults/   # Coverage reporting
```

## Test Categories

### Unit Tests
Test individual components in complete isolation with all dependencies mocked.

**Characteristics**:
- Fast execution (< 1 second per test)
- No external dependencies
- Deterministic results
- Single responsibility testing

**Example**:
```csharp
[TestFixture]
public class PuzzleGeneratorTests
{
    private IPuzzleGenerator puzzleGenerator;
    private Mock<ILogger> mockLogger;
    private Mock<IPuzzleValidator> mockValidator;

    [SetUp]
    public void Setup()
    {
        mockLogger = new Mock<ILogger>(MockBehavior.Strict);
        mockValidator = new Mock<IPuzzleValidator>(MockBehavior.Strict);
        puzzleGenerator = new PuzzleGenerator(mockLogger.Object, mockValidator.Object);
    }

    [Test]
    public async Task GenerateAsync_WhenValidParameters_ShouldReturnValidSolution()
    {
        // Arrange
        var parameters = new GenerationParameters
        {
            Width = 10,
            Height = 10,
            Difficulty = DifficultyEnum.Easy1
        };

        mockValidator.Setup(v => v.IsValid(It.IsAny<ISolutionProvider>()))
                    .Returns(true);

        // Act
        var result = await puzzleGenerator.GenerateAsync(parameters);

        // Assert
        result.Should().NotBeNull();
        result.HashiField.Should().NotBeNull();
        result.BridgeCoordinates.Should().NotBeEmpty();
        
        mockValidator.Verify(v => v.IsValid(It.IsAny<ISolutionProvider>()), Times.Once);
    }
}
```

### Integration Tests
Test component interactions and real system behavior.

**Characteristics**:
- Test multiple components together
- Use real implementations where appropriate
- Verify end-to-end workflows
- May involve file system, network, or external services

**Example**:
```csharp
[TestFixture]
public class PuzzleGenerationIntegrationTests
{
    private IContainer container;
    private IPuzzleGenerator puzzleGenerator;
    private IHashiSolver solver;

    [SetUp]
    public void Setup()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<AutoFacGeneratorModule>();
        builder.RegisterModule<AutoFacLinearSolverModule>();
        builder.RegisterModule<AutoFacLoggingModule>();
        
        container = builder.Build();
        puzzleGenerator = container.Resolve<IPuzzleGenerator>();
        solver = container.Resolve<IHashiSolver>();
    }

    [Test]
    public async Task GenerateAndSolve_WhenEasyDifficulty_ShouldProduceValidPuzzle()
    {
        // Arrange
        var parameters = new GenerationParameters
        {
            Width = 8,
            Height = 8,
            Difficulty = DifficultyEnum.Easy1
        };

        // Act
        var puzzle = await puzzleGenerator.GenerateAsync(parameters);
        var solverResult = await solver.SolveAsync(puzzle.HashiField);

        // Assert
        puzzle.Should().NotBeNull();
        solverResult.Should().Be(SolverStatusEnum.Optimal);
    }
}
```

### UI Tests
Test ViewModel behavior and MVVM pattern implementation.

**Characteristics**:
- Test command execution
- Verify property change notifications
- Test binding scenarios
- Mock view-related dependencies

**Example**:
```csharp
[TestFixture]
public class MainViewModelTests
{
    private MainViewModel viewModel;
    private Mock<IPuzzleGenerator> mockPuzzleGenerator;
    private Mock<ITimerProvider> mockTimerProvider;
    private Mock<ILogger> mockLogger;

    [SetUp]
    public void Setup()
    {
        mockPuzzleGenerator = new Mock<IPuzzleGenerator>(MockBehavior.Strict);
        mockTimerProvider = new Mock<ITimerProvider>(MockBehavior.Strict);
        mockLogger = new Mock<ILogger>(MockBehavior.Strict);
        
        viewModel = new MainViewModel(
            mockPuzzleGenerator.Object,
            mockTimerProvider.Object,
            mockLogger.Object);
    }

    [Test]
    public async Task NewGameCommand_WhenExecuted_ShouldStartNewGame()
    {
        // Arrange
        var expectedSolution = new Mock<ISolutionProvider>().Object;
        
        mockPuzzleGenerator.Setup(g => g.GenerateAsync(It.IsAny<GenerationParameters>()))
                          .ReturnsAsync(expectedSolution);
        mockTimerProvider.Setup(t => t.Reset());
        mockTimerProvider.Setup(t => t.Start());

        // Act
        await viewModel.NewGameCommand.ExecuteAsync(null);

        // Assert
        viewModel.CurrentSolution.Should().Be(expectedSolution);
        viewModel.GameState.Should().Be(GameStateEnum.Playing);
        
        mockPuzzleGenerator.Verify(g => g.GenerateAsync(It.IsAny<GenerationParameters>()), Times.Once);
        mockTimerProvider.Verify(t => t.Reset(), Times.Once);
        mockTimerProvider.Verify(t => t.Start(), Times.Once);
    }
}
```

## Test Naming Conventions

### Standard Format
`[MethodName]_When[TestConditions]_Should[ExpectedResult]`

### Examples
```csharp
// Good naming examples
[Test]
public void Constructor_WhenLoggerIsNull_ShouldThrowArgumentNullException()

[Test]
public async Task LoadPuzzleAsync_WhenFileExists_ShouldReturnValidSolution()

[Test]
public async Task GenerateAsync_WhenDifficultyIsExpert_ShouldCreateComplexPuzzle()

[Test]
public void SetProperty_WhenValueChanges_ShouldRaisePropertyChangedEvent()

[Test]
public void CanExecuteCommand_WhenGameStateIsPlaying_ShouldReturnTrue()
```

### Descriptive Test Names
```csharp
// More descriptive when needed
[Test]
public async Task GenerateAsync_WhenGridSizeIsLargerThanMaximum_ShouldThrowArgumentOutOfRangeException()

[Test]
public async Task SolveAsync_WhenPuzzleHasNoSolution_ShouldReturnInfeasibleStatus()

[Test]
public void PropertyChanged_WhenGameStateChangesToCompleted_ShouldNotifySubscribers()
```

## Test Patterns

### Arrange-Act-Assert Pattern
All tests should follow the AAA pattern for clarity and consistency.

```csharp
[Test]
public async Task ExampleTest_WhenCondition_ShouldExpectation()
{
    // Arrange - Set up test data and mocks
    var inputData = new TestData();
    var mockDependency = new Mock<IDependency>(MockBehavior.Strict);
    mockDependency.Setup(d => d.Method()).Returns(expectedValue);
    
    var systemUnderTest = new SystemUnderTest(mockDependency.Object);

    // Act - Execute the method being tested
    var result = await systemUnderTest.MethodUnderTest(inputData);

    // Assert - Verify the results
    result.Should().NotBeNull();
    result.Property.Should().Be(expectedValue);
    
    mockDependency.Verify(d => d.Method(), Times.Once);
}
```

### Setup and Teardown
```csharp
[TestFixture]
public class ExampleTests
{
    private SystemUnderTest systemUnderTest;
    private Mock<IDependency> mockDependency;

    [SetUp]
    public void Setup()
    {
        // Initialize common test dependencies
        mockDependency = new Mock<IDependency>(MockBehavior.Strict);
        systemUnderTest = new SystemUnderTest(mockDependency.Object);
    }

    [TearDown]
    public void TearDown()
    {
        // Verify all mock expectations were met
        mockDependency.VerifyAll();
        
        // Clean up resources if needed
        systemUnderTest?.Dispose();
    }
}
```

## Mock Usage Guidelines

### MockBehavior.Strict
Always use `MockBehavior.Strict` to ensure all mock interactions are explicitly defined.

```csharp
[Test]
public void ExampleTest_WhenCondition_ShouldExpectation()
{
    // Arrange
    var mockLogger = new Mock<ILogger>(MockBehavior.Strict);
    var mockService = new Mock<IService>(MockBehavior.Strict);

    // Setup only the methods that should be called
    mockLogger.Setup(l => l.Info(It.IsAny<string>()));
    mockService.Setup(s => s.Process(It.IsAny<string>()))
              .Returns("processed");

    var systemUnderTest = new SystemUnderTest(mockLogger.Object, mockService.Object);

    // Act
    var result = systemUnderTest.DoWork("input");

    // Assert
    result.Should().Be("processed");
    
    // Verify all setups were called
    mockLogger.Verify(l => l.Info(It.IsAny<string>()), Times.Once);
    mockService.Verify(s => s.Process("input"), Times.Once);
}
```

### Mock Verification
```csharp
[Test]
public void ExampleTest_WhenMultipleCallsExpected_ShouldVerifyCallCount()
{
    // Arrange
    var mockRepository = new Mock<IRepository>(MockBehavior.Strict);
    var items = new List<string> { "item1", "item2", "item3" };
    
    mockRepository.Setup(r => r.Save(It.IsAny<string>()))
                  .Returns(true);

    var processor = new BatchProcessor(mockRepository.Object);

    // Act
    processor.ProcessItems(items);

    // Assert
    mockRepository.Verify(r => r.Save(It.IsAny<string>()), Times.Exactly(3));
    mockRepository.Verify(r => r.Save("item1"), Times.Once);
    mockRepository.Verify(r => r.Save("item2"), Times.Once);
    mockRepository.Verify(r => r.Save("item3"), Times.Once);
}
```

### Exception Testing
```csharp
[Test]
public async Task LoadPuzzleAsync_WhenFileNotFound_ShouldThrowFileNotFoundException()
{
    // Arrange
    var mockFileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
    var filename = "nonexistent.json";
    
    mockFileSystem.Setup(fs => fs.FileExists(filename))
                  .Returns(false);

    var loader = new PuzzleLoader(mockFileSystem.Object);

    // Act & Assert
    await loader.Invoking(l => l.LoadPuzzleAsync(filename))
                .Should().ThrowAsync<FileNotFoundException>()
                .WithMessage($"*{filename}*");
}
```

## FluentAssertions Usage

### Basic Assertions
```csharp
[Test]
public void ExampleTest_WhenCondition_ShouldHaveExpectedProperties()
{
    // Arrange
    var puzzle = CreateTestPuzzle();

    // Act
    var result = puzzle.Validate();

    // Assert
    result.Should().NotBeNull();
    result.IsValid.Should().BeTrue();
    result.ErrorMessages.Should().BeEmpty();
    result.Islands.Should().HaveCount(5);
    result.Islands.Should().OnlyContain(island => island.Value > 0);
}
```

### Collection Assertions
```csharp
[Test]
public void ExampleTest_WhenProcessingItems_ShouldHaveExpectedResults()
{
    // Arrange
    var items = new[] { 1, 2, 3, 4, 5 };
    var processor = new ItemProcessor();

    // Act
    var results = processor.ProcessItems(items);

    // Assert
    results.Should().NotBeNull();
    results.Should().HaveCount(5);
    results.Should().OnlyContain(r => r.IsProcessed);
    results.Should().BeInAscendingOrder(r => r.Priority);
    results.Select(r => r.Value).Should().Equal(2, 4, 6, 8, 10);
}
```

### Exception Assertions
```csharp
[Test]
public void ExampleTest_WhenInvalidInput_ShouldThrowSpecificException()
{
    // Arrange
    var processor = new DataProcessor();
    var invalidData = new InvalidTestData();

    // Act & Assert
    processor.Invoking(p => p.Process(invalidData))
            .Should().Throw<ArgumentException>()
            .WithMessage("*invalid data*")
            .And.ParamName.Should().Be("data");
}
```

### Async Assertions
```csharp
[Test]
public async Task ExampleTest_WhenAsyncOperation_ShouldCompleteSuccessfully()
{
    // Arrange
    var service = new AsyncService();
    var parameters = new ServiceParameters();

    // Act
    var result = await service.ProcessAsync(parameters);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Data.Should().NotBeEmpty();
}
```

## Test Data Management

### Test Data Builders
```csharp
public class GenerationParametersBuilder
{
    private int width = 10;
    private int height = 10;
    private DifficultyEnum difficulty = DifficultyEnum.Easy1;

    public GenerationParametersBuilder WithSize(int width, int height)
    {
        this.width = width;
        this.height = height;
        return this;
    }

    public GenerationParametersBuilder WithDifficulty(DifficultyEnum difficulty)
    {
        this.difficulty = difficulty;
        return this;
    }

    public GenerationParameters Build()
    {
        return new GenerationParameters
        {
            Width = width,
            Height = height,
            Difficulty = difficulty
        };
    }
}

// Usage in tests
[Test]
public async Task GenerateAsync_WhenLargePuzzle_ShouldCreateValidSolution()
{
    // Arrange
    var parameters = new GenerationParametersBuilder()
        .WithSize(20, 20)
        .WithDifficulty(DifficultyEnum.Hard1)
        .Build();

    // Act & Assert
    var result = await puzzleGenerator.GenerateAsync(parameters);
    result.Should().NotBeNull();
}
```

### Test Data Files
```csharp
public static class TestDataHelper
{
    public static string GetTestDataPath(string filename)
    {
        return Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", filename);
    }

    public static async Task<string> LoadTestDataAsync(string filename)
    {
        var path = GetTestDataPath(filename);
        return await File.ReadAllTextAsync(path);
    }
}

[Test]
public async Task LoadPuzzleAsync_WhenValidFile_ShouldReturnSolution()
{
    // Arrange
    var testData = await TestDataHelper.LoadTestDataAsync("valid-puzzle.json");
    var tempFile = Path.GetTempFileName();
    await File.WriteAllTextAsync(tempFile, testData);

    try
    {
        // Act
        var result = await puzzleLoader.LoadPuzzleAsync(tempFile);

        // Assert
        result.Should().NotBeNull();
        result.HashiField.Should().NotBeNull();
    }
    finally
    {
        File.Delete(tempFile);
    }
}
```

## Coverage Requirements

### Coverage Targets
- **Unit Tests**: 90%+ code coverage
- **Integration Tests**: 80%+ scenario coverage
- **Critical Paths**: 100% coverage (puzzle generation, solving)

### Coverage Exclusions
- Auto-generated code
- Simple property getters/setters
- Dependency injection modules
- External library wrappers

### Coverage Analysis
```bash
# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate coverage reports
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"CoverageReport"
```

## Performance Testing

### Benchmark Tests
```csharp
[TestFixture]
public class PuzzleGenerationPerformanceTests
{
    [Test]
    public async Task GenerateAsync_WhenStandardPuzzle_ShouldCompleteWithinTimeLimit()
    {
        // Arrange
        var parameters = new GenerationParameters
        {
            Width = 15,
            Height = 15,
            Difficulty = DifficultyEnum.Medium1
        };

        var stopwatch = Stopwatch.StartNew();

        // Act
        var result = await puzzleGenerator.GenerateAsync(parameters);

        // Assert
        stopwatch.Stop();
        result.Should().NotBeNull();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // 5 seconds max
    }
}
```

### Memory Usage Tests
```csharp
[Test]
public async Task GenerateAsync_WhenLargePuzzle_ShouldNotExceedMemoryLimit()
{
    // Arrange
    var initialMemory = GC.GetTotalMemory(true);
    var parameters = new GenerationParameters
    {
        Width = 30,
        Height = 30,
        Difficulty = DifficultyEnum.Expert
    };

    // Act
    var result = await puzzleGenerator.GenerateAsync(parameters);

    // Assert
    var finalMemory = GC.GetTotalMemory(true);
    var memoryUsed = finalMemory - initialMemory;
    
    result.Should().NotBeNull();
    memoryUsed.Should().BeLessThan(50 * 1024 * 1024); // 50MB max
}
```

## Test Utilities

### Common Test Helpers
```csharp
public static class TestHelpers
{
    public static ISolutionProvider CreateTestSolution(int width, int height)
    {
        var field = new int[height, width];
        var islands = new List<(int x, int y, int value)>();
        
        // Create simple test pattern
        field[1, 1] = 2;
        field[1, 3] = 1;
        islands.Add((1, 1, 2));
        islands.Add((3, 1, 1));

        return new SolutionProvider(field, new List<IBridgeCoordinates>(), "Test Puzzle");
    }

    public static Mock<T> CreateStrictMock<T>() where T : class
    {
        return new Mock<T>(MockBehavior.Strict);
    }

    public static void VerifyLogging(Mock<ILogger> mockLogger, LogLevel level, string message)
    {
        mockLogger.Verify(l => l.Log(level, It.Is<string>(s => s.Contains(message))), Times.Once);
    }
}
```

### Test Categories and Attributes
```csharp
[Test]
[Category("Unit")]
[Category("Fast")]
public void FastUnitTest_WhenCondition_ShouldExpectation()
{
    // Fast unit test
}

[Test]
[Category("Integration")]
[Category("Slow")]
public async Task SlowIntegrationTest_WhenCondition_ShouldExpectation()
{
    // Longer running integration test
}

[Test]
[Ignore("Performance test - run manually")]
public void PerformanceTest_WhenLargeDataSet_ShouldCompleteQuickly()
{
    // Performance test
}
```

## Continuous Integration

### Test Execution Strategy
```yaml
# Example CI configuration
test:
  script:
    - dotnet test --configuration Release --no-build --verbosity normal
    - dotnet test --configuration Release --no-build --logger trx --collect:"XPlat Code Coverage"
  artifacts:
    reports:
      junit: "**/*.trx"
      cobertura: "**/coverage.cobertura.xml"
```

### Test Parallelization
```csharp
[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(4)]

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class ParallelTests
{
    // Tests that can run in parallel
}
```

## Best Practices Summary

### Do's
- ✅ Follow AAA pattern consistently
- ✅ Use descriptive test names
- ✅ Mock all dependencies with strict behavior
- ✅ Verify all mock interactions
- ✅ Test edge cases and error conditions
- ✅ Keep tests focused and independent
- ✅ Use FluentAssertions for readable assertions
- ✅ Maintain high code coverage
- ✅ Write tests before or alongside code

### Don'ts
- ❌ Don't test implementation details
- ❌ Don't use magic numbers or strings
- ❌ Don't create interdependent tests
- ❌ Don't ignore failing tests
- ❌ Don't test framework code
- ❌ Don't use loose mock behavior
- ❌ Don't write overly complex tests
- ❌ Don't skip error scenario testing

---

*For code standards and guidelines, see [Development Guide](Development-Guide.md)*