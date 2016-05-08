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

/**
 * Precomputed Atmospheric Scattering
 * Copyright (c) 2008 INRIA
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. Neither the name of the copyright holders nor the names of its
 *    contributors may be used to endorse or promote products derived from
 *    this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */

/**
 * Author: Eric Bruneton
 * Ported to Sxta by: Agustin Santos
*/
namespace Sxta.Proland.Atmo
{
    public partial class PreprocessAtmo
    {
        private string inscatterNShader = @"
// computes higher order scattering (line 9 in algorithm 4.1)

uniform float r; 
uniform vec4 dhdH; 
uniform int layer; 
 
uniform sampler3D deltaJSampler; 
 
#ifdef _VERTEX_ 
 
layout(location=0) in vec4 vertex; 
 
void main() { 
    gl_Position = vertex; 
} 
 
#endif 
 
#ifdef _GEOMETRY_ 
#extension GL_EXT_geometry_shader4 : enable 
 
layout(triangles) in; 
layout(triangle_strip,max_vertices=3) out; 
 
void main() { 
    gl_Position = gl_PositionIn[0]; 
    gl_Layer = layer; 
    EmitVertex(); 
    gl_Position = gl_PositionIn[1]; 
    gl_Layer = layer; 
    EmitVertex(); 
    gl_Position = gl_PositionIn[2]; 
    gl_Layer = layer; 
    EmitVertex(); 
    EndPrimitive(); 
} 
 
#endif 
 
#ifdef _FRAGMENT_ 
 
layout(location=0) out vec4 data; 
 
vec3 integrand(float r, float mu, float muS, float nu, float t) { 
    float ri = sqrt(r * r + t * t + 2.0 * r * mu * t); 
    float mui = (r * mu + t) / ri; 
    float muSi = (nu * t + muS * r) / ri; 
    return texture4D(deltaJSampler, ri, mui, muSi, nu).rgb * transmittance(r, mu, t); 
} 
 
vec3 inscatter(float r, float mu, float muS, float nu) { 
    vec3 raymie = vec3(0.0); 
    float dx = limit(r, mu) / float(INSCATTER_INTEGRAL_SAMPLES); 
    float xi = 0.0; 
    vec3 raymiei = integrand(r, mu, muS, nu, 0.0); 
    for (int i = 1; i <= INSCATTER_INTEGRAL_SAMPLES; ++i) { 
        float xj = float(i) * dx; 
        vec3 raymiej = integrand(r, mu, muS, nu, xj); 
        raymie += (raymiei + raymiej) / 2.0 * dx; 
        xi = xj; 
        raymiei = raymiej; 
    } 
    return raymie; 
} 
 
void main() { 
    float mu, muS, nu; 
    getMuMuSNu(r, dhdH, mu, muS, nu); 
    data.rgb = inscatter(r, mu, muS, nu); 
} 
 
#endif
 ";
    }
}