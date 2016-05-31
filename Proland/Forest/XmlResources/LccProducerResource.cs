using proland;
using Sxta.Render;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sxta.Proland.Forest.XmlResources
{
    public class LccProducerResource : ResourceTemplate<LccProducer>
    {
        public static LccProducerResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new LccProducerResource(manager, name, desc, e);
        }
        public LccProducerResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
        base(3, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,cache,density,plants,maxLevel,deform,");
            TileProducer delegateCb = manager.loadResource(getParameter(desc, e, "density")).get() as TileProducer;
            Plants plants = manager.loadResource(getParameter(desc, e, "plants")).get() as Plants;

            TileCache cache = delegateCb.getCache();
            int tileWidth = cache.getStorage().getTileSize();
            int channels = 4;//hasLayers() ? 4 : cache->getStorage().cast<GPUTileStorage>()->getTexture(0)->getComponents();

            string lccTex = "renderbuffer-" + tileWidth;
            switch (channels)
            {
                case 1:
                    lccTex += "-R8";
                    break;
                case 2:
                    lccTex += "-RG8";
                    break;
                case 3:
                    lccTex += "-RGB8";
                    break;
                default:
                    lccTex += "-RGBA8";
                    break;
            }
            Texture2D lcc = manager.loadResource(lccTex).get() as Texture2D;

            Program copy = new Program(new Module(330, copyLccShader));
            Program dots = manager.loadResource("globalsShaderGS;dots;").get() as Program;

            int maxLevel = -1;
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("maxLevel")))
            {
                getIntParameter(desc, e, "maxLevel", out maxLevel);
            }
            bool deform = e.GetAttribute("deform") != null && e.GetAttribute("deform") == "true";

            this.valueC = new LccProducer(delegateCb, plants, lcc, copy, dots, maxLevel, deform);
        }


        private const string copyLccShader = @"\
            uniform sampler2DArray sourceSampler;
            uniform vec4 sourceOSL;
            #ifdef _VERTEX_
            layout(location=0) in vec4 vertex;
            out vec3 stl;
            void main() {
                gl_Position = vec4(vertex.xy, 0.0, 1.0);
                stl = vec3((vertex.xy * 0.5 + vec2(0.5)) * sourceOSL.z + sourceOSL.xy, sourceOSL.w);
            }
            #endif
            #ifdef _FRAGMENT_
            in vec3 stl;
            layout(location=0) out vec4 data;
            void main() {
                data = vec4(textureLod(sourceSampler, stl, 0.0).rg, 0.0, 0.0);
            }
            #endif
        ";

    }
}
