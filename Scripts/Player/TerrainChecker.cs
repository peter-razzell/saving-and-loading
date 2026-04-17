using Godot;
using System;
using System.Linq;

public enum FootstepEnum
{
    grass = 0,
    rock = 1
    
}

//Note doesn't need to be a raycast any more - doesn't use ANY raycast features! 
public partial class TerrainChecker : RayCast3D
{
    [Signal]
    public delegate void OnSwitchFootstepSoundEventHandler(string type); 

    public override void _Ready()
    {


     
    }

    public override void _Process(double delta)
    {
        //Get the terrain3D region control maps - these are image maps coloured for the different texture regons.
        try
        {   
            Node obj = (Node)GetCollider();

            if (obj.IsClass("Terrain3D"))
            {
                GodotObject terrainData = (GodotObject)obj.Call("get_data"); 

                Vector3 texture = (Vector3)terrainData.Call("get_texture_id", GlobalPosition); 

                GD.Print(texture); 

                if(texture.X == 0)
                {
                    GD.Print("on grass"); 

                    EmitSignal(SignalName.OnSwitchFootstepSound, "grass"); 
                }
                else
                {
                    GD.Print("on rock"); 
                    EmitSignal(SignalName.OnSwitchFootstepSound, "rock"); 
                }
            }

        }
        catch (Exception e )
        {
            GD.Print("SOMETHING FAILED WITH TERRAIN3D CHECKER",  e); 
        }

    }

}
