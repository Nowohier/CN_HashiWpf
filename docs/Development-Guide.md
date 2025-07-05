# Development Guide

This guide provides comprehensive information for developers working on the Hashiwokakero WPF application.

## Development Environment Setup

### Prerequisites
- **.NET 8.0 SDK** or later
- **Visual Studio 2022** (recommended) or **Visual Studio Code**
- **Git** for version control
- **Windows 10/11** (for WPF development)

### IDE Configuration

#### Visual Studio 2022
1. **Workloads**:
   - .NET Desktop Development
   - .NET 8.0 targeting pack

2. **Extensions** (recommended):
   - **XAML Styler**: Automatic XAML formatting
   - **CodeMaid**: Code cleanup and organization
   - **ReSharper** or **CodeRush**: Code analysis and refactoring
   - **EditorConfig**: Consistent formatting across team

3. **Settings**:
   - Enable **"Enable .NET Framework source stepping"**
   - Configure **Exception Settings** for debugging
   - Set **Startup Project** to `Hashi.Gui`

#### Visual Studio Code
1. **Extensions**:
   - **C# Extension Pack**
   - **XAML Language Support**
   - **EditorConfig for VS Code**
   - **GitLens**: Enhanced Git integration

2. **Configuration**:
   - Configure tasks in `.vscode/tasks.json`
   - Set up launch configurations in `.vscode/launch.json`

## Code Standards

### SOLID Principles
Our codebase strictly follows SOLID principles:

#### Single Responsibility Principle (SRP)
- Each class should have only one reason to change
- Separate concerns into different classes

**Example**:
```csharp
// Good - Single responsibility
public class PuzzleGenerator : IPuzzleGenerator
{
    public Task<ISolutionProvider> GenerateAsync(GenerationParameters parameters)
    {
        // Only responsible for puzzle generation
    }
}

public class PuzzleValidator : IPuzzleValidator
{
    public bool IsValid(ISolutionProvider solution)
    {
        // Only responsible for validation
    }
}
```

#### Open/Closed Principle (OCP)
- Open for extension, closed for modification
- Use interfaces and inheritance for extensibility

**Example**:
```csharp
// Base interface
public interface IHintProvider
{
    Task<IHint> GetHintAsync(ISolutionProvider solution);
}

// Extensible implementation
public class BasicHintProvider : IHintProvider
{
    public virtual Task<IHint> GetHintAsync(ISolutionProvider solution)
    {
        // Base implementation
    }
}

public class AdvancedHintProvider : BasicHintProvider
{
    public override Task<IHint> GetHintAsync(ISolutionProvider solution)
    {
        // Extended implementation
    }
}
```

#### Liskov Substitution Principle (LSP)
- Derived classes must be substitutable for their base classes
- Maintain behavioral contracts

#### Interface Segregation Principle (ISP)
- Clients should not depend on interfaces they don't use
- Create focused, cohesive interfaces

**Example**:
```csharp
// Good - Segregated interfaces
public interface IReadOnlySettings
{
    T GetSetting<T>(string key, T defaultValue = default);
}

public interface IWritableSettings
{
    void SetSetting<T>(string key, T value);
    void SaveSettings();
}

// Implement both when needed
public class SettingsProvider : IReadOnlySettings, IWritableSettings
{
    // Implementation
}
```

#### Dependency Inversion Principle (DIP)
- Depend on abstractions, not concretions
- Use dependency injection throughout

**Example**:
```csharp
public class MainViewModel : ObservableObject
{
    private readonly IPuzzleGenerator puzzleGenerator;
    private readonly ILogger logger;

    public MainViewModel(IPuzzleGenerator puzzleGenerator, ILogger logger)
    {
        this.puzzleGenerator = puzzleGenerator ?? throw new ArgumentNullException(nameof(puzzleGenerator));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
```

### Clean Code Practices

#### Naming Conventions
- **Classes**: PascalCase (`HashiGenerator`)
- **Methods**: PascalCase (`GenerateAsync`)
- **Properties**: PascalCase (`CurrentSolution`)
- **Fields**: camelCase with underscore prefix (`_currentSolution`)
- **Constants**: PascalCase (`MaxRetryAttempts`)
- **Local Variables**: camelCase (`puzzleData`)

#### Method Guidelines
```csharp
// Good - Clear, focused method
public async Task<ISolutionProvider> GeneratePuzzleAsync(
    int width, 
    int height, 
    DifficultyEnum difficulty)
{
    // Validate parameters
    if (width <= 0) throw new ArgumentException("Width must be positive", nameof(width));
    if (height <= 0) throw new ArgumentException("Height must be positive", nameof(height));
    
    // Single purpose implementation
    var parameters = new GenerationParameters
    {
        Width = width,
        Height = height,
        Difficulty = difficulty
    };
    
    return await GenerateInternalAsync(parameters);
}
```

#### Error Handling
```csharp
// Good - Specific exception handling
public async Task<ISolutionProvider> LoadPuzzleAsync(string filename)
{
    try
    {
        if (string.IsNullOrWhiteSpace(filename))
            throw new ArgumentException("Filename cannot be null or empty", nameof(filename));
        
        if (!File.Exists(filename))
            throw new FileNotFoundException($"Puzzle file not found: {filename}");
        
        var json = await File.ReadAllTextAsync(filename);
        return JsonConvert.DeserializeObject<SolutionProvider>(json);
    }
    catch (JsonException ex)
    {
        logger.Error($"Failed to parse puzzle file: {filename}", ex);
        throw new InvalidDataException($"Invalid puzzle file format: {filename}", ex);
    }
}
```

### Documentation Standards

#### XML Documentation
All public members must have XML documentation:

```csharp
/// <summary>
/// Generates a new Hashiwokakero puzzle with specified parameters.
/// </summary>
/// <param name="parameters">The generation parameters including size and difficulty.</param>
/// <returns>A task that represents the asynchronous generate operation. The task result contains the generated puzzle.</returns>
/// <exception cref="ArgumentNullException">Thrown when parameters is null.</exception>
/// <exception cref="InvalidOperationException">Thrown when puzzle generation fails after maximum retries.</exception>
public async Task<ISolutionProvider> GenerateAsync(GenerationParameters parameters)
{
    // Implementation
}
```

#### Interface Documentation
Use `<inheritdoc />` for implementations:

```csharp
public interface IPuzzleGenerator
{
    /// <summary>
    /// Generates a new puzzle with the specified parameters.
    /// </summary>
    /// <param name="parameters">The generation parameters.</param>
    /// <returns>The generated puzzle solution.</returns>
    Task<ISolutionProvider> GenerateAsync(GenerationParameters parameters);
}

public class PuzzleGenerator : IPuzzleGenerator
{
    /// <inheritdoc />
    public async Task<ISolutionProvider> GenerateAsync(GenerationParameters parameters)
    {
        // Implementation
    }
}
```

### Dependency Injection

#### Constructor Injection
Always use constructor injection for dependencies:

```csharp
public class MainViewModel : ObservableObject
{
    private readonly IPuzzleGenerator puzzleGenerator;
    private readonly ITimerProvider timerProvider;
    private readonly ILogger logger;

    public MainViewModel(
        IPuzzleGenerator puzzleGenerator,
        ITimerProvider timerProvider,
        ILogger logger)
    {
        this.puzzleGenerator = puzzleGenerator ?? throw new ArgumentNullException(nameof(puzzleGenerator));
        this.timerProvider = timerProvider ?? throw new ArgumentNullException(nameof(timerProvider));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
```

#### Autofac Module Pattern
Create focused modules for registration:

```csharp
public class AutoFacViewModelsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register ViewModels
        builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();
        builder.RegisterType<DialogViewModel>().AsSelf().InstancePerDependency();
        builder.RegisterType<SettingsViewModel>().AsSelf().SingleInstance();
    }
}
```

#### Interface Creation
Every concrete class should have an interface:

```csharp
// Define interface first
public interface ITimerProvider
{
    TimeSpan ElapsedTime { get; }
    bool IsRunning { get; }
    void Start();
    void Stop();
    void Reset();
    event EventHandler<TimeSpan> Tick;
}

// Then implement
public class TimerProvider : ITimerProvider
{
    // Implementation
}
```

## MVVM Pattern Implementation

### ViewModel Guidelines

#### Use CommunityToolkit.Mvvm
```csharp
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Hashiwokakero";

    [ObservableProperty]
    private GameStateEnum gameState = GameStateEnum.NotStarted;

    [RelayCommand]
    private async Task NewGameAsync()
    {
        // Command implementation
    }

    [RelayCommand(CanExecute = nameof(CanSolveGame))]
    private async Task SolveGameAsync()
    {
        // Command implementation
    }

    private bool CanSolveGame() => gameState == GameStateEnum.Playing;
}
```

#### Property Change Notifications
```csharp
public class CustomViewModel : ObservableObject
{
    private string customProperty;

    public string CustomProperty
    {
        get => customProperty;
        set
        {
            if (SetProperty(ref customProperty, value))
            {
                // Additional logic when property changes
                OnPropertyChanged(nameof(DisplayText));
            }
        }
    }

    public string DisplayText => $"Value: {CustomProperty}";
}
```

### View Guidelines

#### Data Binding
Use proper binding modes and validation:

```xml
<!-- Good - Proper binding with validation -->
<TextBox Text="{Binding PlayerName, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged, ValidatesOnDataErrors=True}"
         Style="{StaticResource ValidationTextBoxStyle}" />

<!-- Command binding -->
<Button Command="{Binding NewGameCommand}"
        Content="New Game"
        Style="{StaticResource PrimaryButtonStyle}" />
```

#### Resource Management
```xml
<!-- Define resources in appropriate scope -->
<UserControl.Resources>
    <local:BooleanToVisibilityConverter x:Key="BoolToVisConverter" />
    <local:DifficultyToStringConverter x:Key="DifficultyConverter" />
</UserControl.Resources>
```

## Testing Strategy

### Unit Test Structure

#### Test Class Organization
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

    [TearDown]
    public void TearDown()
    {
        mockLogger.VerifyAll();
        mockValidator.VerifyAll();
    }
}
```

#### Test Naming Convention
Format: `[MethodName]_When[TestConditions]_Should[ExpectedResult]`

```csharp
[Test]
public async Task GenerateAsync_WhenValidParameters_ShouldReturnSolution()
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
}
```

#### Mock Configuration
Use `MockBehavior.Strict` for all mocks:

```csharp
[Test]
public async Task LoadPuzzleAsync_WhenFileNotFound_ShouldThrowFileNotFoundException()
{
    // Arrange
    var filename = "nonexistent.json";
    var mockFileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
    
    mockFileSystem.Setup(fs => fs.FileExists(filename))
                  .Returns(false);

    var loader = new PuzzleLoader(mockFileSystem.Object);

    // Act & Assert
    await loader.Invoking(l => l.LoadPuzzleAsync(filename))
                .Should().ThrowAsync<FileNotFoundException>()
                .WithMessage($"*{filename}*");
}
```

### Test Categories

#### Unit Tests
- Test individual components in isolation
- Mock all dependencies
- Focus on single method behavior

#### Integration Tests
- Test component interactions
- Use real implementations where appropriate
- Verify end-to-end workflows

#### UI Tests
- Test ViewModel command execution
- Verify property change notifications
- Test binding scenarios

## Error Handling

### Exception Strategy

#### Custom Exceptions
```csharp
public class PuzzleGenerationException : Exception
{
    public GenerationParameters Parameters { get; }

    public PuzzleGenerationException(GenerationParameters parameters, string message)
        : base(message)
    {
        Parameters = parameters;
    }

    public PuzzleGenerationException(GenerationParameters parameters, string message, Exception innerException)
        : base(message, innerException)
    {
        Parameters = parameters;
    }
}
```

#### Logging Integration
```csharp
public async Task<ISolutionProvider> GenerateAsync(GenerationParameters parameters)
{
    try
    {
        logger.Info($"Starting puzzle generation with parameters: {JsonConvert.SerializeObject(parameters)}");
        
        var result = await GenerateInternalAsync(parameters);
        
        logger.Info($"Puzzle generation completed successfully");
        return result;
    }
    catch (Exception ex)
    {
        logger.Error($"Puzzle generation failed for parameters: {JsonConvert.SerializeObject(parameters)}", ex);
        throw new PuzzleGenerationException(parameters, "Failed to generate puzzle", ex);
    }
}
```

### User-Friendly Error Messages
```csharp
public class ErrorMessageProvider
{
    public string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            FileNotFoundException => "The puzzle file could not be found. Please check the file path and try again.",
            UnauthorizedAccessException => "You don't have permission to access this file. Please check your permissions.",
            JsonException => "The puzzle file is corrupted or has an invalid format. Please try a different file.",
            PuzzleGenerationException => "Unable to generate a puzzle with the specified settings. Please try different difficulty or size settings.",
            _ => "An unexpected error occurred. Please try again or contact support if the problem persists."
        };
    }
}
```

## Performance Guidelines

### Async/Await Best Practices
```csharp
// Good - Proper async implementation
public async Task<ISolutionProvider> GenerateAsync(GenerationParameters parameters)
{
    // Use ConfigureAwait(false) in library code
    var result = await GenerateInternalAsync(parameters).ConfigureAwait(false);
    return result;
}

// Good - Avoid async void except for event handlers
public async void OnNewGameClick(object sender, EventArgs e)
{
    try
    {
        await NewGameAsync();
    }
    catch (Exception ex)
    {
        logger.Error("Error in new game command", ex);
        // Handle error appropriately
    }
}
```

### Memory Management
```csharp
// Implement IDisposable when needed
public class PuzzleGenerator : IPuzzleGenerator, IDisposable
{
    private bool disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
            }
            disposed = true;
        }
    }
}
```

## Git Workflow

### Branch Strategy
- **main**: Production-ready code
- **develop**: Integration branch for features
- **feature/***: Feature development branches
- **bugfix/***: Bug fix branches
- **hotfix/***: Critical production fixes

### Commit Message Format
```
<type>(<scope>): <description>

<body>

<footer>
```

**Types**: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`

**Example**:
```
feat(generator): add support for custom puzzle sizes

- Allow users to specify custom width and height
- Add validation for minimum and maximum sizes
- Update UI to support size selection

Closes #123
```

### Pull Request Process
1. **Create Feature Branch**: `git checkout -b feature/new-feature`
2. **Implement Changes**: Follow coding standards
3. **Add Tests**: Ensure test coverage
4. **Update Documentation**: Keep docs current
5. **Create PR**: Use template, assign reviewers
6. **Address Feedback**: Make requested changes
7. **Merge**: Squash and merge to main

## Code Review Guidelines

### What to Review
- **Correctness**: Does the code do what it's supposed to do?
- **Design**: Is the code well-designed and appropriate?
- **Functionality**: Does it work as expected?
- **Complexity**: Could it be simpler?
- **Tests**: Are there appropriate tests?
- **Naming**: Are names clear and descriptive?
- **Comments**: Are comments useful and necessary?

### Review Checklist
- [ ] Code follows SOLID principles
- [ ] All public members have XML documentation
- [ ] Unit tests cover new functionality
- [ ] No magic numbers or strings
- [ ] Proper error handling
- [ ] Async/await used correctly
- [ ] Dependencies injected properly
- [ ] No code duplication

## Debugging

### Logging for Debugging
```csharp
public async Task<ISolutionProvider> GenerateAsync(GenerationParameters parameters)
{
    logger.Debug($"Starting generation with parameters: {JsonConvert.SerializeObject(parameters)}");
    
    var stopwatch = Stopwatch.StartNew();
    
    try
    {
        var result = await GenerateInternalAsync(parameters);
        
        logger.Debug($"Generation completed in {stopwatch.ElapsedMilliseconds}ms");
        return result;
    }
    catch (Exception ex)
    {
        logger.Error($"Generation failed after {stopwatch.ElapsedMilliseconds}ms", ex);
        throw;
    }
}
```

### Visual Studio Debugging
- **Breakpoints**: Use conditional breakpoints for complex scenarios
- **Watch Windows**: Monitor variable values
- **Call Stack**: Understand execution flow
- **Immediate Window**: Test expressions during debugging

### Common Debugging Scenarios
- **MVVM Binding Issues**: Check Output window for binding errors
- **Dependency Injection**: Verify registrations in container
- **Async/Await**: Watch for deadlocks and proper exception handling
- **Performance**: Use profiling tools for bottlenecks

---

*For testing patterns and conventions, see [Testing Strategy](Testing-Strategy.md)*