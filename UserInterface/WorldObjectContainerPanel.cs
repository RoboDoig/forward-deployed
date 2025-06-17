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

    //private ItemSlotPanel GrabbedSlot;
    //private SlotData CurrentHeldSlot
    //{
    //    get; 
    //    set
    //    {
    //        field = value;
    //        EmitSignal(SignalName.CurrentHeldSlotChanged, value);
    //    }
    //}

    public override void _Ready()
    {
        base._Ready();

        //GrabbedSlot = (ItemSlotPanel)ItemSlot.Instantiate();
        //AddChild(GrabbedSlot);
        //GrabbedSlot.Hide();

        //CurrentHeldSlotChanged += OnCurrentHeldSlotChanged;
    }

    //private void OnCurrentHeldSlotChanged(SlotData slotData)
    //{
    //    if (slotData != null)
    //    {
    //        GrabbedSlot.SetSlotData(slotData);
    //        GrabbedSlot.Show();
    //    } else
    //    {
    //        GrabbedSlot.Hide();
    //    }
    //}

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

    //void OnSlotClicked(InventoryData inventoryData, int slotIndex, int buttonIndex)
    //{
    //    if (CurrentHeldSlot == null)
    //    {
    //        CurrentHeldSlot = inventoryData.RemoveItemAtIndex(slotIndex);
    //    } else
    //    {
    //        CurrentHeldSlot = inventoryData.DropItemAtIndex(CurrentHeldSlot, slotIndex);
    //    }
    //}

    //public override void _PhysicsProcess(double delta)
    //{
    //    base._PhysicsProcess(delta);
    //    if (GrabbedSlot.Visible)
    //    {
    //        GrabbedSlot.SetGlobalPosition(GetGlobalMousePosition() + new Vector2(5, 5));
    //    }
    //}
}
