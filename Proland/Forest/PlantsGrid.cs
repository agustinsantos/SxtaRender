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

namespace Sxta.Proland.Forest
{

	public class PlantsGrid
	{

		float radius;

		int maxParticlesPerCell;

		Vector2i gridSize;

		int[] cellSizes;

		Vector2f[,] cellContents;

	    public PlantsGrid(float radius, int maxParticlesPerCell)
	    {
	        this.radius = radius;
	        this.maxParticlesPerCell = maxParticlesPerCell;

			gridSize = new Vector2i();
	        gridSize.X = (int) (1.0f / radius);
	        gridSize.Y = (int) (1.0f / radius);
	        cellSizes = new int[gridSize.X * gridSize.Y];
	        cellContents = new Vector2f[gridSize.X * gridSize.Y, maxParticlesPerCell];

			for (int i = 0; i < gridSize.X * gridSize.Y; ++i) {
	            cellSizes[i] = 0;
	        }

	    }

		public Vector2i GetGridSize()
	 	{
	        return gridSize;
	    }

		public Vector2i GetCell(Vector2f p)
	    {
	        int i = (int) System.Math.Floor(p.X * gridSize.X);
	        int j = (int) System.Math.Floor(p.Y * gridSize.Y);
	        return new Vector2i(i, j);
	    }

		public int GetCellSize(Vector2i cell)
	    {
			cell.X = MathHelper.Clamp(cell.X, 0, gridSize.X-1);
			cell.Y = MathHelper.Clamp(cell.Y, 0, gridSize.Y-1);

	        return cellSizes[cell.X + cell.Y * gridSize.X];
	    }

		public Vector2f[] GetCellContent(Vector2i cell)
	    {
			cell.X = MathHelper.Clamp(cell.X, 0, gridSize.X-1);
			cell.Y = MathHelper.Clamp(cell.Y, 0, gridSize.Y-1);

			Vector2f[] contents = new Vector2f[maxParticlesPerCell];

			for(int i = 0; i < maxParticlesPerCell; i++)
				contents[i] = cellContents[cell.X + cell.Y * gridSize.X, i];

			 return contents;
	    }

	    public void AddParticle(Vector2f p)
	    {

			Vector2f r = new Vector2f(radius,radius);
	        Vector2i cmin = GetCell(p - r);
	        Vector2i cmax = GetCell(p + r);

	        cmin.X = System.Math.Max(0, cmin.X);
			cmin.Y = System.Math.Max(0, cmin.Y);
			cmax.X = System.Math.Min(gridSize.X - 1, cmax.X);
			cmax.Y = System.Math.Min(gridSize.Y - 1, cmax.Y);

	        for (int j = cmin.Y; j <= cmax.Y; ++j) 
			{
	            for (int i = cmin.X; i <= cmax.X; ++i) 
				{
	                int index = i + j * gridSize.X;
	                int size = cellSizes[index];
	                if (size < maxParticlesPerCell) 
					{
	                    cellSizes[index] = size + 1;
						cellContents[index, size] = p;
	                } 
	            }
	        }
	    }
	}
}






















