using Godot;
using System;

public partial class ShaderLayerCompositor : ColorRect
{
	// Called when the node enters the scene tree for the first time.
	SubViewportContainer subViewportA;
	SubViewportContainer subViewportB; 
	public override void _Ready()
	{

		this.SetInstanceShaderParameter("tex_a", subViewportA);
		this.SetInstanceShaderParameter("tex_b", subViewportB);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
