using Godot;
using System;

public partial class AttackStats : Node
{
	[Export]
	public float Damage;

	[Export]
    public float Knockback;

	[Export]
    public bool CheckDamage = false;

    [Export]
    public float TrackSpeed = 1;

    NodePath _hurtbox;

	[Export]
    public NodePath Hurtbox
	{
		get 
		{ 
			return new NodePath(_hurtbox.ToString().Substring(3));
		}
		set
		{
			_hurtbox = value;
		}
	}
}
