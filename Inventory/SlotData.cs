using Godot;
using System;

[GlobalClass]
public partial class SlotData : Resource
{
    [Export]
    public OperatorResource OperatorResource;
    [Export]
    public int Quantity;
}
