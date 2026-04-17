shader_type canvas_item;

uniform float minBrightness: hint_range(0.0, 1.0, 0.01) = 0.5;
uniform float minGlowStrength: hint_range(0.0, 8.0, 0.01) = 2.0;
uniform float maxGlowStrength: hint_range(0.0, 8.0, 0.01) = 2.0;

uniform float speed = 1;

void fragment() 
{
	float brightness = 0.2126 * COLOR.r + 0.7152 * COLOR.g + 0.0722 * COLOR.b;
	if (brightness > minBrightness)
	{
		COLOR = texture(TEXTURE, UV) * COLOR * mix(minGlowStrength, maxGlowStrength, abs(sin(TIME * speed)));
	}
}
