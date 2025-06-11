using Godot;
using System;

public partial class WorldObjectContainerPanel : WorldObjectPanel
{
    [Export]
    public GridContainer GridContainer { get; set; }

    [Export]
    public PackedScene ItemSlot { get; set; }

    public void InitialiseContainerPanelFromContainer(WorldItemContainer container)
    {
        for (int i = 0; i < container.ItemCount; i++)
        {
            var slot = ItemSlot.Instantiate();

            GridContainer.AddChild(slot);
        }
    }
}
