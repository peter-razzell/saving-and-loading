using Godot;
using System;

public partial class UiManager : Control
{
    //Signals to pass from save screen back up the chain to game class. 
    [Signal]
    public delegate void OnSaveEventHandler();

    [Signal]
    public delegate void OnLoadEventHandler(String saveFile); 

    [Signal]
    public delegate bool OnInventoryDropButtonPressedEventHandler(string objectID); 

    UiSaveLoad uISaveScreen;  //actually save screen but I didn't think when I named the class. 

    UiInventory uiInventory; 

    public override void _Ready() {

        uiInventory = GetNode<UiInventory>("ui_inventory"); 
        uISaveScreen = GetNode<UiSaveLoad>("ui_save"); 

        uISaveScreen.OnSave += Save;
        uISaveScreen.OnLoad += Load; 
        uiInventory.OnInventoryDropButtonPressed += InventoryDropItem; 
        base._Ready();

    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_pause"))
        {
            if (uISaveScreen.UIIsVisible())
            {
                uISaveScreen.HideScreen(); 
            }
            else
            {
                uISaveScreen.ShowScreen(); 
            }
        }

        else if (@event.IsActionPressed("ui_inventory"))
        {
            if (uiInventory.UIIsVisible())
            {
                uiInventory.HideScreen();
            }
            else
            {
                uiInventory.ShowScreen(); 
            }
        }
    }

    //Emit signal back up the chain 
    void Load(String saveFile)
    {
        EmitSignal(SignalName.OnLoad, saveFile); 
    }

    void Save()
    {
        EmitSignal(SignalName.OnSave);    
    }

    void InventoryDropItem(string pickupID)
    {
        //TODO implement remove item from player inventory. 

        var packedScene = InventoryObjectManager.LookUpPickupItem(pickupID).Instantiate(); 

        InteractablePickup interactablePickup = (InteractablePickup) packedScene; 

        ObjectSpawner.SpawnObject(interactablePickup);

        EmitSignal(SignalName.OnInventoryDropButtonPressed, pickupID);    
    }
}
