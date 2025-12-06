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
    public String nextLevelEntranceID; //ID for the next level entrance to load player to. 

    [Export]
    public String thisLevelEntranceID; //The ID for this entrance.

    public override void _Ready()
    {
        LevelEntrance entrance = GetNode<LevelEntrance>("LevelEntrance");
        
        entrance.levelEntranceID = thisLevelEntranceID;

        interactable = (Interactable)GetNode("Interactable");

        interactable.OnInteracted += Interact;

        base._Ready();
    }

    public void Interact(Interactor interactor)
    {
        if(CheckBody(interactor) == true)
        {
            EmitSignal(SignalName.OnPlayerEntered, nextLevelPath, nextLevelEntranceID);
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
