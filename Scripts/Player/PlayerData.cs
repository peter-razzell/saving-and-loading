using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public partial class PlayerData : Node3D
{
    [Export]
    PlayerInteractor playerInteractor;

    Array<InventoryObject> inv = []; 

    public override void _Ready()
    {
        playerInteractor.OnAddToPlayerInventory += AddToInv;

        base._Ready();
    }

    public void AddToInv(Interactable intItem)
    {
        GD.Print("Adding item to player's inventory", intItem.Name);

        InventoryObject invItem = InventoryObjectManager.LookUpInventoryItem(intItem.pickup.inventoryID, intItem.pickup.pickupID); 

        inv.Add(invItem);
    }

    public bool RemoveFromInv(string ItemID)
    {
        foreach(InventoryObject item in inv)
        {
            GD.Print(item.pickupID, ItemID); 
            if(item.pickupID == ItemID)
            {
                inv.Remove(item); 
                return true; 
            }
        }
        return false; 
    }

    public Array<InventoryObject> GetInv()
    {
        return inv;
    }
    
    public void SetInv(Array<InventoryObject> savedInv)
    {
        inv = savedInv; 
    }


    public void DebugPrintInventory()
    {
        GD.Print("Printing player inventory for debug: "); 

        foreach (InventoryObject ob in inv)
        {
            GD.Print(ob.ToString());
        }
    }

}