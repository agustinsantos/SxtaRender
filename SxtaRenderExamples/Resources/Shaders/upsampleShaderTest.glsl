
const float BORDER = 2.0; // tile border size

uniform vec4 tileWSDF; // size in pixels of one tile (including borders), size in meters of a pixel of the elevation texture, (tileWidth - 2*BORDER) / grid mesh size for display, 1 to flip quad diagonals during rendering with a geometry shader (to avoid geometry aliasing), 0 otherwise

uniform sampler2DArray coarseLevelSampler; // coarse level texture
uniform vec4 coarseLevelOSL; // lower left corner of patch to upsample, one over size in pixels of coarse level texture, layer id

uniform sampler2D residualSampler; // residual texture
uniform vec4 residualOSH; // lower left corner of residual patch to use, scaling factor of residual texture coordinates, scaling factor of residual texture values

uniform sampler2DArray noiseSampler; // noise texture
uniform vec4 noiseUVLH; // noise texture rotation, noise texture layer, scaling factor of noise texture values

uniform mat4 deformation;

#ifdef _VERTEX_

layout(location=0) in vec4 vertex;
out vec2 st;

void main() {
    st = (vertex.xy * 0.5 + 0.5) * tileWSDF.x;
    gl_Position = vertex;
}

#endif

#ifdef _FRAGMENT_

in vec2 st;
layout(location=0) out vec4 data;

void main() {
	float noise = textureLod(noiseSampler, vec3(1,1,1), 0.0).x;
	float noise2 = textureLod(coarseLevelSampler, vec3(1,1,1), 0.0).x;
	vec2 st2 = residualOSH.xy * tileWSDF.xy * noiseUVLH.xy * coarseLevelOSL.xy;
	float zf = textureLod(residualSampler, vec2(1,1), 0.0).x;
    data = vec4(st2.x, st2.y,1,1);

}

#endif