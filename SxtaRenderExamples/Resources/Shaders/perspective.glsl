#ifdef _VERTEX_
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 uv;

out vec3 wpos;
out vec3 wnormal;
out vec2 fuv;

void projection(vec3 normal, out vec3 worldPos, out vec3 worldNormal);
void view_frustum(
            float angle_of_view,
            float aspect_ratio,
            float z_near,
            float z_far
        );


void main() {
    projection(normal, wpos, wnormal);
	//view_frustum(radians(60.0), 4.0/4.0, 0.1, 500.0);
    fuv = uv;
}
#endif

#ifdef _FRAGMENT_
uniform sampler2D sampler;
in vec2 fuv;
layout(location=0) out vec4 color;
void main() {
    color = texture(sampler, fuv);
}
#endif
