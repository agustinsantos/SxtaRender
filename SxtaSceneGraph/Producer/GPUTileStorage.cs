using Sxta.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Render.Producer
{
    /// <summary>
    ///  A TileStorage that stores tiles in 2D array textures. Each tile is stored in
    ///  its own layer of the array.
    /// </summary>
    public class GPUTileStorage : TileStorage
    {


        /**
         * A slot managed by a GPUTileStorage. Corresponds to a layer of a texture.
         */
        public class GPUSlot : Slot
        {

            /**
             * The 2D array texture containing the tile stored in this slot.
             */
            public Texture2DArray t;

            /**
             * The layer of the tile in the 2D texture array 't'.
             */
            public readonly int l;

            /**
             * Creates a new GPUSlot.
             *
             * @param owner the TileStorage that manages this slot.
             * @param index the index of t in the list of textures managed by the
             *      tile storage.
             * @param t the 2D array texture in which the tile is stored.
             * @param l the layer of the tile in the 2D texture array t.
             */
            public GPUSlot(TileStorage owner, int index, Texture2DArray t, int l)
                : base(owner)
            {
                this.t = t;
                this.l = l;
                this.index = index;
            }

            /**
             * Deletes this GPUSlot.
             */
            ~GPUSlot() { }

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

            /**
             * Copies a region of the given frame buffer into this slot.
             *
             * @param fb a frame buffer.
             * @param x lower left corner of the area where the pixels must be read.
             * @param y lower left corner of the area where the pixels must be read.
             * @param w width of the area where the pixels must be read.
             * @param h height of the area where the pixels must be read.
             */
            public virtual void copyPixels(FrameBuffer fb, int x, int y, int w, int h)
            {
                fb.copyPixels(0, 0, l, x, y, w, h, t, 0);
            }

            /**
             * Copies a region of the given pixel buffer into this slot. The region
             * Coordinates are relative to the lower left corner of the slot.
             *
             * @param x lower left corner of the part to be replaced in this slot.
             * @param y lower left corner of the part to be replaced in this slot.
             * @param w the width of the part to be replaced in this slot.
             * @param h the height of the part to be replaced in this slot.
             * @param f the texture components in 'pixels'.
             * @param t the type of each component in 'pixels'.
             * @param pixels the pixels to be copied into this slot.
             */
            public virtual void setSubImage(int x, int y, int w, int h, TextureFormat f, PixelType t, Buffer.Parameters s, Buffer pixels)
            {
                this.t.setSubImage(0, x, y, l, w, h, 1, f, t, s, pixels);
            }


            /**
             * The index of 't' in the list of textures managed by the tile storage.
             */
            internal int index;
        }

        /**
         * Creates a new GPUTileStorage. See #init.
         */
        public GPUTileStorage(int tileSize, int nTiles,
             TextureInternalFormat internalf, TextureFormat f, PixelType t, Texture.Parameters params_, bool useTileMap = false)
        {
            init(tileSize, nTiles, internalf, f, t, params_, useTileMap);
        }

        /**
         * Deletes this GPUTileStorage.
         */
        ~GPUTileStorage()
        {
            if (dirtySlots != null)
            {
                //TODO delete[] dirtySlots;
                throw new NotImplementedException();
            }
        }

        /**
         * Returns the number of textures used to store the tiles.
         */
        public int getTextureCount()
        {
            return textures.Count;
        }

        /**
         * Returns the texture storage whose index is given.
         *
         * @param index an index between 0 and #getTextureCount (excluded).
         */
        public Texture2DArray getTexture(int index)
        {
            return textures[index];
        }

        /**
         * Returns the tile map that stores the mapping between logical tile
         * coordinates (level,tx,ty) and storage tile coordinates in this
         * storage. This mapping texture can be used as an indirection texture on GPU
         * to find the content of a tile from its logical coordinates. May be NULL.
         */
        public Texture2D getTileMap()
        {
            return tileMap;
        }

        /**
         * Notifies this manager that the content of the given slot has changed.
         *
         * @param s a slot whose content has changed.
         */
        public void notifyChange(GPUSlot s)
        {
            if (needMipmaps)
            {
                dirtySlots[s.index].Add(s);
                changes = true;
            }
        }

        /**
         * Generates the mipmap levels of the storage textures. This method only
         * updates the textures whose content has changed since the last call to
         * this method. Changes must be notified with #notifyChange.
         */
        public void generateMipMap()
        {
            if (changes)
            {
                int level = 1;
                int width = tileSize / 2;
                while (width >= 1)
                {
                    fbo.setViewport(new Vector4i(0, 0, width, width));
                    for (int n = 0; n < textures.Count; ++n)
                    {
                        fbo.setTextureBuffer((BufferId)(1 << n), textures[n], level, -1);
                    }
                    for (int n = 0; n < textures.Count; ++n)
                    {
                        fbo.setDrawBuffer((BufferId)(1 << n));
                        foreach (GPUSlot s in dirtySlots[n])
                        {
                            mipmapParams.set(new Vector4i(s.index, s.l, level - 1, width));
                            fbo.drawQuad(mipmapProg);
                        }
                    }
                    width /= 2;
                    level += 1;
                }
                for (int n = 0; n < textures.Count; ++n)
                {
                    dirtySlots[n].Clear();
                }
                changes = false;
            }
        }


        /**
         * Creates an uninitialized GPUTileStorage.
         */
        protected GPUTileStorage()
        { }

        /**
         * Initializes this GPUTileStorage.
         *
         * @param tileSize the size in pixel of each (square) tile.
         * @param nTiles the number of slots in this storage.
         * @param tf the texture storage data format on GPU.
         * @param f the texture components in the storage textures.
         * @param t the type of each component in the storage textures.
         * @param params the texture parameters.
         * @param useTileMap the to associate with this tile storage a texture
         *      representing the mapping between logical tile coordinates
         *      (level,tx,ty) and storage tile coordinates (u,v) in this storage.
         *      This mapping texture can be used as an indirection texture on GPU
         *      to find the content of a tile from its logical coordinates. <i>This
         *      option can only be used if nTextures is equal to one</i>.
         */
        protected void init(int tileSize, int nTiles,
            TextureInternalFormat internalf, TextureFormat f, PixelType t, Texture.Parameters params_, bool useTileMap = false)
        {
            base.init(tileSize, nTiles);

            int maxLayers = Texture2DArray.getMaxLayers();
            int nTextures = nTiles / maxLayers + (nTiles % maxLayers == 0 ? 0 : 1);
            needMipmaps = false;

            for (int i = 0; i < nTextures; ++i)
            {
                int nLayers = i == nTextures - 1 && nTiles % maxLayers != 0 ? nTiles % maxLayers : maxLayers;
                Texture2DArray tex = new Texture2DArray(tileSize, tileSize, nLayers,
                    internalf, f, t, params_, new Buffer.Parameters(), new CPUBuffer<byte>());
                needMipmaps = needMipmaps || (tex.hasMipmaps());
                textures.Add(tex);
                tex.generateMipMap();
                for (int j = 0; j < nLayers; ++j)
                {
                    freeSlots.Add(new GPUSlot(this, i, textures[i], j));
                }
            }

            if (needMipmaps)
            {
                Debug.Assert(nTextures <= 8);
                dirtySlots = new HashSet<GPUSlot>[nTextures];
                fbo = new FrameBuffer();
                fbo.setReadBuffer(BufferId.DEFAULT);
                fbo.setDrawBuffers(BufferId.COLOR0 | BufferId.COLOR1);
                mipmapProg = new Program(new Module(330, mipmapShader));
                Sampler s = new Sampler(new Sampler.Parameters()
                                                    .min(TextureFilter.NEAREST)
                                                    .mag(TextureFilter.NEAREST)
                                                    .wrapS(TextureWrap.CLAMP_TO_EDGE)
                                                    .wrapT(TextureWrap.CLAMP_TO_EDGE));
                for (int i = 0; i < nTextures; ++i)
                {
                    string buf = "input_[" + i + "]";
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
                Debug.Assert(nTextures == 1);
                tileMap = new Texture2D(4096, 8, TextureInternalFormat.RG8, TextureFormat.RG, PixelType.UNSIGNED_BYTE,
                    new Texture.Parameters().wrapS(TextureWrap.CLAMP_TO_EDGE).wrapT(TextureWrap.CLAMP_TO_EDGE).min(TextureFilter.NEAREST).mag(TextureFilter.NEAREST),
                    new Buffer.Parameters(), new CPUBuffer<byte>());
            }
        }

        protected void swap(GPUTileStorage s)
        {
            Debug.Assert(false);
        }



        /**
         * The storage textures used to store the tiles.
         */
        private List<Texture2DArray> textures = new List<Texture2DArray>();

        /**
         * True if the storage texture format needs mipmaping.
         */
        private bool needMipmaps;

        /**
         * True if a least one storage texture has changed since the last call to
         * #generateMipMap.
         */
        private bool changes;

        /**
         * The slots whose mipmap levels are not up to date (one set per texture).
         */
        private ISet<GPUSlot>[] dirtySlots;

        /**
         * Framebuffer used to generate mipmaps.
         */
        private FrameBuffer fbo;

        /**
         * Program used to generate mipmaps.
         */
        private Program mipmapProg;

        /**
         * Parameters used to generate a mipmap level.
         */
        private Uniform4i mipmapParams;

        /**
         * The tile map that stores the mapping between logical tile coordinates
         * (level,tx,ty) and storage tile coordinates (u,v) in this storage. May
         * be NULL.
         */
        private Texture2D tileMap;

        private const string mipmapShader = @"
        uniform ivec4 bufferLayerLevelWidth;
        #ifdef _VERTEX_
        layout(location=0) in vec4 vertex;
        void main () { gl_Position = vertex; }
        #endif
        #ifdef _GEOMETRY_
        #extension GL_EXT_geometry_shader4 : enable
        layout(triangles) in;
        layout(triangle_strip,max_vertices=3) out;
        void main() { 
            gl_Layer = bufferLayerLevelWidth.y;
            gl_Position = gl_PositionIn[0]; 
            EmitVertex(); 
            gl_Position = gl_PositionIn[1]; 
            EmitVertex(); 
            gl_Position = gl_PositionIn[2]; 
            EmitVertex();
            EndPrimitive(); 
        }
        #endif
        #ifdef _FRAGMENT_
        uniform sampler2DArray input_[8];
        layout(location=0) out vec4 output_;
        void main() {
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
            output_ = result * 0.25;
        }
        #endif\n";
    }
}
