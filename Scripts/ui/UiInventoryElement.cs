using Godot;
using System;

public partial class UiInventoryElement : Control
{
    public string inventoryID; 

    [Signal]
    public delegate void OnItemDropEventHandler(String itemID); 

    [Export]
    public Label itemName; 

    [Export]
    public Label itemDescription; 
 

    public override void _Ready() {
        
        base._Ready();
    }

    public void OnButtonPressed()
    {
        GD.Print("emitting signal with ID: ", inventoryID); 
        EmitSignal(SignalName.OnItemDrop, inventoryID); 

        Hide(); 
        
    }

}
