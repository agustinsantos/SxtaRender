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

using proland;
using Sxta.Render;

namespace Sxta.Proland.Forest
{
    public class GPUBufferTileStorage : TileStorage
    {

        public class GPUBufferSlot : Slot
        {

            public GPUBuffer buffer;

            public readonly int offset;

            public int size;

            public uint date;

            public Query query;


            public GPUBufferSlot(TileStorage owner, GPUBuffer buffer, int offset) : base(owner)
            {
                this.buffer = buffer;
                this.offset = offset;
                this.size = -1;
                this.date = 0;
            }

        }

        public GPUBuffer buffer;

        public MeshBuffers mesh;
        const int VERTEX_SIZE = 24;

        public GPUBufferTileStorage(int tileSize, int nTiles) : base(tileSize, nTiles)
        {
            buffer = new GPUBuffer();
            buffer.setData<byte>(tileSize * nTiles, null, BufferUsage.DYNAMIC_COPY); // TODO best mode?
            for (int i = 0; i < nTiles; ++i)
            {
                freeSlots.Add(new GPUBufferSlot(this, buffer, i * tileSize));
            }
            mesh = new MeshBuffers();
            mesh.addAttributeBuffer(0, 3, VERTEX_SIZE, AttributeType.A32F, false);
            mesh.getAttributeBuffer(0).setBuffer(buffer);
            mesh.addAttributeBuffer(1, 3, VERTEX_SIZE, AttributeType.A32F, false);
            mesh.getAttributeBuffer(1).setBuffer(buffer);
        }
    }
}
