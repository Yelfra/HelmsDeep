#include "ParticleSystem.h"

#define STB_IMAGE_IMPLEMENTATION
#include "stb_image.h"

ParticleSystem::ParticleSystem(int numParticles, char* path) {
	particles.resize(numParticles);

	texture = loadTexture(path);

	// Initialize particles with random position, velocity, lifespan, texture coords
	for(auto& particle : particles) {
		particle.position = glm::vec3(getRandomFloat(-0.5, 0.5),
									  getRandomFloat(-0.5, 0.5),
									  getRandomFloat(-0.5, 0.5));
		particle.texture = texture;
		particle.velocity = glm::vec3(getRandomFloat(-0.5, 0.5),
									  getRandomFloat(-0.5, 0.5),
									  getRandomFloat(-0.5, 0.5));
		particle.life = getRandomFloat(25, 50);
		particle.size = getRandomFloat(1, 5);
	}

	// Initialize VAO and VBO for rendering
	glGenVertexArrays(1, &VAO);
	glGenBuffers(1, &VBO);

	glBindVertexArray(VAO);
	glBindBuffer(GL_ARRAY_BUFFER, VBO);
	// Set vertex attribute pointers
	glBufferData(GL_ARRAY_BUFFER, particles.size() * sizeof(Particle), particles.data(), GL_DYNAMIC_DRAW);
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(Particle), (void*)offsetof(Particle, position));
	glEnableVertexAttribArray(0);

	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glBindVertexArray(0);
}

ParticleSystem::~ParticleSystem() {
	glDeleteVertexArrays(1, &VAO);
	glDeleteBuffers(1, &VBO);
}

void ParticleSystem::update(float deltaTime) {
	// Update particle positions based on velocity
	for(int i = 0; i < particles.size(); i++) {
		particles[i].position += particles[i].velocity * deltaTime;

		// Randomize velocity
		particles[i].velocity = glm::vec3(getRandomFloat(-0.5, 0.5),
										  getRandomFloat(-0.5, 0.5),
										  getRandomFloat(-0.5, 0.5));

		// Update particles[i] lifespan, RESET
		particles[i].life -= deltaTime;
		if(particles[i].life < 0.0f) {
			particles[i].position = glm::vec3(getRandomFloat(-0.5, 0.5),
										  getRandomFloat(-0.5, 0.5),
										  getRandomFloat(-0.5, 0.5));
			//particles[i].texture = texture;
			particles[i].life = getRandomFloat(25, 50);
			particles[i].size = getRandomFloat(1, 5);
		}
	}

	// Update VBO
	glBindBuffer(GL_ARRAY_BUFFER, VBO);
	glBufferSubData(GL_ARRAY_BUFFER, 0, particles.size() * sizeof(Particle), particles.data());
	glBindBuffer(GL_ARRAY_BUFFER, 0);
}

void ParticleSystem::render(Shader* shader) {
	glUseProgram(shader->ID);

	// Bind texture
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, texture);
	glUniform1i(glGetUniformLocation(shader->ID, "particleTexture"), 0);

	// Render particles
	glBindVertexArray(VAO);
	glBindBuffer(GL_ARRAY_BUFFER, VBO);
	glDrawArrays(GL_POINTS, 0, particles.size());
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glBindVertexArray(0);

	// Reset to the default shader program
	glUseProgram(0);
}

GLuint ParticleSystem::loadTexture(char* path) {
	std::string pathStr = path;
	std::string texPath(pathStr, 0, pathStr.find_last_of("\\/"));
	texPath.append("\\resources\\smoke-particle.png"); //smoke.bmp");

	GLuint textureID;
	glGenTextures(1, &textureID);

	int width, height, nrChannels;
	stbi_set_flip_vertically_on_load(true);
	unsigned char* data = stbi_load(texPath.c_str(), &width, &height, &nrChannels, 0);

	if(data) {
		GLenum format = GL_RGBA;
		/*if(nrChannels == 1)
			format = GL_RED;
		else if(nrChannels == 3)
			format = GL_RGB;
		else if(nrChannels == 4)
			format = GL_RGBA;*/

		glBindTexture(GL_TEXTURE_2D, textureID);
		glTexImage2D(GL_TEXTURE_2D, 0, format, width, height, 0, format, GL_UNSIGNED_BYTE, data);
		glGenerateMipmap(GL_TEXTURE_2D);

		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	} else {
		std::cout << "Failed to load texture" << std::endl;
	}
	stbi_image_free(data);

	return textureID;
}

float ParticleSystem::getRandomFloat(float min, float max) {
	return min + static_cast<float>(rand()) / static_cast<float>(RAND_MAX / (max - min));
}