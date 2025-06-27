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

    [Export]
    public PackedScene ItemSlot { get; set; }
    private ItemSlotPanel GrabbedSlot;
    private MultiItemInteractionManager MultiItemInteractionManager;

    public override void _Ready()
    {
        OpenPanelsCount = 0;

        GrabbedSlot = (ItemSlotPanel)ItemSlot.Instantiate();
        AddChild(GrabbedSlot);
        GrabbedSlot.Hide();

        MultiItemInteractionManager = new MultiItemInteractionManager(GrabbedSlot);
        AddChild(MultiItemInteractionManager);
    }

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
            }
            else
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

        if (panel.GetType().IsSubclassOf(typeof(WorldObjectContainerPanel)) || panel.GetType() == typeof(WorldObjectContainerPanel))
        {
            var containerPanel = (WorldObjectContainerPanel)panel;
            MultiItemInteractionManager.AddContainerPanel(containerPanel);
        }

        if (panel.GetType().IsSubclassOf(typeof(WorldObjectOperatorMachinePanel)) || panel.GetType() == typeof(WorldObjectOperatorMachinePanel))
        {
            var operatorMachinePanel = (WorldObjectOperatorMachinePanel)panel;
            MultiItemInteractionManager.AddOperatorMachinePanel(operatorMachinePanel);
        }
    }

    public void SetCrosshairScale(Vector2 scaleVector)
    {
        Crosshair.Scale = scaleVector;
    }
}

public partial class MultiItemInteractionManager : Control
{
    private SlotData CurrentHeldItem
    {
        get;
        set
        {
            field = value;
            EmitSignal(SignalName.CurrentHeldItemChanged, value);
        }
    } = null;
    private ItemSlotPanel ItemDisplaySlot;
    [Signal]
    private delegate void CurrentHeldItemChangedEventHandler(SlotData slotData);

    public MultiItemInteractionManager(ItemSlotPanel displaySlot)
    {
        ItemDisplaySlot = displaySlot;

        CurrentHeldItemChanged += OnCurrentHeldItemChanged;
    }

    void OnCurrentHeldItemChanged(SlotData slotData)
    {
        if (slotData != null)
        {
            ItemDisplaySlot.SetSlotData(slotData);
            ItemDisplaySlot.Show();
            ItemDisplaySlot.ZIndex = 1;
        }
        else
        {
            ItemDisplaySlot.Hide();
        }
    }

    public void AddContainerPanel(WorldObjectContainerPanel container)
    {
        container.SlotClicked += (inv, si, bi) =>
        {
            if (CurrentHeldItem == null)
            {
                CurrentHeldItem = inv.RemoveItemAtIndex(si);
            } else
            {
                CurrentHeldItem = inv.DropItemAtIndex(CurrentHeldItem, si);
            }
        };
    }

    public void AddOperatorMachinePanel(WorldObjectOperatorMachinePanel machine)
    {
        machine.GridClicked += (bi) =>
        {
            if (CurrentHeldItem != null)
            {
                machine.AddOperator(CurrentHeldItem);
                CurrentHeldItem = null;
            }
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        if (ItemDisplaySlot.Visible)
        {
            ItemDisplaySlot.SetGlobalPosition(GetGlobalMousePosition() + new Vector2(5, 5));
        }
    }
}
