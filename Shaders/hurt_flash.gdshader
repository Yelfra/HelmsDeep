shader_type canvas_item;

void vertex() {
	// Called for every vertex the material is visible on.
}
uniform float flash_intensity : hint_range(0.0, 1.0);
void fragment() {
	// Called for every pixel the material is visible on.
	vec4 input_color = texture(TEXTURE, UV);
	vec4 new_color = vec4(1.0, 1.0, 1.0, input_color.a);
	COLOR = mix(input_color, new_color, flash_intensity);
}

void light() {
	// Called for every pixel for every light affecting the CanvasItem.
}
