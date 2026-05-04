
using System.Dynamic;

public class PlayerThirst
{

    float CurrentThirst = 100; 

    float MaxThirst = 100; 

    /// <summary>
    /// Empty constructor to default to default values
    /// </summary>
    public PlayerThirst()
    {
        
    }


    public PlayerThirst(float CurrentThirst, float MaxThirst)
    {
        this.CurrentThirst = CurrentThirst;
        this.MaxThirst = MaxThirst; 
        
    }

    //1 second = 1 minute. 60 seconds = 1 hour, 1440 seconds = 1 day
    //ticks down 0.1 every second - 100-0 in 500 seconds. e.g. just under 3 minutes. 

    //could be an "update thirst night" / sleep update with a different rate?? 
    public void UpdateThirst()
    {
        CurrentThirst -= 0.2f; 
    }

    public float GetMaxThirst()
    {
        return MaxThirst;
    }
    
    public float GetCurrentThirst()
    {
        return CurrentThirst; 
    }
    

}