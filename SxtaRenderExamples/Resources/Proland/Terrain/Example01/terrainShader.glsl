/*
 * Author: Agustin Santos, based on examples written by Eric Bruneton, Antoine Begault, Guillaume Piolat.
 */

struct samplerTile {
    sampler2DArray tilePool;    // tile cache
    vec3 tileCoords;    // coords of currently selected tile in tile cache (u,v,layer; u,v in [0,1]^2)
    vec3 tileSize;    // size of currently selected tile in tile cache (du,dv,d; du,dv in [0,1]^2, d in pixels)
};

// returns content of currently selected tile, at uv coordinates (in [0,1]^2; relatively to this tile)
vec4 textureTile(samplerTile tex, vec2 uv) {
    vec3 uvl = tex.tileCoords + vec3(uv * tex.tileSize.xy, 0.0);
    return texture(tex.tilePool, uvl);
}


uniform struct {
    vec4 offset;
    vec4 camera;
    vec2 blending;
    mat4 localToScreen;
} deformation;

uniform samplerTile elevationSampler;

#ifdef _VERTEX_

	layout(location=0) in vec3 vertex;
	out vec2 uv;
	void main() {
		gl_Position = deformation.localToScreen * vec4(vertex.xy * deformation.offset.z + deformation.offset.xy, 1.0, 1.0);
		uv = vertex.xy;
	}

#endif

#ifdef _FRAGMENT_

	in vec2 uv;
	layout(location=0) out vec4 color;
	void main() {
		//vec2 p =  uv * deformation.offset.z + deformation.offset.xy;
		float h = textureTile(elevationSampler, uv).x;
		if (h < 0.1) {
			color = vec4(0.0, 0.0, 0.5, 1.0);
		} else {
			color = vec4(0.0, h, 0.0, 1.0);
		}
		//color = textureTile(elevationSampler, uv);
		color = normalize(color);// * 0.6 + 0.4* vec4(vec3(0.2 + 0.2 * sin(0.1 * length(p))), 1.0);
		color.r += 0.4*mod(dot(floor(deformation.offset.xy / deformation.offset.z + 0.5), vec2(1.0)), 2.0);
	}

#endif