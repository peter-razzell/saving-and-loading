/* 
This shader is under MIT license. Feel free to use, improve and 
change this shader according to your needs and consider sharing 
the modified result to godotshaders.com.
*/
shader_type spatial;
render_mode unshaded; // Typically used for dither effects to keep palette control

uniform sampler2D screen_tex : hint_screen_texture, repeat_disable, filter_nearest;
uniform sampler2D u_dither_tex : repeat_enable, filter_nearest;
uniform sampler2D u_color_tex : filter_nearest;

uniform int u_bit_depth = 8;
uniform float u_contrast = 1.0;
uniform float u_offset = 0.0;
uniform int u_dither_size = 2;

void fragment() {
    // 1. Setup resolution and pixelation
    // In spatial, we use VIEWPORT_SIZE instead of textureSize(TEXTURE)
    vec2 screen_size = VIEWPORT_SIZE / float(u_dither_size);
    vec2 screen_sample_uv = floor(SCREEN_UV * screen_size) / screen_size;
    
    // 2. Sample the screen
    vec3 screen_col = texture(screen_tex, screen_sample_uv).rgb;

    // 3. Calculate luminosity
    float lum = (screen_col.r * 0.299) + (screen_col.g * 0.587) + (screen_col.b * 0.114);

    // 4. Adjust contrast and offset
    lum = (lum - 0.5 + u_offset) * u_contrast + 0.5;
    lum = clamp(lum, 0.0, 1.0);

    // 5. Reduce bit depth
    float bits = float(u_bit_depth);
    lum = floor(lum * bits) / bits;

    // 6. Palette calculations
    ivec2 col_size = textureSize(u_color_tex, 0);
    // Ensure we don't divide by zero if texture is invalid
    float col_x = float(col_size.x / max(col_size.y, 1)) - 1.0; 
    float col_texel_size = 1.0 / max(col_x, 1.0);

    lum = max(lum - 0.00001, 0.0); 
    float lum_lower = floor(lum * col_x) * col_texel_size;
    float lum_upper = (floor(lum * col_x) + 1.0) * col_texel_size;
    float lum_scaled = lum * col_x - floor(lum * col_x); 

    // 7. Dither Mapping
    // We use SCREEN_UV and VIEWPORT_SIZE to ensure the noise aligns with pixels
    ivec2 noise_size = textureSize(u_dither_tex, 0);
    vec2 noise_uv = SCREEN_UV * (VIEWPORT_SIZE / vec2(noise_size));
    float threshold = texture(u_dither_tex, noise_uv).r;

    threshold = threshold * 0.99 + 0.005;

    // 8. Final mix and output
    float ramp_val = lum_scaled < threshold ? 0.0 : 1.0;
    float col_sample = mix(lum_lower, lum_upper, ramp_val);
    
    vec3 final_col = texture(u_color_tex, vec2(col_sample, 0.5)).rgb;

    ALBEDO = final_col;
}