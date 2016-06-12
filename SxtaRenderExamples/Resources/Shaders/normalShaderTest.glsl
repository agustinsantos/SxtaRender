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

uniform vec3 tileSDF; // size in pixels of one tile, tileSize / grid mesh size for display, output format

uniform sampler2DArray elevationSampler; // elevation texture
uniform vec4 elevationOSL; // lower left corner of tile containing patch elevation, one over size in pixels of tiles texture, layer id

uniform vec4 deform;

#ifdef _VERTEX_

layout(location=0) in vec4 vertex;
out vec2 st;

void main() {
    st = vertex.xy;
    gl_Position = vertex;
}

#endif

#ifdef _FRAGMENT_

in vec2 st;
layout(location=0) out vec4 data;

vec3 getWorldPosition(vec2 uv, float h) {
    return vec3(1,1,1);
}

void main() {
	float noise = textureLod(elevationSampler, vec3(1,1,1), 0.0).x;
    data = vec4(elevationOSL.x,tileSDF.y,1,1);
}

#endif

