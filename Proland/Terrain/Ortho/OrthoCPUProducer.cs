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
using System.Collections.Generic;
using System.IO;

namespace Sxta.Proland.Terrain
{
	public class OrthoCPUProducer : TileProducer 
	{
		/**
		* The name of the file containing the tiles to load.
		*/
		string m_fileName = "/Proland/Textures/Terrain/Final/Color.proland";

		/**
		* The number of components per pixel in the tiles to load.
		*/
		int m_channels;

		/**
		* The size of the tiles to load, without borders. A tile contains
		* (tileSize+4)*(tileSize+4)*channels samples.
		*/
		int m_tileSize;

		/**
		* The size in pixels of the border around each tile. A tile contains
		* (tileSize+4)*(tileSize+4)*channels samples.
		*/
		int m_border;

		/**
		* The maximum level of the stored tiles on disk (inclusive).
		*/
		int m_maxLevel;

		/**
		* The offsets of each tile on disk, relatively to offset, for each tile id
		*/
		long[] m_offsets;

		protected  void Start () 
		{
			base.Start ();
			
			CPUTileStorage storage = GetCache().GetStorage(0) as CPUTileStorage;
			
			if (storage == null) {
				throw new InvalidStorageException ("Storage must be a CPUTileStorage");
			}
			
			if(storage.GetDataType() != CPUTileStorage.DATA_TYPE.BYTE) {
				throw new InvalidStorageException ("Storage data type must be byte");
			}

			byte[] data = new byte[7 * 4];
			
			using(Stream stream = new FileStream(Application.dataPath + m_fileName, FileMode.Open))
			{
				stream.Seek(0, SeekOrigin.Begin);
				stream.Read(data, 0, data.Length);
			}
			
			m_maxLevel = System.BitConverter.ToInt32(data, 0);
			m_tileSize = System.BitConverter.ToInt32(data,4);
			m_channels = System.BitConverter.ToInt32(data, 8);
			m_border = System.BitConverter.ToInt32(data, 12);
			//int root = System.BitConverter.ToInt32(data, 16);
			//int tx = System.BitConverter.ToInt32(data, 20);
			//int ty = System.BitConverter.ToInt32(data, 24);

			if(storage.GetChannels() != m_channels) {
				throw new InvalidStorageException ("Storage channels must be " + m_channels);
			}

//			Debug.Log("Proland::OrthoCPUProducer::Start - max level = " + m_maxLevel);
//			Debug.Log("Proland::OrthoCPUProducer::Start - tile size = " + m_tileSize);
//			Debug.Log("Proland::OrthoCPUProducer::Start - channels = " + m_channels);
//			Debug.Log("Proland::OrthoCPUProducer::Start - border = " + m_border);
//			Debug.Log("Proland::OrthoCPUProducer::Start - root level = " + root);
//			Debug.Log("Proland::OrthoCPUProducer::Start - root tx = " + tx);
//			Debug.Log("Proland::OrthoCPUProducer::Start - root ty = " + ty);

			int ntiles = ((1 << (m_maxLevel * 2 + 2)) - 1) / 3;
			m_offsets = new long[ntiles * 2];
			
			data = new byte[ntiles * 2 * 8];
			
			using(Stream stream = new FileStream(Application.dataPath + m_fileName, FileMode.Open))
			{
				stream.Seek(7 * 4, SeekOrigin.Begin);
				stream.Read(data, 0, data.Length);
			}
			
			for(int i = 0; i < ntiles*2; i++)
			{
				m_offsets[i] = System.BitConverter.ToInt64(data, 8 * i);
				//Debug.Log("Proland::OrthoCPUProducer::Start - offset = " + m_offsets[i]);
			}
			
			//Debug.Log("Proland::OrthoCPUProducer::Start - Num offsets = " + m_offsets.Length);

		}

		public int GetChannels() 
		{
			return m_channels;
		}

		public override int GetBorder()
		{
			return m_border;
		}
		
		public override bool HasTile(int level, int tx, int ty)
		{
			return level <= m_maxLevel;
		}

		int GetTileId(int level, int tx, int ty)
		{
			return tx + ty * (1 << level) + ((1 << (2 * level)) - 1) / 3;
		}

		public override void DoCreateTile (int level, int tx, int ty, List<TileStorage.Slot> slot)
		{

			CPUTileStorage.CPUSlot<byte> cpuSlot = slot[0] as CPUTileStorage.CPUSlot<byte>;
			cpuSlot.ClearData();

			int tileid = GetTileId(level, tx, ty);

			long fsize = m_offsets [2 * tileid + 1] - m_offsets [2 * tileid];
			
			if(fsize > (m_tileSize + 2*m_border) * (m_tileSize + 2*m_border) * m_channels)
				throw new InvalidParameterException("file size of tile is larger than actual tile size");

			using(Stream stream = new FileStream(Application.dataPath + m_fileName, FileMode.Open))
			{
				stream.Seek(m_offsets[2 * tileid], SeekOrigin.Begin);
				stream.Read(cpuSlot.GetData(), 0, cpuSlot.GetData().Length);
			}

			base.DoCreateTile (level, tx, ty, slot);
		}
		
	}
}
































