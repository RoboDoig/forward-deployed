using Godot;
using System;

[GlobalClass]
public partial class ItemData : Resource
{
    [Export]
    public string Name;
    [Export(PropertyHint.MultilineText)]
    public string Description;
    [Export]
    public bool Stackable = false;
    [Export]
    public Texture Texture;
    [Export]
    public string OperatorType; // TODO - this is obviously horrible

    public IOperator CreateOperatorFromItem()
    {
        return (IOperator)Activator.CreateInstance(Type.GetType(OperatorType)); // TODO - this is going to break... badly.
    }
}
