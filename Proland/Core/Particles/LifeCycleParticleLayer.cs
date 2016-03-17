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
    class LifeCycleParticleLayer : ParticleLayer, ISwappable<LifeCycleParticleLayer>
    {

        /**
        * Layer specific particle data for managing the lifecycle of %particles.
        */
        public struct LifeCycleParticle
            {
                /**
                * The birth date of this particle, in microseconds. This birth date
                * allows one to compute the age of the particle. If this age is
                * between 0 and fadeInDelay the particle is fading in. If this age is
                * between fadeInDelay and fadeInDelay + activeDelay, the particle
                * is active. Otherwise it is fading out. Note that we do not store the
                * particle's age directly to avoid updating it at each frame.
                */
                internal float birthDate;
            };

        /**
         * Creates a new LifeCycleParticleLayer.
         *
         * @param fadeInDelay the fade in delay of %particles, in microseconds.
         *      0 means that %particles are created directly in active state.
         * @param activeDelay the active delay of %particles, in microseconds.
         * @param fadeOutDelay the fade out delay of %particles, in microseconds.
         *      0 means that %particles are deleted when the become inactive.
         */
        public LifeCycleParticleLayer(float fadeInDelay, float activeDelay, float fadeOutDelay)
        {
            init(fadeInDelay, activeDelay, fadeOutDelay);
        }


        /**
         * Returns the fade in delay of %particles, in microseconds.
         * 0 means that %particles are created directly in active state.
         */
        public float getFadeInDelay()
        {
            return fadeInDelay;
        }

        /**
         * Sets the fade in delay of %particles, in microseconds.
         * 0 means that %particles are created directly in active state.
         *
         * @param delay the new fade in delay.
         */
        public void setFadeInDelay(float delay)
        {
            fadeInDelay = delay;
        }

        /**
         * Returns the active delay of %particles, in microseconds.
         */
        public float getActiveDelay()
        {
            return activeDelay;
        }

        /**
         * Sets the active delay of %particles, in microseconds.
         *
         * @param delay the new active delay.
         */
        public void setActiveDelay(float delay)
        {
            activeDelay = delay;
        }

        /**
         * Returns the fade out delay of %particles, in microseconds.
         * 0 means that %particles are deleted when they become inactive.
         */
        public float getFadeOutDelay()
        {
            return fadeOutDelay;
        }

        /**
         * Sets the fade out delay of %particles, in microseconds.
         * 0 means that %particles are deleted when they become inactive.
         *
         * @param delay the new fade out delay.
         */
        public void setFadeOutDelay(float delay)
        {
            fadeOutDelay = delay;
        }

        /**
         * Returns the lifecycle specific data of the given particle.
         *
         * @param p a particle.
         */
        public LifeCycleParticle getLifeCycle(ParticleStorage.Particle p)
        {
            return (LifeCycleParticle)getParticleData(p);
        }

        /**
         * Returns the birth date of the given particle.
         *
         * @param p a particle.
         * @return the birth date of the given particle, in microseconds.
         */
        public float getBirthDate(ParticleStorage.Particle p)
        {
            return getLifeCycle(p).birthDate;
        }

        /**
         * Returns true if the given particle is fading in.
         */
        public bool isFadingIn(ParticleStorage.Particle p)
        {
            float age = time - getBirthDate(p);
            return age < fadeInDelay;
        }

        /**
         * Returns true if the given particle is active.
         */
        public bool isActive(ParticleStorage.Particle p)
        {
            float age = time - getBirthDate(p);
            return age >= fadeInDelay && age < fadeInDelay + activeDelay;
        }

        /**
         * Returns true if the given particle is fading out.
         */
        public bool isFadingOut(ParticleStorage.Particle p)
        {
            float age = time - getBirthDate(p);
            return age >= fadeInDelay + activeDelay;
        }

        /**
         * Forces the given particle to start fading out.
         */
        public void setFadingOut(ParticleStorage.Particle p)
        {
            // making sure that the intensity won't pop to 1.0 when deleting a fading in particle
            float i = getIntensity(p);
            if (!isFadingOut(p))
            {
                getLifeCycle(p).birthDate = time - (fadeInDelay + activeDelay + (1.0f - i) * fadeOutDelay);
            }
        }

        /**
         * Forces the given particle to be deleted immediatly.
         */
        public void killParticle(ParticleStorage.Particle p)
        {
            float minBirthDate = time - (fadeInDelay + activeDelay + fadeOutDelay);
            getLifeCycle(p).birthDate = minBirthDate - 1.0f;
        }

        /**
         * Returns an intensity for the given particle, based on its current
         * state. This intensity varies between 0 to 1 during fade in, stays equal
         * to 1 when the particle is active, and varies from 1 to 0 during fade out.
         */
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

        /**
         * Updates the current time. We don't need to update the %particles
         * because we store their birth date instead of their age.
         */
        public virtual void moveParticles(float dt)
        {
            time += dt;ojear
        }

        /**
         * Deletes the %particles that have completely faded out.
         */
        public virtual void removeOldParticles()
        {
            // all particles with a birth date less than minBirthDate must be deleted
            float minBirthDate = time - (fadeInDelay + activeDelay + fadeOutDelay);

            ParticleStorage s = getOwner().getStorage();
            //vector<ParticleStorage::Particle*>::iterator i = s->getParticles();
            //vector<ParticleStorage::Particle*>::iterator end = s->end();
            foreach (ParticleStorage.Particle i in s)
            {
                ParticleStorage.Particle p = i;
                if (getBirthDate(p) <= minBirthDate)
                {
                    s.deleteParticle(p);
                }
            }
        }

        /**
         * Creates an uninitialized LifeCycleParticleLayer.
         */
        protected LifeCycleParticleLayer() :
        {

        }

        /**
         * Initializes this LifeCycleParticleLayer. See #LifeCycleParticleLayer.
         */
        internal void init(float fadeInDelay, float activeDelay, float fadeOutDelay)
        {
            this.fadeInDelay = fadeInDelay;
            this.activeDelay = activeDelay;
            this.fadeOutDelay = fadeOutDelay;
            this.time = 0.0f;
        }

        /**
         * Initializes the birth date of the given particle to #time.
         */
        protected virtual void initParticle(ParticleStorage.Particle p)
        {
            getLifeCycle(p).birthDate = time;
        }

        public virtual void swap(LifeCycleParticleLayer p)
        {
            ParticleLayer.Swap(p);
            Std.Swap(ref fadeInDelay, ref p.fadeInDelay);
            Std.Swap(ref fadeOutDelay, ref p.fadeOutDelay);
            Std.Swap(ref activeDelay, ref p.activeDelay);
            Std.Swap(ref time, ref p.time);
        }

        /**
         * The fade in delay of %particles, in microseconds. 0 means that
         * %particles are created directly in active state.
         */
        private float fadeInDelay;

        /**
         * The active delay of %particles, in microseconds.
         */
        private float activeDelay;

        /**
         * The fade out delay of %particles, in microseconds. 0 means that
         * %particles are deleted when the become inactive.
         */
        private float fadeOutDelay;

        /**
         * The current time, in microseconds. This time is updated in
         * #moveParticles and used to set the birth date of particles in
         * #initParticle.
         */
        private float time;
    }
    class LifeCycleParticleLayerResource : ResourceTemplate<LifeCycleParticleLayer>
    {
        public LifeCycleParticleLayerResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) : base(20,manager, name, desc)
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

        virtual bool prepareUpdate()
        {
            oldValue = null;
            newDesc = null;

            return true;
        }
    }
}
