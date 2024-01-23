#include "Shader.h"

Shader::Shader(char* dirPath, char* shaderName) {
	// Loading path to shaders <shaderName>.vert and <shaderName>.frag
	loadShaderPath(dirPath, shaderName);

	// Reading shader files
	const char* vertexShader_PATH = vertexShader_path.c_str();
	//const char* geometryShader_PATH = geometryShader_path.c_str();
	const char* fragmentShader_PATH = fragmentShader_path.c_str();
	//readShaderFiles(vertexShader_PATH, geometryShader_PATH, fragmentShader_PATH);
	readShaderFiles(vertexShader_PATH, fragmentShader_PATH);

	// Initializing source constants
	const char* vertexShader_SOURCE = vertexShader_source.c_str();
	//const char* geometryShader_SOURCE = geometryShader_source.c_str();
	const char* fragmentShader_SOURCE = fragmentShader_source.c_str();

	// Compiling vertex shader
	GLuint vertexShader = glCreateShader(GL_VERTEX_SHADER);
	glShaderSource(vertexShader, 1, &vertexShader_SOURCE, nullptr);
	glCompileShader(vertexShader);

	// Compiling geometry shader
	//GLuint geometryShader = glCreateShader(GL_GEOMETRY_SHADER);
	//glShaderSource(geometryShader, 1, &geometryShader_SOURCE, nullptr);
	//glCompileShader(geometryShader);

	// Compiling fragment shader
	GLuint fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
	glShaderSource(fragmentShader, 1, &fragmentShader_SOURCE, nullptr);
	glCompileShader(fragmentShader);

	// Creating shader program and attaching shaders
	ID = glCreateProgram();

	glAttachShader(ID, vertexShader);
	//glAttachShader(ID, geometryShader);
	glAttachShader(ID, fragmentShader);

	glLinkProgram(ID);

	// Deleting no longer necessary shaders (already linked to shaderProgram)
	glDeleteShader(vertexShader);
	//glDeleteShader(geometryShader);
	glDeleteShader(fragmentShader);
}

void Shader::loadShaderPath(char* dirPath, char* shaderName) {
	// Setting up path strings
	std::string shaderPath(dirPath);

	vertexShader_path.append(dirPath, shaderPath.find_last_of("\\/") + 1);
	//geometryShader_path.append(dirPath, shaderPath.find_last_of("\\/") + 1);
	fragmentShader_path.append(dirPath, shaderPath.find_last_of("\\/") + 1);

	// Locating shader directory
	if(fragmentShader_path[fragmentShader_path.size() - 1] == '/') {
		vertexShader_path.append("shaders/");
		//geometryShader_path.append("shaders/");
		fragmentShader_path.append("shaders/");
	} else if(fragmentShader_path[fragmentShader_path.size() - 1] == '\\') {
		vertexShader_path.append("shaders\\");
		//geometryShader_path.append("shaders\\");
		fragmentShader_path.append("shaders\\");
	} else {
		std::cerr << "Unknown shader position format, shaders directory not found.";
		exit(1);
	}

	// Appending shader name and extension to path strings
	vertexShader_path.append(shaderName);
	vertexShader_path.append(".vert");

	//geometryShader_path.append(shaderName);
	//geometryShader_path.append(".geom");

	fragmentShader_path.append(shaderName);
	fragmentShader_path.append(".frag");
}

//void Shader::readShaderFiles(const char* vertexShader_path, const char* geometryShader_path, const char* fragmentShader_path) {
void Shader::readShaderFiles(const char* vertexShader_path, const char* fragmentShader_path) {
	// Declaring file streams
	std::ifstream vertexShader_file, geometryShader_file, fragmentShader_file;

	// Ensuring ifstream objects can throw exceptions
	vertexShader_file.exceptions(std::ifstream::failbit | std::ifstream::badbit);
	//geometryShader_file.exceptions(std::ifstream::failbit | std::ifstream::badbit);
	fragmentShader_file.exceptions(std::ifstream::failbit | std::ifstream::badbit);
	try {
		// Opening files
		vertexShader_file.open(vertexShader_path);
		//geometryShader_file.open(geometryShader_path);
		fragmentShader_file.open(fragmentShader_path);
		//std::stringstream vertexShader_stream, geometryShader_stream, fragmentShader_stream;
		std::stringstream vertexShader_stream, fragmentShader_stream;

		// Reading file's buffer contents into streams
		vertexShader_stream << vertexShader_file.rdbuf();
		//geometryShader_stream << geometryShader_file.rdbuf();
		fragmentShader_stream << fragmentShader_file.rdbuf();

		// Closing file handlers
		vertexShader_file.close();
		//geometryShader_file.close();
		fragmentShader_file.close();

		// Converting streams into strings
		vertexShader_source = vertexShader_stream.str();
		//geometryShader_source = geometryShader_stream.str();
		fragmentShader_source = fragmentShader_stream.str();
	} catch(std::ifstream::failure e) {
		std::cout << stderr << "ERROR::SHADER::FILE_NOT_SUCCESFULLY_READ" << std::endl;
	}
}


Shader::~Shader() {
	glDeleteProgram(ID);
}

void Shader::setUniform(const std::string& name, bool value) const {
	glUniform1i(glGetUniformLocation(ID, name.c_str()), (int)value);
}
void Shader::setUniform(const std::string& name, int value) const {
	glUniform1i(glGetUniformLocation(ID, name.c_str()), value);
}
void Shader::setUniform(const std::string& name, float value) const {
	glUniform1f(glGetUniformLocation(ID, name.c_str()), value);
}
