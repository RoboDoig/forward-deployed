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

    [Signal]
    public delegate void GridClickedEventHandler(int buttonIndex);
    [Signal]
    public delegate void GraphNodeRemovedEventHandler(OperatorResource op);

    private Dictionary<GraphNodeMetadata, GraphNodeOperator> OperatorMapping;
    private OperatorGraph OperatorGraph;
    private GraphNodeOperator CurrentSelected;

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

    public void AddOperator(SlotData slotData)
    {
        // TODO - account for amounts in slotdata
        OperatorGraph.AddOperator(slotData.OperatorResource);
    }

    public void CreateGraphEditNode(GraphNodeMetadata graphNodeMetadata)
    {
        var graphNode = graphNodeMetadata.Operator.CreateGraphNode();
        Action graphNodeClosedAction = () =>
        {
            if (!graphNodeMetadata.Permanent)
            {
                var removedGraphNode = OperatorGraph.RemoveOperator(graphNodeMetadata.Operator);
                EmitSignal(SignalName.GraphNodeRemoved, removedGraphNode.Operator);
            }
        };
        graphNode.CloseButton.Pressed += graphNodeClosedAction;
        graphNode.TreeExited += () => graphNode.CloseButton.Pressed -= graphNodeClosedAction;

        graphNode.PositionOffset = graphNodeMetadata.LayoutOffset;
        OperatorMapping.Add(graphNodeMetadata, graphNode);
        GraphEdit.AddChild(graphNode);
    }

    public void InitialiseGraphEditorFromOperatorGraph(OperatorGraph operatorGraph)
    {
        OperatorMapping = new Dictionary<GraphNodeMetadata, GraphNodeOperator>(); // TODO - this is always 1-to-1, should instead be some kind of bidirectional dictionary
        OperatorGraph = operatorGraph;

        // Draw existing nodes and edges
        foreach (var vertex in operatorGraph.Graph.Vertices)
        {
            CreateGraphEditNode(vertex);
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
            operatorGraph.ConnectOperators(fromOperator, (int)fromIndex, toOperator, (int)toIndex);
        };
        GraphEdit.ConnectionRequest += connectionRequestHandler;

        Godot.GraphEdit.DisconnectionRequestEventHandler disconnectionRequestHandler = (StringName fromNode, long fromIndex, StringName toNode, long toIndex) =>
        {
            var from = (GraphNodeOperator)GraphEdit.GetNode(fromNode.ToString());
            var fromOperator = OperatorMapping.Where(kvp => kvp.Value == from).First().Key;
            var to = (GraphNodeOperator)GraphEdit.GetNode(toNode.ToString());
            var toOperator = OperatorMapping.Where(kvp => kvp.Value == to).First().Key;
            operatorGraph.DisconnectOperators(fromOperator, (int)fromIndex, toOperator, (int)toIndex);
        };
        GraphEdit.DisconnectionRequest += disconnectionRequestHandler;

        Godot.GraphEdit.NodeSelectedEventHandler nodeSelectedAction = (n) =>
        {
            var selectedNode = (GraphNodeOperator)n;
            CurrentSelected = selectedNode;
        };
        GraphEdit.NodeSelected += nodeSelectedAction;

        Action moveNodeFinishedAction = () => {
            if (CurrentSelected != null)
            {
                var selectedOperator = OperatorMapping.Where(kvp => kvp.Value == CurrentSelected).First().Key;
                selectedOperator.LayoutOffset = CurrentSelected.PositionOffset;
            }
        };
        GraphEdit.EndNodeMove += moveNodeFinishedAction;

        // Connect graph signals
        VertexAction<GraphNodeMetadata> vertexAddedAction = (v) =>
        {
            CreateGraphEditNode(v);
        };
        operatorGraph.Graph.VertexAdded += vertexAddedAction;

        VertexAction<GraphNodeMetadata> vertexRemovedAction = (v) =>
        {
            OperatorMapping[v].QueueFree();
        };
        operatorGraph.Graph.VertexRemoved += vertexRemovedAction;

        EdgeAction<GraphNodeMetadata, STaggedEdge<GraphNodeMetadata, Vector2I>> edgeAddedAction = (e) =>
        {
            GraphEdit.ConnectNode(OperatorMapping[e.Source].Name, e.Tag.X, OperatorMapping[e.Target].Name, e.Tag.Y);
        };
        operatorGraph.Graph.EdgeAdded += edgeAddedAction;

        EdgeAction<GraphNodeMetadata, STaggedEdge<GraphNodeMetadata, Vector2I>> edgeRemovedAction = (e) =>
        {
            GraphEdit.DisconnectNode(OperatorMapping[e.Source].Name, e.Tag.X, OperatorMapping[e.Target].Name, e.Tag.Y);
        };
        operatorGraph.Graph.EdgeRemoved += edgeRemovedAction;

        // Unsubscribe from handlers
        TreeExited += () =>
        {
            GraphEdit.ConnectionRequest -= connectionRequestHandler;
            GraphEdit.DisconnectionRequest -= disconnectionRequestHandler;
            GraphEdit.NodeSelected -= nodeSelectedAction;
            GraphEdit.EndNodeMove -= moveNodeFinishedAction;

            operatorGraph.Graph.EdgeAdded -= edgeAddedAction;
            operatorGraph.Graph.EdgeRemoved -= edgeRemovedAction;
            operatorGraph.Graph.VertexAdded -= vertexAddedAction;
            operatorGraph.Graph.VertexRemoved -= vertexRemovedAction;
        };
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton inputEventMouseButton && @event.IsPressed() && inputEventMouseButton.ButtonIndex == MouseButton.Left)
        {
            var buttonEvent = (InputEventMouseButton)@event;
            EmitSignal(SignalName.GridClicked, (int)buttonEvent.ButtonIndex);
        }
    }
}
