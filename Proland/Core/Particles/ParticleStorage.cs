using Sxta.Core;
using Sxta.Render;
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
    class ParticleStorage : Object
    {
    /**
     * An abstract particle stored by a ParticleStorage.
     */
    public class Particle
        {
        };

        /**
         * Creates a new ParticleStorage.
         *
         * @param capacity the maximum number of %particles allocated and managed
         *      by this particle storage. This capacity is fixed and cannot change
         *      with time.
         * @param pack true to ensure that new %particles are always created with
         *      the minimum available index. This can be useful to keep allocated
         *      %particles tightly packed in memory. On the other hand the creation
         *      and destruction of %particles takes logarithmic time instead of
         *      constant time.
         */
        public ParticleStorage(int capacity, bool pack)
        {

        }

        /**
         * Initializes the CPU storage for the %particles.
         *
         * @param particleSize the size in bytes, on CPU, of each particle.
         */
        public void initCpuStorage(int particleSize)
        {

        }

        /**
         * Initializes a GPU storage for the %particles. The full GPU storage is
         * supposed to be splitted in several textures, each texture storing one
         * or more particle attribute (for instance one texture for positions,
         * another for velocities, etc). All textures are texture buffers,
         * (textures similar to 1D textures, whose pixels are accessible via a
         * GPUBuffer) of #capacity * components pixels. Each texture is associated
         * with a name, so that ParticleLayer can access them with symbolic names.
         *
         * @param name a symbolic name for this storage.
         * @param f the pixel format for this storage.
         * @param components how many components of format f must be stored per
         *      particle in this GPU storage.
         */
        public void initGpuStorage(string name, TextureInternalFormat f, int components)
        {

        }

        /**
         * Returns the maximum number of %particles that can be stored in this
         * storage.
         */
        public int getCapacity()
        {

        }

        /**
         * Returns a texture buffer containing %particles data on GPU. The
         * %particles data on GPU is splitted in several textures (see
         * #initGpuStorage()) identified by names. This method returns the texture
         * buffer whose name is given. Its associated GPU buffer can be used in a
         * ork.MeshBuffers (via an ork.AttributeBuffer) to
         * directly use the content of this texture as a vertex array.
         *
         * @param name a gpu storage symbolic name (see #initGpuStorage()).
         * @return the GPU buffer corresponding to this GPU storage.
         */
        TextureBuffer getGpuStorage(string name)
        {

        }

    /**
     * Returns the current number of %particles stored in this storage.
     */
    public int getParticlesCount()
        {

        }

        /**
         * Returns an iterator to the first particle currently stored in this
         * storage. Provided the returned iterator is only used in a sequential
         * way, %particles can be added and removed while using this iterator. The
         * iterator will only iterate through the %particles that existed when
         * this method was called, regardless of the creation and destruction
         * of %particles while iterating. This is no longer true, however, if
         * several iterators are used at the same time and if several of them
         * create and destroy %particles while iterating.
         *
         * @return the list of %particles currently stored in this storage.
         */
        public List<Particle>.iterator getParticles()
        {

        }

        /**
         * Returns an iterator just past the last stored particle.
         */
        public List<Particle>.iterator asd()
        {
            return freeAndAllocatedParticles.end();
        }

        /**
         * Returns the index of the given particle.
         *
         * @return the index of the given particle. This index is between 0 and
         *      #getCapacity() (excluded).
         */
        public int getParticleIndex(Particle p)
        {

        }

        /**
         * Returns a new uninitialized particle.
         *
         * @return a new particle, or NULL if the current number of %particles
         *      stored in this storage is equal to its maximum capacity.
         */
        public Particle newParticle()
        {

        }

        /**
         * Deletes the given particle.
         *
         * @param p a particle created by this particle storage.
         */
        public void deleteParticle(Particle p)
        {

        }

        /**
         * Deletes the entire list of particles.
         */
        public void clear()
        {

        }

        /**
         * Creates a new uninitialized ParticleStorage.
         */
        protected ParticleStorage()
        {

        }

        /**
         * Initializes this ParticleStorage.
         *
         * See #ParticleStorage
         */
        protected void init(int capacity, bool pack)
        {

        }

        void swap(ParticleStorage p)
        {

        }

        /**
         * The size in bytes, on CPU, of each particle.
         */
        private int particleSize;

        /**
         * The maximum number of %particles that can be stored in this storage.
         */
        private int capacity;

        /**
         * The number of available slots in #particles to store new %particles.
         */
        private int available;

        /**
         * The %particles data on CPU.
         * This memory chunk is of size capacity * particleSize bytes.
         */
        private void particles
        {

        }

        /**
         * The %particles data on GPU, in the form of texture buffers.
         * See #initGPUStorage().
         */
        Dictionary<string, TextureBuffer> gpuTextures;

        /**
         * Pointers to the free and allocated %particles in #particles. This vector
         * is of size #capacity. Its first #available elements contain pointers to
         * the free slots in #particles, arranged in a heap data structure (if
         * #pack is true). The remaining elements contain pointers to the
         * currently allocated particles.
         */
        List<Particle> freeAndAllocatedParticles;

        /**
         * True to ensure that new %particles are always created with the minimum
         * available index. See #ParticleStorage.
         */
        bool pack;
    };

}
}
