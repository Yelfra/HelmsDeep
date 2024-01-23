#version 330 core
layout (triangles) in;
layout (triangle_strip, max_vertices = 3) out;

in vec3 color[];
out vec3 geomColor;

void main() {

    mat3 M = mat3(gl_in[0].gl_Position.xyz, gl_in[1].gl_Position.xyz, gl_in[2].gl_Position.xyz);
    if(determinant(M) < 0) {
        //EndPrimitive();
    //    return;
    }
    
    gl_Position = gl_in[0].gl_Position;
    geomColor = color[0];
    EmitVertex();
    
    gl_Position = gl_in[1].gl_Position;
    geomColor = color[1];
    EmitVertex();
    
    gl_Position = gl_in[2].gl_Position;
    geomColor = color[2];
    EmitVertex();
    
    EndPrimitive();
}