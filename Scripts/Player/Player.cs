using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

//TODO tidy up player logic into separate movement and manager classes?
public partial class Player : CharacterBody3D
{
    public static Player Instance {get; private set;}

    Game game; //TODO delete and have everything go through PlayerManager //Really? Won't that make things HARDER to understand not easier?? ARHGH!

    public Marker3D itemSpawnMarker; 

    [Export]
    public int health = 10; 

    public Node3D head;

    public Camera3D cam;

    [Export]
    float speed = 50f, mouseSensitivity = 0.01f, jumpStr = 10f, acceleration = 10f;

    float currentSpeed; 

    float gravity; //set from Game class

    bool gravityToggle;

    bool walking; 

    FootstepEnum footsteps; 
    //Step time attributes used for footsteps
    Vector3 velCache; //previous frame's velocity, used for working out step times. 

    float stepTimer;

    PlayerInteractor interactor; 

    public PlayerData playerData; 
    public override void _Ready()
    {        
        game = GetOwner<Game>();

        gravity = game.gravity; 

        currentSpeed = speed; 

        Input.MouseMode = Input.MouseModeEnum.Captured;//refactor and remove to game class. 

        head = (Node3D)GetNode("%Head");//easy to break. 
        cam = (Camera3D)GetNode("%Camera3D");//moving it around 
        
        playerData = (PlayerData)GetNode("PlayerData");
        interactor = (PlayerInteractor)GetNode("Interactor");

        itemSpawnMarker = (Marker3D)GetNode("%ItemSpawnMarker");

        game.GameLoaded += AddInventorySignal; 

        TerrainChecker terrainChecker = GetNode<TerrainChecker>("TerrainChecker"); 

        terrainChecker.OnSwitchFootstepSound += SwitchFootstepSound; 

        Instance = this; 

    }

    public void AddInventorySignal()
    {
        game.uiManager.OnInventoryDropButtonPressed += RemoveItemFromInv; 
    }

    public bool RemoveItemFromInv(string itemID)
    {
        return playerData.RemoveFromInv(itemID); 
    }

   
    public override void _Input(InputEvent @event) {

        if(@event is InputEventMouseMotion eventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            cam.RotateX(-eventMouseMotion.Relative.Y   * mouseSensitivity);
            head.RotateY(-eventMouseMotion.Relative.X  *mouseSensitivity); 
        }
        base._Input(@event);
    }

    float heightCounter = 0; 

    public override void _PhysicsProcess(double delta)
    {

        Vector3 vel = Velocity;

        PlayerFootsteps(delta, vel); 

        heightCounter += 1; 
        if(heightCounter == 10)
        {
            // GD.Print("updating height vector"); 
            UpdateHeightVector();
            heightCounter = 0; 
    
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
            walking = true; 

            currentSpeed = speed; 
         
            vel.X = Mathf.Lerp(Velocity.X, dir.X * speed, (float)delta * acceleration);
            vel.Z = Mathf.Lerp(Velocity.Z, dir.Z * speed, (float)delta * acceleration);
        }
        else
        {
            if (walking)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, 0, (float)delta*acceleration); 
                vel.X = Mathf.Lerp(Velocity.X, dir.X * currentSpeed, (float)delta * acceleration);
                vel.Z = Mathf.Lerp(Velocity.Z, dir.X * currentSpeed, (float)delta * acceleration);
                if(currentSpeed < 0.0001f) walking = false; 
            }
            else
            {
                vel.X = Mathf.MoveToward(Velocity.X, 0, (float)delta);
                vel.Z = Mathf.MoveToward(Velocity.Z, 0, (float)delta); 
            }
            
        }

        Velocity = vel;
        velCache = vel;

        MoveAndSlide();

        base._PhysicsProcess(delta);
    }

    void PlayerFootsteps(double delta, Vector3 vel)
    {
        
        //sound
        float currentSpeed = vel.Dot(velCache) / 10;
        stepTimer += (float)delta * currentSpeed;
        if (stepTimer > 3f && IsOnFloor())
        {
            AudioManager.PlayPlayerSteps(footsteps);
            stepTimer = 0f;
        }

    }

    //Reset player on level load. 
    public void ResetOnLevelLoad()
    { 
        // playerData.DebugPrintInventory(); 
        interactor.ResetOnLevelLoad(); 
    }

    float heightVector = 0; 
    float prevHeight = 0; 

    public void UpdateHeightVector()
    {
        heightVector = MathF.Abs(GlobalPosition.Y - prevHeight); 

        prevHeight = GlobalPosition.Y; 

            
    }

    public float GetHeightVector()
    {
        return heightVector; 
    }


    public float GetSpeed()
    {
        return speed;
    }

    public float GetJump()
    {
        return jumpStr; 
    }


    void SwitchFootstepSound(string surfaceType)
    {
        if(surfaceType == "grass")
        {
            footsteps = FootstepEnum.grass; 
            
        }
        else
        {
            footsteps = FootstepEnum.rock; 
            
        }
    }
    
}


