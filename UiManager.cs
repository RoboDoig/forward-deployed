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
    private MultiContainerManager MultiContainerManager;

    public override void _Ready()
    {
        GrabbedSlot = (ItemSlotPanel)ItemSlot.Instantiate();
        AddChild(GrabbedSlot);
        GrabbedSlot.Hide();

        MultiContainerManager = new MultiContainerManager(GrabbedSlot);
        AddChild(MultiContainerManager);
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

        if (panel.GetType().IsSubclassOf(typeof(WorldObjectContainerPanel)) || panel.GetType() == typeof(WorldObjectContainerPanel))
        {
            GD.Print("adding panel");
            MultiContainerManager.AddContainerPanel((WorldObjectContainerPanel)panel);
        }
    }

    public void SetCrosshairScale(Vector2 scaleVector)
    {
        Crosshair.Scale = scaleVector;
    }
}

public partial class MultiContainerManager : Control
{
    private Tuple<InventoryData, SlotData> CurrentHeldItem
    {
        get;
        set
        {
            field = value;
            EmitSignal(SignalName.CurrentHeldItemChanged, value.Item2);
        }
    } = null;
    private ItemSlotPanel ItemDisplaySlot;
    [Signal]
    private delegate void CurrentHeldItemChangedEventHandler(SlotData slotData);

    public MultiContainerManager(ItemSlotPanel displaySlot)
    {
        ItemDisplaySlot = displaySlot;

        CurrentHeldItemChanged += OnCurrentHeldItemChanged;
    }

    public void AddContainerPanel(WorldObjectContainerPanel container)
    {
        container.SlotClicked += (inv, si, bi) =>
        {
            OnSlotClicked(inv, si, bi);
        };
    }

    void OnSlotClicked(InventoryData inventoryData, int slotIndex, int buttonIndex)
    {
        if (CurrentHeldItem == null)
        {
            CurrentHeldItem = new Tuple<InventoryData, SlotData>(inventoryData, inventoryData.RemoveItemAtIndex(slotIndex));
        } else
        {
            CurrentHeldItem = new Tuple<InventoryData, SlotData>(inventoryData, inventoryData.DropItemAtIndex(CurrentHeldItem.Item2, slotIndex));
        }
    }

    void OnCurrentHeldItemChanged(SlotData slotData)
    {
        if (slotData != null)
        {
            ItemDisplaySlot.SetSlotData(slotData);
            ItemDisplaySlot.Show();
        } else
        {
            ItemDisplaySlot.Hide();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (ItemDisplaySlot.Visible)
        {
            ItemDisplaySlot.SetGlobalPosition(GetGlobalMousePosition() + new Vector2(5, 5));
        }
    }
}
