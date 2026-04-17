using System; 
using Godot; 

public class PlayerEnergy
{
    float maxEnergy;
    
    float energy; 

    float decayRate; 

    PlayerData playerData; 

    public PlayerEnergy(PlayerData playerData, float energy, float decayRate)
    {
        this.playerData = playerData;  
        this.energy = 100f;
        this.decayRate = decayRate; 
        this.maxEnergy = energy; 
    }

    //Called by PlayerStatus update
    public float UpdateEnergy()
    {
        energy -= decayRate; 

        GD.Print("energy", energy); 

        // float movement = playerData.GetPlayer().Velocity.Length();
        // float height_change = playerData.GetPlayer().GetHeightVector(); 

        // float mov_component = Mathf.Tan(movement)/(Mathf.Pi/2); //This function clamps range between 0 and 1. 
        // // for 7 movement length this produces a component of about 0.5

        // float movement_multiplier_hunger = Mathf.Clamp(0.1f * mov_component + 2f * height_change, 0.01f, 1f);
        // GD.Print("energy: ", energy); 

        // energy -= movement_multiplier_hunger; 

        return energy; 
    }

    public float GetEnergy()
    {
        return energy; 
    }



}