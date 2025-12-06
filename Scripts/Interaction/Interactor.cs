using Godot;
using Godot.Collections; 
using System;
using System.Collections.Generic;

public partial class Interactor : Area3D
{
    public void Interact(Interactable interactable)
    {
        interactable.EmitSignal(Interactable.SignalName.OnInteracted, this); 
    }
    
    public void Focus(Interactable interactable)
    {
        interactable.EmitSignal(Interactable.SignalName.OnFocused, this); 
    }

    public void Unfocus(Interactable interactable)
    {
        try
        {
            if (interactable != null)
            {
                interactable.EmitSignal(Interactable.SignalName.OnUnfocused, this);
            }
        }
        catch //Object disposed on load
        {
            GD.Print("Loaded new level, object disposed"); 
        }
       
    }
    
    public Interactable GetClosest()
    {
        Interactable closest = null; 
        float closeDist = float.MaxValue;
        Array<Area3D> areas = GetOverlappingAreas();
        if (areas.Count == 0)
        {
            return null;
        }
        
        foreach (Area3D area in areas)
        {
            if (area is Interactable interactable && interactable.IsInsideTree())
            {
                if (Position.DistanceTo(interactable.GlobalPosition) < closeDist)
                {
                    closeDist = Position.DistanceTo(interactable.GlobalPosition);
                    closest = interactable;
                }
            }                       
        }
        return closest; 
     
    }
     
    
}
