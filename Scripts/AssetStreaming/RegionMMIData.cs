

using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// This object contains a dictionary of saved assets / locations across the game world to be streamed by the asset manager. 
/// </summary>
[Tool]
[GlobalClass]
public partial class RegionMMIData : Resource
{
    [Export]
    public Godot.Collections.Array<StreamableObject> regionData = new Godot.Collections.Array<StreamableObject>(); 

    
    public RegionMMIData()
    {
        regionData = new Godot.Collections.Array<StreamableObject>(); 
    }

    public RegionMMIData(Godot.Collections.Array<StreamableObject> regionData)
    {
        this.regionData = regionData;
    }

    public void SetRegionData(Godot.Collections.Array<StreamableObject> regionData)
    {
        this.regionData = regionData;
    }

    public Godot.Collections.Array<StreamableObject> GetRegionData()
    {
        return regionData; 
    }
}