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
