using Godot;
using System;

public partial class UiManager : Control
{
	
	Player player; //Reference to player to access player data. 

	//Signals to pass from save screen back up the chain to game class. 
	[Signal]
	public delegate void OnSaveEventHandler();

	[Signal]
	public delegate void OnLoadEventHandler(String saveFile); 

	[Signal]
	public delegate bool OnInventoryDropButtonPressedEventHandler(string objectID); 

	UiSaveLoad uISaveScreen;  //actually save screen but I didn't think when I named the class. 

	UiInventory uiInventory; 

	GameplayUi gameplayUi; 

	public override void _Ready() {

		player = Player.Instance; 
		PlayerStatus playerState = player.playerData.playerState; 

		uiInventory = GetNode<UiInventory>("ui_inventory"); 
		uISaveScreen = GetNode<UiSaveLoad>("ui_save"); 
		gameplayUi = GetNode<GameplayUi>("ui_gameplay"); 

		playerState.OnUpdateHunger += UpdateHunger; 
		playerState.OnUpdateEnergy += UpdateEnergy; 

		playerState.OnUpdateMaxHunger += gameplayUi.SetHungerMax;
		playerState.OnUpdateMaxEnergy += gameplayUi.SetEnergyMax;

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
		// GD.Print("Dropped object with ID: ", pickupID);    

		var packedScene = InventoryObjectManager.LookUpPickupItem(pickupID).Instantiate(); //BUG - object reference not set to instance of an object. 

		InteractablePickup interactablePickup = (InteractablePickup) packedScene; 

		ObjectSpawner.SpawnObject(interactablePickup);

		EmitSignal(SignalName.OnInventoryDropButtonPressed, pickupID);    
	}

	public void UpdateHunger(float value)
    {
		gameplayUi.UpdateHungerBar(value); 

    }

	public void UpdateEnergy(float value)
    {
		gameplayUi.UpdateEnergyBar(value); 
    }
}
