using Godot;
using System;
using System.Security;

public partial class GameplayUi : Control
{

    [Export]
    TextureProgressBar hunger;

    [Export]
    TextureProgressBar energy; 

    [Export]
    TextureProgressBar warmth; 

    [Export]
    TextureProgressBar thirst; 

    [Export]
    Label hungerLabel, energyLabel, warmthLabel, thirstLabel; 







    public void UpdateHungerBar(float value)
    {
        hunger.Value = value; 
        hungerLabel.Text = value.ToString("n2"); 
    }

    public void SetHungerMax(float value)
    {
        hunger.MaxValue = value; 
    }

    public void UpdateEnergyBar(float value)
    {
        energy.Value = value; 
            energyLabel.Text = value.ToString("n2"); 

    }

    public void SetEnergyMax(float value)
    {
        energy.MaxValue = value; 
        
    }

    public void UpdateWarmthBar(float value)
    {
        warmth.Value = value;
                    warmthLabel.Text = value.ToString("n2"); 

    }

    public void SetWarmthMax(float value)
    {
        warmth.MaxValue = value; 
    }

    public void UpdateThirstBar(float value)
    {
        thirst.Value = value;
                            thirstLabel.Text = value.ToString("n2"); 

    }

    public void SetThirstMax(float value)
    {
        thirst.MaxValue = value; 
    }


}
