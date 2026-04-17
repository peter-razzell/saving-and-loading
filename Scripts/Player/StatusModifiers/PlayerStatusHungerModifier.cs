
using System;
using System.Runtime;


/// <summary>
/// An object to modify base hunger reduction in addition to / or subtracting from base amount
/// E.g. this could be for a wasting disease, or other. 
/// </summary>
class PlayerStatusHungerModifier : PlayerStatusModifier
{
    String description = "hunger"; //get strings into a separate file. 


    //Create a new hunger modifier
    public PlayerStatusHungerModifier()
    {
        type= EPlayerState.hunger; 

        amount = 1f; //also need to have this in some kind of accesible file / config settings etc. 

    }


    
}