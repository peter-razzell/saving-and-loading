using Godot;
using System;

public partial class FPSCounter : Control
{

    [Export]
    Label FPSLabel; 


    public override void _Process(double delta) {

        FPSLabel.Text = "FPS: " + Engine.GetFramesPerSecond().ToString(); 

        base._Process(delta);
    }

    

}
