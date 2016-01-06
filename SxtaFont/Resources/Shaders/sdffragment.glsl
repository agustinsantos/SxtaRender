uniform sampler2D u_texture;
uniform vec4 u_color;
uniform float u_buffer;
uniform float u_gamma;

in  vec2 v_tex;
in  float v_alpha;
in  float v_gamma_scale;

void main() {
    float dist = texture2D(u_texture, v_tex).r;
    float gamma = u_gamma * v_gamma_scale;
    float alpha = smoothstep(u_buffer - gamma, u_buffer + gamma, dist) * v_alpha;
    gl_FragColor = u_color * alpha;
	// gl_FragColor = vec4(u_color.rgb, alpha * u_color.a);
	// float alpha2 = smoothstep(0.45, 0.55, dist);
	// gl_FragColor = vec4(dist,dist,dist,1);
}