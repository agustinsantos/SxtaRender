/*
 * Proland: a procedural landscape rendering library.
 * Copyright (c) 2008-2011 INRIA
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

/*
 * Proland is distributed under a dual-license scheme.
 * You can obtain a specific license from Inria: proland-licensing@inria.fr.
 */

/*
 * Authors: Eric Bruneton, Antoine Begault, Guillaume Piolat.
 */

uniform struct {
    vec4 offset;
    vec4 camera;
    vec2 blending;
    mat4 localToScreen;
    float radius;
    mat3 localToWorld;
	mat4 screenQuadCorners;
    mat4 screenQuadVerticals;
	vec4 screenQuadCornerNorms;
 } deformation;

#ifdef _VERTEX_

layout(location=0) in vec3 vertex;
out vec3 p;

void main() {
    float R = deformation.radius;
	mat4 C = deformation.screenQuadCorners;
    mat4 N = deformation.screenQuadVerticals;
	vec4 L = deformation.screenQuadCornerNorms;
	vec3 P = vec3(vertex.xy * deformation.offset.z + deformation.offset.xy, R);
	
    vec4 uvUV = vec4(vertex.xy, vec2(1.0) - vertex.xy);
    vec4 alpha = uvUV.zxzx * uvUV.wwyy;
    vec4 alphaPrime = alpha * L / dot(alpha, L);

	float k = min(length(P) / dot(alpha, L) * 1.0000003, 1.0);
    float hPrime = (R * (1.0 - k)) / k; // REDONDEA CADA TILE

    gl_Position = (C + hPrime * N) * alphaPrime;
    p = deformation.radius * normalize(deformation.localToWorld * P);
}

#endif

#ifdef _FRAGMENT_

in vec3 p;
layout(location=0) out vec4 data;

void main() {
    data = vec4(vec3(0.2 + 0.2 * sin(0.00001 * length(p.xy))), 1.0);
    data.r += mod(dot(floor(deformation.offset.xy / deformation.offset.z + 0.5), vec2(1.0)), 2.0);
}

#endif