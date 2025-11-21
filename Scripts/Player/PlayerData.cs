using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public partial class PlayerData : Node3D
{
    [Export]
    PlayerInteractor playerInteractor;

    Array<Interactable> inv = []; 


    public override void _Ready()
    {
        playerInteractor.OnAddToPlayerInventory += AddToInv;

        base._Ready();
    }

    public void AddToInv(Interactable item)
    {
        GD.Print("adding item to player's inventory", item.Name);
        inv.Add(item);
        // DebugPrintInventory();

    }

    public Array<Interactable> GetInv()
    {
        return inv;
    }
    
    public void SetInv(Array<Interactable> savedInv)
    {
        inv = savedInv; 
    }


    public void DebugPrintInventory()
    {
        foreach (Interactable ob in inv)
        {
            GD.Print(ob.ToString());
        }
    }

}