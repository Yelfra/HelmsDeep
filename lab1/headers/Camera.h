#pragma once

#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>

#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <glm/gtx/rotate_vector.hpp>
#include <glm/gtx/vector_angle.hpp>

#include <vector>
#include <chrono>
#include <thread>

#include <iostream>

#include "Shader_new.h"
#include "Bezier.h"

#define SPEED_DEFAULT 0.025f
#define SPEED_FAST 0.05f
#define SENSITIVITY_DEFAULT 0.1f;

#define BEZIER_STEPS 100;

class Camera {
private:

	int width, height;

	float speed = SPEED_DEFAULT;
	float sensitivity = SENSITIVITY_DEFAULT;

	std::vector<glm::vec3> controlPoints;
	bool cKeyPressed = false;
	bool xKeyPressed = false;
	bool vKeyPressed = false;
	bool bezierToggle = false;
	float bezierStepSize = 1.0f / BEZIER_STEPS;
	int bezierStep = 0;
	Bezier* bezier;

public:
	glm::vec3 upDirection = glm::vec3(0.0f, 1.0f, 0.0f);
	glm::vec3 position;
	glm::vec3 orientation = glm::vec3(0.0f, 0.0f, 1.0f);

	Camera(int width, int height, glm::vec3 position);
	Camera(int width, int height, glm::vec3 position, Bezier* bezier);
	~Camera();

	void updateMatrixGeometry(float FOV_degrees, float plane_near, float plane_far, Shader* shader, Shader* shader_bSpline, const char* uniform);
	void updateMatrixGeometry2(float FOV_degrees, float plane_near, float plane_far, Shader* shader, Shader* shader_bSpline, const char* uniform);
	void registerInput(GLFWwindow* window);
	glm::vec3 bezierPoint();
	int factorial(int number);
	glm::vec3 getPosition();
};

