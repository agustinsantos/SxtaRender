using Sxta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proland
{
    public class ParticleLayer
    {

        /// <summary>
        /// Creates a new ParticleLayer.
        ///
        /// @param type the layer's type.
        /// @param particleSize the size in bytes of the layer specific data that
        ///      must be stored for each particle.
        /// </summary>
        public ParticleLayer(string type, int particleSize)
        {
            init(particleSize);
        }


        /// <summary>
        /// Returns the ParticleProducer to which this ParticleLayer belongs.
        /// </summary>
        /// <returns></returns>
        public ParticleProducer getOwner()
        {
            return owner;
        }

        /// <summary>
        /// Returns true if this layer is enabled. If it is not the case, its
        /// #moveParticles, #removeOldParticles and #addNewParticles will not
        /// be called by the ParticleProducer.
        /// </summary>
        public bool isEnabled()
        {
            return enabled;
        }

        /// <summary>
        /// Enables or disables this ParticleLayer. The #moveParticles,
        /// #removeOldParticles and #addNewParticles methods are called only
        /// on enabled layers.
        /// </summary>
        /// <param name="enable">true to enable this layer, false otherwise.</param>
        public void setIsEnabled(bool enable)
        {
            this.enabled = enable;
        }

        /// <summary>
        /// Returns the size in bytes of the layer specific data that must be
        /// stored for each particle.
        /// </summary>
        public virtual int getParticleSize()
        {
            return size;
        }

        /// <summary>
        /// Returns a pointer to the layer specific data of the given particle.
        /// </summary>
        /// <param name="p">a particle.</param>
        /// <returns>a pointer to the data that is specific to this layer for the
        /// given particle.</returns>
        public void getParticleData(ParticleStorage.Particle p)
        {
            return (void*)(((unsigned char*) p) +offset);
        }

        /// <summary>
        /// Returns a pointer to the Particle corresponding to the given layer
        /// specific data.
        /// </summary>
        /// <param name="">a pointer to the data that is specific to this layer for a
        /// given particle.</param>
        /// <param name="p"></param>
        /// <returns>the corresponding Particle.</returns>
        public ParticleStorage.Particle getParticle(Object p)
        {
            return (ParticleStorage.Particle)(((unsigned char*) p) -offset);
        }

        /// <summary>
        /// Returns the tile producers used by this ParticleLayer.
        ///
        /// <param name="out">producers the tile producers used by this ParticleLayer.</param>
        /// </summary>
        public virtual void getReferencedProducers(List<TileProducer> producers)
        {
        }

        /// <summary>
        /// Moves the existing %particles.
        /// The default implementation of this method does nothing.
        /// </summary>
        /// <param name="dt">the elapsed time since the last call to this method, in
        /// microseconds.</param>
        public virtual void moveParticles(double dt)
        {

        }

        /// <summary>
        /// Removes old %particles.
        /// The default implementation of this method does nothing.
        /// </summary>
        public virtual void removeOldParticles()
        {

        }

        /// <summary>
        /// Adds new %particles.
        /// The default implementation of this method does nothing.
        /// </summary>
        public virtual void addNewParticles()
        {

        }


        /// <summary>
        /// Initializes this ParticleLayer.
        /// </summary>
        internal void init(int particleSize)
        {
            this.owner = null;
            // padding to 32 bits words
            this.size = (particleSize / 8) * 8 + 8 * (particleSize % 8 != 0 ? particleSize % 8 : 1); //8 * (x % 8 != 0);
            this.offset = 0;
            this.enabled = true;
        }

        /// <summary>
        /// Initializes this ParticleLayer. If this layer needs reference to other
        /// layers it can initialize them in this method (using the template method
        /// ParticleProducer#getLayer()). The default implementation of this method
        /// does nothing.
        /// </summary>
        internal virtual void initialize()
        {

        }

        /// <summary>
        /// Initializes the data that is specific to this layer in the given
        /// particle.The default implementation of this method does nothing.
        /// </summary>
        internal virtual void initParticle(ParticleStorage.Particle p)
        {

        }

        internal virtual void swap(ParticleLayer p)
        {
            Std.Swap(ref owner, ref p.owner);
            Std.Swap(ref offset, ref p.offset);
            Std.Swap(ref enabled, ref p.enabled);
        }


        /// <summary>
        /// The ParticleProducer to which this ParticleLayer belongs.
        /// </summary>
        internal ParticleProducer owner;

        /// <summary>
        /// The size in bytes of the layer specific data that must be stored for
        /// each particle.
        /// </summary>
        internal int size;

        /// <summary>
        /// The offset of the data that is specific to this layer in the global
        /// particle data.
        /// </summary>
        internal int offset;

        /// <summary>
        /// True if this layer is enabled.
        /// </summary>
        internal bool enabled;

    }
}
