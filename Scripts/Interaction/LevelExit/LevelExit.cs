using Godot;
using System;

//Will be base class for doors conncting different levels. 
public partial class LevelExit : Node3D
{

    Interactable interactable;

    [Signal]
    public delegate void OnPlayerEnteredEventHandler(String nextLevelPath, String entranceID);
    public Area3D area;

    [Export]
    public String nextLevelPath;

    [Export]
    public String levelEntranceID; //ID for the level entrance to load player to. 

    [Export]
    public String thisEntranceID; 

    public override void _Ready()
    {
        LevelEntrance entrance = GetNode<LevelEntrance>("LevelEntrance");
        
        entrance.levelEntranceID = thisEntranceID;

        interactable = (Interactable)GetNode("Interactable");

        interactable.OnInteracted += Interact;

        base._Ready();
    }

    public void Interact(Interactor interactor)
    {
        GD.Print("interacting");
        if(CheckBody(interactor) == true)
        {
            EmitSignal(SignalName.OnPlayerEntered, nextLevelPath, levelEntranceID);
        }

    }

    public void Focus(Interactor interactor)
    {

    }

    public void Unfocus(Interactor interactor)
    {
        
    }


    public bool CheckBody(Node3D node)
    {
        GD.Print("something entered");

        GD.Print(node.Name);

        if (node.Name == "Interactor")
        {
            GD.Print("player entered area");
            return true;
        }
        return false; 

    }
}
