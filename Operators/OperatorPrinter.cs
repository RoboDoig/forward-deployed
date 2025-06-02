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

    public override string GetOperatorName()
    {
        return "OperatorPrinter";
    }

    protected override Observable<Unit> CreateDataObservable()
    {
        return InputSubject.Select(x => Unit.Default).Do(x => GD.Print(x));
    }
}
