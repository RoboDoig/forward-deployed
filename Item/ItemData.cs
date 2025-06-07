using Godot;
using System;

[GlobalClass]
public partial class ItemData : Resource
{
    [Export]
    public string Name;
    [Export]
    public string Description;
    [Export]
    public bool Stackable = false;
    [Export]
    public Texture Texture;
}
