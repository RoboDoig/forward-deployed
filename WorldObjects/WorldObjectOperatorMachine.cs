using Godot;
using QuikGraph;
using R3;
using System;
using System.Collections.Generic;

public partial class WorldObjectOperatorMachine : WorldObject
{
    public override PackedScene InterfacePanel => ResourceLoader.Load<PackedScene>("res://UserInterface/world_object_operator_machine_panel.tscn");

    public OperatorGraph OperatorGraph;

    [Export]
    private Light3D SignalLight;

    public override void _Ready()
    {
        OperatorGraph = new OperatorGraph();

        var outputOperator = new OperatorBooleanSignal();
        var outputNode = new OperatorGraph.GraphNodeMetadata
        {
            Operator = outputOperator,
            LayoutOffset = new Vector2(240, 100),
            Permanent = true
        };
        OperatorGraph.AddOperator(outputNode);

        var notOperator = new OperatorNot();
        var notNode = new OperatorGraph.GraphNodeMetadata
        {
            Operator = notOperator,
            LayoutOffset = new Vector2(40, 100),
            Permanent = false
        };
        OperatorGraph.AddOperator(notNode);

        OperatorGraph.ConnectOperators(notNode, outputNode);

        outputOperator.DataSubject.Subscribe(x =>
        {
            CallDeferred(nameof(SignalLightFunction), x);
        });
    }

    private void SignalLightFunction(bool state)
    {
        if (state)
        {
            SignalLight.LightColor = new Color(0, 1, 0);
        }
        else
        {
            SignalLight.LightColor = new Color(1, 0, 0);
        }
    }

    public override WorldObjectPanel CreateInterfacePanel(UiManager uiManager)
    {
        var panel = (WorldObjectOperatorMachinePanel)InterfacePanel.Instantiate();
        panel.Title.Text = ObjectName;

        panel.InitialiseGraphEditorFromOperatorGraph(OperatorGraph);

        return panel;
    }

    public partial class OperatorBooleanSignal : Operator<bool, bool>
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
