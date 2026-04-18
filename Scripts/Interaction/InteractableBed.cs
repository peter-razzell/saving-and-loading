using System.Security.Cryptography;
using Godot; 

public partial class InteractableBed : InteractableObject
{
    
	OverlaySleepEffect overlaySleepEffect;

    public override void _Ready()
    {
		interactable = (Interactable)GetNode("Interactable");

		interactable.OnInteracted += Interact;

		overlaySleepEffect = GetNode<OverlaySleepEffect>("OverlaySleepEffect"); 
    }


    public new void Interact(Area3D interactor)
	{
		GD.Print("interacting with the bed!"); 

		overlaySleepEffect.MakeVisible(); 

		AudioManager.Play(interactSound); 

	}
}