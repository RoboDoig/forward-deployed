using Godot;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[GlobalClass]
public partial class OperatorNot : Operator<bool, bool>
{
    public override string GetOperatorName()
    {
        return "OperatorNOT";
    }

    public override GraphNodeOperator CreateGraphNode()
    {
        PackedScene graphNodeScene = ResourceLoader.Load<PackedScene>("res://UserInterface/graph_node_color.tscn");
        GraphNodeOperatorColor graphNodeOperator = (GraphNodeOperatorColor)graphNodeScene.Instantiate();

        graphNodeOperator.Title = GetOperatorName();

        graphNodeOperator.SetSlot(
            0, true, GetSourceType().GetHashCode(), new Color(1, 1, 1),
            true, GetResultType().GetHashCode(), new Color(1, 1, 1)
        );

        var sub = DataSubject.SubscribeOnCurrentSynchronizationContext().Subscribe(x =>
        {
            // TODO - we need to use call deferred for the subscription in some cases as we may not be able to call things on the graph operator from the observable thread.
            CallDeferred(nameof(GraphNodeOperation), graphNodeOperator, x);
        });

        graphNodeOperator.TreeExited += () =>
        {
            sub.Dispose();
        };

        return graphNodeOperator;
    }

    private void GraphNodeOperation(GraphNodeOperatorColor graphNodeOperator, bool result)
    {
        if (result)
        {
            graphNodeOperator.ColorRect.Color = new Color(0, 1, 0);
        }
        else
        {
            graphNodeOperator.ColorRect.Color = new Color(1, 0, 0);
        }
    }

    protected override Observable<bool> CreateDataObservable()
    {
        return InputSubject.Select(x => !x);
    }
}

