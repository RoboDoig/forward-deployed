using Godot;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[GlobalClass]
public partial class OperatorIntegerTimer : Operator<Unit, int>
{
    public override string GetOperatorName()
    {
        return "OperatorIntegerTimer";
    }

    protected override Observable<int> CreateDataObservable()
    {
        return Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1)).Index();
    }
}
