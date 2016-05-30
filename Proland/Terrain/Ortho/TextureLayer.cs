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
using Sxta.Math;
using System.Collections.Generic;

namespace Sxta.Proland.Terrain
{
	
	public class TextureLayer : TileLayer
	{
		GameObject m_sourceProducerGO;
		TileProducer m_sourceProducer;

		Material m_material;

		RenderTexture m_texture;

		TileProducer m_targetProducer;

		protected override void Start () 
		{
			base.Start();

			m_targetProducer = GetComponent<TileProducer>();
			m_sourceProducer = m_sourceProducerGO.GetComponent<TileProducer>();

			int targetSize = m_targetProducer.GetCache().GetStorage(0).GetTileSize();
			int sourceSize = m_sourceProducer.GetCache().GetStorage(0).GetTileSize();

			if(targetSize != sourceSize && targetSize != sourceSize-1) {
				throw new InvalidParameterException("Target tile size must equal source tile size or source tile size-1");
			}
			
			if(m_targetProducer.GetBorder() != m_sourceProducer.GetBorder()) {
				throw new InvalidParameterException("Target border size must be equal to source border size");
			}

			GPUTileStorage storage = m_targetProducer.GetCache().GetStorage(0) as GPUTileStorage;
			
			if(storage == null) {
				throw new InvalidStorageException("Target storage must be a GPUTileStorage");
			}

			storage = m_sourceProducer.GetCache().GetStorage(0) as GPUTileStorage;
			
			if(storage == null) {
				throw new InvalidStorageException("Source storage must be a GPUTileStorage");
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if(m_texture != null)
				m_texture.Release();
		}

		public override bool  doCreateTile(int level, int tx, int ty, TileStorage.Slot slot) 
		{

			GPUTileStorage.GPUSlot gpuSlot = slot[0] as GPUTileStorage.GPUSlot;

			RenderTexture target = gpuSlot.GetTexture();

			if(m_texture == null)
				m_texture = RenderTexture.Instantiate(target) as RenderTexture;

			Tile sourceTile = m_sourceProducer.FindTile(level, tx, ty, false, true);

			GPUTileStorage.GPUSlot sourceGpuSlot = null;
			
			if(sourceTile != null) {
				sourceGpuSlot = sourceTile.GetSlot(0) as GPUTileStorage.GPUSlot;
			}
			else {
				throw new MissingTileException("Find source producer tile failed");
			}

			Vector3f coords = new Vector3f(0,0,1);

			int targetSize = m_targetProducer.GetCache().GetStorage(0).GetTileSize();
			int sourceSize = m_sourceProducer.GetCache().GetStorage(0).GetTileSize();

			if(targetSize == sourceSize-1)
			{
				coords.x = 1.0f / (float)sourceSize;
				coords.y = 1.0f / (float)sourceSize;
				coords.z = 1.0f - coords.x;
			}

			Graphics.Blit(target, m_texture);

			m_material.SetTexture("_Target", m_texture);
			m_material.SetTexture("_Source", sourceGpuSlot.GetTexture());
			m_material.SetVector("_Coords", coords);

			Graphics.Blit(null, target, m_material);

		}

	}

}


























