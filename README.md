![License](https://img.shields.io/github/license/Nowohier/CN_HashiWpf?cacheSeconds=1)
![GitHub stars](https://img.shields.io/github/stars/Nowohier/CN_HashiWpf)
![.NET Core Desktop](https://github.com/Nowohier/CN_HashiWpf/actions/workflows/dotnet-desktop.yml/badge.svg)

# Hashiwokakero ("build bridges!")

Hashiwokakero is a type of logic puzzle. It is played on a rectangular grid with no standard size, although the grid itself is not usually drawn. Some cells start out with (usually encircled) numbers from 1 to 8 inclusive; these are the "islands". The rest of the cells are empty.

App screenshot:
![App screenshot](/Gui/Hashi.Gui/Resources/hashi_screenshot.png?raw=true)

The goal is to connect all of the islands by drawing a series of bridges between the islands. The bridges must follow certain criteria:

- They must begin and end at distinct islands, travelling a straight line in between.
- They must not cross any other bridges or islands.
- They may only run orthogonally (i.e. they may not run diagonally).
- At most two bridges connect a pair of islands.
- The number of bridges connected to each island must match the number on that island.
- The bridges must connect the islands into a single connected group.

## Tech Stack

| Category | Technology |
|---|---|
| **Platform** | .NET 8.0 / C# 12 |
| **UI Framework** | WPF (Windows Presentation Foundation) |
| **UI Theming** | MahApps.Metro 2.4 |
| **MVVM Toolkit** | CommunityToolkit.Mvvm 8.4 |
| **Rule Engine** | NRules 1.0 |
| **Logging** | NLog 5.3 |
| **Dependency Injection** | Microsoft.Extensions.DependencyInjection |
| **Testing** | NUnit 4.2 + FluentAssertions 7.0 + Moq 4.20 |

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Windows 10/11 (required for WPF)
- (Optional) Visual Studio 2022 17.x or JetBrains Rider

## Installation & Build

```bash
# Clone the repository
git clone https://github.com/Nowohier/CN_HashiWpf.git
cd CN_HashiWpf

# Restore dependencies
dotnet restore

# Build the solution
dotnet build CN_HashiWpf.sln --configuration Release
```

## Running the Application

```bash
dotnet run --project Gui/Hashi.Gui/Hashi.Gui.csproj
```

Or open `CN_HashiWpf.sln` in Visual Studio and run the `Hashi.Gui` project.

## Running Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal
```

## Project Structure

```
CN_HashiWpf/
├── Gui/                    # WPF application (MVVM, theming, translations)
├── Generator/              # Puzzle generation
├── Solver/                 # Linear solver + NRules-based solver
├── PuzzleLoader/           # Loads pre-built puzzle files (.has format)
├── Logging/                # Centralized NLog logging
├── Enums/                  # Shared enumerations
└── Tests/                  # Unit tests (NUnit)
```

## Solution Methods
Solving a Hashiwokakero puzzle is a matter of procedural force: having determined where a bridge must be placed, placing it there can eliminate other possible places for bridges, forcing the placement of another bridge, and so on.

An island showing '3' in a corner, '5' along the outside edge, or '7' anywhere must have at least one bridge radiating from it in each valid direction, for if one direction did not have a bridge, even if all other directions sported two bridges, not enough will have been placed. A '4' in a corner, '6' along the border, or '8' anywhere must have two bridges in each direction. This can be generalized as added bridges obstruct routes: a '3' that can only be travelled from vertically must have at least one bridge each for up and down, for example.

It is common practice to cross off or fill in islands whose bridge quota has been reached. In addition to reducing mistakes, this can also help locate potential "short circuits": keeping in mind that all islands must be connected by one network of bridges, a bridge that would create a closed network that no further bridges could be added to can only be permitted if it immediately yields the solution to the complete puzzle. The simplest example of this is two islands showing '1' aligned with each other; unless they are the only two islands in the puzzle, they cannot be connected by a bridge, as that would complete a network that cannot be added to, and would therefore force those two islands to be unreachable by any others.

Any bridge that would completely isolate a group of islands from another group would not be permitted, as one would then have two groups of islands that could not connect.

All infos from Wikipedia (https://en.wikipedia.org/wiki/Hashiwokakero)

