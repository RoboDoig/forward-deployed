using Godot;
using System;

public partial class ItemSlotPanel : PanelContainer
{
    [Export]
    Label ItemNameLabel;

    public void SetSlotData(SlotData slotData)
    {
        if (slotData != null)
        {
            //ItemNameLabel.Text = slotData.ItemData.Operator.GetOperatorName();
            ItemNameLabel.Text = "OP";
            TooltipText = slotData.ItemData.Operator.GetOperatorName();
        }        
    }
}
