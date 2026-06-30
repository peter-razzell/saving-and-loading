


using System.Collections.Generic;
using Godot;

/// <summary>
/// Allows us to get the level weather states object that corresponds to a specific class. 
/// 
/// Contains the data for the region weather states themselves. 
/// </summary>
public static class RegionWeatherStatesLookup
{

    static List<WeatherState> OverWorldStates = new List<WeatherState>
    {
        new WeatherState(EWeatherState.Sunny, new WeatherParameter(-10f)),
        new WeatherState(EWeatherState.Cloudy, new WeatherParameter(-5f)),
        new WeatherState(EWeatherState.Cold, new WeatherParameter(2f))
    }; 

    /// <summary>
    /// TEST of switching between weather states as a region changes.
    /// </summary>
    static List<WeatherState> Level0States = new List<WeatherState>
    {
        new WeatherState(EWeatherState.Cold, new WeatherParameter(-20f)),
        new WeatherState(EWeatherState.Cloudy, new WeatherParameter(-15f))
    }; 

    static Dictionary<string, RegionWeatherStates> keyValuePairs = new Dictionary<string, RegionWeatherStates>
    {
        {"Overworld", new RegionWeatherStates(OverWorldStates)},
        {"Test", new RegionWeatherStates(Level0States)}
    }; 

    public static RegionWeatherStates GetLevelWeatherStatesWithID(string LevelID)
    {
        try
        {
            return keyValuePairs[LevelID]; 

        }
        catch
        {
            GD.Print($"WEATHER: no state for {LevelID}");

            return new RegionWeatherStates(new List<WeatherState>(){new WeatherState(EWeatherState.Error, new WeatherParameter(100f))}); 
        }
    }
    
}