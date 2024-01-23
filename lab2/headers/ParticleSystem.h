#pragma once

#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>

#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <glm/gtx/rotate_vector.hpp>
#include <glm/gtx/vector_angle.hpp>

#include <assimp/Importer.hpp>      
#include <assimp/scene.h>           
#include <assimp/postprocess.h> 

#include <chrono>
#include <thread>

#include <vector>
#include <glm/glm.hpp>

#include "Shader.h"

struct Particle {
    glm::vec3 position;
    GLuint texture;
    glm::vec3 velocity;
    float life;
    float size;
};

class ParticleSystem {
private:
    std::vector<Particle> particles;
    GLuint VAO, VBO;
    GLuint texture;

public:
    ParticleSystem(int numParticles, char* path);
    ~ParticleSystem();

    void update(float deltaTime);
    void render(Shader* shader);

    GLuint loadTexture(char* path);

    float getRandomFloat(float min, float max);
};
