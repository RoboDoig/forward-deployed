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

    protected virtual Observable<TResult> CreateDataObservable()
    {
        return Observable.Never<TResult>();
    }
}
