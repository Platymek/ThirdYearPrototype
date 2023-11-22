using Godot;
using System;

public partial class CameraAnchor : Node3D
{
	[Export]
	Actor _player;

	[Export]
	Actor _target;

	// offset when pointing towards opponent
	[Export (PropertyHint.Range, "-180,180")]
	int _angleOffset;

	Camera3D _camera;

	public override void _Ready()
	{
		base._Ready();

		_camera = GetNode<Camera3D>("Camera3D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		// track player position
		Position = _player.Position;


		// rotate camera on X axis to point to target //

		Vector3 camRotation = _camera.Rotation;

		float distanceFromTarget = _player.Position.DistanceTo(_target.Position);

		camRotation.X = Mathf.DegToRad(_angleOffset) + new Vector2(0, Position.Y)
			.AngleToPoint(new Vector2(distanceFromTarget, 0));

		_camera.Rotation = camRotation;


		// rotate on Y axis to point to target

		Vector3 rotation = Rotation;

		rotation.Y = new Vector2(-Position.Z, Position.X)
			.AngleToPoint(new Vector2(-_target.Position.Z, _target.Position.X));

		Rotation = rotation;
	}
}
