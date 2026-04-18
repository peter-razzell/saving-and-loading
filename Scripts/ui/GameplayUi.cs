using Godot;
using System;

public partial class GameplayUi : Control
{

    [Export]
    TextureProgressBar hunger;

    [Export]
    TextureProgressBar energy; 



    public void UpdateHungerBar(float value)
    {
        hunger.Value = value; 
    }

    public void SetHungerMax(float value)
    {
        hunger.MaxValue = value; 
    }

    public void UpdateEnergyBar(float value)
    {
        // GD.Print("setting energy value to", value); 
        energy.Value = value; 
    }

    public void SetEnergyMax(float value)
    {
        energy.MaxValue = value; 
        
    }


}
