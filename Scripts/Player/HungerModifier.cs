
using System;
using System.Runtime;

class HungerModifier : PlayerStatusModifier
{
    String description = "hunger"; //get strings into a separate file. 


    public HungerModifier()
    {
        type= EPlayerState.hunger; 

        amount = 1f; //also need to have this in some kind of accesible file / config settings etc. 

    }


    
}