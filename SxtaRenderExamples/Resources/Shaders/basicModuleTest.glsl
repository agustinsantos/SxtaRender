#ifdef _VERTEX_
layout(location=0) in vec4 vertex;
out vec2 uv;
void main() {
    gl_Position = vertex;
    uv = vertex.xy * 1.0;
}
#endif
#ifdef _FRAGMENT_
uniform sampler2D sampler;
in vec2 uv;
layout(location=0) out vec4 color;
void main() {
    color = vec4(1.0,1.0,1.0,1.0);
}
#endif
