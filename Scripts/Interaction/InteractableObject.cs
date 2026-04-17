

using Godot;


/// <summary>
/// Base class for interactable objects.
/// 
/// Inherited by InteractablePickup for inventory logic
/// 
/// Note - contains the Interactable class as a composition not inheritance. 
/// </summary>
public partial class InteractableObject : SaveableNode
{
    [Export]
    protected CollisionObject3D collision; 

    [Export]
    protected MeshInstance3D mesh; 

    [Export]
    protected Material focusMat; 

    [Export]
    protected Material interactedMat; 

    protected Material defaultMat; 

    public bool interacted; 

    protected string interactSound = "uid://c2877a2mxbya3"; 

    protected Interactable interactable;

    public override void _Ready()
    {
        defaultMat = mesh.MaterialOverride;
	 
		interactable = (Interactable)GetNode("Interactable");

		interactable.OnFocused += Focus;
	 
		interactable.OnUnfocused += Unfocus;
	 
		interactable.OnInteracted += Interact;

        base._Ready();
    }

    //Overriden by Interactable Pickup
    public void Interact(Area3D interactor)
	{
		if (!interacted || interactor == null)
		{
			AudioManager.Play(interactSound); 

			interacted = true;

			mesh.MaterialOverride = interactedMat;

			defaultMat = interactedMat;
		}
	}

	public void Focus(Area3D interactor)
	{
		mesh.MaterialOverride = focusMat;
	}

	public void Unfocus(Area3D interactor)
	{
		mesh.MaterialOverride = defaultMat;
	}





    
}