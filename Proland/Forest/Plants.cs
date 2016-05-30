/*
 * Proland: a procedural landscape rendering library.
 * Website : http://proland.inrialpes.fr/
 * Copyright (c) 2008-2015 INRIA - LJK (CNRS - Grenoble University)
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 * 
 * 2. Redistributions in binary form must reproduce the above copyright notice, 
 * this list of conditions and the following disclaimer in the documentation 
 * and/or other materials provided with the distribution.
 * 
 * 3. Neither the name of the copyright holder nor the names of its contributors 
 * may be used to endorse or promote products derived from this software without 
 * specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE 
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 */
/*
 * Proland is distributed under the Berkeley Software Distribution 3 Licence. 
 * For any assistance, feedback and enquiries about training programs, you can check out the 
 * contact page on our website : 
 * http://proland.inrialpes.fr/
 */
/*
* Main authors: Eric Bruneton, Antoine Begault, Guillaume Piolat.
 * Modified and ported to C# and Sxta Engine by Agustin Santos and Daniel Olmedo 2015-2016
*/

using Sxta.Core;
using Sxta.Render;
using Sxta.Render.Resources;
using System.Collections.Generic;

namespace Sxta.Proland.Forest
{
    /// <summary>
    /// Provides classes to render many 3D models on a %terrain with OpenGL instancing.
    /// </summary>
    public class Plants : ISwappable<Plants>
    {

        /**
         * The GLSL Shader that is able to determine if a seed is valid (see DrawPlantsTask). If
         * it is, it will determine its properties, such as the size of the plant, its type,
         * color etc... Eliminated seeds will contain (0, 0, 0, 0) values.
         */
        public Program selectProg;

        /**
         * The GLSL Shader used to render the plants shadows, based on the selected seeds.
         */
        public Program shadowProg;

        /**
         * The GLSL Shader used to render the plants, based on the selected seeds.
         */
        public Program renderProg;

        /**
         * Creates a new Plants.
         *
         * @param minLevel the first level to display the models from this Plants.
         * @param maxLevel the last level to display the models from this Plants.
         * @param maxDistance the furthest distance at which to display the models.
         * @param lodDistance the distance between each lod.
         */
        public Plants(int minLevel, int maxLevel, int minDensity, int maxDensity, int tileCacheSize, float maxDistance)
        {
            init(minLevel, maxLevel, minDensity, maxDensity, tileCacheSize, maxDistance );
        }


        /**
         * Deletes this Plants.
         */
        //public ~Plants();

        /**
         * TODO.
         */
        public int getMinLevel()
        {
            return minLevel;
        }

        /**
         * TODO.
         */
        public int getMaxLevel()
        {
            return maxLevel;
        }

        /**
         * TODO.
         */
        public int getMinDensity()
        {
            return minDensity;
        }

        /**
         * TODO.
         */
        public int getMaxDensity()
        {
            return maxDensity;
        }

        /**
         * TODO.
         */
        public int getTileCacheSize()
        {
            return tileCacheSize;
        }

        /**
         * TODO.
         */
        public float getMaxDistance()
        {
            return maxDistance;
        }

        /**
         * TODO.
         */
        public int getPatternCount()
        {
            return patterns.Count;
        }

        /**
         * TODO.
         */
        public float getPoissonRadius()
        {
            return (float)(1.0 / System.Math.Sqrt(0.5 * (minDensity + maxDensity) * System.Math.PI / POISSON_COVERAGE));
        }

        /**
         * Returns the i'th pattern.
         */
        public  MeshBuffers  getPattern(int index)
        {
            return patterns[index];
        }

        /**
         * TODO.
         */
        public void addPattern(MeshBuffers pattern)
        {
            patterns.Add(pattern);
        }

        /**
         * TODO.
         */
        public void setMaxDistance(float maxDistance)
        {
            this.maxDistance = maxDistance;
            if (renderProg.getUniform1f("maxTreeDistance") != null)
            {
                renderProg.getUniform1f("maxTreeDistance").set(maxDistance);
            }
        }


        /**
         * Creates a new Plants.
         */
        protected Plants() { }

        /**
         * Initializes a Plants fields.
         *
         * See #Plants.
         */
        protected void init(int minLevel, int maxLevel, int minDensity, int maxDensity, int tileCacheSize, float maxDistance)
        {
            this.minLevel = minLevel;
            this.maxLevel = maxLevel;
            this.minDensity = minDensity;
            this.maxDensity = maxDensity;
            this.tileCacheSize = tileCacheSize;
            this.maxDistance = maxDistance;
        }


        public void swap(Plants p)
        {
            Std.Swap(ref selectProg, ref p.selectProg);
            Std.Swap(ref renderProg, ref p.renderProg);
            Std.Swap(ref minLevel, ref p.minLevel);
            Std.Swap(ref maxLevel, ref p.maxLevel);
            Std.Swap(ref maxDensity, ref p.maxDensity);
            Std.Swap(ref tileCacheSize, ref p.tileCacheSize);
            Std.Swap(ref maxDistance, ref p.maxDistance);
            Std.Swap(ref patterns, ref p.patterns);
        }


        private int minLevel;

        private int maxLevel;

        private int minDensity;

        private int maxDensity;

        private int tileCacheSize;

        private float maxDistance;

        private List<MeshBuffers> patterns;

        private const float TWO_PI = (float)(System.Math.PI * 2);

        private const float K_SMALLEST_RANGE = 0.000001f;

        // % of plane covered with disks when genereated with algorithm below
        public const float POISSON_COVERAGE = 0.6826f;


    }
}
