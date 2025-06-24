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

    private int openPanelsCount;
    private int OpenPanelsCount
    {
        get { return openPanelsCount;}
        set
        {
            openPanelsCount = value;
            EmitSignal(SignalName.OpenPanelsCountChanged);

            if (openPanelsCount > 0)
            {
                EmitSignal(SignalName.RequestUiControls);
            } else
            {
                EmitSignal(SignalName.RequestReleaseUiControls);
            }
        }
    }

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
            //MultiContainerManager.AddContainerPanel((WorldObjectContainerPanel)panel);
            var containerPanel = (WorldObjectContainerPanel)panel;

            //containerPanel.SlotClicked += (iv, si, bi) => { GD.Print("Clicked an inventory slot."); };

            MultiItemInteractionManager.AddContainerPanel(containerPanel);
        }

        if (panel.GetType().IsSubclassOf(typeof(WorldObjectOperatorMachinePanel)) || panel.GetType() == typeof(WorldObjectOperatorMachinePanel))
        {
            var operatorMachinePanel = (WorldObjectOperatorMachinePanel)panel;

            //operatorMachinePanel.GridClicked += (bi) => { GD.Print("Clicked an operator grid"); };
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
    private SlotData currentHeldItem;
    private SlotData CurrentHeldItem
    {
        get { return currentHeldItem; }
        set
        {
            currentHeldItem = value;
            EmitSignal(SignalName.CurrentHeldItemChanged, value);
        }
    }
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

public partial class MultiContainerManager : Control
{
    private Tuple<InventoryData, SlotData> currentHeldItem;
    private Tuple<InventoryData, SlotData> CurrentHeldItem
    {
        get { return currentHeldItem; }
        set
        {
            currentHeldItem = value;
            EmitSignal(SignalName.CurrentHeldItemChanged, value.Item2);
        }
    }
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
