using Godot;
using System;

public partial class Actor : CharacterBody3D
{

	[Export]
	Node3D target; 

	[Export]
	NavigationAgent3D navAgent; 

	[Export]
	RayCast3D ray;

	[Export]
	MeshInstance3D mesh; 

	[Export]
	AnimationPlayer animationPlayer; 


	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;


    public override void _Ready()
    {
		target = (Node3D)GetTree().GetNodesInGroup("Player")[0]; 

		GD.Print("target is:", target.Name); 


        base._Ready();
    }


	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		velocity.Y -= 9.8f * (float)delta;

		

		if(GlobalPosition.DistanceTo(target.GlobalPosition) > 5  ) //Move the actor towards the player if distance to player is > 5 
		{
			navAgent.TargetPosition = target.GlobalPosition; //Set target to the player's location 

			if(animationPlayer.CurrentAnimation == "stand")
			{
				animationPlayer.Play("walk_01"); 
			}

			Vector3 direction = (navAgent.GetNextPathPosition() - GlobalPosition).Normalized();
			velocity.X = Mathf.Lerp(velocity.X, direction.X * Speed, 0.5f);
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * Speed, 0.5f);

			// 1. Calculate the ideal transform as if we were instantly looking at the player
            try
			{
				Transform3D targetTransform = GlobalTransform.LookingAt(new Vector3(navAgent.GetNextPathPosition().X, GlobalPosition.Y, navAgent.GetNextPathPosition().Z), Vector3.Up);

				// 2. Extract Quaternions from the current and target basis
				Quaternion currentQuat = GlobalTransform.Basis.GetRotationQuaternion();
				Quaternion targetQuat = targetTransform.Basis.GetRotationQuaternion();

				// 3. Smoothly interpolate between them
				Quaternion newQuat = currentQuat.Slerp(targetQuat, 3f * (float)delta);
				
				// 4. Apply the new rotation while keeping the current position
				GlobalTransform = new Transform3D(new Basis(newQuat), GlobalPosition);
			
			}
			catch
			{
				//GD.Print("Already reached path position"); 
			}

           
            

		}
		else
		{
			LookAt(new Vector3(target.GlobalPosition.X, GlobalPosition.Y, target.GlobalPosition.Z), Vector3.Up);
			animationPlayer.Play("stand");
		}



		Velocity = velocity;
		MoveAndSlide();
	}
}
