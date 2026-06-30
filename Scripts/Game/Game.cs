using Godot;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

public partial class Game : Node3D
{

	public static Game Instance {get; private set;}

	/// <summary>
	/// does nothing currently
	/// </summary>
	[Export]
	public bool debug = false;

	[Signal]
	public delegate void GameLoadedEventHandler(); 

	[Export]
	public float gravity = 9.8f; 

	//Manager classes: 
	public SaverLoader saverLoader;

	public UiManager uiManager;

	public DayNightCycleManager dayNightCycleManager;

	public ColourDitherShader colourDitherShader;  

	public Root root;  
	
	/// <summary>
    /// Time since the start of the game
    /// </summary>
	public double Time {get; set;}

    


	public override void _Ready()
	{
		saverLoader = (SaverLoader)GetNode("%SaverLoader");

		uiManager = (UiManager)GetNode("%ui_manager");

		root = (Root)GetNode("%Root");

		dayNightCycleManager = (DayNightCycleManager)GetNode("%DayNightCycle"); 

		colourDitherShader = (ColourDitherShader)GetNode("Colour_Dither_Shader"); //this is NULL!
		
		root.OnLevelExitReached += LoadNextLevel; //applied from root load level

		root.OnLoadLevel += ApplyLevelData; //applied from root switch level 

		uiManager.OnSave += Save;

		uiManager.OnLoad += Load;

		root.LoadFirstLevel(); 

		EmitSignal(SignalName.GameLoaded);  

		Instance = this; 
		
		base._Ready();
	}

    public override void _Process(double delta)
    {
		Time += delta; 

		if(colourDitherShader == null)
		{
			colourDitherShader = (ColourDitherShader)GetNode("Colour_Dither_Shader"); 
		}
		if(dayNightCycleManager == null)
		{
			dayNightCycleManager = (DayNightCycleManager)GetNode("%DayNightCycle"); 
		}

        base._Process(delta);
    }

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("debug_quit"))
		{
			GetTree().Quit();
		}
		base._Input(@event);
	}

	//Save game. 
	public void Save()
	{
		if (saverLoader != null)
		{
			saverLoader.SaveGame();
			
			// replace with signal to play audio 
			AudioManager.Play("res://Assets/Sound/GDC/BluezoneCorp - Steampunk Machines/Bluezone_BC0305_steampunk_machine_mechanical_texture_heavy_impact_011.wav"); 

		}
		else
		{
			GD.Print("Save Loader is null.");
		}
	}

	//Save level on exit of the level so changes to level persist if returned. 
	public void SaveLevel()
	{
		saverLoader.SaveLevelToBuffer(); 
	}

	//Load game. 
	public void Load(string saveFile)
	{
		saverLoader.LoadGame(saveFile);
	}

	public void LoadNextLevel()
	{
		SaveLevel();
		root.LoadLevelAsync();
	}

	/// <summary>
    /// Calls the saverloader method which applies level data from the buffer. levelPath not curently needed 
    /// </summary>
    /// <param name="levelPath"></param>
	public void ApplyLevelData(String levelPath)
	{
		saverLoader.LoadLevelFromBuffer();
	}

	/// <summary>
	/// STUPID sticking plaster..
	/// </summary>
	public void SetDayNightCycleManager()
	{
		dayNightCycleManager = (DayNightCycleManager)GetNode("%DayNightCycle");
		

	}
	
}
