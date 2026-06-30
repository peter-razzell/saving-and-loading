

using Godot;

public partial class NodeRegionsCreator : Node
{
    
    public Godot.Collections.Dictionary<int, Godot.Collections.Array<Node>> SaveAndClearOriginalNodes(Godot.Collections.Array<Node> children, GodotObject terrainData)
	{
		//Save original nodes in a resource file so they can be reloaded later if needed. 
		Godot.Collections.Dictionary<int, Godot.Collections.Array<PackedScene>> packedNodesByRegion = new Godot.Collections.Dictionary<int, Godot.Collections.Array<PackedScene>>();

        Godot.Collections.Dictionary<int, Godot.Collections.Array<Node>> nodesByRegion = new Godot.Collections.Dictionary<int, Godot.Collections.Array<Node>>(); 

		foreach(Node3D child in children)
		{
			int regionID = (int) terrainData.Call("get_region_idp", child.Position); 

			PackedScene packedScene = new PackedScene();
			packedScene.Pack(child);

			if(packedNodesByRegion.TryGetValue(regionID, out Godot.Collections.Array<PackedScene> value))
			{
				GD.Print($"adding {packedScene.ResourceName} to node region {regionID}");
				value.Add(packedScene);
			} 
			else packedNodesByRegion.Add(regionID, [packedScene]);

            if(nodesByRegion.TryGetValue(regionID, out Godot.Collections.Array<Node> nodeValue))
            {
                nodeValue.Add(child); 
            }


		}

		foreach(int key in packedNodesByRegion.Keys)
		{
			RegionNodeData regionNodeData = new RegionNodeData(packedNodesByRegion[key]);

			DirAccess dirAccess = DirAccess.Open("user://");
			dirAccess.Remove($"region_node_data_{key}.tres"); 

			ResourceSaver.Save(regionNodeData, $"user://region_node_data_{key}.tres", ResourceSaver.SaverFlags.None); 
		}		

        return nodesByRegion; 
	}

}