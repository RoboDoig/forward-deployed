using Godot;
using System;

public partial class WorldObjectContainerPanel : WorldObjectPanel
{
    [Export]
    public GridContainer GridContainer { get; set; }

    [Export]
    public PackedScene ItemSlot { get; set; }

    public void InitialiseConteinerPanelFromInventoryData(InventoryData inventoryData)
    {
        foreach (var slotData in inventoryData.SlotDatas)
        {
            var itemSlot = (ItemSlotPanel)ItemSlot.Instantiate();

            itemSlot.SetSlotData(slotData);

            GridContainer.AddChild(itemSlot);
        }
    }
}
