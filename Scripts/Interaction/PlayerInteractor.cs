
using Godot;

public partial class PlayerInteractor : Interactor
{
    Interactable cached;

    [Signal]
    public delegate void OnAddToPlayerInventoryEventHandler(Interactable interactable); 

    //Could be made more efficient - e.g. don't check every physics tick
    public override void _PhysicsProcess(double delta)
    {
        Interactable close = GetClosest();

        if (close != null && close != cached)
        {
            Focus(close);
            if (cached != null)
            {
                Unfocus(cached);
            }
            cached = close;
        }
        else if (close == null && cached != null)
        {
            Unfocus(cached);
            cached = null;
        }

        base._PhysicsProcess(delta);
    }

    public new void Interact(Interactable interactable)
    {
        if(interactable.GetParent() is InteractablePickup) //works like a dream! 
        {
            EmitSignal(SignalName.OnAddToPlayerInventory, interactable); 
        }
        else if(interactable.GetParent() is LevelExit)
        {
            // No logic to handle, level exiting, saving and loading etc is handled in LevelExit. 
        }

        base.Interact(interactable); 
    }
    public override void _Input(InputEvent @event)
    {

        if (@event.IsActionPressed("player_interact") && cached != null)
        {
            Interact(cached);
        }
        base._Input(@event);
    }
    
    public void ResetOnLevelLoad()
    {
        cached = null; 
    }
}