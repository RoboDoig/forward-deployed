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

    private MultiContainerManager MultiContainerManager = new MultiContainerManager();

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

        if (panel.GetType().IsSubclassOf(typeof(WorldObjectContainerPanel)))
        {
            MultiContainerManager.AddContainerPanel((WorldObjectContainerPanel)panel);
        }

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

public partial class MultiContainerManager : Node3D
{
    [Signal]
    private delegate void CurrentHeldSlotChangedEventHandler(SlotData slotData);
    private List<WorldObjectContainerPanel> ContainerPanels;
    private ItemSlotPanel GrabbedSlot;
    private SlotData CurrentHeldSlot
    {
        get;
        set
        {
            field = value;
            EmitSignal(SignalName.CurrentHeldSlotChanged, value);
        }
    }

    public void AddContainerPanel(WorldObjectContainerPanel panel)
    {
        ContainerPanels.Add(panel);
    }

    public void RemoveContainerPanel(WorldObjectContainerPanel panel)
    {
        ContainerPanels.Remove(panel);
    }
}
