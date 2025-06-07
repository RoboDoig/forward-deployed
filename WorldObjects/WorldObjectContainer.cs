using Godot;
using System;
using System.Reflection.Metadata;

public partial class WorldObjectContainer : WorldObject
{
    public override PackedScene InterfacePanel => ResourceLoader.Load<PackedScene>("res://world_object_container_panel.tscn");

    public WorldItemContainer WorldItemContainer;

    public override WorldObjectPanel CreateInterfacePanel(UiManager uiManager)
    {
        var panel = (WorldObjectContainerPanel)InterfacePanel.Instantiate();
        panel.Title.Text = ObjectName;

        return panel;
    }
}
