using Godot;
using System;

public partial class UiInventoryElement : Control
{
    [Export]
    public Label itemName; 

    [Export]
    public Label itemDescription; 

    public override void _Ready() {
        
        base._Ready();
    }

}
