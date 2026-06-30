using System;
using System.Collections.Generic;
using Godot;


[Tool]
[GlobalClass]
public partial class StreamableObject : Resource
{

    /// <summary>
    /// Just for compatibility 
    /// </summary>
    [Export]
    public string assetID; 

    //render all of these MMIS
    [Export]
    public Godot.Collections.Array<Mesh> meshes;

    //at all of these transforms
    [Export]
    public Godot.Collections.Array<Transform3D> transforms; 

    [Export]
    public ObjectAttributes objectAttributes; 

    public StreamableObject(string sceneFilePath)
    {
        
    }

    public StreamableObject()
    {
        
    }

    public StreamableObject(Godot.Collections.Array<Mesh> meshes, Godot.Collections.Array<Transform3D> transforms)
    {
        this.meshes = meshes;
        this.transforms = transforms; 
    }

    public StreamableObject(string assetID, Godot.Collections.Array<Mesh> meshes, Godot.Collections.Array<Transform3D> transforms)
    {
        this.meshes = meshes;
        this.transforms = transforms; 
        this.assetID = assetID; 
    }

    public StreamableObject(Godot.Collections.Array<Mesh> meshes, Godot.Collections.Array<Transform3D> transforms, ObjectAttributes objectAttributes)
    {
        this.meshes = meshes;
        this.transforms = transforms;
        this.objectAttributes = objectAttributes; 
        
    }

    public bool CheckStreamableObjectIsNode(string assetID)
    {
        if(assetID == this.assetID)
        {
            return true; 
        }
        return false; 
    }

    public void AddTransform(Transform3D transform3D)
    {
        transforms.Add(transform3D); 
    }

    

    
     
}