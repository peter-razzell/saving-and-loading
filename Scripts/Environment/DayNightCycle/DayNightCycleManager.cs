using Godot; 

public partial class DayNightCycleManager : Node
{

    float timeOfDay = 0f; // Current time of day in seconds

    [Export]
    float lerpSpeed = 1000f; 

    [Export]
    float dayLength = 240f; // Length of a full day in seconds

    DirectionalLight3D sun;

    [Export]
    Environment actualEnvironment;
    
    [Export]
    Environment dayEnvironmentParams;

    [Export]
    Environment nightEnvironmentParams;


    public override void _Ready()
    {

       

        base._Ready();
    }

    public override void _Process(double delta)
    {
        
        timeOfDay += (float)delta; // Increment time of day based on delta time

        if (timeOfDay >= dayLength)
        {
            timeOfDay = 0f; // Reset time of day after a full cycle
        }

        if(actualEnvironment != null)
        {
            if(IsInstanceValid(sun))
            {
                RotateSun(delta); 

            }
            else
            {
                sun = Root.Instance.currentLevel.ReturnLevelSunOrNull(); 
            }

            //have a "morning" and "evening" environment which massively ups the R value on sun / fog glow - makes a fake "sunrise"
            if(timeOfDay < dayLength/2)
            {
                if(sun.LightEnergy < 1.8f)
                {
                    sun.LightEnergy = float.Lerp(sun.LightEnergy, 1.8f, lerpSpeed / 10 * (float)delta);
                }
                
                LerpToEnvironment(dayEnvironmentParams, delta);
                
            }
            else
            {
                if(sun.LightEnergy > 0.1f)
                {
                    sun.LightEnergy = float.Lerp(sun.LightEnergy, 0.1f, lerpSpeed /10 * (float)delta);
                }

                LerpToEnvironment(nightEnvironmentParams, delta);
            }

            
            
            
        }
        else
        {
            actualEnvironment = Game.Instance.colourDitherShader.GetDitherShaderCamera().Environment; 

        }


        base._Process(delta);
    }

    public void LerpToEnvironment(Environment targetEnv, double delta)
    {
      
        

      
        actualEnvironment.AmbientLightEnergy = float.Lerp(actualEnvironment.AmbientLightEnergy, targetEnv.AmbientLightEnergy, (float) delta * lerpSpeed * 10); 



        actualEnvironment.BackgroundColor.Lerp(targetEnv.BackgroundColor, (float) delta * lerpSpeed); 
        actualEnvironment.BackgroundEnergyMultiplier = float.Lerp(actualEnvironment.BackgroundEnergyMultiplier, targetEnv.BackgroundEnergyMultiplier, 
        (float)delta * lerpSpeed); 

        actualEnvironment.AmbientLightColor.Lerp(targetEnv.AmbientLightColor, (float) delta * lerpSpeed);
        actualEnvironment.FogLightEnergy = float.Lerp(actualEnvironment.FogLightEnergy, targetEnv.FogLightEnergy, (float)delta * lerpSpeed); 
        actualEnvironment.FogLightColor.Lerp(targetEnv.FogLightColor, (float)delta * lerpSpeed); 


    }

    // public void LerpToNight(double delta)
    // {
     
       


        
    //     actualEnvironment.AmbientLightEnergy  = float.Lerp(actualEnvironment.AmbientLightEnergy, nightEnvironmentParams.AmbientLightEnergy, (float) delta * lerpSpeed); 


    //     actualEnvironment.BackgroundColor.Lerp(nightEnvironmentParams.BackgroundColor, (float) delta * lerpSpeed); 
    //     actualEnvironment.BackgroundEnergyMultiplier = float.Lerp(actualEnvironment.BackgroundEnergyMultiplier, nightEnvironmentParams.BackgroundEnergyMultiplier,
    //     (float)delta * lerpSpeed); 
    //     actualEnvironment.AmbientLightColor.Lerp(nightEnvironmentParams.AmbientLightColor, (float) delta * lerpSpeed);
    //     actualEnvironment.FogLightEnergy = float.Lerp(actualEnvironment.FogLightEnergy, nightEnvironmentParams.FogLightEnergy, (float)delta * lerpSpeed); 
    //     actualEnvironment.FogLightColor.Lerp(nightEnvironmentParams.FogLightColor, (float)delta * lerpSpeed); 
        
    // }

    public void RotateSun(double delta)
    {
        var a = new Quaternion(sun.Transform.Basis); 
        var b = new Quaternion(sun.Transform.Rotated(new Vector3(-1, 0, 0), Mathf.DegToRad(360)/dayLength*(float)delta).Basis); //rotated 

        var c= a.Slerp(b, lerpSpeed * (float)delta); 

        Transform3D t = sun.Transform; 

        t.Basis = new Basis(b); 

        sun.Transform = t; 
    }

    public void SetSun(DirectionalLight3D sun)
    {
        this.sun = sun;
    }

}
