using Godot;


//This is for saving the state of interactable items in the world e.g. a door, light, or pickup.  
public partial class SavedInteractableData : SavedData
{
    [Export] //Saveable attributes have to be export variables.
    public bool interacted;
}