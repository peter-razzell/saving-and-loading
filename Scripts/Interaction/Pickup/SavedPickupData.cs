using Godot;


// Could refactor into a pickup data class, with export variables of saved attributeS?? 
public partial class SavedPickupData : SavedData
{
    [Export]
    public bool interacted;
}