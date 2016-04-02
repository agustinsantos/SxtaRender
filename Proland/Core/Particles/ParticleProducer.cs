using log4net;
using proland;
using Sxta.Core;
using Sxta.Render;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace proland
{
    public class ParticleProducer : ISwappable<ParticleProducer>
    {

        /// <summary>
        /// Callback used to get the parameters of a particle in #copyToTexture().
        /// @param producer a particle producer.
        /// @param p a particle produced by 'producer'.
        /// @param[out] params the parameters for the given particle.
        /// </summary>
        public delegate bool getParticleParams(ParticleProducer producer, ParticleStorage.Particle p, float _params);

        /// <summary>
        /// Creates a new ParticleProducer.
        /// </summary>
        /// <param name="type">the type of this ParticleProducer.</param>
        /// <param name="storage">the storage to create and store the %particles.</param>
        public ParticleProducer(string type, ParticleStorage storage)
        {
            init(storage);
        }

        /// <summary>
        /// Creates an uninitialized ParticleProducer.
        /// </summary>
        internal ParticleProducer()
        {

        }

        /// <summary>
        /// Creates an uninitialized ParticleProducer.
        /// @param type the type of this %producer.
        /// </summary>
        internal ParticleProducer(string type)
        {

        }

        /// <summary>
        /// Returns the ParticleStorage used by this %producer to create and store
        /// its %particles.
        /// </summary>
        public ParticleStorage getStorage()
        {
            return storage;
        }

        /// <summary>
        /// Returns the number of layers of this %producer.
        /// </summary>
        /// <returns></returns>
        public int getLayerCount()
        {
            return (layers.Count());
        }

        /// <summary>
        /// Returns the layer of this %producer whose index is given.
        /// @param index a layer index between 0 and #getLayerCount (exclusive).
        /// </summary>
        public ParticleLayer getLayer(int index)
        {
            return layers[index];
        }

        /// <summary>
        /// Returns the first found layer of type T.
        /// @templateparam T the type of the layer to be looked for.
        /// @return the first found layer of type T.
        /// </summary>

        /**
    public template<typename T> T* getLayer() const
        {
        for (int i = 0; i<getLayerCount(); ++i) {
            ParticleLayer l = getLayer(i);
            T r = l.cast<T>();
            if (r != null) {
                return r.get();
            }
        }
            return null;
        }
**/
        /// <summary>
        /// Returns true if the list of layers is not empty. False otherwise.
        /// </summary>
        public bool hasLayers()
        {
            return layers.Count() > 0;
        }

        /// <summary>
        /// Adds a Layer to this %producer.
        /// @param l the layer to be added.
        /// </summary>
        public void addLayer(ParticleLayer l)
        {
            Debug.Assert(l.owner == null);
            l.owner = this;
            layers.Add(l);
        }

        /// <summary>
        /// Returns the tile producers used by this ParticleProducer.
        /// @param[out] producers the tile producers used by this ParticleProducer.
        /// </summary>
        public virtual void getReferencedProducers(List<TileProducer> producers)
        {
            for (int i = 0; i < (int)layers.Count(); i++)
            {
                layers[i].getReferencedProducers(producers);
            }
        }

        /// <summary>
        /// Returns the size in bytes of the data that must be stored for each
        /// particle. The default implementation of this method returns 0.
        /// </summary>
        public virtual int getParticleSize()
        {
            return 0;
        }

        /// <summary>
        /// Updates the %particles produced by this %producer. This method calls
        /// #moveParticles(), #removeOldParticles() and #addNewParticles(), in
        /// this order.
        /// @param dt the elapsed time since the last call to this method, in
        ///      microseconds.
        /// </summary>
        /// <param name="dt">the elapsed time since the last call to this method, in
        /// microseconds.</param>
        public virtual void updateParticles(double dt)
        {
            if (!initialized)
            {
                initialize();
            }
            moveParticles(dt);
            removeOldParticles();
            addNewParticles();
        }

        /// <summary>
        /// Moves the existing %particles.
        /// The default implementation of this method calls the corresponding
        /// method on each layer of this %producer.
        /// </summary>
        /// <param name="dt">the elapsed time since the last call to this method, in
        /// microseconds.</param>
        public virtual void moveParticles(double dt)
        {
            for (int i = 0; i < layers.Count(); ++i)
            {
                if (layers[i].isEnabled())
                {
                    layers[i].moveParticles(dt);
                }
            }
        }

        /// <summary>
        /// Removes old %particles.
        /// The default implementation of this method calls the corresponding
        /// method on each layer of this %producer.
        /// </summary>
        public virtual void removeOldParticles()
        {
            for (int i = 0; i < layers.Count(); ++i)
            {
                if (layers[i].isEnabled())
                {
                    layers[i].removeOldParticles();
                }
            }
        }

        /// <summary>
        /// Adds new particles.
        /// The default implementation of this method calls the corresponding
        /// method on each layer of this %producer.
        /// </summary>
        public virtual void addNewParticles()
        {
            for (int i = 0; i < layers.Count(); ++i)
            {
                if (layers[i].isEnabled())
                {
                    layers[i].addNewParticles();
                }
            }
        }

        /// <summary>
        /// Returns a new and initialized particle.
        /// The default implementation of this method creates a new particle with
        /// ParticleStorage#newParticle and initializes it by calling on each layer
        /// the ParticleLayer#initParticle method.
        /// </summary>
        public virtual ParticleStorage.Particle newParticle()
        {
            ParticleStorage.Particle p = storage.newParticle();
            if (p != null)
            {
                for (int i = 0; i < layers.Count(); ++i)
                {
                    layers[i].initParticle(p);
                }
            }
            return p;
        }

        /// <summary>
        /// Copies the %particles data to the given texture. The texture size must
        /// be ceil(paramCount / 4) times maxParticles, where maxParticles is the
        /// ParticleStorage capacity. Each particle is stored in its own row of
        /// the texture, with parameters stored in columns, four per column.
        /// </summary>
        /// <param name="t">the destination texture, or null to create a new one.</param>
        /// <param name="paramCount">the number of parameters to be stored per particle.</param>
        /// <param name="getParams">the function used to get the parameters of each
        /// particle.</param>
        /// <param name="useFuncRes">whether the user-defined function's return value
        ///  has an influence on what should be stored in the texture.
        /// If true, the user will have to determine how to access to the particles
        /// on GPU. Otherwise, the particles will be stored depending on their
        /// index in the storage. Default is false.</param>
        /// <returns>the given texture, or a new one if t was not of correct size.</returns>
        public Texture2D copyToTexture(Texture2D t, int paramCount, getParticleParams getParams, bool useFuncRes = false)
        {
            int width = (int)Math.Ceiling(paramCount / 4.0f);
            int height = getStorage().getCapacity();
            if (t == null || t.getWidth() != width || t.getHeight() != height)
            {
                t = new Texture2D(width, height, TextureInternalFormat.RGBA16F,
                    TextureFormat.RGBA, PixelType.FLOAT, new Texture.Parameters().wrapS(TextureWrap.CLAMP_TO_BORDER).wrapT(TextureWrap.CLAMP_TO_BORDER).min(TextureFilter.NEAREST).mag(TextureFilter.NEAREST),
                    new Sxta.Render.Buffer.Parameters(), new CPUBuffer<byte>(null));
            }
            if (_params == null || paramSize < 4 * width * height)
            {
                if (_params != null)
                {
                    //delete[] _params;
                }
                _params = new float[4 * width * height];
            }
            int maxHeight = 0;
            //List<ParticleStorage.Particle>.iterator i = storage.getParticles();
            //List<ParticleStorage.Particle>.iterator end = storage.end();
            int h = 0;
            foreach (ParticleStorage.Particle i in storage.getParticles())
            {
                ParticleStorage.Particle p = i;
                if (useFuncRes)
                {
                    h += getParams(this, p, _params + 4 * width * h);
                }
                else
                {
                    h = storage.getParticleIndex(p);
                    getParams(this, p, _params + 4 * width * h);
                    ++h;
                }
                maxHeight = Math.Max(maxHeight, h);
            }

            if (maxHeight > 0)
            {
                t.setSubImage(0, 0, 0, width, maxHeight, TextureFormat.RGBA, PixelType.FLOAT, new Sxta.Render.Buffer.Parameters(), new CPUBuffer<byte>(_params));
            }
            return t;
        }

        /// <summary>
        /// Initializes this ParticleProducer. See #ParticleProducer.
        /// </summary>
        internal void init(ParticleStorage storage)
        {
            this.storage = storage;
            this.paramSize = 0;
            this._params = null;
            this.initialized = false;
        }

        public virtual void swap(ParticleProducer p)
        {
            Std.Swap(ref storage, ref p.storage);
            //    std::swap(layers, p->layers);
            Std.Swap(ref paramSize, ref p.paramSize);
            Std.Swap(ref _params, ref p._params);
            Std.Swap(ref initialized, ref p.initialized);
        }


        /// <summary>
        /// The ParticleStorage used by this %producer to create and store its
        /// %particles.
        /// </summary>
        private ParticleStorage storage;

        /// <summary>
        /// The ParticleLayer associated with this %producer.
        /// </summary>
        private List<ParticleLayer> layers;

        /// <summary>
        /// The size of the #params array.
        /// </summary>
        private int paramSize;

        /// <summary>
        /// Temporary array used in #copyToTexture.
        /// </summary>
        private float[] _params;

        /// <summary>
        /// True if this %producer and its layers have been initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// Initializes the storage and the layers associated with this %producer.
        /// </summary>
        private void initialize()
        {
            Debug.Assert(!initialized);
            int totalSize = (getParticleSize() / 8) * 8 + 8 * (getParticleSize() % 8 != 0 ? getParticleSize() % 8 : 1);//(getParticleSize() % 8 != 0);
            for (int i = 0; i < layers.Count(); ++i)
            {
                layers[i].offset = totalSize;
                totalSize += layers[i].getParticleSize();
            }

            storage.initCpuStorage(totalSize);
            for (int i = 0; i < layers.Count(); ++i)
            {
                layers[i].initialize();
            }
            initialized = true;
        }
    }
    class ParticleProducerResource : ResourceTemplate<ParticleProducer>
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ParticleProducerResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null) :
        base(20, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,storage,");

            ParticleStorage storage = (ParticleStorage)manager.loadResource(getParameter(desc, e, "storage")).get();

            XmlNode n = e.FirstChild;
            while (n != null)
            {
                XmlElement f = n as XmlElement;
                if (f == null)
                {
                    n = n.NextSibling;
                    continue;
                }

                ParticleLayer l = (ParticleLayer)manager.loadResource(desc, f);
                if (l != null)
                {
                    valueC.addLayer(l);
                }
                else
                {
                    if (log.IsWarnEnabled)
                    {
                        log.Warn("Unknown scene node element '" + f.Value + "'");
                    }
                }
                n = n.NextSibling;
            }

            valueC.init(storage);

        }
    }
#if TODO
extern const char particleProducer[] = "particleProducer";

static ResourceFactory::Type<particleProducer, ParticleProducerResource> ParticleProducerType;
#endif
}
