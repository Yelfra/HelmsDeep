#version 330 core
layout (points) in;
layout (triangle_strip, max_vertices = 4) out;

out vec2 geomTexCoords;
//in float size;

void main() {
    vec4 point = gl_in[0].gl_Position;
    point = point / point.w;

    vec2 size = vec2(0.1, 0.1);

    gl_Position = point + vec4(-size.x, -size.y, 0.0, 0.0);
    geomTexCoords = vec2(0, 0);
    EmitVertex();

    gl_Position = point + vec4(size.x, -size.y, 0.0, 0.0);
    geomTexCoords = vec2(1, 0);
    EmitVertex();

    gl_Position = point + vec4(-size.x, size.y, 0.0, 0.0);
    geomTexCoords = vec2(0, 1);
    EmitVertex();

    gl_Position = point + vec4(size.y, size.y, 0.0, 0.0);
    geomTexCoords = vec2(1, 1);
    EmitVertex();
    
    EndPrimitive();
}