shader_type canvas_item;

uniform sampler2D v1col : source_color, filter_nearest;
uniform sampler2D v1depth : source_color, filter_nearest;
uniform sampler2D v2col : source_color, filter_nearest;
uniform sampler2D v2depth : source_color, filter_nearest;
uniform int pixel_size : hint_range(1, 16) = 2; // Size of the 'chunks'

// The depth textures from your two viewports

void fragment() {

	// 1. Calculate the resolution of the screen
    vec2 size = vec2(textureSize(v1col, 0));

    // 2. Snap the UV to the nearest pixel grid
    // We multiply by size, floor it to get whole pixels, then divide back
    vec2 pixel_uv = floor(UV * size / float(pixel_size)) / (size / float(pixel_size));

    // 3. USE pixel_uv instead of UV for all your texture samples below
    vec4 col1 = texture(v1col, pixel_uv);
    vec4 col2 = texture(v2col, pixel_uv);
    float depth1 = texture(v1depth, pixel_uv).x;
    float depth2 = texture(v2depth, pixel_uv).x;

	COLOR = col1;

	if (col2.a < 0.99) {
    col2.a = 0.0;
	}

	if (col1.a == 0.0) depth1 = 1.0;
    if (col2.a == 0.0) depth2 = 1.0;

	if(depth2 > depth1){
		COLOR = col2;
	}
}