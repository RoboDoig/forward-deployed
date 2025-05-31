using Godot;
using System;

public partial class UiManager : CanvasLayer
{
    [Signal]
    public delegate void RequestUiControlsEventHandler();

    [Signal]
    public delegate void RequestReleaseUiControlsEventHandler();

    private bool CanCreatePanels = true;

    public void CreateWorldObjectPanel(WorldObject worldObject)
    {
        if (CanCreatePanels)
        {
            CanCreatePanels = false;
            var panel = (WorldObjectPanel)worldObject.InterfacePanel.Instantiate();
            AddChild(panel);
            panel.Title.Text = worldObject.ObjectName;

            EmitSignal(SignalName.RequestUiControls);

            panel.CloseButton.Pressed += () =>
            {
                panel.QueueFree();

                EmitSignal(SignalName.RequestReleaseUiControls);

                CanCreatePanels = true;
            };
        }
    }
}
