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

	[Export]
	public float Defense = 1;

    NodePath _hurtbox;

	[Export]
    public NodePath Hurtbox
	{
		get 
		{
			NodePath n = _hurtbox == null || _hurtbox.ToString().Length < 3
                ? null
				: new NodePath(_hurtbox.ToString().Substring(3));

			return n;
		}
		set
		{
			_hurtbox = value;
		}
	}
}
