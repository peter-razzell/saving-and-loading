using System;
using Godot;

public partial class InventoryObject : Node
{

    //TODO -- https://www.reddit.com/r/godot/comments/1l9hpdz/how_do_you_get_a_uid_at_runtime_in_code_godot_44/
    
    public string pickupID; //ID of the pickup / object in world this inventory object corresponds to. 

    public string ID; //inventroy item ID. 

    [Export]
    public string InvName; 

    [Export]
    public string Description; 





}