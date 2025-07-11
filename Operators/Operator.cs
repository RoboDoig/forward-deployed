using Godot;
using R3;
using System;

// TODO - Note that once this is constructed by Godot the operator will start. We may want more logic for actully starting the data observable when we decide.
// TODO - Operator is a bad name? This is better described as a Transform.
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

    public override IDisposable GetInputAtSlotIndex(int idx)
    {
        switch (idx)
        {
            case 0:
                return InputSubject;
            default: return null;
        }
    }

    protected virtual Subject<TSource> CreateInputSubject()
    {
        return new Subject<TSource>();
    }

    protected virtual Observable<TResult> CreateDataObservable()
    {
        return Observable.Never<TResult>();
    }

    public Type[] GetSourceType()
    {
        return [typeof(TSource)];
    }

    public Type[] GetResultType()
    {
        return [typeof(TResult)];
    }

    public override GraphNodeOperator CreateGraphNode()
    {
        PackedScene graphNodeScene = ResourceLoader.Load<PackedScene>("res://UserInterface/graph_node_text.tscn");
        GraphNodeOperatorText graphNodeOperator = (GraphNodeOperatorText)graphNodeScene.Instantiate();

        graphNodeOperator.Title = GetOperatorName();

        graphNodeOperator.SetSlot(
            0, true, typeof(TSource).GetHashCode(), new Color(1, 1, 1),
            true, typeof(TResult).GetHashCode(), new Color(1, 1, 1)
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
}
