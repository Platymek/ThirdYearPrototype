using Godot;
using Godot.Collections;
using System;

public partial class Opponent : Actor
{
	OpponentAttackStats _opponentAttackStats;

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

		CurrentAttacks = new()
		{
			{ AttackTypes.CloseToCorner, new Array<string>() },
			{ AttackTypes.FarFromCorner, new Array<string>() },
			{ AttackTypes.Neutral, new Array<string>() },
			{ AttackTypes.MixUp, new Array<string>() },
		};

		CurrentAttacks[AttackTypes.CloseToCorner].Add("Opponent/SumoPressure");
		CurrentAttacks[AttackTypes.FarFromCorner].Add("Opponent/BigPush");
		CurrentAttacks[AttackTypes.Neutral].Add("Opponent/SumoAdvance");
		CurrentAttacks[AttackTypes.MixUp].Add("Opponent/BigChop");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		if (_opponentAttackStats.FollowSpeed > 0 && Position2D.DistanceTo(_target.Position2D) > 2 && CurrentKnockback <= 0)
		{
			Move(new Vector3(0, 0, -_opponentAttackStats.FollowSpeed * (float)delta));
		}


	}
}
