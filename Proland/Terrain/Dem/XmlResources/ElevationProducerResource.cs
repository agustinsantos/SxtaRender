using proland;
using Sxta.Proland.Terrain;
using Sxta.Render;
using Sxta.Render.Resources;
using Sxta.Render.Resources.XmlResources;
using System;
using System.Xml;


namespace Sxta.Proland.Core.Producer.XmlResources
{
    public class ElevationProducerResource : ResourceTemplate<ElevationProducer>
    {
        public static ElevationProducerResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new ElevationProducerResource(manager, name, desc, e);
        }
        public ElevationProducerResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
                base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,cache,residuals,face,upsampleProg,blendProg,gridSize,noise,flip,");
            valueC = new ElevationProducer();
            valueC.init(manager, this, name, desc, e);
        }

        public override bool prepareUpdate()
        {
#if TODO

                if (dynamic_cast<Resource*>(upsample.get())->changed()) {
                    invalidateTiles();
                } else if (blend != NULL && dynamic_cast<Resource*>(blend.get())->changed()) {
                    invalidateTiles();
                }
                return ResourceTemplate<40, ElevationProducer>::prepareUpdate();


            if ((Resource)(valueC.upsample).changed())
            {
                valueC.invalidateTiles();
            }
            else if (valueC.blend != null && ((Resource)(valueC.blend)).get.changed())
            {
                valueC.invalidateTiles();
            }
            return new ResourceTemplate <ElevationProducer>(40,null,null,null).prepareUpdate();
#endif
            throw new NotImplementedException();

        }
    }
}