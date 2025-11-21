using Godot;
using System;

//This node is part of level exit scene - entrances and exits are combined
public partial class LevelEntrance : Node3D
{
    //A reference to this is a member variable of LevelExit, but this is still marked [Export] in rare cases of instantiating this on its own
    [Export]
    public String levelEntranceID; //ID corresponds to the parameter of the same name in LevelExit. 
}
