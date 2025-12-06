using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UiInventory : Control
{
    
    PlayerData playerData; 

    VBoxContainer invContainer;

    CanvasLayer canvas; 

    Godot.Collections.Array<UiInventoryElement> invInstances = new Godot.Collections.Array<UiInventoryElement>(); 

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
        canvas.Show();

        PopulateContainers(); 
                
        Input.MouseMode = Input.MouseModeEnum.Confined;
    }

    public void HideScreen()
    {
        FreeContainers(); 
        
        canvas.Hide();

        Input.MouseMode = Input.MouseModeEnum.Captured;  
    }

    public bool UIIsVisible()
    {
        return canvas.Visible; 
    }
    
    public void PopulateContainers()
    {
        PackedScene element = GD.Load<PackedScene>("res://Scenes/UI/ui_inventory_element.tscn");

        Godot.Collections.Array<InventoryItem> data = playerData.GetInv(); 

        GD.Print("player data array size = ", data.Count);

        List<String> names = new List<string>(); 

        foreach (var item in data)
        {
            GD.Print("Inventory mgr found item with name: ", item.InvName);
            names.Add(item.InvName); 
        } 

        foreach(InventoryItem item in data)
        {
            GD.Print("populating inventory UI with name: ", item.InvName); 
            UiInventoryElement node = (UiInventoryElement) element.Instantiate();

            node.itemName.Text = item.InvName;  

            node.itemDescription.Text = item.Description;

            invContainer.AddChild(node);

            invInstances.Add(node); 
        }       
    }

    public void FreeContainers()
    {
        var children = invContainer.GetChildren();
        foreach(var child in children)
        {
            child.QueueFree(); 
        }

        invInstances.Clear(); 

    }
}
