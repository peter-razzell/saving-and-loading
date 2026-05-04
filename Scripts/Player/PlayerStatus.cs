

using System.Collections.Generic;
using Godot;

/// <summary>
/// A class which contains Hunger, Thirst, Tiredness, Cold classes. 
/// 
/// Its _Process() method calls update methods on all these classes.
/// !Over time this may contribue to lag spikes as the logic gets more complex!!
/// </summary>
public partial class PlayerStatus : Node 
{
    [Signal]
    public delegate void OnUpdateHungerEventHandler(float value);

    [Signal]
    public delegate void OnUpdateEnergyEventHandler(float value); 

    [Signal]
    public delegate void OnUpdateWarmthEventHandler(float value); 

    [Signal]
    public delegate void OnUpdateThirstEventHandler(float value); 

    [Signal]
    public delegate void OnUpdateMaxHungerEventHandler(float value); 

    [Signal]
    public delegate void OnUpdateMaxEnergyEventHandler(float value);

    [Signal]
    public delegate void OnUpdateMaxWarmthEventHandler(float value); 

    [Signal]
    public delegate void OnUpdateMaxThirstEventHandler(float value); 

    [Signal]
    public delegate void OnDieEventHandler();

    List<PlayerStatusModifier> playerStateModifiers = new List<PlayerStatusModifier>(); 

    PlayerHunger hunger; 

    PlayerEnergy energy; 

    PlayerWarmth warmth; 

    PlayerThirst thirst; 

    float health = 100f; 

    /// <summary>
    /// This value controls the frequency in seconds at which status is updated. 
    /// </summary>
    double updateDelta = 1;

    double currentDelta;

    public PlayerStatus(PlayerData playerData, double updateDelta, float energyDecayRate, float energyMax, float startingWarmth, float maxWarmth)
    {
        this.updateDelta = updateDelta; 
        currentDelta = updateDelta; 

        hunger = new PlayerHunger(); 
        energy = new PlayerEnergy(energyMax, energyDecayRate);  
        warmth = new PlayerWarmth(startingWarmth, maxWarmth);
        thirst = new PlayerThirst(); 


        EmitSignal(SignalName.OnUpdateMaxEnergy, energyMax); //100 //TODO make Max values consistent 
        EmitSignal(SignalName.OnUpdateMaxHunger, hunger.GetMaxHunger()); //100f
        EmitSignal(SignalName.OnUpdateMaxWarmth, maxWarmth); //100
        EmitSignal(SignalName.OnUpdateMaxThirst, thirst.GetMaxThirst()); //100
        

        playerStateModifiers.Add(new PlayerStatusHungerModifier());      
    }

    public override void _Ready()
    {
        base._Ready();
    }

    
    public override void _Process(double delta)
    {
        UpdatePlayerStateDelay(delta);

        base._Process(delta);
    }
    

    public void UpdatePlayerStateDelay(double delta)
    {
        currentDelta -= delta; 

        if(currentDelta < 0)
        {
            UpdatePlayerStateDelta();
            currentDelta = updateDelta; 
        }
    }

    
    void UpdatePlayerStateDelta()
    {
        hunger.Updatehunger(); //Clamped between 0.1f and 1f / 100 - each 1 = 30 calories? - 1000-100 ticks. 

        energy.UpdateEnergy(); //-0.2 ([Export] of PlayerData) every tick / 100 - 500 seconds to 0. In game 8hrs 20m

        warmth.UpdateWarmth(); //+temp/10 every tick. 

        thirst.UpdateThirst(); //-0.2 every tick. / 100 - 500 seconds to 0. In game 8 hours 20 minutes



        UpdateHealth(); 
        
        EmitSignal(SignalName.OnUpdateHunger, hunger.GetCalories());     
        EmitSignal(SignalName.OnUpdateEnergy, energy.GetEnergy());   
        EmitSignal(SignalName.OnUpdateWarmth, warmth.GetWarmth());
        EmitSignal(SignalName.OnUpdateThirst, thirst.GetCurrentThirst()); 
       
    }

    public void OnDeath()
    {
        GD.Print("Player has died. Game over. Delegate works!!");
        //call the OnDie event, which can be subscribed to by other classes (e.g. PlayerData) to handle player death. 
    }

    public void EatFood(float calories)
    {
        hunger.EatFood(calories); 
        
    }

    public void Sleep(int hours)
    {
        
        for(int i = 0; i < hours*100; i++)
        {
            hunger.Updatehunger();
        }

        energy.Sleep(); 
        //should I add 80 to the game time?        
    }

    void UpdateHealth()
    {
        if (hunger.GetCalories() <= 0 || energy.GetEnergy() <= 0 || warmth.GetWarmth() <= 0 || thirst.GetCurrentThirst() <= 0 )
        {
            health -= 0.1f; 
        }

        if (health <= 0){
            health = 0; 
            OnDieEventHandler die = new OnDieEventHandler(OnDeath); //call the OnDie event, which can be subscribed to by other classes (e.g. PlayerData) to handle player death. 
            die.Invoke();
        }  
    }

}