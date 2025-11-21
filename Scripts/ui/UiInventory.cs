using Godot;
using System;
using System.Linq;

public partial class UiInventory : Control
{
    
    PlayerData playerData; 

    VBoxContainer invContainer;

    CanvasLayer canvas; 


    public override void _Ready() {

        Player player = (Player)GetTree().GetNodesInGroup("Player")[0];
        playerData = player.playerData; 

        canvas = GetNode<CanvasLayer>("CanvasLayer");
        canvas.Hide(); 
        invContainer = GetNode<VBoxContainer>("%invContainer");
        
       

        base._Ready();
    }

    public void ShowScreen()
    {
        GD.Print("canvas showing"); 
        canvas.Show();

        foreach(Interactable inter in playerData.GetInv())
        {
            GD.Print("adding element from inventory to ui"); 
            PackedScene element = GD.Load<PackedScene>("res://Scenes/UI/ui_inventory_element.tscn");
            UiInventoryElement node = (UiInventoryElement) element.Instantiate();



            node._Ready(); //force ready function call. 
            node.Visible = true;  

            GD.Print("is the node visible? ", node.IsVisibleInTree());

            node.SetText("hello"); //not able to find this node!  
            invContainer.AddChild(node);


        }
                
        Input.MouseMode = Input.MouseModeEnum.Confined;
    }

    public void HideScreen()
    {
        canvas.Hide();

        Input.MouseMode = Input.MouseModeEnum.Captured;  
    }


    public bool UIIsVisible()
    {
        GD.Print(" visibility = ", canvas.Visible);
        return canvas.Visible; 
    }
}
