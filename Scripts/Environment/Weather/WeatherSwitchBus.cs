using System.Runtime.CompilerServices;
using System.Threading;
using Godot;

public partial class WeatherSwitchBus : Node
{
    static WeatherSwitchBus Instance; 

    WeatherManager WeatherManager; 
    // Signals

    [Signal]
    public delegate void OnSwitchWeatherEventHandler(); 

    public static WeatherSwitchBus GetInstance()
    {
        if(Instance == null)
        {
            Instance = new WeatherSwitchBus();        
            
        }
        return Instance; 
    }

    public void RegisterWeatherManager(WeatherManager weatherManager)
    {
        WeatherManager = weatherManager; 
    }

    public void SwitchWeatherMessage()
    {
        EmitSignal(SignalName.OnSwitchWeather); 
    }

    public void UpdateLevelWeatherState(string regionID)
    {
        WeatherManager.UpdateLevelWeatherState(regionID);
    }

    public WeatherParameter GetCurrentWeatherParameters()
    {
        return WeatherManager.getCurrentWeatherParams();
        
    }

   

}