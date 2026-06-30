using System;
using System.ComponentModel;
using Godot;

/// <summary>
/// Weather manager class. I'm using https://www.youtube.com/watch?v=UNP5wEqLKmM as a guide. 
/// </summary>
public partial class WeatherManager: Node
{
    /// <summary>
    /// The list of weather states in this environment / map region. 
    /// </summary>
    RegionWeatherStates levelWeatherStates;

    /// <summary>
    /// The current weather state - bundles parameters and a state enum. 
    /// USELESS?
    /// </summary>
    WeatherState currentWeatherState; 

    /// <summary>N
    /// The actual parameters currently being displayed.
    /// </summary>
    WeatherParameter currentParameters; 

    /// <summary>
    /// The parameters of the next weather - if different to current parameters, will tween/lerp towards. 
    /// </summary>
    WeatherParameter nextParameters; 

    [Export]
    Timer switchTimer;

    [Export]
    float DebugWeatherSwitchTime = 10f; 

    public override void _Ready()
    {

        WeatherSwitchBus.GetInstance().OnSwitchWeather += NextWeather; 

        WeatherSwitchBus.GetInstance().RegisterWeatherManager(this); 

        SetTimer(); 

        if(switchTimer != null) switchTimer.Timeout += NextWeather; 

        base._Ready();


    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    void SetTimer()
    {
        if (switchTimer == null)
        {
            return;
        }
        switchTimer.WaitTime = DebugWeatherSwitchTime; 
        switchTimer.Start(); 
        // switchTimer.Timeout += NextWeather; //need to get rid of this after switching weather  
       
    }

    void UnsetTimer()
    {
        switchTimer.Timeout -= NextWeather;
        // switchTimer.QueueFree();          
    }

    public void NextWeather()
    {
        if(switchTimer != null)
        {
            // UnsetTimer(); 
        }

        // GD.Print("changing weather"); 
        levelWeatherStates.NextWeather();

        currentWeatherState = levelWeatherStates.GetWeather(); 

        // GD.Print("weather is now,", currentWeatherState.GetState());

        SetTimer();  
    }

    public void SetParameters(WeatherParameter weatherParameter)
    {
        currentParameters = weatherParameter; 
    }

    public WeatherParameter getCurrentWeatherParams()
    {
        if(currentWeatherState != null)
        {
            return currentWeatherState.GetWeatherParameter(); 
        }
        return new WeatherParameter(-100f); // default
    }

    /// <summary>
    /// This method is called from the _Ready() method of the Level class. 
    /// </summary>
    /// <param name="regionID"></param>
    public void UpdateLevelWeatherState(string regionID)
    {
        levelWeatherStates = RegionWeatherStatesLookup.GetLevelWeatherStatesWithID(regionID); 
        //change level weather states. 

    }
    
}