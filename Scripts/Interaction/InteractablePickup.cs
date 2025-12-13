using System;
using Godot;
using Godot.Collections;
using Microsoft.VisualBasic.FileIO;

//TODO Rename class - DOESN'T inherit from but contains a reference TO interactable. 
public partial class InteractablePickup : SaveableNode
{
    //the ID of the inventory object this interactable corresponds to. 
    [Export]
    public string inventoryID; 

    //There MUST be a better way to do this!! 
    [Export]
    public string pickupID; 

    [Export]
    CollisionObject3D collision; 

    [Export]
    MeshInstance3D mesh;

    [Export]
    Material focusMat;

    [Export]
    Material interactedMat;

    Material defaultMat;

    Interactable interactable;

    bool interacted;

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

    public void Interact(Area3D interactor)
    {
        AudioManager.Play("C:/Users/Lenovo/Documents/Godot Projects/saving-and-loading/Assets/Sound/GDC/BluezoneCorp - Stone Impact/Bluezone_BC0297_stone_impact_hammer_015.wav"); 
     
        interacted = true;

        //not working for key?
        if (disappears)
        {
            GD.Print("hiding key?? Mesh name: ", mesh.Name); 

            mesh.Visible = false; 
            mesh.Hide(); // mesh disappears for pickups. 
            collision.ProcessMode = CollisionObject3D.ProcessModeEnum.Disabled; 
        }
     
        mesh.MaterialOverride = interactedMat;

        defaultMat = interactedMat;
    }

    public void Focus(Area3D interactor)
    {
        mesh.MaterialOverride = focusMat;
    }

    public void Unfocus(Area3D interactor)
    {
        mesh.MaterialOverride = defaultMat;
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