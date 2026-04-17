using Godot;
using System;

public partial class EnvironmentManager : WorldEnvironment
{
    


    [Export]
    DirectionalLight3D sun;

    [Export]
    float maxenergy = 4.5f,  minenergy = 0; 


    public override void _Process(double delta)
    {

        double time = Game.Instance.Time; //game time since the start of the game. 

        //TODO Day and night cycle. 
    
        // sun.RotateX((float)delta * 0.1f); 

        // Night time. 
        // if(sun.Rotation.X > 0 || sun.Rotation.X < 180)
        // {
        //     sun.LightEnergy = Mathf.Lerp(sun.LightEnergy, maxenergy, (float)delta * 0.2f); //sunset
            
        // }
        // else{
        //     sun.LightEnergy = 0f; //Mathf.Lerp(sun.LightEnergy, minenergy, (float)delta * 50f); 

        // }


        base._Process(delta);
    }



}
