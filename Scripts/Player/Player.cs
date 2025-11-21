using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Player : CharacterBody3D
{
    Game game; 

    [Export]
    public int health = 10; 

    public Node3D head;

    public Camera3D cam;

    [Export]
    float speed = 10f, mouseSensitivity = 0.01f, jumpStr = 200f;

    float gravity; //set from Game class

    bool gravityToggle;

    //Step time attributes used for footsteps
    Vector3 velCache; //previous frame's velocity, used for working out step times. 

    float stepTimer;

    PlayerInteractor interactor; 

    public PlayerData playerData; 
    public override void _Ready()
    {
        game = GetOwner<Game>();

        gravity = game.gravity; 

        Input.MouseMode = Input.MouseModeEnum.Captured;//refactor and remove to game class. 


        head = (Node3D)GetNode("Head");//easy to break. 
        cam = (Camera3D)GetNode("Head/Camera3D");
        playerData = (PlayerData)GetNode("PlayerData");
        interactor = (PlayerInteractor)GetNode("Interactor"); 

    }
    
    public override void _Input(InputEvent @event) {

        if(@event is InputEventMouseMotion eventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            cam.RotateX(-eventMouseMotion.Relative.Y   * mouseSensitivity);
            head.RotateY(-eventMouseMotion.Relative.X  *mouseSensitivity); 
        }
        base._Input(@event);
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 vel = Velocity;

        //sound
        float currentSpeed = vel.Dot(velCache) / 10;
        stepTimer += (float)delta * currentSpeed;
        if (stepTimer > 3f && IsOnFloor())
        {
            AudioManager.PlayPlayerSteps();
            stepTimer = 0f;
        }


        if (gravityToggle == true)
        {
            gravity = 0;
        }
        else
        {
            gravity = 9.8f;
        }

        vel.Y -= gravity * (float)delta;


        Vector2 inputDir = Input.GetVector("player_left", "player_right", "player_forward", "player_back", 0f);

        //Jump
        if (Input.IsActionPressed("player_jump") && IsOnFloor())
        {
            if (Input.IsActionJustPressed("player_jump"))
            {
                AudioManager.Play("res://Assets/Sound/GDC/BluezoneCorp - Steampunk Weapon And Textures/Bluezone_BC0296_steampunk_weapon_cannon_shot_013_02.wav");
            }
            GD.Print("jump!");
            vel.Y += jumpStr * (float)delta;
        }
        if (Input.IsActionJustPressed("gravity_toggle"))
        {
            vel.Y = 0f;
            gravityToggle = !gravityToggle;
        }


        Vector3 dir = (head.GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        if (dir != Vector3.Zero)
        {
            vel.X = dir.X * speed;
            vel.Z = dir.Z * speed;
        }
        else
        {
            vel.X = Mathf.MoveToward(Velocity.X, 0, speed);
            vel.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
        }

        Velocity = vel;
        velCache = vel;

        MoveAndSlide();

        base._PhysicsProcess(delta);
    }

    //Reset player on level load. 
    public void ResetOnLevelLoad()
    {
        interactor.ResetOnLevelLoad(); 
    
    }
    
}
