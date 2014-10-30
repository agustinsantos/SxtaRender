#ifdef _VERTEX_
layout(location=0) in vec4 vertex;
out vec2 uv;
void main() {
    gl_Position = vertex;
    uv = vertex.xy * 0.5 + vec2(0.5);
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
