using Godot;
using System;

public partial class WorldObject : StaticBody3D
{
    [Export]
    public string ObjectName = "WorldObject";

    [Export]
    public string Description = "Object description.";

    public virtual PackedScene InterfacePanel => ResourceLoader.Load<PackedScene>("res://world_object_panel.tscn");

    public virtual WorldObjectPanel CreateInterfacePanel(UiManager uiManager)
    {
        var panel = (WorldObjectPanel)InterfacePanel.Instantiate();
        panel.Title.Text = ObjectName;

        return panel;
    }
}
