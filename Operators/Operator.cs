using Godot;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.HttpRequest;

public class Operator<TSource, TResult> : Operator
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
}

public abstract class Operator
{

}
