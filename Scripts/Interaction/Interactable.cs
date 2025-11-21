using Godot;
using System;
using System.Diagnostics.Tracing;

public partial class Interactable : Area3D
{

    [Signal]
    public delegate void OnInteractedEventHandler(Interactor interactor);

    [Signal]
    public delegate void OnFocusedEventHandler(Interactor interactor);

    [Signal]
    public delegate void OnUnfocusedEventHandler(Interactor interactor); 
}
