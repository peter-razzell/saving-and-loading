shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, filter_nearest;
uniform sampler2D u_dither_tex : repeat_enable, filter_nearest;
uniform sampler2D u_color_tex_r : filter_nearest;
uniform sampler2D u_color_tex_g : filter_nearest;
uniform sampler2D u_color_tex_b : filter_nearest;
uniform sampler2D u_color_tex_gr : filter_nearest;


uniform int u_bit_depth;
uniform float u_contrast;
uniform float u_offset;
uniform int u_dither_size;

void fragment()
{
    // Calculate the pixelated UV first
    vec2 screen_size = vec2(textureSize(TEXTURE, 0)) / float(u_dither_size);
    vec2 pixelated_uv = floor(UV * screen_size) / screen_size;

    // Calculate the screen-space version of that pixelated UV
    // This ensures we pick the palette based on the center of the pixelated block
    vec2 pixelated_screen_uv = floor(SCREEN_UV * (vec2(1.0) / SCREEN_PIXEL_SIZE / float(u_dither_size))) * (SCREEN_PIXEL_SIZE * float(u_dither_size));


	vec3 screen_col = texture(TEXTURE, pixelated_uv).rgb;

    // 3. Calculate Luminosity
    float lum = (screen_col.r * 0.299) + (screen_col.g * 0.587) + (screen_col.b * 0.114);
    lum = (lum - 0.5 + u_offset) * u_contrast + 0.5;
    lum = clamp(lum, 0.0, 1.0);

    // Banding
    float bits = float(u_bit_depth);
    lum = floor(lum * bits) / bits;

    // 4. Palette Math (Assuming both palettes are the same size)
    ivec2 col_size = textureSize(u_color_tex_r, 0);
    float col_x = float(col_size.x / col_size.y) - 1.0;
    float col_texel_size = 1.0 / col_x;

    lum = max(lum - 0.00001, 0.0);
    float lum_lower = floor(lum * col_x) * col_texel_size;
    float lum_upper = (floor(lum * col_x) + 1.0) * col_texel_size;
    float lum_scaled = lum * col_x - floor(lum * col_x);

    // 5. Dithering
    ivec2 noise_size = textureSize(u_dither_tex, 0);
    vec2 inv_noise_size = vec2(1.0 / float(noise_size.x), 1.0 / float(noise_size.y));
    vec2 noise_uv = UV * inv_noise_size * screen_size;
    float threshold = texture(u_dither_tex, noise_uv).r;
    threshold = threshold * 0.99 + 0.005;

    float ramp_val = lum_scaled < threshold ? 0.0f : 1.0f;
    float col_sample = mix(lum_lower, lum_upper, ramp_val);

	// Sample the control layer using the PIXELATED uv

    // 6. THE FIX: Sample BOTH palettes
    vec3 pal_r = texture(u_color_tex_r, vec2(col_sample, 0.5)).rgb;
    vec3 pal_g = texture(u_color_tex_g, vec2(col_sample, 0.5)).rgb;
	vec3 pal_b = texture(u_color_tex_b, vec2(col_sample, 0.5)).rgb;
	vec3 pal_gr = texture(u_color_tex_gr, vec2(col_sample, 0.5)).rgb;

    // 7. Choose the color based on the red/green check
    vec3 final_col;
    if (screen_col.r < 0.1 && screen_col.g < 0.1 && screen_col.b < 0.1){
		final_col = pal_gr; 
	}
	
	else if (screen_col.r > screen_col.g && screen_col.r > screen_col.b) {
    	final_col = pal_r;
	}
	else if (screen_col.b > screen_col.r && screen_col.b > screen_col.g) {
		final_col = pal_b;

	}
	else if (screen_col.g > screen_col.b && screen_col.g > screen_col.r){
		final_col = pal_g;
	}
	//COLOR.rgb = final_col;
	//
	//vec2 grid_uv = round(UV * float(40)) / float(40);
	//
	//vec3 text = texture(final_col, grid_uv).rgb;


    COLOR.rgb = final_col;
}