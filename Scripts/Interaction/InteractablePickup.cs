using System;
using Godot;
using Godot.Collections;
using Microsoft.VisualBasic.FileIO;

//TODO Rename class - DOESN'T inherit from but contains a reference TO interactable. 
public partial class InteractablePickup : InteractableObject
{
	string pickupSound = "uid://c2877a2mxbya3"; 
	//the ID of the inventory object this interactable corresponds to. 
	[Export]
	public string inventoryID; 

	//Pickups have an ID, inventory items have an ID. This way, the same pickup with the same mesh instance can be tied to multiple different 
	//inventory objects. E.g. a letter object with  different letters. 
	[Export]
	public string pickupID; 

	// [Export]
	// CollisionObject3D collision; 

	// [Export]
	// MeshInstance3D mesh;

	// [Export]
	// Material focusMat;

	// [Export]
	// Material interactedMat;

	// Material defaultMat;

	// Interactable interactable;

	// public bool interacted;

	[Export]
	bool disappears; 

	public override void _Ready()
	{
		defaultMat = mesh.MaterialOverride;
	 
		interactable = (Interactable)GetNode("Interactable");

		GD.Print("interactable pickup UID : ", pickupID);  

		interactable.OnFocused += Focus;
	 
		interactable.OnUnfocused += Unfocus;
	 
		interactable.OnInteracted += Interact;
	}

	public new void Interact(Area3D interactor)
	{
		//This function may be called to force an interaction when loading a level to keep interacted pickups correct, in this case interactor will be null
		//and interacted will already be set to true. This could be a separate function called forceinteract. 
		if (!interacted || interactor == null)
		{
			AudioManager.Play(pickupSound); 

			interacted = true;

			if (disappears)
			{
				mesh.Visible = false; 
				mesh.Hide(); 
				collision.ProcessMode = CollisionObject3D.ProcessModeEnum.Disabled; 
			}
			else
			{
				//Thinking about chests, or any item which adds to inventory but does not remove anything? 
				GD.Print("Not making this object disappear!"); 
			}
		
			mesh.MaterialOverride = interactedMat;

			defaultMat = interactedMat;
		}

	   
	}

	//Delete old instance to prevent duplication with loading of new instance.
	public override void OnBeforeLoad()
	{
		QueueFree();
	}

	public override void OnLoad(SavedData data)
	{
		GlobalPosition = data.position;

		if (data is SavedInteractableData pickupData)
		{
			interacted = pickupData.interacted;

			if (interacted == true)
			{
				GD.Print("Auto interacting on loading top preserve interactable state!");
				Interact(null);
			}
		}
	}

	//Called because it is in the persist group 
	public override void OnSave(Array<SavedData> savedData)
	{
		GD.Print("OnSave method of pickups in scene called for object: ", this.inventoryID);
	 
		SavedInteractableData pickupData = new()
		{
			position = GlobalPosition,
	 
			scenePath = SceneFilePath,
	 
			interacted = interacted
		};

		savedData.Add(pickupData);
	}
}
