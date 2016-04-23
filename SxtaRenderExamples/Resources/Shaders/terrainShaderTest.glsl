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
    vec4 p = vec4(vertex.xy * deformation.offset.z + deformation.offset.xy, -10.0, 1.0);
    gl_Position = deformation.localToScreen * p;
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
