in vec2 sampleCoord;

out vec4 fragColor;

uniform sampler2D u_fontTexture;
uniform vec4 color;
uniform float u_buffer;
uniform float u_gamma;

void main()
{
    fragColor = color * smoothstep(u_buffer - u_gamma, u_buffer + u_gamma, texture(u_fontTexture, sampleCoord).r);
}