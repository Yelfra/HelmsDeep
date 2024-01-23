#pragma once

#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>

#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <glm/gtx/rotate_vector.hpp>
#include <glm/gtx/vector_angle.hpp>

#include <iostream>

#include "Shader.h"

#define SPEED_DEFAULT 0.00025f
#define SPEED_FAST 0.0005f
#define SENSITIVITY_DEFAULT 0.1f;

class Camera {
private:
	int width, height;

	float speed = SPEED_DEFAULT;
	float sensitivity = SENSITIVITY_DEFAULT;

public:
	glm::vec3 position;
	glm::vec3 orientation = glm::vec3(0.0f, 0.0f, -1.0f);
	glm::vec3 upDirection = glm::vec3(0.0f, 1.0f, 0.0f);

	Camera(int width, int height, glm::vec3 position);
	~Camera();

	void updateMatrixGeometry(float FOV_degrees, float plane_near, float plane_far, Shader* shader, const char* uniform);
	void registerInput(GLFWwindow* window);
};

