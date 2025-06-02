using Godot;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Operator<TSource, TResult> : IOperator
{
    public Subject<TSource> InputSubject { get; protected set; }
    public ConnectableObservable<TResult> DataSubject { get; protected set; }

    public Operator()
    {
        InputSubject = CreateInputSubject();
        DataSubject = CreateDataObservable().Publish();
        DataSubject.Connect();
    }

    public virtual string GetOperatorName()
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

    public virtual GraphNodeOperator CreateGraphNode()
    {
        PackedScene graphNodeScene = ResourceLoader.Load<PackedScene>("res://graph_node.tscn");
        GraphNodeOperator graphNodeOperator = (GraphNodeOperator)graphNodeScene.Instantiate();

        graphNodeOperator.Title = GetOperatorName();

        var sub = DataSubject.Subscribe(x =>
        {
            graphNodeOperator.DisplayLabel.Text = x.ToString();
        });

        // The subscription must be disposed when the graph node exits the tree, otherwise we'll get errors from the observable chain.
        graphNodeOperator.TreeExited += () =>
        {
            sub.Dispose();
        };

        return graphNodeOperator;
    }
}
