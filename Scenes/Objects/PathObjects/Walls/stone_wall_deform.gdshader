shader_type spatial;

uniform sampler2D noise_texture;
uniform float distortion_strength = 0.3;
uniform sampler2D material_tex ;

void vertex() {
    // Use the UV.y (which spans along the path length) to sample noise
    float noise_val = texture(noise_texture, vec2(UV.x, UV.y)).r;
    
    // Displace vertices on X and Z axis based on noise to change thickness
    VERTEX.x = VERTEX.x + (noise_val - 0.5) * distortion_strength;
    VERTEX.z = VERTEX.z + (noise_val - 0.5) * distortion_strength;
	if(noise_val < 0.7){
			VERTEX.y = VERTEX.y + (noise_val - 0.5) * distortion_strength * distortion_strength; 
	}
	vec2 uv_map = vec2(VERTEX.x, VERTEX.y); 
	
	
	
	//VERTEX.z = clamp(VERTEX.z, -2, 2); 
}
void fragment()
{
	
	float col_noise = texture(noise_texture, UV).r; 
	
	vec3 final_col = vec3(texture(material_tex, UV).r, texture(material_tex, UV).g, texture(material_tex, UV).b); 
	
	ALBEDO = final_col; 	
	
}