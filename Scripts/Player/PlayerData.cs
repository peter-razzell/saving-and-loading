using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public partial class PlayerData : Node3D
{
    [Export]
    PlayerInteractor playerInteractor;

    Array<InventoryItem> inv = []; 

    public override void _Ready()
    {
        playerInteractor.OnAddToPlayerInventory += AddToInv;

        base._Ready();
    }

    public void AddToInv(Interactable intItem)
    {
        GD.Print("Adding item to player's inventory", intItem.Name);

        InventoryItem invItem = InventoryObjectManager.LookUpInventoryItem(intItem.pickup.DataLookupKey); 

        inv.Add(invItem);
    }

    public Array<InventoryItem> GetInv()
    {
        return inv;
    }
    
    public void SetInv(Array<InventoryItem> savedInv)
    {
        inv = savedInv; 
    }


    public void DebugPrintInventory()
    {
        GD.Print("Printing player inventory for debug: "); 

        foreach (InventoryItem ob in inv)
        {
            GD.Print(ob.ToString());
        }
    }

}