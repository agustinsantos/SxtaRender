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
using System.Diagnostics;

namespace proland
{
    public class ParticleStorage : ISwappable<ParticleStorage>
    {
        /// <summary>
        /// An abstract particle stored by a ParticleStorage.
        /// </summary>
        public abstract class Particle
        {
        };

        /// <summary>
        /// Creates a new ParticleStorage.
        ///
        /// @param capacity the maximum number of %particles allocated and managed
        ///      by this particle storage. This capacity is fixed and cannot change
        ///      with time.
        /// @param pack true to ensure that new %particles are always created with
        ///      the minimum available index. This can be useful to keep allocated
        ///      %particles tightly packed in memory. On the other hand the creation
        ///      and destruction of %particles takes logarithmic time instead of
        ///      constant time.
        /// </summary>
        public ParticleStorage(int capacity, bool pack)
        {
            init(capacity, pack);
        }

        /// <summary>
        /// Initializes the CPU storage for the %particles.
        ///
        /// @param particleSize the size in bytes, on CPU, of each particle.
        /// </summary>
        public void initCpuStorage(int particleSize)
        {
            Debug.Assert(particleSize > 0);
            // we reserve additional space in each particle to store the index in
            // #freeAndAllocatedParticles of the element that points to this particle
            particleSize += sizeof(int);

            if (particles != null)
            {
                Debug.Assert(particleSize == this.particleSize);
                return;
            }
            this.particleSize = particleSize;
            unsigned char* data = new unsigned char[capacity * particleSize];
            for (int i = 0; i < capacity; ++i)
            {
                freeAndAllocatedParticles.Add((Particle)(data + i * particleSize));
            }
            if (pack)
            {
                make_heap(freeAndAllocatedParticles.begin(), freeAndAllocatedParticles.end(), greater<Particle*>());
            }
            particles = (Object)data;
        }

        /// <summary>
        /// Initializes a GPU storage for the %particles. The full GPU storage is
        /// supposed to be splitted in several textures, each texture storing one
        /// or more particle attribute (for instance one texture for positions,
        /// another for velocities, etc). All textures are texture buffers,
        /// (textures similar to 1D textures, whose pixels are accessible via a
        /// GPUBuffer) of #capacity * components pixels. Each texture is associated
        /// with a name, so that ParticleLayer can access them with symbolic names.
        ///
        /// @param name a symbolic name for this storage.
        /// @param f the pixel format for this storage.
        /// @param components how many components of format f must be stored per
        ///      particle in this GPU storage.
        /// </summary>
        public void initGpuStorage(string name, TextureInternalFormat f, int components)
        {
            int pixelSize;
            switch (f)
            {
                case TextureInternalFormat.R8:
                    pixelSize = 1;
                    break;
                case TextureInternalFormat.RG8:
                case TextureInternalFormat.R16F:
                    pixelSize = 2;
                    break;
                case TextureInternalFormat.RGBA8:
                case TextureInternalFormat.RG16F:
                case TextureInternalFormat.R32F:
                    pixelSize = 4;
                    break;
                case TextureInternalFormat.RG32F:
                case TextureInternalFormat.RGBA16F:
                    pixelSize = 8;
                    break;
                case TextureInternalFormat.RGBA32F:
                    pixelSize = 16;
                    break;
                default:
                    // unsupported formats
                    throw exception();
                    break;
            }
            GPUBuffer b = new GPUBuffer();
            b.setData(capacity * components * pixelSize, (IntPtr)null, BufferUsage.STREAM_DRAW);
            TextureBuffer t = new TextureBuffer(f, b);
            gpuTextures[name] = t;
        }

        /// <summary>
        /// Returns the maximum number of %particles that can be stored in this
        /// storage.
        /// </summary>
        public int getCapacity()
        {
            return capacity;
        }

        /// <summary>
        /// Returns a texture buffer containing %particles data on GPU. The
        /// %particles data on GPU is splitted in several textures (see
        /// #initGpuStorage()) identified by names. This method returns the texture
        /// buffer whose name is given. Its associated GPU buffer can be used in a
        /// ork.MeshBuffers (via an ork.AttributeBuffer) to
        /// directly use the content of this texture as a vertex array.
        ///
        /// @param name a gpu storage symbolic name (see #initGpuStorage()).
        /// @return the GPU buffer corresponding to this GPU storage.
        /// </summary>
        TextureBuffer getGpuStorage(string name) //TOSEE Using dictionaries = not usefull.
        {
            TextureBuffer i;
            gpuTextures.TryGetValue(name, out i);
            if (gpuTextures.ContainsKey(name))
            {
                return i;
            }
            return null;
        }

        /// <summary>
        /// Returns the current number of %particles stored in this storage.
        /// </summary>
        public int getParticlesCount()
        {
            return capacity - available;
        }

        /// <summary>
        /// Returns an iterator to the first particle currently stored in this
        /// storage. Provided the returned iterator is only used in a sequential
        /// way, %particles can be added and removed while using this iterator. The
        /// iterator will only iterate through the %particles that existed when
        /// this method was called, regardless of the creation and destruction
        /// of %particles while iterating. This is no longer true, however, if
        /// several iterators are used at the same time and if several of them
        /// create and destroy %particles while iterating.
        ///
        /// @return the list of %particles currently stored in this storage.
        /// </summary>
        public List<Particle> getParticles()
        {
            return freeAndAllocatedParticles;
        }

        /// <summary>
        /// Returns an iterator just past the last stored particle.
        /// </summary>
        //public List<Particle>.iterator asd()
        //{
        //    return freeAndAllocatedParticles.end();
        //}

        /// <summary>
        /// Returns the index of the given particle.
        ///
        /// @return the index of the given particle. This index is between 0 and
        ///      #getCapacity() (excluded).
        /// </summary>
        public int getParticleIndex(Particle p)
        {
            return (((unsigned char*) p) -((unsigned char*) particles)) / particleSize;
        }

        /// <summary>
        /// Returns a new uninitialized particle.
        ///
        /// @return a new particle, or NULL if the current number of %particles
        ///      stored in this storage is equal to its maximum capacity.
        /// </summary>
        public Particle newParticle()
        {
            Debug.Assert(particles != null);
            if (available == 0)
            {
                return null;
            }
            if (pack)
            {
                pop_heap(freeAndAllocatedParticles.begin(), freeAndAllocatedParticles.begin() + available, greater<Particle>());
            }
            Particle p = freeAndAllocatedParticles[--available];
            *((int*)(((unsigned char*) p) +particleSize - sizeof(int))) = available;
            return p;
        }

        /// <summary>
        /// Deletes the given particle.
        ///
        /// @param p a particle created by this particle storage.
        /// </summary>
        public void deleteParticle(Particle p)
        {
            Debug.Assert(particles != null && p != null && available < capacity);
            Particle q = freeAndAllocatedParticles[available];
            int index = *((int*)(((unsigned char*) p) +particleSize - sizeof(int)));
            *((int*)(((unsigned char*) q) +particleSize - sizeof(int))) = index;
            freeAndAllocatedParticles[index] = q;
            freeAndAllocatedParticles[available++] = p;
            if (pack)
            {
                push_heap(freeAndAllocatedParticles.begin(), freeAndAllocatedParticles.begin() + available, greater<Particle*>());
            }
        }

        /// <summary>
        /// Deletes the entire list of particles.
        /// </summary>
        public void clear()
        {
            available = capacity;
        }

        /// <summary>
        /// Creates a new uninitialized ParticleStorage.
        /// </summary>
        protected ParticleStorage(ParticleStorage p)
        {
            Std.Swap(ref particleSize, ref p.particleSize);
            Std.Swap(ref capacity, ref p.capacity);
            Std.Swap(ref available, ref p.available);
            Std.Swap(ref particles, ref p.particles);
            Std.Swap(ref gpuTextures, ref p.gpuTextures);
            Std.Swap(ref freeAndAllocatedParticles, ref p.freeAndAllocatedParticles);
            Std.Swap(ref pack, ref p.pack);
        }

        /// <summary>
        /// Initializes this ParticleStorage.
        ///
        /// See #ParticleStorage
        /// </summary>
        protected void init(int capacity, bool pack)
        {
            this.capacity = capacity;
            this.available = capacity;
            this.particles = null;
            this.pack = pack;
        }

        public void swap(ParticleStorage p)
        {

        }

        /// <summary>
        /// The size in bytes, on CPU, of each particle.
        /// </summary>
        private int particleSize;

        /// <summary>
        /// The maximum number of %particles that can be stored in this storage.
        /// </summary>
        private int capacity;

        /// <summary>
        /// The number of available slots in #particles to store new %particles.
        /// </summary>
        private int available;

        /// <summary>
        /// The %particles data on CPU.
        /// This memory chunk is of size capacity * particleSize bytes.
        /// </summary>
        private Object particles;

        /// <summary>
        /// The %particles data on GPU, in the form of texture buffers.
        /// See #initGPUStorage().
        /// </summary>
        private Dictionary<string, TextureBuffer> gpuTextures;

        /// <summary>
        /// Pointers to the free and allocated %particles in #particles. This vector
        /// is of size #capacity. Its first #available elements contain pointers to
        /// the free slots in #particles, arranged in a heap data structure (if
        /// #pack is true). The remaining elements contain pointers to the
        /// currently allocated particles.
        /// </summary>
        private List<Particle> freeAndAllocatedParticles;

        /// <summary>
        /// True to ensure that new %particles are always created with the minimum
        /// available index. See #ParticleStorage.
        /// </summary>
        bool pack;
    };

    class ParticleStorageResource : ResourceTemplate<ParticleStorage>
    {
        public ParticleStorageResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
                base(50, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,capacity,pack,");

            bool pack = true;
            int capacity;
            getIntParameter(desc, e, "capacity", capacity);

            if (e.GetAttribute("pack") != null)
            {
                pack = e.GetAttribute("pack") == "true"; //== 0;
            }

            valueC.init(capacity, pack);

        }
    };
#if TODO
extern const char particleStorage[] = "particleStorage";

static ResourceFactory::Type<particleStorage, ParticleStorageResource> ParticleStorageType;
#endif
}

