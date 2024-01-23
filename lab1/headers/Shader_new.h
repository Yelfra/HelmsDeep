#pragma once

#include <iostream>
#include <string>
#include <fstream>
#include <sstream>

#include <glad/glad.h>

class Shader {
private:
	std::string vertexShader_path;
	//std::string geometryShader_path;
	std::string fragmentShader_path;

	std::string vertexShader_source;
	//std::string geometryShader_source;
	std::string fragmentShader_source;

	//void readShaderFiles(const char* vertexShader_path, const char* geometryShader_path, const char* fragmentShader_path);
	void readShaderFiles(const char* vertexShader_path, const char* fragmentShader_path);
	void loadShaderPath(char* dirPath, char* shaderName);
public:
	Shader(char* dirPath, char* shaderName);
	~Shader();

	GLuint ID;

	void setUniform(const std::string& name, bool value) const;
	void setUniform(const std::string& name, int value) const;
	void setUniform(const std::string& name, float value) const;
};

