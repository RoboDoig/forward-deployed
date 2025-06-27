using Godot;
using System;

[GlobalClass]
public partial class SlotData : Resource
{
    [Export]
    public ItemData ItemData;
    [Export]
    public int Quantity;
}
