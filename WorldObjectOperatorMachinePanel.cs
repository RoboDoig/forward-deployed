using Godot;
using QuikGraph;
using System;
using System.Collections.Generic;

public partial class WorldObjectOperatorMachinePanel : WorldObjectPanel
{
    [Export]
    public GraphEdit GraphEdit;

    private Dictionary<IOperator, GraphNodeOperator> OperatorMapping;

    public void InitialiseGraphEditorFromOperatorGraph(OperatorGraph operatorGraph)
    {
        OperatorMapping = new Dictionary<IOperator, GraphNodeOperator>();

        // Draw existing nodes and edges
        foreach (var vertex in operatorGraph.Graph.Vertices)
        {
            var graphNode = vertex.CreateGraphNode();

            OperatorMapping.Add(vertex, graphNode);

            GraphEdit.AddChild(graphNode);

            graphNode.SetSlot(
                0, true, vertex.GetSourceType().GetHashCode(), new Color(1, 1, 1),
                true, vertex.GetResultType().GetHashCode(), new Color(1, 1, 1)
            );
        }

        foreach (var edge in operatorGraph.Graph.Edges)
        {
            GraphEdit.ConnectNode(OperatorMapping[edge.Source].Name, 0, OperatorMapping[edge.Target].Name, 0);
        }

        // Connect graph edit signals
        GraphEdit.ConnectionRequest += (a, b, c, d) =>
        {

        };

        GraphEdit.DisconnectionRequest += (a, b, c, d) =>
        {

        };

        // Connect graph signals
        operatorGraph.Graph.VertexAdded += (a) =>
        {

        };

        operatorGraph.Graph.VertexRemoved += (a) =>
        {

        };

        operatorGraph.Graph.EdgeAdded += (a) =>
        {

        };

        operatorGraph.Graph.EdgeRemoved += (a) =>
        {

        };
    }

    private bool TryConnectOperators()
    {
        return false;
    }

    private bool TryDisconnectOperators()
    {
        return false;
    }
}
