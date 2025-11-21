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

        vBoxContainer = (VBoxContainer)GetNode("VBoxContainer"); 
        // loadGameInstance = (HSplitContainer)GetNode("VBoxContainer/LoadGameContainer");


        //populate vertical container with save game buttons 
        base._Ready();
    }

    public void PopulateContainers(string[] saveList)
    {
        GD.Print("list from save selector:");
        GD.Print(saveList);
        foreach (String save in saveList)
        {
            LoadGameContainer loadGameInstance = ResourceLoader.Load<PackedScene>("uid://d72fkdtm5aam").Instantiate() as LoadGameContainer;

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
        //get rid of the containers after closing the window
        GD.Print("need to implement a way to free the containers!"); 
    }
    
    //signal propogates back up through the ui layers. 
    public void LoadGame(String saveFile)
    {
        GD.Print("Selector button pressed"); 
        EmitSignal(SignalName.OnLoadSaveSelector, saveFile); 
    }


}
