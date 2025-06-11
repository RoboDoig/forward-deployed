using Godot;
using System;

[GlobalClass]
public partial class ItemData : Resource
{
    [Export]
    public OperatorResource Operator;

    //public IOperator CreateOperatorFromItem()
    //{
    //    return (IOperator)Activator.CreateInstance(Type.GetType(OperatorType)); // TODO - this is going to break... badly.
    //}
}
