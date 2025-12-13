using Godot;
using System;
using System.Collections.Generic; 

public partial class InventoryObjectManager : Node
{

    public override void _Ready() {

        base._Ready();
    }

    public static InventoryObject LookUpInventoryItem(string inventoryID, string pickupID)
    {
        
        try
        {
            GD.Print("trying to load item with UID: ", inventoryID); 
            PackedScene packedItem = GD.Load<PackedScene>(inventoryID); 

            Node item = packedItem.Instantiate(); 

            InventoryObject inventoryItem = (InventoryObject) item; 

            inventoryItem.pickupID = pickupID; 

            inventoryItem.ID = inventoryID;

            GD.Print("success, loaded: ", inventoryItem.InvName); 

            return inventoryItem; 

        }
        catch
        {
            GD.Print("Unable to find item in inventory, UID is: ", inventoryID); 
            return new InventoryObject(); 
        }

    }

    public static PackedScene LookUpPickupItem(string pickupID)
    {
        try
        {
            PackedScene pickupItem = GD.Load<PackedScene>(pickupID); 

            return pickupItem;       
        }
        catch
        {
            GD.Print("Error returning pickup item, perhaps the UID is incorrect? UID = ", pickupID); 
            return null;    
        }
    }
    
}
