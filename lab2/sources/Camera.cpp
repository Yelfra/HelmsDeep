#include "Camera.h"

Camera::Camera(int width, int height, glm::vec3 position) {
	this->width = width;
	this->height = height;
	this->position = position;
}

Camera::~Camera() {}

void Camera::updateMatrixGeometry(float FOV_degrees, float plane_near, float plane_far, Shader* shader, const char* uniformLocation) {
	
	glm::mat4 viewMatrix = glm::lookAt(position, position + orientation, upDirection);
	glm::mat4 projectionMatrix = glm::perspective(glm::radians(FOV_degrees), static_cast<float>(width / height), plane_near, plane_far);

	GLint uniformLocation_cameraMatrix = glGetUniformLocation(shader->ID, uniformLocation);
	glUniformMatrix4fv(uniformLocation_cameraMatrix, 1, GL_FALSE, glm::value_ptr(projectionMatrix * viewMatrix));
}

void Camera::registerInput(GLFWwindow* window) {
	// Movement - WASD
	if(glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS) {
		position += speed * orientation;
	}
	if(glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS) {
		position += speed * -glm::normalize(glm::cross(orientation, upDirection));
	}
	if(glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS) {
		position += speed * -orientation;
	}
	if(glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS) {
		position += speed * glm::normalize(glm::cross(orientation, upDirection));
	}

	// Movement - Space Ctrl
	if(glfwGetKey(window, GLFW_KEY_SPACE) == GLFW_PRESS) {
		position += speed * upDirection;
	} 
	else if(glfwGetKey(window, GLFW_KEY_LEFT_CONTROL) == GLFW_PRESS) {
		position += speed * -upDirection;
	}
	// Speed up
	if(glfwGetKey(window, GLFW_KEY_LEFT_SHIFT) == GLFW_PRESS) {
		speed = SPEED_FAST;
	} else {
		speed = SPEED_DEFAULT;
	}

	// Orientation - Mouse
	if(glfwGetMouseButton(window, GLFW_MOUSE_BUTTON_LEFT) == GLFW_PRESS) {
		// Hiding cursor when pressing button
		glfwSetInputMode(window, GLFW_CURSOR, GLFW_CURSOR_HIDDEN);

		// Fetching cursor position coordinates
		double cursor_x;
		double cursor_y;
		glfwGetCursorPos(window, &cursor_x, &cursor_y);

		// Panning camera
		float rotation_x = sensitivity * static_cast<float>(cursor_y - height / 2);
		float rotation_y = sensitivity * static_cast<float>(cursor_x - width / 2);
		
		// Looking up/down
		glm::vec3 orientation_x = glm::rotate(orientation,
											  glm::radians(-rotation_x),
											  glm::normalize(glm::cross(orientation, upDirection)));
		
		if(glm::angle(orientation_x, upDirection) > glm::radians(5.0f) || glm::angle(orientation_x, -upDirection) > glm::radians(5.0f)) {
			orientation = orientation_x;
		}

		// Looking left/right
		orientation = glm::rotate(orientation,
								  glm::radians(-rotation_y),
								  upDirection);

		// Placing the cursor in the center of the window
		glfwSetCursorPos(window, (width / 2), (height / 2));

	} else if(glfwGetMouseButton(window, GLFW_MOUSE_BUTTON_LEFT) == GLFW_RELEASE) {
		// Revealing cursor when button is released
		glfwSetInputMode(window, GLFW_CURSOR, GLFW_CURSOR_NORMAL);
	}
}