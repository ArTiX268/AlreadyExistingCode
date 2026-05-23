shader_type canvas_item;

uniform float size: hint_range(0.0, 1.0, 0.01);
uniform float opacity: hint_range(0.0, 1.0, 0.01);
uniform vec3 color: source_color;

const vec2 center = vec2(0.5);
const float maxDistance = length(center);

void fragment() 
{
	float distanceToCenter = length(UV - center);
	if (maxDistance - distanceToCenter < size)
	{
		COLOR *= vec4(color, mix(1, 0, (maxDistance - distanceToCenter) / size));
	}
	else
		COLOR.a = 0.0;
}
