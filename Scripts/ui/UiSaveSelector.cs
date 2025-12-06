using Godot;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Linq;

public partial class UiSaveSelector : Control
{
    [Signal]
    public delegate void OnLoadSaveSelectorEventHandler(string save);

    Godot.Collections.Array<LoadGameContainer> loadGameInstances = new Godot.Collections.Array<LoadGameContainer>();

    // LoadGameContainer loadGameInstance; //HBox container  scene

    VBoxContainer vBoxContainer;


    public override void _Ready()
    {

        vBoxContainer = (VBoxContainer)GetNode("%VBoxContainer"); 
        // loadGameInstance = (HSplitContainer)GetNode("VBoxContainer/LoadGameContainer");


        //populate vertical container with save game buttons 
        base._Ready();
    }

    //Same code for inventory 
    public void PopulateContainers(string[] saveList)
    {
        GD.Print(saveList);
        
        var res = ResourceLoader.Load<PackedScene>("uid://d72fkdtm5aam");

        foreach (String save in saveList)
        {
            LoadGameContainer loadGameInstance = (LoadGameContainer)res.Instantiate(); 
            
            loadGameInstance.saveFile = save;

            loadGameInstance.button.Text = save; //just changing the textto the file name to make it easier! 

            vBoxContainer.AddChild(loadGameInstance);

            loadGameInstances.Add(loadGameInstance); 

            loadGameInstances[^1].OnLoadGameContainerButtonPressed += LoadGame; //connects the last item in the list to the load game 
        }

    }
    
    public void FreeContainers()
    {
        var children = vBoxContainer.GetChildren();
        foreach(var child in children)
        {
            child.QueueFree(); 
        }

        //!REALLY IMPORTANT CODE TO PREVENT MAJOR BUGS PROBABLY AND SLOW DOWNS FROM PLAYING GAME FOR A LONG TIME. 
        foreach(LoadGameContainer item in loadGameInstances)
        {
            item.OnLoadGameContainerButtonPressed -= LoadGame; 
        }
        loadGameInstances.Clear(); 
        
        //get rid of the contain    ers after closing the window
    }
    
    //signal propogates back up through the ui layers. 
    public void LoadGame(String saveFile)
    {
        EmitSignal(SignalName.OnLoadSaveSelector, saveFile); 
    }


}
