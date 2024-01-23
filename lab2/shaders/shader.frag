#version 330 core
out vec4 FragColor;

in vec2 geomTexCoords;
uniform sampler2D particleTexture;

void main() {
    vec4 texColor = texture(particleTexture, geomTexCoords);

    if (texColor.a < 0.1) discard;

    FragColor = texColor;  
} 