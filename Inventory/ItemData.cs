using Godot;
using System;

[GlobalClass]
public partial class ItemData : Resource
{
    [Export]
    public OperatorResource Operator;
    [Export]
    public Texture2D Icon;
}
