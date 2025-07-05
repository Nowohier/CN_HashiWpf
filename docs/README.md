# Hashiwokakero WPF Project Documentation

This folder contains comprehensive documentation for the Hashiwokakero WPF application project.

## Documentation Overview

### Quick Navigation
- 🏠 **[Home](Home.md)** - Project overview and navigation
- 🚀 **[Getting Started](Getting-Started.md)** - Installation and setup
- 👤 **[User Guide](User-Guide.md)** - How to play and use the application

### Architecture & Development
- 🏗️ **[Architecture](Architecture.md)** - System design and patterns
- 🔧 **[Core Components](Core-Components.md)** - Detailed component documentation
- 💻 **[Development Guide](Development-Guide.md)** - Code standards and best practices
- 🧪 **[Testing Strategy](Testing-Strategy.md)** - Test patterns and conventions

### Configuration & Reference
- ⚙️ **[Configuration & Settings](Configuration-Settings.md)** - Settings and internationalization
- 📚 **[API Reference](API-Reference.md)** - Interfaces and classes documentation
- 🔧 **[Troubleshooting](Troubleshooting.md)** - Common issues and solutions

## Documentation Structure

### For Users
1. Start with **[Home](Home.md)** for project overview
2. Follow **[Getting Started](Getting-Started.md)** to install and run
3. Learn gameplay with **[User Guide](User-Guide.md)**
4. Customize with **[Configuration & Settings](Configuration-Settings.md)**

### For Developers
1. Understand the **[Architecture](Architecture.md)**
2. Study **[Core Components](Core-Components.md)**
3. Follow **[Development Guide](Development-Guide.md)** for standards
4. Implement **[Testing Strategy](Testing-Strategy.md)**
5. Reference **[API Reference](API-Reference.md)** as needed

### For Troubleshooting
- Check **[Troubleshooting](Troubleshooting.md)** for common issues
- Review relevant section based on your problem area

## Documentation Maintenance

### Contributing to Documentation
1. **Follow Markdown standards**: Use consistent formatting
2. **Keep examples current**: Update code samples with changes
3. **Cross-reference appropriately**: Link between related sections
4. **Update table of contents**: Maintain navigation consistency

### Documentation Standards
- **Clear headings**: Use descriptive section headers
- **Code examples**: Include practical, working examples
- **Screenshots**: Update images when UI changes
- **Cross-platform notes**: Mention OS-specific considerations

## Project Technology Stack

### Application Framework
- **.NET 8.0** with C# 12 language features
- **WPF** for desktop UI with MahApps.Metro styling
- **MVVM Pattern** using CommunityToolkit.Mvvm

### Architecture & Design
- **Clean Architecture** with separation of concerns
- **SOLID Principles** implementation
- **Dependency Injection** with Autofac container

### Core Technologies
- **NRules** for business rule engine and hint system
- **Google OR-Tools** for linear optimization and solving
- **NLog** for structured logging
- **Newtonsoft.Json** for serialization

### Testing & Quality
- **NUnit** testing framework
- **FluentAssertions** for readable assertions
- **Moq** for mocking with strict behavior
- **Autofac** for dependency injection in tests

## Quick Reference

### Key Directories
```
docs/                           # This documentation
├── Home.md                     # Project overview and navigation
├── Getting-Started.md          # Installation and setup guide
├── Architecture.md             # High-level system design
├── Core-Components.md          # Detailed component docs
├── Development-Guide.md        # Code standards and practices
├── Testing-Strategy.md         # Test patterns and conventions
├── Configuration-Settings.md   # Settings and i18n
├── User-Guide.md              # Gameplay and features
├── API-Reference.md           # Technical reference
├── Troubleshooting.md         # Common issues and solutions
└── README.md                  # This file
```

### Project Structure
```
CN_HashiWpf/
├── Enums/                     # Shared enumerations
├── Generator/                 # Puzzle generation logic
├── Gui/                       # WPF application and UI
├── Logging/                   # NLog-based logging
├── PuzzleLoader/              # Puzzle file I/O
├── Solver/                    # Linear solver and rules
├── Tests/                     # Unit test projects
└── docs/                      # This documentation
```

### Essential Commands
```bash
# Build the solution
dotnet build

# Run tests
dotnet test

# Run the application (Windows only)
dotnet run --project Gui/Hashi.Gui/Hashi.Gui.csproj

# Clean and rebuild
dotnet clean && dotnet build
```

## Support and Contributing

### Getting Help
1. **Check documentation**: Most questions are answered here
2. **Search issues**: Look for existing GitHub issues
3. **Create issue**: Report bugs or request features
4. **Community**: Engage with other developers

### Contributing
1. **Read [Development Guide](Development-Guide.md)**: Understand code standards
2. **Follow [Testing Strategy](Testing-Strategy.md)**: Write comprehensive tests
3. **Update documentation**: Keep docs current with changes
4. **Submit pull requests**: Follow the established workflow

## License and Attribution

This documentation is part of the Hashiwokakero WPF project and follows the same license as the main project.

### External References
- **Hashiwokakero rules**: Based on [Wikipedia](https://en.wikipedia.org/wiki/Hashiwokakero)
- **MVVM pattern**: Microsoft documentation and community best practices
- **Clean Architecture**: Robert C. Martin's Clean Architecture principles
- **SOLID principles**: Object-oriented design principles

---

*Last updated: 2024 - This documentation is maintained alongside the codebase*