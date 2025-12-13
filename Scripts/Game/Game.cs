using Godot;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

public partial class Game : Node3D
{
    [Signal]
    public delegate void GameLoadedEventHandler(); 

    [Export]
    public float gravity = 9.8f; 

    public SaverLoader saverLoader;

    public UiManager uiManager;

    public Root root;

    public override void _Ready()
    {
        saverLoader = (SaverLoader)GetNode("%SaverLoader");

        uiManager = (UiManager)GetNode("%ui_manager");

        root = (Root)GetNode("%Root");

        root.OnLevelExitReached += LoadNextLevel;

        root.OnLoadLevel += ApplyLevelData;

        uiManager.OnSave += Save;

        uiManager.OnLoad += Load;

        root.LoadFirstLevel(); 

        EmitSignal(SignalName.GameLoaded);  

        base._Ready();
    }

    //TODO Input manager? 
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

    public void ApplyLevelData(String levelPath)
    {
        saverLoader.LoadLevelFromBuffer(levelPath);
    }
    
}