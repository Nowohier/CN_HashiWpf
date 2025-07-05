# Architecture

The Hashiwokakero WPF application follows clean architecture principles with a modular, testable, and maintainable design.

## High-Level Architecture

### Architectural Pattern
The application implements a **layered architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │    Views    │  │ ViewModels  │  │  Behaviors/Helpers  │  │
│  │    (XAML)   │  │   (MVVM)    │  │    (UI Logic)       │  │
│  └─────────────┘  └─────────────┘  └─────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    Application Layer                        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │  Providers  │  │  Managers   │  │     Messages        │  │
│  │ (Services)  │  │ (Resources) │  │   (Messaging)       │  │
│  └─────────────┘  └─────────────┘  └─────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    Domain Layer                             │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │  Generator  │  │   Solver    │  │      Rules          │  │
│  │ (Puzzles)   │  │ (OR-Tools)  │  │   (NRules)          │  │
│  └─────────────┘  └─────────────┘  └─────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                 Infrastructure Layer                        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │   Logging   │  │ Puzzle I/O  │  │     Enums           │  │
│  │  (NLog)     │  │  (Files)    │  │   (Shared)          │  │
│  └─────────────┘  └─────────────┘  └─────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

### Design Principles

#### SOLID Principles
- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Derived classes are substitutable for base classes
- **Interface Segregation**: Clients depend only on interfaces they use
- **Dependency Inversion**: Depend on abstractions, not concretions

#### Clean Architecture Benefits
- **Testability**: Each layer can be tested independently
- **Maintainability**: Changes in one layer don't affect others
- **Flexibility**: Easy to swap implementations
- **Scalability**: Can add new features without major refactoring

## Project Structure

### Core Projects

#### Gui Layer
```
Gui/
├── Hashi.Gui/              # Main WPF application
│   ├── ViewModels/         # MVVM view models
│   ├── Views/              # XAML views and windows
│   ├── Behaviors/          # UI behaviors
│   ├── Converters/         # Value converters
│   ├── Providers/          # Application services
│   └── AutoFac/            # Dependency injection
├── Hashi.Gui.Interfaces/   # Interface definitions
├── Hashi.Gui.Language/     # Language resources
└── Hashi.Gui.Translation/  # Translation services
```

#### Domain Layer
```
Generator/
├── Hashi.Generator/        # Puzzle generation logic
└── Hashi.Generator.Interfaces/

Solver/
├── Hashi.LinearSolver/     # OR-Tools integration
├── Hashi.LinearSolver.Interfaces/
└── Hashi.Rules/            # NRules business rules
```

#### Infrastructure Layer
```
Logging/
├── Hashi.Logging/          # NLog implementation
└── Hashi.Logging.Interfaces/

PuzzleLoader/
├── Hashi.SolvedPuzzles/    # File I/O operations
└── Hashi.SolvedPuzzles.Interfaces/

Enums/
└── Hashi.Enums/            # Shared enumerations
```

## MVVM Pattern Implementation

### Overview
The application uses the **Model-View-ViewModel (MVVM)** pattern with CommunityToolkit.Mvvm for robust UI architecture.

### Components

#### Views (XAML)
```xml
<!-- MainWindow.xaml -->
<Window x:Class="Hashi.Gui.Views.MainWindow"
        DataContext="{Binding MainViewModel, Source={StaticResource Locator}}">
    <Grid>
        <!-- UI Elements bind to ViewModel properties -->
    </Grid>
</Window>
```

#### ViewModels
```csharp
public class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Hashiwokakero";

    [RelayCommand]
    private async Task NewGameAsync()
    {
        // Command implementation
    }
}
```

#### Models
```csharp
public interface IHashiPoint
{
    int X { get; set; }
    int Y { get; set; }
    HashiPointTypeEnum PointType { get; set; }
}
```

### Data Flow
1. **User Interaction** → View captures input
2. **Command Binding** → ViewModel executes commands
3. **Service Calls** → ViewModel calls application services
4. **Property Updates** → ViewModel notifies property changes
5. **UI Updates** → View reflects changes automatically

## Dependency Injection

### Autofac Container
The application uses **Autofac** for dependency injection with a modular registration approach.

#### Module Structure
```csharp
public class AutoFacMainModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register all sub-modules
        builder.RegisterModule<AutoFacViewModelsModule>();
        builder.RegisterModule<AutoFacProvidersModule>();
        builder.RegisterModule<AutoFacGeneratorModule>();
        builder.RegisterModule<AutoFacLinearSolverModule>();
        builder.RegisterModule<AutoFacRulesModule>();
        builder.RegisterModule<AutoFacLoggingModule>();
        // ... other modules
    }
}
```

#### Registration Examples
```csharp
// Singleton registration
builder.RegisterType<SettingsProvider>()
    .As<ISettingsProvider>()
    .SingleInstance();

// Factory registration
builder.Register<Func<int, int, HashiPointTypeEnum, IHashiPoint>>(context =>
{
    var c = context.Resolve<IComponentContext>();
    return (x, y, pointType) => c.Resolve<IHashiPoint>(
        new NamedParameter("x", x),
        new NamedParameter("y", y),
        new NamedParameter("pointType", pointType));
});
```

### Benefits
- **Loose Coupling**: Components depend on interfaces
- **Testability**: Easy to mock dependencies
- **Configuration**: Centralized dependency configuration
- **Lifetime Management**: Automatic lifecycle management

## Communication Patterns

### Messaging System
The application uses a message-based communication system for decoupled component interaction.

#### Message Types
```csharp
public class GameStateChangedMessage
{
    public GameStateEnum NewState { get; set; }
    public GameStateEnum OldState { get; set; }
}

public class PuzzleGeneratedMessage
{
    public ISolutionProvider Solution { get; set; }
}
```

#### Message Handlers
```csharp
public class MainViewModel : ObservableObject
{
    public MainViewModel(IMessenger messenger)
    {
        messenger.Register<GameStateChangedMessage>(this, OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedMessage message)
    {
        // Handle state change
    }
}
```

### Provider Pattern
Application services are implemented using the Provider pattern for consistent service interfaces.

#### Service Interface
```csharp
public interface ITimerProvider
{
    TimeSpan ElapsedTime { get; }
    void Start();
    void Stop();
    void Reset();
    event EventHandler<TimeSpan> Tick;
}
```

#### Implementation
```csharp
public class TimerProvider : ITimerProvider
{
    private readonly DispatcherTimer timer;
    
    public TimerProvider()
    {
        timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        timer.Tick += OnTick;
    }
    
    // Implementation details...
}
```

## Key Architectural Components

### 1. Generator System
- **Purpose**: Creates playable puzzles with varying difficulty
- **Algorithm**: Constraint-based generation with backtracking
- **Integration**: Validates puzzles using solver before presenting

### 2. Solver Engine
- **Technology**: Google OR-Tools CP-SAT solver
- **Approach**: Constraint satisfaction problem modeling
- **Features**: Optimal solution finding, solution verification

### 3. Rules Engine
- **Technology**: NRules framework
- **Purpose**: Hint generation and game validation
- **Benefits**: Declarative rule definition, easy rule modification

### 4. Logging System
- **Technology**: NLog with structured logging
- **Configuration**: Configurable output targets and levels
- **Integration**: Dependency injection for testability

## Performance Considerations

### UI Performance
- **Virtualization**: Large grids use UI virtualization
- **Async Operations**: Long-running tasks are asynchronous
- **Resource Management**: Proper disposal of resources

### Algorithm Performance
- **Caching**: Generated puzzles are cached when appropriate
- **Parallel Processing**: Multi-threading for generation algorithms
- **Memory Management**: Efficient data structures for large grids

### Scalability
- **Modular Design**: Easy to add new difficulty levels
- **Extensible Rules**: New game rules can be added easily
- **Plugin Architecture**: Support for future extensions

## Testing Architecture

### Test Structure
Each production project has corresponding test projects following the same architectural patterns.

### Test Categories
- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test component interactions
- **UI Tests**: Test user interface behavior (when applicable)

### Test Patterns
- **Arrange-Act-Assert**: Consistent test structure
- **Mock Behavior**: Strict mocking with Moq
- **Fluent Assertions**: Readable test assertions

---

*For detailed component documentation, see [Core Components](Core-Components.md)*