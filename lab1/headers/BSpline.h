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

#include <iostream>
#include <vector>

#include "Shader_new.h"
#include "Camera.h"

#define BSPLINE_STEPS 100;

class BSpline {
private:
	std::vector<float> vertices; // convert to glm::vec3 if possible
	std::vector<float> tangent_vertices; // convert to glm::vec3 if possible
	std::vector<unsigned int> vertex_indices;

	std::vector<glm::vec3> controlPoints;

	bool resetCamera = true;

	int current_index = 0;
	glm::vec3 previous_point = glm::vec3(0.0f);

	float B_factor = 1.0f / 6.0f;
	glm::mat4x4 B_matrix = glm::mat4x4(
		glm::vec4(-1, 3, -3, 1),
		glm::vec4(3, -6, 3, 0),
		glm::vec4(-3, 0, 3, 0),
		glm::vec4(1, 4, 1, 0));

	float tangent_factor = 1.0f / 2.0f;
	glm::mat4x3 tangent_matrix = glm::mat3x4(
		glm::vec4(-1, 3, -3, 1),
		glm::vec4(2, -4, 2, 0),
		glm::vec4(-1, 0, 1, 0));

	float spline_time = 0.0f;
	int spline_vertex_index = 0;

	int bSplineDeg = 3;

	int bSplineSteps = BSPLINE_STEPS;
	float bSplineStepSize = 1.0f / BSPLINE_STEPS;

public:
	BSpline(char* path, int degree);
	~BSpline();

	GLuint VAO;
	GLuint VBO;
	GLuint VBO_tang;
	GLuint EBO;

	void setupBuffers();

	std::vector<float> getVertices();
	unsigned int getVerticesNum();
	std::vector<float> getTangVertices();
	unsigned int getTangVerticesNum();
	std::vector<unsigned int> getIndices();
	unsigned int getIndicesNum();

	void updateAnimation(Shader* shader, Camera* camera);

	glm::vec3 bSplinePoint();
	void loadBSpline();
	int factorial(int number);

	void pushControlPoint(glm::vec3 coordinates);
	void popControlPoint();

	void resetBezier();
};

