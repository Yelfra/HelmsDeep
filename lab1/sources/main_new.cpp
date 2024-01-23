// Local Headers
#include "Shader_new.h"
#include "Mesh.h"
#include "Camera.h"
#include "Bezier.h"
#include "BSpline.h"

// System Headers
#include <glad/glad.h>
#include <GLFW/glfw3.h>

#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>

#include <assimp/Importer.hpp>      
#include <assimp/scene.h>           
#include <assimp/postprocess.h> 

#define STB_IMAGE_IMPLEMENTATION
#include "stb_image.h"

// Standard Headers
#include <iostream>
#include <cstdlib>
#include <vector>

// Window width & height
unsigned int window_width = 1280;
unsigned int window_height = 800;

void framebuffer_size_callback(GLFWwindow* window, int width, int height) {
	window_width = width;
	window_height = height;

	// Update viewport
	glViewport(0, 0, width, height);
}

int main(int argc, char * argv[]) {
	/*********************************************************************************************/
	// Initializing GLFW
	glfwInit();
	//glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
	//glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 6);
	//glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
	//glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
	// Initializing Window
	GLFWwindow* window = glfwCreateWindow(window_width, window_height, "7", nullptr, nullptr);
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
	glClearColor(0.15f, 0.1f, 0.1f, 0.1f);
	// Enabling Z-buffers
	glEnable(GL_DEPTH_TEST);
	// Enabling face culling
	//glEnable(GL_CULL_FACE);

	/*********************************************************************************************/
	// Setting up Shader
	Shader* shader = new Shader(argv[0], "shader");
	Shader* shaderBezier = new Shader(argv[0], "shaderBezier");

	// Setting up Bezier
	Bezier* bezier = new Bezier();
	BSpline* bSpline = new BSpline(argv[0], 3);

	// Setting up Camera
	Camera* camera = new Camera(window_width, window_height, glm::vec3(0.0f, 0.0f, 2.0f), bezier);

		// Setting up uniform variables
		//GLint uniformLocation_modelMatrix = glGetUniformLocation(shader->ID, "modelMatrix");
		//GLint uniformLocation_viewMatrix = glGetUniformLocation(shader->ID, "viewMatrix");
		//GLint uniformLocation_projectionMatrix = glGetUniformLocation(shader->ID, "projectionMatrix");

		//glm::mat4 identityMatrix = glm::mat4(1);
		//glm::mat4 modelMatrix = identityMatrix;
		//glm::mat4 viewMatrix = identityMatrix;
		//glm::mat4 projectionMatrix = identityMatrix;

		//viewMatrix = glm::translate(viewMatrix, glm::vec3(0.0f, 0.0f, -5.0f));
		//projectionMatrix = glm::perspective(glm::radians(45.0f), static_cast<float>(window_width / window_height), 0.1f, 100.0f);

	/*********************************************************************************************/
	// Setting up Mesh
	Mesh* mesh = new Mesh(argv[0]);

	/*********************************************************************************************/
	
	// Keeping the window open
	while(!glfwWindowShouldClose(window)) {
		// Clearing the new frame with background colour
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		// Modifying matrices
		//modelMatrix = glm::rotate(modelMatrix, glm::radians(rotation), glm::vec3(0.0f, 1.0f, 0.0f));
		//viewMatrix = glm::translate(viewMatrix, glm::vec3(0.0f, 0.0f, 0.0f));
		//projectionMatrix = glm::perspective(glm::radians(45.0f), static_cast<float>(window_width / window_height), 0.1f, -1.0f);

		// Calling shader
		glUseProgram(shader->ID);
		//glUniformMatrix4fv(uniformLocation_modelMatrix, 1, GL_FALSE, &modelMatrix[0][0]);
		//glUniformMatrix4fv(uniformLocation_viewMatrix, 1, GL_FALSE, &viewMatrix[0][0]);
		//glUniformMatrix4fv(uniformLocation_projectionMatrix, 1, GL_FALSE, &projectionMatrix[0][0]);

		camera->registerInput(window);
		//bezier->loadBezier();
		//bezier->updateBuffers();
		camera->updateMatrixGeometry(45.0f, 0.1f, 300.0f, shader, shaderBezier, "cameraMatrix");
		
		bSpline->updateAnimation(shader, camera);

		// Drawing
		// Mesh
		glBindVertexArray(mesh->VAO);
			glPolygonMode(GL_FRONT_AND_BACK, GL_LINE); // GL_POINT, GL_LINE, GL_FILL
			glDrawElements(GL_TRIANGLES, mesh->getIndicesNum(), GL_UNSIGNED_INT, 0);
		glBindVertexArray(0);

		//camera->updateMatrixGeometry(45.0f, 0.1f, 100.0f, shaderBezier, "cameraMatrix");

		// Bezier
		//glUseProgram(shaderBezier->ID);
		//glBindVertexArray(bezier->VAO);
			////glDrawArrays(GL_LINE_STRIP, 0, bezier->getVerticesNum());
			//glDrawElements(GL_LINE_STRIP, bezier->getIndicesNum(), GL_UNSIGNED_INT, 0);
		//glBindVertexArray(0);

		// BSpline
		glUseProgram(shaderBezier->ID);

		camera->updateMatrixGeometry2(45.0f, 0.1f, 300.0f, shader, shaderBezier, "cameraMatrix");
		glBindVertexArray(bSpline->VAO);
		glBindBuffer(GL_ARRAY_BUFFER, bSpline->VBO);
			glDrawArrays(GL_LINE_STRIP, 0, bSpline->getIndicesNum());

		glBindBuffer(GL_ARRAY_BUFFER, bSpline->VBO_tang);
			glDrawArrays(GL_LINES, 0, bSpline->getIndicesNum() * 2);
			//glDrawElements(GL_LINE_STRIP, bSpline->getIndicesNum(), GL_UNSIGNED_INT, 0);
			//std::cout << glGetError() << std::endl << std::endl;
		glBindVertexArray(0);

		// Swapping buffers, processing window events
		glfwSwapBuffers(window);
		glfwPollEvents();

		if(glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS) {
			glfwSetWindowShouldClose(window, true);
		}
	}

	/*********************************************************************************************/
	// Deleting shader, mesh, camera
	shader->~Shader();
	mesh->~Mesh();
	bezier->~Bezier();
	camera->~Camera();

	// Terminating GLFW
	glfwTerminate();

	return EXIT_SUCCESS;
}
