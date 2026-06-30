

using System;
using Godot;

[GlobalClass]
[Tool]
public partial class RegionNodeData : Resource
{
    [Export]
    public Godot.Collections.Array<PackedScene> regionData = new Godot.Collections.Array<PackedScene>();

    public RegionNodeData(Godot.Collections.Array<PackedScene> nodes)
    {
        regionData = nodes; 
    }

    
    public RegionNodeData()
    {
        
    }

    public Godot.Collections.Array<PackedScene> GetScenes()
    {
        foreach(PackedScene p in regionData)
        {
            GD.Print(p);

        }
        return regionData; 
    }


}
