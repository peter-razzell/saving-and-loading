using Godot;
using System;


//Autoload
public partial class ObjectSpawner : Node
{
    public static ObjectSpawner Instance; 

    public override void _Ready()
    {
        Instance = this; 
    }

    public static void SpawnObject(Node3D node)
    {

        //TODO fix - currently spawns key at the players initial position 
        Vector3 position = Player.Instance.itemSpawnMarker.GlobalPosition; //change this to a spawner position in front of player view. 

        //Get current level
        Level current = Root.Instance.currentLevel; 

        //add child
        current.AddChild(node);

        //set position. 
        node.GlobalPosition = position; 
        
    }

}
