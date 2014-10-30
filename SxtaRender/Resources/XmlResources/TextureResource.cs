using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sxta.Render.Resources.XmlResources
{
    public class TextureResource
    {
        public static void getParameters(ResourceDescriptor desc, XmlElement e, out TextureInternalFormat ff, out TextureFormat f, out PixelType t)
        {
            string v = e.Attributes["internalformat"].Value;
            if (string.IsNullOrWhiteSpace(v))
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Missing 'internalformat' attribute");
                }
                throw new ArgumentException("Missing 'internalformat' attribute");
            }

            if (!Enum.TryParse<TextureInternalFormat>(v, out ff))
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Bad 'internalformat' attribute : " + v);
                }
                throw new ArgumentException();
            }

            v = e.Attributes["format"].Value;
            if (string.IsNullOrWhiteSpace(v))
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Missing 'format' attribute");
                }
                throw new ArgumentException("Missing 'format' attribute");
            }

            if (!Enum.TryParse<TextureFormat>(v, out f))
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Bad 'format' attribute" + v);
                }
                throw new ArgumentException("Bad 'format' attribute" + v);
            }

            v = e.Attributes["type"].Value;
            if (string.IsNullOrWhiteSpace(v))
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Missing 'type' attribute");
                }
                throw new ArgumentException("Missing 'type' attribute");
            }
            if (!Enum.TryParse<PixelType>(v, out t))
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Bad 'type' attribute" + v);
                }
                throw new ArgumentException("Bad 'type' attribute" + v);
            }
        }


        public static void getParameters(ResourceDescriptor desc, XmlElement e, ref Texture.Parameters params_)
        {
            string v = e.GetAttribute("min");
            if (!string.IsNullOrWhiteSpace(v))
            {
                TextureFilter filter;
                if (Enum.TryParse<TextureFilter>(v, out filter))
                {
                    params_.min(filter);
                }
                else
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("Bad minification attribute " + v);
                    }
                    throw new ArgumentException("Bad minification attribute " + v);
                }
            }

            v = e.GetAttribute("mag");
            if (!string.IsNullOrWhiteSpace(v))
            {
                TextureFilter filter;
                if (Enum.TryParse<TextureFilter>(v, out filter))
                {
                    params_.mag(filter);
                }
                else
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("Bad magnification attribute " + v);
                    }
                    throw new ArgumentException("Bad magnification attribute " + v);
                }
            }

            v = e.GetAttribute("wraps");
            if (!string.IsNullOrWhiteSpace(v))
            {
                TextureWrap wrap;
                if (Enum.TryParse<TextureWrap>(v, out wrap))
                {
                    params_.wrapS(wrap);
                }
                else
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("Bad wrap S attribute " + v);
                    }
                    throw new ArgumentException("Bad wrap S attribute " + v);
                }
            }

            v = e.GetAttribute("wrapt");
            if (!string.IsNullOrWhiteSpace(v))
            {
                TextureWrap wrap;
                if (Enum.TryParse<TextureWrap>(v, out wrap))
                {
                    params_.wrapT(wrap);
                }
                else
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("Bad wrap T attribute " + v);
                    }
                    throw new ArgumentException("Bad wrap T attribute " + v);
                }
            }

            v = e.GetAttribute("wrapr");
            if (!string.IsNullOrWhiteSpace(v))
            {
                TextureWrap wrap;
                if (Enum.TryParse<TextureWrap>(v, out wrap))
                {
                    params_.wrapR(wrap);
                }
                else
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("Bad wrap R attribute " + v);
                    }
                    throw new ArgumentException("Bad wrap R attribute " + v);
                }
            }

            v = e.GetAttribute("borderType");
            int borderType = 0;
            if (string.IsNullOrWhiteSpace(v) || v == "INT")
            {
                borderType = 0;
            }
            else if (v == "FLOAT")
            {
                borderType = 1;
            }
            else if (v == "IINT")
            {
                borderType = 2;
            }
            else if (v == "IUNSIGNED_INT")
            {
                borderType = 3;
            }
            else
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Bad border type attribute " + v);
                }
                throw new ArgumentException("Bad border type attribute " + v);
            }

            if (!string.IsNullOrWhiteSpace(e.GetAttribute("borderr")))
            {
                float r, g, b, a;
                Resource.getFloatParameter(desc, e, "borderr", out r);
                Resource.getFloatParameter(desc, e, "borderg", out g);
                Resource.getFloatParameter(desc, e, "borderb", out b);
                Resource.getFloatParameter(desc, e, "bordera", out a);
                switch (borderType)
                {
                    case 0:
                        params_.borderi((int)(r), (int)(g), (int)(b), (int)(a));
                        break;
                    case 1:
                        params_.borderf((float)(r), (float)(g), (float)(b), (float)(a));
                        break;
                    case 2:
                        params_.borderIi((int)(r), (int)(g), (int)(b), (int)(a));
                        break;
                    case 3:
                        params_.borderIui((uint)(r), (uint)(g), (uint)(b), (uint)(a));
                        break;
                }
            }


            if (!string.IsNullOrWhiteSpace(e.GetAttribute("minLevel")))
            {
                float level;
                Resource.getFloatParameter(desc, e, "minLevel", out level);
                params_.minLevel((int)level);
            }
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("maxLevel")))
            {
                float level;
                Resource.getFloatParameter(desc, e, "maxLevel", out level);
                params_.maxLevel((int)level);
            }
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("minLod")))
            {
                float lod;
                Resource.getFloatParameter(desc, e, "minLod", out lod);
                params_.lodMin(lod);
            }
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("maxLod")))
            {
                float lod;
                Resource.getFloatParameter(desc, e, "maxLod", out lod);
                params_.lodMax(lod);
            }
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("bias")))
            {
                float lod;
                Resource.getFloatParameter(desc, e, "bias", out lod);
                params_.lodBias(lod);
            }

            if (!string.IsNullOrWhiteSpace(e.GetAttribute("anisotropy")))
            {
                float a;
                Resource.getFloatParameter(desc, e, "anisotropy", out a);
                params_.maxAnisotropyEXT(a);
            }

            v = e.GetAttribute("compare");
            if (!string.IsNullOrWhiteSpace(v))
            {
                Function func;
                if (Enum.TryParse<Function>(v, out func))
                {

                    params_.compareFunc(func);
                }
                else
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("Bad compare function attribute " + v);
                    }
                    throw new ArgumentException("Bad compare function attribute " + v);
                }
            }

            v = e.GetAttribute("swizzle");
            if (!string.IsNullOrWhiteSpace(v))
            {
                if (v.Length == 4)
                {
                    params_.swizzle(v[0], v[1], v[2], v[3]);
                }
                else
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("Bad swizzle attribute " + v);
                    }
                    throw new ArgumentException("Bad swizzle attribute " + v);
                }
            }
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }

    public class Texture1DResource : ResourceTemplate<Texture1D>
    {
        public static Texture1DResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new Texture1DResource(manager, name, desc, e);
        }

        public Texture1DResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(0, manager, name, desc)
        {
            e = (e == null ? desc.descriptor : e);
            TextureInternalFormat tf;
            TextureFormat f;
            PixelType t;
            Texture.Parameters params_ = new Texture.Parameters();
            Buffer.Parameters s = new Buffer.Parameters();
            int w;
            try
            {
                checkParameters(desc, e, "name,source,internalformat,format,type,min,mag,wraps,minLod,maxLod,width,height,");
                getIntParameter(desc, e, "width", out w);
                TextureResource.getParameters(desc, e, out tf, out f, out t);
                TextureResource.getParameters(desc, e, ref params_);
                s.compressedSize(desc.getSize());
                valueC.init(w, tf, f, t, params_, s, (Buffer)desc.getData());
                this.valueC.Name = name;
                desc.clearData();
            }
            catch (Exception ex)
            {
                desc.clearData();
                throw ex;
            }
        }
    }

    public class Texture1DArrayResource : ResourceTemplate<Texture1DArray>
    {
        public static Texture1DArrayResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new Texture1DArrayResource(manager, name, desc, e);
        }

        public Texture1DArrayResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(0, manager, name, desc)
        {
            e = (e == null ? desc.descriptor : e);
            TextureInternalFormat tf;
            TextureFormat f;
            PixelType t;
            Texture.Parameters params_ = new Texture.Parameters();
            Buffer.Parameters s = new Buffer.Parameters();
            int w;
            int l;
            try
            {
                checkParameters(desc, e, "name,source,internalformat,format,type,min,mag,wraps,minLod,maxLod,width,height,");
                getIntParameter(desc, e, "width", out w);
                getIntParameter(desc, e, "height", out l);
                TextureResource.getParameters(desc, e, out tf, out f, out t);
                TextureResource.getParameters(desc, e, ref params_);
                s.compressedSize(desc.getSize());
                s.compressedSize(desc.getSize());
                valueC.init(w, l, tf, f, t, params_, s, (Buffer)desc.getData());
                this.valueC.Name = name;
                desc.clearData();
            }
            catch (Exception ex)
            {
                desc.clearData();
                throw ex;
            }
        }
    }

    public class Texture2DResource : ResourceTemplate<Texture2D>
    {
        public static Texture2DResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new Texture2DResource(manager, name, desc, e);
        }

        public Texture2DResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(0, manager, name, desc)
        {
            e = (e == null ? desc.descriptor : e);
            TextureInternalFormat tf;
            TextureFormat f;
            PixelType t;
            Texture.Parameters params_ = new Texture.Parameters();
            Buffer.Parameters s = new Buffer.Parameters();
            int w;
            int h;
            try
            {
                checkParameters(desc, e, "name,source,internalformat,format,type,min,mag,wraps,wrapt,minLod,maxLod,compare,borderType,borderr,borderg,borderb,bordera,maxAniso,width,height,");
                getIntParameter(desc, e, "width", out w);
                getIntParameter(desc, e, "height", out h);
                TextureResource.getParameters(desc, e, out tf, out f, out t);
                TextureResource.getParameters(desc, e, ref params_);
                s.compressedSize(desc.getSize());
                valueC = new Texture2D();
                buff = (Buffer)desc.getData();
                this.valueC.init(w, h, tf, f, t, params_, s, buff);
                this.valueC.Name = name;
                desc.clearData();
            }
            catch (Exception ex)
            {
                desc.clearData();
                throw ex;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (buff != null && buff is IDisposable)
                ((IDisposable)buff).Dispose();
        }

        Buffer buff;
    }

    public class Texture2DArrayResource : ResourceTemplate<Texture2DArray>
    {
        public static Texture2DArrayResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new Texture2DArrayResource(manager, name, desc, e);
        }

        public Texture2DArrayResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(0, manager, name, desc)
        {
            e = (e == null ? desc.descriptor : e);
            TextureInternalFormat tf;
            TextureFormat f;
            PixelType t;
            Texture.Parameters params_ = new Texture.Parameters();
            Buffer.Parameters s = new Buffer.Parameters();
            int w, h, l;
            try
            {
                checkParameters(desc, e, "name,source,internalformat,format,type,min,mag,wraps,wrapt,minLod,maxLod,compare,borderType,borderr,borderg,borderb,bordera,maxAniso,width,height,depth,layers,");
                getIntParameter(desc, e, "width", out w);
                getIntParameter(desc, e, "height", out h);
                if (e.Attributes["depth"] != null)
                {
                    getIntParameter(desc, e, "depth", out l);
                }
                else
                {
                    getIntParameter(desc, e, "layers", out l);
                }
                if (h % l != 0)
                {
                    throw new Exception("Inconsistent 'height' and 'layers' attributes");
                }
                TextureResource.getParameters(desc, e, out tf, out f, out t);
                TextureResource.getParameters(desc, e, ref params_);
                s.compressedSize(desc.getSize());
                valueC.init(w, h / l, l, tf, f, t, params_, s, (Buffer)desc.getData());
                this.valueC.Name = name;
                desc.clearData();
            }
            catch (Exception ex)
            {
                desc.clearData();
                throw ex;
            }
        }
    }

    public class Texture3DResource : ResourceTemplate<Texture3D>
    {
        public static Texture3DResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new Texture3DResource(manager, name, desc, e);
        }

        public Texture3DResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(0, manager, name, desc)
        {
            e = (e == null ? desc.descriptor : e);
            TextureInternalFormat tf;
            TextureFormat f;
            PixelType t;
            Texture.Parameters params_ = new Texture.Parameters();
            Buffer.Parameters s = new Buffer.Parameters();
            int w;
            int h;
            int d;
            try
            {
                checkParameters(desc, e, "name,source,internalformat,format,type,min,mag,wraps,wrapt,wrapr,minLod,maxLod,width,height,depth,");
                getIntParameter(desc, e, "width", out w);
                getIntParameter(desc, e, "height", out h);
                getIntParameter(desc, e, "depth", out d);
                if (h % d != 0)
                {
                    throw new Exception("Inconsistent 'height' and 'depth' attributes");
                }
                TextureResource.getParameters(desc, e, out tf, out f, out t);
                TextureResource.getParameters(desc, e, ref params_);
                s.compressedSize(desc.getSize());
                valueC.init(w, h / d, d, tf, f, t, params_, s, (Buffer)desc.getData());
                this.valueC.Name = name;
                desc.clearData();
            }
            catch (Exception ex)
            {
                desc.clearData();
                throw ex;
            }
        }
    }

    public class TextureCubeResource : ResourceTemplate<TextureCube>
    {
        public static TextureCubeResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new TextureCubeResource(manager, name, desc, e);
        }

        public TextureCubeResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(0, manager, name, desc)
        {
            e = (e == null ? desc.descriptor : e);
            TextureInternalFormat tf;
            TextureFormat f;
            PixelType t;
            Texture.Parameters params_ = new Texture.Parameters();
            Buffer.Parameters[] s = new Buffer.Parameters[6];
            int w;
            int h;
            try
            {
                checkParameters(desc, e, "name,source,internalformat,format,type,min,mag,wraps,wrapt,minLod,maxLod,width,height,");
                getIntParameter(desc, e, "width", out w);
                getIntParameter(desc, e, "height", out h);
                if (h != 6 * w)
                {
                    throw new Exception("Inconsistent 'width' and 'height' attributes");
                }
                TextureResource.getParameters(desc, e, out tf, out f, out t);
                TextureResource.getParameters(desc, e, ref params_);

                int bpp = EnumConversion.getFormatSize(f, t);

                Buffer[] pixels = new Buffer[6];
                for (int i = 0; i < 6; ++i)
                {
#if TODO
                    pixels[i] = new CPUBuffer<byte>(desc.getData() + i * w * w * bpp);
 
#endif
                    throw new NotImplementedException();
                }

                valueC.init(w, w, tf, f, t, params_, s, pixels);
                this.valueC.Name = name;
                desc.clearData();
            }
            catch (Exception ex)
            {
                desc.clearData();
                throw ex;
            }
        }
    }

    public class TextureCubeArrayResource : ResourceTemplate<TextureCubeArray>
    {
        public static TextureCubeArrayResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new TextureCubeArrayResource(manager, name, desc, e);
        }

        public TextureCubeArrayResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(0, manager, name, desc)
        {
            e = (e == null ? desc.descriptor : e);
            TextureInternalFormat tf;
            TextureFormat f;
            PixelType t;
            Texture.Parameters params_ = new Texture.Parameters();
            Buffer.Parameters s = new Buffer.Parameters();
            int w;
            int h;
            try
            {
                checkParameters(desc, e, "name,source,internalformat,format,type,min,mag,wraps,wrapt,minLod,maxLod,width,height,");
                getIntParameter(desc, e, "width", out w);
                getIntParameter(desc, e, "height", out h);
                if (h % (6 * w) != 0)
                {
                    throw new Exception("Inconsistent 'width' and 'height' attributes");
                }
                TextureResource.getParameters(desc, e, out tf, out f, out t);
                TextureResource.getParameters(desc, e, ref params_);

                valueC.init(w, w, h / (6 * w), tf, f, t, params_, s, (Buffer)desc.getData());
                this.valueC.Name = name;
                desc.clearData();
            }
            catch (Exception ex)
            {
                desc.clearData();
                throw ex;
            }
        }
    }

    public class TextureRectangleResource : ResourceTemplate<TextureRectangle>
    {
        public static TextureRectangleResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new TextureRectangleResource(manager, name, desc, e);
        }

        public TextureRectangleResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(0, manager, name, desc)
        {
            e = (e == null ? desc.descriptor : e);
            TextureInternalFormat tf;
            TextureFormat f;
            PixelType t;
            Texture.Parameters params_ = new Texture.Parameters();
            Buffer.Parameters s = new Buffer.Parameters();
            int w;
            int h;
            try
            {
                checkParameters(desc, e, "name,source,internalformat,format,type,min,mag,wraps,wrapt,maxAniso,width,height,");
                getIntParameter(desc, e, "width", out w);
                getIntParameter(desc, e, "height", out h);
                TextureResource.getParameters(desc, e, out tf, out f, out t);
                TextureResource.getParameters(desc, e, ref params_);
                s.compressedSize(desc.getSize());
                valueC.init(w, h, tf, f, t, params_, s, (Buffer)desc.getData());
                this.valueC.Name = name;
                desc.clearData();
            }
            catch (Exception ex)
            {
                desc.clearData();
                throw ex;
            }
        }
    }
}
