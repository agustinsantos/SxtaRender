using Sxta.Math;
using System.Collections.Generic;
using System.Drawing;

namespace SxtaRender.Fonts
{
    /// <summary>
    /// A texture atlas is used to pack several small regions into a single texture. 
    /// 
    /// The actual implementation is based on the article by Jukka Jylänki : "A 
    /// Thousand Ways to Pack the Bin - A Practical Approach to Two-Dimensional 
    /// Rectangle Bin Packing", February 27, 2010. 
    /// More precisely, this is an implementation of the Skyline Bottom-Left
    /// algorithm based on C++ sources provided by Jukka Jylänki at: 
    /// http://clb.demon.fi/files/RectangleBinPack/ 
    /// </summary>
    public class TextureAtlas
    {

        /// <summary>
        /// Creates a new empty texture atlas.
        /// </summary>
        /// <param name="width">width of the atlas</param>
        /// <param name="height ">height of the atlas</param>
        public TextureAtlas(int width, int height)
        {

            // We want a one pixel border around the whole atlas to avoid any artefact when 
            // sampling texture 
            Vector3i node = new Vector3i(1, 1, width - 2);

            this.nodes = new List<Vector3i>();
            this.used = 0;
            this.width = width;
            this.height = height;

            this.nodes.Add(node);
        }

        /// <summary>
        /// Allocate a new region in the atlas.
        /// Returns the coordinates of the allocated region
        /// </summary>
        /// <param name="width">width of the region to allocate</param>
        /// <param name="height">height of the region to allocate</param>
        /// <returns></returns>
        public Rectangle GetNewRegion(int width, int height, int margin)
        {
            int y;
            Vector3i node, prev;
            width = width + 2 * margin;
            height = height + 2 * margin;
            Rectangle region = new Rectangle(0, 0, width, height);

            int best_height = int.MaxValue;
            int best_index = -1;
            int best_width = int.MaxValue;
            for (int i = 0; i < this.nodes.Count; ++i)
            {
                y = Fit(i, width, height);
                if (y >= 0)
                {
                    node = this.nodes[i];
                    if (((y + height) < best_height) ||
                        (((y + height) == best_height) && (node.Z < best_width)))
                    {
                        best_height = y + height;
                        best_index = i;
                        best_width = node.Z;
                        region.X = node.X;
                        region.Y = y;
                    }
                }
            }

            if (best_index == -1)
            {
                region.X = -1;
                region.Y = -1;
                region.Width = 0;
                region.Height = 0;
                return region;
            }

            node = new Vector3i();
            node.X = region.X;
            node.Y = region.Y + height;
            node.Z = width;
            this.nodes.Insert(best_index, node);

            for (int i = best_index + 1; i < this.nodes.Count; ++i)
            {
                node = this.nodes[i];
                prev = this.nodes[i - 1];

                if (node.X < (prev.X + prev.Z))
                {
                    int shrink = prev.X + prev.Z - node.X;
                    node.X += shrink;
                    node.Z -= shrink;
                    this.nodes[i] = node;
                    if (node.Z <= 0)
                    {
                        this.nodes.RemoveAt(i);
                        --i;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            Merge();
            this.used += width * height;
            return region;
        }
 
        /// <summary>
        /// Remove all allocated regions from the atlas.
        /// </summary>
        public void Clear()
        {
            Vector3i node = new Vector3i(1, 1, 1);

            this.nodes.Clear();
            this.used = 0;
            // We want a one pixel border around the whole atlas to avoid any artefact when
            // sampling texture
            node.Z = this.width - 2;

            this.nodes.Add(node);
        }

        private int Fit(int index, int width, int height)
        {
            int width_left;

            Vector3i node = this.nodes[index];
            int x = node.X;
            int y = node.Y;
            width_left = width;
            int i = index;

            if ((x + width) > (this.width - 1))
            {
                return -1;
            }
            y = node.Y;
            while (width_left > 0)
            {
                node = this.nodes[i];
                if (node.Y > y)
                {
                    y = node.Y;
                }
                if ((y + height) > (this.height - 1))
                {
                    return -1;
                }
                width_left -= node.Z;
                ++i;
            }
            return y;
        }

        public void Merge()
        {
            Vector3i node, next;
            for (int i = 0; i < this.nodes.Count - 1; ++i)
            {
                node = this.nodes[i];
                next = this.nodes[i + 1];
                if (node.Y == next.Y)
                {
                    node.Z += next.Z;
                    this.nodes[i] = node;
                    this.nodes.RemoveAt(i + 1);
                    --i;
                }
            }
        }

        /// <summary>
        /// Allocated nodes
        /// </summary>
        private List<Vector3i> nodes;

        /// <summary>
        /// Width (in pixels) of the underlying texture
        /// </summary>
        public int width;

        /// <summary>
        /// Height (in pixels) of the underlying texture
        /// </summary>
        public int height;

        /// <summary>
        /// Allocated surface size 
        /// </summary>
        private int used;
   }
}
