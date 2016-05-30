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

namespace Sxta.Proland.Terrain
{
	
	public class OrthoProducer : TileProducer
	{

		public class Uniforms
		{
			public int tileWidth, coarseLevelSampler, coarseLevelOSL;
			public int noiseSampler, noiseUVLH, noiseColor;
			public int noiseRootColor;
			public int residualOSH, residualSampler;

			public Uniforms()
			{
				tileWidth = Shader.PropertyToID("_TileWidth");
				coarseLevelSampler = Shader.PropertyToID("_CoarseLevelSampler");
				coarseLevelOSL = Shader.PropertyToID("_CoarseLevelOSL");
				noiseSampler = Shader.PropertyToID("_NoiseSampler");
				noiseUVLH = Shader.PropertyToID("_NoiseUVLH");
				noiseColor = Shader.PropertyToID("_NoiseColor");
				noiseRootColor = Shader.PropertyToID("_NoiseRootColor");
				residualOSH = Shader.PropertyToID("_ResidualOSH");
				residualSampler = Shader.PropertyToID("_ResidualSampler");
			}
		}

		GameObject m_orthoCPUProducerGO;
		OrthoCPUProducer m_orthoCPUProducer;

		//The Program to perform the upsampling and add procedure on GPU.
		Material m_upsampleMat;

		Color m_rootNoiseColor = new Color(0.5f,0.5f,0.5f,0.5f);

		Color m_noiseColor = new Color(1.0f,1.0f,1.0f,1.0f);

     	//Maximum quadtree level, or -1 to allow any level
		int m_maxLevel = -1;

		bool m_hsv = true;

		int m_seed = 0;

		float[] m_noiseAmp = new float[]{0,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255};

		Uniforms m_uniforms;

		PerlinNoise m_noise;

		Texture2D[] m_noiseTextures;

		Texture2D m_residueTex;

		protected override void Start () 
		{
			base.Start();

			if(m_orthoCPUProducerGO != null)
				m_orthoCPUProducer = m_orthoCPUProducerGO.GetComponent<OrthoCPUProducer>();

			int tileSize = GetCache().GetStorage(0).GetTileSize();

			if(m_orthoCPUProducer != null && m_orthoCPUProducer.GetTileSize(0) != tileSize) {
				throw new InvalidParameterException("ortho CPU tile size must match ortho tile size");
			}

			GPUTileStorage storage = GetCache().GetStorage(0) as GPUTileStorage;
			
			if(storage == null) {
				throw new InvalidStorageException("Storage must be a GPUTileStorage");
			}

			m_uniforms = new Uniforms();

			m_noise = new PerlinNoise(m_seed);

			m_residueTex = new Texture2D(tileSize, tileSize, TextureFormat.ARGB32, false);
			m_residueTex.wrapMode = TextureWrapMode.Clamp;
			m_residueTex.filterMode = FilterMode.Point;

			CreateOrthoNoise();
		}
		
		public override int GetBorder() {
			return 2;
		}
		
		public override bool HasTile(int level, int tx, int ty) {
			return (m_maxLevel == -1 || level <= m_maxLevel);
		}

		public override void DoCreateTile(int level, int tx, int ty, List<TileStorage.Slot> slot)
		{

			GPUTileStorage.GPUSlot gpuSlot = slot[0] as GPUTileStorage.GPUSlot;
			
			int tileWidth = gpuSlot.GetOwner().GetTileSize();
			int tileSize = tileWidth - 4;

			GPUTileStorage.GPUSlot parentGpuSlot = null;
			Tile parentTile = null;
			
			if (level > 0) 
			{	
				parentTile = FindTile(level - 1, tx / 2, ty / 2, false, true);
				
				if(parentTile != null)
					parentGpuSlot = parentTile.GetSlot(0) as GPUTileStorage.GPUSlot;
				else  {
					throw new MissingTileException("Find parent tile failed");
				}
			}

			m_upsampleMat.SetFloat(m_uniforms.tileWidth, tileWidth);

			if (level > 0) 
			{
				RenderTexture tex = parentGpuSlot.GetTexture();

				m_upsampleMat.SetTexture(m_uniforms.coarseLevelSampler, tex);

				float dx = (float)(tx % 2) * (float)(tileSize / 2);
				float dy = (float)(ty % 2) * (float)(tileSize / 2);
				
				Vector4f coarseLevelOSL = new Vector4((dx+0.5f) / (float)tex.width, (dy+0.5f) / (float)tex.height, 1.0f / (float)tex.width, 0.0f);
				
				m_upsampleMat.SetVector(m_uniforms.coarseLevelOSL, coarseLevelOSL);
			} 
			else {
				m_upsampleMat.SetVector(m_uniforms.coarseLevelOSL,  new Vector4(-1.0f, -1.0f, -1.0f, -1.0f));
			}

			if (m_orthoCPUProducer != null && m_orthoCPUProducer.HasTile(level, tx, ty)) 
			{
				Tile orthoCPUTile = m_orthoCPUProducer.FindTile(level, tx, ty, false, true);
				CPUTileStorage.CPUSlot<byte> orthoCPUSlot = null;
				
				if(orthoCPUTile != null) {
					orthoCPUSlot = orthoCPUTile.GetSlot(0) as CPUTileStorage.CPUSlot<byte>;
				}
				else  {
					throw new MissingTileException("Find orthoCPU tile failed");
				}

				int c = m_orthoCPUProducer.GetChannels();
				Color32 col = new Color32();
				byte[] data = orthoCPUSlot.GetData();

				for(int x = 0; x < tileWidth; x++)
				{
					for(int y = 0; y < tileWidth; y++)
					{
						col.r = data[(x+y*tileWidth)*c];

						if(c > 1) col.g = data[(x+y*tileWidth)*c+1];
						if(c > 2) col.b = data[(x+y*tileWidth)*c+2];
						if(c > 3) col.a = data[(x+y*tileWidth)*c+3];

						m_residueTex.SetPixel(x, y, col);
					}
				}

				m_residueTex.Apply();

				m_upsampleMat.SetTexture(m_uniforms.residualSampler, m_residueTex);
				m_upsampleMat.SetVector(m_uniforms.residualOSH, new Vector4( 0.5f/(float)tileWidth, 0.5f/(float)tileWidth, 1.0f/(float)tileWidth, 0.0f));
			} 
			else {
				m_upsampleMat.SetTexture(m_uniforms.residualSampler, null);
				m_upsampleMat.SetVector(m_uniforms.residualOSH, new Vector4( -1,-1,-1,-1));
			}

			float rs = level < m_noiseAmp.Length ? m_noiseAmp[level] : 0.0f;
			
			int noiseL = 0;
			int face = GetTerrainNode().GetFace();

			if(rs != 0.0f)
			{
				if (face == 1) 
				{
					int offset = 1 << level;
					int bottomB = m_noise.Noise2D(tx + 0.5f, ty + offset) > 0.0f ? 1 : 0;
					int rightB = (tx == offset - 1 ? m_noise.Noise2D(ty + offset + 0.5f, offset) : m_noise.Noise2D(tx + 1.0f, ty + offset + 0.5f)) > 0.0f ? 2 : 0;
					int topB = (ty == offset - 1 ? m_noise.Noise2D((3.0f * offset - 1.0f - tx) + 0.5f, offset) : m_noise.Noise2D(tx + 0.5f, ty + offset + 1.0f)) > 0.0f ? 4 : 0;
					int leftB = (tx == 0 ? m_noise.Noise2D((4.0f * offset - 1.0f - ty) + 0.5f, offset) : m_noise.Noise2D(tx, ty + offset + 0.5f)) > 0.0f ? 8 : 0;
					noiseL = bottomB + rightB + topB + leftB;
				} 
				else if (face == 6) 
				{
					int offset = 1 << level;
					int bottomB = (ty == 0 ? m_noise.Noise2D((3.0f * offset - 1.0f - tx) + 0.5f, 0) : m_noise.Noise2D(tx + 0.5f, ty - offset)) > 0.0f ? 1 : 0;
					int rightB = (tx == offset - 1.0f ? m_noise.Noise2D((2.0f * offset - 1.0f - ty) + 0.5f, 0) : m_noise.Noise2D(tx + 1.0f, ty - offset + 0.5f)) > 0.0f ? 2 : 0;
					int topB = m_noise.Noise2D(tx + 0.5f, ty - offset + 1.0f) > 0.0f ? 4 : 0;
					int leftB = (tx == 0 ? m_noise.Noise2D(3.0f * offset + ty + 0.5f, 0) : m_noise.Noise2D(tx, ty - offset + 0.5f)) > 0.0f ? 8 : 0;
					noiseL = bottomB + rightB + topB + leftB;
				} 
				else 
				{
					int offset = (1 << level) * (face - 2);
					int bottomB = m_noise.Noise2D(tx + offset + 0.5f, ty) > 0.0f ? 1 : 0;
					int rightB = m_noise.Noise2D((tx + offset + 1) % (4 << level), ty + 0.5f) > 0.0f ? 2 : 0;
					int topB = m_noise.Noise2D(tx + offset + 0.5f, ty + 1.0f) > 0.0f ? 4 : 0;
					int leftB = m_noise.Noise2D(tx + offset, ty + 0.5f) > 0.0f ? 8 : 0;
					noiseL = bottomB + rightB + topB + leftB;
				}
			}
			
			int[] noiseRs = new int[]{ 0, 0, 1, 0, 2, 0, 1, 0, 3, 3, 1, 3, 2, 2, 1, 0 };
			int noiseR = noiseRs[noiseL];

			int[] noiseLs = new int[]{ 0, 1, 1, 2, 1, 3, 2, 4, 1, 2, 3, 4, 2, 4, 4, 5 };
			noiseL = noiseLs[noiseL];
			
			m_upsampleMat.SetTexture(m_uniforms.noiseSampler, m_noiseTextures[noiseL]);
			m_upsampleMat.SetVector(m_uniforms.noiseUVLH, new Vector4(noiseR, (noiseR + 1) % 4, 0.0f, m_hsv ? 1.0f : 0.0f));


			if(m_hsv)
			{
				Vector4f col = m_noiseColor * rs / 255.0f;
				col.w *= 2.0f;
				m_upsampleMat.SetVector(m_uniforms.noiseColor, col);
			} 
			else 
			{
				Vector4f col = m_noiseColor * rs * 2.0f / 255.0f;
				col.w *= 2.0f;
				m_upsampleMat.SetVector(m_uniforms.noiseColor, col);
			}

			m_upsampleMat.SetVector(m_uniforms.noiseRootColor, m_rootNoiseColor);

			Graphics.Blit(null, gpuSlot.GetTexture(), m_upsampleMat);

			base.DoCreateTile(level, tx, ty, slot);

		}

		void CreateOrthoNoise()
		{
			int tileWidth = GetCache().GetStorage(0).GetTileSize();
			m_noiseTextures = new Texture2D[6];
			Color col = new Color();
			
			int[] layers = new int[]{0, 1, 3, 5, 7, 15};
			int rand = 1234567;
			
			for (int nl = 0; nl < 6; ++nl) 
			{
				int l = layers[nl];

				m_noiseTextures[nl] = new Texture2D(tileWidth, tileWidth, TextureFormat.ARGB32, false, true);

				// corners
				for (int j = 0; j < tileWidth; ++j) {
					for (int i = 0; i < tileWidth; ++i) {
						m_noiseTextures[nl].SetPixel(i,j, new Color(0.5f,0.5f,0.5f,0.5f));
					}
				}
				
				// bottom border
				Random.seed = (l & 1) == 0 ? 7654321 : 5647381;
				//Random.seed = 5647381;
				for (int v = 2; v < 4; ++v) {
					for (int h = 4; h < tileWidth - 4; ++h) {
						for (int c = 0; c < 4; ++c) {
							col[c] = Random.value;
						}

						m_noiseTextures[nl].SetPixel(h,v,col);
						m_noiseTextures[nl].SetPixel(tileWidth-1-h,3-v,col);
					}
				}
	
				// right border
				Random.seed = (l & 2) == 0 ? 7654321 : 5647381;
				//Random.seed = 5647381;
				for (int h = tileWidth - 3; h >= tileWidth - 4; --h) {
					for (int v = 4; v < tileWidth - 4; ++v) {
						for (int c = 0; c < 4; ++c) {
							col[c] = Random.value;
						}

						m_noiseTextures[nl].SetPixel(h,v,col);
						m_noiseTextures[nl].SetPixel(2*tileWidth-5-h,tileWidth-1-v,col);
					}
				}
				
				// top border
				Random.seed = (l & 4) == 0 ? 7654321 : 5647381;
				//Random.seed = 5647381;
				for (int v = tileWidth - 2; v < tileWidth; ++v) {
					for (int h = 4; h < tileWidth - 4; ++h) {
						for (int c = 0; c < 4; ++c) {
							col[c] = Random.value;
						}
						
						m_noiseTextures[nl].SetPixel(h,v,col);
						m_noiseTextures[nl].SetPixel(tileWidth-1-h,2*tileWidth-5-v,col);
					}
				}
				
				// left border
				Random.seed = (l & 8) == 0 ? 7654321 : 5647381;
				//Random.seed = 5647381;
				for (int h = 1; h >= 0; --h) {
					for (int v = 4; v < tileWidth - 4; ++v) {
						for (int c = 0; c < 4; ++c) {
							col[c] = Random.value;
						}
						
						m_noiseTextures[nl].SetPixel(h,v,col);
						m_noiseTextures[nl].SetPixel(3-h,tileWidth-1-v,col);
					}
				}
				
				// center
				Random.seed = rand;
				for (int v = 4; v < tileWidth - 4; ++v) {
					for (int h = 4; h < tileWidth - 4; ++h) {
						for (int c = 0; c < 4; ++c) {
							col[c] = Random.value;
						}

						m_noiseTextures[nl].SetPixel(h,v,col);
					}
				}
				
				//randomize for next texture
				rand = (rand * 1103515245 + 12345) & 0x7FFFFFFF;

				m_noiseTextures[nl].Apply();

			}
			
		}

	}

}



















