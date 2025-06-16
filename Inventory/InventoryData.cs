using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class InventoryData : Resource
{
    [Export]
    public SlotData[] SlotDatas;
    [Signal]
    public delegate void InventoryUpdatedEventHandler(InventoryData inventoryData);

    public Tuple<InventoryData, SlotData> RemoveItemAtIndex(int index)
    {
        var slot = SlotDatas[index];
        SlotDatas[index] = null;

        EmitSignal(SignalName.InventoryUpdated, this);

        return new Tuple<InventoryData, SlotData>(this, slot);
    }

    public Tuple<InventoryData, SlotData> DropItemAtIndex(SlotData slotToDrop, int atIndex)
    {
        var currentlyAtSlot = SlotDatas[atIndex];
        SlotDatas[atIndex] = slotToDrop;

        EmitSignal(SignalName.InventoryUpdated, this);

        return new Tuple<InventoryData, SlotData>(this, currentlyAtSlot);
    }
}
