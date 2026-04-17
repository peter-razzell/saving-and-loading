

using Godot;

/// <summary>
/// Manager class to link all player classes to all non-player classes. Every signal should pass through here to get in or out of player. 
/// </summary>
public partial class PlayerManager : Node
{
    public Game game = Game.Instance; //Instance of the game. Will this crash on load if game hasn't been instanced yet? 

    public override void _Ready()
    {
        if(game == null)
        {
            game = Game.Instance; 
        }
        base._Ready();
    }




    

}