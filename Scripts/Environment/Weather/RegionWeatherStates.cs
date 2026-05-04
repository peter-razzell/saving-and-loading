

using System;
using System.Collections.Generic;
using Godot; 

/// <summary>
/// This class wraps a list of weathers which can be iterated through in a level. 
/// 
/// I want to be able to control the parameters and weather types from Export - not sure how to do this...
/// 
/// Maybe a drop down for states 
/// 
/// </summary>
public partial class RegionWeatherStates : Node
{
    List<WeatherState> weatherStates; 

    int current = 0; 

    public RegionWeatherStates(List<WeatherState> weatherStates)
    {
        this.weatherStates = weatherStates;

    }

    public void NextWeather()
    {
        if(current < weatherStates.Count - 1)
        {
            current++;
        }
        else
        {
            current = 0; 
        }
        //cycles through the states. 
        // current = current < weatherStates.Count -1 ? current ++ : 0; //My attempt at a tenary operator https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/conditional-operator 
            
    }

    /// <summary>
    /// This might be weird - I could just create a wrapper class of State and WeatherParameter to avoid this,
    /// but this way does have the advantage of changing the parameters of cloudy weather in one region vs another
    /// So a bit more control / region-specific? 
    /// </summary>
    /// <returns></returns>
    public WeatherState GetWeather()
    {
        return weatherStates[current]; 
    }


    

    

}