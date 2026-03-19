//created by chatgpt!

shader_type canvas_item;

uniform sampler2D tex_a;
uniform sampler2D tex_b;

void fragment() {
    vec4 a = texture(tex_a, UV);
    vec4 b = texture(tex_b, UV);

    COLOR = a + b; // or proper alpha blend
}