using Godot;
using System;

public partial class Player : Actor
{
	// Properties //

	[Export]
	bool _stateCancellable = true;

	[Export]
	int _chargePunchState = 0;


	// Node Functions //

	public override void _Ready()
	{
		base._Ready();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		// actions possible when the player can cancel their state
		if (_stateCancellable)
		{
			// when up is pressed, start charging punch
			if (Input.IsActionJustPressed("ui_up"))
			{
				PunchCharge();
			}
		}

		// code for each state
		switch (State)
		{
			// plays a different looping animation for heavy charge
			case "Player/PunchCharge":
			case "Player/PunchChargeFull":

				// when the player releases the button, they attack
				if (Input.IsActionJustReleased("ui_up"))
				{
					// charge of punch affects type of punch
					// (which is modified by animation player)
					switch (_chargePunchState)
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
		}
	}


	// Functions //

	void PunchCharge()
	{
		State = "Player/PunchCharge";
		_stateCancellable = false;
	}

	void PunchLight()
	{
		State = "Player/PunchLight";
		_stateCancellable = false;
	}

	void PunchMedium()
	{
		State = "Player/PunchMedium";
		_stateCancellable = false;
	}

	void PunchHeavy()
	{
		State = "Player/PunchHeavy";
		_stateCancellable = false;
	}
}
