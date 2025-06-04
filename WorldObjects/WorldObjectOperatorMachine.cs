using Godot;
using QuikGraph;
using R3;
using System;
using System.Collections.Generic;

public partial class WorldObjectOperatorMachine : WorldObject
{
    public override PackedScene InterfacePanel => ResourceLoader.Load<PackedScene>("res://world_object_operator_machine_panel.tscn");

    public OperatorGraph OperatorGraph;

    public override void _Ready()
    {
        OperatorGraph = new OperatorGraph();

        //var operatorPrinter = new OperatorPrinter();
        //var timer = new OperatorIntegerTimer();

        //OperatorGraph.AddOperator(operatorPrinter);
        //OperatorGraph.AddOperator(timer);

        //OperatorGraph.ConnectOperators(timer, operatorPrinter);

        OperatorGraph.AddOperator(new OperatorBooleanSignal());
    }

    public override WorldObjectPanel CreateInterfacePanel(UiManager uiManager)
    {
        var panel = (WorldObjectOperatorMachinePanel)InterfacePanel.Instantiate();
        panel.Title.Text = ObjectName;

        panel.InitialiseGraphEditorFromOperatorGraph(OperatorGraph);

        return panel;
    }

    public class OperatorBooleanSignal : Operator<bool, bool>
    {
        public override string GetOperatorName()
        {
            return "OperatorBooleanSignal";
        }

        protected override Observable<bool> CreateDataObservable()
        {
            return InputSubject.Select(x => x);
        }
    }
}
