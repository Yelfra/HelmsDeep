// Local Headers
#include "Shader.h"
#include "Camera.h"
#include "ParticleSystem.h"

// System Headers
//#include <glad/glad.h>
//#include <GLFW/glfw3.h>
//
//#include <glm/glm.hpp>
//#include <glm/gtc/matrix_transform.hpp>
//
//#include <assimp/Importer.hpp>      
//#include <assimp/scene.h>           
//#include <assimp/postprocess.h> 

// Standard Headers
#include <iostream>
#include <cstdlib>
#include <vector>

// Window width & height
GLFWwindow* window;
unsigned int window_width = 1280;
unsigned int window_height = 800;

void framebuffer_size_callback(GLFWwindow* window, int width, int height) {
	window_width = width;
	window_height = height;

	// Update viewport
	glViewport(0, 0, width, height);
}

void initGLFW() {
	// Initializing GLFW
	glfwInit();

	// Initializing Window
	window = glfwCreateWindow(window_width, window_height, "Particles", nullptr, nullptr);
	if(window == nullptr) {
		std::cout << stderr << "Failed to Create Window, OpenGL Context" << std::endl;
		exit(EXIT_FAILURE);
	}
	glfwMakeContextCurrent(window);

	// Loading configurations for OpenGL with glad
	gladLoadGL();
	// VSYNC
	glfwSwapInterval(0);
	// Adjusting window dimensions when changed
	glfwSetFramebufferSizeCallback(window, framebuffer_size_callback);

	// Setting up the background colour of newly loaded frames
	glClearColor(0.1, 0.1, 0.1, 0.1);
	// Enabling Z-buffers
	glEnable(GL_DEPTH_TEST);
	// Enabling face culling
	//glEnable(GL_CULL_FACE);

	// Randomization seed
	srand(glfwGetTime());
}

int main(int argc, char* argv[]) {
	initGLFW();

	/*********************************************************************************************/
	// Setting up Shader
	Shader* shader = new Shader(argv[0], "shader");

	// Setting up Camera
	Camera* camera = new Camera(window_width, window_height, glm::vec3(0.0f, 0.0f, 2.0f));

	// Setting up ParticleSystem
	ParticleSystem* particleSystem = new ParticleSystem(100, argv[0]);

	/*********************************************************************************************/
	// Keeping the window open
	while(!glfwWindowShouldClose(window)) {
		// Clearing the new frame with background colour
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		// Camera input
		camera->registerInput(window);
		camera->updateMatrixGeometry(45.0f, 0.1f, 1000.0f, shader, "cameraMatrix");

		// Drawing particles
		particleSystem->render(shader);
		particleSystem->update(0.001f);

		// Swapping buffers, processing window events
		glfwSwapBuffers(window);
		glfwPollEvents();

		if(glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS) {
			glfwSetWindowShouldClose(window, true);
		}
	}

	/*********************************************************************************************/
	// Deleting
	shader->~Shader();
	camera->~Camera();
	particleSystem->~ParticleSystem();

	// Terminating GLFW
	glfwTerminate();

	return EXIT_SUCCESS;
}
