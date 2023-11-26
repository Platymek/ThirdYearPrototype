using Godot;
using System;

public partial class PlayerAttackStats : AttackStats
{
    [Export]
    public bool StateCancellable = true;

    [Export]
    public int ChargePunchState = 0;

    [Export]
    public float StopDistance = 1;
}
