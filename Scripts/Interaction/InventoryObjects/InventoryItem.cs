using System;
using Godot;

public partial class InventoryItem : Node
{
    [Export]
    public String InvName; 

    [Export]
    public String Description; 

    
    //Add another field of a new class called "object data" which will be the object data - any buffs/ debuffs etc.
    // public InventoryItem(String Name, String Description)
    // {
    //     InvName = Name;
    //     this.Description = Description;
    // }



}