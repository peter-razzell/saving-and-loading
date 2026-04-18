using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

//TODO tidy up player logic into separate movement and manager classes?
public partial class Player : CharacterBody3D
{
    public static Player Instance {get; private set;}

    Game game; //TODO delete and have everything go through PlayerManager - a player audio controller would be useful! 

    public Marker3D itemSpawnMarker; 

    public Node3D head;

    public Camera3D cam;
    
    [Export]
    public int health = 10;     

    [Export]
    float speed = 50f, mouseSensitivity = 0.01f, jumpStr = 10f, acceleration = 10f;

    /// <summary>
    /// Self explanatory
    /// </summary>
    float currentSpeed; 

    /// <summary>
    /// Global physics gravity set from Game class. 
    /// </summary>
    float gravity; 

    /// <summary>
    /// Just fun to fly around... 
    /// </summary>
    bool gravityToggle;

    /// <summary>
    /// Is the player walking?
    /// </summary>
    bool walking; 

    /// <summary>
    /// Footstep enum used to determine which footstep sound the AudioManager plays. A class called TerrainChecker sends a signal here to change this.
    /// </summary>
    FootstepEnum footsteps; 

    /// <summary>
    /// Velocity cache - this just caches the previous velocity, used for calculations like height change over time 
    /// </summary>
    Vector3 velCache; 

    /// <summary>
    /// How frequently to play the step sound 
    /// </summary>
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

    public override void _PhysicsProcess(double delta)
    {

        Vector3 vel = Velocity;

        PhysPro_PlayerFootsteps(delta, vel); 

        PhysPro_HeightCounter();

        if (gravityToggle == true)
        {
            gravity = 0;
        }
        else
        {
            gravity = 9.8f;
        }

        Velocity = PhysPro_Movement(Velocity, delta);

        velCache = Velocity;

        MoveAndSlide();

        base._PhysicsProcess(delta);
    }

    //* THE FOLLOWING METHODS ARE CALLED BY _PhysicsProcess()

    /// <summary>
    /// Player footsteps method, calls the player footstep audio every second or so as long as player is moving. 
    /// </summary>
    /// <param name="delta"></param>
    /// <param name="vel"></param>
    void PhysPro_PlayerFootsteps(double delta, Vector3 vel)
    {
        float currentSpeed = vel.Dot(velCache) / 10;
        stepTimer += (float)delta * currentSpeed;
        if (stepTimer > 3f && IsOnFloor())
        {
            AudioManager.PlayPlayerSteps(footsteps);
            stepTimer = 0f;
        }
    }


    float heightCounter = 0; 

    /// <summary>
    /// Method encapsulating the UpdateHeightVector to call it every 10 _PhysicsProcess cycles 
    /// </summary>
    void PhysPro_HeightCounter()
    {
        heightCounter += 1; 
        if(heightCounter == 10)
        {
            UpdateHeightVector();
            heightCounter = 0; 
        }

    }

    /// <summary>
    /// Movement logic for player. Separated to make things cleaner. 
    /// </summary>
    /// <param name="vel"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    Vector3 PhysPro_Movement(Vector3 vel, double delta)
    {
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
        return vel; 
    }


    //* PUBLIC METHODS

    /// <summary>
    /// Reset player on level load. resets interactor currently to removed cached item. 
    /// </summary>
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

    /// <summary>
    /// Switches footstep sound - move out of player. 
    /// </summary>
    /// <param name="surfaceType"></param>
    void SwitchFootstepSound(int surfaceType)
    {
        if(surfaceType == 0)
        {
            footsteps = FootstepEnum.grass;    
        }
        else if(surfaceType == 1)
        {
            footsteps = FootstepEnum.rock; 
        }
        //Add more footstep surface types HERE. 
        else
        {
            footsteps = FootstepEnum.rock; 
        }
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

}