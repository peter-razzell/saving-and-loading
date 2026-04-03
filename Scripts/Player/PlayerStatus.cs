

using System.Collections.Generic;
using Godot;

public class PlayerStatus
{
    //every physics tick, player state is updated

    //each player state has a base decay rate, which is modified by modifiers 

    //playerstat has a list of modifiers. 

    //could be a modifier object?

    //every time the game is loaded, the player's state is loaded 

    public delegate void OnDie();

    List<PlayerStatusModifier> playerStateModifiers = new List<PlayerStatusModifier>(); 

    //This is the CONSTRUCTOR - CALLED A "PRIMARY CONSTRUCTOR"??
    float hunger , thirst , cold;

    // float hungerDecayRate = 0.1f, thirstDecayRate = 0.1f, coldDecayRate = 0.1f;

    float health = 100f; 

    
    public PlayerStatus()
    {
        //default constructor, if no values are given, set to 100. 
        this.hunger = 100; 
        this.thirst = 100; 
        this.cold = 100; 
        playerStateModifiers.Add(new HungerModifier()); //1 hunger modifier! 
    }

    
    public void UpdatePlayerState()
    {
        float hungerDecayRate = 0.1f, thirstDecayRate = 0.1f, coldDecayRate = 0.1f;

        //for each modifier, apply the modifier to the relevant stat.

        foreach(PlayerStatusModifier m in playerStateModifiers)
        {
            if(m is HungerModifier)
            {
                hungerDecayRate += m.GetAmount(); //add the modifier value to the decay rate
            }
            //  else if(m is ThirstModifier)
            // {
            //     thirstDecayRate += m.GetAmount(); 
            // }
            //  else if(m is ColdModifier)
            // {
            //     coldDecayRate += m.GetAmount(); 
            // }
        }

        hunger -= hungerDecayRate;
        thirst -= thirstDecayRate;
        cold -= coldDecayRate;
              
        GD.Print("hunger = "+hunger+", thirst = "+thirst+", cold = " + cold, "health = " + health); 

        if(hunger < 0)
        {            
            hunger = 0; 

            health -= 0.10f; //if hunger is 0, health decreases.
        }
       
        if (thirst < 0)
        {            
            thirst = 0; 

            health -= 0.1f; //if thirst is 0, health decreases.
        }
       
        if (cold < 0)
        {            
           cold = 0; 

           health -= 0.1f; //if cold is 0, health decreases.
        }
       

        if (health < 0)
        {
            health = 0; 
            OnDie die = new OnDie(OnDeath); //call the OnDie event, which can be subscribed to by other classes (e.g. PlayerData) to handle player death. 
            die.Invoke();
            //player dies. 
        }
       
    }

    public void OnDeath()
    {
        GD.Print("Player has died. Game over. dELEGATE WORKS!");
        //call the OnDie event, which can be subscribed to by other classes (e.g. PlayerData) to handle player death. 
    }




}