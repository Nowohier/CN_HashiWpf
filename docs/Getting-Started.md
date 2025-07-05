# Getting Started

This guide will help you get the Hashiwokakero WPF application up and running on your development machine.

## Prerequisites

### System Requirements
- **Operating System**: Windows 10 or later
- **Framework**: .NET 8.0 SDK or later
- **Development Environment**: Visual Studio 2022 (recommended) or Visual Studio Code
- **Memory**: 4GB RAM minimum, 8GB recommended
- **Storage**: 500MB available space

### Required Software
1. **[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**
   - Download and install the latest .NET 8.0 SDK
   - Verify installation: `dotnet --version`

2. **Visual Studio 2022** (recommended)
   - Community, Professional, or Enterprise edition
   - Include the following workloads:
     - .NET Desktop Development
     - .NET 8.0 targeting pack
   - Or use **Visual Studio Code** with C# extension

3. **Git** (for version control)
   - Download from [git-scm.com](https://git-scm.com/)

## Installation

### 1. Clone the Repository
```bash
git clone https://github.com/Nowohier/CN_HashiWpf.git
cd CN_HashiWpf
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Build the Solution
```bash
# Build entire solution
dotnet build

# Or build specific configuration
dotnet build --configuration Release
```

### 4. Run the Application
```bash
# Run from command line
dotnet run --project Gui/Hashi.Gui/Hashi.Gui.csproj

# Or open in Visual Studio and press F5
```

## Building from Source

### Visual Studio
1. Open `CN_HashiWpf.sln` in Visual Studio
2. Select **Build > Build Solution** (Ctrl+Shift+B)
3. Press **F5** to run with debugging or **Ctrl+F5** to run without debugging

### Command Line
```bash
# Debug build
dotnet build --configuration Debug

# Release build
dotnet build --configuration Release

# Clean and rebuild
dotnet clean
dotnet build
```

## Running Tests

### All Tests
```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run with coverage (if configured)
dotnet test --collect:"XPlat Code Coverage"
```

### Specific Test Projects
```bash
# Run specific test project
dotnet test Tests/Hashi.Generator.Test/Hashi.Generator.Test.csproj

# Run tests matching a pattern
dotnet test --filter "TestCategory=Unit"
```

## Project Structure Overview

```
CN_HashiWpf/
├── Enums/                     # Shared enumerations
│   └── Hashi.Enums/
├── Generator/                 # Puzzle generation logic
│   ├── Hashi.Generator/
│   └── Hashi.Generator.Interfaces/
├── Gui/                       # WPF application
│   ├── Hashi.Gui/             # Main WPF app
│   ├── Hashi.Gui.Interfaces/
│   ├── Hashi.Gui.Language/
│   └── Hashi.Gui.Translation/
├── Logging/                   # Logging infrastructure
│   ├── Hashi.Logging/
│   └── Hashi.Logging.Interfaces/
├── PuzzleLoader/              # Puzzle file loading
│   ├── Hashi.SolvedPuzzles/
│   └── Hashi.SolvedPuzzles.Interfaces/
├── Solver/                    # Solving algorithms
│   ├── Hashi.LinearSolver/
│   ├── Hashi.LinearSolver.Interfaces/
│   └── Hashi.Rules/
└── Tests/                     # Unit tests
    ├── Hashi.Generator.Test/
    ├── Hashi.Gui.Test/
    ├── Hashi.LinearSolver.Test/
    ├── Hashi.Logging.Tests/
    ├── Hashi.Rules.Test/
    └── Hashi.SolvedPuzzles.Test/
```

## First Run

### 1. Launch the Application
After building successfully, launch the application. You should see the main game window with:
- Menu bar with File, Game, and Help options
- Game grid area for puzzle display
- Control panel with difficulty settings
- Status bar showing game information

### 2. Generate Your First Puzzle
1. Go to **Game > New Game** (or press Ctrl+N)
2. Select a difficulty level (Easy 1 recommended for first try)
3. Choose grid size (10x10 is good for beginners)
4. Click **Generate** to create a new puzzle

### 3. Playing the Game
1. **Click between islands** to place bridges
2. **Click again** to add a second bridge
3. **Right-click** to remove bridges
4. **Use hints** if you get stuck (Game > Hint)
5. **Solve automatically** for learning (Game > Solve)

## Development Environment Setup

### Visual Studio Configuration
1. Install recommended extensions:
   - **XAML Styler** for XAML formatting
   - **CodeMaid** for code cleanup
   - **EditorConfig** for consistent formatting

2. Configure debugging:
   - Set `Hashi.Gui` as startup project
   - Enable "Enable .NET Framework source stepping"
   - Configure exception settings for debugging

### VS Code Configuration
1. Install extensions:
   - **C# Extension Pack**
   - **XAML Language Support**
   - **EditorConfig for VS Code**

2. Configure tasks and launch settings in `.vscode/`

## Common Issues

### Build Errors
- **Error MSB4019**: Missing Windows Desktop SDK
  - Solution: Install .NET Desktop Development workload in Visual Studio

- **Package restore failed**: Network or proxy issues
  - Solution: Check internet connection, configure proxy if needed

### Runtime Issues
- **Application won't start**: Missing .NET 8.0 runtime
  - Solution: Install .NET 8.0 Desktop Runtime

- **UI not displaying correctly**: Missing WPF dependencies
  - Solution: Ensure Windows Desktop Runtime is installed

## Next Steps

Once you have the application running:

1. **Explore the [User Guide](User-Guide.md)** to learn how to play
2. **Read the [Architecture](Architecture.md)** documentation to understand the codebase
3. **Check the [Development Guide](Development-Guide.md)** for coding standards
4. **Review [Testing Strategy](Testing-Strategy.md)** for test patterns

## Support

If you encounter issues:
- Check [Troubleshooting](Troubleshooting.md) for common solutions
- Review closed issues on GitHub
- Create a new issue with detailed error information

---

*Ready to start developing? Check out the [Development Guide](Development-Guide.md) next!*