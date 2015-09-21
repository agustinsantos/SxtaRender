using Sxta.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    /// <summary>
    /// Stores a box with four sized areas; content, padding, a border and margin. See
    /// http://www.w3.org/TR/REC-CSS2/box.html#box-dimensions for a diagram.
    /// </summary>
    public class Box
    {
        public enum Area
        {
            MARGIN = 0,
            BORDER = 1,
            PADDING = 2,
            CONTENT = 3,
        }

        public enum Edge
        {
            TOP = 0,
            RIGHT = 1,
            BOTTOM = 2,
            LEFT = 3,
        }

        /// Initialises a zero-sized box.
        public Box()
        {
            this.content = new Vector2f(0, 0);
            this.offset = new Vector2f(0, 0);
        }

        /// Initialises a box with a default content area and no padding, borders and margins.
        public Box(Vector2f content)
        {
            this.content = content;
            this.offset = new Vector2f(0, 0);
        }


        /// Returns the offset of this box. This will usually be (0, 0).
        /// @return The box's offset.
        public Vector2f GetOffset()
        {
            return this.offset;
        }

        /// Returns the top-left position of one of the box's areas, relative to the top-left of the border area. This
        /// means the position of the margin area is likely to be negative.
        /// @param area[in] The desired area.
        /// @return The position of the area.
        public Vector2f GetPosition(Area area = Area.CONTENT)
        {
            Vector2f area_position = new Vector2f(offset.X - area_edges[(int)Area.MARGIN, (int)Edge.LEFT], offset.Y - area_edges[(int)Area.MARGIN, (int)Edge.TOP]);
            for (int i = 0; i < (int)area; i++)
            {
                area_position.X += area_edges[i, (int)Edge.LEFT];
                area_position.Y += area_edges[i, (int)Edge.TOP];
            }

            return area_position;
        }

        /// Returns the size of one of the box's areas. This will include all inner areas.
        /// @param area[in] The desired area.
        /// @return The size of the requested area.
        public Vector2f GetSize(Area area = Area.CONTENT)
        {
            Vector2f area_size = new Vector2f(content);
            for (int i = (int)Area.PADDING; i >= (int)area; i--)
            {
                area_size.X += (area_edges[i, (int)Edge.LEFT] + area_edges[i, (int)Edge.RIGHT]);
                area_size.Y += (area_edges[i, (int)Edge.TOP] + area_edges[i, (int)Edge.BOTTOM]);
            }

            return area_size;
        }

        /// Sets the offset of the box, relative usually to the owning element. This should only be set for auxiliary
        /// boxes of an element.
        /// @param offset[in] The offset of the box from the primary box.
        public void SetOffset(Vector2f _offset)
        {
            this.offset = _offset;
        }


        /// Sets the size of the content area.
        /// @param content[in] The size of the new content area.
        public void SetContent(Vector2f _content)
        {
            this.content = _content;
        }

        /// Sets the size of one of the edges of one of the box's outer areas.
        /// @param area[in] The area to change.
        /// @param edge[in] The area edge to change.
        /// @param size[in] The new size of the area segment.
        public void SetEdge(Area area, Edge edge, float size)
        {
            area_edges[(int)area, (int)edge] = size;
        }

        /// Returns the size of one of the area edges.
        /// @param area[in] The desired area.
        /// @param edge[in] The desired edge.
        /// @return The size of the requested area edge.
        public float GetEdge(Area area, Edge edge)
        {
            return area_edges[(int)area, (int)edge];
        }

        /// Returns the cumulative size of one edge up to one of the box's areas.
        /// @param area[in] The area to measure up to (and including). So, MARGIN will return the width of the margin, and PADDING will be the sum of the margin, border and padding.
        /// @param edge[in] The desired edge.
        /// @return The cumulative size of the edge.
        public float GetCumulativeEdge(Area area, Edge edge)
        {
            float size = 0;
            int max_area = Math.Min((int)area, 2);
            for (int i = 0; i <= max_area; i++)
                size += area_edges[i, (int)edge];

            return size;
        }

        /// Compares the size of the content area and the other area edges.
        /// @return True if the boxes represent the same area.
        public static bool operator ==(Box lhs, Box rhs)
        {
            bool equals = true;
            for (int i = 0; i < NUM_AREAS; i++)
                for (int j = 0; j < NUM_EDGES; j++)
                    equals = equals && (lhs.area_edges[i, j] == rhs.area_edges[i, j]);
            return lhs.content == rhs.content && equals;
        }

        /// Compares the size of the content area and the other area edges.
        /// @return True if the boxes do not represent the same area.
        public static bool operator !=(Box lhs, Box rhs)
        {
            return !(lhs == rhs);
        }

        private const int NUM_AREAS = 3;		// ignores CONTENT
        private const int NUM_EDGES = 4;
        private Vector2f content;
        private float[,] area_edges = new float[NUM_AREAS, NUM_EDGES];
        private Vector2f offset;
    }
}
