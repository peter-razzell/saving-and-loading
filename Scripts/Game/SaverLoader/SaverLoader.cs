

using System;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class SaverLoader : Node
{
    [Export]
    Player player;

    Viewport viewport;

    [Export]
    UiSaveLoad ui; 

    Game game;

    /*The buffer contains information about each level. It is populated on entering a level for the first time, or on load. 
    On save, this information is saved, so every save file contains information about every level.*/
    Dictionary<String, Array<SavedData>> levelBuffer;

    string loadFilePath = ""; //Path to load (a file that exists)
    string saveFilePath = ""; //The path to save (a file that doesn't exist yet)
    string genericFilePath = "user://savegames/save_?.res"; //used to create the filepath
    string saveFolderPath ="user://savegames/" ; //the current save file path combining savePath + saveFile + identifying digit (saveCount).
    string[] saveFiles = [];
    int saveCount;

    public override void _Ready()
    {
        game = GetOwner<Game>();

        levelBuffer = new Dictionary<string, Array<SavedData>>();

        viewport = player.cam.GetViewport();

        saveCount = GetSaveCount(); //makes directory if it doesn't already exist. 

        base._Ready();
    }

    //Returns a list of the saved games, used to populate load game container.
    public string[] GetSaveNames()
    {
        DirAccess dir = DirAccess.Open("user://"); //open the user directory as a DirAccess object

        if (dir.DirExists("savegames"))
        {
            //user/savegames already exists
            return ResourceLoader.ListDirectory(saveFolderPath);
        }
        return [];
    }

    //Returns the number of saved games.
    int GetSaveCount()
    {
        //See https://docs.godotengine.org/en/stable/classes/class_diraccess.html  
        DirAccess dir = DirAccess.Open("user://"); 
        if (dir.DirExists("savegames"))
        {
            saveFiles = ResourceLoader.ListDirectory(saveFolderPath);
            return saveFiles.Length; 
        }
        else
        {
            dir.MakeDir("savegames");
            return 0;
        }
    }

    //Updates saveFilePath to the next path. Called when game is saved. 
    void UpdateSaveFilePath()
    {
        saveFilePath = genericFilePath.Replace("?", GetSaveCount().ToString());
    }

    //Saves the level to the levelBuffer when leaving a level. 
    public void SaveLevelToBuffer()
    {
        var savedDataArr = new Array<SavedData>();

        //Call OnSave function for persisting items so they can add their information to the savedData array
        GetTree().CallGroup("Persist", "OnSave", savedDataArr);

        String path = game.root.GetCurrentLevelPath();

        if (levelBuffer.ContainsKey(path))
        {
            levelBuffer[path] = savedDataArr;
        }
        else
        {
            levelBuffer.Add(path, savedDataArr);
        }
    }

    //Saves the game. 
    public void SaveGame()
    {
        //Ensures filePath points to latest existing filepath rather than loaded file. 
        UpdateSaveFilePath();

        SavedGame savedGame = new SavedGame();

        //Hacky way of getting image from viewport for save icon. 
        // ui.Hide();
        Image img = viewport.GetTexture().GetImage();
        // ui.Show();
        savedGame.viewportImage = img;

        //Save level path so the game knows which level to load when opening the save.
        savedGame.level = game.root.GetCurrentLevelPath();

        //Save player attributes
        savedGame.playerHealth = player.health;
        savedGame.playerPos = player.GlobalPosition;
        Array<Variant> varInv = [];
        foreach (Interactable inter in player.playerData.GetInv()) //Stores inventory as variant due to bug with godot [Export].
        {
            varInv.Add((Variant)inter);

        }
        savedGame.playerInv = varInv;
        savedGame.playerHeadRot = player.head.GlobalRotation;
        savedGame.playerCamRot = player.cam.GlobalRotation;

        //Save other items in scene (pickups, enemies) to the array of SavedData
        var savedDataArr = new Array<SavedData>();

        GetTree().CallGroup("Persist", "OnSave", savedDataArr);

        String path = game.root.GetCurrentLevelPath();

        //Add the current scene to the level buffer, which already contains data for other scenes from SaveLevelToBuffer method. 
        if (levelBuffer.ContainsKey(path))
        {
            levelBuffer[path] = savedDataArr;
        }
        else
        {
            levelBuffer.Add(path, savedDataArr);
        }

        savedGame.levelsData = levelBuffer;

        ResourceSaver.Save(savedGame, saveFilePath);

    }

    bool CheckSaveFileExists()
    {
        return ResourceLoader.Exists(saveFilePath);
    }

    public void LoadGame(string saveFile)
    {
        //Otherwise unable to load if game initialised with zero saves, due to saveCount check 
        saveCount = GetSaveCount();

        //Shouldn't be possible to equal zero if we've made it this far. 
        if (saveCount < 1)
        {
            return;
        }

        loadFilePath = saveFolderPath + saveFile;

        //ReplaceDeep cache mode prevents C# from just using an existing copy of the resource in cache. 
        SavedGame savedGame = ResourceLoader.Load(loadFilePath, cacheMode: ResourceLoader.CacheMode.ReplaceDeep) as SavedGame;
        if (savedGame == null)
        {
            GD.Print("No save game found.");
            return;
        }
        game.root.LoadLevel(savedGame.level);

        GetTree().CallGroup("Persist", "OnBeforeLoad");

        //Load player data
        Vector3 newPos = savedGame.playerPos;
        player.GlobalPosition = newPos;
        player.health = savedGame.playerHealth;
        player.head.GlobalRotation = savedGame.playerHeadRot;
        player.cam.GlobalRotation = savedGame.playerCamRot;

        Array<Interactable> interInv = [];

        foreach (Variant var in savedGame.playerInv)
        {
            interInv.Add((Interactable)var);
        }
        player.playerData.SetInv(interInv);

        //Load levelsData, this array contains data for all levels in the save file. 
        foreach (SavedData item in savedGame.levelsData[savedGame.level])
        {
            Node restoredNode = ResourceLoader.Load<PackedScene>(item.scenePath).Instantiate();

            game.root.currentLevel.AddChild(restoredNode);

            if (restoredNode is SaveableNode saveableNode)
            {
                saveableNode.OnLoad(item);
            }
        }

        //Set levelBuffer to levelData. When switching levels this buffer will be accessed, ensuring level state is correct. 
        levelBuffer = savedGame.levelsData;
    }
    
    public void LoadLevelFromBuffer(String levelPath)
    {    
        //Same logic for loading the levelsData in LoadGame function. 
        if (levelBuffer.TryGetValue(levelPath, out Array<SavedData> value))
        {
            foreach (SavedData item in value)
            {
                Node restoredNode = ResourceLoader.Load<PackedScene>(item.scenePath).Instantiate();

                game.root.currentLevel.AddChild(restoredNode);

                if (restoredNode is SaveableNode saveableNode)
                {
                    saveableNode.OnLoad(item);
                }
            }

        }
        else
        {
            GD.Print("No path found for level: ", levelPath);
        }

    }

    //*Temporary solution --> see https://forum.godotengine.org/t/embedding-a-screenshot-thumbnail-in-a-save-game/18079/3 to store image in metadata JSON file. 
    public Image GetSaveImage()
    {
        UpdateSaveFilePath();

        if (ResourceLoader.Exists(saveFilePath))
        {
            SavedGame savedGame = ResourceLoader.Load(saveFilePath, cacheMode: ResourceLoader.CacheMode.ReplaceDeep) as SavedGame;

            return savedGame.viewportImage;
        }
        return null;
    }
}