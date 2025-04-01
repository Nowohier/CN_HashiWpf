package org.linearsolution;

import org.chocosolver.solver.Model;
import org.chocosolver.solver.Solution;
import org.chocosolver.solver.variables.BoolVar;
import org.chocosolver.solver.variables.IntVar;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class linearsolutionwithiterativ {

    private ArrayList<node> nodes = new ArrayList<>();
    private ArrayList<edge> edges = new ArrayList<>();
    private ArrayList<edgepairs> delta = new ArrayList<>();
    public int numberofsol = 0;
    private int countofsol = 0;
    private char[][] fieldchars;
    private boolean solvable = false;


    public linearsolutionwithiterativ(int[][] mainfield) {
        //Model und Solver erstellen
        Model model = new Model("my Model");
        //Erstelle jeden Knoten
        int numberofnodes = 0;
        for (int row = 0; row < mainfield.length; row++) {
            for (int col = 0; col < mainfield[row].length; col++) {
                if (mainfield[row][col] != 0) {
                    numberofnodes++;
                    nodes.add(new node(mainfield[row][col], row, col, numberofnodes - 1));
                }
            }
        }
        //setze für jeden Knoten die Nachbarn
        for (node node : nodes) {
            node.setallneighbours(mainfield, nodes);
        }
        //setze für jeden Knoten die lowerneighbours und upneighbours,
        //lowernighbours = vom aktuellen Knoten obere und linken Nachbar, upneighbours = vom aktuellen Knoten rechten und unteren Nachbar
        for (node node : nodes) {
            if (node.getUp() != null) {
                node.getLowerneighbours().add(node.getUp());
            }
            if (node.getLeft() != null) {
                node.getLowerneighbours().add(node.getLeft());
            }
            if (node.getRight() != null) {
                node.getUpneighbours().add(node.getRight());
            }
            if (node.getDown() != null) {
                node.getUpneighbours().add(node.getDown());
            }
        }

        //alle möglichen Kanten werden in edges hinzugefügt
        for (node node : nodes) {
            for (node nodelow : node.getLowerneighbours()) {
                boolean checker = false;
                edge edgeadd = new edge(node, nodelow);
                for (edge edge : edges) {
                    if (edge.getNode1().getX() == edgeadd.getNode1().getX() && edge.getNode1().getY() == edgeadd.getNode1().getY() && edge.getNode2().getX() == edgeadd.getNode2().getX() && edge.getNode2().getY() == edgeadd.getNode2().getY()) {
                        checker = true;
                        break;
                    }
                }
                if (!checker) {
                    edges.add(edgeadd);
                }
            }
            for (node nodeup : node.getUpneighbours()) {
                boolean checker = false;
                edge edgeadd = new edge(node, nodeup);
                for (edge edge : edges) {
                    if (edge.getNode1().getX() == edgeadd.getNode1().getX() && edge.getNode1().getY() == edgeadd.getNode1().getY() && edge.getNode2().getX() == edgeadd.getNode2().getX() && edge.getNode2().getY() == edgeadd.getNode2().getY()) {
                        checker = true;
                        break;
                    }
                }
                if (!checker) {
                    edges.add(edgeadd);
                }
            }
        }

        //suchen von möglichen sich kreuzenden Kanten, diese Kantenpaare werden in delta gespeichert
        for (node node : nodes) {
            //kontrolliert für den Knoten nach unten
            if (node.getDown() != null) {
                List<node> givendown = node.downblocked(nodes);
                boolean checkdown = false;
                for (node nodedown : givendown) {
                    for (edgepairs edge : delta) {
                        if ((edge.getEdge1()[0] == node.getNumber() && edge.getEdge1()[1] == node.getDown().getNumber()
                                && edge.getEdge2()[0] == nodedown.getNumber() && edge.getEdge2()[1] == nodedown.getRight().getNumber())
                                || (edge.getEdge2()[0] == node.getNumber() && edge.getEdge2()[1] == node.getDown().getNumber()
                                && edge.getEdge1()[0] == nodedown.getNumber() && edge.getEdge1()[1] == nodedown.getRight().getNumber())) {
                            checkdown = true;
                        }
                    }
                    if (!checkdown) {
                        delta.add(new edgepairs(node.getNumber(), node.getDown().getNumber(), nodedown.getNumber(), nodedown.getRight().getNumber()));
                    }
                }
            }
            //kontrolliert für den Knoten nach rechts
            if (node.getRight() != null) {
                List<node> givenright = node.rightblocked(nodes);
                boolean checkright = false;
                for (node noderight : givenright) {
                    for (edgepairs edge : delta) {
                        if ((edge.getEdge1()[0] == node.getNumber() && edge.getEdge1()[1] == node.getRight().getNumber()
                                && edge.getEdge2()[0] == noderight.getNumber() && edge.getEdge2()[1] == noderight.getDown().getNumber())
                                || (edge.getEdge2()[0] == node.getNumber() && edge.getEdge2()[1] == node.getRight().getNumber()
                                && edge.getEdge1()[0] == noderight.getNumber() && edge.getEdge1()[1] == noderight.getDown().getNumber())) {
                            checkright = true;
                        }
                    }
                    if (!checkright) {
                        delta.add(new edgepairs(node.getNumber(), node.getRight().getNumber(), noderight.getNumber(), noderight.getDown().getNumber()));
                    }
                }
            }
        }
        //erstellt die Variablen x
        IntVar[] x = new IntVar[edges.size()];
        //erstellt die Variablen y
        IntVar[] y = new IntVar[edges.size()];

        //jeder x Wert entspricht der Anzahl an Kanten zwischen zwei Knoten, damit ist der Wert zwischen einschließlich 0 und 2
        for (int i = 0; i < x.length; i++) {
            x[i] = model.intVar(edges.get(i).getNode1().getY() + "/" + edges.get(i).getNode1().getX() + "," + edges.get(i).getNode2().getY() + "/" + edges.get(i).getNode2().getX(), 0, 2);
        }

        //jeder y Wert gibt an, ob eine Kante zwischen zwei Knoten existiert
        for (int i = 0; i < y.length; i++) {
            y[i] = model.intVar(edges.get(i).getNode1().getY() + "/" + edges.get(i).getNode1().getX() + "," + edges.get(i).getNode2().getY() + "/" + edges.get(i).getNode2().getX(), 0, 1);
        }


        for (node node : nodes) {
            //speichert in posedgeslow die Positionen von den Kanten in x,
            //welche zwischen dem aktuellen Knoten und dem Knoten aus lowerneighbours sind
            List<Integer> posedgeslow = new ArrayList<>();
            for (node nodelow : node.getLowerneighbours()) {
                for (int i = 0; i < edges.size(); i++) {
                    if (edges.get(i).getNode1() == nodelow && edges.get(i).getNode2() == node) {
                        posedgeslow.add(i);
                    }
                }
            }
            //speichert in posedgesup die Positionen von den Kanten in x,
            //welche zwischen dem aktuellen Knoten und dem Knoten aus upneighbours sind
            List<Integer> posedgesup = new ArrayList<>();
            for (node nodeup : node.getUpneighbours()) {
                for (int i = 0; i < edges.size(); i++) {
                    if (edges.get(i).getNode1() == node && edges.get(i).getNode2() == nodeup) {
                        posedgesup.add(i);
                    }
                }
            }
            //erstelle zwei Variablen um die x Werte summieren zu können
            IntVar lowIntVars = model.intVar(0);
            IntVar upIntVars = model.intVar(0);
            //in lowIntVars wird die Summe der x Werte von den Kanten gespeichert, welche einen lowerneighbours als Knoten haben
            for (Integer low : posedgeslow) {
                lowIntVars = model.sum("lowintvars", lowIntVars, x[low]).intVar();
            }
            //in upIntVars wird die Summe der x Werte von den Kanten gespeichert, welche einen upneighbours als Knoten haben
            for (Integer up : posedgesup) {
                upIntVars = model.sum("lowintvars", upIntVars, x[up]).intVar();
            }
            //lowIntVars und upIntVars zusammen addiert sollen dem Wert des aktuellen Knotens entsprechen
            model.sum(new IntVar[]{lowIntVars, upIntVars}, "=", node.getValue()).post();
        }

        //der x Wert der Kante befindet sich zwischen dem y Wert der Kante und dem doppelten des y Werts
        for (int i = 0; i < edges.size(); i++) {
            model.arithm(y[i], "<=", x[i]).post();
            IntVar times = model.intVar(0, 2);
            model.times(y[i], 2, times).post();
            model.arithm(x[i], "<=", times).post();
        }

        //das addieren von den y Werten der beiden Kanten in den jeweiligen Kantenpaaren muss kleiner gleich 1
        //dabei wird in first und second die Stelle der Kanten gespeichert an der diese sich im y Array befinden
        for (edgepairs edgepair : delta) {
            int first = 0;
            int second = 0;
            for (int i = 0; i < edges.size(); i++) {
                if (edges.get(i).getNode1().getNumber() == edgepair.getEdge1()[0] && edges.get(i).getNode2().getNumber() == edgepair.getEdge1()[1]) {
                    first = i;
                }
                if (edges.get(i).getNode1().getNumber() == edgepair.getEdge2()[0] && edges.get(i).getNode2().getNumber() == edgepair.getEdge2()[1]) {
                    second = i;
                }
            }
            model.arithm(model.sum("sumy", y[first], y[second]), "<=", 1).post();
        }

        //Folgende auskommentieren um die 7. Bedingung benutzen zu können
  /*      //addiert die y Werte zusammen und diese sollen >= Anzahl an Knoten - 1 sein
        IntVar sumofedgesy = model.intVar(0);

        for (IntVar intvar : y) {
            sumofedgesy = model.sum("sumofedgesy", sumofedgesy, intvar).intVar();
        }

        model.arithm(sumofedgesy, ">=", nodes.size() - 1).post();*/

        //erstellt die erste mögliche Lösung
        int[] values;
        Solution solution = model.getSolver().findSolution();
        List<helper> allcomp = new ArrayList<>();
        values = new int[x.length];
        if (solution != null) {
            for (int i = 0; i < x.length; i++) {
                values[i] = solution.getIntVal(x[i]);
            }
            //suche alle komponenten
            allcomp = findComponents(edges, nodes.size(), values);
        }
        countofsol++;
        while (allcomp.size() > 1 && solution != null) {
            model.getSolver().reset();
            for (helper comp : allcomp) {
                BoolVar[] constrains = new BoolVar[comp.getEdges().size()];
                for (int j = 0; j < comp.getEdges().size(); j++) {
                    constrains[j] = x[comp.getEdges().get(j)].ne(values[comp.getEdges().get(j)]).boolVar();
                }
                model.or(constrains).post();
            }
            solution = model.getSolver().findSolution();
            if (solution != null) {
                countofsol++;
                values = new int[x.length];
                for (int i = 0; i < x.length; i++) {
                    values[i] = solution.getIntVal(x[i]);
                }
                allcomp.clear();
                allcomp = findComponents(edges, nodes.size(), values);
            }
        }
        if (allcomp.size() == 1) {
            solvable = true;
            numberofsol++;
        }
        fieldchars = new char[mainfield.length][mainfield[0].length];
        for (char[] fieldchar : fieldchars) {
            Arrays.fill(fieldchar, '0');
        }
        for (node node : nodes) {
            fieldchars[node.getY()][node.getX()] = (char) (node.getValue() + 48);
        }
        int iter = 1;
        for (int i = 0; i < values.length; i++) {
            if (values[i] != 0) {
                System.out.println(iter +". Kante:" + " Von Knoten (" + edges.get(i).getNode1().getY() + "/" + edges.get(i).getNode1().getX() + ") mit Wert " + edges.get(i).getNode1().getValue() + " nach Knoten (" + edges.get(i).getNode2().getY() + "/" + edges.get(i).getNode2().getX() + ") mit Wert " + edges.get(i).getNode2().getValue() + " mit Kantenanzahl: " + values[i] + "." + '\n');
                printedges(edges.get(i), values[i]);
                iter++;
            }
        }
        System.out.println(Arrays.deepToString(fieldchars)
                .replace("],", "},\n").replace("[", "{")
                .replaceAll("[\\[\\]]", "}"));

        System.out.println("\nEs wurden " + countofsol + " Lösungen generiert bis zur Lösungsfindung.");
        if (solvable) {
            Solution solutions = model.getSolver().findSolution();
            if (solutions != null) {
                numberofsol = 2;
            }
        }
    }

    //Findet die Zusammenhangskomponenten
    public List<helper> findComponents(List<edge> edges, int numNodes, int[] value) {
        List<helper> components = new ArrayList<>();
        boolean[] besucht = new boolean[numNodes];

        //Erstelle den Graphen in einer Liste wobei gespeichert wird,
        //zu welchen Knoten eine Kante existiert für den jeweiligen Knoten
        List<helper> graph = new ArrayList<>();
        for (int i = 0; i < numNodes; i++) {
            graph.add(new helper(new ArrayList<>(), new ArrayList<>()));
        }

        for (int i = 0; i < edges.size(); i++) {
            if (value[i] != 0) {
                graph.get(edges.get(i).getNode1().getNumber()).getNode().add(edges.get(i).getNode2().getNumber());
                graph.get(edges.get(i).getNode2().getNumber()).getNode().add(edges.get(i).getNode1().getNumber());
                graph.get(edges.get(i).getNode1().getNumber()).getEdges().add(i);
            }
        }

        for (int node = 0; node < numNodes; node++) {
            if (!besucht[node]) {
                helper component = new helper(new ArrayList<>(), new ArrayList<>());
                dfs(graph, node, besucht, component);
                components.add(component);
            }
        }
        return components;
    }

    //Führt die Tiefensuche aus
    private void dfs(List<helper> graph, int node, boolean[] besucht, helper component) {
        besucht[node] = true;
        component.getNode().add(node);
        for (int i = 0; i < graph.get(node).getEdges().size(); i++) {
            component.getEdges().add(graph.get(node).getEdges().get(i));
        }

        for (int neighbor : graph.get(node).getNode()) {
            if (!besucht[neighbor]) {
                dfs(graph, neighbor, besucht, component);
            }
        }
    }

    //Erstellt die Kanten im 2dArray für die Ausgabe
    public void printedges(edge edge, int edgenumber) {
        if (edge.getNode1().getX() < edge.getNode2().getX()) {
            for (int i = edge.getNode1().getX() + 1; i < edge.getNode2().getX(); i++) {
                if (edgenumber == 1) {
                    this.fieldchars[edge.getNode1().getY()][i] = '-';
                }
                if (edgenumber == 2) {
                    this.fieldchars[edge.getNode1().getY()][i] = '=';
                }
            }
        }
        if (edge.getNode1().getX() > edge.getNode2().getX()) {
            for (int i = edge.getNode2().getX() + 1; i < edge.getNode1().getX(); i++) {
                if (edgenumber == 1) {
                    this.fieldchars[edge.getNode1().getY()][i] = '-';
                }
                if (edgenumber == 2) {
                    this.fieldchars[edge.getNode1().getY()][i] = '=';
                }
            }
        }
        if (edge.getNode1().getY() < edge.getNode2().getY()) {
            for (int i = edge.getNode1().getY() + 1; i < edge.getNode2().getY(); i++) {
                if (edgenumber == 1) {
                    this.fieldchars[i][edge.getNode1().getX()] = '|';
                }
                if (edgenumber == 2) {
                    this.fieldchars[i][edge.getNode1().getX()] = '"';
                }
            }
        }
        if (edge.getNode1().getY() > edge.getNode2().getY()) {
            for (int i = edge.getNode2().getY() + 1; i < edge.getNode1().getY(); i++) {
                if (edgenumber == 1) {
                    this.fieldchars[i][edge.getNode1().getX()] = '|';
                }
                if (edgenumber == 2) {
                    this.fieldchars[i][edge.getNode1().getX()] = '"';
                }
            }
        }
    }
}
