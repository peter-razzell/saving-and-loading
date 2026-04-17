using System;
using Godot;

public partial class InventoryObject : Node
{
    //TODO -- https://www.reddit.com/r/godot/comments/1l9hpdz/how_do_you_get_a_uid_at_runtime_in_code_godot_44/
    [Signal]
    public delegate bool OnInteractDisappearEventHandler(string pickupID);//for some fucked reason I'm using the pickup ID... 

    public string pickupID; //ID of the pickup / object in world this inventory object corresponds to. 

    public string ID; //inventroy item ID. 

    [Export]
    public string InvName; 

    [Export]
    public string Description; 
   
    [Export]
    public bool invInteractable; //is this interactable from within the inventory - show or hide interact button 

    [Export]
    public bool removeOnInteract; 

    [Export]
    public string interactText; //only shows up if this is interactable. 


    //Currently overridden by FoodInventoryObject and should be overridden by every interactable Obj. 
    public void OnObjectInteract()
    {
        
    }
}