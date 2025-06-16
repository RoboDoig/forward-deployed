using Godot;
using System;
using System.Collections.Generic;

public partial class UiManager : CanvasLayer
{
    [Signal]
    public delegate void RequestUiControlsEventHandler();

    [Signal]
    public delegate void RequestReleaseUiControlsEventHandler();
    [Signal]
    private delegate void OpenPanelsCountChangedEventHandler();

    private bool CanCreatePanels = true;

    [Export]
    private TextureRect Crosshair;

    private int OpenPanelsCount
    {
        get;
        set
        {
            field = value;
            EmitSignal(SignalName.OpenPanelsCountChanged);

            if (field > 0)
            {
                EmitSignal(SignalName.RequestUiControls);
            } else
            {
                EmitSignal(SignalName.RequestReleaseUiControls);
            }
        }
    } = 0;

    public void CreateWorldObjectPanel(WorldObject worldObject)
    {
        var panel = worldObject.CreateInterfacePanel(this);
        AddChild(panel);

        OpenPanelsCount++;

        panel.CloseInitiated += () =>
        {
            OpenPanelsCount--;
        };
    }

    public void SetCrosshairScale(Vector2 scaleVector)
    {
        Crosshair.Scale = scaleVector;
    }
}
