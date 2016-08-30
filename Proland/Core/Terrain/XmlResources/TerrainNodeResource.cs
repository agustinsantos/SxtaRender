using proland;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sxta.Proland.Core.Terrain.XmlResources
{
    public class TerrainNodeResource : ResourceTemplate<TerrainNode>
    {
        public static TerrainNodeResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new TerrainNodeResource(manager, name, desc, e);
        }
        public TerrainNodeResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(0, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            float size;
            float zmin;
            float zmax;
            Deformation deform = null;
            float splitFactor;
            int maxLevel;
            checkParameters(desc, e, "name,size,zmin,zmax,deform,radius,splitFactor,horizonCulling,maxLevel,");
            this.valueC = new TerrainNode();
            getFloatParameter(desc, e, "size", out size);
            getFloatParameter(desc, e, "zmin", out zmin);
            getFloatParameter(desc, e, "zmax", out zmax);
            string deformAttr = e.GetAttribute("deform");
            if (deformAttr == "sphere")
            {
                deform = new SphericalDeformation(size);
            }
            if (deformAttr == "cylinder")
            {
                float radius;
                getFloatParameter(desc, e, "radius", out radius);
                deform = new CylindricalDeformation(radius);
            }
            if (deform == null)
            {
                deform = new Deformation();
            }
            getFloatParameter(desc, e, "splitFactor", out splitFactor);
            getIntParameter(desc, e, "maxLevel", out maxLevel);

            TerrainQuad root = new TerrainQuad(null, null, 0, 0, -size, -size, 2.0 * size, zmin, zmax);
            this.valueC.init(deform, root, splitFactor, maxLevel);

            string horizonCullingAttr = e.GetAttribute("horizonCulling");
            if (horizonCullingAttr != null && horizonCullingAttr == "false")
            {
                this.valueC.horizonCulling = false;
            }
        }
    } 
}
