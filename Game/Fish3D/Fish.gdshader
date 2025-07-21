shader_type spatial;

uniform float speed = 5.0; // time_scale
uniform float sway = 0.01; // side_to_side

void vertex() {
    float time = TIME * speed;
    VERTEX.x += cos(time) * sway;
}

void fragment() {
    ALPHA = 0.5;
}
