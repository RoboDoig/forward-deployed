using Godot;
using System;
using System.Reflection.Metadata;

public partial class WorldObjectContainer : WorldObject
{
    public override PackedScene InterfacePanel => ResourceLoader.Load<PackedScene>("res://UserInterface/world_object_container_panel.tscn");

    [Export]
    public InventoryData InventoryData;

    public override void _Ready()
    {

    }

    public override WorldObjectPanel CreateInterfacePanel(UiManager uiManager)
    {
        var panel = (WorldObjectContainerPanel)InterfacePanel.Instantiate();
        panel.Title.Text = ObjectName;

        panel.InitialiseConteinerPanelFromInventoryData(InventoryData);

        return panel;
    }
}
