#pragma once

#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>

#include <assimp/Importer.hpp>      
#include <assimp/scene.h>           
#include <assimp/postprocess.h> 

#include <chrono>
#include <thread>

#include <iostream>
#include <vector>

#define BEZIER_STEPS 100;

class Bezier {
private:
	std::vector<float> vertices; // convert to glm::vec3 if possible
	std::vector<unsigned int> vertex_indices;

	std::vector<glm::vec3> controlPoints;
	int bezierSteps = BEZIER_STEPS;
	float bezierStepSize = 1.0f / BEZIER_STEPS;
	int bezierStep = 0;

	bool updateRequired = false;

public:
	Bezier();
	~Bezier();

	GLuint VAO;
	GLuint VBO;
	GLuint EBO;

	void setupBuffers();
	void updateBuffers();

	std::vector<float> getVertices();
	unsigned int getVerticesNum();
	std::vector<unsigned int> getIndices();
	unsigned int getIndicesNum();

	glm::vec3 bezierPoint();
	void loadBezier();
	int factorial(int number);

	void pushControlPoint(glm::vec3 coordinates);
	void popControlPoint();

	void resetBezier();
};

