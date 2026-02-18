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
    public String doorID; //ID for the linked pair of LevelExits to load player to. 

    [Export]
    public LevelEntrance entranceMarker; 

    public override void _Ready()
    {
        LevelEntrance entrance = GetNode<LevelEntrance>("LevelEntrance"); //The marker for the player to spawn at. 
        
        interactable = (Interactable)GetNode("Interactable");

        interactable.OnInteracted += Interact;

        base._Ready();
    }

    public void Interact(Interactor interactor)
    {
        if(CheckBody(interactor) == true)
        {
            EmitSignal(SignalName.OnPlayerEntered, nextLevelPath, doorID);
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
