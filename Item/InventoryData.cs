using Godot;
using System;

[GlobalClass]
public partial class InventoryData : Resource
{
    [Export]
    public SlotData[] SlotDatas;
}
