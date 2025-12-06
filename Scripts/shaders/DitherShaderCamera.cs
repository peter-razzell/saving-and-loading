using Godot;
using System;

public partial class DitherShaderCamera : Control
{
    
    [Export]
    Camera3D target; //target to apply shader to. 


    [Export] 
    Camera3D origin; //player camera input 


    public override void _Ready() {
        
        base._Ready();

    }

    public override void _Process(double delta) {

        if(origin != null)
        {
            target.GlobalTransform = origin.GlobalTransform; 
        }
        base._Process(delta);
    }


}
