using Godot;
using System;


//This class applies the dither shader to the camera 
public partial class DitherShaderCamera : Control
{
	[Export]
	Material shader; 
	SubViewportContainer SubViewportContainer; 
	
	[Export]
	Camera3D target; //target to apply shader to. 


	[Export] 
	Camera3D origin; //player camera input 

	Boolean applyShader; 

	public override void _Ready() {
		
		SubViewportContainer = GetNode<SubViewportContainer>("%SubViewportContainer"); 
		base._Ready();

	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("shader_toggle"))
		{
			applyShader = !applyShader;

			//Doesn't work because origin and target are in different scenes (game and player respectively). 
			if (applyShader)
			{
				SubViewportContainer.Material = shader; 
			}
			else
			{
				SubViewportContainer.Material = null; 
				
			}
		}
		base._Input(@event);
	}


	public override void _Process(double delta) {

		if(origin != null)
		{
			target.GlobalTransform = origin.GlobalTransform; 
		}
		
		base._Process(delta);
	}


}
