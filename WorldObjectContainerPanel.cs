using Godot;
using System;

public partial class WorldObjectContainerPanel : WorldObjectPanel
{
    [Export]
    public GridContainer GridContainer { get; set; }

    [Export]
    public PackedScene ItemSlot { get; set; }

    [Export]
    ItemSlotPanel GrabbedSlot;

    public SlotData GrabbedSlotData;
    public void InitialiseContainerPanelFromContainer(WorldItemContainer container)
    {
        for (int i = 0; i < container.ItemCount; i++)
        {
            var slot = (ItemSlotPanel)ItemSlot.Instantiate();

            GridContainer.AddChild(slot);
        }
    }

    public void InitialiseContainerPanelFromInventoryData(InventoryData inventoryData)
    {
        inventoryData.InventoryInteraction += OnInventoryInteract;
        TreeExited += () => inventoryData.InventoryInteraction -= OnInventoryInteract;

        foreach (var slotData in inventoryData.SlotDatas)
        {
            var slot = (ItemSlotPanel)ItemSlot.Instantiate();
            slot.SetSlotData(slotData);

            slot.SlotClicked += (si, bi) =>
            {
                inventoryData.OnSlotClicked(si, bi);
            };

            GridContainer.AddChild(slot);
        }
    }

    void OnInventoryInteract(InventoryData inventoryData, int slotIndex, int buttonIndex)
    {
        GrabbedSlotData = inventoryData.SlotDatas[slotIndex];

        if (GrabbedSlotData != null)
        {
            GrabbedSlot.Show();
            GrabbedSlot.SetSlotData(GrabbedSlotData);
        }
        else
        {
            GrabbedSlot.Hide();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (GrabbedSlot.Visible)
        {
            GrabbedSlot.SetGlobalPosition(GetGlobalMousePosition() + new Vector2(5, 5));
        }
    }
}
