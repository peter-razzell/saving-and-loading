
using System;
using System.Diagnostics.Tracing;
using System.Numerics;
using System.Threading.Tasks;
using Godot;

public partial class Root : Node3D
{

    public static Root Instance; 
    //Contains information about the currently loaded level.

    [Signal]
    public delegate void OnLevelExitReachedEventHandler();


    //Sent when a new level is loaded to retrieve that level's current data from the save file. 
    [Signal]
    public delegate void OnLoadLevelEventHandler(String levelPath);

    String entranceID = "default"; //cache it here rather than pass it needlessly back to game and here again. 

    public Level currentLevel;

    String levelPath  = "res://Scenes/Levels/level_0.tscn"; //default val

    Game game;

    Player player;

    public async override void _Ready()
    {
        Instance = this; 

        player = GetNode<Player>("%Player");

        game = GetParent<Game>();

        base._Ready();

    }

    public void LoadFirstLevel()
    {
        LoadLevel(levelPath); 
    }

    //1. Emits a signal when the exit is reached, received by Game. 
    public void LevelExitReached(String levelPath, String entranceID)
    {
        // GD.Print("level exit reached in root");
        this.levelPath = levelPath;
        this.entranceID = entranceID; 

        EmitSignal(SignalName.OnLevelExitReached);
    }

   
    //2. Loads the level from path (when loading save file or initialising game)
    public void LoadLevel(String levelPath)
    {
        PackedScene nextLevel = GD.Load<PackedScene>(levelPath);

        Node3D newLevel = (Node3D)nextLevel.Instantiate();

        Level level = (Level)newLevel; 

        level.OnLevelLoaded += OnLevelLoaded;

        SwitchLevel(level); 

    }

    //2. Asynchronously loads the next level (when loading from stepping through a portal/door)
    public async void LoadLevelAsync()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        GetTree().Paused = true;

        PackedScene nextLevel = GD.Load<PackedScene>(levelPath);

        Level newLevel = (Level)nextLevel.Instantiate();

        newLevel.OnLevelLoaded += OnLevelLoaded;

        GetTree().Paused = false;

        SwitchLevel(newLevel);
    }

    //3. Switch to the newly loaded level, used by both loading methods
    //TODO make loading screen. 
    public void SwitchLevel(Level newLevel)
    {
        if (newLevel != null)
        {
            foreach (Node child in GetChildren())
            {
                if (!child.IsInGroup("DoNotDestroyOnLoad"))
                {
                    RemoveChild(child);
                    child.QueueFree();
                }
            }

            AddChild(newLevel);

            currentLevel = newLevel;

            //delete all nodes with saveable data to prevent duplication when loading the saved versions.         
            if (game.saverLoader.levelBuffer.ContainsKey(GetCurrentLevelPath()))
            {
                foreach(Node node in currentLevel.GetChildren())
                {
                    if(node is SaveableNode saveableNode)
                    {
                        node.QueueFree(); 
                    }
                }
            }

            //Received by Game class, which then applies saved level data using SaverLoader.LoadLevelFromBuffer
            EmitSignal(SignalName.OnLoadLevel, levelPath);

            AudioManager.EndPrevLevelAudio();

            //TODO change audio based on level. 
            AudioManager.Play("res://Assets/Sound/GDC/Bolt - Immersive Creek -  Ambisonic Recordings of Undisturbed Creeks in Vermont/WATRFlow_Babbling Brook, Snow Melt, Calm, Constant, Bubbling_BOLT_Immersive Creek_RODE NTSF1 XY.wav");
        }
    }
    
    //4. Called by Level emitting a signal when it has been loaded, sets up new level signals. 
    public void OnLevelLoaded(Level level)
    {
        currentLevel = level; 
        foreach (LevelExit exit in level.levelExits)
        {
            exit.OnPlayerEntered += LevelExitReached;
        }

        //ResetPlayer called here because Level has been loaded with exits etc. 
        ResetPlayer(GetPlayerMarkerCoordinates());

    }

    //Returns the current level path for saving
    public String GetCurrentLevelPath()
    {
        return currentLevel.SceneFilePath;
    }

    //*The functions below are called by SwitchLevel
    //Get marker coordinates to move player when loading level
    Godot.Vector3 GetPlayerMarkerCoordinates()
    {
        Godot.Collections.Array<Node> levelEntrances = GetTree().GetNodesInGroup("LevelEntrance");

        GD.Print(levelEntrances.Count); 
        foreach (Node node in levelEntrances)
        {
            if (node is LevelEntrance levelEntrance)
            {
                if (levelEntrance.levelEntranceID == entranceID)
                {
                    return levelEntrance.GlobalPosition;
                }
            }
        }

        // GD.Print("WARNING NO LEVEL ENTRANCE FOUND FOR PLAYER, RETURNING DEFAULT POSITION");
        return new Godot.Vector3(0, 0, 0); //default player debug position
        
    }

    //Reset player location and remove any cached data from previous level 
    void ResetPlayer(Godot.Vector3 coords)
    {
        player.GlobalPosition = coords; 
        player.ResetOnLevelLoad(); 
    }
   
}
