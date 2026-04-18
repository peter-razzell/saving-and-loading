using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;



/// <summary>
/// This class is a sort of weird collection which contains:
/// 
///  - The player's inventory as an array of objects
/// 
///  - The player's current status / health etc.
/// 
///  - The player interactor - which is used to interacting with objects in the world. 
/// 
/// Having them all together connected by this class is useful as the systems interact with each other a lot. 
/// 
/// Also, as this is all the player's "data", it can be loaded here when loading the game.
/// 
/// </summary>
public partial class PlayerData : Node3D
{
    Player player = Player.Instance; //Reference needed for multiple places in player status. E.g. getting movement data to calculate calories expended.
    
    [Export]
    PlayerInteractor playerInteractor;

    [Export]
    //Putting this here so it's accessible and can be changed. 
    double playerStatusUpdateFreq;

    public PlayerStatus playerState; 

    Array<InventoryObject> inv = []; 

    public event PlayerStatus.OnDieEventHandler OnDeath; //Event that other classes can subscribe to, to be notified when the player dies.

    [Export]
    float energyDecayRate = 0.1f, energyMax = 100f; 


    public override void _Ready()
    {
        playerState = new PlayerStatus(this, playerStatusUpdateFreq, energyDecayRate, energyMax); //TODO this needs to be loaded and saved

        OnDeath += OnPlayerDeath; 

        playerInteractor.OnAddToPlayerInventory += AddToInv;
        playerInteractor.OnSleepInBed += SleepInBed; 

        base._Ready();
    }
    
    double updateFrequency = 1; //update every 1000 ms - I am putting single use variables above the methods which require them. 
    public override void _PhysicsProcess(double delta)
    {
        if(playerState != null)
        {
            playerState.UpdatePlayerStateDelay(delta);
        }
        else
        {
            playerState = new PlayerStatus(this, playerStatusUpdateFreq, energyDecayRate, energyMax); 
        }
          
    
    }
    void AddToInv(InteractablePickup pickup)
    {
        // GD.Print("Adding item to player's inventory", intItem.Name);

        try
        {
            // InteractableObject obj = intItem.interactableObject; 
            // InteractablePickup pickup = (InteractablePickup)obj; 
            // !Reference to Class outside of player folder. 
            InventoryObject invItem = InventoryObjectManager.LookUpInventoryItem(pickup.inventoryID, pickup.pickupID); 

            // Subscribe to the interact signal of interactable inventory items
            if (invItem.invInteractable)
            {
                if(invItem is FoodInventoryObject foodInvItem)
                {
                    foodInvItem.OnFoodEaten += playerState.EatFood; 
                    foodInvItem.OnInteractDisappear += RemoveFromInv; 
                }
            }

            inv.Add(invItem);
            
        }
        catch
        {
            GD.Print("This method shouldn't be running, interactable item that is not a pickup is trying to add itself to player's inventory: ", pickup.Name);

        }

    }

    public bool RemoveFromInv(string pickupID)
    {
        GD.Print("remove from inv called, pikcup id = ", pickupID); 

        foreach(InventoryObject item in inv)
        {
            if(item.pickupID == pickupID)
            {
                GD.Print("removing item"); 
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

    //TODO - make player death
    public void OnPlayerDeath()
    {
        //handle player death (e.g. show game over screen, reset game, etc.)
    }

    public Player GetPlayer()
    {
        return player; 
    }

    void SleepInBed()
    {
        
    }
}
