using Godot;
using System;

public partial class OpponentModifiers : Node
{
    [Export]
    public float MixUpChance = 0.5f;

    [Export]
    public float CornerThreshold = 4f;
}
