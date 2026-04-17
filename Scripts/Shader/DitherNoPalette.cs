using Godot;
using System;

public partial class DitherNoPalette : Control
{
    
    [Export]
    Camera3D target; 

    [Export]
    Camera3D origin; 

    public override void _Process(double delta) {

        target.GlobalTransform = origin.GlobalTransform; 

    }

}
