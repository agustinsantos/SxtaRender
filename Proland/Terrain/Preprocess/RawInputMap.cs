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
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Sxta.Proland.Terrain
{
	/*
	 * Used to load data from a raw file. The file maybe 8 or 16 bit.
	 * If the file is 16 bit it may have a mac or windows byte order.
	 * If the file is large the option of caching maybe enabed so 
	 * only the tiles that are need are loaded from the file. 
	 * This is slower but will allow files that are larger than the maximum
	 * memory of the system to be processed.
	 * 
	 */
	public class RawInputMap : InputMap 
	{
		enum BYTE_ORDER { WINDOWS, MAC };

		enum BYTES { BIT16, BIT8 };

		//The width of this map.
		int width;
		
		//The height of this map.
		int height;
		
		//The number of components per pixel of this map.
		int channels;

         string fileName = "Textures/Terrain/HeightMap.raw";

        BYTE_ORDER m_byteOrder = BYTE_ORDER.MAC;

		BYTES m_bytes = BYTES.BIT16;

		bool useCaching = false;

		float[] data;

		public override int Width
        {
            get
            {
                return width;
            }
        }

		public override int Height
        {
            get
            {
                return height;
            }
        }

		public override int GetChannels() {
			return channels;
		}

        protected override void Start()
        {
             base.Start();

            if (!useCaching)
            {
                //If caching not used load all data into memory.

                if (m_bytes == BYTES.BIT8)
                    LoadRawFile8(fileName);
                else if (m_bytes == BYTES.BIT16)
                    LoadRawFile16(fileName, m_byteOrder == BYTE_ORDER.MAC);

                Debug.WriteLine("Proland.RawInputMap.Start - " + fileName + " loaded");
            }
        }

        public override Vector4f GetValue (int x, int y)
		{
			Vector4f v = new Vector4f();

			x = MathHelper.Clamp(x, 0, width-1);
			y = MathHelper.Clamp(y, 0, height-1);

			v.X = data[ (x + y * width) * channels + 0 ];

			if(channels > 1) v.Y = data[ (x + y * width) * channels + 1 ];
			if(channels > 2) v.Z = data[ (x + y * width) * channels + 2 ];
			if(channels > 3) v.W = data[ (x + y * width) * channels + 3 ];

			return v;
		}

		public override float[] GetValues (int tx, int ty)
		{
			if(!useCaching)
			{	
				//If caching not used load all data into memory.
				return base.GetValues(tx, ty);
			}
			else
			{
				//If caching is used load only the tile needed into memory.

				int c = GetChannels();
				int tileSize = GetTileSize();
				
				float[] v = new float[tileSize * tileSize * c];
				float[] strip = new float[tileSize * c];

				for (int j = 0; j < tileSize; ++j) 
				{
					//the index into the file that the current strip can be found at
					long idx = ((long)tx + (long)(ty+j) * (long)width) * (long)c;

					//The data for a 2D map can be accessed in the file in contiguous strips
					if(m_bytes == BYTES.BIT8)
						LoadStrip8(fileName, idx, strip);
					else if(m_bytes == BYTES.BIT16)
						LoadStrip16(fileName, (tx + (ty+j) * width) * c * 2, strip, m_byteOrder == BYTE_ORDER.MAC);

					for (int i = 0; i < tileSize; ++i) 
					{
						int off = (i + j * tileSize) * c;
						v [off] = strip[i * c + 0];
						
						if (c > 1) {
							v [off + 1] = strip[i * c + 1];
						}
						if (c > 2) {
							v [off + 2] = strip[i * c + 2];
						}
						if (c > 3) {
							v [off + 3] = strip[i * c + 3];
						}
					}
				}
			
				return v;
			}
		}

		void LoadRawFile8(string path) 
		{	
			long size = width * height * channels;
			byte[] data = new byte[size];
			
			using(Stream stream = new FileStream(path, FileMode.Open))
			{
				stream.Seek(0, SeekOrigin.Begin);
				stream.Read(data, 0, data.Length);
			}
			
			this.data = new float[size];
			
			for(long x = 0; x < size; x++) 
			{
				this.data[x] = (float)data[x] / 255.0f;
			};
			
		}

		void LoadRawFile16(string path, bool bigendian) 
		{	
			long size = width * height * channels;
			byte[] data = new byte[size * 2];

			using(Stream stream = new FileStream(path, FileMode.Open))
			{
				stream.Seek(0, SeekOrigin.Begin);
				stream.Read(data, 0, data.Length);
			}

			this.data = new float[size];

			for(long x = 0, i = 0; x < size; x++) 
			{
				//Extract 16 bit data and normalize.
				this.data[x] = (bigendian) ? (data[i++]*256.0f + data[i++]) : (data[i++] + data[i++]*256.0f);
				this.data[x] /= 65535.0f;
			};

		}

		void LoadStrip8(string path, long off, float[] strip)
		{
			
			byte[] data = new byte[strip.Length];
			
			using(Stream stream = new FileStream(path, FileMode.Open))
			{
				stream.Seek(off, SeekOrigin.Begin);
				stream.Read(data, 0, data.Length);
			}
			
			for(int x = 0; x < strip.Length; x++) 
			{
				strip[x] = (float)data[x] / 255.0f;
			};
		}

		void LoadStrip16(string path, long off, float[] strip, bool bigendian)
		{

			byte[] data = new byte[strip.Length*2];

			using(Stream stream = new FileStream(path, FileMode.Open))
			{
				stream.Seek(off, SeekOrigin.Begin);
				stream.Read(data, 0, data.Length);
			}

			for(int x = 0, i = 0; x < strip.Length; x++) 
			{
				//Extract 16 bit data and normalize.
				strip[x] = (bigendian) ? (data[i++]*256.0f + data[i++]) : (data[i++] + data[i++]*256.0f);
				strip[x] /= 65535.0f;
			};
		}


	}
}


























