
using System.Threading.Tasks;
using Godot;

/// <summary>
/// Controls a UI scene of the same name, instantiated when the player interacts with a bed.
/// 
/// Instantiates a black ColorRect and Text, waits for 5 seconds, and disappears.
/// 
/// </summary>
public partial class OverlaySleepEffect: Control
{
    [Export]
    float sleepLengthInSeconds; 
    Timer sleepTimer;

    double startTime, endTime; 
    public override void _Ready()
    {

        base._Ready();

    }

    public async Task MakeVisible()
    {
        Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;

        Visible = true; 

        sleepTimer = new Timer(); 

        startTime = Game.Instance.Time; 
    
        AddChild(sleepTimer); 

        sleepTimer.Start(sleepLengthInSeconds);
        sleepTimer.Timeout += EndSleep; 

    }

    public void EndSleep()
    {

        endTime = Game.Instance.Time; 
        Visible = false;
        GD.Print("start sleep time", startTime, "end sleep time", endTime); 

        //Logic to change player condition as a result of sleep time here. 

        Input.MouseMode = Input.MouseModeEnum.Captured;

        sleepTimer.QueueFree(); 

    }

}
