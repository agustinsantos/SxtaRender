using Sxta.Core;
using Sxta.Math;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace proland
{
    public class RandomParticleLayer : ParticleLayer, ISwappable<RandomParticleLayer>
    {

        /// <summary>
        /// Layer specific particle data for managing random %particles.
        /// </summary>
        public struct RandomParticle
        {
            /// <summary>
            /// The current particle random position.
            /// </summary>
            //TOSEE
            public Vector3f randomPos;
        };

        /// <summary>
        /// Creates a new RandomParticleLayer.
        ///
        /// @param bounds the bounds of the random positions.
        /// </summary>
        public RandomParticleLayer(Box3f bounds) : base("RandomParticleLayer", sizeof(RandomParticle))
        {
            init(bounds);
        }

        /// <summary>
        /// Returns the random specific data of the given particle.
        ///
        /// @param p a particle.
        /// </summary>
        public RandomParticle getRandomParticle(ParticleStorage.Particle p)
        {
            return (RandomParticle)getParticleData(p);
        }

        public virtual void addNewParticles()
        {

        }


        /// <summary>
        /// Creates an uninitialized RandomParticleLayer.
        /// </summary>
        protected RandomParticleLayer() : base("RandomParticleLayer", sizeof(RandomParticle))
        {
            init(bounds);
        }

        /// <summary>
        /// Initializes this RandomParticleLayer. See #RandomParticleLayer.
        /// </summary>
        protected void init(Box3f bounds)
        {
            this.bounds = bounds;
        }

        /// <summary>
        /// Initializes the random position of the given particle.
        /// </summary>
        protected virtual void initParticle(ParticleStorage.Particle p)
        {
            RandomParticle r = getRandomParticle(p);
            r.randomPos.X = bounds.xmin + (bounds.xmax - bounds.xmin) * (rand() / float(RAND_MAX));
            r.randomPos.Y = bounds.ymin + (bounds.ymax - bounds.ymin) * (rand() / float(RAND_MAX));
            r.randomPos.Z = bounds.zmin + (bounds.zmax - bounds.zmin) * (rand() / float(RAND_MAX));
        }

        public void swap(RandomParticleLayer p)
        {
            ParticleLayer.swap(p);
            Std.Swap(ref bounds, ref p.bounds);
        }


        /// <summary>
        /// The bounds of the random positions.
        /// </summary>
        private Box3f bounds;
    }
    class RandomParticleLayerResource : ResourceTemplate<RandomParticleLayer>
    {
        public RandomParticleLayerResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
                base(50, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,xmin,xmax,ymin,ymax,zmin,zmax,");

            Box3f bounds = new Box3f(0f, 1f, 0f, 1f, 0f, 1f);

            if (e.GetAttribute("xmin") != null)
            {
                getFloatParameter(desc, e, "xmin", &(bounds.xmin));
            }
            if (e.GetAttribute("xmax") != null)
            {
                getFloatParameter(desc, e, "xmax", &(bounds.xmax));
            }
            if (e.GetAttribute("ymin") != null)
            {
                getFloatParameter(desc, e, "ymin", &(bounds.ymin));
            }
            if (e.GetAttribute("ymax") != null)
            {
                getFloatParameter(desc, e, "ymax", &(bounds.ymax));
            }
            if (e.GetAttribute("zmin") != null)
            {
                getFloatParameter(desc, e, "zmin", &(bounds.zmin));
            }
            if (e.GetAttribute("zmax") != null)
            {
                getFloatParameter(desc, e, "zmax", &(bounds.zmax));
            }

            valueC.init(bounds);
        }

        public virtual bool prepareUpdate()
        {
            oldValue = null;
            newDesc = null;

            return true;
        }
    };
#if TODO
extern const char randomParticleLayer[] = "randomParticleLayer";

    static ResourceFactory::Type<randomParticleLayer, RandomParticleLayerResource> RandomParticleLayerType;
#endif
}
