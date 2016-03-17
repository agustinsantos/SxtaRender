using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proland
{
    class ParticleLayer
    {

        /**
         * Creates a new ParticleLayer.
         *
         * @param type the layer's type.
         * @param particleSize the size in bytes of the layer specific data that
         *      must be stored for each particle.
         */
        public ParticleLayer(char type, int particleSize)
        {
            throw new NotImplementedException();
        }

        /**
         * Returns the ParticleProducer to which this ParticleLayer belongs.
         */
        public void ParticleProducer getOwner()
        {
            throw new NotImplementedException();
        }

        /**
         * Returns true if this layer is enabled. If it is not the case, its
         * #moveParticles, #removeOldParticles and #addNewParticles will not
         * be called by the ParticleProducer.
         */
        public bool isEnabled()
        {
            throw new NotImplementedException();
        }

        /**
         * Enables or disables this ParticleLayer. The #moveParticles,
         * #removeOldParticles and #addNewParticles methods are called only
         * on enabled layers.
         *
         * @param enable true to enable this layer, false otherwise.
         */
        public void setIsEnabled(bool enable)
        {
            throw new NotImplementedException();
        }

        /**
         * Returns the size in bytes of the layer specific data that must be
         * stored for each particle.
         */
        public virtual int getParticleSize()
        {
            throw new NotImplementedException();
        }

        /**
         * Returns a pointer to the layer specific data of the given particle.
         *
         * @param p a particle.
         * @return a pointer to the data that is specific to this layer for the
         *      given particle.
         */
        public void getParticleData(ParticleStorage.Particle p)
        {
            throw new NotImplementedException();
            //return (void*)(((unsigned char*) p) +offset);
        }

    }
}
