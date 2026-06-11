using Godot;
using System;
using System.Threading.Tasks;


public partial class ColourDitherShader : Control
{
    
    [Export]
    Camera3D Origin;

    [Export]
    Camera3D Target;

    [Export]
    Camera3D Target_b; 

    [Export]
    SubViewport viewport; 

    [Export]
    SubViewportContainer ditherViewport;


    public async Task _Ready()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); 

        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {

        Target.GlobalTransform = Origin.GlobalTransform; 
        if(!(Target_b is null))
        {
                    Target_b.GlobalTransform = Origin.GlobalTransform; 
        }

        base._PhysicsProcess(delta);
    }

    public void SetViewPort()
    {
        var tex = viewport.GetTexture();

        var mat = ditherViewport.Material; 

        ((ShaderMaterial)mat).SetShaderParameter("col_screen_tex", tex); 

    }

}
