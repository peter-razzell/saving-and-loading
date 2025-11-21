using Godot;
using System;
using System.Security.Cryptography;

public partial class UiSaveLoad : Control
{

    [Signal]
    public delegate void OnLoadEventHandler(string saveFile);

    [Signal]
    public delegate void OnSaveEventHandler(); 

    bool pause = false;

    CanvasLayer canvas;

    [Export]
    TextureRect prevSaveImg;

    Game game;

    UiSaveSelector saveSelect; 


    public override void _Ready()
    {
        game = (Game)GetTree().GetFirstNodeInGroup("Root");

        canvas = GetNode<CanvasLayer>("CanvasLayer");

        saveSelect = GetNode<UiSaveSelector>("%UiSaveSelector");

        saveSelect.OnLoadSaveSelector += LoadGame; 

        canvas.Hide(); 
        
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

    public void ShowScreen()
    {
        canvas.Show();
                
        UpdateSaveImage();

        Input.MouseMode = Input.MouseModeEnum.Confined;
    }

    public void HideScreen()
    {
        canvas.Hide();

        saveSelect.FreeContainers();

        saveSelect.Hide();
        
        saveSelect.ProcessMode = ProcessModeEnum.Disabled; 

        Input.MouseMode = Input.MouseModeEnum.Captured;  
    }

    void UpdateSaveImage()
    {
        prevSaveImg.Texture = new ImageTexture();

        prevSaveImg.Texture.Set("image", game.saverLoader.GetSaveImage());

    }

    public void OnLoadPressed()
    {
        if (saveSelect.ProcessMode is ProcessModeEnum.Disabled)
        {
            saveSelect.PopulateContainers(game.saverLoader.GetSaveNames()); 
            saveSelect.ProcessMode = ProcessModeEnum.Inherit;
            saveSelect.Show(); 
        }
    }

    public void OnSavePressed()
    {
        EmitSignal(SignalName.OnSave);

        UpdateSaveImage(); 
    }

    public void LoadGame(String saveFile)
    {
        GD.Print("Loading game from UI screen"); 
        EmitSignal(SignalName.OnLoad, saveFile);
    }

    public bool UIIsVisible()
    {
        GD.Print(" visibility = ", canvas.Visible);

        return canvas.Visible; 
    }
    

}
