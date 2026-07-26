#version 330 core

layout (location = 0) in vec2 aPos;

uniform mat4 uProjection;
uniform vec2 uPosition;
uniform float uRotation;
uniform vec2 uScale;
uniform vec4 uColor;

out vec4 vColor;

void main()  {
    vec2 scaledPos = aPos * uScale;

    float c = cos(uRotation);
    float s = sin(uRotation);
    vec2 rotatedPos = vec2(
        scaledPos.x * c - scaledPos.y * s,
        scaledPos.x * s + scaledPos.y * c
    );

    vec2 finalPos = rotatedPos + uPosition;

    gl_Position = uProjection * vec4(finalPos, 0.0, 1.0);
    vColor = uColor;
}