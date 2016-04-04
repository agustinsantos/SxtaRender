uniform vec4 offset;

#ifdef _VERTEX_
    vec4 camera;
    vec2 blending;
    uniform mat4 uMVMatrix;
    uniform mat4 uPMatrix;


layout(location=0) in vec3 vertex;
layout(location = 1) in vec3 aColor;
out vec4 p;

void main() {
    p = vec4(vertex.xy * offset.z + offset.xy, vertex.z, 1.0);
    gl_Position = uPMatrix * uMVMatrix * p;
}

#endif

#ifdef _FRAGMENT_

in vec4 p;
layout(location=0) out vec4 data;

void main() {
    data = vec4(vec3(0.2 + 0.3 * sin(10.1 * length(p.xy))), 1.0);
    data.r += mod(dot(floor(offset.xy / offset.z + 0.5), vec2(1.0)), 4);
}

#endif
