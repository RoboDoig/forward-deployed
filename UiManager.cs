using Godot;
using System;

public partial class UiManager : CanvasLayer
{
    [Signal]
    public delegate void RequestUiControlsEventHandler();

    [Signal]
    public delegate void RequestReleaseUiControlsEventHandler();

    private bool CanCreatePanels = true;

    [Export]
    private TextureRect Crosshair;

    public void CreateWorldObjectPanel(WorldObject worldObject)
    {
        if (CanCreatePanels)
        {
            CanCreatePanels = false;
            var panel = worldObject.CreateInterfacePanel(this);
            AddChild(panel);

            EmitSignal(SignalName.RequestUiControls);

            panel.CloseButton.Pressed += () =>
            {
                panel.QueueFree();

                EmitSignal(SignalName.RequestReleaseUiControls);

                CanCreatePanels = true;
            };
        }
    }

    public void SetCrosshairScale(Vector2 scaleVector)
    {
        Crosshair.Scale = scaleVector;
    }
}
