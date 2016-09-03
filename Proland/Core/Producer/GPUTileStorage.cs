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
using Sxta.Render;
using Sxta.Render.Core;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace proland
{
    public class GPUTileStorage : TileStorage, ISwappable<GPUTileStorage>
    {
//#if DEBUG
//        TraceOpenTKDisposableObject traceDisposable;
//#endif

        const string mipmapShader = @"
uniform ivec4 bufferLayerLevelWidth;
#ifdef _VERTEX_
layout(location= 0) in vec4 vertex;
void main() { gl_Position = vertex; }
#endif
#ifdef _GEOMETRY_
#extension GL_EXT_geometry_shader4 : enable
layout(triangles) in;
layout(triangle_strip, max_vertices= 3) out;
void main() { gl_Layer = bufferLayerLevelWidth.y; gl_Position = gl_PositionIn[0]; EmitVertex(); gl_Position = gl_PositionIn[1]; EmitVertex(); gl_Position = gl_PositionIn[2]; EmitVertex(); EndPrimitive(); }
#endif
#ifdef _FRAGMENT_
uniform sampler2DArray input_[8];
layout(location= 0) out vec4 output_;
void main()
    {
vec2 xy = floor(gl_FragCoord.xy);
vec4 uv = vec4(xy + vec2(0.25), xy + vec2(0.75)) / float(bufferLayerLevelWidth.w);
      vec4 result;
      switch (bufferLayerLevelWidth.x) {
      case 0:
          result = texture(input_[0], vec3(uv.zw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
          result += texture(input_[0], vec3(uv.xw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
          result += texture(input_[0], vec3(uv.zy, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
          result += texture(input_[0], vec3(uv.zw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
          break;
      case 1:
          result = texture(input_[1], vec3(uv.zw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
          result += texture(input_[1], vec3(uv.xw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
          result += texture(input_[1], vec3(uv.zy, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         result += texture(input_[1], vec3(uv.zw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
          break;
      case 2:
          result = texture(input_[2], vec3(uv.zw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
          result += texture(input_[2], vec3(uv.xw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
          result += texture(input_[2], vec3(uv.zy, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         result += texture(input_[2], vec3(uv.zw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         break;
     case 3:
         result = texture(input_[3], vec3(uv.zw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         result += texture(input_[3], vec3(uv.xw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         result += texture(input_[3], vec3(uv.zy, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         result += texture(input_[3], vec3(uv.zw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         break;
     case 4:
         result = texture(input_[4], vec3(uv.zw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         result += texture(input_[4], vec3(uv.xw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         result += texture(input_[4], vec3(uv.zy, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         result += texture(input_[4], vec3(uv.zw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         break;
     case 5:
         result = texture(input_[5], vec3(uv.zw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         result += texture(input_[5], vec3(uv.xw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         result += texture(input_[5], vec3(uv.zy, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         result += texture(input_[5], vec3(uv.zw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         break;
     case 6:
         result = texture(input_[6], vec3(uv.zw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         result += texture(input_[6], vec3(uv.xw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         result += texture(input_[6], vec3(uv.zy, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         result += texture(input_[6], vec3(uv.zw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         break;
     case 7:
         result = texture(input_[7], vec3(uv.zw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         result += texture(input_[7], vec3(uv.xw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         result += texture(input_[7], vec3(uv.zy, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         result += texture(input_[7], vec3(uv.zw, bufferLayerLevelWidth.y), bufferLayerLevelWidth.z);
         break;
     }
	    output_ = result* 0.25;
}
#endif";

        /// <summary>
        /// A slot managed by a GPUTileStorage. Corresponds to a layer of a texture.
        /// </summary>
        public class GPUSlot : Slot
        {
            /// <summary>
            /// The 2D array texture containing the tile stored in this slot.
            /// </summary>
            public Texture2DArray t;

            /// <summary>
            /// The layer of the tile in the 2D texture array 't'.
            /// </summary>
            public readonly int l;

            /// <summary>
            /// Creates a new GPUSlot.
            /// </summary>
            /// <param name="owner">the TileStorage that manages this slot.</param>
            /// <param name="index">the index of t in the list of textures managed by the
            ///       tile storage.</param>
            /// <param name="t">the 2D array texture in which the tile is stored.</param>
            /// <param name="l">the layer of the tile in the 2D texture array t.</param>
            public GPUSlot(TileStorage owner, int index, Texture2DArray t, int l) : base(owner)
            {
                this.l = l;
                this.t = t;
                this.index = index;
            }

            public int getIndex()
            {
                return index;
            }
            public int getWidth()
            {
                return t.getWidth();
            }
            public int getHeight()
            {
                return t.getHeight();
            }


            /// <summary>
            /// Copies a region of the given frame buffer into this slot. 
            /// </summary>
            /// <param name="fb">a frame buffer.</param>
            /// <param name="x">lower left corner of the area where the pixels must be read.</param>
            /// <param name="y">lower left corner of the area where the pixels must be read.</param>
            /// <param name="w">width of the area where the pixels must be read.</param>
            /// <param name="h">height of the area where the pixels must be read.</param>
            public virtual void copyPixels(FrameBuffer fb, int x, int y, int w, int h)
            {
                fb.copyPixels(0, 0, l, x, y, w, h, t, 0);
            }



            /// <summary>
            /// Copies a region of the given pixel buffer into this slot. The region
            /// Coordinates are relative to the lower left corner of the slot. 
            /// </summary>
            /// <param name="x">lower left corner of the part to be replaced in this slot.</param>
            /// <param name="y">lower left corner of the part to be replaced in this slot.</param>
            /// <param name="w">the width of the part to be replaced in this slot.</param>
            /// <param name="h">the height of the part to be replaced in this slot.</param>
            /// <param name="f">the texture components in 'pixels'.</param>
            /// <param name="t">the type of each component in 'pixels'.</param>
            /// <param name="s"></param>
            /// <param name="pixels">the pixels to be copied into this slot.</param>
            public virtual void setSubImage(int x, int y, int w, int h, TextureFormat f, PixelType t, Sxta.Render.Buffer.Parameters s, Sxta.Render.Buffer pixels)
            {
                this.t.setSubImage(0, x, y, l, w, h, 1, f, t, s, pixels);
            }

            /// <summary>
            /// The index of 't' in the list of textures managed by the tile storage.
            /// </summary>
            internal int index;
        }

        /// <summary>
        /// Creates a new GPUTileStorage. See #init.
        /// </summary>
        public GPUTileStorage(int tileSize, int nTiles, TextureInternalFormat internalf, TextureFormat f, PixelType t, Texture.Parameters _params, bool useTileMap = false) : base(tileSize, nTiles)
        {
//#if DEBUG
//            traceDisposable = new TraceOpenTKDisposableObject();
//#endif

            init(tileSize, nTiles, internalf, f, t, _params, useTileMap);
        }

        ~GPUTileStorage()
        {
            //Debugger.Break();
            Dispose(false);
        }

        #region IDisposable implementation

        protected override void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
//#if DEBUG
//                traceDisposable.CheckDispose();
//#endif
                if (disposing)
                {
                    base.Dispose(disposing);
                    if (dirtySlots != null)
                        foreach (SortedSet<GPUSlot> set in dirtySlots)
                        {
                            if (set != null)
                            {
                                foreach (GPUSlot slot in set)
                                {
                                    slot.Dispose();
                                }
                                set.Clear();
                            }
                        }
                    dirtySlots = null;


                    if (textures != null)
                        foreach (Texture tex in textures)
                        {
                            if (tex != null)
                                tex.Dispose();
                        }
                    textures.Clear();
                    textures = null;

                    if (fbo != null)
                        fbo.Dispose();
                    if (mipmapProg != null)
                        mipmapProg.Dispose();
                    if (tileMap != null)
                        tileMap.Dispose();
                    m_Disposed = true;
                }
            }
        }

        #endregion
        /// <summary>
        /// Returns the texture storage whose index is given.
        /// </summary>
        /// <param name="index">index an index between 0 and #getTextureCount (excluded).</param>
        /// <returns></returns>
        public Texture2DArray getTexture(int index)
        {
            return textures[index];
        }
        /// <summary>
        /// Returns the number of textures used to store the tiles.
        /// </summary>
        /// <returns></returns>
        public int getTextureCount()
        {
            return textures.Count();
        }

        /// <summary>
        /// Returns the tile map that stores the mapping between logical tile coordinates(level, tx, ty)
        /// and storage tile coordinates in this storage.This mapping texture can be used as an indirection
        /// texture on GPU to find the content of a tile from its logical coordinates.May be NULL.
        /// </summary>
        /// <returns></returns>
        public Texture2D getTileMap()
        {
            return tileMap;
        }

        /// <summary>
        /// Notifies this manager that the content of the given slot has changed.
        /// </summary>
        /// <param name="s">a slot whose content has changed</param>
        public void notifyChange(GPUSlot s)
        {
            if (needMipmaps)
            {
                dirtySlots[s.index].Add(s);
                changes = true;
            }
        }

        /// <summary>
        /// Generates the mipmap levels of the storage textures. This method only
        /// updates the textures whose content has changed since the last call to
        /// this method.Changes must be notified with #notifyChange.
        /// </summary>
        public void generateMipMap()
        {
            if (changes)
            {
                int level = 1;
                int width = tileSize / 2;
                while (width >= 1)
                {
                    fbo.setViewport(new Sxta.Math.Vector4i(0, 0, width, width));
                    for (int n = 0; n < textures.Count(); n++)
                    {
                        fbo.setTextureBuffer((BufferId)(1 << n), textures[n], level, -1);
                    }
                    for (int n = 0; n < textures.Count(); n++)
                    {
                        fbo.setDrawBuffer((BufferId)(1 << n));
                        foreach (GPUSlot s in dirtySlots[n])
                        {
                            mipmapParams.set(new Sxta.Math.Vector4i(s.index, s.l, level - 1, width));
                            fbo.drawQuad(mipmapProg);
                        }
                    }
                    width /= 2;
                    level += 1;
                }
                for (int n = 0; n < textures.Count(); n++)
                {
                    dirtySlots[n].Clear();
                }
                changes = false;
            }
        }

        internal void init(int tileSize, int nTiles, TextureInternalFormat internalf, TextureFormat f, PixelType t, Texture.Parameters _params, bool useTileMap)
        {
            base.init(tileSize, nTiles);

            int maxLayers = Texture2DArray.getMaxLayers();
            int nTextures = nTiles / maxLayers + (nTiles % maxLayers == 0 ? 0 : 1);
            needMipmaps = false;
            for (int i = 0; i < nTextures; i++)
            {
                int nLayers = i == nTextures - 1 && nTiles % maxLayers != 0 ? nTiles % maxLayers : maxLayers;
                Texture2DArray tex = new Texture2DArray(tileSize, tileSize, nLayers, internalf, f, t, _params, new Sxta.Render.Buffer.Parameters(), new CPUBuffer<byte>());
                needMipmaps = needMipmaps || (tex.hasMipmaps());
                textures.Add(tex);
                tex.generateMipMap();
                for (int j = 0; j < nLayers; j++)
                {
                    freeSlots.Add(new GPUSlot(this, i, textures[i], j));
                }
            }

            if (needMipmaps)
            {
                Debug.Assert(nTextures <= 8, "GPUTileStorage");
                dirtySlots = new SortedSet<GPUSlot>[nTextures];
                fbo = new FrameBuffer();
                fbo.setReadBuffer(BufferId.DEFAULT);
                fbo.setDrawBuffers(BufferId.COLOR0 | BufferId.COLOR1);
                mipmapProg = new Program(new Module(330, mipmapShader));
                Sampler s = new Sampler(new Sampler.Parameters().min(TextureFilter.NEAREST).mag(TextureFilter.NEAREST).wrapS(TextureWrap.CLAMP_TO_EDGE).wrapT(TextureWrap.CLAMP_TO_EDGE));
                for (int i = 0; i < nTextures; i++)
                {
                    string buf = String.Format("input_[{0}]", i);
                    mipmapProg.getUniformSampler(buf).set(textures[i]);
                    mipmapProg.getUniformSampler(buf).setSampler(s);
                }
                mipmapParams = mipmapProg.getUniform4i("bufferLayerLevelWidth");
            }
            else
            {
                dirtySlots = null;
            }
            changes = false;
            if (useTileMap)
            {
                Debug.Assert(nTextures == 1, "GPUTileStorage");
                tileMap = new Texture2D(4096, 8, TextureInternalFormat.RG8, TextureFormat.RG, PixelType.UNSIGNED_BYTE,
                    new Texture.Parameters().wrapS(TextureWrap.CLAMP_TO_EDGE).wrapT(TextureWrap.CLAMP_TO_EDGE).min(TextureFilter.NEAREST).mag(TextureFilter.NEAREST),
                    new Sxta.Render.Buffer.Parameters(), new CPUBuffer<byte>());
            }
        }

        public void swap(GPUTileStorage s)
        {
            Debug.Assert(false, "GPUTileStorage");
        }

        /// <summary>
        /// The storage textures used to store the tiles.
        /// </summary>
        private List<Texture2DArray> textures = new List<Texture2DArray>();

        /// <summary>
        /// True if the storage texture format needs mipmaping.
        /// </summary>
        private bool needMipmaps;

        /// <summary>
        /// True if a least one storage texture has changed since the last call to #generateMipMap.
        /// </summary>
        private bool changes;

        /// <summary>
        /// The slots whose mipmap levels are not up to date (one set per texture).
        /// </summary>
        private SortedSet<GPUSlot>[] dirtySlots;

        /// <summary>
        /// Framebuffer used to generate mipmaps.
        /// </summary>
        private FrameBuffer fbo;

        /// <summary>
        /// Program used to generate mipmaps
        /// </summary>
        private Program mipmapProg;

        /// <summary>
        /// Parameters used to generate a mipmap level.
        /// </summary>
        private Uniform4i mipmapParams;
        /// <summary>
        /// The tile map that stores the mapping between logical tile coordinates (level,tx,ty) 
        /// and storage tile coordinates(u, v) in this storage.May be NULL.
        /// </summary>
        private Texture2D tileMap;

    }
}
