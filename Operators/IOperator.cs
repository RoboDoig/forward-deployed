using Godot;
using R3;
using System;
using static Godot.HttpRequest;

public interface IOperator
{
    // TODO - this is also not very nice
    public string GetOperatorName();

    // NOTE - above two things relate to making graph nodes for editing. Operators should probably just have a reference (e.g. and attribute) that says how they want to be visualized. UI should then be responsible for finding the correct graph node visualizer

    public Type GetSourceType();
    public Type GetResultType();

    public GraphNodeOperator CreateGraphNode();
}