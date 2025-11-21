using Godot;
using System;

public partial class UiInventoryElement : Control
{
    [Export]
    public TextEdit elementText; 

    public override void _Ready() {

        GD.Print("UI element ready to be referenced");
        
        base._Ready();
    }

    public void SetText(String text)
    {
        elementText = GetNode<TextEdit>("PanelContainer/TextEdit"); 
        elementText.Text = text; 
    }
}
