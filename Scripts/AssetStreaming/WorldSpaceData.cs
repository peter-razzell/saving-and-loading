

using System.Collections.Generic;
using Godot;

[Tool]
public partial class WorldSpaceData : Resource
{
    [Export]
    public Godot.Collections.Dictionary<int, RegionMMIData> worldSpaceData = new Godot.Collections.Dictionary<int, RegionMMIData>();

    public WorldSpaceData()
    {
        worldSpaceData = new Godot.Collections.Dictionary<int, RegionMMIData>(); 
    }
    public WorldSpaceData(Godot.Collections.Dictionary<int, RegionMMIData> worldSpaceData)
    {
        this.worldSpaceData =  worldSpaceData; 
    }

    public Godot.Collections.Dictionary<int, RegionMMIData> GetWorldSpaceData()
    {
        return worldSpaceData; 
    }

    public void AddRegion(int key, RegionMMIData regionData)
    {
        worldSpaceData.Add(key, regionData); 
    }
}