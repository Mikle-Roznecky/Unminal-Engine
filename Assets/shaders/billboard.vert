#version 330 core
layout (location = 0) in vec3 aPos;

uniform mat4 view;
uniform mat4 projection;
uniform vec3 billboardPos;
uniform vec2 scale;

void main() {
    vec3 cameraRight = vec3(view[0][0], view[1][0], view[2][0]);
    vec3 cameraUp    = vec3(view[0][1], view[1][1], view[2][1]);

    vec3 worldPos = billboardPos + (aPos.x * scale.x * cameraRight) + (aPos.y * scale.y * cameraUp);
    gl_Position = projection * view * vec4(worldPos, 1.0);
}