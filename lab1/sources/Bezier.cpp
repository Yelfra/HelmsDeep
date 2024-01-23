#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>

#include <assimp/Importer.hpp>      
#include <assimp/scene.h>           
#include <assimp/postprocess.h> 

#include <iostream>

#include "Bezier.h"

Bezier::Bezier() {
	setupBuffers();
}
Bezier::~Bezier() {
	// Deleting VAO, VBO, EBO
	glDeleteBuffers(1, &EBO);
	glDeleteBuffers(1, &VBO);
	glDeleteVertexArrays(1, &VAO);
}

void Bezier::setupBuffers() {
	// Generating VAO, VBO, EBO
	glGenVertexArrays(1, &VAO);
	glGenBuffers(1, &VBO);
	glGenBuffers(1, &EBO);

	updateBuffers();
}

void Bezier::updateBuffers() {
	if(!updateRequired || controlPoints.size() < 2) {
		return;
	}

	// Passing data to buffers
	glBindVertexArray(VAO);
		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(float) * getVerticesNum(), &vertices[0], GL_STATIC_DRAW);
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (void*)0);

		glEnableVertexAttribArray(0);

		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
		glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int) * getIndicesNum(), (void*)(&vertex_indices[0]), GL_STATIC_DRAW);
	glBindVertexArray(0);

	updateRequired = false;
}

void Bezier::loadBezier() {
	if(!updateRequired || controlPoints.size() < 2) {
		return;
	}

	vertices.clear();
	vertex_indices.clear();

	float b = 0.0f;
	int n = controlPoints.size(); // in reality, n+1

	for(int step = 0; step < bezierSteps; step++) {
		glm::vec3 p = glm::vec3(0.0f);
		float t = step * bezierStepSize;

		for(int i = 0; i <= n; i++) {
			int index = i % n; // to loop around back to 0
			b = factorial(n) / (factorial(i) * factorial(n - i)) * glm::pow(t, i) * glm::pow(1.0f - t, n - i);
			p.x += b * controlPoints[index].x;
			p.y += b * controlPoints[index].y;
			p.z += b * controlPoints[index].z;
		}

		vertices.push_back(p.x);
		vertices.push_back(p.y);
		vertices.push_back(p.z);
		vertex_indices.push_back(step);
	}

	updateRequired = false;
}

glm::vec3 Bezier::bezierPoint() {
	if(bezierStep >= vertices.size()) {
		bezierStep = 0;
	}
	glm::vec3 p = glm::vec3(0.0f);

	p.x = vertices[bezierStep++];
	p.y = vertices[bezierStep++];
	p.z = vertices[bezierStep++];

	std::this_thread::sleep_for(std::chrono::milliseconds(10));
	return p;
}

int Bezier::factorial(int number) {
	int result = 1;
	for(int i = 1; i <= number; i++) {
		result *= i;
	}
	return result;
}

std::vector<float> Bezier::getVertices() {
	return vertices;
}
unsigned int Bezier::getVerticesNum() {
	return vertices.size();
}

std::vector<unsigned int> Bezier::getIndices() {
	return vertex_indices;
}
unsigned int Bezier::getIndicesNum() {
	return vertex_indices.size();
}

void Bezier::pushControlPoint(glm::vec3 coordinates) {
	controlPoints.push_back(coordinates);
	std::cout << "ADDED control point : " << controlPoints[controlPoints.size() - 1].x << " " << controlPoints[controlPoints.size() - 1].y << " " << controlPoints[controlPoints.size() - 1].z << std::endl;
	updateRequired = true;
}
void Bezier::popControlPoint() {
	if(controlPoints.size() != 0) {
		std::cout << "REMOVED control point : " << controlPoints[controlPoints.size() - 1].x << " " << controlPoints[controlPoints.size() - 1].y << " " << controlPoints[controlPoints.size() - 1].z << std::endl;
		controlPoints.pop_back();
		updateRequired = true;
	}
}

void Bezier::resetBezier() {
	bezierStep = 0;
}