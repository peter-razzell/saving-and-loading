using System;
using Godot;
using Godot.Collections;

//TODO Rename class - DOESN'T inherit from but contains a reference TO interactable. 
public partial class InteractablePickup : SaveableNode
{
    //the key for the lookup table
    [Export]
    public String DataLookupKey; 

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

        interactable.OnFocused += Focus;
     
        interactable.OnUnfocused += Unfocus;
     
        interactable.OnInteracted += Interact;
    }

    public void Interact(Area3D interactor)
    {
        AudioManager.Play("C:/Users/Lenovo/Documents/Godot Projects/saving-and-loading/Assets/Sound/GDC/BluezoneCorp - Stone Impact/Bluezone_BC0297_stone_impact_hammer_015.wav"); 
     
        interacted = true;

        if (disappears)
        {
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

        

        if (data is SavedPickupData pickupData)
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
        GD.Print("OnSave method of pickups in scene called for object: ", this.DataLookupKey);
     
        SavedPickupData pickupData = new()
        {
            position = GlobalPosition,
     
            scenePath = SceneFilePath,
     
            interacted = interacted
        };

        savedData.Add(pickupData);
    }
}