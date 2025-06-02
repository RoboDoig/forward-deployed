using Godot;
using QuikGraph;
using System;
using System.Collections.Generic;

public partial class WorldObjectOperatorMachine : WorldObject
{
    public override PackedScene InterfacePanel => ResourceLoader.Load<PackedScene>("res://world_object_operator_machine_panel.tscn");

    public OperatorGraph OperatorGraph;

    public override void _Ready()
    {
        OperatorGraph = new OperatorGraph();

        var operatorPrinter = new OperatorPrinter();
        var timer = new OperatorIntegerTimer();

        OperatorGraph.AddOperator(operatorPrinter);
        OperatorGraph.AddOperator(timer);

        OperatorGraph.ConnectOperators(timer, operatorPrinter);
    }

    public override WorldObjectPanel CreateInterfacePanel(UiManager uiManager)
    {
        var panel = (WorldObjectOperatorMachinePanel)InterfacePanel.Instantiate();
        panel.Title.Text = ObjectName;

        panel.InitialiseGraphEditorFromOperatorGraph(OperatorGraph);

        return panel;
    }
}
