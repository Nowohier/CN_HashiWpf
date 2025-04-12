
# Hashiwokakero ("build bridges!")

Hashiwokakero is a type of logic puzzle. It is played on a rectangular grid with no standard size, although the grid itself is not usually drawn. Some cells start out with (usually encircled) numbers from 1 to 8 inclusive; these are the "islands". The rest of the cells are empty.

The goal is to connect all of the islands by drawing a series of bridges between the islands. The bridges must follow certain criteria:

- They must begin and end at distinct islands, travelling a straight line in between.
- They must not cross any other bridges or islands.
- They may only run orthogonally (i.e. they may not run diagonally).
- At most two bridges connect a pair of islands.
- The number of bridges connected to each island must match the number on that island.
- The bridges must connect the islands into a single connected group.



Info from Wikipedia (https://en.wikipedia.org/wiki/Hashiwokakero)
## Solution Methods
Solving a Hashiwokakero puzzle is a matter of procedural force: having determined where a bridge must be placed, placing it there can eliminate other possible places for bridges, forcing the placement of another bridge, and so on.

An island showing '3' in a corner, '5' along the outside edge, or '7' anywhere must have at least one bridge radiating from it in each valid direction, for if one direction did not have a bridge, even if all other directions sported two bridges, not enough will have been placed. A '4' in a corner, '6' along the border, or '8' anywhere must have two bridges in each direction. This can be generalized as added bridges obstruct routes: a '3' that can only be travelled from vertically must have at least one bridge each for up and down, for example.

It is common practice to cross off or fill in islands whose bridge quota has been reached. In addition to reducing mistakes, this can also help locate potential "short circuits": keeping in mind that all islands must be connected by one network of bridges, a bridge that would create a closed network that no further bridges could be added to can only be permitted if it immediately yields the solution to the complete puzzle. The simplest example of this is two islands showing '1' aligned with each other; unless they are the only two islands in the puzzle, they cannot be connected by a bridge, as that would complete a network that cannot be added to, and would therefore force those two islands to be unreachable by any others.

Any bridge that would completely isolate a group of islands from another group would not be permitted, as one would then have two groups of islands that could not connect.

Info from Wikipedia (https://en.wikipedia.org/wiki/Hashiwokakero)
## Screenshots

![Screenshot of the UI](/Gui/Hashi.Gui/Resources/hashi_screenshot.png?raw=true "Optional Title")

