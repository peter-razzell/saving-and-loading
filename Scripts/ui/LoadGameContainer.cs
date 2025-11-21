using Godot;
using System;

public partial class LoadGameContainer : HSplitContainer
{

    [Signal]
    public delegate void OnLoadGameContainerButtonPressedEventHandler(string saveFile); 
    [Export]
    public string saveFile;

    [Export]
    public Button button;

    [Export]
    public TextureRect textureRect;


    public override void _Ready()
    {
        button.Pressed += Load;
        base._Ready();
    }
    
    void Load()
    {
        GD.Print("container button pressed"); 
        EmitSignal(SignalName.OnLoadGameContainerButtonPressed, saveFile); 
        
    }
}
