using Godot;
using System;

[GlobalClass]
public partial class OperatorResource : Resource
{
    [Export]
    public Texture2D Icon;

    public virtual GraphNodeOperator CreateGraphNode()
    {
        return null;
    }

    public virtual string GetOperatorName()
    {
        return "Default";
    }
}
