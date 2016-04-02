using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sxta.Render;
using Sxta.Render.Scenegraph;
using Sxta.Math;
using Sxta.Core;
using System.Diagnostics;

namespace proland
{
    /**
     * @defgroup terrain terrain
     * Provides a framework to draw and update view-dependent, quadtree based terrains.
     * This framework provides classes to represent the %terrain quadtree, classes to
     * associate data produced by proland::TileProducer to the quads of this
     * quadtree, as well as classes to update and draw such terrains (which can be
     * deformed to get spherical or cylindrical terrains).
     * @ingroup proland
     */

    /**
     * A view dependent, quadtree based %terrain. This class provides access to the
     * %terrain quadtree, defines the %terrain deformation (can be used to get planet
     * sized terrains), and defines how the %terrain quadtree must be subdivided based
     * on the viewer position. This class does not give any direct or indirect access
     * to the %terrain data (elevations, normals, texture, etc). The %terrain data must
     * be managed by proland::TileProducer, and stored in
     * proland::TileStorage. The link between with the %terrain quadtree is
     * provided by the TileSampler class.
     * @ingroup terrain
     * @authors Eric Bruneton, Antoine Begault, Guillaume Piolat
     */
    public class TerrainNode : Object
    {
        int HORIZON_SIZE = 256;

        /// <summary>
        /// The deformation of this %terrain. In the %terrain <i>local</i> space the
        ///%terrain sea level surface is flat.In the %terrain<i> deformed</i> space
        /// the sea level surface can be spherical or cylindrical (or flat if the
        /// identity deformation is used).
        /// </summary>
        public Deformation deform;

        /// <summary>
        /// The root of the %terrain quadtree. This quadtree is subdivided based on the
        /// current viewer position by the #update method.
        /// </summary>
        public TerrainQuad root;

        /// <summary>
        /// Describes how the %terrain quadtree must be subdivided based on the viewer
        /// distance.For a field of view of 80 degrees, and a viewport width of 1024
        /// pixels, a quad of size L will be subdivided into subquads if the viewer
        /// distance is less than splitFactor* L.For a smaller field of view and/or
        /// a larger viewport, the quad will be subdivided at a larger distance, so
        /// that its size in pixels stays more or less the same.This number must be
        /// strictly larger than 1.
        /// </summary>
        public float splitFactor;

        /// <summary>
        /// True to subdivide invisible quads based on distance, like visible ones.
        /// The default value of this flag is false.
        /// </summary>
        public bool splitInvisibleQuads;

        /// <summary>
        /// True to perform horizon occlusion culling tests.
        /// </summary>
        public bool horizonCulling;

        /// <summary>
        /// The maximum level at which the %terrain quadtree must be subdivided (inclusive).
        /// The %terrain quadtree will never be subdivided beyond this level, even if the
        /// viewer comes very close to the %terrain.
        /// </summary>
        public int maxLevel;

        /// <summary>
        /// The %terrain elevation below the current viewer position. This field must be
        /// updated manually by users(the TileSamplerZ class can do this for you).
        /// It is used to compute the 3D distance between the viewer and a quad, to decide
        /// whether this quad must be subdivided or not.
        /// </summary>
        public static float groundHeightAtCamera = 0.0f;

        /// <summary>
        /// The value #groundHeightAtCamera will have at the next frame.
        /// </summary>
        public static float nextGroundHeightAtCamera = 0.0f;

        /// <summary>
        /// Creates a new TerrainNode.
        /// </summary>
        /// <param name="deform">Creates a new TerrainNode.</param>
        /// <param name="root">the root of the %terrain quadtree.</param>
        /// <param name="splitFactor">how the %terrain quadtree must be subdivided (see
        /// #splitFactor).</param>
        /// <param name="maxLevel">the maximum level at which the %terrain quadtree must be
        ///subdivided(inclusive).</param>
        public TerrainNode(Deformation deform, TerrainQuad root, float splitFactor, int maxLevel) : base()
        {
            init(deform, root, splitFactor, maxLevel);
        }
        /// <summary>
        /// Creates an uninitialized TerrainNode.
        /// </summary>
        public TerrainNode() : base()
        {
        }
        /// <summary>
        /// Returns the current viewer frustum planes in the deformed #terrain
        /// space(see #deform). These planes are updated by the #update method.
        /// </summary>
        /// <returns></returns>
        public Vector3d getDeformedCamera()
        {
            return deformedCameraPos;
        }

        /// <summary>
        /// Returns the current viewer position in the local %terrain space
        /// (see #deform). This position is updated by the #update method.
        /// </summary>
        public Vector4d[] getDeformedFrustumPlanes()
        {
            return deformedFrustumPlanes;
        }

        /// <summary>
        /// Returns the current viewer position in the local %terrain space
        /// (see #deform). This position is updated by the #update method.
        /// </summary>
        /// <returns></returns>
        public Vector3d getLocalCamera()
        {
            return localCameraPos;
        }

        /// <summary>
        /// Returns the distance between the current viewer position and the
        /// given bounding box.This distance is measured in the local %terrain
        /// space(with Deformation#getLocalDist), with altitudes divided by
        /// #getDistFactor() to take deformations into account.
        /// </summary>
        /// <returns></returns>
        public float getCameraDist(Box3d localBox)
        {
            return (float)Math.Max(Math.Abs(localCameraPos.Z - localBox.zmax) / distFactor,
                   Math.Max(Math.Min(Math.Abs(localCameraPos.X - localBox.xmin), Math.Abs(localCameraPos.X - localBox.xmax)),
                        Math.Min(Math.Abs(localCameraPos.Y - localBox.ymin), Math.Abs(localCameraPos.Y - localBox.ymax))));
        }

        /// <summary>
        /// Returns the visibility of the given bounding box from the current
        /// viewer position.This visibility is computed with
        /// Deformation#getVisbility.
        /// </summary>
        /// <returns></returns>
        public SceneManager.visibility getVisibility(Box3d localBox)
        {
            return deform.getVisibility(this, localBox);
        }

        /// <summary>
        /// Returns the viewer distance at which a quad is subdivided, relatively
        /// to the quad size.This relative distance is equal to #splitFactor for
        /// a field of view of 80 degrees and a viewport width of 1024 pixels.It
        /// is larger for smaller field of view angles and/or larger viewports.
        /// </summary>
        /// <returns></returns>
        public float getSplitDistance()
        {
            Debug.Assert(float.IsInfinity(splitDist) == false);
            Debug.Assert(splitDist > 1.0f);
            return splitDist;
        }

        /// <summary>
        /// Returns the ratio between local and deformed lengths at #getLocalCamera().
        /// </summary>
        /// <returns></returns>
        public float getDistFactor()
        {
            return distFactor;
        }

        /// <summary>
        /// Updates the %terrain quadtree based on the current viewer position.
        /// The viewer position relatively to the local and deformed %terrain
        /// spaces is computed based on the given SceneNode, which represents
        /// the %terrain position in the scene graph(which also contains the
        /// current viewer position).
        /// </summary>
        /// <param name="owner">the SceneNode representing the terrain position in
        /// the global scene graph.</param>
        public void update(SceneNode owner)
        {
            Matrix4d MatdeformedCameraPos = owner.getLocalToCamera();
            MatdeformedCameraPos.Invert();
            deformedCameraPos = MatdeformedCameraPos * Vector3d.Zero;
            SceneManager.getFrustumPlanes(owner.getLocalToScreen(), deformedFrustumPlanes);
            localCameraPos = deform.deformedToLocal(deformedCameraPos);

            Matrix4d m = deform.localToDeformedDifferential(localCameraPos, true);
            distFactor = Math.Max((float)new Vector3d(m.R0C0, m.R1C0, m.R2C0).Length, (float)new Vector3d(m.R0C1, m.R1C1, m.R2C1).Length);

            FrameBuffer fb = SceneManager.getCurrentFrameBuffer();
            Vector3d left = deformedFrustumPlanes[0].Xyz;
            left.Normalize();
            Vector3d right = deformedFrustumPlanes[1].Xyz;
            right.Normalize();
            //TOSEE Acos
            float fov = (float)Math.Acos(Vector3d.Dot(-left,right));
            splitDist = (float)(splitFactor * fb.getViewport().Z / 1024.0f * Math.Tan(40.0f / 180.0f * Math.PI) / Math.Tan(fov / 2.0f));
            if (splitDist < 1.1f || !(float.IsInfinity(splitDist) == false))
            {
                splitDist = 1.1f;
            }

            // initializes data structures for horizon occlusion culling
            if (horizonCulling && localCameraPos.Z <= root.zmax)
            {
                Matrix4d MatdeformedDir = owner.getLocalToCamera();
                MatdeformedDir.Invert();
                Vector3d deformedDir = MatdeformedDir * Vector3d.UnitZ;
                Vector2d localDir = (deform.deformedToLocal(deformedDir) - localCameraPos).Xy;
                localDir.Normalize();
                localCameraDir = new Matrix2f((float)localDir.Y, (float)-localDir.X, (float)-localDir.X, (float)-localDir.Y);
                for (int i = 0; i < HORIZON_SIZE; ++i)
                {
                    //TOSEE INFINITY
                    horizon[i] = (float)Double.NegativeInfinity;
                }
            }

            root.update();
        }

        /// <summary>
        /// Adds the given bounding box as an occluder. <i>The bounding boxes must
        /// be added in front to back order</i>.
        /// </summary>
        /// <param name="occluder">a bounding box in local (i.e. non deformed) coordinates.</param>
        /// <returns>true is the given bounding box is occluded by the bounding boxes
        /// previously added as occluders by this method.</returns>
        public bool addOccluder(Box3d occluder)
        {
            if (!horizonCulling || localCameraPos.Z > root.zmax)
            {
                return false;
            }
            Vector2f[] corners = new Vector2f[4];
            Vector2d o = localCameraPos.Xy;
            corners[0] = localCameraDir * (Vector2f)(new Vector2d(occluder.xmin, occluder.ymin) - o);
            corners[1] = localCameraDir * (Vector2f)(new Vector2d(occluder.xmin, occluder.ymax) - o);
            corners[2] = localCameraDir * (Vector2f)(new Vector2d(occluder.xmax, occluder.ymin) - o);
            corners[3] = localCameraDir * (Vector2f)(new Vector2d(occluder.xmax, occluder.ymax) - o);
            if (corners[0].Y <= 0.0f || corners[1].Y <= 0.0f || corners[2].Y <= 0.0f || corners[3].Y <= 0.0f)
            {
                // skips bounding boxes that are not fully behind the "near plane"
                // of the reference frame used for horizon occlusion culling
                return false;
            }
            float dzmin = (float)(occluder.zmin - localCameraPos.Z);
            float dzmax = (float)(occluder.zmax - localCameraPos.Z);
            Vector3f[] bounds = new Vector3f[4];
            bounds[0] = new Vector3f(corners[0].X, dzmin, dzmax) / corners[0].Y;
            bounds[1] = new Vector3f(corners[1].X, dzmin, dzmax) / corners[1].Y;
            bounds[2] = new Vector3f(corners[2].X, dzmin, dzmax) / corners[2].Y;
            bounds[3] = new Vector3f(corners[3].X, dzmin, dzmax) / corners[3].Y;
            float xmin = Math.Min(Math.Min(bounds[0].X, bounds[1].X), Math.Min(bounds[2].X, bounds[3].X)) * 0.33f + 0.5f;
            float xmax = Math.Max(Math.Max(bounds[0].X, bounds[1].X), Math.Max(bounds[2].X, bounds[3].X)) * 0.33f + 0.5f;
            float zmin = Math.Min(Math.Min(bounds[0].Y, bounds[1].Y), Math.Min(bounds[2].Y, bounds[3].Y));
            float zmax = Math.Max(Math.Max(bounds[0].Z, bounds[1].Z), Math.Max(bounds[2].Z, bounds[3].Z));

            int imin = Math.Max((int)(Math.Floor(xmin * HORIZON_SIZE)), 0);
            int imax = Math.Min((int)(Math.Ceiling(xmax * HORIZON_SIZE)), HORIZON_SIZE - 1);

            // first checks if the bounding box projection is below the current horizon line
            bool occluded = imax >= imin;
            for (int i = imin; i <= imax; ++i)
            {
                if (zmax > horizon[i])
                {
                    occluded = false;
                    break;
                }
            }
            if (!occluded)
            {
                // if it is not, updates the horizon line with the projection of this bounding box
                imin = Math.Max((int)(Math.Ceiling(xmin * HORIZON_SIZE)), 0);
                imax = Math.Min((int)(Math.Floor(xmax * HORIZON_SIZE)), HORIZON_SIZE - 1);
                for (int i = imin; i <= imax; ++i)
                {
                    horizon[i] = Math.Max(horizon[i], zmin);
                }
            }
            return occluded;
        }

        /// <summary>
        /// Returns true if the given bounding box is occluded by the bounding boxes
        /// previously added by #addOccluder().
        /// </summary>
        /// <param name="box">a bounding box in local (i.e. non deformed) coordinates.</param>
        /// <returns>true is the given bounding box is occluded by the bounding boxes
        /// previously added as occluders by #addOccluder.</returns>
        public bool isOccluded(Box3d box)
        {
            if (!horizonCulling || localCameraPos.Z > root.zmax)
            {
                return false;
            }
            Vector2f[] corners = new Vector2f[4];
            Vector2d o = localCameraPos.Xy;
            corners[0] = localCameraDir * (Vector2f)(new Vector2d(box.xmin, box.ymin) - o);
            corners[1] = localCameraDir * (Vector2f)(new Vector2d(box.xmin, box.ymax) - o);
            corners[2] = localCameraDir * (Vector2f)(new Vector2d(box.xmax, box.ymin) - o);
            corners[3] = localCameraDir * (Vector2f)(new Vector2d(box.xmax, box.ymax) - o);
            if (corners[0].Y <= 0.0f || corners[1].Y <= 0.0f || corners[2].Y <= 0.0f || corners[3].Y <= 0.0f)
            {
                return false;
            }
            float dz = (float)(box.zmax - localCameraPos.Z);
            corners[0] = new Vector2f(corners[0].X, dz) / corners[0].Y;
            corners[1] = new Vector2f(corners[1].X, dz) / corners[1].Y;
            corners[2] = new Vector2f(corners[2].X, dz) / corners[2].Y;
            corners[3] = new Vector2f(corners[3].X, dz) / corners[3].Y;
            float xmin = Math.Min(Math.Min(corners[0].X, corners[1].X), Math.Min(corners[2].X, corners[3].X)) * 0.33f + 0.5f;
            float xmax = Math.Max(Math.Max(corners[0].X, corners[1].X), Math.Max(corners[2].X, corners[3].X)) * 0.33f + 0.5f;
            float zmax = Math.Max(Math.Max(corners[0].Y, corners[1].Y), Math.Max(corners[2].Y, corners[3].Y));
            int imin = Math.Max((int)(Math.Floor(xmin * HORIZON_SIZE)), 0);
            int imax = Math.Min((int)(Math.Ceiling(xmax * HORIZON_SIZE)), HORIZON_SIZE - 1);
            for (int i = imin; i <= imax; ++i)
            {
                if (zmax > horizon[i])
                {
                    return false;
                }
            }
            return imax >= imin;
        }

        /**
         * Initializes this TerrainNode.
         *
         * @param deform the %terrain deformation.
         * @param root the root of the %terrain quadtree.
         * @param splitFactor how the %terrain quadtree must be subdivided (see
         *      #splitFactor).
         * @param maxLevel the maximum level at which the %terrain quadtree must be
         *      subdivided (inclusive).
         */
        protected void init(Deformation deform, TerrainQuad root, float splitFactor, int maxLevel)
        {
            this.deform = deform;
            this.root = root;
            this.splitFactor = splitFactor;
            this.splitInvisibleQuads = false;
            this.horizonCulling = true;
            this.splitDist = 1.1f;
            this.maxLevel = maxLevel;
            root.owner = this;
            horizon = new float[HORIZON_SIZE];
        }

        internal void swap(TerrainNode t)
        {
            Std.Swap(ref deform, ref t.deform);
            Std.Swap(ref root, ref t.root);
            Std.Swap(ref splitFactor, ref t.splitFactor);
            Std.Swap(ref maxLevel, ref t.maxLevel);
            Std.Swap(ref deformedCameraPos, ref t.deformedCameraPos);
            Std.Swap(ref localCameraPos, ref t.localCameraPos);
            Std.Swap(ref splitDist, ref t.splitDist);

            for (int i = 0; i < 6; ++i)
            {
                Std.Swap(ref deformedFrustumPlanes[i], ref t.deformedFrustumPlanes[i]);
            }
        }


        /**
         * The current viewer position in the deformed %terrain space (see #deform).
         */
        private Vector3d deformedCameraPos;

        /**
         * The current viewer frustum planes in the deformed #terrain space (see #deform).
         */
        private Vector4d[] deformedFrustumPlanes = new Vector4d[6];

        /**
         * The current viewer position in the local %terrain space (see #deform).
         */
        private Vector3d localCameraPos;

        /**
         * The viewer distance at which a quad is subdivided, relatively to the quad size.
         */
        private float splitDist;

        /**
         * The ratio between local and deformed lengths at #localCameraPos.
         */
        private float distFactor;

        /**
         * Local reference frame used to compute horizon occlusion culling.
         */
        private Matrix2f localCameraDir;

        /**
         * Rasterized horizon elevation angle for each azimuth angle.
         */
        private float[] horizon;
    }
#if TODO
    class TerrainNodeResource : ResourceTemplate<0, TerrainNode>
{
    public TerrainNodeResource(ptr<ResourceManager> manager, const string &name, ptr<ResourceDescriptor> desc, const TiXmlElement* e = NULL) :
        ResourceTemplate<0, TerrainNode>(manager, name, desc)
    {
        e = e == NULL? desc->descriptor : e;
        float size;
        float zmin;
        float zmax;
        ptr<Deformation> deform;
        float splitFactor;
        int maxLevel;
        checkParameters(desc, e, "name,size,zmin,zmax,deform,radius,splitFactor,horizonCulling,maxLevel,");
        getFloatParameter(desc, e, "size", &size);
        getFloatParameter(desc, e, "zmin", &zmin);
        getFloatParameter(desc, e, "zmax", &zmax);
        if (e->Attribute("deform") != NULL && strcmp(e->Attribute("deform"), "sphere") == 0) {
            deform = new SphericalDeformation(size);
}
        if (e->Attribute("deform") != NULL && strcmp(e->Attribute("deform"), "cylinder") == 0) {
            float radius;
            getFloatParameter(desc, e, "radius", &radius);
            deform = new CylindricalDeformation(radius);
        }
        if (deform == NULL) {
            deform = new Deformation();
        }
        getFloatParameter(desc, e, "splitFactor", &splitFactor);
        getIntParameter(desc, e, "maxLevel", &maxLevel);

        ptr<TerrainQuad> root = new TerrainQuad(NULL, NULL, 0, 0, -size, -size, 2.0 * size, zmin, zmax);
        init(deform, root, splitFactor, maxLevel);

        if (e->Attribute("horizonCulling") != NULL && strcmp(e->Attribute("horizonCulling"), "false") == 0) {
            horizonCulling = false;
        }
    }
};

extern const char terrainNode[] = "terrainNode";

static ResourceFactory::Type<terrainNode, TerrainNodeResource> TerrainNodeType;
#endif
}
