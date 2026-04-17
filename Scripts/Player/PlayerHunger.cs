using Godot;

//Contained within PlayerStatus 
public class PlayerHunger
{

    float calories = 1000; //calories 
    PlayerData playerData; 

    float currentMovementSpeed; 

    public PlayerHunger(PlayerData playerData)
    {
        this.playerData = playerData; 
        currentMovementSpeed = playerData.GetPlayer().GetSpeed(); //speed of 10 results in a vector length of 7
    }

    //called by PlayerStatus - calculates hunger reduction based on speed. 
    public void Updatehunger()
    {
        float movement = playerData.GetPlayer().Velocity.Length();
        float height_change = playerData.GetPlayer().GetHeightVector(); 

        float mov_component = Mathf.Tan(movement)/(Mathf.Pi/2); //This function clamps range between 0 and 1. 
        // for 7 movement length this produces a component of about 0.5

        float movement_multiplier_hunger = Mathf.Clamp(0.1f * mov_component + 2f * height_change, 0.01f, 1f);
        GD.Print("calories", calories); 

        calories -= movement_multiplier_hunger; 
            
    }

    public void EatFood(float foodCalories)
    {
        calories += foodCalories; 
        
    }

    public float GetCalories()
    {
        return calories; 
    }
    

}