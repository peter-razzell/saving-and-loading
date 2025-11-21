
using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections; 

public partial class SavedGame: Resource
{
    //Current level string - so that the correct level is loaded 
    [Export]
    public String level;

    //Array of levels data - one SavedData item per level 
    [Export]
    public Godot.Collections.Dictionary<String, Array<SavedData>> levelsData; 

    [Export]
    public Image viewportImage;

    //Player attributes 
    [Export]
    public int playerHealth;

    [Export]
    public Vector3 playerPos;

    [Export]
    public Array<Variant> playerInv = [];

    [Export]
    public Vector3 playerHeadRot;

    [Export]
    public Vector3 playerCamRot; 
    
}