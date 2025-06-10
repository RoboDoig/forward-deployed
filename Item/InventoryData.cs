using Godot;
using System;

[GlobalClass]
public partial class InventoryData : Resource
{
    [Export]
    public SlotData[] SlotDatas;

    [Signal]
    public delegate void InventoryInteractionEventHandler(InventoryData inventoryData, int slotIndex, int buttonIndex);

    public void OnSlotClicked(int slotIndex, int buttonIndex)
    {
        EmitSignal(SignalName.InventoryInteraction, this, slotIndex, buttonIndex);
    }
}
