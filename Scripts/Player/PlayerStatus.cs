

using System.Collections.Generic;
using Godot;

//A wrapper for hunger, thirst, cold, tired status trackers. 
public partial class PlayerStatus : Node 
{
    //every physics tick, player state is updated

    //each player state has a base decay rate, which is modified by modifiers 

    //playerstat has a list of modifiers. 

    //could be a modifier object?

    //every time the game is loaded, the player's state is loaded 

    [Signal]
    public delegate void OnUpdateHungerEventHandler(float value);

    [Signal]
    public delegate void OnUpdateEnergyEventHandler(float value); 

    [Signal]
    public delegate void OnUpdateMaxHungerEventHandler(float value); 

    [Signal]
    public delegate void OnUpdateMaxEnergyEventHandler(float value);


    [Signal]
    public delegate void OnDieEventHandler();

    List<PlayerStatusModifier> playerStateModifiers = new List<PlayerStatusModifier>(); 

    //This is the CONSTRUCTOR - CALLED A "PRIMARY CONSTRUCTOR"??
    float thirst , cold;

    PlayerHunger hunger; 

    PlayerEnergy energy; 


    float health = 100f; 

    //This value controls the frequency at which player state is updated 
    double updateDelta = 1;

    double currentDelta;

    public override void _Ready()
    {
        base._Ready();
    }
    
    public PlayerStatus(PlayerData playerData, double updateDelta, float energyDecayRate, float energyMax)
    {
        this.updateDelta = updateDelta; 
        currentDelta = updateDelta; 

        hunger = new PlayerHunger(playerData); 
        energy = new PlayerEnergy(playerData, energyMax, energyDecayRate);  

        EmitSignal(SignalName.OnUpdateEnergy, energyMax);
        EmitSignal(SignalName.OnUpdateMaxHunger, 2000f); //TODO make a max hunger! 


        //default constructor, if no values are given, set to 100. 
        
        this.thirst = 100; 
        this.cold = 100; 
        playerStateModifiers.Add(new PlayerStatusHungerModifier()); 


     
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

    
    public void UpdatePlayerStateDelta()
    {
        GD.Print("updating player state"); 
        float thirstDecayRate = 0.1f, coldDecayRate = 0.1f;

        hunger.Updatehunger();

        energy.UpdateEnergy(); 
        
        //Might not do it like this
        // foreach(PlayerStatusModifier m in playerStateModifiers)
        // {
            // if(m is PlayerStatusHungerModifier)
            // {
            //     hungerDecayRate += m.GetAmount(); //add the modifier value to the decay rate
            // } 
        // }

        thirst -= thirstDecayRate; //! Get rid of and replace with objects for each need! 
        cold -= coldDecayRate; //! Get rid of and replace with objects for each need! 
       
        //! Get rid of and replace with objects for each need! 
        if (thirst < 0){            
            thirst = 0; 

            health -= 0.1f; //if thirst is 0, health decreases.
        }
       
        if (cold < 0){            
           cold = 0; 

           health -= 0.1f; //if cold is 0, health decreases.
        }

        if (health < 0){
            health = 0; 
            OnDieEventHandler die = new OnDieEventHandler(OnDeath); //call the OnDie event, which can be subscribed to by other classes (e.g. PlayerData) to handle player death. 
            die.Invoke();
        }  

        EmitSignal(SignalName.OnUpdateHunger, hunger.GetCalories());     
        EmitSignal(SignalName.OnUpdateEnergy, energy.GetEnergy());   
       
    }

    public void OnDeath()
    {
        GD.Print("Player has died. Game over. dELEGATE WORKS!");
        //call the OnDie event, which can be subscribed to by other classes (e.g. PlayerData) to handle player death. 
    }

    public void EatFood(float calories)
    {
        hunger.EatFood(calories); 
        
    }




}