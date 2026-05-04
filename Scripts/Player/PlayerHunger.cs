using System;
using Godot;

//Contained within PlayerStatus 
public class PlayerHunger
{
    /// <summary>
    /// Starting calories rather than max calories. 
    /// </summary>
    float Calories = 100f;

    float MaxHunger = 100f;

    // float currentMovementSpeed; 

    public PlayerHunger()
    {
        // currentMovementSpeed = Player.Instance.GetSpeed(); //speed of 10 results in a vector length of 7
    }

    /// <summary>
    /// Called by PlayerStatus in its update method 
    /// </summary>
    public void Updatehunger()
    {
        float movement = Player.Instance.Velocity.Length();
        float height_change = Player.Instance.GetHeightVector(); 

        float mov_component = Mathf.Tan(movement)/(Mathf.Pi/2); //This function clamps range between 0 and 1. 
        // for 7 movement length this produces a component of about 0.5

        //clamping result of hunger calculation between 0.1f and 1f. 
        float movement_multiplier_hunger = Mathf.Clamp(0.1f * mov_component + 2f * height_change, 0.1f, 1f);
        GD.Print("calories", Calories); 

        Calories -= movement_multiplier_hunger; 
            
    }

    public void EatFood(float foodCalories)
    {
        Calories += foodCalories; 
        
    }

    public float GetCalories()
    {
        return Calories; 
    }

    public float GetMaxHunger()
    {
        return MaxHunger; 
    }
    

}