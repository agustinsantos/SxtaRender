uniform camera {
    vec3 worldCameraPos;
};

uniform mat4 localToScreen;
uniform mat4 localToWorld;

#ifdef _VERTEX_

layout(location = 0) in vec3 POS;

void projection() {
    gl_Position = localToScreen * vec4(POS, 1.0);
}

void projection(out vec3 worldVertex) {
    gl_Position = localToScreen * vec4(POS, 1.0);
    worldVertex = (localToWorld * vec4(POS, 1.0)).xyz;
}

void projection(vec3 normal, out vec3 worldVertex, out vec3 worldNormal) {
    gl_Position = localToScreen * vec4(POS, 1.0);
    worldVertex = (localToWorld * vec4(POS, 1.0)).xyz;
    worldNormal = (localToWorld * vec4(normal, 0.0)).xyz;
}

void noprojection(out vec2 uv) {
    gl_Position =   localToScreen * vec4(POS, 1.0) ;
    uv = POS.xy * 0.5 + vec2(0.5);
}

void view_frustum(float angle_of_view,
				float aspect_ratio,
				float z_near,
				float z_far)
       {		
           gl_Position =  mat4( // vec4 are vectors stored as columns
                vec4(1.0/tan(angle_of_view * 0.5),           0.0, 0.0, 0.0),
                vec4(0.0, aspect_ratio/tan(angle_of_view * 0.5),  0.0, 0.0),
                vec4(0.0, 0.0,    (z_far+z_near)/(z_far-z_near), 1.0),
                vec4(0.0, 0.0, -2.0*z_far*z_near/(z_far-z_near), 0.0)
            ) * localToWorld * vec4(POS, 1.0);
        }
#endif

#ifdef _FRAGMENT_

vec3 viewDir(vec3 worldP) {
    return normalize(worldP - worldCameraPos);
}

#endif
