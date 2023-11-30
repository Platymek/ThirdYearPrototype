using Godot;
using Godot.Collections;
using System;

public partial class Opponent : Actor
{
	public override string State 
	{ 
		get => base.State; 
		set
		{
			switch (value)
			{
				case "idle":

					if (_idleTimer != null)
					{
						if (_idleTimer.IsStopped())
						{
							_idleTimer.Start();
						}
					}

					break;
			}

			base.State = value;
		}
	}


	// detectes how far the opponent is from each wall

	Node3D _rays;

	RayCast3D _rayBack;
	public float DistanceFromBackWall
	{
		get => _rayBack.IsColliding()
			? (_rayBack.GetCollisionPoint() - _rayBack.GlobalPosition).Length()
			: 0;
	}

	RayCast3D _rayLeft;
	public float DistanceFromLeftWall
	{
		get => _rayLeft.IsColliding()
			? (_rayLeft.GetCollisionPoint() - _rayLeft.GlobalPosition).Length()
			: 0;
	}

	RayCast3D _rayRight;
	public float DistanceFromRightWall
	{
		get => _rayRight.IsColliding()
			? (_rayRight.GetCollisionPoint() - _rayRight.GlobalPosition).Length()
			: 0;
	}

	OpponentAttackStats _opponentAttackStats;

	// modifiers which affect how the opponent will always act for easy tweaking
	OpponentModifiers _modifiers;

	RandomNumberGenerator _attackDecider;

	// when the timer ends, the opponent will attack
	Timer _idleTimer;

	enum AttackTypes
	{
		CloseToCorner,
		FarFromCorner,
		Neutral,
		MixUp,
	}

	Dictionary<AttackTypes, Array<string>> CurrentAttacks;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		_opponentAttackStats = GetNode<OpponentAttackStats>("AttackStats");
		_idleTimer = GetNode<Timer>("IdleTimer");
		_modifiers = GetNode<OpponentModifiers>("Modifiers");

		_rays = GetNode<Node3D>("Rays");
		_rayBack = _rays.GetNode<RayCast3D>("Back");
		_rayLeft = _rays.GetNode<RayCast3D>("Left");
		_rayRight = _rays.GetNode<RayCast3D>("Right");

		_attackDecider = new();
		_attackDecider.Randomize();

		// if the opponent is idle, start the idle timer
		if (State == "idle")
		{
			_idleTimer.Start();
		}

		// initialise attack dictionary
		CurrentAttacks = new()
		{
			{ AttackTypes.CloseToCorner, new Array<string>() },
			{ AttackTypes.FarFromCorner, new Array<string>() },
			{ AttackTypes.Neutral, new Array<string>() },
			{ AttackTypes.MixUp, new Array<string>() },
		};


		// add starting attacks //

		CurrentAttacks[AttackTypes.CloseToCorner].Add("Opponent/SumoPressure");
		CurrentAttacks[AttackTypes.FarFromCorner].Add("Opponent/BigPush");
		CurrentAttacks[AttackTypes.Neutral].Add("Opponent/SumoAdvance");
		CurrentAttacks[AttackTypes.MixUp].Add("Opponent/BigChop");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		if (_target is Player player)
		{
			if (_opponentAttackStats.FollowSpeed > 0 && Position2D.DistanceTo(player.Position2D) > 2 && CurrentKnockback <= 0)
			{
				Move(new Vector3(0, 0, -_opponentAttackStats.FollowSpeed * (float)delta));
			}

			switch (State)
			{
				case "idle":

					// if timer stopped, pick an attack
					if (_idleTimer.IsStopped())
					{
						_attackDecider.Randomize();
						float mixUpResult = _attackDecider.Randf();

						GD.Print(mixUpResult, _modifiers.MixUpChance);

						if (mixUpResult < _modifiers.MixUpChance)
						{
							// choose attack using the same float normalised between 0 and 1
							float mixUpChoice = mixUpResult / _modifiers.MixUpChance;

							int mixUpChoiceIndex = (int)Mathf.Floor(mixUpChoice * CurrentAttacks[AttackTypes.MixUp].Count);

							State = CurrentAttacks[AttackTypes.MixUp][mixUpChoiceIndex];
						}
						else
						{
							float attackChoice = mixUpResult / (1 - _modifiers.MixUpChance);

							// choose type of attack based on 
							AttackTypes attackType

								// got the player close to the wall behind them?
								= player.DistanceFromBackWall < _modifiers.CornerThreshold
								? AttackTypes.CloseToCorner

								// is close to the wall behind themselves?
								: (_rayBack.GetCollisionPoint() - _rayBack.GlobalPosition).Length() < _modifiers.CornerThreshold
								? AttackTypes.FarFromCorner

								// otherwise, just perform a neutral attack
								: AttackTypes.Neutral;


							GD.Print(attackType);

							int attackIndex = (int)Mathf.Floor(attackChoice * CurrentAttacks[attackType].Count);

							State = CurrentAttacks[attackType][attackIndex];
						}
					}
					// else, turn around player to get better angle against wall
					else
					{
						if (DistanceFromLeftWall - _modifiers.RotationAcceptanceThreshold < DistanceFromRightWall)
						{
							Move(new(1f * (float)delta, 0, 0));
						}
						else if (DistanceFromRightWall - _modifiers.RotationAcceptanceThreshold < DistanceFromLeftWall)
						{
							Move(new(-1f * (float)delta, 0, 0));
						}
					}


					break;
			}
		}
	}
}
