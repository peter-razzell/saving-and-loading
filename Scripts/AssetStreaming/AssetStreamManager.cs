using Godot;
using Godot.Collections;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

/// <summary>
/// Asset Stream Manager 
/// Intended to stream assets for the open world. Serialised assets based on region location.
/// </summary>
[Tool]
public partial class AssetStreamManager : Node3D
{
	[Export]
	string nodeRegionFileName = "region_node_data_"; 
	
	[Export]
	string MMIRegionFileName = "region_MMI_data_"; 
	/// <summary>
	/// current region position for editor - used to determine which assets to load into the editor window.
	/// </summary>
	int currentRegionPosition; 

	/// <summary>
	/// Controls whether the assets are streamed from the database OR editable nodes are used in editor. 
	/// 
	/// true = in edit mode
	/// false = in streaming mode
	/// </summary>
	bool editMode = true;

	Camera3D activeCamera; 

	bool bakedDatabase = false; 

	private bool _createDatabase; 
	[Export]
	bool createDatabase 
	{
		get => _createDatabase; set
		{
			if(_createDatabase != value)
			{
				CreateDatabase(value); 
				_createDatabase = false; 
			}
		}
	}

	private bool _reloadOriginalNodeData; 
	[Export]
	bool reloadOriginalNodeData 
	{
		get => _reloadOriginalNodeData; set
		{
			if(_reloadOriginalNodeData != value)
			{
				ReloadAllOriginalNodeData(); 
				_reloadOriginalNodeData = false; 
			}
		}
	}

	/// <summary>
	/// list of currently loaded assets used for keeping track of what is in the scene and where
	/// </summary>
	System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<string, Array<MultiMeshInstance3D>>> currentlyLoadedAssetsByRegion
	 = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<string, Array<MultiMeshInstance3D>>>();

	/// <summary>
	/// ONLY saving to and loading from -- SHOULD NOT be used to check what's loaded! 
	/// </summary>
	System.Collections.Generic.Dictionary<int, Array<StreamableObject>> streamablesByRegion = 
	new System.Collections.Generic.Dictionary<int, Array<StreamableObject>>(); 

	Godot.Collections.Dictionary<int, Array<Node>> NodesByRegion = new Godot.Collections.Dictionary<int, Array<Node>>(); 

	GodotObject terrain3D, terrainData;  

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetActiveCam(); 
		GetTerrain();
		if (!Engine.IsEditorHint())
		{
			// GD.Print("setting edit mode to false - so should LOAD!"); 
			editMode = false; 

			KillAllChildren();//clears the decks - in case any meshinstances have accidentally been saved somehow! 
		}
	}

	#region // CREATE DATABASE -------------------------------------------------------------------

	void CreateDatabase(bool createDatabase)
	{
		AssetDataBaseCreator assetDataBaseCreator = new AssetDataBaseCreator();
		
		editMode = false; //reminder - editMode = working with nodes. false = working with stremableObjects

		streamablesByRegion = assetDataBaseCreator.CreateDatabase(GetChildren(), terrainData);

		//Save to a WorldSpaceData resource.
		SaveStreamablesByRegionToDisk();

		NodeRegionsCreator nodeRegionsCreator = new NodeRegionsCreator();
		NodesByRegion = nodeRegionsCreator.SaveAndClearOriginalNodes(GetChildren(), terrainData); 	

		GD.Print(NodesByRegion.Count); 

		KillAllChildren(); 

			
	}

	/// <summary>
	/// Helper method for CreateDatabase - gets the mesh array from the Node3D - strips out everything we don't need so loading is smooth
	/// 
	/// COULD be a source of the bug which replaces meshes with other meshes? 
	/// </summary>
	/// <param name="node3D"></param>
	/// <returns></returns>
	Array<Mesh> GenerateMeshArrayForSaving(Node3D node3D)
	{
		Array<Mesh> returnArr = new Array<Mesh>();
		Array<Node> components = node3D.GetChildren();

		foreach(Node component in components)
		{
			if(component is MeshInstance3D meshComponent)
			{
				returnArr.Add(meshComponent.Mesh); 
			}
		}
		return returnArr; 
	}

	#endregion
	#region // PROCESS AND POLLING -------------------------------------------------------------------

	/// <summary>
	/// Length of time in seconds between region polls. 
	/// </summary>
	[Export]
	float pollingInterval = 1f; 
	float currentPolling = 0f; 

	public override void _Process(double delta)
	{
		//prevent anything else from happening if we're in edit mode. 
		if(editMode == true) return; 

		//Otherwise, UpdatePolling. 
		UpdateAssetStream(delta); 
	}

	/// <summary>
	/// Update Polling Attributes
	/// </summary>
	bool loadingMMIPhase = false; 

	bool loadingNodePhase = false; 
	/// <summary>
	/// This needs to be used to load the Node3D regions
	/// </summary>
	Godot.Collections.Dictionary<int, Vector2I> MMIRegions = new Godot.Collections.Dictionary<int, Vector2I>(); 

	[Export]
	int MMIRadius = 3; 

	Godot.Collections.Dictionary<int, Vector2I> nodeRegions = new Godot.Collections.Dictionary<int, Vector2I>(); 

	[Export]
	int nodeRadius = 1; 

 	int pollingLoopI = 0; 
	int pollingLoopJ = 0;

	int[]MMIkeys;
	int[]nodeRegionKeys; 

	/// <summary>
	/// Update Polling - run every _Process when not in editMode 
	/// </summary>
	/// <param name="delta"></param>
	void UpdateAssetStream(double delta)
	{
		currentPolling += (float) delta; 	

		if(!loadingMMIPhase || !loadingNodePhase)
		{
			UpdatePolling(); 
		}
		

		//This will only run if loadingPhase is TRUE, and will run every _Process loop until loadingPhase is set to false. 
		//This way, the same region will continue 
		if(loadingMMIPhase)
		{
			MMILoadingPhase(); 
		}

		if (loadingNodePhase)
		{
			NodeLoadingPhase();
		}
	}

	void UpdatePolling()
	{
		//Note CheckRegionPositionChanged will update the region position if it has changed. 
		if(currentPolling > pollingInterval && CheckRegionPositionChanged())
		{
			MMIRegions = GetRegionsByRadius(MMIRadius);
			
			MMIkeys = MMIRegions.Keys.ToArray<int>();

			nodeRegions = GetRegionsByRadius(nodeRadius);
			
			nodeRegionKeys = nodeRegions.Keys.ToArray<int>();

			//make sure we don't load the MMI regions where there should be nodes instead. 
			foreach(int key in nodeRegions.Keys)
			{
				MMIRegions.Remove(key); 
			}

			currentPolling = 0;

			if(terrain3D == null)
			{
				GetTerrain();
			}

			//Deinitialise all regions NOT in the loading grid. 
			foreach(int allRegionID in currentlyLoadedAssetsByRegion.Keys)
			{
				if (!MMIRegions.ContainsKey(allRegionID))
				{
					DeinitialiseMMIAssetsByRegion(allRegionID); 
				}
			}

			DeinitialiseNodeRegionsOutsideOfArea(nodeRegions); 

			loadingMMIPhase = true; 
			loadingNodePhase = true; 
		}
	}

	void MMILoadingPhase()
	{
		if(pollingLoopI <= MMIRegions.Keys.Count - 1)
		{
			int regionID = MMIkeys[pollingLoopI]; //this is a heavy LINQ expression

			//If the current region hasn't yet been loaded 
			if (!currentlyLoadedAssetsByRegion.ContainsKey(regionID))
			{
				// GD.Print("LOADING region: ", regionID); 
								
				bool loaded = RequestLoadMMIRegionAssets(MMIRegionFileName, regionID, true); //begins to load assets for a region

				if(loaded == true)
				{
					// GD.Print(regionID); 
					if (MMIRegions.ContainsKey(regionID))
					{
						ReinitialiseMMIRegionByID(regionID,MMIRegions[regionID]); 
					}
					if(pollingLoopI < MMIRegions.Keys.Count - 1)
					{
						pollingLoopI ++; //Only go to the next region when loaded == true	
					}
					else
					{
						loadingMMIPhase = false; //if loaded == true AND pollingUpdateLoopI is more than Key count
						pollingLoopI = 0; 
					}
				}
			}
			else //else - the region has already been loaded, so the data can be looked up in the currentlyLoadedAssetsByRegion dictionary
			{
				pollingLoopI ++;
				// GD.Print("region already in currently loaded assets: ", regionID);
				// GD.Print(currentlyLoadedAssetsByRegion.Keys); 
			}
		}
		else //Break out of the loadingPhase and back into normal updatePolling (checking if we've moved into a new area). 
		{
			GD.Print($"FINISHING MMI LOOP: loop final values {pollingLoopI}, {pollingLoopJ}"); 
			currentPolling = 0; 
			pollingLoopI = 0;
			loadingMMIPhase = false; 
		}
		
	}

	void NodeLoadingPhase()
	{
		if(pollingLoopJ <= nodeRegions.Keys.Count -1)
		{
			bool loaded = RequestLoadMMIRegionAssets(nodeRegionFileName, nodeRegionKeys[pollingLoopJ], false); 
			if(loaded)
			{
				pollingLoopJ ++; //should be some kind of timeout - so if enough loops pass and it STILL hasn't worked 
			}
		}
		else
		{
			GD.Print($"FINISHING NODE LOOP: loop final values {pollingLoopI}, {pollingLoopJ}"); 

			pollingLoopJ = 0; 
			loadingNodePhase = false;
		}
		
	}

	#endregion
	#region NODE REGIONS ------------------------------------------------------------------- 
	
	void DeinitialiseNodeRegionsOutsideOfArea(Godot.Collections.Dictionary<int,Vector2I> areas)
	{
		foreach(int key in NodesByRegion.Keys)
		{
			GD.Print("trying to deinitialise node in region", key); 

			if (!areas.ContainsKey(key))
			{
				DeinitialiseNodeRegion(key);

			}
			else
			{
				GD.Print ("could not deinitialise as it is in areas dictionary"); 
			}
		}
	}

	void DeinitialiseNodeRegion(int RegionID)
	{
		if (NodesByRegion.ContainsKey(RegionID))
		{
			GD.Print("getting closer to deinitialising!"); 
			foreach(Node n in NodesByRegion[RegionID])
			{
				// GD.Print("should DEFINITELY be deinitialising!"); 
				n?.QueueFree(); 
			}
		}
	

		//this needs to deinitialise the NODEs but NOT the MMIS - and vice versa in the MMI one! 
	}
	
	/// <summary>
	/// Switch back to edit mode - reloading original nodes. 
	/// 
	/// 
	/// </summary>
	void ReloadAllOriginalNodeData()
	{
		editMode = true; 

		KillAllChildren(); 

		DirAccess dirAccess = DirAccess.Open("user://"); 

		string[] files = dirAccess.GetFiles(); 

		foreach(string file in files)
		{
			GD.Print(file); 
			if (file.Contains("node"))
			{
				GD.Print("reloading", file); 
				RegionNodeData regionNodeData = (RegionNodeData) ResourceLoader.Load("user://" + file, typeHint: "RegionNodeData", cacheMode: ResourceLoader.CacheMode.ReplaceDeep); 

				Array<PackedScene> scenes = regionNodeData.GetScenes(); 

				GD.Print("number of scenes in region: ", scenes.Count); 
				foreach(PackedScene scene in scenes)
				{
					GD.Print("scene being intantiated");
					Node node = scene.Instantiate();
					
					AddChild(node); 

					//only works in editor - but this can only be called in editor!
					node.Owner = GetTree().EditedSceneRoot; 
				}	
			}
		}

	}
	
	# endregion

	# region STREAMABLE REGIONS -------------------------------------------------------------------


	/// <summary>
	/// Save the Dictionary<int, Array<PackedScene>> as a WorldSpaceData class. This contains a Dictionary<int, RegionData>,
	/// RegionData wraps the PackedScene array - this setup is to enable serialisation 
	/// </summary>
	void SaveStreamablesByRegionToDisk()
	{
		//Remove any pre-existing files to preven... weird behaviour
		DirAccess dirAccess = DirAccess.Open("user://");
		string[] existingFiles = dirAccess.GetFiles();
		foreach(string file in existingFiles)
		{
			if (file.Contains("region"))
			{
				dirAccess.Remove(file); 
			}
		}

		//create new files. 
		foreach(int k in streamablesByRegion.Keys)
		{
			Array<StreamableObject> regionData = streamablesByRegion[k];
			RegionMMIData regionMMIData = new RegionMMIData(regionData); 
			Error regionSave = ResourceSaver.Save(regionMMIData,$"user://region_mmi_data_{k}.tres", ResourceSaver.SaverFlags.None); 
			if(regionSave != Error.Ok) 
			{
				GD.Print("Error serialising asset streaming - error is : ", regionSave);
			}
		}
	}


	/// <summary>
	/// Reads the WorldSpaceData asset from the disk to stream the data. 
	/// </summary>
	bool RequestLoadMMIRegionAssets(string resourcePath, int regionID, bool isMMI)
	{
		string typeHint = "RegionNodeData"; 
		if (isMMI)
		{
			typeHint = "RegionMMIData";

		}
	

		string fileName = resourcePath + regionID + ".tres"; 

			GD.Print($"requesting to load assets for {fileName}"); 


		string[] files  = DirAccess.GetFilesAt("user://");  

		foreach(string file in files)
		{
			
			GD.Print(file); 

			if(file == fileName)
			{
				GD.Print("whyyy??"); 
			}
		}

		
		//Initialises as a resource
		if(files.Contains(fileName))
		{
			GD.Print($"LOADSTATUS requesting to load {resourcePath}");

			ResourceLoader.ThreadLoadStatus threadLoadStatus = ResourceLoader.LoadThreadedGetStatus("user://" + fileName); 

			//if it hasn't yet been loaded, start loading it.
			if(threadLoadStatus == ResourceLoader.ThreadLoadStatus.InvalidResource)
			{
				// GD.Print("LOADSTATUS starting loading"); 
				Error error  = ResourceLoader.LoadThreadedRequest("user://" + fileName, typeHint: typeHint , false, ResourceLoader.CacheMode.ReplaceDeep); 

				return false; //not yet loaded
			}
			else if(threadLoadStatus == ResourceLoader.ThreadLoadStatus.Loaded)
			{

				if (isMMI)
				{
					Resource regionDataRes = ResourceLoader.LoadThreadedGet("user://" + fileName);

					GD.Print("FILE NAME!!"); 

					try
					{
						RegionMMIData regionData = (RegionMMIData) regionDataRes; 
						if(regionData == null) return true; //if there isn't anything in the region, return. 


						GD.Print($"LOADSTATUS Loaded {resourcePath}", regionData);

						if (!streamablesByRegion.ContainsKey(regionID))
						{
							streamablesByRegion.Add(regionID, regionData.regionData); 

						}
					}
					catch(Exception e )
					{
						GD.Print("FILE NAME", e);
						GD.Print("FILE NAME is: ", fileName); 

					}

					

				}
				else
				{
					//!REFACTOR - THIS SHOULD NOT BE HERE - DUPLICATION OF CODE FOR MMI AND NODE REGIONS 
					RegionNodeData regionDataRes = (RegionNodeData) ResourceLoader.LoadThreadedGet("user://" + fileName);

					if(regionDataRes == null) return true; //if there isn't anything in the region. return

					if (nodeRegions.ContainsKey(regionID))
					{
						Array<PackedScene> scenes = regionDataRes.GetScenes(); 

						GD.Print("FILE NAME loading NODES! number of NODES in this region: ", scenes.Count); 
						//I want to do ALL of this on a thread as well! 
						foreach(PackedScene scene in scenes)
						{
							Node node = scene.Instantiate();



							if(!NodesByRegion.TryAdd(regionID, [node]))
							{
								NodesByRegion[regionID].Add(node); 
							} 
							
							AddChild(node); 

							//only works in editor - but this can only be called in editor!
							node.Owner = GetTree().EditedSceneRoot; 
						}
					}


				}
			


				//Iterate through each region and re-add to the runtime dictionary packedAssetsByRegion. 
				// totalStreamableAssets.Add(regionID, regionData.regionData); 

				return true; 
			}
			else if(threadLoadStatus == ResourceLoader.ThreadLoadStatus.InProgress)
			{
				GD.Print("LOADSTATUS ", fileName, " ", threadLoadStatus); 
				return false; 
			}
			else if(threadLoadStatus == ResourceLoader.ThreadLoadStatus.Failed)
			{
				GD.Print("LOADSTATUS failed to load region: ", regionID); 
				return true; //to prevent getting stuck in infinite load. 
			}
		}
		else
		{
			GD.Print($"LOADSTATUS can't access the file {fileName} now because it can't find it in the folder");
		}
		return true; //catch all - enables us to move on if there is no file or some kind of problem with the file 
	}

	/// <summary>
	/// Iterates through regions - calls ReinitialiseStreamableAsset on that region. 
	/// </summary>
	/// <param name="regionID"></param>
	/// <param name="regionLocation"></param>
	void ReinitialiseMMIRegionByID(int regionID, Vector2I regionLocation)
	{
		// GD.Print("reinitialising region,  id =", regionID); 
		//Prevents accidentally loading the assets for a region multiple times. 
		if(currentlyLoadedAssetsByRegion.ContainsKey(regionID)) return; 

		currentlyLoadedAssetsByRegion.Add(regionID, new System.Collections.Generic.Dictionary<string, Array<MultiMeshInstance3D>>());

		if (streamablesByRegion.ContainsKey(regionID))
		{
			// GD.Print("reinitialising region", regionID, "assets found = ",streamablesByRegion[regionID].ToString()); 

			foreach(StreamableObject streamable in streamablesByRegion[regionID])
			{
				ReinitialiseStreamableAsset(streamable, regionID, regionLocation); 
			}
		} 
		else
		{
			// GD.Print("No assets in this region :(", regionID); 	
			// RequestLoadRegionAssets(regionID); 
		}
	}

	/// <summary>
	/// Initialises one asset for one region - if there are 50 trees in a region, initialises a MMI with 50 instances, using 
	/// StreamableObject
	/// </summary>
	/// <param name="streamable"></param>
	/// <param name="regionID"></param>
	/// <param name="regionLocation"></param>
	void ReinitialiseStreamableAsset(StreamableObject streamable, int regionID, Vector2I regionLocation)
	{
		// GD.Print("reinitialising a streamable asset", streamable.meshes.ToString()); 

		Array<MultiMeshInstance3D> assetMultiMeshes = new Array<MultiMeshInstance3D>();

		Array<Mesh> componentMeshes = streamable.meshes;

		//Create the multimeshes (one Node has several meshes - hence the for loop)
		foreach(Mesh component in componentMeshes)
		{
			// GD.Print("iterating through components");

            MultiMesh multiMesh = new MultiMesh
            {
                TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,

                Mesh = component,

                CustomAabb = component.GetAabb()
            };

            MultiMeshInstance3D MMI = new MultiMeshInstance3D
            {
                Multimesh = multiMesh
            };

            assetMultiMeshes.Add(MMI);
			AddChild(MMI);
			Vector2I regionCoords = regionLocation; 
			int regionSize = (int) terrain3D.Call("get_region_size"); //actually returns a terrain3D enum
			MMI.CustomAabb = new Aabb(new Vector3(regionCoords.X * regionSize, 0, regionCoords.Y * regionSize), new Vector3(regionSize,regionSize,regionSize)); 
			
			//Can be removed later but helpful to manipulate nodes for debugging purposes. 
			var sceneRoot = GetTree().EditedSceneRoot;
			if (sceneRoot != null && sceneRoot.IsAncestorOf(MMI))
			{
				MMI.Owner = sceneRoot;
			}

			else if (!Engine.IsEditorHint())
			{
				MMI.Owner = GetTree().CurrentScene; 
			}
		}


		//Set the instance transforms
		foreach(MultiMeshInstance3D mesh in assetMultiMeshes)
		{
			mesh.Multimesh.InstanceCount = streamable.transforms.Count; 
			for( int i = 0; i < streamable.transforms.Count; i++)
			{
				mesh.Multimesh.SetInstanceTransform(i, streamable.transforms[i]); 
			}
		}

		//Add the newly created assetMultiMeshes to the currentlyLoadedAssets dictionary 
		if (!currentlyLoadedAssetsByRegion[regionID].ContainsKey(streamable.assetID))
		{		
			currentlyLoadedAssetsByRegion[regionID].Add(streamable.assetID, assetMultiMeshes); //add the newly instantiated node to the currently loaded assets dictionary
		}
		else
		{
			throw new Exception("trying to add the same streamable to currentlyLoadedAssetsByRegion[RegionID] twice -- this SHOULND'T HAPPEN");
		}
	
	}

	/// <summary>
	/// Currently called in main process loop to deinitialise any regions not in the adjacents list. 
	/// Queuefree and remove from CurrentlyLoadedAssetsByRegion
	/// </summary>
	/// <param name="regionID"></param>
	void DeinitialiseMMIAssetsByRegion(int regionID)
	{
		//Delete the assets from the scene 
		foreach(Array<MultiMeshInstance3D> assetMeshList in currentlyLoadedAssetsByRegion[regionID].Values)
		{
			foreach(MultiMeshInstance3D multiMesh in assetMeshList)
			{
				multiMesh?.QueueFree(); 	//"not set so instance of an object - if saving before creating DB " 
			}
		}

		//Remove the references in the tracker dictionary 
		if (currentlyLoadedAssetsByRegion.ContainsKey(regionID))
		{
			currentlyLoadedAssetsByRegion[regionID].Clear(); 
			currentlyLoadedAssetsByRegion.Remove(regionID);
		}
	}

	# endregion
	
	# region HELPER METHODS -------------------------------------------------------------------

	/// <summary>
	/// Checks region position has changed and if so updates region position
	/// </summary>
	/// <returns></returns>
	bool CheckRegionPositionChanged()
	{
		if(terrain3D == null)
		{
			GetTerrain();
		}
		if(activeCamera == null)
		{
			SetActiveCam(); 
		}

		int check = (int)  terrainData.Call("get_region_idp", activeCamera.GlobalPosition);
		if(check != currentRegionPosition)
		{
			currentRegionPosition = check;
			return true;
		}
		return false; 
	}

	/// <summary>
	/// Returns a list of the regionIDs of adjacent regions. 
	/// 
	/// Bug - will occasionally say "key already added to dictionary when the camera goes out of bounds!
	/// </summary>
	/// <returns></returns>
	Godot.Collections.Dictionary<int, Vector2I> GetRegionsByRadius(int radius)
	{
		Godot.Collections.Dictionary<int, Vector2I> adjacentRegions = new Godot.Collections.Dictionary<int, Vector2I>();  

		Vector3 camPos = activeCamera.GlobalPosition;

		Vector2I currentRegionPos = (Vector2I) terrainData.Call("get_region_location", camPos); 

		int currentRegionID = (int)terrainData.Call("get_region_idp", camPos);

		//In a try catch - as occasionally an out of bounds region is part of this - 
		//causes duplicate Key exception as all out of bounds regions have the same ID

		
		for(int i = -radius; i < radius + 1; i++)
		{
			for(int j = -radius; j < radius + 1; j++)
			{
				try
				{
					int regionID = (int)terrainData.Call("get_region_id", new Vector2I(currentRegionPos.X + i, currentRegionPos.Y + j));
					adjacentRegions.Add(regionID, new Vector2I(currentRegionPos.X + i, currentRegionPos.Y + j)); 
				}
				catch(Exception e) //this will sometimes through an exception due to multiple key error - if a region is out of bounds, they all have the same ID
				{
					GD.Print($"Error - {currentRegionPos.X + i}, {currentRegionPos.Y + j} out of map boundary. Dictionary Key Exception: ", e);
				}
			}
		}


		// GD.Print("should load following regions:"); 
		foreach(Vector2I vec in adjacentRegions.Values)
		{
			// GD.Print(vec); 
		}
		
		return adjacentRegions; 		
	}

	/// <summary>
	/// Make sure this is what you want to do - used when switching BACK from streaming to edit mode.
	/// </summary>
	void KillAllChildren()
	{
		Array<Node> children = GetChildren(); 

		foreach(Node child in children)
		{
			child.QueueFree(); 
		}
	}

	/// <summary>
	/// Assign Terrain classes for region calculations
	/// </summary>
	public void GetTerrain()
	{
		terrain3D = GetTree().GetFirstNodeInGroup("Terrain3D");
		terrainData = (GodotObject) terrain3D.Call("get_data");

		Godot.Collections.Dictionary regionsArr = (Godot.Collections.Dictionary) terrainData.Call("get_regions_all");

		// GD.Print("getting region array"); 
		foreach(Variant var in regionsArr.Keys)
		{
			// GD.Print(var.ToString()); 
		}
		// Use regionsArr as needed, e.g. iterate over active regions
	}
  
	/// <summary>
	/// On saving the scene, deinitialise all region assets to prevent duplicates appearing in game. 
	/// </summary>
	/// <param name="what"></param>
    public override void _Notification(int what)
    {
		if (what == NotificationEditorPreSave)
		{
			foreach(int key in currentlyLoadedAssetsByRegion.Keys)
			{
				DeinitialiseMMIAssetsByRegion(key); 

				
			}

			currentlyLoadedAssetsByRegion = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<string, Array<MultiMeshInstance3D>>>(); 

		}
        base._Notification(what);
    }

	void SetActiveCam()
	{
		if (Engine.IsEditorHint())
		{
			activeCamera = EditorInterface.Singleton.GetEditorViewport3D().GetCamera3D();

		}
		else
		{
			activeCamera = Player.Instance.cam; 
		}
	}
	#endregion
}