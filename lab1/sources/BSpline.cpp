#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>

#include <assimp/Importer.hpp>      
#include <assimp/scene.h>           
#include <assimp/postprocess.h>
#include <glm/gtc/matrix_transform.hpp>

#include <iostream>
#include <fstream>

#include "BSpline.h"

BSpline::BSpline(char* path_char, int degree) {
	std::string path = path_char;
	std::string dirPath(path, 0, path.find_last_of("\\/"));
	std::string resPath(dirPath);
	resPath.append("\\resources");
	std::string bSplinePath(resPath);
	bSplinePath.append("\\bspline.txt");

	std::ifstream bSplineFile(bSplinePath);

	if(!bSplineFile) {
		std::cerr << "Failed to open bspline.txt" << std::endl;
		return;
	}

	std::string line;
	while(std::getline(bSplineFile, line)) {
		if(line.size() > 2 && line[0] == 'v' && line[1] == ' ') {
			std::istringstream iss(line);
			char v;
			float x, y, z;
			if(iss >> v >> x >> y >> z) {
				controlPoints.push_back(glm::vec3(x, y, z));
			} else {
				std::cerr << "Failed to parse a line: " << line << std::endl;
			}
		}
	}

	bSplineFile.close();

	loadBSpline();

	setupBuffers();
}
BSpline::~BSpline() {
	// Deleting VAO, VBO, EBO
	glDeleteBuffers(1, &EBO);
	glDeleteBuffers(1, &VBO);
	glDeleteVertexArrays(1, &VAO);
}

void BSpline::loadBSpline() {
	if(controlPoints.size() < 2) {
		return;
	}

	int n = controlPoints.size();

	for(int i = 0; i < n - bSplineDeg; i++) {
		glm::mat4x3 controlPoint_matrix = glm::mat4x3(
			controlPoints[i],
			controlPoints[i + 1],
			controlPoints[i + 2],
			controlPoints[i + 3]);
		glm::mat4x3 spline = controlPoint_matrix * B_matrix;

		/*for(int i = 0; i < 4; i++) {
			for(int j = 0; j < 3; j++) {
				std::cout << spline[i][j] << " ";
			}
			std::cout << std::endl;
		}
		std::cout << std::endl;*/

		for(int step = 0; step < bSplineSteps; step++) {
			float t = step * bSplineStepSize;
			glm::vec4 t_vector = glm::vec4(glm::pow(t, 3), glm::pow(t, 2), t, 1);
			glm::vec4 t_vector_der = glm::vec4(3.0f * glm::pow(t, 2), 2.0f * t, 1.0f, 0);


			glm::vec3 point_t = B_factor * spline * t_vector;

			//std::cout << point_t.x << " " << point_t.y << " " << point_t.z << std::endl;

			vertices.push_back(point_t.x);
			vertices.push_back(point_t.y);
			vertices.push_back(point_t.z);

			//glm::vec3 tangent = glm::normalize(tangent_factor * spline * t_vector_der);
			glm::vec3 tangent = tangent_factor * spline * t_vector_der;
			tangent_vertices.push_back(point_t.x);
			tangent_vertices.push_back(point_t.y);
			tangent_vertices.push_back(point_t.z);

			tangent_vertices.push_back(tangent.x);
			tangent_vertices.push_back(tangent.y);
			tangent_vertices.push_back(tangent.z);

			//std::cout << "POINT: " << point_t.x << " " << point_t.y << " " << point_t.z << std::endl;
			//std::cout << "TANG: " << tangent.x << " " << tangent.y << " " << tangent.z << std::endl << std::endl;


			vertex_indices.push_back(vertex_indices.size());
		}
	}
}

void BSpline::setupBuffers() {
	// Generating VAO, VBO, EBO
	glGenVertexArrays(1, &VAO);
	glGenBuffers(1, &VBO);
	glGenBuffers(1, &VBO_tang);
	glGenBuffers(1, &EBO);

	// Passing data to buffers
	glBindVertexArray(VAO);
	glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(float) * getVerticesNum(), &vertices[0], GL_STATIC_DRAW);
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (void*)0);
	
	glBindBuffer(GL_ARRAY_BUFFER, VBO_tang);
		glBufferData(GL_ARRAY_BUFFER, sizeof(float) * getVerticesNum() * 2, &tangent_vertices[0], GL_STATIC_DRAW);
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (void*)0);

	glEnableVertexAttribArray(0);

	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int) * getIndicesNum(), (void*)(&vertex_indices[0]), GL_STATIC_DRAW);
	glBindVertexArray(0);
}

//PROBLEMS:

//avion se ne rotira - problem s implementacijom kutova/tangente

void BSpline::updateAnimation(Shader* shader, Camera* camera) {
	if(current_index + 3 >= vertices.size()) {
		current_index = 0;
		resetCamera = true;
		//vertex_index = 0;
		//time_index = 0;
		//return;
	}

	glm::mat4x3 controlPoint_matrix = glm::mat4x3(
		controlPoints[spline_vertex_index],
		controlPoints[spline_vertex_index + 1],
		controlPoints[spline_vertex_index + 2],
		controlPoints[spline_vertex_index + 3]);
	glm::vec4 t_vector_der = glm::vec4(3.0f * glm::pow(spline_time, 2), 2.0f * spline_time, 1.0f, 0);

	// Current point
	glm::vec3 current_point = glm::vec3(vertices[current_index], vertices[current_index + 1], vertices[current_index + 2]);
	current_index += 3;

	spline_time += bSplineStepSize;
	if(spline_time >= bSplineSteps && spline_vertex_index < controlPoints.size() - bSplineDeg) {
		spline_vertex_index++;
		spline_time = 0;
	}

	// Tangent
	glm::mat4x3 spline_tangent = controlPoint_matrix * B_matrix;
	glm::vec3 tangent = glm::normalize(tangent_factor * spline_tangent * t_vector_der);


	// Rotate
	glm::vec3 z_os = glm::vec3(0, 0, 1.0f);
	glm::vec3 rotationAxis = glm::normalize(glm::cross(tangent, z_os));

	float cosAngle = glm::dot(tangent, z_os);
	//std::cout << "cosAngle: " << cosAngle << std::endl;

	float angle = acos(cosAngle);

	//std::cout << "angle: " << angle << std::endl;

	glm::mat4x4 rotationMatrix = glm::rotate(glm::mat4x4(1.0f), angle, rotationAxis);

	// Translate
	//glm::mat4x4 animationMatrix = glm::translate(glm::mat4x4(1.0f), current_point); // tocka dolje (ne desno)
	glm::mat4x4 animationMatrix = glm::translate(glm::mat4x4(1.0f), current_point) * rotationMatrix; // tocka dolje (ne desno)
	//animationMatrix = rotationMatrix * animationMatrix;

	// Previous point
	previous_point = current_point;

	/*for(int i = 0; i < 4; i++) {
		for(int j = 0; j < 4; j++) {
			std::cout << rotationMatrix[i][j] << " ";
		}
		std::cout << std::endl;
	}
	std::cout << std::endl;*/

	GLint animationMatrixLocation = glGetUniformLocation(shader->ID, "animationMatrix");
	glUniformMatrix4fv(animationMatrixLocation, 1, GL_FALSE, glm::value_ptr(animationMatrix));

	if(resetCamera) {
		//camera->orientation = current_point;
		resetCamera = false;
	}

	std::this_thread::sleep_for(std::chrono::milliseconds(1));
}
/*
glm::vec3 BSpline::bSplinePoint() {
	if(bSplineStep >= vertices.size()) {
		bSplineStep = 0;
	}
	glm::vec3 p = glm::vec3(0.0f);

	p.x = vertices[bSplineStep++];
	p.y = vertices[bSplineStep++];
	p.z = vertices[bSplineStep++];

	std::this_thread::sleep_for(std::chrono::milliseconds(10));
	return p;
}
*/
int BSpline::factorial(int number) {
	int result = 1;
	for(int i = 1; i <= number; i++) {
		result *= i;
	}
	return result;
}

std::vector<float> BSpline::getVertices() {
	return vertices;
}
unsigned int BSpline::getVerticesNum() {
	return vertices.size();
}
std::vector<float> BSpline::getTangVertices() {
	return tangent_vertices;
}
unsigned int BSpline::getTangVerticesNum() {
	return tangent_vertices.size();
}

std::vector<unsigned int> BSpline::getIndices() {
	return vertex_indices;
}
unsigned int BSpline::getIndicesNum() {
	return vertex_indices.size();
}

void BSpline::pushControlPoint(glm::vec3 coordinates) {
	controlPoints.push_back(coordinates);
	std::cout << "ADDED control point : " << controlPoints[controlPoints.size() - 1].x << " " << controlPoints[controlPoints.size() - 1].y << " " << controlPoints[controlPoints.size() - 1].z << std::endl;
	//updateRequired = true;
}
void BSpline::popControlPoint() {
	if(controlPoints.size() != 0) {
		std::cout << "REMOVED control point : " << controlPoints[controlPoints.size() - 1].x << " " << controlPoints[controlPoints.size() - 1].y << " " << controlPoints[controlPoints.size() - 1].z << std::endl;
		controlPoints.pop_back();
		//updateRequired = true;
	}
}