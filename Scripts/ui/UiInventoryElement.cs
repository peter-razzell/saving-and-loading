using Godot;
using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Security;

public partial class UiInventoryElement : Control
{
    public string inventoryID; 

    [Signal]
    public delegate void OnItemDropEventHandler(String itemID); 

    [Signal]
    public delegate void OnItemInventoryInteractEventHandler();

    [Export]
    public Label itemName; 

    [Export]
    public Label itemDescription; 

    [Export]
    public Button interactButton; 
 

    public override void _Ready() {

    
        
        base._Ready();
    }

    public void OnDropBtnPressed()
    {
        GD.Print("emitting signal with ID: ", inventoryID); 
        EmitSignal(SignalName.OnItemDrop, inventoryID); 

        Hide(); 
        
    }

    public void OnInteractBtnPressed()
    {
        GD.Print("INTERACT BUTTON PRESSED"); 
        EmitSignal(SignalName.OnItemInventoryInteract); 

        Hide(); //for now - there might be interactable items which DON'T need to be hidden! 
    }



}
