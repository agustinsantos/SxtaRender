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

using System.Collections;

namespace Sxta.Proland.Forest
{
	/**
	* Allows fast-computing of the available area around a point, using
	* angles. Acquired from Qizhi Yu's implementation of his thesis, itself
	* acquired from Daniel Dunbar & Gred Humphreys in "A Spatial Data structure
	* for fast poisson disk sample generation".
	*/
    public class RangeList
    {
        public class RangeEntry
        {
			public float min = 0.0f;
			public float max = 0.0f;

			public RangeEntry() {}

			public RangeEntry(RangeEntry re)
			{
				min = re.min;
				max = re.max;
			}
        }

        static readonly float TWO_PI = (float)System.Math.PI*2.0f;

        static readonly float K_SMALLEST_RANGE = 0.000001f;

        //List of entries corresponding to neighboring objects.
        RangeEntry[] ranges;

        //Number of entries.
        int numRanges = 0;
        
        //Size of ranges.
        int rangesSize = 8;

        /// <summary>
        /// Creates a new RangleList.
        /// </summary>
        public RangeList()
        {
            ranges = new RangeEntry[rangesSize];

			for(int i = 0; i < rangesSize; i++)
				ranges[i] = new RangeEntry();
        }

        /// <summary>
        /// Returns the number of ranges in this list.
        /// </summary>
        /// <returns></returns>
		public int GetRangeCount() {
            return numRanges;
        }

        /// <summary>
        /// Returns the range entry at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
		public RangeEntry GetRange(int index) {
            return ranges[index];
        }

        /// <summary>
        /// Removes an entry.
        /// </summary>
        /// <param name="pos">the index of the entry to remove.</param>
		public void DeleteRange(int pos)
        {
            if (pos < numRanges - 1) 
			{
				RangeEntry[] source = new RangeEntry[numRanges - (pos+1)];

				for(int i = 0; i < source.Length; i++)
					source[i] = new RangeEntry(ranges[pos+1+i]);

				for(int i = 0; i < source.Length; i++) {
					ranges[pos+i] = source[i];
				}
            }
            numRanges--;
        }

        /**
         * Adds an entry at a given position.
         *
         * param pos an index.
         * param min begining of the angle to remove.
         * param max end of the angle to remove.
         */
		public void InsertRange(int pos, float min, float max)
        {
            if (numRanges == rangesSize) 
			{
                rangesSize++;
                RangeEntry[] tmp = new RangeEntry[rangesSize];
				System.Array.Copy(ranges, tmp, numRanges);
                ranges = tmp;
            }

            if (pos < numRanges) 
			{
				RangeEntry[] source = new RangeEntry[numRanges - pos];
				
				for(int i = 0; i < source.Length; i++)
					source[i] = new RangeEntry(ranges[pos+i]);
				
				for(int i = 0; i < source.Length; i++) {
					ranges[pos+1+i] = source[i];
				}
            }

            ranges[pos].min = min;
            ranges[pos].max = max;
            numRanges++;
        }

        /**
         * Resets the list of range entries.
         *
         * param min min angle to search from.
         * param max max angle to search from.
         */
        public void Reset(float min, float max)
        {
            numRanges = 1;
            ranges[0].min = min;
            ranges[0].max = max;
        }

        /**
         * Removes an area from the available neighboring.
         *
         * param min begining of the angle to remove.
         * param max end of the angle to remove.
         */
		public void Subtract(float min, float max)
        {
            if (min > TWO_PI) {
                Subtract(min - TWO_PI, max - TWO_PI);
            } 
	        else if (max < 0) {
                Subtract(min + TWO_PI, max + TWO_PI);
            } 
	        else if (min < 0) {
                Subtract(0, max);
                Subtract(min + TWO_PI, TWO_PI);
            } 
	        else if (max > TWO_PI) {
                Subtract(min, TWO_PI);
                Subtract(0, max - TWO_PI);
            } 
	        else if (numRanges > 0) 
            {

                int pos;
                if (min < ranges[0].min) {
                    pos = -1;
                } 
	            else 
	            {
                    int lo = 0;
                    int mid = 0;
                    int hi = numRanges;
                    while (lo < hi - 1) 
		            {
                        mid = (lo + hi) >> 1;
                        if (ranges[mid].min < min) {
                            lo = mid;
                        } 
		                else {
                            hi = mid;
                        }
                    }
                    pos = lo;
                }

                if (pos == -1) {
                    pos = 0;
                } 
	            else if (min < ranges[pos].max) 
	            {
                    float c = ranges[pos].min;
                    float d = ranges[pos].max; 

                    if (min - c < K_SMALLEST_RANGE) 
		            {
                        if (max < d) {
                            ranges[pos].min = max;
                        } 
		                else {
                            DeleteRange(pos);
                        }
                    } 
		            else 
		            {
                        ranges[pos].max = min;
                        if (max < d) {
                            InsertRange(pos + 1, max, d);
                        }
                        pos++;
                    }
                } 
	            else 
	            {
                    if (pos < numRanges - 1 && max > ranges[pos + 1].min) {
                        pos++;
                    } 
		            else {
                        return;
                    }
                }

                while (pos < numRanges && max >= (ranges[pos].min)) 
	            {
                    if (ranges[pos].max - max < K_SMALLEST_RANGE) {
                        DeleteRange(pos);
                    } 
		            else 
		            {
                        ranges[pos].min = max;
                        if (ranges[pos].max > max) {
                            break;
                        }
                    }
                }
            }
        }

    }

}
