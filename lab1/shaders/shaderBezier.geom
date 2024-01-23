#version 330 core
layout (points) in;
layout (points, max_vertices = 1) out;

in vec3 color[];
out vec3 geomColor;

void main() {
    for (int i = 0; i < gl_in.length(); i++) {
        gl_Position = gl_in[i].gl_Position;
        geomColor = color[i];
        EmitVertex();
    }
    EndPrimitive();
}