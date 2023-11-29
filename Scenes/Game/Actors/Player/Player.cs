using Godot;
using System;

public partial class Player : Actor
{
	PlayerAttackStats _playerAttackStats;

	Node3D _rays;
	RayCast3D _rayBack;

	public float DistanceFromBackWall
	{
		get => _rayBack.IsColliding()
			? (_rayBack.GetCollisionPoint() - _rayBack.GlobalPosition).Length()
			: 0;
	}

	public override string State 
	{ 
		get => base.State;
		set
		{
			base.State = value;

			if (_playerAttackStats != null)
			{
				if (_playerAttackStats.StateCancellable)
				{
					_playerAttackStats.TrackSpeed = 1;
				}
			}
		}
	}

	Timer _dodgeTimer;


	// Node Functions //

	public override void _Ready()
	{
		base._Ready();

		_dodgeTimer = GetNode<Timer>("DodgeTimer");
		_playerAttackStats = GetNode<PlayerAttackStats>("AttackStats");

		_rays = GetNode<Node3D>("Rays");
		_rayBack = _rays.GetNode<RayCast3D>("Back");
	} 

	public override void _Process(double delta)
	{
		base._Process(delta);

		// actions possible when the player can cancel their state
		if (_playerAttackStats.StateCancellable)
		{
			// when left is pressed, dodge left
			if (Input.IsActionPressed("ui_left"))
			{
				Dodge(true);
			}
			// when right is pressed, dodge right
			else if (Input.IsActionPressed("ui_right"))
			{
				Dodge(false);
			}
			// when up is pressed, start walking forwards
			else if (Input.IsActionPressed("ui_up"))
			{
				WalkForward();
			}
			// when down is pressed, start blocking
			else if (Input.IsActionJustPressed("ui_down"))
			{
				BlockStart();
			}
		}

		// code for each state
		switch (State)
		{
			case "Player/WalkForward":

				// if no withing a certain range, walk towards opponent
				if (Position2D.DistanceTo(_target.Position2D) > _playerAttackStats.StopDistance)
				{
					Move(new Vector3(0, 0, -4f * (float)delta));

					if (Input.IsActionJustReleased("ui_up"))
					{
						Idle();
					}
				}
				// if within range, begin charging punch
				else
				{
					PunchCharge();
				}

				break;


			// plays a different looping animation for heavy charge
			case "Player/PunchCharge":
			case "Player/PunchChargeFull":

				// when the player releases the button, they attack and stop tracking
				if (!Input.IsActionPressed("ui_up") && _playerAttackStats.CanPunch)
				{
					_playerAttackStats.TrackSpeed = 0;

					// charge of punch affects type of punch
					// (which is modified by animation player)
					switch (_playerAttackStats.ChargePunchState)
					{
						case 2:
							PunchHeavy(); break;
						case 1:
							PunchMedium(); break;
						default:
							PunchLight(); break;
					}
				}

				break;


			// dodge directions use the same function with a different parameter
			case "Player/DodgeLeft":
				ProcessDodge((float)delta, true); break;
			case "Player/DodgeRight":
				ProcessDodge((float)delta, false); break;


			// block attacks (defence in AnimationPlayer)
			case "Player/BlockStart":

				if (JustHit)
				{
					Block();
				}

				if (!Input.IsActionPressed("ui_down"))
				{
					BlockStop();
				}

				break;


			// block attacks (defence in AnimationPlayer)
			case "Player/Block":

				if (!Input.IsActionPressed("ui_down"))
				{
					BlockStop();
				}

				break;
		}
	}


	// Functions //

	void Idle()
	{
		State = "idle";

		_playerAttackStats.StateCancellable = true;
	}

	void WalkForward()
	{
		State = "Player/WalkForward";
		_playerAttackStats.StateCancellable = true;
	}

	void PunchCharge()
	{
		State = "Player/PunchCharge";
		_playerAttackStats.StateCancellable = false;
	}

	void PunchLight()
	{
		State = "Player/PunchLight";
		_playerAttackStats.StateCancellable = false;
	}

	void PunchMedium()
	{
		State = "Player/PunchMedium";
		_playerAttackStats.StateCancellable = false;
	}

	void PunchHeavy()
	{
		State = "Player/PunchHeavy";
		_playerAttackStats.StateCancellable = false;
	}

	void Dodge(bool left)
	{
		State = left
			? "Player/DodgeLeft"
			: "Player/DodgeRight";

		_playerAttackStats.StateCancellable = false;
		_dodgeTimer.Start();
	}

	void BlockStart()
	{
		State = "Player/BlockStart";

		_playerAttackStats.Defense = 0;
	}

	void Block()
	{
		State = "Player/Block";

		_playerAttackStats.Defense = 0.3f;
	}

	void BlockStop()
	{
		Idle();

		State = "Player/BlockStop";
	}

	void ProcessDodge(float delta, bool left)
	{
		// if dodge ended, change state
		if (_dodgeTimer.IsStopped())
		{
			State = "idle";
		}
		// otherwise, move left or right
		else
		{
			Vector3 velocity = new Vector3(
				delta 
					* 6
					// decrease speed towards end of dodge
					* (0.0f + (float)(_dodgeTimer.TimeLeft / _dodgeTimer.WaitTime) * 1.0f)
					* (left ? -1 : 1),
				0, 0);

			Move(velocity);
		}
	}
}
