using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;


public partial class PlayerData : Node3D
{
    [Export]
    PlayerInteractor playerInteractor;

    PlayerStatus playerState; 

    Array<InventoryObject> inv = []; 

    public event PlayerStatus.OnDie OnDeath; //event that other classes can subscribe to, to be notified when the player dies.

    

    public override void _Ready()
    {
        playerState = new PlayerStatus(); //this needs to be loaded and saved . 

        OnDeath += OnPlayerDeath; //subscribe to the OnDeath event, so that the OnPlayerDeath method will be called when the player dies.

        playerInteractor.OnAddToPlayerInventory += AddToInv;

        base._Ready();
    }

    //Update player state every phyiscs process - I don't want a rabbit hole of everything needlessly being a node
    //the buck stops here.

    double updateFrequency = 1; //update every 1000 ms 
    public override void _PhysicsProcess(double delta)
    {
        updateFrequency -= delta; 
        if(updateFrequency < 0)
        {
            playerState.UpdatePlayerState();
            // GD.Print("updating player state");
            updateFrequency = 1;
        }
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

    public void SetState()
    {
        
    }


    public void DebugPrintInventory()
    {
        GD.Print("Printing player inventory for debug: "); 

        foreach (InventoryObject ob in inv)
        {
            GD.Print(ob.ToString());
        }
    }

    public void OnPlayerDeath()
        {
            GD.Print("DEATH CATASTOPHE: UH OH, THE PLAYER HAS DIED. HANDLE DEATH IN THIS METHOD.");
            //handle player death (e.g. show game over screen, reset game, etc.)
        }

}
