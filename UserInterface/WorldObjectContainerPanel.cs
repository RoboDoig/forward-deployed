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

    public void InitialiseConteinerPanelFromInventoryData(InventoryData inventoryData)
    {
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
                    GD.Print(slotData.ItemData.Operator.GetOperatorName());
                } else
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
