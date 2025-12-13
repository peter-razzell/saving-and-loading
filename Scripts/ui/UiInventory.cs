using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;

public partial class UiInventory : Control
{

    [Signal]
    public delegate void OnInventoryDropButtonPressedEventHandler(string itemID); 
    
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

        Godot.Collections.Array<InventoryObject> invData = playerData.GetInv(); 

        foreach(InventoryObject invObject in invData)
        {
            UiInventoryElement uiInvElement = (UiInventoryElement) element.Instantiate();

            uiInvElement.itemName.Text = invObject.InvName;  

            uiInvElement.itemDescription.Text = invObject.Description;

            GD.Print("object ID = ", invObject.pickupID); 

            uiInvElement.inventoryID = invObject.pickupID; 

            uiInvElement.OnItemDrop += DropButtonPressed; 

            invContainer.AddChild(uiInvElement);

            invInstances.Add(uiInvElement); 
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

    void DropButtonPressed(string pickupID)
    {
        //remove from player data inventory. 

        EmitSignal(SignalName.OnInventoryDropButtonPressed, pickupID);
   
    }
}
