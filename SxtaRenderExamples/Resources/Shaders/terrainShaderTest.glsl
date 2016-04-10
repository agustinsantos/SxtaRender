uniform struct {
    vec4 offset;
    vec4 camera;
    vec2 blending;
    mat4 localToScreen;
} deformation;

#ifdef _VERTEX_

layout(location=0) in vec4 vertex;

out vec2 uv;
void main() {
    gl_Position = vec4(vertex.x - 1.0, vertex.y, vertex.z, vertex.w);
    uv = vertex.xy;
}

#endif

#ifdef _FRAGMENT_
uniform sampler2D sampler;
in vec2 uv;
layout(location=0) out vec4 color;

void main() {
	color = texture(sampler, uv);
}

#endif
