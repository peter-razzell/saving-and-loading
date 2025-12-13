using Godot;
using Godot.Collections; 
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Level : Node3D
{    
    public Array<LevelExit> levelExits = new Array<LevelExit>();

    public Array<LevelEntrance> levelEntrances = new Array<LevelEntrance>();

    [Signal]
    public delegate void OnLevelLoadedEventHandler(Level level); 

    public override void _Ready() {

        foreach (Node node in GetTree().GetNodesInGroup("LevelExit"))
        {
            levelExits.Add((LevelExit)node);
        }
        foreach (Node node in GetTree().GetNodesInGroup("LevelEntrance"))
        {
            levelEntrances.Add((LevelEntrance)node);
        }

        GD.Print("level loaded", levelEntrances.Count, levelExits.Count);  

        EmitSignal(SignalName.OnLevelLoaded, this); //emitting the signal before connected in Root ready function? 

        base._Ready();
    }
    
}
