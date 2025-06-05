using Godot;
using QuikGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using static OperatorGraph;

public partial class WorldObjectOperatorMachinePanel : WorldObjectPanel
{
    [Export]
    public GraphEdit GraphEdit;

    [Export]
    public Button AddTimerButton;
    [Export]
    public Button AddIsEvenButton;
    [Export]
    public Button AddNotButton;

    private Dictionary<GraphNodeMetadata, GraphNodeOperator> OperatorMapping;
    private OperatorGraph OperatorGraph;

    public override void _Ready()
    {
        base._Ready();

        Action addTimerPressedHandler = () =>
        {
            var timer = new OperatorIntegerTimer();
            OperatorGraph.AddOperator(timer); // TODO - this should maybe signal 'up' to the world object
        };
        AddTimerButton.Pressed += addTimerPressedHandler;

        Action isEvenPressedHandler = () =>
        {
            var isEven = new OperatorIntegerIsEven();
            OperatorGraph.AddOperator(isEven);
        };
        AddIsEvenButton.Pressed += isEvenPressedHandler;

        Action notPressedHandler = () =>
        {
            var not = new OperatorNot();
            OperatorGraph.AddOperator(not);
        };
        AddNotButton.Pressed += notPressedHandler;
    }

    public void InitialiseGraphEditorFromOperatorGraph(OperatorGraph operatorGraph)
    {
        OperatorMapping = new Dictionary<GraphNodeMetadata, GraphNodeOperator>();
        OperatorGraph = operatorGraph;

        // Draw existing nodes and edges
        foreach (var vertex in operatorGraph.Graph.Vertices)
        {
            var graphNode = vertex.Operator.CreateGraphNode();
            graphNode.PositionOffset = vertex.LayoutOffset;

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
        VertexAction<GraphNodeMetadata> vertexAddedAction = (v) =>
        {
            var graphNode = v.Operator.CreateGraphNode();
            OperatorMapping.Add(v, graphNode);
            GraphEdit.AddChild(graphNode);
        };
        operatorGraph.Graph.VertexAdded += vertexAddedAction;

        operatorGraph.Graph.VertexRemoved += (v) =>
        {

        };

        EdgeAction<GraphNodeMetadata, Edge<GraphNodeMetadata>> edgeAddedAction = (e) =>
        {
            GraphEdit.ConnectNode(OperatorMapping[e.Source].Name, 0, OperatorMapping[e.Target].Name, 0);
        };
        operatorGraph.Graph.EdgeAdded += edgeAddedAction;

        EdgeAction<GraphNodeMetadata, Edge<GraphNodeMetadata>> edgeRemovedAction = (e) =>
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
            operatorGraph.Graph.VertexAdded -= vertexAddedAction;
        };
    }
}
