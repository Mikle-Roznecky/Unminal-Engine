#version 330 core
out vec4 FragColor;
in vec3 FragPos;
in vec3 Normal;
uniform vec3 objectColor;
uniform vec3 viewPos;
void main() {
    vec3 ambient = vec3(0.3) * objectColor;
    vec3 lightDir = normalize(vec3(0.5, 1.0, 0.5) - FragPos);
    vec3 norm = normalize(Normal);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * objectColor;
    FragColor = vec4(ambient + diffuse, 1.0);
}