using Godot;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class OperatorBinaryCombinator<TSource1, TSource2, TResult> : OperatorResource
{
    public Subject<TSource1> InputSubject1 { get; protected set; }
    public Subject<TSource2> InputSubject2 { get; protected set; }
    public ConnectableObservable<TResult> DataSubject { get; protected set; }

    public OperatorBinaryCombinator()
    {
        InputSubject1 = new Subject<TSource1>();
        InputSubject2 = new Subject<TSource2>();
        DataSubject = CreateDataObservable().Publish();
        DataSubject.Connect();
    }

    public override IDisposable GetInputAtSlotIndex(int idx)
    {
        switch (idx)
        {
            case 0:
                return InputSubject1;
            case 1:
                return InputSubject2;
            default: return null;
        }
    }

    public override GraphNodeOperator CreateGraphNode()
    {
        PackedScene graphNodeScene = ResourceLoader.Load<PackedScene>("res://UserInterface/graph_node_text.tscn");
        GraphNodeOperatorText graphNodeOperator = (GraphNodeOperatorText)graphNodeScene.Instantiate();

        graphNodeOperator.Title = GetOperatorName();

        graphNodeOperator.SetSlot(
            0, true, typeof(TSource1).GetHashCode(), new Color(1, 1, 1),
            true, typeof(TResult).GetHashCode(), new Color(1, 1, 1)
        );
        graphNodeOperator.SetSlot(
            1, true, typeof(TSource2).GetHashCode(), new Color(1, 1, 1),
            false, typeof(TResult).GetHashCode(), new Color(1, 1, 1)
        );

        var sub = DataSubject.SubscribeOnCurrentSynchronizationContext().Subscribe(x =>
        {
            // TODO - we need to use call deferred for the subscription in some cases as we may not be able to call things on the graph operator from the observable thread.
            CallDeferred(nameof(GraphNodeOperation), graphNodeOperator, x.ToString());
        });

        // The subscription must be disposed when the graph node exits the tree, otherwise we'll get errors from the observable chain.
        graphNodeOperator.TreeExited += () =>
        {
            sub.Dispose();
        };

        return graphNodeOperator;
    }

    private void GraphNodeOperation(GraphNodeOperatorText graphNodeOperator, string result)
    {
        graphNodeOperator.DisplayLabel.Text = result;
    }

    protected virtual Observable<TResult> CreateDataObservable()
    {
        return Observable.Never<TResult>();
    }
}
