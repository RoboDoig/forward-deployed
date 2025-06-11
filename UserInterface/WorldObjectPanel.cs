using Godot;
using System;

public partial class WorldObjectPanel : Control
{
    [Export]
    public Button CloseButton { get; private set; }

    [Export]
    public Label Title { get; private set; }

    public override void _Ready()
    {
    }
}
