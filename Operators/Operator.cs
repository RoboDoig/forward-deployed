using Godot;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO - Note that once this is constructed by Godot the operator will start. We may want more logic for actully starting the data observable when we decide.
public partial class Operator<TSource, TResult> : OperatorResource
{
    public Subject<TSource> InputSubject { get; protected set; }
    public ConnectableObservable<TResult> DataSubject { get; protected set; }

    public Operator()
    {
        InputSubject = CreateInputSubject();
        DataSubject = CreateDataObservable().Publish();
        DataSubject.Connect();
    }

    public override string GetOperatorName()
    {
        return "Operator";
    }

    protected virtual Subject<TSource> CreateInputSubject() {
        return new Subject<TSource>();
    }

    protected virtual Observable<TResult> CreateDataObservable()
    {
        return Observable.Never<TResult>();
    }

    public Type GetSourceType()
    {
        return typeof(TSource);
    }

    public Type GetResultType()
    {
        return typeof(TResult);
    }

    public override GraphNodeOperator CreateGraphNode()
    {
        PackedScene graphNodeScene = ResourceLoader.Load<PackedScene>("res://graph_node.tscn");
        GraphNodeOperator graphNodeOperator = (GraphNodeOperator)graphNodeScene.Instantiate();

        graphNodeOperator.Title = GetOperatorName();

        graphNodeOperator.SetSlot(
            0, true, GetSourceType().GetHashCode(), new Color(1, 1, 1),
            true, GetResultType().GetHashCode(), new Color(1, 1, 1)
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

    public void GraphNodeOperation(GraphNodeOperator graphNodeOperator, string result)
    {
        graphNodeOperator.DisplayLabel.Text = result;
    }
}
