using Godot;
using System;

public partial class ItemSlotPanel : PanelContainer
{
    [Export]
    Label ItemNameLabel;
    [Export]
    TextureRect ItemIcon;
    [Signal]
    public delegate void SlotClickedEventHandler(int slotIndex, int buttonIndex);

    public void SetSlotData(SlotData slotData)
    {
        if (slotData != null)
        {
            //ItemNameLabel.Text = slotData.ItemData.Operator.GetOperatorName();
            ItemNameLabel.Text = "OP";
            TooltipText = slotData.ItemData.Operator.GetOperatorName();
            ItemIcon.Texture = slotData.ItemData.Icon;
        }        
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton inputEventMouseButton && @event.IsPressed() && inputEventMouseButton.ButtonIndex == MouseButton.Left)
        {
            var buttonEvent = (InputEventMouseButton)@event;
            EmitSignal(SignalName.SlotClicked, GetIndex(), (int)buttonEvent.ButtonIndex);
        }
    }
}
