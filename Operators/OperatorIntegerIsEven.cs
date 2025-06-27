using Godot;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[GlobalClass]
public partial class OperatorIntegerIsEven : Operator<int, bool>
{
    public override string GetOperatorName()
    {
        return "OperatorIntegerIsEven";
    }

    protected override Observable<bool> CreateDataObservable()
    {
        return InputSubject.Select(x =>
        {
            return int.IsEvenInteger(x);
        });
    }
}

