

using System;
using System.Collections.Generic;
using System.IO;
using Godot;

[Tool]
public partial class AssetDataBaseCreator : Node3D
{
    Dictionary<int, Godot.Collections.Array<StreamableObject>> streamablesByRegion = new Dictionary<int, Godot.Collections.Array<StreamableObject>>(); 
    public Dictionary<int, Godot.Collections.Array<StreamableObject>> CreateDatabase(Godot.Collections.Array<Node> children, GodotObject terrainData)
	{
		
		//Children of the asset stream manager - to be populated into streamablesByRegion

		//Unique nodes by region - this will have 1 copy of each node type in each region - to check against when 
		//either creating a new MMI, or adding a transform to it's record as a streamable object. 

		//Iterate through every child node of the AssetStreamManager
		foreach(Node3D node3D in children)
		{
			//Get the region id and Vector2I region coords for the child node 				
			int regionID = (int)terrainData.Call("get_region_idp", node3D.GlobalPosition);
			Vector2I regionLocation =  (Vector2I) terrainData.Call("get_region_location", node3D.GlobalPosition); 

			Godot.Collections.Array<Mesh> meshes = GenerateMeshArrayForSaving(node3D);

			if (!streamablesByRegion.ContainsKey(regionID))
			{
				streamablesByRegion.Add(regionID, new Godot.Collections.Array<StreamableObject>()); 
			}
			Godot.Collections.Array<StreamableObject> regionStreamables = streamablesByRegion[regionID];

			string assetID = null; 
			if (node3D.HasMeta("type_id"))
			{
				assetID =  node3D.GetMeta("type_id").AsString();
			}
			else  
			{
				assetID = node3D.Name; 
				GD.Print($"[Warning] Node '{node3D.Name}' lacks type_id metadata and a valid SceneFilePath. Falling back to Node Name.");
			} 

			StreamableObject existingStreamable = null;

			foreach(StreamableObject streamable in regionStreamables)
			{
				if(streamable.assetID == assetID)
				{
					existingStreamable = streamable; 
					existingStreamable.AddTransform(node3D.Transform);
					break; 
				}
			}

			if(existingStreamable == null)
			{
				// GD.Print($"Creating brand new database entry for: {assetID}");

				StreamableObject streamableObject = new StreamableObject(assetID, meshes, new Godot.Collections.Array<Transform3D>() { node3D.Transform });
				regionStreamables.Add(streamableObject); 
			}
		}
		
		//Save to a WorldSpaceData resource.
		// SaveStreamablesByRegionToDisk();

		//Delete the unstreamed original nodes, now that the dictionary has been populated and saved to a resource. 
		// SaveAndClearOriginalNodes(children, terrainData); 	

        return streamablesByRegion; 	
	}

	/// <summary>
	/// Helper method for CreateDatabase - gets the mesh array from the Node3D - strips out everything we don't need so loading is smooth
	/// 
	/// COULD be a source of the bug which replaces meshes with other meshes? 
	/// </summary>
	/// <param name="node3D"></param>
	/// <returns></returns>
	Godot.Collections.Array<Mesh> GenerateMeshArrayForSaving(Node3D node3D)
	{
		Godot.Collections.Array<Mesh> returnArr = new Godot.Collections.Array<Mesh>();
		Godot.Collections.Array<Node> components = node3D.GetChildren();

		foreach(Node component in components)
		{
			if(component is MeshInstance3D meshComponent)
			{
				returnArr.Add(meshComponent.Mesh); 
			}
		}
		return returnArr; 
		
	}

	
	/// <summary>
	/// Called from CreateDatabase
	/// 
	/// Saves original nodes to a separate scene file so they can be reloaded later on if any changes need to be made to the scene. 
	/// 
	/// Deletes original separate mesh instance nodes from scene 
	/// </summary>
	
    
}
