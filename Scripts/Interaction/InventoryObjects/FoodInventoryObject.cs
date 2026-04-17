using Godot; 

public partial class FoodInventoryObject : InventoryObject
{
    //not necessary? 
    [Signal]
    public delegate void OnFoodEatenEventHandler(float calories); 

    [Export]
    public float calories = 100; 


    public new void OnObjectInteract()
    {
        GD.Print("Food object eaten"); 

        EmitSignal(SignalName.OnFoodEaten, calories);

        if (removeOnInteract)
        {
            EmitSignal(InventoryObject.SignalName.OnInteractDisappear, pickupID); 
        }
        

    }



}