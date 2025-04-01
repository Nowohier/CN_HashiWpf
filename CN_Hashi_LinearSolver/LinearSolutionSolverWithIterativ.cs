using CNHashiLinearSolver.Models;
using Google.OrTools.Sat;
using System.Diagnostics;

namespace CNHashiLinearSolver
{
    /// <summary>
    /// Represents a linear solution solver with iterative approach.
    /// </summary>
    public class LinearSolutionSolverWithIterativ
    {
        private char[][] fieldChars;

        public CpSolverStatus Solve(int[][] mainField)
        {
            var islands = new List<Island>();
            var bridges = new List<Bridge>();
            var delta = new List<BridgePair>();

            var solver = new CpSolver();
            var model = new CpModel();

            // Create all nodes
            var numberOfNodes = 0;
            for (var row = 0; row < mainField.Length; row++)
            {
                for (var col = 0; col < mainField[row].Length; col++)
                {
                    if (mainField[row][col] == 0) continue;
                    numberOfNodes++;
                    islands.Add(new Island(mainField[row][col], row, col, numberOfNodes - 1));
                }
            }

            // Set islands neighbors
            foreach (var island in islands)
            {
                island.SetAllNeighbors(mainField, islands);
            }

            // Set islands lower and upper neighbors
            foreach (var island in islands)
            {
                if (island.Up != null)
                {
                    island.LowerNeighbors.Add(island.Up);
                }
                if (island.Left != null)
                {
                    island.LowerNeighbors.Add(island.Left);
                }
                if (island.Right != null)
                {
                    island.UpNeighbors.Add(island.Right);
                }
                if (island.Down != null)
                {
                    island.UpNeighbors.Add(island.Down);
                }
            }

            // Alle möglichen Kanten werden in bridges hinzugefügt
            foreach (var island in islands)
            {
                foreach (var islandLow in island.LowerNeighbors)
                {
                    var checker = false;
                    var bridgeAdd = new Bridge(island, islandLow);
                    foreach (var bridge in bridges)
                    {
                        if (bridge.Island1.X == bridgeAdd.Island1.X && bridge.Island1.Y == bridgeAdd.Island1.Y && bridge.Island2.X == bridgeAdd.Island2.X && bridge.Island2.Y == bridgeAdd.Island2.Y)
                        {
                            checker = true;
                            break;
                        }
                    }
                    if (!checker)
                    {
                        bridges.Add(bridgeAdd);
                    }
                }
                foreach (var islandUp in island.UpNeighbors)
                {
                    var checker = false;
                    var bridgeAdd = new Bridge(island, islandUp);
                    foreach (var bridge in bridges)
                    {
                        if (bridge.Island1.X == bridgeAdd.Island1.X && bridge.Island1.Y == bridgeAdd.Island1.Y && bridge.Island2.X == bridgeAdd.Island2.X && bridge.Island2.Y == bridgeAdd.Island2.Y)
                        {
                            checker = true;
                            break;
                        }
                    }
                    if (!checker)
                    {
                        bridges.Add(bridgeAdd);
                    }
                }
            }

            // Suchen von möglichen sich kreuzenden Kanten, diese Kantenpaare werden in delta gespeichert
            foreach (var island in islands)
            {
                // Kontrolliert für den Knoten nach unten
                if (island.Down != null)
                {
                    var potentialBlockingIslandsDown = island.DownBlocked(islands);
                    var checkDown = false;
                    foreach (var islandDown in potentialBlockingIslandsDown)
                    {
                        foreach (var bridgePair in delta)
                        {
                            if ((bridgePair.Bridge1[0] == island.Number && bridgePair.Bridge1[1] == island.Down.Number
                                    && bridgePair.Bridge2[0] == islandDown.Number && bridgePair.Bridge2[1] == islandDown.Right.Number)
                                    || (bridgePair.Bridge2[0] == island.Number && bridgePair.Bridge2[1] == island.Down.Number
                                    && bridgePair.Bridge1[0] == islandDown.Number && bridgePair.Bridge1[1] == islandDown.Right.Number))
                            {
                                checkDown = true;
                            }
                        }
                        if (!checkDown)
                        {
                            delta.Add(new BridgePair(island.Number, island.Down.Number, islandDown.Number, islandDown.Right.Number));
                        }
                    }
                }
                // Kontrolliert für den Knoten nach rechts
                if (island.Right != null)
                {
                    var potentialBlockingIslandsRight = island.RightBlocked(islands);
                    var checkRight = false;
                    foreach (var islandRight in potentialBlockingIslandsRight)
                    {
                        foreach (var bridgePair in delta)
                        {
                            if ((bridgePair.Bridge1[0] == island.Number && bridgePair.Bridge1[1] == island.Right.Number
                                    && bridgePair.Bridge2[0] == islandRight.Number && bridgePair.Bridge2[1] == islandRight.Down.Number)
                                    || (bridgePair.Bridge2[0] == island.Number && bridgePair.Bridge2[1] == island.Right.Number
                                    && bridgePair.Bridge1[0] == islandRight.Number && bridgePair.Bridge1[1] == islandRight.Down.Number))
                            {
                                checkRight = true;
                            }
                        }
                        if (!checkRight)
                        {
                            delta.Add(new BridgePair(island.Number, island.Right.Number, islandRight.Number, islandRight.Down.Number));
                        }
                    }
                }
            }

            var x = new IntVar[bridges.Count];
            var y = new IntVar[bridges.Count];

            // Jeder x Wert entspricht der Anzahl an Kanten zwischen zwei Knoten, damit ist der Wert zwischen einschließlich 0 und 2
            for (var i = 0; i < x.Length; i++)
            {
                x[i] = model.NewIntVar(0, 2, $"{bridges[i].Island1.Y}/{bridges[i].Island1.X},{bridges[i].Island2.Y}/{bridges[i].Island2.X}");
            }

            // Jeder y Wert gibt an, ob eine Kante zwischen zwei Knoten existiert
            for (var i = 0; i < y.Length; i++)
            {
                y[i] = model.NewIntVar(0, 1, $"{bridges[i].Island1.Y}/{bridges[i].Island1.X},{bridges[i].Island2.Y}/{bridges[i].Island2.X}");
            }

            foreach (var island in islands)
            {
                // Speichert in posEdgesLow die Positionen von den Kanten in x,
                // welche zwischen dem aktuellen Knoten und dem Knoten aus lowerNeighbours sind
                var positionBridgesLow = new List<int>();
                foreach (var islandLow in island.LowerNeighbors)
                {
                    for (var i = 0; i < bridges.Count; i++)
                    {
                        if (bridges[i].Island1 == islandLow && bridges[i].Island2 == island)
                        {
                            positionBridgesLow.Add(i);
                        }
                    }
                }
                // Speichert in posEdgesUp die Positionen von den Kanten in x,
                // welche zwischen dem aktuellen Knoten und dem Knoten aus upNeighbours sind
                var positionBridgesUp = new List<int>();
                foreach (var islandUp in island.UpNeighbors)
                {
                    for (var i = 0; i < bridges.Count; i++)
                    {
                        if (bridges[i].Island1 == island && bridges[i].Island2 == islandUp)
                        {
                            positionBridgesUp.Add(i);
                        }
                    }
                }
                // Erstellt zwei Variablen um die x Werte summieren zu können
                var lowIntVars = LinearExpr.Sum(positionBridgesLow.Select(low => x[low]).ToList());
                var upIntVars = LinearExpr.Sum(positionBridgesUp.Select(up => x[up]).ToList());



                // lowIntVars und upIntVars zusammen addiert sollen dem Wert des aktuellen Knotens entsprechen
                model.Add(upIntVars + lowIntVars == island.Value);
            }

            // Der x Wert der Kante befindet sich zwischen dem y Wert der Kante und dem doppelten des y Werts
            for (var i = 0; i < bridges.Count; i++)
            {
                model.Add(y[i] <= x[i]);
                var times = model.NewIntVar(0, 2, string.Empty);
                model.Add(y[i] * 2 == times);
                model.Add(x[i] <= times);
            }

            // Das Addieren von den y Werten der beiden Kanten in den jeweiligen Kantenpaaren muss kleiner gleich 1
            // Dabei wird in first und second die Stelle der Kanten gespeichert an der diese sich im y Array befinden
            foreach (var bridgePair in delta)
            {
                var first = 0;
                var second = 0;
                for (var i = 0; i < bridges.Count; i++)
                {
                    if (bridges[i].Island1.Number == bridgePair.Bridge1[0] && bridges[i].Island2.Number == bridgePair.Bridge1[1])
                    {
                        first = i;
                    }
                    if (bridges[i].Island1.Number == bridgePair.Bridge2[0] && bridges[i].Island2.Number == bridgePair.Bridge2[1])
                    {
                        second = i;
                    }
                }
                model.Add(y[first] + y[second] <= 1);
            }

            // Folgende auskommentieren um die 7. Bedingung benutzen zu können
            /* // Addiert die y Werte zusammen und diese sollen >= Anzahl an Knoten - 1 sein
            IntVar sumOfEdgesY = model.IntVar(0);

            foreach (var intvar in y)
            {
                sumOfEdgesY = model.Sum("sumOfEdgesY", sumOfEdgesY, intvar).IntVar();
            }

            model.Arithm(sumOfEdgesY, ">=", nodes.Count - 1).Post(); */

            var status = solver.Solve(model);
            if (!status.Equals(CpSolverStatus.Optimal))
            {
                return status;
            }

            var solution = solver.Response.Solution;
            var values = new long[x.Length];

            for (var i = 0; i < x.Length; i++)
            {
                values[i] = solution[i];
            }

            var allComp = FindComponents(bridges, islands.Count, values);

            while (allComp.Count > 1)
            {

                // ToDo
                foreach (var comp in allComp)
                {
                    var constraints = new ILiteral[comp.Bridges.Count];
                    for (var j = 0; j < comp.Bridges.Count; j++)
                    {
                        constraints[j] = model.NewBoolVar($"constraint_{comp.Bridges[j]}");
                        model.Add(x[comp.Bridges[j]] != values[comp.Bridges[j]]).OnlyEnforceIf(constraints[j]);
                    }
                    model.AddBoolOr(constraints);
                }

                status = solver.Solve(model);
                if (!status.Equals(CpSolverStatus.Optimal))
                {
                    return status;
                }

                solution = solver.Response.Solution;
                values = new long[x.Length];

                for (int i = 0; i < x.Length; i++)
                {
                    values[i] = solution[i];
                }
                allComp.Clear();
                allComp = FindComponents(bridges, islands.Count, values);
                Debug.WriteLine(allComp.Count);
            }

            if (allComp.Count == 1)
            {
                //solvable = true;
                //NumberOfSol++;
            }


            fieldChars = new char[mainField.Length][];
            for (var i = 0; i < fieldChars.Length; i++)
            {
                fieldChars[i] = new char[mainField[0].Length];
                Array.Fill(fieldChars[i], '0');
            }
            foreach (var node in islands)
            {
                fieldChars[node.Y][node.X] = (char)(node.Value + 48);
            }
            var iteration = 1;
            for (var i = 0; i < values.Length; i++)
            {
                if (values[i] == 0) continue;
                Debug.WriteLine($"{iteration++}. Kante: Von Knoten ({bridges[i].Island1.Y}/{bridges[i].Island1.X}) mit Wert {bridges[i].Island1.Value} nach Knoten ({bridges[i].Island2.Y}/{bridges[i].Island2.X}) mit Wert {bridges[i].Island2.Value} mit Kantenanzahl: {values[i]}.\n");
                PrintBridges(bridges[i], values[i]);
            }
            Debug.WriteLine(string.Join("\n", fieldChars.Select(row => "{" + string.Join(", ", row) + "}")));

            return status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bridges"></param>
        /// <param name="amountIslands"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<Helper> FindComponents(List<Bridge> bridges, int amountIslands, long[] value)
        {
            var components = new List<Helper>();
            var checkedIslands = new bool[amountIslands];

            // Erstelle den Graphen in einer Liste wobei gespeichert wird,
            // zu welcher Insel eine Bridge existiert für die jeweilige Insel
            var graph = new List<Helper>();
            for (var i = 0; i < amountIslands; i++)
            {
                graph.Add(new Helper(new List<int>(), new List<int>()));
            }

            for (var i = 0; i < bridges.Count; i++)
            {
                if (value[i] == 0) continue;
                graph[bridges[i].Island1.Number].Islands.Add(bridges[i].Island2.Number);
                graph[bridges[i].Island2.Number].Islands.Add(bridges[i].Island1.Number);
                graph[bridges[i].Island1.Number].Bridges.Add(i);
            }

            for (var islandIndex = 0; islandIndex < amountIslands; islandIndex++)
            {
                if (checkedIslands[islandIndex]) continue;
                var component = new Helper(new List<int>(), new List<int>());
                FindConnectedComponents(graph, islandIndex, checkedIslands, component);
                components.Add(component);
            }
            return components;
        }

        /// <summary>
        /// Depth-first search algorithm to find all connected components.
        /// </summary>
        /// <param name="graph">A list of helper elements.</param>
        /// <param name="islandIndex">The current island index.</param>
        /// <param name="checkedIslands">A list containing values if islands have been checked already or not.</param>
        /// <param name="component">The actual helper component.</param>
        private void FindConnectedComponents(IReadOnlyList<Helper> graph, int islandIndex, IList<bool> checkedIslands, Helper component)
        {
            checkedIslands[islandIndex] = true;
            component.Islands.Add(islandIndex);
            for (var i = 0; i < graph[islandIndex].Bridges.Count; i++)
            {
                component.Bridges.Add(graph[islandIndex].Bridges[i]);
            }

            foreach (var neighbor in graph[islandIndex].Islands.Where(neighbor => !checkedIslands[neighbor]))
            {
                FindConnectedComponents(graph, neighbor, checkedIslands, component);
            }
        }

        /// <summary>
        /// Prints the bridges on the field.
        /// </summary>
        /// <param name="bridge"></param>
        /// <param name="edgeNumber"></param>
        public void PrintBridges(Bridge bridge, long edgeNumber)
        {
            if (bridge.Island1.X < bridge.Island2.X)
            {
                for (var i = bridge.Island1.X + 1; i < bridge.Island2.X; i++)
                {
                    fieldChars[bridge.Island1.Y][i] = edgeNumber switch
                    {
                        1 => '-',
                        2 => '=',
                        _ => fieldChars[bridge.Island1.Y][i]
                    };
                }
            }
            if (bridge.Island1.X > bridge.Island2.X)
            {
                for (var i = bridge.Island2.X + 1; i < bridge.Island1.X; i++)
                {
                    fieldChars[bridge.Island1.Y][i] = edgeNumber switch
                    {
                        1 => '-',
                        2 => '=',
                        _ => fieldChars[bridge.Island1.Y][i]
                    };
                }
            }
            if (bridge.Island1.Y < bridge.Island2.Y)
            {
                for (var i = bridge.Island1.Y + 1; i < bridge.Island2.Y; i++)
                {
                    fieldChars[i][bridge.Island1.X] = edgeNumber switch
                    {
                        1 => '|',
                        2 => '"',
                        _ => fieldChars[i][bridge.Island1.X]
                    };
                }
            }
            if (bridge.Island1.Y > bridge.Island2.Y)
            {
                for (var i = bridge.Island2.Y + 1; i < bridge.Island1.Y; i++)
                {
                    fieldChars[i][bridge.Island1.X] = edgeNumber switch
                    {
                        1 => '|',
                        2 => '"',
                        _ => fieldChars[i][bridge.Island1.X]
                    };
                }
            }
        }
    }

    public class Helper
    {
        public List<int> Islands { get; }
        public List<int> Bridges { get; }

        public Helper(List<int> islands, List<int> bridges)
        {
            Islands = islands;
            Bridges = bridges;
        }
    }
}
