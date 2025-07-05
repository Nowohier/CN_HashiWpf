# User Guide

This comprehensive guide will help you learn how to play Hashiwokakero and master the features of the WPF application.

## Getting Started with Hashiwokakero

### What is Hashiwokakero?

Hashiwokakero (橋をかけろ) is a Japanese logic puzzle that translates to "build bridges!" It's a captivating puzzle where you connect numbered islands with bridges following specific rules.

### Game Objective

Connect all numbered islands on the grid using bridges so that:
- Each island has exactly the number of bridges shown on it
- All islands are connected in a single network
- No bridges cross each other

## Game Rules

### Basic Rules

#### 1. Island Numbers
- Each island shows a number from 1 to 8
- This number indicates how many bridges must connect to that island
- The bridges can be distributed in any orthogonal direction (up, down, left, right)

#### 2. Bridge Placement
- **Orthogonal Only**: Bridges can only run horizontally or vertically
- **Maximum Two**: At most 2 bridges can connect any pair of islands
- **Direct Connection**: Bridges must run in straight lines between islands
- **No Crossing**: Bridges cannot cross each other or pass through islands

#### 3. Network Connection
- **Single Network**: All islands must be connected through bridges
- **No Isolation**: No island or group of islands can be isolated
- **Complete Connection**: Every island must be reachable from every other island

### Advanced Rules

#### Island Completion
- When an island has the correct number of bridges, it's considered "complete"
- Complete islands are often highlighted or marked differently
- You can use this visual feedback to verify your progress

#### Forced Moves
- Some moves are mandatory based on the island's number and available connections
- Corner islands with high numbers have limited connection options
- Edge islands have fewer possible directions than center islands

## Application Interface

### Main Window Layout

#### Menu Bar
- **File**: New Game, Open Game, Save Game, Exit
- **Game**: Hint, Solve, Pause, Statistics
- **View**: Zoom, Theme, Language
- **Help**: Rules, About

#### Game Grid
- **Interactive Area**: Click and drag to place bridges
- **Islands**: Numbered circles on the grid
- **Bridges**: Lines connecting islands
- **Grid Lines**: Help visualize the layout

#### Control Panel
- **Difficulty Selector**: Choose puzzle difficulty
- **Size Controls**: Set grid dimensions
- **Timer**: Shows elapsed time
- **Score**: Current game score

#### Status Bar
- **Game Status**: Current game state
- **Hint Counter**: Number of hints used
- **Progress**: Completion percentage

### Game Controls

#### Mouse Controls
- **Single Click**: Place one bridge between adjacent islands
- **Double Click**: Place two bridges (maximum allowed)
- **Right Click**: Remove bridges
- **Drag**: Visual feedback for bridge placement

#### Keyboard Shortcuts
- **Ctrl+N**: New Game
- **Ctrl+O**: Open Game
- **Ctrl+S**: Save Game
- **Ctrl+Z**: Undo last move
- **Ctrl+Y**: Redo move
- **H**: Get hint
- **Space**: Pause/Resume
- **F1**: Show help

## Playing the Game

### Starting a New Game

#### 1. Select Difficulty
Choose from multiple difficulty levels:
- **Easy (1-3)**: Fewer islands, simpler patterns
- **Medium (4-6)**: More islands, moderate complexity
- **Hard (7-9)**: Many islands, complex patterns
- **Expert (10)**: Maximum complexity, advanced techniques

#### 2. Choose Grid Size
Select grid dimensions:
- **Small (8x8)**: Quick games, good for beginners
- **Medium (15x15)**: Standard size, balanced gameplay
- **Large (25x25)**: Long games, complex puzzles
- **Custom**: Specify exact dimensions

#### 3. Generate Puzzle
Click "Generate" to create a new puzzle with your selected parameters.

### Basic Playing Strategy

#### Step 1: Identify Obvious Moves
Look for islands where the number of bridges is forced:
- **Corner islands with 3+**: Must have bridges in all available directions
- **Edge islands with 5+**: Must have bridges in all available directions
- **Center islands with 7+**: Must have bridges in all directions

#### Step 2: Complete Simple Islands
Start with islands that have limited options:
- Islands with value 1 next to only one other island
- Islands with value 2 that can only connect to two other islands

#### Step 3: Use Elimination
- If an island already has the correct number of bridges, no more can be added
- This eliminates possibilities for neighboring islands

#### Step 4: Prevent Isolation
- Ensure no island or group becomes isolated
- Watch for potential "dead ends" in your network

### Advanced Techniques

#### Forced Bridge Analysis
When an island has limited connection options, some bridges become mandatory:

```
Example:
    3
    |
2---4---1
    |
    2
```

The center island (4) must connect to all neighbors, forcing all adjacent bridges.

#### Connectivity Analysis
- Ensure all islands remain connected
- Avoid creating separate groups that cannot reconnect
- Plan bridge placement to maintain network integrity

#### Constraint Propagation
- When one island is completed, it affects neighboring islands
- Use this cascading effect to solve complex puzzles
- Look for chains of forced moves

## Application Features

### Hint System

#### Getting Hints
- **Manual Hints**: Press 'H' or click the Hint button
- **Auto Hints**: Enable automatic hint highlighting
- **Hint Types**: Different types of hints are available

#### Hint Categories
1. **Forced Bridges**: Bridges that must be placed
2. **Impossible Bridges**: Bridges that cannot be placed
3. **Completion Hints**: Islands that can be completed
4. **Isolation Warnings**: Potential connectivity issues

#### Using Hints Effectively
- Use hints sparingly to maintain challenge
- Try to understand the logic behind each hint
- Hints are educational tools, not solutions

### Solve Feature

#### Automatic Solving
- **Full Solve**: Automatically complete the entire puzzle
- **Step-by-Step**: Solve one move at a time
- **Partial Solve**: Solve until user intervention needed

#### Educational Use
- Watch the solver work to learn techniques
- Understand the logic behind each move
- Use for verification of your own solutions

### Timer and Scoring

#### Timer Features
- **Elapsed Time**: Shows time since game start
- **Pause/Resume**: Pause timer during breaks
- **Best Times**: Track personal records

#### Scoring System
- **Base Score**: Points for completing the puzzle
- **Time Bonus**: Bonus for faster completion
- **Hint Penalty**: Point deduction for using hints
- **Difficulty Multiplier**: Higher difficulty = more points

### Save and Load

#### Game State Persistence
- **Auto-Save**: Automatically saves progress
- **Manual Save**: Save game at any point
- **Multiple Saves**: Keep multiple saved games

#### File Format
- **JSON Format**: Human-readable save files
- **Cross-Platform**: Save files work across different systems
- **Backup**: Automatic backup of save files

## Accessibility Features

### Visual Accessibility

#### High Contrast Mode
- **Enhanced Visibility**: Better contrast for visual impairments
- **Color Blind Support**: Alternative color schemes
- **Customizable Colors**: Adjust colors to personal needs

#### Zoom Features
- **Grid Zoom**: Magnify the game grid
- **Interface Scaling**: Scale UI elements
- **Keyboard Navigation**: Full keyboard accessibility

### Assistive Technology

#### Screen Reader Support
- **Island Descriptions**: Audible island information
- **Bridge Status**: Spoken bridge placement feedback
- **Game Progress**: Audible progress updates

#### Motor Accessibility
- **Keyboard-Only Play**: Complete game control via keyboard
- **Customizable Controls**: Remap keys to personal preferences
- **Auto-Repeat**: Configurable key repeat rates

## Troubleshooting Common Issues

### Gameplay Problems

#### "I can't place a bridge"
- Check if the islands are aligned orthogonally
- Verify you haven't exceeded the 2-bridge limit
- Ensure the path is clear (no crossing bridges)

#### "I made a mistake"
- Use Undo (Ctrl+Z) to reverse recent moves
- Use the hint system to verify your logic
- Consider restarting if early in the game

#### "The puzzle seems impossible"
- Every generated puzzle has a unique solution
- Use the hint system to find the next move
- Try working backwards from completed islands

### Application Issues

#### Performance Problems
- **Slow Generation**: Reduce grid size or difficulty
- **Memory Issues**: Close other applications
- **Stuttering**: Disable animations in settings

#### Display Issues
- **Blurry Text**: Adjust Windows display scaling
- **Wrong Colors**: Check theme and color settings
- **Missing Elements**: Reset UI layout in settings

## Tips for Success

### Beginner Tips
1. **Start Small**: Begin with easy puzzles on small grids
2. **Use Hints**: Don't hesitate to use hints while learning
3. **Study Solutions**: Watch the solver to learn techniques
4. **Practice Regularly**: Consistent play improves skills

### Intermediate Tips
1. **Pattern Recognition**: Learn common island patterns
2. **Constraint Thinking**: Always consider all constraints
3. **Systematic Approach**: Develop a consistent solving method
4. **Error Analysis**: Learn from mistakes

### Advanced Tips
1. **Multiple Hypotheses**: Consider several possible solutions
2. **Proof by Contradiction**: Assume a move and check consequences
3. **Network Analysis**: Always consider global connectivity
4. **Optimization**: Find the most efficient solution path

## Puzzle Variants

### Difficulty Variations
- **Easy**: Fewer islands, obvious moves
- **Medium**: Balanced challenge, some advanced techniques
- **Hard**: Many islands, complex interactions
- **Expert**: Maximum complexity, all techniques required

### Size Variations
- **Mini (6x6)**: Quick puzzles for coffee breaks
- **Standard (15x15)**: Traditional puzzle size
- **Large (25x25)**: Extended gameplay sessions
- **Mega (30x30+)**: Maximum challenge

### Special Modes
- **Timed Mode**: Race against the clock
- **Zen Mode**: No timer, relaxed gameplay
- **Challenge Mode**: Pre-designed difficult puzzles
- **Daily Puzzle**: New puzzle every day

## Multiplayer and Social Features

### Score Sharing
- **Leaderboards**: Compare scores with others
- **Achievements**: Unlock achievements for milestones
- **Statistics**: Track your progress over time

### Puzzle Sharing
- **Export Puzzles**: Share interesting puzzles
- **Import Puzzles**: Play community-created puzzles
- **Puzzle Rating**: Rate and review puzzles

## Settings and Customization

### Game Settings
- **Auto-Save**: Automatically save progress
- **Hint Behavior**: Configure hint display
- **Timer Settings**: Show/hide timer
- **Sound Effects**: Enable/disable audio

### Display Settings
- **Theme**: Choose visual theme
- **Colors**: Customize game colors
- **Animations**: Enable/disable animations
- **Grid Style**: Choose grid appearance

### Language Settings
- **Interface Language**: Change UI language
- **Number Format**: Localized number display
- **Date Format**: Localized date display

## Keyboard Reference

### Game Controls
- **Arrow Keys**: Navigate grid
- **Space**: Place/remove bridge
- **Enter**: Confirm selection
- **Escape**: Cancel operation

### Menu Shortcuts
- **Ctrl+N**: New Game
- **Ctrl+O**: Open Game
- **Ctrl+S**: Save Game
- **Ctrl+Q**: Quit Application

### View Controls
- **+/-**: Zoom in/out
- **0**: Reset zoom
- **F11**: Toggle fullscreen
- **Ctrl+T**: Change theme

## Frequently Asked Questions

### Q: Can I create my own puzzles?
A: Yes, the application includes a puzzle editor mode where you can create and share custom puzzles.

### Q: Are there multiple solutions to puzzles?
A: No, every generated puzzle has exactly one unique solution.

### Q: Can I play offline?
A: Yes, the application works completely offline. Internet is only needed for sharing features.

### Q: How do I report bugs?
A: Use the Help > Report Bug menu option or visit the project's GitHub page.

### Q: Can I change the appearance?
A: Yes, the application supports multiple themes and customizable colors.

---

*For technical documentation, see [API Reference](API-Reference.md)*