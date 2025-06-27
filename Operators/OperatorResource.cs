using Godot;
using System;

[GlobalClass]
public partial class OperatorResource : Resource
{
    public virtual GraphNodeOperator CreateGraphNode()
    {
        return null;
    }

    public virtual string GetOperatorName()
    {
        return "Default";
    }
}
