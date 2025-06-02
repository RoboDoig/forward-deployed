using Godot;
using QuikGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
        }

        foreach (var edge in operatorGraph.Graph.Edges)
        {
            // TODO - hard coded indices
            GraphEdit.ConnectNode(OperatorMapping[edge.Source].Name, 0, OperatorMapping[edge.Target].Name, 0);
        }

        // Connect graph edit signals, TODO - repeated code for looking up dict in reverse
        Godot.GraphEdit.ConnectionRequestEventHandler connectionRequestHandler = (StringName fromNode, long fromIndex, StringName toNode, long toIndex) =>
        {
            var from = (GraphNodeOperator)GraphEdit.GetNode(fromNode.ToString());
            var fromOperator = OperatorMapping.Where(kvp => kvp.Value == from).First().Key;
            var to = (GraphNodeOperator)GraphEdit.GetNode(toNode.ToString());
            var toOperator = OperatorMapping.Where(kvp => kvp.Value == to).First().Key;
            operatorGraph.ConnectOperators(fromOperator, toOperator);
        };
        GraphEdit.ConnectionRequest += connectionRequestHandler;

        Godot.GraphEdit.DisconnectionRequestEventHandler disconnectionRequestHandler = (StringName fromNode, long fromIndex, StringName toNode, long toIndex) =>
        {
            var from = (GraphNodeOperator)GraphEdit.GetNode(fromNode.ToString());
            var fromOperator = OperatorMapping.Where(kvp => kvp.Value == from).First().Key;
            var to = (GraphNodeOperator)GraphEdit.GetNode(toNode.ToString());
            var toOperator = OperatorMapping.Where(kvp => kvp.Value == to).First().Key;
            operatorGraph.DisconnectOperators(fromOperator, toOperator);
        };
        GraphEdit.DisconnectionRequest += disconnectionRequestHandler;

        // Connect graph signals
        operatorGraph.Graph.VertexAdded += (v) =>
        {

        };

        operatorGraph.Graph.VertexRemoved += (v) =>
        {

        };

        EdgeAction<IOperator, Edge<IOperator>> edgeAddedAction = (e) =>
        {
            GraphEdit.ConnectNode(OperatorMapping[e.Source].Name, 0, OperatorMapping[e.Target].Name, 0);
        };
        operatorGraph.Graph.EdgeAdded += edgeAddedAction;

        EdgeAction<IOperator, Edge<IOperator>> edgeRemovedAction = (e) =>
        {
            GraphEdit.DisconnectNode(OperatorMapping[e.Source].Name, 0, OperatorMapping[e.Target].Name, 0);
        };
        operatorGraph.Graph.EdgeRemoved += edgeRemovedAction;

        // Unsubscribe from handlers
        TreeExited += () =>
        {
            GraphEdit.ConnectionRequest -= connectionRequestHandler;
            GraphEdit.DisconnectionRequest -= disconnectionRequestHandler;
            operatorGraph.Graph.EdgeAdded -= edgeAddedAction;
            operatorGraph.Graph.EdgeRemoved -= edgeRemovedAction;
        };
    }
}
