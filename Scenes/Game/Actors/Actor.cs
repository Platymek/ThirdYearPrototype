using Godot;
using MonoCustomResourceRegistry;
using System;

[RegisteredTypeAttribute(nameof(Actor), baseType = "Node3D")]
public partial class Actor : Node3D
{
	// Properties //

	[Export]
	Actor _target;

	[Export]
	ActorStats _stats;

	string _state;

	[Export]
	public virtual string State
	{
		get
		{
			return _state;
		}

		set
		{
			_state = value;
			_animationPlayer.Play(value);
		}
	}

	AnimationPlayer _animationPlayer;
	AttackStats _attackStats;

	public float Health;

	public float CurrentKnockback;


	// Node functions // 

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		// get nodes
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_attackStats = GetNode<AttackStats>("AttackStats");

		// initialise variables
		Health = _stats.MaxHealth;
		CurrentKnockback = 0;

		GD.Print("hello");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		// check hurtbox if set to
		if (_attackStats.CheckDamage)
		{
			GD.Print($"{Name} Attacked with hurtbox {_attackStats.Hurtbox}! And hit...");

			foreach (Area3D box in GetNode<Area3D>(_attackStats.Hurtbox).GetOverlappingAreas())
			{
				Node victim = box.GetOwner<Node>();

				GD.Print($"{victim.Name} on layer {box.CollisionLayer}...");

				if (victim is Actor actor)
				{
					actor.Hurt(
						_stats.Damage * _attackStats.Damage, 
						_stats.Knockback * _attackStats.Knockback);
				}
			}

			_attackStats.CheckDamage = false;
		}


		if (CurrentKnockback > 0)
		{
			ProcessKnockback((float)delta);
		}
	}


	// Functions //

	void Hurt(float damage, float knockback)
	{
		Health -= damage;
		CurrentKnockback += knockback;
	}

	protected void Move(Vector3 velocity)
	{
		// get velocity
		Position += velocity.Rotated(Vector3.Up, Rotation.Y);
	}

	void ProcessKnockback(float delta)
	{
		Move(new Vector3(0, 0, CurrentKnockback * delta * _stats.KnockbackSpeed));

		CurrentKnockback -= delta * _stats.KnockbackDecceleration;
	}
}
