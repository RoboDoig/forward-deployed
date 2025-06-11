using Godot;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class OperatorNot : Operator<bool, bool>
{
    public override string GetOperatorName()
    {
        return "OperatorNOT";
    }

    protected override Observable<bool> CreateDataObservable()
    {
        return InputSubject.Select(x => !x);
    }
}

