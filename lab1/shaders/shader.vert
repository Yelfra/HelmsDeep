#version 330 core
layout (location = 0) in vec3 aPos;

uniform mat4 cameraMatrix;
uniform mat4 animationMatrix;

out vec3 color;

void main() {
    color = vec3(0.5,0,1);
    //gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(aPos, 1.0);

    vec4 animatedPosition = animationMatrix * vec4(aPos, 1.0);

    gl_Position = cameraMatrix * animatedPosition;
    //gl_Position = cameraMatrix * vec4(aPos, 1.0);
}