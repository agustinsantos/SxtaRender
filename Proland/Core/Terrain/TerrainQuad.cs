using Sxta.Math;
using Sxta.Render.Scenegraph;
using System;

namespace proland
{
    public class TerrainQuad : TerrainNode
    {

        /// <summary>
        /// The parent quad of this quad.
        /// </summary>
        public TerrainQuad parent;

        /// <summary>
        /// The level of this quad in the quadtree (0 for the root).
        /// </summary>
        public int level;

        /// <summary>
        /// The logical x coordinate of this quad (between 0 and 2^level).
        /// </summary>
        public int tx;

        /// <summary>
        /// The logical y coordinate of this quad (between 0 and 2^level).
        /// </summary>
        public int ty;

        /// <summary>
        /// The physical x coordinate of the lower left corner of this quad
        /// (in local space).
        /// </summary>
        public double ox;

        /// <summary>
        /// The physical y coordinate of the lower left corner of this quad
        /// (in local space).
        /// </summary>
        public double oy;

        /// <summary>
        /// The physical size of this quad (in local space).
        /// </summary>
        public double l;

        /// <summary>
        /// The minimum %terrain elevation inside this quad. This field must
        /// be updated manually by users(the TileSamplerZ class can
        /// do this for you).
        /// </summary>
        public float zmin;

        /// <summary>
        /// The maximum %terrain elevation inside this quad. This field must
        /// be updated manually by users(the TileSamplerZ class can
        /// do this for you).
        /// </summary>
        public float zmax;

        /// <summary>
        /// The four subquads of this quad. If this quad is not subdivided,
        /// the four values are NULL.The subquads are stored in the
        /// following order: bottomleft, bottomright, topleft, topright.
        /// </summary>
        public TerrainQuad[] children = new TerrainQuad[4];

        /// <summary>
        /// The visibility of the bounding box of this quad from the current
        /// viewer position.The bounding box is computed using #zmin and
        /// #zmax, which must therefore be up to date to get a correct culling
        /// of quads out of the view frustum.This visibility only takes frustum
        /// culling into account.
        /// </summary>
        public SceneManager.visibility visible;

        /// <summary>
        /// True if the bounding box of this quad is occluded by the bounding
        /// boxes of the quads in front of it.
        /// </summary>
        public bool occluded;

        /// <summary>
        /// True if the quad is invisible, or if all its associated tiles are
        /// produced and available in cache(this may not be the case if the
        /// asynchronous mode is used in a TileSampler).
        /// </summary>
        public bool drawable;

        /// <summary>
        /// Creates a new TerrainQuad.
        /// </summary>
        /// <param name="owner">the TerrainNode to which the %terrain quadtree belongs.</param>
        /// <param name="parent">the parent quad of this quad.</param>
        /// <param name="tx">the logical x coordinate of this quad.</param>
        /// <param name="ty">the logical y coordinate of this quad.</param>
        /// <param name="ox">the physical x coordinate of the lower left corner of this quad.</param>
        /// <param name="oy">the physical y coordinate of the lower left corner of this quad.</param>
        /// <param name="l">the physical size of this quad.</param>
        /// <param name="zmin">the minimum %terrain elevation inside this quad.</param>
        /// <param name="zmax">the maximum %terrain elevation inside this quad.</param>
        public TerrainQuad(TerrainNode owner, TerrainQuad parent, int tx, int ty, double ox, double oy, double l, float zmin, float zmax)
            : base()
        {
            this.parent = parent;
            this.level = parent == null ? 0 : parent.level + 1;
            this.tx = tx;
            this.ty = ty;
            this.ox = ox;
            this.oy = oy;
            this.l = l;
            this.zmin = zmin;
            this.zmax = zmax;
            this.occluded = false;
            this.drawable = true;
            this.owner = owner;            
        }

        /// <summary>
        /// Returns the TerrainNode to which the %terrain quadtree belongs.
        /// </summary>
        /// <returns></returns>
        public TerrainNode getOwner()
        {
            return owner;
        }

        /// <summary>
        /// Returns true if this quad is not subdivided.
        /// </summary>
        /// <returns></returns>
        public bool isLeaf()
        {
            return children[0] == null;
        }

        /// <summary>
        /// Returns the number of quads in the tree below this quad.
        /// </summary>
        /// <returns></returns>
        public int getSize()
        {
            int s = 1;
            if (isLeaf())
            {
                return s;
            }
            else
            {
                return s + children[0].getSize() + children[1].getSize() +
                    children[2].getSize() + children[3].getSize();
            }
        }

        /// <summary>
        /// Returns the depth of the tree below this quad.
        /// </summary>
        /// <returns></returns>
        public int getDepth()
        {
            if (isLeaf())
            {
                return level;
            }
            else
            {
                return Math.Max(Math.Max(children[0].getDepth(), children[1].getDepth()),
                    Math.Max(children[2].getDepth(), children[3].getDepth()));
            }
        }

        /// <summary>
        /// Subdivides or unsubdivides this quad based on the current
        /// viewer distance to this quad, relatively to its size. This
        /// method uses the current viewer position provided by the
        /// TerrainNode to which this quadtree belongs.
        /// </summary>
        public void update()
        {
            SceneManager.visibility v = parent == null ? SceneManager.visibility.PARTIALLY_VISIBLE : parent.visible;
            if (v == SceneManager.visibility.PARTIALLY_VISIBLE)
            {
                Box3d localBox = new Box3d(ox, ox + l, oy, oy + l, zmin, zmax);
                visible = owner.getVisibility(localBox);
            }
            else
            {
                visible = v;
            }

            // here we reuse the occlusion test from the previous frame:
            // if the quad was found unoccluded in the previous frame, we suppose it is
            // still unoccluded at this frame. If it was found occluded, we perform
            // an occlusion test to check if it is still occluded.
            if (visible != SceneManager.visibility.INVISIBLE && occluded)
            {
                occluded = owner.isOccluded(new Box3d(ox, ox + l, oy, oy + l, zmin, zmax));
                if (occluded)
                {
                    visible = SceneManager.visibility.INVISIBLE;
                }
            }
            double ground = TerrainNode.groundHeightAtCamera;
            float dist = owner.getCameraDist(new Box3d(ox, ox + l, oy, oy + l, Math.Min(0.0, ground), Math.Max(0.0, ground)));
            //Console.WriteLine(dist);
            if ((owner.splitInvisibleQuads || visible != SceneManager.visibility.INVISIBLE) && dist < l * owner.getSplitDistance() && level < owner.maxLevel)
            {
                if (isLeaf())
                {
                    subdivide();
                }

                int[] order = new int[4];
                double ox = owner.getLocalCamera().X;
                double oy = owner.getLocalCamera().Y;
                double cx = this.ox + l / 2.0;
                double cy = this.oy + l / 2.0;
                if (oy < cy)
                {
                    if (ox < cx)
                    {
                        order[0] = 0;
                        order[1] = 1;
                        order[2] = 2;
                        order[3] = 3;
                    }
                    else
                    {
                        order[0] = 1;
                        order[1] = 0;
                        order[2] = 3;
                        order[3] = 2;
                    }
                }
                else
                {
                    if (ox < cx)
                    {
                        order[0] = 2;
                        order[1] = 0;
                        order[2] = 3;
                        order[3] = 1;
                    }
                    else
                    {
                        order[0] = 3;
                        order[1] = 1;
                        order[2] = 2;
                        order[3] = 0;
                    }
                }

                children[order[0]].update();
                children[order[1]].update();
                children[order[2]].update();
                children[order[3]].update();

                // we compute a more precise occlusion for the next frame (see above),
                // by combining the occlusion status of the child nodes
                occluded = children[0].occluded && children[1].occluded && children[2].occluded && children[3].occluded;
            }
            else
            {
                if (visible != SceneManager.visibility.INVISIBLE)
                {
                    // we add the bounding box of this quad to the occluders list
                    occluded = owner.addOccluder(new Box3d(ox, ox + l, oy, oy + l, zmin, zmax));
                    if (occluded)
                    {
                        visible = SceneManager.visibility.INVISIBLE;
                    }
                }
                if (!isLeaf())
                {
                    children[0] = null;
                    children[1] = null;
                    children[2] = null;
                    children[3] = null;
                }
            }
        }

        /// <summary>
        /// The TerrainNode to which this %terrain quadtree belongs.
        /// </summary>
        internal TerrainNode owner;

        /// <summary>
        /// Creates the four subquads of this quad.
        /// </summary>
        private void subdivide()
        {
            float hl = (float)l / 2.0f;
            children[0] = new TerrainQuad(owner, this, 2 * tx, 2 * ty, ox, oy, hl, zmin, zmax);
            children[1] = new TerrainQuad(owner, this, 2 * tx + 1, 2 * ty, ox + hl, oy, hl, zmin, zmax);
            children[2] = new TerrainQuad(owner, this, 2 * tx, 2 * ty + 1, ox, oy + hl, hl, zmin, zmax);
            children[3] = new TerrainQuad(owner, this, 2 * tx + 1, 2 * ty + 1, ox + hl, oy + hl, hl, zmin, zmax);
        }
        
    }
}
