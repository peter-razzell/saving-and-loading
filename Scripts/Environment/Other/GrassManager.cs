using Godot;
using System;

public partial class GrassManager : Node3D
{
    Node Terrain3DInstance;

    public override void _Ready()
    {
        Terrain3DInstance = this.GetParent();

        if (Terrain3DInstance.IsClass("Terrain3D"))
        {
            GD.Print("found terrain3D instance for grass"); //Check it's even getting the terrain3D - parent of this node

            GodotObject terrainData = (GodotObject)Terrain3DInstance.Call("get_data"); 

            //Get region player is currently in
            GodotObject currentRegion = (GodotObject)terrainData.Call("get_regionp", Player.Instance.GlobalPosition);

            //gets the multimesh instances dictionary for the current region
            GodotObject instances = (GodotObject)currentRegion.Call("get_instances"); 

            //instances structure
            
            
            //Get the grass objects in the current region / (adjacent regions?) 

            //Change the render layers to 1 and 2 in the multimesh instance 3D bitmask. 

            //call  Terrain3DInstancer.update_mmis() to rebuild multimesh instances. 
        }

        base._Ready();
    }



    

}
