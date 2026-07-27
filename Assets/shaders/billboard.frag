#version 330 core
out vec4 FragColor;

in vec2 TexCoord;

uniform sampler2D billboardTex;

void main() {
    vec4 texColor = texture(billboardTex, TexCoord);
    
    if (texColor.a < 0.01) {
        discard;
    }
    
    FragColor = texColor; 
}
