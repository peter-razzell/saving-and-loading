using Godot;
using Godot.Collections; 
using System;
using System.Linq;
using System.Threading.Tasks;

//Holds an array of entrances and exits to the level. s
public partial class Level : Node3D
{    
	/// <summary>
    /// This ID is used to get the weather state for the region this level is a part of (1 region can have multiple levels. E.g indoors) 
    /// </summary>
	[Export]
	public string RegionID; 
	public Array<LevelExit> levelExits = new Array<LevelExit>();

	public Array<LevelEntrance> levelEntrances = new Array<LevelEntrance>();

	// RegionWeatherStates regionWeatherStates; 

	string RegionName; //Region is used for weather.

	bool Indoors; //if true then has an ambient indoor temperature modifier? 

	[Signal]
	public delegate void OnLevelLoadedEventHandler(Level level); 

	public override void _Ready() {

		// regionWeatherStates = RegionWeatherStatesLookup.GetLevelWeatherStatesWithID(RegionID);

		foreach (Node node in GetTree().GetNodesInGroup("LevelExit"))
		{
			levelExits.Add((LevelExit)node);
		}
		foreach (Node node in GetTree().GetNodesInGroup("LevelEntrance"))
		{
			levelEntrances.Add((LevelEntrance)node);
		}

		EmitSignal(SignalName.OnLevelLoaded, this); //emitting the signal before connected in Root ready function? 

		WeatherSwitchBus.GetInstance().UpdateLevelWeatherState(RegionID);

		WeatherSwitchBus.GetInstance().SwitchWeatherMessage(); 

		base._Ready();
	}
	
}
