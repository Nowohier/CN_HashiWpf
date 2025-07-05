# Core Components

This document provides detailed documentation for the core components of the Hashiwokakero WPF application.

## GUI Layer Components

### MainViewModel
The central view model that orchestrates the entire application.

**Location**: `Gui/Hashi.Gui/ViewModels/MainViewModel.cs`

**Key Responsibilities**:
- Game state management
- Puzzle generation coordination
- UI command handling
- Timer and scoring management

**Key Properties**:
```csharp
public class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private GameStateEnum gameState;

    [ObservableProperty]
    private ISolutionProvider? currentSolution;

    [ObservableProperty]
    private TimeSpan elapsedTime;

    [ObservableProperty]
    private int score;
}
```

**Key Commands**:
- `NewGameCommand`: Initiates new puzzle generation
- `SolveCommand`: Solves the current puzzle
- `HintCommand`: Provides gameplay hints
- `SaveGameCommand`: Saves current progress
- `LoadGameCommand`: Loads saved game

### DialogViewModel
Handles all modal dialogs in the application.

**Location**: `Gui/Hashi.Gui/ViewModels/DialogViewModel.cs`

**Features**:
- Multiple dialog types (OK, OK/Cancel, Yes/No, Yes/No/Cancel)
- Image support (Information, Warning, Error, Question)
- Customizable button text and behavior

**Usage Pattern**:
```csharp
var dialogVm = new DialogViewModel(
    title: "Confirm Action",
    content: "Are you sure you want to start a new game?",
    button: DialogButton.YesNo,
    image: DialogImage.Question);
```

### Game Board Management

#### HashiPoint
Represents individual points (islands) on the game board.

**Interface**: `IHashiPoint`
```csharp
public interface IHashiPoint
{
    int X { get; set; }
    int Y { get; set; }
    HashiPointTypeEnum PointType { get; set; }
    int Value { get; set; }
    int ConnectedBridges { get; set; }
}
```

**Point Types**:
- `Empty`: No island
- `Island`: Numbered island (1-8)
- `Bridge`: Bridge connection
- `Highlight`: Highlighted for hints

#### HashiBridge
Represents bridge connections between islands.

**Interface**: `IHashiBridge`
```csharp
public interface IHashiBridge
{
    IHashiPoint StartPoint { get; set; }
    IHashiPoint EndPoint { get; set; }
    int BridgeCount { get; set; }
    BridgeOperationTypeEnum OperationType { get; set; }
}
```

**Operations**:
- `Add`: Add a bridge
- `Remove`: Remove a bridge
- `Toggle`: Toggle bridge state

### Resource Management

#### ResourceManager
Handles application resources and initial setup.

**Interface**: `IResourceManager`
```csharp
public interface IResourceManager
{
    void PrepareUi();
    void ResetSettingsAndLoadFromDefault();
}
```

**Responsibilities**:
- Create necessary directories and files
- Load default settings and configurations
- Prepare embedded resources
- Initialize localization resources

## Application Services

### SettingsProvider
Manages application settings and preferences.

**Interface**: `ISettingsProvider`
```csharp
public interface ISettingsProvider
{
    T GetSetting<T>(string key, T defaultValue = default);
    void SetSetting<T>(string key, T value);
    void SaveSettings();
    void LoadSettings();
    void ResetToDefaults();
}
```

**Settings Categories**:
- **Game Settings**: Default difficulty, grid size, hints enabled
- **UI Settings**: Theme, language, window size and position
- **Performance Settings**: Animation preferences, resource limits

### TimerProvider
Handles game timing functionality.

**Interface**: `ITimerProvider`
```csharp
public interface ITimerProvider
{
    TimeSpan ElapsedTime { get; }
    bool IsRunning { get; }
    void Start();
    void Stop();
    void Reset();
    void Pause();
    void Resume();
    event EventHandler<TimeSpan> Tick;
}
```

**Features**:
- High-precision timing
- Pause/resume functionality
- Event-based time updates
- Integration with game state

### HintProvider
Provides intelligent hints using the rules engine.

**Interface**: `IHintProvider`
```csharp
public interface IHintProvider
{
    Task<IHint> GetHintAsync(ISolutionProvider solution);
    Task<IEnumerable<IHint>> GetAllHintsAsync(ISolutionProvider solution);
    bool CanProvideHint(ISolutionProvider solution);
}
```

**Hint Types**:
- **Forced Bridges**: Bridges that must be placed
- **Impossible Bridges**: Bridges that cannot be placed
- **Isolation Warnings**: Potential island isolation
- **Completion Hints**: Near-completion suggestions

## Domain Layer Components

### Puzzle Generator

#### HashiGenerator
Core puzzle generation engine.

**Interface**: `IHashiGenerator`
```csharp
public interface IHashiGenerator
{
    Task<ISolutionProvider> GenerateAsync(
        int numberOfIslands,
        int width,
        int height,
        int difficulty,
        bool checkDifficulty = true);
    
    Task<ISolutionProvider> GenerateAsync(GenerationParameters parameters);
}
```

**Generation Algorithm**:
1. **Island Placement**: Randomly place islands on grid
2. **Bridge Generation**: Create initial bridge network
3. **Constraint Application**: Apply difficulty-based constraints
4. **Validation**: Ensure puzzle has unique solution
5. **Optimization**: Minimize/maximize certain characteristics

**Difficulty Levels**:
- **Easy (1-3)**: Few islands, simple connections
- **Medium (4-6)**: More islands, moderate complexity
- **Hard (7-9)**: Many islands, complex patterns
- **Expert (10)**: Maximum complexity, advanced techniques required

#### GenerationParameters
Configuration for puzzle generation.

```csharp
public class GenerationParameters
{
    public int Width { get; set; } = 10;
    public int Height { get; set; } = 10;
    public int NumberOfIslands { get; set; } = 20;
    public DifficultyEnum Difficulty { get; set; } = DifficultyEnum.Medium1;
    public bool CheckDifficulty { get; set; } = true;
    public int MaxIterations { get; set; } = 1000;
    public bool AllowIsolatedIslands { get; set; } = false;
}
```

### Solver Engine

#### HashiSolver
Solves Hashiwokakero puzzles using constraint programming.

**Interface**: `IHashiSolver`
```csharp
public interface IHashiSolver
{
    Task<SolverStatusEnum> SolveAsync(int[,] grid);
    Task<SolverStatusEnum> SolveLazy(int[,] grid);
    Task<ISolutionProvider> GetSolutionAsync(int[,] grid);
    Task<bool> HasUniqueSolutionAsync(int[,] grid);
}
```

**Solver Status**:
- `Optimal`: Found optimal solution
- `Feasible`: Found valid solution
- `Infeasible`: No solution exists
- `Unknown`: Solver could not determine
- `ModelInvalid`: Invalid puzzle format

**Constraint Types**:
1. **Island Constraints**: Each island must have correct number of bridges
2. **Bridge Constraints**: At most 2 bridges between any pair of islands
3. **Connectivity**: All islands must be connected
4. **No Crossing**: Bridges cannot cross each other

#### OR-Tools Integration
The solver uses Google OR-Tools CP-SAT solver for optimal performance.

**Variable Modeling**:
```csharp
// Bridge variables: x[i,j] represents number of bridges between islands i and j
IntVar[,] bridgeVars = new IntVar[numIslands, numIslands];

// For each pair of islands that can be connected
for (int i = 0; i < numIslands; i++)
{
    for (int j = i + 1; j < numIslands; j++)
    {
        if (CanConnect(islands[i], islands[j]))
        {
            bridgeVars[i, j] = model.NewIntVar(0, 2, $"bridge_{i}_{j}");
        }
    }
}
```

### Rules Engine

#### NRules Integration
The application uses NRules for business logic and hint generation.

**Rule Structure**:
```csharp
public class ForcedBridgeRule : Rule
{
    public override void Define()
    {
        Island island = null;
        
        When()
            .Match<Island>(() => island)
            .Where(() => island.RequiredBridges == island.PossibleBridges.Count())
            .And(() => island.ConnectedBridges < island.RequiredBridges);
            
        Then()
            .Do(ctx => ctx.Insert(new ForcedBridgeHint(island)));
    }
}
```

**Rule Categories**:
- **Completion Rules**: When islands must be completed
- **Isolation Rules**: Prevent island isolation
- **Impossible Rules**: Identify impossible bridges
- **Optimization Rules**: Suggest optimal moves

#### Fact Types
```csharp
public class Island
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int RequiredBridges { get; set; }
    public int ConnectedBridges { get; set; }
    public List<Bridge> PossibleBridges { get; set; }
}

public class Bridge
{
    public Island From { get; set; }
    public Island To { get; set; }
    public int Count { get; set; }
    public bool IsPlaced { get; set; }
}
```

## Infrastructure Components

### Logging System

#### Logger Implementation
NLog-based logging with structured logging support.

**Interface**: `ILogger`
```csharp
public interface ILogger
{
    void Trace(string message);
    void Debug(string message);
    void Info(string message);
    void Warn(string message);
    void Error(string message);
    void Fatal(string message);
    void Error(string message, Exception exception);
    void Fatal(string message, Exception exception);
}
```

**Configuration**: NLog.config
```xml
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <targets>
    <target xsi:type="File"
            name="file"
            fileName="logs/hashi-${shortdate}.log"
            layout="${longdate} ${level:uppercase=true} ${logger} ${message} ${exception:format=tostring}" />
  </targets>
  <rules>
    <logger name="*" minlevel="Debug" writeTo="file" />
  </rules>
</nlog>
```

#### LoggerFactory
Creates logger instances for different components.

**Interface**: `ILoggerFactory`
```csharp
public interface ILoggerFactory
{
    ILogger CreateLogger<T>();
    ILogger CreateLogger(string name);
}
```

### Puzzle I/O

#### PuzzleLoader
Loads and saves puzzles from/to files.

**Interface**: `IPuzzleLoader`
```csharp
public interface IPuzzleLoader
{
    Task<ISolutionProvider> LoadPuzzleAsync(string filename);
    Task SavePuzzleAsync(ISolutionProvider puzzle, string filename);
    Task<IEnumerable<string>> GetAvailablePuzzlesAsync();
}
```

**Supported Formats**:
- **JSON**: Primary format for puzzle storage
- **XML**: Alternative format for interoperability
- **Binary**: Compressed format for large puzzles

#### File Structure
```json
{
  "name": "Easy Puzzle 1",
  "difficulty": 1,
  "width": 10,
  "height": 10,
  "islands": [
    { "x": 1, "y": 1, "value": 2 },
    { "x": 3, "y": 1, "value": 3 }
  ],
  "solution": [
    { "from": { "x": 1, "y": 1 }, "to": { "x": 3, "y": 1 }, "count": 1 }
  ]
}
```

## Shared Components

### Enumerations

#### Game State
```csharp
public enum GameStateEnum
{
    NotStarted,
    Playing,
    Paused,
    Completed,
    Solved,
    Failed
}
```

#### Difficulty Levels
```csharp
public enum DifficultyEnum
{
    Easy1 = 0,
    Easy2 = 1,
    Easy3 = 2,
    Medium1 = 3,
    Medium2 = 4,
    Medium3 = 5,
    Hard1 = 6,
    Hard2 = 7,
    Hard3 = 8,
    Expert = 9
}
```

#### Bridge Operations
```csharp
public enum BridgeOperationTypeEnum
{
    Add,
    Remove,
    Toggle
}
```

### Extension Methods

#### Collection Extensions
```csharp
public static class CollectionExtensions
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);
    }
    
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
    {
        return source == null || !source.Any();
    }
}
```

#### Grid Extensions
```csharp
public static class GridExtensions
{
    public static IEnumerable<(int x, int y)> GetNeighbors(this int[,] grid, int x, int y)
    {
        // Return orthogonal neighbors within grid bounds
    }
    
    public static bool IsValidPosition(this int[,] grid, int x, int y)
    {
        return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
    }
}
```

## Performance Considerations

### Memory Management
- **Proper Disposal**: IDisposable implementation for resources
- **Weak References**: For event handlers in long-lived objects
- **Object Pooling**: For frequently created/destroyed objects

### Async Operations
- **Task-Based**: All long-running operations are async
- **ConfigureAwait**: Proper async configuration
- **Cancellation**: Support for operation cancellation

### Caching
- **Solution Caching**: Generated solutions are cached
- **Resource Caching**: UI resources are cached appropriately
- **Memory Limits**: Configurable cache size limits

---

*For development guidelines and standards, see [Development Guide](Development-Guide.md)*