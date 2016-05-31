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

using Sxta.Math;
using Sxta.Render;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sxta.Proland.Forest.XmlResources
{
    public class PlantsResource : ResourceTemplate<Plants>
    {
        public static PlantsResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new PlantsResource(manager, name, desc, e);
        }
        public PlantsResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
        base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,selectProg,shadowProg,renderProg,minLevel,maxLevel,tileCacheSize,maxDistance,lodDistance,minDensity,maxDensity,patternCount,");
            int minLevel;
            int maxLevel;
            int tileCacheSize;
            float maxDistance;
            float lodDistance;
            int minDensity;
            int maxDensity;
            int patternCount;
            getIntParameter(desc, e, "minLevel", out minLevel);
            getIntParameter(desc, e, "maxLevel", out maxLevel);
            getIntParameter(desc, e, "tileCacheSize", out tileCacheSize);
            getFloatParameter(desc, e, "maxDistance", out maxDistance);
            getFloatParameter(desc, e, "lodDistance", out lodDistance);
            getIntParameter(desc, e, "minDensity", out minDensity);
            getIntParameter(desc, e, "maxDensity", out maxDensity);
            getIntParameter(desc, e, "patternCount", out patternCount);

            this.valueC.selectProg = manager.loadResource(getParameter(desc, e, "selectProg")).get() as Program;
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("shadowProg") ))
            {
                this.valueC.shadowProg = manager.loadResource(getParameter(desc, e, "shadowProg")).get() as Program;
            }
            this.valueC.renderProg = manager.loadResource(getParameter(desc, e, "renderProg")).get() as Program;

            if (this.valueC.renderProg.getUniform1f("maxTreeDistance") != null)
            {
                this.valueC.renderProg.getUniform1f("maxTreeDistance").set(maxDistance);
            }

            int minVertices = 2 * maxDensity;
            int maxVertices = 0;
            for (int i = 0; i < patternCount; ++i)
            {
                Mesh<Vector3f, ushort> pattern;
                int density = minDensity + (int)((maxDensity - minDensity) * rand.Next());
                pattern = new Mesh<Vector3f, ushort>(Vector3f.SizeInBytes, sizeof(ushort), MeshMode.POINTS, MeshUsage.GPU_STATIC, density, 0);
                pattern.addAttributeType(0, 3, AttributeType.A32F, false);

                radius = (float)(1.0 / System.Math.Sqrt(density * System.Math.PI / Plants.POISSON_COVERAGE));
                generatePattern(pattern);
                //generateRandomPattern(pattern, minDensity);

                this.valueC.addPattern(pattern.getBuffers());
                minVertices = System.Math.Min(minVertices, pattern.getVertexCount());
                maxVertices = System.Math.Max(maxVertices, pattern.getVertexCount());
            }

            this.valueC = new Plants(minLevel, maxLevel, minVertices, maxVertices, tileCacheSize, maxDistance);

        }

        private const int seed = 1234567;

        public Random rand = new Random(seed);

        public RangeList ranges;

        public PlantsGrid grid;

        public float radius;

        public float randUnsignedInt()
        {
            //union {
            //    float f;
            //    unsigned int ui;
            //}
            //val;
            //val.ui = (lrandom(&rand) & 0xFFFF) | ((lrandom(&rand) & 0xFFFF) << 16);
            return (float)rand.NextDouble();
        }

        public void generatePattern(Mesh<Vector3f, ushort> pattern)
        {
            ranges = new RangeList();
            grid = new PlantsGrid(4.0f * radius, 64);

            List<Vector2f> candidates = new List<Vector2f>();

            Vector2f p = new Vector2f(0.5f, 0.5f);
            candidates.Add(p);
            pattern.addVertex(new Vector3f(p.X, p.Y, randUnsignedInt()));
            grid.AddParticle(p);

            while (candidates.Count > 0)
            {
                // selects a candidate at random
                int c = rand.Next() % candidates.Count;
                p = candidates[c];
                // removes this candidate from the list
                candidates[c] = candidates[candidates.Count - 1];
                candidates.RemoveAt(candidates.Count);

                ranges.Reset(0.0f, 2.0f * M_PI);
                findNeighborRanges(p);

                while (ranges.GetRangeCount() != 0)
                {
                    // selects a range at random
                    RangeList.RangeEntry re = ranges.GetRange(rand.Next() % ranges.GetRangeCount());
                    // selects a point at random in this range
                    float angle = re.min + (re.max - re.min) * rand.Next();
                    ranges.Subtract(angle - M_PI / 3.0f, angle + M_PI / 3.0f);

                    Vector2f pt = p + new Vector2f((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 2.0f * radius;
                    if (pt.X >= 0.0 && pt.X < 1.0 && pt.Y >= 0.0 && pt.Y < 1.0)
                    {
                        candidates.Add(pt);
                        pattern.addVertex(new Vector3f(pt.X, pt.Y, randUnsignedInt()));
                        grid.AddParticle(pt);
                    }
                }
            }
        }

        public void generateRandomPattern(Mesh<Vector3f, ushort> pattern, int n)
        {
            for (int i = 0; i < n; ++i)
            {
                float x = rand.Next();
                float y = rand.Next();
                pattern.addVertex(new Vector3f(x, y, randUnsignedInt()));
            }
        }

        public void findNeighborRanges(Vector2f p)
        {
            Vector2i gridSize = grid.GetGridSize();
            Vector2i cell = grid.GetCell(p);

            float rangeSqrD = 16.0f * radius * radius;

            int n = grid.GetCellSize(cell);
            Vector2f[] neighbors = grid.GetCellContent(cell);
            int count = 0;
            for (int j = 0; j < n; ++j)
            {
                Vector2f ns = neighbors[j];
                if (ns == p)
                {
                    continue;
                }
                Vector2f v = ns - p;
                float sqrD = v.LengthSquared;
                if (sqrD < rangeSqrD)
                {
                    float dist = (float)System.Math.Sqrt(sqrD);
                    float angle = (float)System.Math.Atan2(v.Y, v.X);
                    float theta = MathHelper.Safe_acos(0.25f * dist / radius);
                    count++;
                    ranges.Subtract(angle - theta, angle + theta);
                }
            }
        }
        private const float M_PI = (float)System.Math.PI;
    }
}
