using System;
using Godot;

public partial class SavedData: Resource
{
    [Export]
    public Vector3 position;

    [Export]
    public String scenePath; 
    
}