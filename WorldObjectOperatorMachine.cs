using Godot;
using QuikGraph;
using System;

public partial class WorldObjectOperatorMachine : WorldObject
{
    public override PackedScene InterfacePanel => ResourceLoader.Load<PackedScene>("res://world_object_operator_machine_panel.tscn");

    public AdjacencyGraph<IOperator, Edge<IOperator>> OperatorGraph;

    public override void _Ready()
    {
        OperatorGraph = new AdjacencyGraph<IOperator, Edge<IOperator>>();

        OperatorGraph.AddVertex(new OperatorPrinter());
    }

    public override WorldObjectPanel CreateInterfacePanel(UiManager uiManager)
    {
        var panel = (WorldObjectOperatorMachinePanel)InterfacePanel.Instantiate();
        panel.Title.Text = ObjectName;

        return panel;
    }
}
