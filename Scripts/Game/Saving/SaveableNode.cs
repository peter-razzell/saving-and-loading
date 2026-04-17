using System;
using Godot;
using Godot.Collections;


//Class acts as interface for saveable nodes, currently InteractablePickup inherits
public partial class SaveableNode : Node3D
{
    public virtual void OnSave(Array<SavedData> datas)
    {
        GD.Print("On Save base function called - missing an override somewhere? Object is: ", this.Name); 

    }
    public virtual void OnBeforeLoad()
    {

    }
    public virtual void OnLoad(SavedData data)
    {

    }

}