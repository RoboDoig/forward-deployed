using Godot;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class OperatorPrinter : Operator<int, Unit>
{
    public OperatorPrinter() : base()
    {

    }

    protected override Observable<Unit> CreateDataObservable()
    {
        return InputSubject.Select(x =>
        {
            GD.Print(x);
            return Unit.Default;
        });
    }
}
