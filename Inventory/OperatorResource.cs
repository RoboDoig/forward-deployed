using Godot;
using System;

[GlobalClass]
public partial class OperatorResource : Resource
{
    public virtual GraphNodeOperator CreateGraphNode()
    {
        return null;
    }
}
