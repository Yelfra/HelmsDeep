#include "Camera.h"

Camera::Camera(int width, int height, glm::vec3 position) {
	// Assigning values
	this->width = width;
	this->height = height;
	this->position = position;
}
Camera::Camera(int width, int height, glm::vec3 position, Bezier* bezier) {
	// Assigning values
	this->width = width;
	this->height = height;
	this->position = position;
	this->bezier = bezier;
}

Camera::~Camera() {}

void Camera::updateMatrixGeometry(float FOV_degrees, float plane_near, float plane_far, Shader* shader, Shader* shader_bSpline, const char* uniformLocation) {

	glm::mat4 viewMatrix = glm::mat4(1.0f);
	glm::mat4 projectionMatrix = glm::mat4(1.0f);

	//if(bezierToggle) {
		//position = bezier->bezierPoint();
		//viewMatrix = glm::lookAt(position, glm::vec3(0.0f), upDirection);
	//} else {
		viewMatrix = glm::lookAt(position, position + orientation, upDirection);
	//}

	projectionMatrix = glm::perspective(glm::radians(FOV_degrees), static_cast<float>(width / height), plane_near, plane_far);

	GLint uniformLocation_cameraMatrix = glGetUniformLocation(shader->ID, uniformLocation);
	glUniformMatrix4fv(uniformLocation_cameraMatrix, 1, GL_FALSE, glm::value_ptr(projectionMatrix * viewMatrix));
}

void Camera::updateMatrixGeometry2(float FOV_degrees, float plane_near, float plane_far, Shader* shader, Shader* shader_bSpline, const char* uniformLocation) {

	glm::mat4 viewMatrix = glm::mat4(1.0f);
	glm::mat4 projectionMatrix = glm::mat4(1.0f);

	//if(bezierToggle) {
		//position = bezier->bezierPoint();
		//viewMatrix = glm::lookAt(position, glm::vec3(0.0f), upDirection);
	//} else {
	viewMatrix = glm::lookAt(position, position + orientation, upDirection);
//}

	projectionMatrix = glm::perspective(glm::radians(FOV_degrees), static_cast<float>(width / height), plane_near, plane_far);


	GLint uniformLocation_cameraMatrix_bSpline = glGetUniformLocation(shader_bSpline->ID, uniformLocation);
	glUniformMatrix4fv(uniformLocation_cameraMatrix_bSpline, 1, GL_FALSE, glm::value_ptr(projectionMatrix * viewMatrix));
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

	// Setting control points for Bezier
	if(glfwGetKey(window, GLFW_KEY_C) == GLFW_PRESS && !cKeyPressed) {
		cKeyPressed = true;
		bezier->pushControlPoint(position);
		//controlPoints.push_back(position);
	} else if(glfwGetKey(window, GLFW_KEY_C) == GLFW_RELEASE && cKeyPressed) {
		cKeyPressed = false;
	}
	// Removing control points for Bezier
	if(glfwGetKey(window, GLFW_KEY_X) == GLFW_PRESS && !xKeyPressed) {
		xKeyPressed = true;
		bezier->popControlPoint();
		//controlPoints.pop_back();
	} else if(glfwGetKey(window, GLFW_KEY_X) == GLFW_RELEASE && xKeyPressed) {
		xKeyPressed = false;
	}
	// Putting Bezier in action
	if(glfwGetKey(window, GLFW_KEY_V) == GLFW_PRESS && !vKeyPressed) {
		vKeyPressed = true;
		bezierToggle = !bezierToggle;
		//bezier->resetBezier();
		//bezierStep = 0;
	} else if(glfwGetKey(window, GLFW_KEY_V) == GLFW_RELEASE && vKeyPressed) {
		vKeyPressed = false;
	}
}

glm::vec3 Camera::bezierPoint() {

	glm::vec3 p = glm::vec3(0.0f);
	if(controlPoints.size() < 2) {
		bezierToggle = false;
		return p;
	}
	float t = bezierStep * bezierStepSize;
	float t_0 = 1.0f - t;
	float sum = 0.0f;
	int n = controlPoints.size();

	float b = 0.0f;
	for(int i = 0; i <= n; i++) {
		int index = i % n; // to loop around back to 0
		//std::cout << "Index:" << index << std::endl;
		b = factorial(n) / (factorial(index) * factorial(n - index)) * glm::pow(t, index) * glm::pow(1.0f - t, n - index);
		
		p.x += b * controlPoints[index].x;
		p.y += b * controlPoints[index].y;
		p.z += b * controlPoints[index].z;
	}

	bezierStep++;
	if(bezierStep == 100) {
		bezierStep = 0;
	}
	std::this_thread::sleep_for(std::chrono::milliseconds(10));
	return p;
}

int Camera::factorial(int number) {
	int result = 1;
	for(int i = 1; i <= number; i++) {
		result *= i;
	}
	return result;
}

glm::vec3 Camera::getPosition() {
	return position;
}