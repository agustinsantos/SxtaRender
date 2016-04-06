uniform struct {
    vec4 offset;
    vec4 camera;
    vec2 blending;
    mat4 localToScreen;
} deformation;

#ifdef _VERTEX_

layout(location=0) in vec3 vertex;
out vec4 p;

void main() {
    p = vec4(vertex.xy * deformation.offset.z + deformation.offset.xy, 0.0, 1.0);
    gl_Position = deformation.localToScreen * p;
}

#endif

#ifdef _FRAGMENT_

in vec4 p;
layout(location=0) out vec4 data;

void main() {
    data = vec4(vec3(0.2 + 0.2 * sin(0.1 * length(p.xy))), 1.0);
    data.r += mod(dot(floor(deformation.offset.xy / deformation.offset.z + 0.5), vec2(1.0)), 2.0);
}

#endif
