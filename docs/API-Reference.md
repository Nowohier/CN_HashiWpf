# API Reference

This document provides comprehensive reference documentation for the key interfaces, classes, and APIs in the Hashiwokakero WPF application.

## Core Interfaces

### Puzzle Generation

#### IPuzzleGenerator
Generates new Hashiwokakero puzzles with specified parameters.

```csharp
namespace Hashi.Generator.Interfaces
{
    /// <summary>
    /// Interface for generating Hashiwokakero puzzles.
    /// </summary>
    public interface IPuzzleGenerator
    {
        /// <summary>
        /// Generates a new puzzle asynchronously with the specified parameters.
        /// </summary>
        /// <param name="numberOfIslands">The number of islands to place in the puzzle.</param>
        /// <param name="sizeLength">The width of the puzzle grid.</param>
        /// <param name="sizeWidth">The height of the puzzle grid.</param>
        /// <param name="difficulty">The difficulty level of the puzzle.</param>
        /// <param name="beta">Additional difficulty parameter for fine-tuning.</param>
        /// <param name="checkDifficulty">Whether to validate the difficulty level.</param>
        /// <returns>A task representing the asynchronous operation containing the generated puzzle.</returns>
        Task<ISolutionProvider> GenerateHashAsync(int numberOfIslands, int sizeLength, int sizeWidth, 
            int difficulty, int beta, bool checkDifficulty);

        /// <summary>
        /// Creates a hash field asynchronously for puzzle generation.
        /// </summary>
        /// <param name="numberOfIslands">The number of islands to place.</param>
        /// <param name="sizeLength">The length of the grid.</param>
        /// <param name="sizeWidth">The width of the grid.</param>
        /// <param name="difficulty">The difficulty level.</param>
        /// <param name="beta">Additional difficulty parameter.</param>
        /// <param name="checkDifficulty">Whether to validate difficulty.</param>
        /// <returns>A task representing the asynchronous operation containing the field array.</returns>
        Task<int[][]> CreateHashAsync(int numberOfIslands, int sizeLength, int sizeWidth, 
            int difficulty, int beta, bool checkDifficulty);
    }
}
```

**Usage Example**:
```csharp
var generator = container.Resolve<IPuzzleGenerator>();
var puzzle = await generator.GenerateHashAsync(20, 15, 15, 5, 0, true);
```

#### GenerationParameters
Configuration object for puzzle generation.

```csharp
/// <summary>
/// Parameters for puzzle generation.
/// </summary>
public class GenerationParameters
{
    /// <summary>
    /// Gets or sets the width of the puzzle grid.
    /// </summary>
    public int Width { get; set; } = 15;

    /// <summary>
    /// Gets or sets the height of the puzzle grid.
    /// </summary>
    public int Height { get; set; } = 15;

    /// <summary>
    /// Gets or sets the number of islands to place.
    /// </summary>
    public int NumberOfIslands { get; set; } = 20;

    /// <summary>
    /// Gets or sets the difficulty level.
    /// </summary>
    public DifficultyEnum Difficulty { get; set; } = DifficultyEnum.Medium1;

    /// <summary>
    /// Gets or sets whether to validate difficulty.
    /// </summary>
    public bool CheckDifficulty { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum generation attempts.
    /// </summary>
    public int MaxIterations { get; set; } = 1000;
}
```

### Puzzle Solving

#### IHashiSolver
Solves Hashiwokakero puzzles using constraint programming.

```csharp
namespace Hashi.LinearSolver.Interfaces
{
    /// <summary>
    /// Interface for solving Hashiwokakero puzzles using linear programming.
    /// </summary>
    public interface IHashiSolver
    {
        /// <summary>
        /// Solves the puzzle asynchronously using the CP-SAT solver.
        /// </summary>
        /// <param name="grid">The puzzle grid with island values.</param>
        /// <returns>The status of the solving operation.</returns>
        Task<SolverStatusEnum> SolveAsync(int[,] grid);

        /// <summary>
        /// Solves the puzzle lazily with early termination.
        /// </summary>
        /// <param name="grid">The puzzle grid with island values.</param>
        /// <returns>The status of the solving operation.</returns>
        Task<SolverStatusEnum> SolveLazy(int[,] grid);

        /// <summary>
        /// Gets the complete solution for the puzzle.
        /// </summary>
        /// <param name="grid">The puzzle grid with island values.</param>
        /// <returns>The complete solution including bridge placements.</returns>
        Task<ISolutionProvider> GetSolutionAsync(int[,] grid);

        /// <summary>
        /// Checks if the puzzle has a unique solution.
        /// </summary>
        /// <param name="grid">The puzzle grid with island values.</param>
        /// <returns>True if the puzzle has exactly one solution.</returns>
        Task<bool> HasUniqueSolutionAsync(int[,] grid);
    }
}
```

**Usage Example**:
```csharp
var solver = container.Resolve<IHashiSolver>();
var status = await solver.SolveAsync(puzzleGrid);

if (status == SolverStatusEnum.Optimal)
{
    var solution = await solver.GetSolutionAsync(puzzleGrid);
    // Use the solution
}
```

### Solution Management

#### ISolutionProvider
Represents a complete puzzle solution with islands and bridges.

```csharp
namespace Hashi.Generator.Interfaces.Providers
{
    /// <summary>
    /// Provides access to puzzle solution data.
    /// </summary>
    public interface ISolutionProvider
    {
        /// <summary>
        /// Gets the puzzle field as a 2D array.
        /// </summary>
        IReadOnlyList<int[]>? HashiField { get; }

        /// <summary>
        /// Gets the bridge coordinates for the solution.
        /// </summary>
        List<IBridgeCoordinates>? BridgeCoordinates { get; }

        /// <summary>
        /// Gets the name or identifier of the puzzle.
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// Gets the islands in the puzzle.
        /// </summary>
        IEnumerable<IIsland> Islands { get; }

        /// <summary>
        /// Gets the bridges in the solution.
        /// </summary>
        IEnumerable<IBridge> Bridges { get; }

        /// <summary>
        /// Gets the difficulty level of the puzzle.
        /// </summary>
        DifficultyEnum Difficulty { get; }

        /// <summary>
        /// Validates the solution integrity.
        /// </summary>
        /// <returns>True if the solution is valid.</returns>
        bool IsValid();

        /// <summary>
        /// Gets the completion percentage of the current state.
        /// </summary>
        /// <returns>Percentage complete (0.0 to 1.0).</returns>
        double GetCompletionPercentage();
    }
}
```

#### IBridgeCoordinates
Represents bridge placement coordinates.

```csharp
namespace Hashi.Generator.Interfaces.Models
{
    /// <summary>
    /// Represents coordinates for bridge placement.
    /// </summary>
    public interface IBridgeCoordinates
    {
        /// <summary>
        /// Gets the starting point of the bridge.
        /// </summary>
        Point Point1 { get; }

        /// <summary>
        /// Gets the ending point of the bridge.
        /// </summary>
        Point Point2 { get; }

        /// <summary>
        /// Gets the number of bridges between the points.
        /// </summary>
        int AmountBridges { get; }

        /// <summary>
        /// Gets whether the bridge is horizontal.
        /// </summary>
        bool IsHorizontal { get; }

        /// <summary>
        /// Gets whether the bridge is vertical.
        /// </summary>
        bool IsVertical { get; }

        /// <summary>
        /// Gets the length of the bridge.
        /// </summary>
        int Length { get; }
    }
}
```

### Game Models

#### IHashiPoint
Represents a point (island or empty space) on the game grid.

```csharp
namespace Hashi.Gui.Interfaces.Models
{
    /// <summary>
    /// Represents a point on the Hashiwokakero grid.
    /// </summary>
    public interface IHashiPoint
    {
        /// <summary>
        /// Gets or sets the X coordinate.
        /// </summary>
        int X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate.
        /// </summary>
        int Y { get; set; }

        /// <summary>
        /// Gets or sets the type of point (Empty, Island, Bridge, etc.).
        /// </summary>
        HashiPointTypeEnum PointType { get; set; }

        /// <summary>
        /// Gets or sets the value of the island (1-8 for islands, 0 for empty).
        /// </summary>
        int Value { get; set; }

        /// <summary>
        /// Gets or sets the number of bridges currently connected.
        /// </summary>
        int ConnectedBridges { get; set; }

        /// <summary>
        /// Gets whether this point is an island.
        /// </summary>
        bool IsIsland { get; }

        /// <summary>
        /// Gets whether this island is complete (has all required bridges).
        /// </summary>
        bool IsComplete { get; }

        /// <summary>
        /// Gets the available connection directions.
        /// </summary>
        IEnumerable<Direction> AvailableDirections { get; }
    }
}
```

#### IHashiBridge
Represents a bridge connection between two islands.

```csharp
namespace Hashi.Gui.Interfaces.Models
{
    /// <summary>
    /// Represents a bridge between two islands.
    /// </summary>
    public interface IHashiBridge
    {
        /// <summary>
        /// Gets or sets the starting point of the bridge.
        /// </summary>
        IHashiPoint StartPoint { get; set; }

        /// <summary>
        /// Gets or sets the ending point of the bridge.
        /// </summary>
        IHashiPoint EndPoint { get; set; }

        /// <summary>
        /// Gets or sets the number of bridges (1 or 2).
        /// </summary>
        int BridgeCount { get; set; }

        /// <summary>
        /// Gets or sets the operation type for this bridge.
        /// </summary>
        BridgeOperationTypeEnum OperationType { get; set; }

        /// <summary>
        /// Gets whether the bridge is horizontal.
        /// </summary>
        bool IsHorizontal { get; }

        /// <summary>
        /// Gets whether the bridge is vertical.
        /// </summary>
        bool IsVertical { get; }

        /// <summary>
        /// Gets the length of the bridge.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Validates the bridge placement.
        /// </summary>
        /// <returns>True if the bridge placement is valid.</returns>
        bool IsValid();
    }
}
```

## Application Services

### Settings Management

#### ISettingsProvider
Manages application settings and preferences.

```csharp
namespace Hashi.Gui.Interfaces.Providers
{
    /// <summary>
    /// Provides access to application settings.
    /// </summary>
    public interface ISettingsProvider
    {
        /// <summary>
        /// Gets a setting value with optional default.
        /// </summary>
        /// <typeparam name="T">The type of the setting value.</typeparam>
        /// <param name="key">The setting key.</param>
        /// <param name="defaultValue">The default value if setting not found.</param>
        /// <returns>The setting value or default.</returns>
        T GetSetting<T>(string key, T defaultValue = default);

        /// <summary>
        /// Sets a setting value.
        /// </summary>
        /// <typeparam name="T">The type of the setting value.</typeparam>
        /// <param name="key">The setting key.</param>
        /// <param name="value">The value to set.</param>
        void SetSetting<T>(string key, T value);

        /// <summary>
        /// Saves all settings to persistent storage.
        /// </summary>
        void SaveSettings();

        /// <summary>
        /// Loads settings from persistent storage.
        /// </summary>
        void LoadSettings();

        /// <summary>
        /// Resets all settings to default values.
        /// </summary>
        void ResetToDefaults();

        /// <summary>
        /// Checks if a setting exists.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <returns>True if the setting exists.</returns>
        bool HasSetting(string key);

        /// <summary>
        /// Removes a setting.
        /// </summary>
        /// <param name="key">The setting key to remove.</param>
        void RemoveSetting(string key);

        /// <summary>
        /// Event raised when settings change.
        /// </summary>
        event EventHandler<SettingsChangedEventArgs> SettingsChanged;
    }
}
```

### Timer Services

#### ITimerProvider
Provides game timing functionality.

```csharp
namespace Hashi.Gui.Interfaces.Providers
{
    /// <summary>
    /// Provides timing services for the game.
    /// </summary>
    public interface ITimerProvider
    {
        /// <summary>
        /// Gets the elapsed time since the timer started.
        /// </summary>
        TimeSpan ElapsedTime { get; }

        /// <summary>
        /// Gets whether the timer is currently running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the timer.
        /// </summary>
        void Stop();

        /// <summary>
        /// Resets the timer to zero.
        /// </summary>
        void Reset();

        /// <summary>
        /// Pauses the timer without resetting.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes a paused timer.
        /// </summary>
        void Resume();

        /// <summary>
        /// Event raised every second while the timer is running.
        /// </summary>
        event EventHandler<TimeSpan> Tick;

        /// <summary>
        /// Event raised when timer state changes.
        /// </summary>
        event EventHandler<TimerStateChangedEventArgs> StateChanged;
    }
}
```

### Hint System

#### IHintProvider
Provides intelligent hints for puzzle solving.

```csharp
namespace Hashi.Gui.Interfaces.Providers
{
    /// <summary>
    /// Provides hints for puzzle solving.
    /// </summary>
    public interface IHintProvider
    {
        /// <summary>
        /// Gets the next available hint for the current puzzle state.
        /// </summary>
        /// <param name="solution">The current puzzle state.</param>
        /// <returns>A hint for the next move, or null if no hints available.</returns>
        Task<IHint?> GetHintAsync(ISolutionProvider solution);

        /// <summary>
        /// Gets all available hints for the current puzzle state.
        /// </summary>
        /// <param name="solution">The current puzzle state.</param>
        /// <returns>Collection of all available hints.</returns>
        Task<IEnumerable<IHint>> GetAllHintsAsync(ISolutionProvider solution);

        /// <summary>
        /// Checks if hints are available for the current state.
        /// </summary>
        /// <param name="solution">The current puzzle state.</param>
        /// <returns>True if hints are available.</returns>
        bool CanProvideHint(ISolutionProvider solution);

        /// <summary>
        /// Gets hints of a specific type.
        /// </summary>
        /// <param name="solution">The current puzzle state.</param>
        /// <param name="hintType">The type of hint to retrieve.</param>
        /// <returns>Hints of the specified type.</returns>
        Task<IEnumerable<IHint>> GetHintsByTypeAsync(ISolutionProvider solution, HintTypeEnum hintType);

        /// <summary>
        /// Event raised when new hints become available.
        /// </summary>
        event EventHandler<HintsAvailableEventArgs> HintsAvailable;
    }
}
```

#### IHint
Represents a gameplay hint.

```csharp
namespace Hashi.Gui.Interfaces.Models
{
    /// <summary>
    /// Represents a hint for puzzle solving.
    /// </summary>
    public interface IHint
    {
        /// <summary>
        /// Gets the type of hint.
        /// </summary>
        HintTypeEnum HintType { get; }

        /// <summary>
        /// Gets the description of the hint.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the islands involved in this hint.
        /// </summary>
        IEnumerable<IHashiPoint> InvolvedIslands { get; }

        /// <summary>
        /// Gets the suggested action for this hint.
        /// </summary>
        HintActionEnum SuggestedAction { get; }

        /// <summary>
        /// Gets the confidence level of this hint (0.0 to 1.0).
        /// </summary>
        double Confidence { get; }

        /// <summary>
        /// Gets the priority of this hint (higher = more important).
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Applies the hint to the puzzle state.
        /// </summary>
        /// <param name="solution">The puzzle state to modify.</param>
        void Apply(ISolutionProvider solution);

        /// <summary>
        /// Validates that this hint is still applicable.
        /// </summary>
        /// <param name="solution">The current puzzle state.</param>
        /// <returns>True if the hint is still valid.</returns>
        bool IsValid(ISolutionProvider solution);
    }
}
```

## Logging System

### ILogger
Logging interface for application-wide logging.

```csharp
namespace Hashi.Logging.Interfaces
{
    /// <summary>
    /// Provides logging functionality with multiple levels.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a trace-level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Trace(string message);

        /// <summary>
        /// Logs a debug-level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Debug(string message);

        /// <summary>
        /// Logs an information-level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Info(string message);

        /// <summary>
        /// Logs a warning-level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Warn(string message);

        /// <summary>
        /// Logs an error-level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Error(string message);

        /// <summary>
        /// Logs a fatal-level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Fatal(string message);

        /// <summary>
        /// Logs an error-level message with exception details.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        void Error(string message, Exception exception);

        /// <summary>
        /// Logs a fatal-level message with exception details.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        void Fatal(string message, Exception exception);
    }
}
```

### ILoggerFactory
Factory for creating logger instances.

```csharp
namespace Hashi.Logging.Interfaces
{
    /// <summary>
    /// Factory for creating logger instances.
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Creates a logger for the specified type.
        /// </summary>
        /// <typeparam name="T">The type to create a logger for.</typeparam>
        /// <returns>A logger instance for the type.</returns>
        ILogger CreateLogger<T>();

        /// <summary>
        /// Creates a logger with the specified name.
        /// </summary>
        /// <param name="name">The name for the logger.</param>
        /// <returns>A logger instance with the specified name.</returns>
        ILogger CreateLogger(string name);

        /// <summary>
        /// Configures the logging system.
        /// </summary>
        /// <param name="configuration">The logging configuration.</param>
        void Configure(LoggingConfiguration configuration);
    }
}
```

## Enumerations

### Core Game Enums

#### DifficultyEnum
Represents puzzle difficulty levels.

```csharp
namespace Hashi.Enums
{
    /// <summary>
    /// Represents the difficulty level of the puzzle.
    /// </summary>
    public enum DifficultyEnum
    {
        /// <summary>Easy level 1 - Beginner friendly.</summary>
        Easy1 = 0,
        
        /// <summary>Easy level 2 - Simple patterns.</summary>
        Easy2 = 1,
        
        /// <summary>Easy level 3 - Basic techniques.</summary>
        Easy3 = 2,
        
        /// <summary>Medium level 1 - Moderate challenge.</summary>
        Medium1 = 3,
        
        /// <summary>Medium level 2 - Balanced difficulty.</summary>
        Medium2 = 4,
        
        /// <summary>Medium level 3 - Advanced patterns.</summary>
        Medium3 = 5,
        
        /// <summary>Hard level 1 - Complex puzzles.</summary>
        Hard1 = 6,
        
        /// <summary>Hard level 2 - Very challenging.</summary>
        Hard2 = 7,
        
        /// <summary>Hard level 3 - Expert level.</summary>
        Hard3 = 8,
        
        /// <summary>Expert - Maximum difficulty.</summary>
        Expert = 9
    }
}
```

#### GameStateEnum
Represents the current game state.

```csharp
namespace Hashi.Enums
{
    /// <summary>
    /// Represents the current state of the game.
    /// </summary>
    public enum GameStateEnum
    {
        /// <summary>Game has not been started.</summary>
        NotStarted,
        
        /// <summary>Game is currently being played.</summary>
        Playing,
        
        /// <summary>Game is paused.</summary>
        Paused,
        
        /// <summary>Game has been completed successfully.</summary>
        Completed,
        
        /// <summary>Game was solved automatically.</summary>
        Solved,
        
        /// <summary>Game ended in failure or was abandoned.</summary>
        Failed
    }
}
```

#### SolverStatusEnum
Represents the status of puzzle solving operations.

```csharp
namespace Hashi.Enums
{
    /// <summary>
    /// Represents the status of the solver.
    /// </summary>
    public enum SolverStatusEnum
    {
        /// <summary>Status is unknown or not determined.</summary>
        Unknown = 0,
        
        /// <summary>The puzzle model is invalid.</summary>
        ModelInvalid = 1,
        
        /// <summary>A feasible solution was found.</summary>
        Feasible = 2,
        
        /// <summary>No solution exists for the puzzle.</summary>
        Infeasible = 3,
        
        /// <summary>An optimal solution was found.</summary>
        Optimal = 4
    }
}
```

#### HashiPointTypeEnum
Represents the type of a point on the game grid.

```csharp
namespace Hashi.Enums
{
    /// <summary>
    /// Represents the type of a point on the Hashiwokakero grid.
    /// </summary>
    public enum HashiPointTypeEnum
    {
        /// <summary>Empty space on the grid.</summary>
        Empty,
        
        /// <summary>An island with a number (1-8).</summary>
        Island,
        
        /// <summary>A bridge connection.</summary>
        Bridge,
        
        /// <summary>A highlighted point (for hints or selection).</summary>
        Highlight,
        
        /// <summary>An invalid or error state.</summary>
        Invalid
    }
}
```

## Event Args and Data Structures

### Event Arguments

#### SettingsChangedEventArgs
Event arguments for settings change notifications.

```csharp
/// <summary>
/// Event arguments for settings change events.
/// </summary>
public class SettingsChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the key of the setting that changed.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the old value of the setting.
    /// </summary>
    public object? OldValue { get; }

    /// <summary>
    /// Gets the new value of the setting.
    /// </summary>
    public object? NewValue { get; }

    /// <summary>
    /// Initializes a new instance of the SettingsChangedEventArgs class.
    /// </summary>
    public SettingsChangedEventArgs(string key, object? oldValue, object? newValue)
    {
        Key = key;
        OldValue = oldValue;
        NewValue = newValue;
    }
}
```

#### GameStateChangedEventArgs
Event arguments for game state changes.

```csharp
/// <summary>
/// Event arguments for game state change events.
/// </summary>
public class GameStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the previous game state.
    /// </summary>
    public GameStateEnum OldState { get; }

    /// <summary>
    /// Gets the new game state.
    /// </summary>
    public GameStateEnum NewState { get; }

    /// <summary>
    /// Gets the timestamp of the state change.
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Initializes a new instance of the GameStateChangedEventArgs class.
    /// </summary>
    public GameStateChangedEventArgs(GameStateEnum oldState, GameStateEnum newState)
    {
        OldState = oldState;
        NewState = newState;
        Timestamp = DateTime.UtcNow;
    }
}
```

## Extension Methods

### Collection Extensions

```csharp
namespace Hashi.Gui.Extensions
{
    /// <summary>
    /// Extension methods for collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Executes an action for each element in the collection.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="action">The action to execute for each element.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// Determines whether the collection is null or empty.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>True if the collection is null or empty; otherwise, false.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// Returns the collection if it's not null, otherwise returns an empty collection.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>The collection or an empty collection if null.</returns>
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T>? source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}
```

### Grid Extensions

```csharp
namespace Hashi.Gui.Extensions
{
    /// <summary>
    /// Extension methods for working with grids.
    /// </summary>
    public static class GridExtensions
    {
        /// <summary>
        /// Gets the orthogonal neighbors of a grid position.
        /// </summary>
        /// <param name="grid">The grid to search.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The valid neighboring coordinates.</returns>
        public static IEnumerable<(int x, int y)> GetNeighbors(this int[,] grid, int x, int y)
        {
            var neighbors = new[]
            {
                (x - 1, y),     // Left
                (x + 1, y),     // Right
                (x, y - 1),     // Up
                (x, y + 1)      // Down
            };

            return neighbors.Where(pos => grid.IsValidPosition(pos.Item1, pos.Item2));
        }

        /// <summary>
        /// Determines whether the specified position is valid within the grid bounds.
        /// </summary>
        /// <param name="grid">The grid to check.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>True if the position is valid; otherwise, false.</returns>
        public static bool IsValidPosition(this int[,] grid, int x, int y)
        {
            return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
        }

        /// <summary>
        /// Gets all islands (non-zero values) in the grid.
        /// </summary>
        /// <param name="grid">The grid to search.</param>
        /// <returns>Coordinates and values of all islands.</returns>
        public static IEnumerable<(int x, int y, int value)> GetIslands(this int[,] grid)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y] > 0)
                    {
                        yield return (x, y, grid[x, y]);
                    }
                }
            }
        }
    }
}
```

## Usage Examples

### Creating a Complete Game Session

```csharp
// Resolve dependencies
var generator = container.Resolve<IPuzzleGenerator>();
var solver = container.Resolve<IHashiSolver>();
var hintProvider = container.Resolve<IHintProvider>();
var timerProvider = container.Resolve<ITimerProvider>();

// Generate a new puzzle
var puzzle = await generator.GenerateHashAsync(20, 15, 15, 5, 0, true);

// Start timing
timerProvider.Start();

// Get a hint if needed
var hint = await hintProvider.GetHintAsync(puzzle);
if (hint != null)
{
    Console.WriteLine($"Hint: {hint.Description}");
}

// Solve the puzzle
var status = await solver.SolveAsync(puzzle.HashiField);
if (status == SolverStatusEnum.Optimal)
{
    Console.WriteLine("Puzzle solved successfully!");
}

// Stop timing
timerProvider.Stop();
Console.WriteLine($"Time elapsed: {timerProvider.ElapsedTime}");
```

### Working with Settings

```csharp
// Get settings provider
var settings = container.Resolve<ISettingsProvider>();

// Load user preferences
var difficulty = settings.GetSetting("Game.DefaultDifficulty", DifficultyEnum.Medium1);
var gridSize = settings.GetSetting("Game.DefaultGridSize", 15);

// Update settings
settings.SetSetting("Game.ShowHints", true);
settings.SetSetting("UI.Theme", "Dark");

// Save changes
settings.SaveSettings();

// Listen for changes
settings.SettingsChanged += (sender, args) =>
{
    Console.WriteLine($"Setting '{args.Key}' changed from '{args.OldValue}' to '{args.NewValue}'");
};
```

---

*For troubleshooting and common issues, see [Troubleshooting](Troubleshooting.md)*