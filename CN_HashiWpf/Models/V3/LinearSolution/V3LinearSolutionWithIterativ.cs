using CNHashiWpf.Models.V3.LinearSolution;
using Google.OrTools.Sat;
using System.Diagnostics;


namespace CN_HashiWpf.Models.V3.LinearSolution
{
    public class V3LinearSolutionWithIterativ
    {
        private char[][] fieldChars;

        public CpSolverStatus Solve(int[][] mainField)
        {
            var nodes = new List<V3LinearIsland>();
            var bridges = new List<V3LinearBridge>();
            var delta = new List<V3LinearBridgePair>();

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
                    nodes.Add(new V3LinearIsland(mainField[row][col], row, col, numberOfNodes - 1));
                }
            }

            // Set node neighbors
            foreach (var node in nodes)
            {
                node.SetAllNeighbors(mainField, nodes);
            }

            // Set node lower and upper neighbors
            foreach (var node in nodes)
            {
                if (node.Up != null)
                {
                    node.LowerNeighbours.Add(node.Up);
                }
                if (node.Left != null)
                {
                    node.LowerNeighbours.Add(node.Left);
                }
                if (node.Right != null)
                {
                    node.UpNeighbours.Add(node.Right);
                }
                if (node.Down != null)
                {
                    node.UpNeighbours.Add(node.Down);
                }
            }

            // Alle möglichen Kanten werden in bridges hinzugefügt
            foreach (var node in nodes)
            {
                foreach (var nodelow in node.LowerNeighbours)
                {
                    var checker = false;
                    var edgeAdd = new V3LinearBridge(node, nodelow);
                    foreach (var edge in bridges)
                    {
                        if (edge.Node1.X == edgeAdd.Node1.X && edge.Node1.Y == edgeAdd.Node1.Y && edge.Node2.X == edgeAdd.Node2.X && edge.Node2.Y == edgeAdd.Node2.Y)
                        {
                            checker = true;
                            break;
                        }
                    }
                    if (!checker)
                    {
                        bridges.Add(edgeAdd);
                    }
                }
                foreach (var nodeup in node.UpNeighbours)
                {
                    var checker = false;
                    var edgeAdd = new V3LinearBridge(node, nodeup);
                    foreach (var edge in bridges)
                    {
                        if (edge.Node1.X == edgeAdd.Node1.X && edge.Node1.Y == edgeAdd.Node1.Y && edge.Node2.X == edgeAdd.Node2.X && edge.Node2.Y == edgeAdd.Node2.Y)
                        {
                            checker = true;
                            break;
                        }
                    }
                    if (!checker)
                    {
                        bridges.Add(edgeAdd);
                    }
                }
            }

            // Suchen von möglichen sich kreuzenden Kanten, diese Kantenpaare werden in delta gespeichert
            foreach (var node in nodes)
            {
                // Kontrolliert für den Knoten nach unten
                if (node.Down != null)
                {
                    var givenDown = node.DownBlocked(nodes);
                    var checkDown = false;
                    foreach (var nodedown in givenDown)
                    {
                        foreach (var edge in delta)
                        {
                            if ((edge.Edge1[0] == node.Number && edge.Edge1[1] == node.Down.Number
                                    && edge.Edge2[0] == nodedown.Number && edge.Edge2[1] == nodedown.Right.Number)
                                    || (edge.Edge2[0] == node.Number && edge.Edge2[1] == node.Down.Number
                                    && edge.Edge1[0] == nodedown.Number && edge.Edge1[1] == nodedown.Right.Number))
                            {
                                checkDown = true;
                            }
                        }
                        if (!checkDown)
                        {
                            delta.Add(new V3LinearBridgePair(node.Number, node.Down.Number, nodedown.Number, nodedown.Right.Number));
                        }
                    }
                }
                // Kontrolliert für den Knoten nach rechts
                if (node.Right != null)
                {
                    var givenRight = node.RightBlocked(nodes);
                    var checkRight = false;
                    foreach (var noderight in givenRight)
                    {
                        foreach (var edge in delta)
                        {
                            if ((edge.Edge1[0] == node.Number && edge.Edge1[1] == node.Right.Number
                                    && edge.Edge2[0] == noderight.Number && edge.Edge2[1] == noderight.Down.Number)
                                    || (edge.Edge2[0] == node.Number && edge.Edge2[1] == node.Right.Number
                                    && edge.Edge1[0] == noderight.Number && edge.Edge1[1] == noderight.Down.Number))
                            {
                                checkRight = true;
                            }
                        }
                        if (!checkRight)
                        {
                            delta.Add(new V3LinearBridgePair(node.Number, node.Right.Number, noderight.Number, noderight.Down.Number));
                        }
                    }
                }
            }

            var x = new IntVar[bridges.Count];
            var y = new IntVar[bridges.Count];

            // Jeder x Wert entspricht der Anzahl an Kanten zwischen zwei Knoten, damit ist der Wert zwischen einschließlich 0 und 2
            for (var i = 0; i < x.Length; i++)
            {
                x[i] = model.NewIntVar(0, 2, $"{bridges[i].Node1.Y}/{bridges[i].Node1.X},{bridges[i].Node2.Y}/{bridges[i].Node2.X}");
            }

            // Jeder y Wert gibt an, ob eine Kante zwischen zwei Knoten existiert
            for (var i = 0; i < y.Length; i++)
            {
                y[i] = model.NewIntVar(0, 1, $"{bridges[i].Node1.Y}/{bridges[i].Node1.X},{bridges[i].Node2.Y}/{bridges[i].Node2.X}");
            }

            foreach (var node in nodes)
            {
                // Speichert in posEdgesLow die Positionen von den Kanten in x,
                // welche zwischen dem aktuellen Knoten und dem Knoten aus lowerNeighbours sind
                var posEdgesLow = new List<int>();
                foreach (var nodeLow in node.LowerNeighbours)
                {
                    for (var i = 0; i < bridges.Count; i++)
                    {
                        if (bridges[i].Node1 == nodeLow && bridges[i].Node2 == node)
                        {
                            posEdgesLow.Add(i);
                        }
                    }
                }
                // Speichert in posEdgesUp die Positionen von den Kanten in x,
                // welche zwischen dem aktuellen Knoten und dem Knoten aus upNeighbours sind
                var posEdgesUp = new List<int>();
                foreach (var nodeUp in node.UpNeighbours)
                {
                    for (var i = 0; i < bridges.Count; i++)
                    {
                        if (bridges[i].Node1 == node && bridges[i].Node2 == nodeUp)
                        {
                            posEdgesUp.Add(i);
                        }
                    }
                }
                // Erstellt zwei Variablen um die x Werte summieren zu können
                var lowIntVars = LinearExpr.Sum(posEdgesLow.Select(low => x[low]).ToList());
                var upIntVars = LinearExpr.Sum(posEdgesUp.Select(up => x[up]).ToList());



                // lowIntVars und upIntVars zusammen addiert sollen dem Wert des aktuellen Knotens entsprechen
                model.Add(upIntVars + lowIntVars == node.Value);
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
            foreach (var edgePair in delta)
            {
                var first = 0;
                var second = 0;
                for (var i = 0; i < bridges.Count; i++)
                {
                    if (bridges[i].Node1.Number == edgePair.Edge1[0] && bridges[i].Node2.Number == edgePair.Edge1[1])
                    {
                        first = i;
                    }
                    if (bridges[i].Node1.Number == edgePair.Edge2[0] && bridges[i].Node2.Number == edgePair.Edge2[1])
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

            var allComp = FindComponents(bridges, nodes.Count, values);

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
                allComp = FindComponents(bridges, nodes.Count, values);
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
            foreach (var node in nodes)
            {
                fieldChars[node.Y][node.X] = (char)(node.Value + 48);
            }
            var iteration = 1;
            for (var i = 0; i < values.Length; i++)
            {
                if (values[i] == 0) continue;
                Debug.WriteLine($"{iteration++}. Kante: Von Knoten ({bridges[i].Node1.Y}/{bridges[i].Node1.X}) mit Wert {bridges[i].Node1.Value} nach Knoten ({bridges[i].Node2.Y}/{bridges[i].Node2.X}) mit Wert {bridges[i].Node2.Value} mit Kantenanzahl: {values[i]}.\n");
                PrintBridges(bridges[i], values[i]);
            }
            Debug.WriteLine(string.Join("\n", fieldChars.Select(row => "{" + string.Join(", ", row) + "}")));

            return status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bridges"></param>
        /// <param name="numNodes"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<Helper> FindComponents(List<V3LinearBridge> bridges, int numNodes, long[] value)
        {
            var components = new List<Helper>();
            var besucht = new bool[numNodes];

            // Erstelle den Graphen in einer Liste wobei gespeichert wird,
            // zu welchen Knoten eine Kante existiert für den jeweiligen Knoten
            var graph = new List<Helper>();
            for (var i = 0; i < numNodes; i++)
            {
                graph.Add(new Helper(new List<int>(), new List<int>()));
            }

            for (var i = 0; i < bridges.Count; i++)
            {
                if (value[i] == 0) continue;
                graph[bridges[i].Node1.Number].Node.Add(bridges[i].Node2.Number);
                graph[bridges[i].Node2.Number].Node.Add(bridges[i].Node1.Number);
                graph[bridges[i].Node1.Number].Bridges.Add(i);
            }

            for (var node = 0; node < numNodes; node++)
            {
                if (besucht[node]) continue;
                var component = new Helper(new List<int>(), new List<int>());
                Dfs(graph, node, besucht, component);
                components.Add(component);
            }
            return components;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="node"></param>
        /// <param name="besucht"></param>
        /// <param name="component"></param>
        private void Dfs(List<Helper> graph, int node, bool[] besucht, Helper component)
        {
            besucht[node] = true;
            component.Node.Add(node);
            for (var i = 0; i < graph[node].Bridges.Count; i++)
            {
                component.Bridges.Add(graph[node].Bridges[i]);
            }

            foreach (var neighbor in graph[node].Node.Where(neighbor => !besucht[neighbor]))
            {
                Dfs(graph, neighbor, besucht, component);
            }
        }

        /// <summary>
        /// Prints the bridges on the field.
        /// </summary>
        /// <param name="bridge"></param>
        /// <param name="edgeNumber"></param>
        public void PrintBridges(V3LinearBridge bridge, long edgeNumber)
        {
            if (bridge.Node1.X < bridge.Node2.X)
            {
                for (var i = bridge.Node1.X + 1; i < bridge.Node2.X; i++)
                {
                    fieldChars[bridge.Node1.Y][i] = edgeNumber switch
                    {
                        1 => '-',
                        2 => '=',
                        _ => fieldChars[bridge.Node1.Y][i]
                    };
                }
            }
            if (bridge.Node1.X > bridge.Node2.X)
            {
                for (var i = bridge.Node2.X + 1; i < bridge.Node1.X; i++)
                {
                    fieldChars[bridge.Node1.Y][i] = edgeNumber switch
                    {
                        1 => '-',
                        2 => '=',
                        _ => fieldChars[bridge.Node1.Y][i]
                    };
                }
            }
            if (bridge.Node1.Y < bridge.Node2.Y)
            {
                for (var i = bridge.Node1.Y + 1; i < bridge.Node2.Y; i++)
                {
                    fieldChars[i][bridge.Node1.X] = edgeNumber switch
                    {
                        1 => '|',
                        2 => '"',
                        _ => fieldChars[i][bridge.Node1.X]
                    };
                }
            }
            if (bridge.Node1.Y > bridge.Node2.Y)
            {
                for (var i = bridge.Node2.Y + 1; i < bridge.Node1.Y; i++)
                {
                    fieldChars[i][bridge.Node1.X] = edgeNumber switch
                    {
                        1 => '|',
                        2 => '"',
                        _ => fieldChars[i][bridge.Node1.X]
                    };
                }
            }
        }
    }

    public class Helper
    {
        public List<int> Node { get; }
        public List<int> Bridges { get; }

        public Helper(List<int> node, List<int> bridges)
        {
            Node = node;
            Bridges = bridges;
        }
    }
}
