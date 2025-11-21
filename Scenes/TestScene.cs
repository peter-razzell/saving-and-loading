using Godot;
using System;

public partial class TestScene : Node3D
{
    [Export]
    public SaverLoader saverLoader;

    public override void _Ready() {
        base._Ready();
    }
    
    public override void _Input(InputEvent @event) {

        if (@event.IsActionPressed("debug_quit"))
        {
            GetTree().Quit(); 
            
        }
        base._Input(@event);
    }
}
