using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Channels;

public partial class AudioManager : Node
{

    int volume; //set in UI settings 

    [Export]
    int channels = 8;

    string bus = "master"; //not sure? 

    static Array<AudioStreamPlayer> availablePlayers = new Array<AudioStreamPlayer>();

    static Array<AudioStreamPlayer> activePlayers = new Array<AudioStreamPlayer>();


    static AudioStreamPlayer footstepPlayer = new AudioStreamPlayer(); 

    static Array<string> queue = new Array<string>();


    public override void _Ready()
    {

        // GD.Print("footstep player parent = ", footstepPlayer.GetParent()); 
        AddChild(footstepPlayer);

        // footstepPlayer.Reparent(this); 

        for (int i = 0; i < channels; i++)
        {
            var audioPlayer = new AudioStreamPlayer();
            availablePlayers.Add(audioPlayer);
            AddChild(audioPlayer);
            audioPlayer.Finished += () => OnStreamFinished(audioPlayer); //chatgpt helped with this line. Never would I have thought to use lambda! 


            audioPlayer.Bus = bus; //I guess its an important parameter for audio players
        }
        AudioStream footstep = (AudioStream)ResourceLoader.Load("res://Assets/Sound/GDC/BluezoneCorp - Stone Impact/Bluezone_BC0297_stone_impact_steel_bar_01_010.wav");

        footstepPlayer.Stream = footstep;


    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("debug_mute"))
        {
            GD.Print("mute pressed"); 
            if (volume == 100)
            {
                volume = 0;
                foreach(AudioStreamPlayer player in activePlayers)
                {
                    player.VolumeLinear = 0f; 
                }
            }
            else
            {
                volume = 100; 
                foreach(AudioStreamPlayer player in activePlayers)
                {
                    player.VolumeLinear = 100f; 
                }
            }
        }
        base._Input(@event);
    }

    //Removes audioplayer from active audio players list when audio finishes.
    void OnStreamFinished(AudioStreamPlayer audioPlayer)
    {
        audioPlayer.Stream = null;
        availablePlayers.Add(audioPlayer); 
        activePlayers.Remove(audioPlayer); 
    }

    public static void EndPrevLevelAudio()
    {
        foreach (AudioStreamPlayer player in activePlayers)
        {
            player.Stop();
            player.Stream = null;
            availablePlayers.Add(player);
        }
        activePlayers.Clear();

    }

    //Plays audio, adds sound path to play queue, checks if a player is available and makes one available if not. 
    public static void Play(string soundPath)
    {
        queue.Add(soundPath);
        if (availablePlayers.Count == 0)
        {
            activePlayers[0].Stop();
            availablePlayers.Add(activePlayers[0]); //on the next physics tick, this will be reloaded with the new sound. 
        }
    }
    
    //Plays player footsteps - separate from other level audio
    public static void PlayPlayerSteps()
    {
        footstepPlayer.Play();   
    }

    public override void _PhysicsProcess(double delta)
    {
        
        if(queue.Count > 0 && availablePlayers.Count > 0)
        {
            availablePlayers[0].Stream = (AudioStream)ResourceLoader.Load(queue.First());
            queue.RemoveAt(0); //no pop_front() method for C# lists
            availablePlayers[0].Play();
            activePlayers.Add(availablePlayers[0]); 
            availablePlayers.RemoveAt(0);//just removing from list, not deleting. 

        }
        base._PhysicsProcess(delta);
    }


}
