using System; 
using Godot; 

public class PlayerEnergy
{
    float maxEnergy;
    
    float energy; 

    float decayRate; 

    public PlayerEnergy(float energy, float decayRate)
    {
        this.energy = energy;
        this.decayRate = decayRate; 
        this.maxEnergy = energy; 
    }

    //Called by PlayerStatus update
    public float UpdateEnergy()
    {
        GD.Print("decay rate is:", decayRate); //for some reason decay rate is 1! Even though it should be 0.1? 
        energy -= 0.1f; 

        return energy; 
    }

    public float GetEnergy()
    {
        return energy; 
    }

    public void Sleep()
    {
        energy = maxEnergy; 
        GD.Print("energy has been restored while sleeping, energy = ", energy); 
    }



}