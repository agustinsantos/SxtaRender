using Sxta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sxta.Render.Resources;
using System.Xml;
using Sxta.Render.Resources.XmlResources;

namespace proland
{
    public class LifeCycleParticleLayer : ParticleLayer, ISwappable<LifeCycleParticleLayer>
    {

        /// <summary>
        /// Layer specific particle data for managing the lifecycle of %particles.
        /// </summary>
        public struct LifeCycleParticle
        {
            /// <summary>
            /// The birth date of this particle, in microseconds. This birth date
            /// allows one to compute the age of the particle. If this age is
            /// between 0 and fadeInDelay the particle is fading in. If this age is
            /// between fadeInDelay and fadeInDelay + activeDelay, the particle
            /// is active. Otherwise it is fading out. Note that we do not store the
            /// particle's age directly to avoid updating it at each frame.
            /// </summary>
            internal float birthDate;
        };

        /// <summary>
        /// Creates a new LifeCycleParticleLayer.
        ///
        /// @param fadeInDelay the fade in delay of %particles, in microseconds.
        ///      0 means that %particles are created directly in active state.
        /// @param activeDelay the active delay of %particles, in microseconds.
        /// @param fadeOutDelay the fade out delay of %particles, in microseconds.
        ///      0 means that %particles are deleted when the become inactive.
        /// </summary>
        public LifeCycleParticleLayer(float fadeInDelay, float activeDelay, float fadeOutDelay) : base("LifeCycleParticleLayer", sizeof(LifeCycleParticle))
        {
            init(fadeInDelay, activeDelay, fadeOutDelay);
        }
        /// <summary>
        /// Creates an uninitialized LifeCycleParticleLayer.
        /// </summary>
        protected LifeCycleParticleLayer() : base("LifeCycleParticleLayer", sizeof(LifeCycleParticle))
        {

        }

        /// <summary>
        /// Returns the fade in delay of %particles, in microseconds.
        /// 0 means that %particles are created directly in active state.
        /// </summary>
        public float getFadeInDelay()
        {
            return fadeInDelay;
        }

        /// <summary>
        /// Sets the fade in delay of %particles, in microseconds.
        /// 0 means that %particles are created directly in active state.
        ///
        /// @param delay the new fade in delay.
        /// </summary>
        public void setFadeInDelay(float delay)
        {
            fadeInDelay = delay;
        }

        /// <summary>
        /// Returns the active delay of %particles, in microseconds.
        /// </summary>
        public float getActiveDelay()
        {
            return activeDelay;
        }

        /// <summary>
        /// Sets the active delay of %particles, in microseconds.
        ///
        /// @param delay the new active delay.
        /// </summary>
        public void setActiveDelay(float delay)
        {
            activeDelay = delay;
        }

        /// <summary>
        /// Returns the fade out delay of %particles, in microseconds.
        /// 0 means that %particles are deleted when they become inactive.
        /// </summary>
        public float getFadeOutDelay()
        {
            return fadeOutDelay;
        }

        /// <summary>
        /// Sets the fade out delay of %particles, in microseconds.
        /// 0 means that %particles are deleted when they become inactive.
        ///
        /// @param delay the new fade out delay.
        /// </summary>
        public void setFadeOutDelay(float delay)
        {
            fadeOutDelay = delay;
        }

        /// <summary>
        /// Returns the lifecycle specific data of the given particle.
        ///
        /// @param p a particle.
        /// </summary>
        public LifeCycleParticle getLifeCycle(ParticleStorage.Particle p)
        {
            return (LifeCycleParticle)getParticleData(p);
        }

        /// <summary>
        /// Returns the birth date of the given particle.
        ///
        /// @param p a particle.
        /// @return the birth date of the given particle, in microseconds.
        /// </summary>
        public float getBirthDate(ParticleStorage.Particle p)
        {
            return getLifeCycle(p).birthDate;
        }

        /// <summary>
        /// Returns true if the given particle is fading in.
        /// </summary>
        public bool isFadingIn(ParticleStorage.Particle p)
        {
            float age = time - getBirthDate(p);
            return age < fadeInDelay;
        }

        /// <summary>
        /// Returns true if the given particle is active.
        /// </summary>
        public bool isActive(ParticleStorage.Particle p)
        {
            float age = time - getBirthDate(p);
            return age >= fadeInDelay && age < fadeInDelay + activeDelay;
        }

        /// <summary>
        /// Returns true if the given particle is fading out.
        /// </summary>
        public bool isFadingOut(ParticleStorage.Particle p)
        {
            float age = time - getBirthDate(p);
            return age >= fadeInDelay + activeDelay;
        }

        /// <summary>
        /// Forces the given particle to start fading out.
        /// </summary>
        public void setFadingOut(ParticleStorage.Particle p)
        {
            // making sure that the intensity won't pop to 1.0 when deleting a fading in particle
            float i = getIntensity(p);
            if (!isFadingOut(p))
            {
                getLifeCycle(p).birthDate = time - (fadeInDelay + activeDelay + (1.0f - i) * fadeOutDelay);
            }
        }

        /// <summary>
        /// Forces the given particle to be deleted immediatly.
        /// </summary>
        public void killParticle(ParticleStorage.Particle p)
        {
            float minBirthDate = time - (fadeInDelay + activeDelay + fadeOutDelay);
            getLifeCycle(p).birthDate = minBirthDate - 1.0f;
        }

        /// <summary>
        /// Returns an intensity for the given particle, based on its current
        /// state. This intensity varies between 0 to 1 during fade in, stays equal
        /// to 1 when the particle is active, and varies from 1 to 0 during fade out.
        /// </summary>
        public float getIntensity(ParticleStorage.Particle p)
        {
            float t = time - getBirthDate(p);
            if (t < fadeInDelay)
            {
                return t / fadeInDelay;
            }
            else
            {
                t -= fadeInDelay;
                if (t < activeDelay)
                {
                    return 1.0f;
                }
                else
                {
                    t -= activeDelay;
                    return Math.Max(0.0f, 1.0f - t / fadeOutDelay);
                }
            }
        }

        /// <summary>
        /// Updates the current time. We don't need to update the %particles
        /// because we store their birth date instead of their age.
        /// </summary>
        public virtual void moveParticles(double dt)
        {
            time += (float)dt; //TOSEE
        }

        /// <summary>
        /// Deletes the %particles that have completely faded out.
        /// </summary>
        public virtual void removeOldParticles()
        {
            // all particles with a birth date less than minBirthDate must be deleted
            float minBirthDate = time - (fadeInDelay + activeDelay + fadeOutDelay);

            ParticleStorage s = getOwner().getStorage();
            //vector<ParticleStorage::Particle*>::iterator i = s->getParticles();
            //vector<ParticleStorage::Particle*>::iterator end = s->end();
            foreach (ParticleStorage.Particle i in s.getParticles())
            {
                ParticleStorage.Particle p = i;
                if (getBirthDate(p) <= minBirthDate)
                {
                    s.deleteParticle(p);
                }
            }
        }


        /// <summary>
        /// Initializes this LifeCycleParticleLayer. See #LifeCycleParticleLayer.
        /// </summary>
        internal void init(float fadeInDelay, float activeDelay, float fadeOutDelay)
        {
            this.fadeInDelay = fadeInDelay;
            this.activeDelay = activeDelay;
            this.fadeOutDelay = fadeOutDelay;
            this.time = 0.0f;
        }

        /// <summary>
        /// Initializes the birth date of the given particle to #time.
        /// </summary>
        protected virtual void initParticle(ParticleStorage.Particle p)
        {
            getLifeCycle(p).birthDate = time;
        }

        public virtual void swap(LifeCycleParticleLayer p)
        {
            ParticleLayer.swap(p);
            Std.Swap(ref fadeInDelay, ref p.fadeInDelay);
            Std.Swap(ref fadeOutDelay, ref p.fadeOutDelay);
            Std.Swap(ref activeDelay, ref p.activeDelay);
            Std.Swap(ref time, ref p.time);
        }

        /// <summary>
        /// The fade in delay of %particles, in microseconds. 0 means that
        /// %particles are created directly in active state.
        /// </summary>
        private float fadeInDelay;

        /// <summary>
        /// The active delay of %particles, in microseconds.
        /// </summary>
        private float activeDelay;

        /// <summary>
        /// The fade out delay of %particles, in microseconds. 0 means that
        /// %particles are deleted when the become inactive.
        /// </summary>
        private float fadeOutDelay;

        /// <summary>
        /// The current time, in microseconds. This time is updated in
        /// #moveParticles and used to set the birth date of particles in
        /// #initParticle.
        /// </summary>
        private float time;
    }
    class LifeCycleParticleLayerResource : ResourceTemplate<LifeCycleParticleLayer>
    {
        public LifeCycleParticleLayerResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) : base(20, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,fadeInDelay,fadeOutDelay,activeDelay,unit,");

            float fadeInDelay = 5.0f;
            float fadeOutDelay = 5.0f;
            float activeDelay = 30.0f;

            float unit = 1000000.0f; // delays are converted into seconds.

            if (e.GetAttribute("unit") != null)
            {
                if (e.GetAttribute("unit") == "s")
                {
                    unit = 1000000.0f;
                }
                else if (e.GetAttribute("unit") == "ms")
                {
                    unit = 1000.0f;
                }
                else if (e.GetAttribute("unit") == "us")
                {
                    unit = 10.0f;
                }
            }

            //delays are taken in seconds
            getFloatParameter(desc, e, "fadeInDelay", out fadeInDelay);
            getFloatParameter(desc, e, "fadeOutDelay", out fadeOutDelay);
            getFloatParameter(desc, e, "activeDelay", out activeDelay);

            valueC.init(fadeInDelay * unit, activeDelay * unit, fadeOutDelay * unit);
        }

        public virtual bool prepareUpdate()
        {
            oldValue = null;
            newDesc = null;

            return true;
        }
    }
}
