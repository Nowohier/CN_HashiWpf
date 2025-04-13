// ReSharper disable CommentTypo

namespace Hashi.Gui.Test.Models.V3;

[TestFixture]
public class V3Test
{
    //[Test]
    //public void CheckMovesCalculate()
    //{
    //    var hashi1 = new Tactic();
    //    hashi1.MainField = new int[][]
    //    {
    //        new int[] { 2, 0, 2 },
    //        new int[] { 0, 0, 0 },
    //        new int[] { 2, 0, 0 }
    //    };

    //    for (int row = 0; row < hashi1.MainField.Length; row++)
    //    {
    //        for (int col = 0; col < hashi1.MainField[row].Length; col++)
    //        {
    //            if (hashi1.MainField[row][col] != 0)
    //            {
    //                hashi1.Nodes.Add(new V3Island(hashi1.MainField[row][col], row, col));
    //            }
    //        }
    //    }
    //    foreach (var node in hashi1.Nodes)
    //    {
    //        node.SetAllNeighbors(hashi1.MainField, hashi1.Nodes);
    //    }
    //    hashi1.MovesCalculate(hashi1.Nodes[0], hashi1.Nodes, hashi1.AllMoveLists);
    //    Assert.IsTrue(hashi1.AllMoveLists[0].PossibleMoves.Contains("D2"));
    //    Assert.IsTrue(hashi1.AllMoveLists[0].PossibleMoves.Contains("D1R1"));
    //    Assert.IsTrue(hashi1.AllMoveLists[0].PossibleMoves.Contains("R2"));
    //    Assert.AreEqual(3, hashi1.AllMoveLists[0].PossibleMoves.Count);
    //    hashi1.MovesCalculate(hashi1.Nodes[1], hashi1.Nodes, hashi1.AllMoveLists);
    //    Assert.IsTrue(hashi1.AllMoveLists[1].PossibleMoves.Contains("L2"));
    //    Assert.AreEqual(1, hashi1.AllMoveLists[1].PossibleMoves.Count);
    //    hashi1.MovesCalculate(hashi1.Nodes[2], hashi1.Nodes, hashi1.AllMoveLists);
    //    Assert.IsTrue(hashi1.AllMoveLists[2].PossibleMoves.Contains("U2"));
    //    Assert.AreEqual(1, hashi1.AllMoveLists[2].PossibleMoves.Count);
    //}

    //[Test]
    //public void CheckTactic1()
    //{
    //    var hashi1 = new Tactic(new int[][]
    //    {
    //        new int[] { 4, 0, 5, 0, 4 },
    //        new int[] { 0, 0, 0, 0, 0 },
    //        new int[] { 3, 0, 0, 0, 3 },
    //        new int[] { 0, 0, 0, 0, 0 },
    //        new int[] { 0, 0, 1, 0, 0 }
    //    });

    //    var hashi2 = new Tactic(new int[][]
    //    {
    //        new int[] { 2, 0, 2, 0, 0 },
    //        new int[] { 0, 0, 0, 0, 0 },
    //        new int[] { 4, 0, 0, 0, 1 },
    //        new int[] { 0, 0, 0, 0, 0 },
    //        new int[] { 1, 0, 1, 0, 0 }
    //    });
    //    var tactic1Edges = hashi1.Bridges;
    //    foreach (var node in hashi1.Nodes)
    //    {
    //        if (node.Y == 2 && node.X == 0)
    //        {
    //            Assert.IsTrue(node.RightBlocked(tactic1Edges, hashi1.MainField));
    //        }
    //        if (node.Y == 2 && node.X == 4)
    //        {
    //            Assert.IsTrue(node.LeftBlocked(tactic1Edges, hashi1.MainField));
    //        }
    //    }
    //    var tactics2Edges = hashi2.Bridges;
    //    foreach (var node in hashi2.Nodes)
    //    {
    //        if (node.Y == 0 && node.X == 2)
    //        {
    //            Assert.IsTrue(node.DownBlocked(tactics2Edges, hashi2.MainField));
    //        }
    //        if (node.Y == 4 && node.X == 2)
    //        {
    //            Assert.IsTrue(node.UpBlocked(tactics2Edges, hashi2.MainField));
    //        }
    //    }
    //}

    //[Test]
    //public void CheckTactic2()
    //{
    //    var hashi1 = new Tactic();
    //    hashi1.MainField = new int[][]
    //    {
    //        new int[] { 1, 0, 1 },
    //        new int[] { 0, 0, 0 },
    //        new int[] { 1, 0, 0 }
    //    };
    //    for (int row = 0; row < hashi1.MainField.Length; row++)
    //    {
    //        for (int col = 0; col < hashi1.MainField[row].Length; col++)
    //        {
    //            if (hashi1.MainField[row][col] != 0)
    //            {
    //                hashi1.Nodes.Add(new V3Island(hashi1.MainField[row][col], row, col));
    //            }
    //        }
    //    }
    //    foreach (var node in hashi1.Nodes)
    //    {
    //        node.SetAllNeighbors(hashi1.MainField, hashi1.Nodes);
    //    }
    //    foreach (var node in hashi1.Nodes)
    //    {
    //        hashi1.MovesCalculate(node, hashi1.Nodes, hashi1.AllMoveLists);
    //    }
    //    hashi1.AddEdges(hashi1.AllMoveLists[1].PossibleMoves[0], hashi1.AllMoveLists[1].Node, hashi1.Bridges);
    //    hashi1.Tactic2(hashi1.AllMoveLists[2]);
    //    Assert.AreEqual(0, hashi1.AllMoveLists[2].PossibleMoves.Count);
    //}

    //[Test]
    //public void CheckTactic3()
    //{
    //    var hashi1 = new Tactic();
    //    hashi1.MainField = new int[][]
    //    {
    //        new int[] { 1, 0, 1 },
    //        new int[] { 0, 0, 0 },
    //        new int[] { 1, 0, 0 }
    //    };
    //    for (int row = 0; row < hashi1.MainField.Length; row++)
    //    {
    //        for (int col = 0; col < hashi1.MainField[row].Length; col++)
    //        {
    //            if (hashi1.MainField[row][col] != 0)
    //            {
    //                hashi1.Nodes.Add(new V3Island(hashi1.MainField[row][col], row, col));
    //            }
    //        }
    //    }
    //    foreach (var node in hashi1.Nodes)
    //    {
    //        node.SetAllNeighbors(hashi1.MainField, hashi1.Nodes);
    //    }
    //    foreach (var node in hashi1.Nodes)
    //    {
    //        hashi1.MovesCalculate(node, hashi1.Nodes, hashi1.AllMoveLists);
    //    }
    //    Assert.AreEqual(2, hashi1.AllMoveLists[0].PossibleMoves.Count);
    //    hashi1.AddEdges(hashi1.AllMoveLists[0].PossibleMoves[0], hashi1.AllMoveLists[0].Node, hashi1.Bridges);
    //    hashi1.Tactic3(hashi1.AllMoveLists[0]);
    //    Assert.AreEqual(1, hashi1.AllMoveLists[0].PossibleMoves.Count);
    //}

    //[Test]
    //public void CheckDoMove()
    //{
    //    var hashi1 = new Tactic();
    //    var node1 = new V3Island();
    //    var possibleMoves = new List<string> { "U1", "D1", "L1", "R1" };
    //    var move = new NodeMoves(node1, possibleMoves);
    //    var intersectMove = hashi1.DoMove(move, node1);
    //    Assert.AreEqual("", intersectMove);

    //    var hashi2 = new Tactic();
    //    var node2 = new V3Island();
    //    var possibleMoves1 = new List<string> { "U2", "D2", "L2", "R2" };
    //    var move1 = new NodeMoves(node2, possibleMoves1);
    //    var intersectMove1 = hashi2.DoMove(move1, node2);
    //    Assert.AreEqual("", intersectMove1);

    //    var hashi3 = new Tactic();
    //    var node3 = new V3Island();
    //    var possibleMoves2 = new List<string> { "U2D2L2R2", "D2U2R2L2" };
    //    var move2 = new NodeMoves(node3, possibleMoves2);
    //    var intersectMove2 = hashi3.DoMove(move2, node3);
    //    Assert.AreEqual("L2D2R2U2", intersectMove2);

    //    var hashi4 = new Tactic();
    //    var node4 = new V3Island();
    //    var possibleMoves3 = new List<string> { "U1D1L1R1", "D1U1R1L1" };
    //    var move3 = new NodeMoves(node4, possibleMoves3);
    //    var intersectMove3 = hashi4.DoMove(move3, node4);
    //    Assert.AreEqual("L1D1R1U1", intersectMove3);
    //}

    //[Test]
    //public void CheckAddEdges()
    //{
    //    var hashi1 = new Tactic(new int[][]
    //    {
    //        new int[] { 0, 0, 1, 0, 0 },
    //        new int[] { 0, 0, 0, 0, 0 },
    //        new int[] { 1, 0, 4, 0, 1 },
    //        new int[] { 0, 0, 0, 0, 0 },
    //        new int[] { 0, 0, 1, 0, 0 }
    //    });
    //    Assert.AreEqual(8, hashi1.Bridges.Count);
    //}

    //[Test]
    //public void CheckDfs()
    //{
    //    var hashi1 = new Tactic(new int[][]
    //    {
    //        new int[] { 0, 0, 1, 0, 0 },
    //        new int[] { 0, 0, 0, 0, 0 },
    //        new int[] { 1, 0, 4, 0, 1 },
    //        new int[] { 0, 0, 0, 0, 0 },
    //        new int[] { 0, 0, 1, 0, 0 }
    //    });
    //    hashi1.WhichOne = new V3Island[hashi1.Nodes.Count];
    //    hashi1.Done = new bool[hashi1.Nodes.Count];
    //    for (int i = 0; i < hashi1.Nodes.Count; i++)
    //    {
    //        hashi1.WhichOne[i] = hashi1.Nodes[i];
    //    }
    //    bool dfs = hashi1.Dfs(hashi1.Nodes[0], hashi1.Nodes[0], hashi1.Bridges);
    //    Assert.IsTrue(dfs);
    //}

    //[Test]
    //public void CheckCreateNewEdges()
    //{
    //    var gen = new V3Generator();
    //    int[][] mainField = new int[18][];
    //    for (int i = 0; i < 18; i++)
    //    {
    //        mainField[i] = new int[18];
    //    }
    //    Random rand = new Random();
    //    int roww = rand.Next(18);
    //    int coll = rand.Next(18);
    //    var node = new V3Island(0, roww, coll);
    //    gen.Nodes.Add(node);
    //    int countOfEdges = 0;
    //    // Erstellt so lange Knoten, bis die gewünschte Anzahl erstellt wurde
    //    while (true)
    //    {
    //        int size = gen.Nodes.Count;
    //        for (int i = 0; i < size; i++)
    //        {
    //            mainField = gen.CreateNode(mainField, gen.Nodes[i]);
    //            if (gen.Nodes.Count == 108)
    //            {
    //                break;
    //            }
    //        }
    //        if (countOfEdges == gen.Bridges.Count || gen.Nodes.Count == 108)
    //        {
    //            break;
    //        }
    //        countOfEdges = gen.Bridges.Count;
    //    }
    //    foreach (var nodee in gen.Nodes)
    //    {
    //        nodee.SetAllNeighbors(mainField, gen.Nodes);
    //    }
    //    int edgesCount = gen.Bridges.Count;
    //    gen.CreateNewEdges(mainField, 50);
    //    Assert.IsTrue(gen.Bridges.Count > edgesCount);
    //}

    //[Test]
    //public void CheckSetBeta()
    //{
    //    var gen = new V3Generator();
    //    int[][] mainField = new int[18][];
    //    for (int i = 0; i < 18; i++)
    //    {
    //        mainField[i] = new int[18];
    //    }
    //    Random rand = new Random();
    //    int roww = rand.Next(18);
    //    int coll = rand.Next(18);
    //    var node = new V3Island(0, roww, coll);
    //    gen.Nodes.Add(node);
    //    int countOfEdges = 0;
    //    // Erstellt so lange Knoten, bis die gewünschte Anzahl erstellt wurde
    //    while (true)
    //    {
    //        int size = gen.Nodes.Count;
    //        for (int i = 0; i < size; i++)
    //        {
    //            mainField = gen.CreateNode(mainField, gen.Nodes[i]);
    //            if (gen.Nodes.Count == 108)
    //            {
    //                break;
    //            }
    //        }
    //        if (countOfEdges == gen.Bridges.Count || gen.Nodes.Count == 108)
    //        {
    //            break;
    //        }
    //        countOfEdges = gen.Bridges.Count;
    //    }
    //    foreach (var nodee in gen.Nodes)
    //    {
    //        nodee.SetAllNeighbors(mainField, gen.Nodes);
    //    }
    //    gen.CreateNewEdges(mainField, 50);
    //    gen.SetBeta(mainField, 100);
    //    int edgesCount = gen.Bridges.Count / 2;
    //    int numberOfDoubleEdges = 0;
    //    foreach (var edge in gen.Bridges)
    //    {
    //        if (edge.Count == 2)
    //        {
    //            numberOfDoubleEdges++;
    //        }
    //    }
    //    Assert.AreEqual(edgesCount, numberOfDoubleEdges);
    //}
}