#pragma once

#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>

#include <assimp/Importer.hpp>      
#include <assimp/scene.h>           
#include <assimp/postprocess.h> 

#include <iostream>
#include <vector>

#include "Shader_new.h"

class Mesh {
private:
	std::string path;


	std::vector<float> vertices; // convert to glm::vec3 if possible
	std::vector<unsigned int> vertex_indices;

	// Bounding Box stores {x_min, x_max, y_min, y_max, z_min, z_max}
	std::vector<float> boundingBox;

public:
	Mesh(char* path);
	~Mesh();

	GLuint VAO;
	GLuint VBO;
	GLuint EBO;

	void setupBuffers();
	void updateBuffers();
	void animateObject(Shader* shader);

	void loadMesh();

	std::vector<float> getBoundingBox();
	std::vector<float> getVertices();
	unsigned int getVerticesNum();
	std::vector<unsigned int> getIndices();
	unsigned int getIndicesNum();
};

