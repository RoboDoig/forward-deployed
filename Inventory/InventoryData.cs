using Godot;
using System;

[GlobalClass]
public partial class InventoryData : Resource
{
    [Export]
    public SlotData[] SlotDatas;
    [Signal]
    public delegate void InventoryUpdatedEventHandler(InventoryData inventoryData);

    public SlotData RemoveItemAtIndex(int index)
    {
        var slot = SlotDatas[index];
        SlotDatas[index] = null;

        EmitSignal(SignalName.InventoryUpdated, this);

        return slot;
    }
    //public void OnSlotClicked(int si, int bi)
    //{
    //    var slotData = SlotDatas[si];
    //    if (slotData != null )
    //    {
    //        GD.Print(slotData.ItemData.Operator.GetOperatorName());
    //    };
    //}
}
