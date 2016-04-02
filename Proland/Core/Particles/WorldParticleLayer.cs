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
    public class WorldParticleLayer : ParticleLayer, ISwappable<WorldParticleLayer>
    {
        double UNINITIALIZED = -1e9;
        /// <summary>
        /// Layer specific particle data for managing %particles in world space.
        /// </summary>
        public struct WorldParticle
        {
            /// <summary>
            /// The current particle position in world space.
            /// </summary>
            public Vector3d worldPos;

            /// <summary>
            /// The current particle velocity in world space.
            /// </summary>
            public Vector3d worldVelocity;
        };

        /// <summary>
        /// Creates a new WorldParticleLayer.
        ///
        /// @param speedFactor a global scaling factor to be applied to all particle
        ///      velocities.
        /// </summary>
        public WorldParticleLayer(float speedFactor) : base("WorldParticleLayer", sizeof(WorldParticle))
        {

        }

        /// <summary>
        /// Creates an uninitialized WorldParticleLayer.
        /// </summary>
        protected WorldParticleLayer() : base("WorldParticleLayer", sizeof(WorldParticle))
        {

        }

        /// <summary>
        /// Returns the global scaling factor to be applied to all particle
        /// velocities.
        /// </summary>
        public float getSpeedFactor()
        {
            return speedFactor;
        }

        /// <summary>
        /// Sets the global scaling factor to be applied to all particle
        /// velocities.
        /// </summary>
        public void setSpeedFactor(float speedFactor)
        {
            this.speedFactor = speedFactor;
        }

        /// <summary>
        /// Returns true if this WorldParticleLayer is in paused state.
        /// </summary>
        public bool isPaused()
        {
            return paused;
        }

        /// <summary>
        /// Sets the paused state of this WorldParticleLayer.
        ///
        /// @param paused if true, particles position won't be updated.
        /// </summary>
        public void setPaused(bool paused)
        {
            this.paused = paused;
        }

        /// <summary>
        /// Returns the world space specific data of the given particle.
        ///
        /// @param p a particle.
        /// </summary>
        public WorldParticle getWorldParticle(ParticleStorage.Particle p)
        {
            return (WorldParticle)getParticleData(p);
        }

        /// <summary>
        /// Moves the %particles based on their velocity. The velocities are not
        /// updated. This should be done by another layer.
        /// </summary>
        public virtual void moveParticles(double dt)
        {
            if (paused)
            {
                return;
            }
            float DT = (float)(dt * speedFactor * 1e-6);
            ParticleStorage s = getOwner().getStorage();
            //List<ParticleStorage.Particle>.iterator i = s.getParticles();
            //List<ParticleStorage.Particle>.iterator end = s.end();
            
            foreach (ParticleStorage.Particle i in s.getParticles())
            {
                WorldParticle w = getWorldParticle(i);
                if (w.worldPos.X != UNINITIALIZED && w.worldPos.Y != UNINITIALIZED && w.worldPos.Z != UNINITIALIZED && w.worldVelocity.X != UNINITIALIZED && w.worldVelocity.Y != UNINITIALIZED && w.worldVelocity.Z != UNINITIALIZED)
                {
                    w.worldPos += w.worldVelocity * DT;
                }
            }
        }

        /// <summary>
        /// Initializes this WorldParticleLayer. See #WorldParticleLayer.
        /// </summary>
        protected void init(float speedFactor)
        {

        }

        /// <summary>
        /// Initializes the world position and velocity of the given particle.
        /// </summary>
        protected virtual void initParticle(ParticleStorage.Particle p)
        {
            WorldParticle w = getWorldParticle(p);
            w.worldPos = new Vector3d(UNINITIALIZED, UNINITIALIZED, UNINITIALIZED);
            w.worldVelocity = new Vector3d(UNINITIALIZED, UNINITIALIZED, UNINITIALIZED);
        }

        public virtual void swap(WorldParticleLayer p)
        {
            ParticleLayer.swap(p);
            Std.Swap(ref speedFactor, ref p.speedFactor);
        }


        /// <summary>
        /// Global scaling factor to be applied to all particle velocities.
        /// </summary>
        private float speedFactor;

        /// <summary>
        /// If true, particles position won't be updated.
        /// </summary>
        private bool paused;
    }
    class WorldParticleLayerResource : ResourceTemplate<WorldParticleLayer>
    {
        public WorldParticleLayerResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
                base(50, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,speedFactor,");

            float speedFactor = 1.0f;

            if (e.GetAttribute("speedFactor") != null)
            {
                getFloatParameter(desc, e, "speedFactor", out speedFactor);
            }

            valueC.init((int)speedFactor);
        }

        public virtual bool prepareUpdate()
        {
            oldValue = null;
            newDesc = null;

            return true;
        }
    };
#if TODO
extern const char worldParticleLayer[] = "worldParticleLayer";

static ResourceFactory::Type<worldParticleLayer, WorldParticleLayerResource> WorldParticleLayerType;
#endif
}
