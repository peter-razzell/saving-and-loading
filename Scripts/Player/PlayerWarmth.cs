
using System.Security.Cryptography;
using Godot;

public class PlayerWarmth
{
    //Tied to weather - this long dark console commands list reveals how weather is done https://the-long-dark-modding.fandom.com/wiki/Console_commands#Usage

    /// <summary>
    /// Temperature where the player is.
    /// </summary>
    float currentTemperature; 

    /// <summary>
    /// starting warmth amount
    /// </summary>
    float startingWarmth;

    /// <summary>
    /// maximum warmth possible
    /// </summary>
    float maxWarmth; 

    /// <summary>
    /// how warm or cold the player is currently. If this reaches zero, health begins to decrease like TLD. 
    /// </summary>
    float currentWarmth; 

    WeatherParameter weatherParameter;

    

    public PlayerWarmth(float startingWarmth, float maxWarmth)
    {
        this.startingWarmth = startingWarmth;
        currentWarmth = startingWarmth; 
        this.maxWarmth = maxWarmth; 
    }

    /// <summary>
    /// Updates the player's warmth by the current temperature / 10. 
    /// PlayerWarmth also has a currentTemperature value. 
    /// </summary>
    public void UpdateWarmth()
    {
        WeatherSwitchBus weatherSwitchBus = WeatherSwitchBus.GetInstance(); 

        WeatherParameter weatherParameter = weatherSwitchBus.GetCurrentWeatherParameters();

        currentTemperature = weatherParameter.GetTemperature(); 

        currentWarmth += currentTemperature / 10; // -1f/10 = -0.1f --> 

        if(currentWarmth < 0) currentWarmth = 0; 

        else if(currentWarmth > maxWarmth) currentWarmth = maxWarmth; 

        GD.Print("current temp = ", currentTemperature, " current warmth = ", currentWarmth); 
    }

    public float GetWarmth()
    {
        return currentWarmth; 
    }

   
}