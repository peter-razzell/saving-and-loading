using Godot;
using System;


//This class applies the dither shader to the camera 
public partial class DitherShaderCamera : Control
{

	[Export]
	Camera3D t_a; //target to apply shader to. 

	[Export]
	Camera3D t_b; //target b

	[Export] 
	Camera3D origin; //player camera input 

	Camera3D depth_a;
	Camera3D depth_b;

    bool applyShader; 

	public override void _Ready() {
		
		depth_a = GetNode<Camera3D>("%L1DepthCam");
		depth_b = GetNode<Camera3D>("%L2DepthCam"); 

		base._Ready();

	}

	public override void _Process(double delta) {

		if(origin != null)
		{
			t_a.GlobalTransform = origin.GlobalTransform; 
			t_b.GlobalTransform = origin.GlobalTransform; 
			depth_a.GlobalTransform = origin.GlobalTransform;
			depth_b.GlobalTransform = origin.GlobalTransform; 
		}
		
		base._Process(delta);
	}


}
