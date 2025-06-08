using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[GlobalClass]
public partial class SlotData : Resource
{
    const int MAX_STACK_SIZE = 99;

    [Export]
    public ItemData ItemData;
    [Export]
    public int Quantity;
}

