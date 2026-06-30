using Godot; 

/// <summary>
/// This class will contain the parameters of the weather. 
/// WeatherState (or weather manager?) will contain a current / next version of this to tween between values.
/// </summary>
public partial class WeatherParameter : Node
{
    [Export]
    float temperature; 

    WorldEnvironment weatherEnvironmentParams; // interpolate between the current environment and the parameters of this environment.
    //Never actually change the environment that is loaded, just interpolate parameters. 

    public WeatherParameter(float temperature)
    {
        this.temperature = temperature; 
    }

    public float GetTemperature()
    {
        return temperature; 
    }
    
}