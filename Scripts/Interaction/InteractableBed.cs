using Godot; 

public partial class InteractableBed : InteractableObject
{
    


    public new void Interact(Area3D interactor)
	{
		if (!interacted || interactor == null)
		{
			AudioManager.Play(interactSound); 

			interacted = true;

			mesh.MaterialOverride = interactedMat;

			defaultMat = interactedMat;

            
		}
	}
}