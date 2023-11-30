using Godot;
using MonoCustomResourceRegistry;
using System;

[RegisteredTypeAttribute(nameof(Actor), baseType = "Node3D")]
public partial class Actor : Node3D
{
	// Properties //

	[Export]
	protected Actor _target;

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
			if (_state != value)
			{
				_state = value;

				if (_animationPlayer != null)
				{
					_animationPlayer.Play(value);
				}

				GD.Print($"State: {value}");
			}
		}
	}

	public Vector2 Position2D
	{
		get
		{
			Vector2 position = new Vector2(
				Position.X,
				Position.Z);

			return position;
		}

		set
		{
			Vector3 position = new Vector3(
				value.X,
				Position.Y,
				value.Y);

			Position = position;
		}
	}

	AnimationPlayer _animationPlayer;
	private AttackStats _attackStats;

	public float Health;
	public float CurrentKnockback;

	public float HealthPercentage
	{
		get => Health / _stats.MaxHealth;
	}

	// if true, disable JustHit, else make true
	bool _justJustHit;
	public bool JustHit;

	float _previousDamage;
	float _previousKnockback;


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

		_justJustHit = false;
		JustHit = false;

		State = _state;
		_animationPlayer.Play(_state);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		// check hurtbox if set to
		if (_attackStats.CheckDamage && _attackStats.Hurtbox != null)
		{
			CheckDamage();
		}

		// process knockback
		if (CurrentKnockback != 0)
		{
			ProcessKnockback((float)delta);
		}
		// do not attempt to track if the actor has no trackspeed
		else if (_attackStats.TrackSpeed != 0)
		{
			TrackTarget((float)delta);
		}

		if (!_justJustHit)
		{
			_justJustHit = true;
			JustHit = true;
		}
		else
		{
			JustHit = false;
		}
	}


	// Functions //

	void CheckDamage()
	{
		GD.Print($"{Name} Attacked with hurtbox {_attackStats.Hurtbox}! And hit...");

		// check every overlapping area
		foreach (Area3D box in GetNode<Area3D>(_attackStats.Hurtbox).GetOverlappingAreas())
		{
			Node victim = box.GetOwner<Node>();

			if (victim is Actor actor)
			{
				if (actor.Name != Name)
				{
					actor.Hurt(
						_stats.Damage * _attackStats.Damage,
						_stats.Knockback * _attackStats.Knockback);
				}
			}
		}

		_attackStats.CheckDamage = false;
	}

	void TrackTarget(float delta)
	{
		// get angle difference (the smallest angle to the target)

		float angleToTarget = new Vector2(-Position.Z, Position.X)
			.AngleToPoint(new Vector2(-_target.Position.Z, _target.Position.X))
			* -1;

		float angleDifference = Mathf.AngleDifference(Rotation.Y, angleToTarget);


		// if the difference is 0, no rotation is needed

		if (angleDifference != 0)
		{
			// rotate character

			float deltaAngle = Mathf.Tau * delta * _attackStats.TrackSpeed * _stats.TrackSpeed;

			Vector3 newRotation = Rotation;

			newRotation.Y += angleDifference < 0 
				? -deltaAngle
				: deltaAngle;

			Rotation = newRotation;


			// get new angle difference

			float newAngleToTarget = new Vector2(-Position.Z, Position.X)
				.AngleToPoint(new Vector2(-_target.Position.Z, _target.Position.X))
				* -1;

			float newAngleDifference = Mathf.AngleDifference(Rotation.Y, newAngleToTarget);


			// if the sign has changed, it means that the character has faced the target
			// and point the character to the target accordingly

			if ((angleDifference < 0
				&& newAngleDifference > 0)
				|| (angleDifference > 0
				&& newAngleDifference < 0))
			{
				Rotation = new Vector3(
					Rotation.X,
					newAngleToTarget,
					Rotation.Z);
			}
		}

	}

	void Hurt(float damage, float knockback)
	{
		float finalDamage = damage * _attackStats.Defense;
		float finalKnockback = knockback * _attackStats.Defense;

		Health -= finalDamage;
		CurrentKnockback = finalKnockback;
		_justJustHit = false;

		_previousDamage = damage;
		_previousKnockback = knockback;

		GD.Print($"{Name} was hit for {_attackStats.Defense} damage and {finalKnockback} knockback");
	}

	// move on local rotation
	protected void Move(Vector3 velocity)
	{
		// get velocity
		Position += (velocity * _stats.Speed).Rotated(Vector3.Up, Rotation.Y);
	}

	void ProcessKnockback(float delta)
	{
		Move(new Vector3(0, 0, CurrentKnockback * delta * _stats.KnockbackSpeed));

		bool positiveKnockback = CurrentKnockback > 0;

        CurrentKnockback -= delta * _stats.KnockbackDecceleration
			* (positiveKnockback ? 1 : -1);

		bool newPositiveKnockback = CurrentKnockback > 0;

		if (positiveKnockback != newPositiveKnockback)
		{
			CurrentKnockback = 0;
		}
    }

	public void WallBounce()
	{
		if (CurrentKnockback > 0)
		{
			Hurt(_previousDamage, -_previousKnockback);
		}
		else if (CurrentKnockback < 0)
        {
            Hurt(_previousDamage, 0);
        }
	}
}
