using Godot;
using System;

public partial class WorldObjectContainerPanel : WorldObjectPanel
{
    [Export]
    public GridContainer GridContainer { get; set; }

    [Export]
    public PackedScene ItemSlot { get; set; }

    private ItemSlotPanel GrabbedSlot;

    public override void _Ready()
    {
        GrabbedSlot = (ItemSlotPanel)ItemSlot.Instantiate();
        AddChild(GrabbedSlot);
        GrabbedSlot.Hide();
    }

    public void InitialiseContainerPanelFromInventoryData(InventoryData inventoryData)
    {
        inventoryData.InventoryUpdated += OnInventoryUpdated;
        PopulateItemGrid(inventoryData);

        TreeExited += () =>
        {
            inventoryData.InventoryUpdated -= OnInventoryUpdated;
        };
    }

    private void OnInventoryUpdated(InventoryData inventoryData)
    {
        PopulateItemGrid(inventoryData);
    }

    void PopulateItemGrid(InventoryData inventoryData)
    {
        foreach (var child in GridContainer.GetChildren())
        {
            child.QueueFree();
        }

        foreach (var slotData in inventoryData.SlotDatas)
        {
            var itemSlot = (ItemSlotPanel)ItemSlot.Instantiate();
            itemSlot.SetSlotData(slotData);

            itemSlot.SlotClicked += (si, bi) =>
            {
                if (slotData != null)
                {
                    GrabbedSlot.SetSlotData(inventoryData.SlotDatas[si]);
                    GrabbedSlot.Show();
                    inventoryData.RemoveItemAtIndex(si);
                }
                else
                {
                    GrabbedSlot.Hide();
                }
            };

            GridContainer.AddChild(itemSlot);
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
