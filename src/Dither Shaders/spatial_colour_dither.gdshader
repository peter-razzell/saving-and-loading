shader_type spatial;

uniform sampler2D u_dither_tex : repeat_enable, filter_nearest;

uniform int u_bit_depth = 8;        // bits per channel
uniform float u_contrast = 1.0;
uniform float u_offset = 0.0;
uniform int u_dither_size = 1;

void fragment()
{
    /* --- Pixelation via subviewport resolution --- */
    vec2 screen_size = vec2(textureSize(TEXTURE, 0)) / float(u_dither_size);
    vec2 screen_sample_uv = floor(UV * screen_size) / screen_size;
    vec3 color = texture(TEXTURE, screen_sample_uv).rgb;

    /* --- Contrast / offset in linear colour --- */
    color = (color - 0.5 + u_offset) * u_contrast + 0.5;
    color = clamp(color, 0.0, 1.0);

    /* --- Dither threshold sampling --- */
    ivec2 noise_size = textureSize(u_dither_tex, 0);
    vec2 noise_uv = UV * vec2(screen_size) / vec2(noise_size);
    float threshold = texture(u_dither_tex, noise_uv).r;
    threshold = threshold * 0.99 + 0.005;

    /* --- Per-channel colour quantisation with dithering --- */
    float steps = float(u_bit_depth);

    vec3 dithered;
    dithered.r = floor(color.r * steps + threshold) / steps;
    dithered.g = floor(color.g * steps + threshold) / steps;
    dithered.b = floor(color.b * steps + threshold) / steps;

    COLOR = vec4(dithered, 1.0);
}
