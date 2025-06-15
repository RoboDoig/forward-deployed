using Godot;
using System;

public partial class WorldObjectPanel : Control
{
    [Export]
    private Button CloseButton { get; set; }

    [Export]
    public Label Title { get; private set; }

    [Signal]
    public delegate void CloseInitiatedEventHandler();

    private bool beingMoved;
    private Vector2 moveStartPosition;

    public override void _Ready()
    {
        Title.GuiInput += (e) =>
        {
            if (e is InputEventMouseButton inputEventMouseButton && inputEventMouseButton.ButtonIndex == MouseButton.Right)
            {
                if (inputEventMouseButton.IsPressed())
                {
                    moveStartPosition = inputEventMouseButton.Position;
                    beingMoved = true;
                } else if (inputEventMouseButton.IsReleased())
                {
                    beingMoved = false;
                }
            }
        };

        CloseButton.Pressed += () =>
        {
            EmitSignal(SignalName.CloseInitiated);
            QueueFree();
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        if (beingMoved)
        {
            GlobalPosition = GetGlobalMousePosition() - moveStartPosition;
        }
    }
}
