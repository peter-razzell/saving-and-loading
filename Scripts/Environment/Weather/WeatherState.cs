
using System.Data;
using Godot; 
  public enum EWeatherState
    {
        Sunny,
        Cloudy,
        Cold,

        Error
    }

public partial class WeatherState : Node
{
    EWeatherState EState; 

    WeatherParameter Parameter; 

    public WeatherState(EWeatherState state, WeatherParameter parameter)
    {
        this.EState = state; 
        this.Parameter = parameter; 
    }
    
    public EWeatherState GetState()
    {
        return EState; 
    }

    public WeatherParameter GetWeatherParameter()
    {
        return Parameter;
    }

}