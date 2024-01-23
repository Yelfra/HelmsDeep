#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>

#include <assimp/Importer.hpp>      
#include <assimp/scene.h>           
#include <assimp/postprocess.h> 

#include <iostream>

#include "Mesh.h"

Mesh::Mesh(char* path) {
	this->path = path;
	boundingBox.reserve(6);

	loadMesh();
	setupBuffers();
}
Mesh::~Mesh() {
	// Deleting VAO, VBO, EBO
	glDeleteBuffers(1, &EBO);
	glDeleteBuffers(1, &VBO);
	glDeleteVertexArrays(1, &VAO);
}

void Mesh::setupBuffers() {
	// Generating VAO, VBO, EBO
	glGenVertexArrays(1, &VAO);
	glGenBuffers(1, &VBO);
	glGenBuffers(1, &EBO);

	updateBuffers();
}
void Mesh::updateBuffers() {
	// Passing data to buffers
	glBindVertexArray(VAO);
		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(float) * getVerticesNum(), &vertices[0], GL_STATIC_DRAW);
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (void*)0);

		glEnableVertexAttribArray(0);

		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
		glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int) * getIndicesNum(), (void*)(&vertex_indices[0]), GL_STATIC_DRAW);
	glBindVertexArray(0);
}

void Mesh::animateObject(Shader* shader) {


	//GLint uniformLocation_cameraMatrix = glGetUniformLocation(shader->ID, uniformLocation);

	//glUniformMatrix4fv(uniformLocation_cameraMatrix, 1, GL_FALSE, glm::value_ptr(projectionMatrix * viewMatrix));
}

void Mesh::loadMesh() {
	// Loading with Assimp
	Assimp::Importer importer;

	std::string dirPath(path, 0, path.find_last_of("\\/"));
	std::string resPath(dirPath);
	resPath.append("\\resources");
	std::string objPath(resPath);
	//objPath.append("\\glava\\glava.obj");
	objPath.append("\\f16.obj");

	const aiScene* scene = importer.ReadFile(objPath.c_str(),
											 aiProcess_CalcTangentSpace |
											 aiProcess_Triangulate |
											 aiProcess_JoinIdenticalVertices |
											 aiProcess_SortByPType);
	if(!scene) {
		std::cerr << importer.GetErrorString();
		return;
	}

	// Processing loaded meshes
	if(scene->HasMeshes()) {
		aiMesh* mesh = scene->mMeshes[0];

		std::cout << "Loading polygon mesh..." << std::endl;

		float x, y, z;
		float x_min = std::numeric_limits<float>::max(), x_max = std::numeric_limits<float>::min();
		float y_min = std::numeric_limits<float>::max(), y_max = std::numeric_limits<float>::min();
		float z_min = std::numeric_limits<float>::max(), z_max = std::numeric_limits<float>::min();

		for(int i = 0; i < mesh->mNumVertices; i++) {
			x = mesh->mVertices[i].x;
			y = mesh->mVertices[i].y;
			z = mesh->mVertices[i].z;

			if(x < x_min) x_min = x; if(x > x_max) x_max = x;
			if(y < y_min) y_min = y; if(y > y_max) y_max = y;
			if(z < z_min) z_min = z; if(z > z_max) z_max = z;
		}
		boundingBox.push_back(x_min);	boundingBox.push_back(x_max);
		boundingBox.push_back(y_min);	boundingBox.push_back(y_max);
		boundingBox.push_back(z_min);	boundingBox.push_back(z_max);

		float x_average = (x_min + x_max) / 2;
		float y_average = (y_min + y_max) / 2;
		float z_average = (z_min + z_max) / 2;

		float M = std::max({ x_max - x_min, y_max - y_min, z_max - z_min });

		for(int i = 0; i < mesh->mNumVertices; i++) {
			x = (mesh->mVertices[i].x - x_average) * 2 / M;
			y = (mesh->mVertices[i].y - y_average) * 2 / M;
			z = (mesh->mVertices[i].z - z_average) * 2 / M;
			
			vertices.push_back(x);
			vertices.push_back(y);
			vertices.push_back(z);
		}

		for(int i = 0; i < mesh->mNumFaces; i++) {
			for(int j = 0; j < mesh->mFaces[i].mNumIndices; j++) {
				vertex_indices.push_back(mesh->mFaces[i].mIndices[j]);
			}
		}

		std::cout << "Mesh successfully loaded." << std::endl;
	}
}

std::vector<float> Mesh::getBoundingBox() {
	return boundingBox;
}

std::vector<float> Mesh::getVertices() {
	return vertices;
}
unsigned int Mesh::getVerticesNum() {
	return vertices.size();
}

std::vector<unsigned int> Mesh::getIndices() {
	return vertex_indices;
}
unsigned int Mesh::getIndicesNum() {
	return vertex_indices.size();
}