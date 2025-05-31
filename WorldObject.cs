using Godot;
using System;

public partial class WorldObject : StaticBody3D
{
    [Export]
    public string ObjectName = "WorldObject";

    [Export]
    public string Description = "Object description.";

    public readonly PackedScene InterfacePanel = ResourceLoader.Load<PackedScene>("res://world_object_panel.tscn");
}
