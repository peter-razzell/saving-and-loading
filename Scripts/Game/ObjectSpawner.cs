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

       
       //change this to a spawner position that changes as more items are added - checks the position doesn't already contain an item, 
       //if it does, offset based on certain parameters (other objects in the scene for example). 
        Vector3 position = Player.Instance.itemSpawnMarker.GlobalPosition;  

        //Get current level
        Level current = Root.Instance.currentLevel; 

        //add child
        current.AddChild(node);

        //set position. 
        node.GlobalPosition = position; 
        
    }

}
