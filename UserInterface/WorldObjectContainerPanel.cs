using Godot;
using System;

public partial class WorldObjectContainerPanel : WorldObjectPanel
{
    [Export]
    public GridContainer GridContainer { get; set; }

    [Export]
    public PackedScene ItemSlot { get; set; }
    [Signal]
    public delegate void SlotClickedEventHandler(InventoryData inventoryData, int slotIndex, int buttonIndex);

    public override void _Ready()
    {
        base._Ready();
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
                EmitSignal(SignalName.SlotClicked, inventoryData, si, bi);
            };

            GridContainer.AddChild(itemSlot);
        }
    }
}
