using Godot;
using R3;

public interface IOperator
{
    // TODO - this needs to be somewhere else, OK for now to do testing but it shouldn't be the operator responsibility
    public PackedScene GraphNode => ResourceLoader.Load<PackedScene>("res://graph_node.tscn");

    // TODO - this is also not very nice
    public string GetOperatorName();
}