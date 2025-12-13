using System;
using Godot;
using Godot.Collections;


//Class acts as a kind of interface for saveable nodes, currently InteractablePickup inherits
public partial class SaveableNode : Node3D
{
    public virtual void OnSave(Array<SavedData> datas)
    {

    }
    public virtual void OnBeforeLoad()
    {

    }
    public virtual void OnLoad(SavedData data)
    {

    }

}