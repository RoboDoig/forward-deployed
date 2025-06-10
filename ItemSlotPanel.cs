using Godot;
using System;

public partial class ItemSlotPanel : PanelContainer
{
    [Export]
    public Label NameLabel;

    [Signal]
    public delegate void SlotClickedEventHandler(int slotIndex, int buttonIndex);

    public void SetSlotData(SlotData slotData)
    {
        if (slotData != null)
        {
            NameLabel.Text = slotData.ItemData.Name;
            TooltipText = $"{slotData.ItemData.Name}: {slotData.ItemData.Description}";
        }
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton && @event.IsPressed())
        {
            var buttonEvent = (InputEventMouseButton) @event;
            EmitSignal(SignalName.SlotClicked, GetIndex(), (int)buttonEvent.ButtonIndex);
        }
    }
}
