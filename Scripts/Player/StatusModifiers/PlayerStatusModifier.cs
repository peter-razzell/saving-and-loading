public class PlayerStatusModifier
{
    //holds the modifier

    //rate multiplier, what it is modifiying. 

    //or subclases for each 

    protected EPlayerState type; 

    protected float amount; 


    public PlayerStatusModifier(EPlayerState type = EPlayerState.idle) //idle is default. 
    {
        this.type = type; 
    } 

    public float GetAmount()
    {
        return amount; 
    }

    
}