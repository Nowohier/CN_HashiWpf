//using Google.OrTools.ModelBuilder;
//using Google.OrTools.Sat;

//namespace CNHashiWpf.Models.V3.LinearSolution
//{
//    public class V3LinearOriginal
//    {
//        private List<V3LinearNode> nodes = new List<V3LinearNode>();
//        private List<V3LinearEdge> edges = new List<V3LinearEdge>();
//        private List<EdgePairs> delta = new List<EdgePairs>();
//        public int NumberOfSol { get; private set; } = 0;
//        private int countOfSol = 0;
//        private char[][] fieldChars;
//        private bool solvable = false;

//        public V3LinearOriginal(int[][] mainfield)
//        {
//            // Model und Solver erstellen
//            Model model = new Model("my Model");
//            // Erstelle jeden Knoten
//            int numberOfNodes = 0;
//            for (int row = 0; row < mainfield.Length; row++)
//            {
//                for (int col = 0; col < mainfield[row].Length; col++)
//                {
//                    if (mainfield[row][col] != 0)
//                    {
//                        numberOfNodes++;
//                        nodes.Add(new V3LinearNode(mainfield[row][col], row, col, numberOfNodes - 1));
//                    }
//                }
//            }
//            // Setze für jeden Knoten die Nachbarn
//            foreach (var node in nodes)
//            {
//                node.SetAllNeighbours(mainfield, nodes);
//            }
//            // Setze für jeden Knoten die lowerNeighbours und upNeighbours
//            foreach (var node in nodes)
//            {
//                if (node.Up != null)
//                {
//                    node.LowerNeighbours.Add(node.Up);
//                }
//                if (node.Left != null)
//                {
//                    node.LowerNeighbours.Add(node.Left);
//                }
//                if (node.Right != null)
//                {
//                    node.UpNeighbours.Add(node.Right);
//                }
//                if (node.Down != null)
//                {
//                    node.UpNeighbours.Add(node.Down);
//                }
//            }

//            // Alle möglichen Kanten werden in edges hinzugefügt
//            foreach (var node in nodes)
//            {
//                foreach (var nodelow in node.LowerNeighbours)
//                {
//                    bool checker = false;
//                    var edgeAdd = new V3LinearEdge(node, nodelow);
//                    foreach (var edge in edges)
//                    {
//                        if (edge.Node1.X == edgeAdd.Node1.X && edge.Node1.Y == edgeAdd.Node1.Y && edge.Node2.X == edgeAdd.Node2.X && edge.Node2.Y == edgeAdd.Node2.Y)
//                        {
//                            checker = true;
//                            break;
//                        }
//                    }
//                    if (!checker)
//                    {
//                        edges.Add(edgeAdd);
//                    }
//                }
//                foreach (var nodeup in node.UpNeighbours)
//                {
//                    bool checker = false;
//                    var edgeAdd = new V3LinearEdge(node, nodeup);
//                    foreach (var edge in edges)
//                    {
//                        if (edge.Node1.X == edgeAdd.Node1.X && edge.Node1.Y == edgeAdd.Node1.Y && edge.Node2.X == edgeAdd.Node2.X && edge.Node2.Y == edgeAdd.Node2.Y)
//                        {
//                            checker = true;
//                            break;
//                        }
//                    }
//                    if (!checker)
//                    {
//                        edges.Add(edgeAdd);
//                    }
//                }
//            }

//            // Suchen von möglichen sich kreuzenden Kanten, diese Kantenpaare werden in delta gespeichert
//            foreach (var node in nodes)
//            {
//                // Kontrolliert für den Knoten nach unten
//                if (node.Down != null)
//                {
//                    var givenDown = node.DownBlocked(nodes);
//                    bool checkDown = false;
//                    foreach (var nodedown in givenDown)
//                    {
//                        foreach (var edge in delta)
//                        {
//                            if ((edge.Edge1[0] == node.Number && edge.Edge1[1] == node.Down.Number
//                                    && edge.Edge2[0] == nodedown.Number && edge.Edge2[1] == nodedown.Right.Number)
//                                    || (edge.Edge2[0] == node.Number && edge.Edge2[1] == node.Down.Number
//                                    && edge.Edge1[0] == nodedown.Number && edge.Edge1[1] == nodedown.Right.Number))
//                            {
//                                checkDown = true;
//                            }
//                        }
//                        if (!checkDown)
//                        {
//                            delta.Add(new EdgePairs(node.Number, node.Down.Number, nodedown.Number, nodedown.Right.Number));
//                        }
//                    }
//                }
//                // Kontrolliert für den Knoten nach rechts
//                if (node.Right != null)
//                {
//                    var givenRight = node.RightBlocked(nodes);
//                    bool checkRight = false;
//                    foreach (var noderight in givenRight)
//                    {
//                        foreach (var edge in delta)
//                        {
//                            if ((edge.Edge1[0] == node.Number && edge.Edge1[1] == node.Right.Number
//                                    && edge.Edge2[0] == noderight.Number && edge.Edge2[1] == noderight.Down.Number)
//                                    || (edge.Edge2[0] == node.Number && edge.Edge2[1] == node.Right.Number
//                                    && edge.Edge1[0] == noderight.Number && edge.Edge1[1] == noderight.Down.Number))
//                            {
//                                checkRight = true;
//                            }
//                        }
//                        if (!checkRight)
//                        {
//                            delta.Add(new EdgePairs(node.Number, node.Right.Number, noderight.Number, noderight.Down.Number));
//                        }
//                    }
//                }
//            }
//            // Erstellt die Variablen x
//            IntVar[] x = new IntVar[edges.Count];
//            // Erstellt die Variablen y
//            IntVar[] y = new IntVar[edges.Count];

//            // Jeder x Wert entspricht der Anzahl an Kanten zwischen zwei Knoten, damit ist der Wert zwischen einschließlich 0 und 2
//            for (int i = 0; i < x.Length; i++)
//            {
//                x[i] = model.IntVar(edges[i].Node1.Y + "/" + edges[i].Node1.X + "," + edges[i].Node2.Y + "/" + edges[i].Node2.X, 0, 2);
//            }

//            // Jeder y Wert gibt an, ob eine Kante zwischen zwei Knoten existiert
//            for (int i = 0; i < y.Length; i++)
//            {
//                y[i] = model.IntVar(edges[i].Node1.Y + "/" + edges[i].Node1.X + "," + edges[i].Node2.Y + "/" + edges[i].Node2.X, 0, 1);
//            }

//            foreach (var node in nodes)
//            {
//                // Speichert in posEdgesLow die Positionen von den Kanten in x,
//                // welche zwischen dem aktuellen Knoten und dem Knoten aus lowerNeighbours sind
//                List<int> posEdgesLow = new List<int>();
//                foreach (var nodelow in node.LowerNeighbours)
//                {
//                    for (int i = 0; i < edges.Count; i++)
//                    {
//                        if (edges[i].Node1 == nodelow && edges[i].Node2 == node)
//                        {
//                            posEdgesLow.Add(i);
//                        }
//                    }
//                }
//                // Speichert in posEdgesUp die Positionen von den Kanten in x,
//                // welche zwischen dem aktuellen Knoten und dem Knoten aus upNeighbours sind
//                List<int> posEdgesUp = new List<int>();
//                foreach (var nodeup in node.UpNeighbours)
//                {
//                    for (int i = 0; i < edges.Count; i++)
//                    {
//                        if (edges[i].Node1 == node && edges[i].Node2 == nodeup)
//                        {
//                            posEdgesUp.Add(i);
//                        }
//                    }
//                }
//                // Erstellt zwei Variablen um die x Werte summieren zu können
//                IntVar lowIntVars = model.IntVar(0);
//                IntVar upIntVars = model.IntVar(0);
//                // In lowIntVars wird die Summe der x Werte von den Kanten gespeichert, welche einen lowerNeighbours als Knoten haben
//                foreach (var low in posEdgesLow)
//                {
//                    lowIntVars = model.Sum("lowIntVars", lowIntVars, x[low]).IntVar();
//                }
//                // In upIntVars wird die Summe der x Werte von den Kanten gespeichert, welche einen upNeighbours als Knoten haben
//                foreach (var up in posEdgesUp)
//                {
//                    upIntVars = model.Sum("upIntVars", upIntVars, x[up]).IntVar();
//                }
//                // lowIntVars und upIntVars zusammen addiert sollen dem Wert des aktuellen Knotens entsprechen
//                model.Sum(new IntVar[] { lowIntVars, upIntVars }, "=", node.Value).Post();
//            }

//            // Der x Wert der Kante befindet sich zwischen dem y Wert der Kante und dem doppelten des y Werts
//            for (int i = 0; i < edges.Count; i++)
//            {
//                model.Arithm(y[i], "<=", x[i]).Post();
//                IntVar times = model.IntVar(0, 2);
//                model.Times(y[i], 2, times).Post();
//                model.Arithm(x[i], "<=", times).Post();
//            }

//            // Das Addieren von den y Werten der beiden Kanten in den jeweiligen Kantenpaaren muss kleiner gleich 1
//            // Dabei wird in first und second die Stelle der Kanten gespeichert an der diese sich im y Array befinden
//            foreach (var edgepair in delta)
//            {
//                int first = 0;
//                int second = 0;
//                for (int i = 0; i < edges.Count; i++)
//                {
//                    if (edges[i].Node1.Number == edgepair.Edge1[0] && edges[i].Node2.Number == edgepair.Edge1[1])
//                    {
//                        first = i;
//                    }
//                    if (edges[i].Node1.Number == edgepair.Edge2[0] && edges[i].Node2.Number == edgepair.Edge2[1])
//                    {
//                        second = i;
//                    }
//                }
//                model.Arithm(model.Sum("sumY", y[first], y[second]), "<=", 1).Post();
//            }

//            // Folgende auskommentieren um die 7. Bedingung benutzen zu können
//            /* // Addiert die y Werte zusammen und diese sollen >= Anzahl an Knoten - 1 sein
//            IntVar sumOfEdgesY = model.IntVar(0);

//            foreach (var intvar in y)
//            {
//                sumOfEdgesY = model.Sum("sumOfEdgesY", sumOfEdgesY, intvar).IntVar();
//            }

//            model.Arithm(sumOfEdgesY, ">=", nodes.Count - 1).Post(); */

//            // Erstellt die erste mögliche Lösung
//            int[] values;
//            Solution solution = model.GetSolver().FindSolution();
//            List<Helper> allComp = new List<Helper>();
//            values = new int[x.Length];
//            if (solution != null)
//            {
//                for (int i = 0; i < x.Length; i++)
//                {
//                    values[i] = solution.GetIntVal(x[i]);
//                }
//                // Suche alle Komponenten
//                allComp = FindComponents(edges, nodes.Count, values);
//            }
//            countOfSol++;
//            while (allComp.Count > 1 && solution != null)
//            {
//                model.GetSolver().Reset();
//                foreach (var comp in allComp)
//                {
//                    BoolVar[] constrains = new BoolVar[comp.Bridges.Count];
//                    for (int j = 0; j < comp.Bridges.Count; j++)
//                    {
//                        constrains[j] = x[comp.Bridges[j]].Ne(values[comp.Bridges[j]]).BoolVar();
//                    }
//                    model.Or(constrains).Post();
//                }
//                solution = model.GetSolver().FindSolution();
//                if (solution != null)
//                {
//                    countOfSol++;
//                    values = new int[x.Length];
//                    for (int i = 0; i < x.Length; i++)
//                    {
//                        values[i] = solution.GetIntVal(x[i]);
//                    }
//                    allComp.Clear();
//                    allComp = FindComponents(edges, nodes.Count, values);
//                }
//            }
//            if (allComp.Count == 1)
//            {
//                solvable = true;
//                NumberOfSol++;
//            }
//            fieldChars = new char[mainfield.Length][];
//            for (int i = 0; i < fieldChars.Length; i++)
//            {
//                fieldChars[i] = new char[mainfield[0].Length];
//                Array.Fill(fieldChars[i], '0');
//            }
//            foreach (var node in nodes)
//            {
//                fieldChars[node.Y][node.X] = (char)(node.Value + 48);
//            }
//            int iter = 1;
//            for (int i = 0; i < values.Length; i++)
//            {
//                if (values[i] != 0)
//                {
//                    Console.WriteLine($"{iter}. Kante: Von Knoten ({edges[i].Node1.Y}/{edges[i].Node1.X}) mit Wert {edges[i].Node1.Value} nach Knoten ({edges[i].Node2.Y}/{edges[i].Node2.X}) mit Wert {edges[i].Node2.Value} mit Kantenanzahl: {values[i]}.\n");
//                    PrintBridges(edges[i], values[i]);
//                    iter++;
//                }
//            }
//            Console.WriteLine(string.Join("\n", fieldChars.Select(row => "{" + string.Join(", ", row) + "}")));

//            Console.WriteLine($"\nEs wurden {countOfSol} Lösungen generiert bis zur Lösungsfindung.");
//            if (solvable)
//            {
//                Solution solutions = model.GetSolver().FindSolution();
//                if (solutions != null)
//                {
//                    NumberOfSol = 2;
//                }
//            }
//        }

//        // Findet die Zusammenhangskomponenten
//        public List<Helper> FindComponents(List<V3LinearEdge> edges, int numNodes, int[] value)
//        {
//            List<Helper> components = new List<Helper>();
//            bool[] besucht = new bool[numNodes];

//            // Erstelle den Graphen in einer Liste wobei gespeichert wird,
//            // zu welchen Knoten eine Kante existiert für den jeweiligen Knoten
//            List<Helper> graph = new List<Helper>();
//            for (int i = 0; i < numNodes; i++)
//            {
//                graph.Add(new Helper(new List<int>(), new List<int>()));
//            }

//            for (int i = 0; i < edges.Count; i++)
//            {
//                if (value[i] != 0)
//                {
//                    graph[edges[i].Node1.Number].Node.Add(edges[i].Node2.Number);
//                    graph[edges[i].Node2.Number].Node.Add(edges[i].Node1.Number);
//                    graph[edges[i].Node1.Number].Bridges.Add(i);
//                }
//            }

//            for (int node = 0; node < numNodes; node++)
//            {
//                if (!besucht[node])
//                {
//                    Helper component = new Helper(new List<int>(), new List<int>());
//                    Dfs(graph, node, besucht, component);
//                    components.Add(component);
//                }
//            }
//            return components;
//        }

//        // Führt die Tiefensuche aus
//        private void Dfs(List<Helper> graph, int node, bool[] besucht, Helper component)
//        {
//            besucht[node] = true;
//            component.Node.Add(node);
//            for (int i = 0; i < graph[node].Bridges.Count; i++)
//            {
//                component.Bridges.Add(graph[node].Bridges[i]);
//            }

//            foreach (var neighbor in graph[node].Node)
//            {
//                if (!besucht[neighbor])
//                {
//                    Dfs(graph, neighbor, besucht, component);
//                }
//            }
//        }

//        // Erstellt die Kanten im 2D-Array für die Ausgabe
//        public void PrintBridges(V3LinearEdge edge, int edgeNumber)
//        {
//            if (edge.Node1.X < edge.Node2.X)
//            {
//                for (int i = edge.Node1.X + 1; i < edge.Node2.X; i++)
//                {
//                    if (edgeNumber == 1)
//                    {
//                        fieldChars[edge.Node1.Y][i] = '-';
//                    }
//                    if (edgeNumber == 2)
//                    {
//                        fieldChars[edge.Node1.Y][i] = '=';
//                    }
//                }
//            }
//            if (edge.Node1.X > edge.Node2.X)
//            {
//                for (int i = edge.Node2.X + 1; i < edge.Node1.X; i++)
//                {
//                    if (edgeNumber == 1)
//                    {
//                        fieldChars[edge.Node1.Y][i] = '-';
//                    }
//                    if (edgeNumber == 2)
//                    {
//                        fieldChars[edge.Node1.Y][i] = '=';
//                    }
//                }
//            }
//            if (edge.Node1.Y < edge.Node2.Y)
//            {
//                for (int i = edge.Node1.Y + 1; i < edge.Node2.Y; i++)
//                {
//                    if (edgeNumber == 1)
//                    {
//                        fieldChars[i][edge.Node1.X] = '|';
//                    }
//                    if (edgeNumber == 2)
//                    {
//                        fieldChars[i][edge.Node1.X] = '"';
//                    }
//                }
//            }
//            if (edge.Node1.Y > edge.Node2.Y)
//            {
//                for (int i = edge.Node2.Y + 1; i < edge.Node1.Y; i++)
//                {
//                    if (edgeNumber == 1)
//                    {
//                        fieldChars[i][edge.Node1.X] = '|';
//                    }
//                    if (edgeNumber == 2)
//                    {
//                        fieldChars[i][edge.Node1.X] = '"';
//                    }
//                }
//            }
//        }
//    }

//    public class Helper
//    {
//        public List<int> Node { get; }
//        public List<int> Bridges { get; }

//        public Helper(List<int> node, List<int> edges)
//        {
//            Node = node;
//            Bridges = edges;
//        }
//    }

//    public class EdgePairs
//    {
//        public int[] Edge1 { get; }
//        public int[] Edge2 { get; }

//        public EdgePairs(int node1, int node2, int node3, int node4)
//        {
//            Edge1 = new[] { node1, node2 };
//            Edge2 = new[] { node3, node4 };
//        }
//    }
//}