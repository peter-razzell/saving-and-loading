using Godot;
using System;
using System.Collections.Generic; 

public partial class InventoryObjectManager : Node
{

    public override void _Ready() {

        base._Ready();
    }

    public static InventoryItem LookUpInventoryItem(string UID)
    {
        
        try
        {
            GD.Print("trying to load item with UID: ", UID); 
            PackedScene packedItem = GD.Load<PackedScene>(UID); 

            Node item = packedItem.Instantiate(); 

            InventoryItem inventoryItem = (InventoryItem) item; 

            GD.Print("success, loaded: ", inventoryItem.InvName); 

            return inventoryItem; 

        }
        catch
        {
            GD.Print("Unable to find item in inventory, UID is: ", UID); 
            return new InventoryItem(); 
        }
    }
    
}
