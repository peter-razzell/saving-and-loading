
using System;
using System.Diagnostics.Tracing;
using System.Numerics;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// Class which controls loading the level. Adds the level as a child object. Game is it's parent both in scene tree and code (root instance of game node). 
/// </summary>
public partial class Root : Node3D
{

	public static Root Instance; 
	//Contains information about the currently loaded level.

	[Signal]
	public delegate void OnLevelExitReachedEventHandler();


	//Sent when a new level is loaded to retrieve that level's current data from the save file. 
	[Signal]
	public delegate void OnLoadLevelEventHandler(String levelPath);

	String doorID = "default"; //This is the ID of the door to spawn the player at. Cached here rather than pass it needlessly back to game and here again. 

	public Level currentLevel;

	[Export]
	String currentLevelPath  = "res://Scenes/Levels/level_0.tscn"; //default val

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
		LoadLevel(currentLevelPath); 
	}

	//1. Emits a signal when the exit is reached, received by Game. 
	public void LevelExitReached(String levelPath, String doorID)
	{
		// GD.Print("level exit reached in root");
		this.currentLevelPath = levelPath;
		this.doorID = doorID; 

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

		PackedScene nextLevel = GD.Load<PackedScene>(currentLevelPath);

		Level newLevel = (Level)nextLevel.Instantiate();

		newLevel.OnLevelLoaded += OnLevelLoaded;

		GetTree().Paused = false;

		SwitchLevel(newLevel);
	}

	//3. Switch to the newly loaded level, used by both loading methods
	public void SwitchLevel(Level newLevel)
	{
		if (newLevel != null)
		{
			//deletes the previous level
			foreach (Node child in GetChildren())
			{
				if (!child.IsInGroup("DoNotDestroyOnLoad"))
				{
					RemoveChild(child);
					child.QueueFree();
				}
			}

			//Adds the new level and assigns currentLevel reference to this new level. 
			AddChild(newLevel);
			currentLevel = newLevel;

			//Delete default pickups in level IF the levelBuffer already has the level - e.g. player has visited this level beforehand! 
			if (game.saverLoader.levelBuffer.ContainsKey(GetCurrentLevelPath()))
			{
				foreach(Node node in currentLevel.GetChildren())
				{
					if(node.IsInGroup("Persist")) //Note - NOT using savable node here as non-persisting objects that can be interacted with are unfortunately also descended from saveablenode - whoops! 
					{
						GD.Print("deleting something which should be a pickup: ", node.Name); 
						node.QueueFree(); 
					}
				}
			}

			//Received by Game class, which then applies saved level data using SaverLoader.LoadLevelFromBuffer
			EmitSignal(SignalName.OnLoadLevel, currentLevelPath);

			AudioManager.EndPrevLevelAudio();

			//TODO change audio based on level. 
			AudioManager.Play("res://Assets/Sound/GDC/Bolt - Immersive Creek -  Ambisonic Recordings of Undisturbed Creeks in Vermont/WATRFlow_Babbling Brook, Snow Melt, Calm, Constant, Bubbling_BOLT_Immersive Creek_RODE NTSF1 XY.wav");
		}
	}
	
	//4. Called by Level emitting a signal when it has been loaded, sets up new level signals. 
	public void OnLevelLoaded(Level level)
	{
		GD.Print("Level loaded"); 
		currentLevel = level; 
		foreach (LevelExit exit in level.levelExits)
		{
			exit.OnPlayerEntered += LevelExitReached;
		}

		//ResetPlayer called here because Level has been loaded with exits etc. 
		ResetPlayer(GetPlayerMarkerCoordinates());

	}

	//Returns the current level path for saving
	public string GetCurrentLevelPath()
	{
		return currentLevel.SceneFilePath;
	}

	//*The functions below are called by SwitchLevel
	//Get marker coordinates to move player when loading level
	Godot.Vector3 GetPlayerMarkerCoordinates()
	{
		Godot.Collections.Array<Node> levelEntrances = GetTree().GetNodesInGroup("LevelExit");

		GD.Print(levelEntrances.Count); 
		foreach (Node node in levelEntrances)
		{
			if (node is LevelExit levelExit)
			{
				if (levelExit.doorID == doorID)
				{
					return levelExit.entranceMarker.GlobalPosition;
				}
			}
		}

		GD.Print("WARNING NO LEVEL ENTRANCE FOUND FOR PLAYER, RETURNING DEFAULT POSITION");
		return new Godot.Vector3(0, 10, 0); //default player debug position
		
	}

	//Reset player location and remove any cached data from previous level 
	void ResetPlayer(Godot.Vector3 coords)
	{
		player.GlobalPosition = coords; 
		player.ResetOnLevelLoad(); 		GD.Print("player pos = ", player.GlobalPosition); 

	}
   
}
