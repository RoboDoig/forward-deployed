using Godot;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[GlobalClass]
public partial class OperatorAddInteger : OperatorBinaryCombinator<int, int, int>
{
    public override string GetOperatorName()
    {
        return "OperatorIntegerTimer";
    }

    protected override Observable<int> CreateDataObservable()
    {
        return InputSubject1.CombineLatest(InputSubject2, (xs, ys) => xs + ys);
    }
}
