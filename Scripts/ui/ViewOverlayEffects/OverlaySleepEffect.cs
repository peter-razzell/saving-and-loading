
using System.Threading.Tasks;
using Godot;

/// <summary>
/// Controls a scene of the same name, instantiated when the player interacts with a bed.
/// 
/// </summary>
public partial class OverlaySleepEffect: Control
{
    Timer sleepTimer;

    double startTime, endTime; 
    public override void _Ready()
    {

        sleepTimer = new Timer(); 
        base._Ready();

    }

    public async Task MakeVisible()
    {
        Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;

        Visible = true; 

        startTime = Game.Instance.Time; 
    
        AddChild(sleepTimer); 

        sleepTimer.Start(5);
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
