using Godot;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class OperatorPrinter : ObservableOperator<int, string>
{
    public OperatorPrinter() : base()
    {

    }

    public override string GetOperatorName()
    {
        return "OperatorPrinter";
    }

    protected override Observable<string> CreateDataObservable()
    {
        return InputSubject.Select(x => x.ToString());
    }
}
