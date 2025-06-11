using Godot;
using System;

[GlobalClass]
public partial class InventoryData : Resource
{
    [Export]
    public SlotData[] SlotDatas;

    //public void OnSlotClicked(int si, int bi)
    //{
    //    var slotData = SlotDatas[si];
    //    if (slotData != null )
    //    {
    //        GD.Print(slotData.ItemData.Operator.GetOperatorName());
    //    };
    //}
}
