#version 330 core
layout (location = 0) in vec3 aPos;
out vec3 color;

uniform mat4 cameraMatrix;

void main() {
    color = vec3(0,1,0);
    gl_Position = cameraMatrix * vec4(aPos, 1.0);
    //gl_Position = vec4(aPos, 1.0);
}