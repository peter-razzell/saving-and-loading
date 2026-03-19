//Gemini 

shader_type spatial;
render_mode unshaded;

uniform sampler2D depth_tex : hint_depth_texture, filter_nearest;

void fragment() {
    float depth = texture(depth_tex, SCREEN_UV).x;
    ALBEDO = vec3(depth); // Outputs depth as black-and-white pixels
}

//void vertex(){
	//POSITION = vec4(VERTEX, 1.0);
//}