using log4net;
using Sxta.Math;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sxta.Render.Scenegraph.XmlResources
{
    public class SceneNodeResource : ResourceTemplate<SceneNode>
    {
        public static SceneNodeResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new SceneNodeResource(manager, name, desc, e);
        }

        public SceneNodeResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(50, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,flags,value,");
            this.valueC = new SceneNode();
            string flags = e.GetAttribute("flags");
            if (!string.IsNullOrWhiteSpace(flags))
            {
                foreach (string flag in flags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    this.valueC.addFlag(flag);
                }
            }
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("name")))
            {
                this.valueC.addFlag(e.GetAttribute("name"));
            }

            Matrix4d ltop = Matrix4d.Identity;

            foreach (XmlNode n in e.ChildNodes)
            {
                XmlElement f = n as XmlElement;
                if (f == null)
                {
                    continue;
                }
                if (f.Name == "translate")
                {
                    float x, y, z;
                    checkParameters(desc, f, "x,y,z,");
                    getFloatParameter(desc, f, "x", out x);
                    getFloatParameter(desc, f, "y", out y);
                    getFloatParameter(desc, f, "z", out z);
                    ltop = ltop * Matrix4d.CreateTranslation(x, y, z); //AQUI MULT MATRIX CAMBIADO
                }
                else if (f.Name == "scale")
                {
                    float x, y, z;
                    checkParameters(desc, f, "x,y,z,");
                    getFloatParameter(desc, f, "x", out x);
                    getFloatParameter(desc, f, "y", out y);
                    getFloatParameter(desc, f, "z", out z);
                    ltop = ltop * Matrix4d.Scale(x, y, z); //AQUI MULT MATRIX CAMBIADO
                }
                else if (f.Name == "rotatex")
                {
                    float a;
                    checkParameters(desc, f, "angle,");
                    getFloatParameter(desc, f, "angle", out a);
                    ltop = ltop * Matrix4d.CreateRotationX(Math.MathHelper.ToRadians(a)); //AQUI MULT MATRIX CAMBIADO
                }
                else if (f.Name == "rotatey")
                {
                    float a;
                    checkParameters(desc, f, "angle,");
                    getFloatParameter(desc, f, "angle", out a); 
                    ltop = ltop * Matrix4d.CreateRotationY(Math.MathHelper.ToRadians(a)); //AQUI MULT MATRIX CAMBIADO
                }
                else if (f.Name == "rotatez")
                {
                    float a;
                    checkParameters(desc, f, "angle,");
                    getFloatParameter(desc, f, "angle", out a);
                    ltop = ltop * Matrix4d.CreateRotationZ(Math.MathHelper.ToRadians(a)); //AQUI MULT MATRIX CAMBIADO
                }
                else if (f.Name == "bounds")
                {
                    bool ok = true; ;
                    float xmin, xmax, ymin, ymax, zmin, zmax;
                    checkParameters(desc, f, "xmin,xmax,ymin,ymax,zmin,zmax,");
                    ok &= getFloatParameter(desc, f, "xmin", out xmin);
                    ok &= getFloatParameter(desc, f, "xmax", out xmax);
                    ok &= getFloatParameter(desc, f, "ymin", out ymin);
                    ok &= getFloatParameter(desc, f, "ymax", out ymax);
                    ok &= getFloatParameter(desc, f, "zmin", out zmin);
                    ok &= getFloatParameter(desc, f, "zmax", out zmax);
                    if (!ok)
                    {
                        throw new Exception("Invalid bounds");
                    }
                    this.valueC.setLocalBounds(new Box3d(xmin, xmax, ymin, ymax, zmin, zmax));

                }
                else if (f.Name.StartsWith("uniform"))
                {
                    checkParameters(desc, f, "name,id,x,y,z,w,sampler,texture,type,");
                    string type = string.IsNullOrWhiteSpace(f.GetAttribute("type")) ? "FLOAT" : f.GetAttribute("type");
                    int t;
                    if (type == "BOOL")
                    {
                        t = 0;
                    }
                    else if (type == "INT")
                    {
                        t = 1;
                    }
                    else if (type == "UINT")
                    {
                        t = 2;
                    }
                    else if (type == "FLOAT")
                    {
                        t = 3;
                    }
                    else
                    {
                        t = 4;
                    }
                    float x = 0.0f, y = 0.0f, z = 0.0f, w = 0.0f;
                    Texture texture = null;
                    string id = getParameter(desc, f, "id");
                    int paramCount = 0;
                    if (f.GetAttribute("x") != null)
                    {
                        getFloatParameter(desc, f, "x", out x);
                        paramCount++;
                        if (f.GetAttribute("y") != null)
                        {
                            getFloatParameter(desc, f, "y", out y);
                            paramCount++;
                            if (f.GetAttribute("z") != null)
                            {
                                getFloatParameter(desc, f, "z", out z);
                                paramCount++;
                                if (f.GetAttribute("w") != null)
                                {
                                    getFloatParameter(desc, f, "w", out w);
                                    paramCount++;
                                }
                            }
                        }
                        switch (t)
                        {
                            case 0:
                                switch (paramCount)
                                {
                                    case 1:
                                        this.valueC.addValue(new Value1b(id, x != 0));
                                        break;
                                    case 2:
                                        this.valueC.addValue(new Value2b(id, new Vector2b(x != 0, y != 0)));
                                        break;
                                    case 3:
                                        this.valueC.addValue(new Value3b(id, new Vector3b(x != 0, y != 0, z != 0)));
                                        break;
                                    case 4:
                                        this.valueC.addValue(new Value4b(id, new Vector4b(x != 0, y != 0, z != 0, w != 0)));
                                        break;
                                }
                                break;

                            case 1:
                                switch (paramCount)
                                {
                                    case 1:
                                        this.valueC.addValue(new Value1i(id, (int)x));
                                        break;
                                    case 2:
                                        this.valueC.addValue(new Value2i(id, new Vector2i((int)x, (int)y)));
                                        break;
                                    case 3:
                                        this.valueC.addValue(new Value3i(id, new Vector3i((int)x, (int)y, (int)z)));
                                        break;
                                    case 4:
                                        this.valueC.addValue(new Value4i(id, new Vector4i((int)x, (int)y, (int)z, (int)w)));
                                        break;
                                }
                                break;
                            case 2:
                                switch (paramCount)
                                {
                                    case 1:
                                        this.valueC.addValue(new Value1ui(id, (uint)x));
                                        break;
                                    case 2:
                                        this.valueC.addValue(new Value2ui(id, new Vector2ui((uint)x, (uint)y)));
                                        break;
                                    case 3:
                                        this.valueC.addValue(new Value3ui(id, new Vector3ui((uint)x, (uint)y, (uint)z)));
                                        break;
                                    case 4:
                                        this.valueC.addValue(new Value4ui(id, new Vector4ui((uint)x, (uint)y, (uint)z, (uint)w)));
                                        break;
                                }
                                break;
                            case 3:
                                switch (paramCount)
                                {
                                    case 1:
                                        this.valueC.addValue(new Value1f(id, x));
                                        break;
                                    case 2:
                                        this.valueC.addValue(new Value2f(id, new Vector2f(x, y)));
                                        break;
                                    case 3:
                                        this.valueC.addValue(new Value3f(id, new Vector3f(x, y, z)));
                                        break;
                                    case 4:
                                        this.valueC.addValue(new Value4f(id, new Vector4f(x, y, z, w)));
                                        break;
                                }
                                break;
                            case 4:
                                switch (paramCount)
                                {
                                    case 1:
                                        this.valueC.addValue(new Value1d(id, x));
                                        break;
                                    case 2:
                                        this.valueC.addValue(new Value2d(id, new Vector2d(x, y)));
                                        break;
                                    case 3:
                                        this.valueC.addValue(new Value3d(id, new Vector3d(x, y, z)));
                                        break;
                                    case 4:
                                        this.valueC.addValue(new Value4d(id, new Vector4d(x, y, z, w)));
                                        break;
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (f.GetAttribute("texture") != null)
                        {
                            texture = manager.loadResource(getParameter(desc, f, "texture")).get() as Texture;
                            UniformType uType = UniformType.SAMPLER_1D;
                            if (texture is Texture1D)
                            {
                                if (isIntegerTexture(texture))
                                {
                                    uType = UniformType.INT_SAMPLER_1D;
                                }
                                else if (isUnsignedIntegerTexture(texture))
                                {
                                    uType = UniformType.UNSIGNED_INT_SAMPLER_1D;
                                }
                                else
                                {
                                    uType = UniformType.SAMPLER_1D;
                                }
                            }
                            else if (texture is Texture1DArray)
                            {
                                if (isIntegerTexture(texture))
                                {
                                    uType = UniformType.INT_SAMPLER_1D_ARRAY;
                                }
                                else if (isUnsignedIntegerTexture(texture))
                                {
                                    uType = UniformType.UNSIGNED_INT_SAMPLER_1D_ARRAY;
                                }
                                else
                                {
                                    uType = UniformType.SAMPLER_1D_ARRAY;
                                }
                            }
                            else if (texture is Texture2D)
                            {
                                if (isIntegerTexture(texture))
                                {
                                    uType = UniformType.INT_SAMPLER_2D;
                                }
                                else if (isUnsignedIntegerTexture(texture))
                                {
                                    uType = UniformType.UNSIGNED_INT_SAMPLER_2D;
                                }
                                else
                                {
                                    uType = UniformType.SAMPLER_2D;
                                }
                            }
                            else if (texture is Texture2DArray)
                            {
                                if (isIntegerTexture(texture))
                                {
                                    uType = UniformType.INT_SAMPLER_2D_ARRAY;
                                }
                                else if (isUnsignedIntegerTexture(texture))
                                {
                                    uType = UniformType.UNSIGNED_INT_SAMPLER_2D_ARRAY;
                                }
                                else
                                {
                                    uType = UniformType.SAMPLER_2D_ARRAY;
                                }
                            }
                            else if (texture is Texture2DMultisample)
                            {
                                if (isIntegerTexture(texture))
                                {
                                    uType = UniformType.INT_SAMPLER_2D_MULTISAMPLE;
                                }
                                else if (isUnsignedIntegerTexture(texture))
                                {
                                    uType = UniformType.UNSIGNED_INT_SAMPLER_2D_MULTISAMPLE;
                                }
                                else
                                {
                                    uType = UniformType.SAMPLER_2D_MULTISAMPLE;
                                }
                            }
                            else if (texture is Texture2DMultisampleArray)
                            {
                                if (isIntegerTexture(texture))
                                {
                                    uType = UniformType.INT_SAMPLER_2D_MULTISAMPLE_ARRAY;
                                }
                                else if (isUnsignedIntegerTexture(texture))
                                {
                                    uType = UniformType.UNSIGNED_INT_SAMPLER_2D_MULTISAMPLE_ARRAY;
                                }
                                else
                                {
                                    uType = UniformType.SAMPLER_2D_MULTISAMPLE_ARRAY;
                                }
                            }
                            else if (texture is Texture3D)
                            {
                                if (isIntegerTexture(texture))
                                {
                                    uType = UniformType.INT_SAMPLER_3D;
                                }
                                else if (isUnsignedIntegerTexture(texture))
                                {
                                    uType = UniformType.UNSIGNED_INT_SAMPLER_3D;
                                }
                                else
                                {
                                    uType = UniformType.SAMPLER_3D;
                                }
                            }
                            else if (texture is TextureBuffer)
                            {
                                if (isIntegerTexture(texture))
                                {
                                    uType = UniformType.INT_SAMPLER_BUFFER;
                                }
                                else if (isUnsignedIntegerTexture(texture))
                                {
                                    uType = UniformType.UNSIGNED_INT_SAMPLER_BUFFER;
                                }
                                else
                                {
                                    uType = UniformType.SAMPLER_BUFFER;
                                }
                            }
                            else if (texture is TextureCube)
                            {
                                if (isIntegerTexture(texture))
                                {
                                    uType = UniformType.INT_SAMPLER_CUBE;
                                }
                                else if (isUnsignedIntegerTexture(texture))
                                {
                                    uType = UniformType.UNSIGNED_INT_SAMPLER_CUBE;
                                }
                                else
                                {
                                    uType = UniformType.SAMPLER_CUBE;
                                }
                            }
                            else if (texture is TextureCubeArray)
                            {
                                if (isIntegerTexture(texture))
                                {
                                    uType = UniformType.INT_SAMPLER_CUBE_MAP_ARRAY;
                                }
                                else if (isUnsignedIntegerTexture(texture))
                                {
                                    uType = UniformType.UNSIGNED_INT_SAMPLER_CUBE_MAP_ARRAY;
                                }
                                else
                                {
                                    uType = UniformType.SAMPLER_CUBE_MAP_ARRAY;
                                }
                            }
                            else if (texture is TextureRectangle)
                            {
                                if (isIntegerTexture(texture))
                                {
                                    uType = UniformType.INT_SAMPLER_2D_RECT;
                                }
                                else if (isUnsignedIntegerTexture(texture))
                                {
                                    uType = UniformType.UNSIGNED_INT_SAMPLER_2D_RECT;
                                }
                                else
                                {
                                    uType = UniformType.SAMPLER_2D_RECT;
                                }
                            }
                            else
                            {
                                Debug.Assert(false);
                            }
                            this.valueC.addValue(new ValueSampler(uType, id, texture));
                        }
                        else
                        {
                            //printf("%s\n", f.Name);
                            Debug.Assert(false);
                        }
                    }
                }
                else if (f.Name == "module")
                {
                    string id = getParameter(desc, f, "id");
                    string value = getParameter(desc, f, "value");
                    Module module = manager.loadResource(value).get() as Module;
                    this.valueC.addModule(id, module);

                }
                else if (f.Name == "mesh")
                {
                    checkParameters(desc, f, "id,value,");
                    string id = getParameter(desc, f, "id");
                    string value = getParameter(desc, f, "value");
                    MeshBuffers mesh = manager.loadResource(value).get() as MeshBuffers;
                    this.valueC.addMesh(id, mesh);

                }
                else if (f.Name == "field")
                {
                    checkParameters(desc, f, "id,value,");
                    string id = getParameter(desc, f, "id");
                    string value = getParameter(desc, f, "value");
                    Object field = manager.loadResource(value);
                    this.valueC.addField(id, field);
                }
                else if (f.Name == "method")
                {
                    checkParameters(desc, f, "id,value,enabled,");
                    string id = getParameter(desc, f, "id");
                    string value = getParameter(desc, f, "value");
                    TaskFactory meth = manager.loadResource(value).get() as TaskFactory;
                    Method method = new Method(meth);
                    string enable = f.GetAttribute("enabled");
                    if (enable != null && enable == "false")
                    {
                        method.setIsEnabled(false);
                    }
                    this.valueC.addMethod(id, method);

                }
                else if (f.Name == "node")
                {
                    SceneNode child = null;
                    string value = f.GetAttribute("value");
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        child = manager.loadResource(value).get() as SceneNode;
                    }
                    else
                    {
                        child = new SceneNodeResource(manager, "", desc, f).valueC;
                    }
                    this.valueC.addChild(child);

                }
                else
                {
                    string id = getParameter(desc, f, "id");
                    Object field = ResourceFactory.getInstance().create(manager, f.Name, desc, f);//manager.loadResource(string(value));
                    if (field != null)
                    {
                        this.valueC.addField(id, field);
                    }
                    else
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("Unknown scene node element '" + f.Name + "'");
                        }
                    }
                }
            }

            this.valueC.setLocalToParent(ltop);
        }
        private bool isIntegerTexture(Texture t)
        {
            switch (t.getInternalFormat())
            {
                case TextureInternalFormat.R8:
                case TextureInternalFormat.R8_SNORM:
                case TextureInternalFormat.R16:
                case TextureInternalFormat.R16_SNORM:
                case TextureInternalFormat.RG8:
                case TextureInternalFormat.RG8_SNORM:
                case TextureInternalFormat.RG16:
                case TextureInternalFormat.RG16_SNORM:
                case TextureInternalFormat.R3_G3_B2:
                case TextureInternalFormat.RGB4:
                case TextureInternalFormat.RGB5:
                case TextureInternalFormat.RGB8:
                case TextureInternalFormat.RGB8_SNORM:
                case TextureInternalFormat.RGB10:
                case TextureInternalFormat.RGB12:
                case TextureInternalFormat.RGB16:
                case TextureInternalFormat.RGB16_SNORM:
                case TextureInternalFormat.RGBA2:
                case TextureInternalFormat.RGBA4:
                case TextureInternalFormat.RGB5_A1:
                case TextureInternalFormat.RGBA8:
                case TextureInternalFormat.RGBA8_SNORM:
                case TextureInternalFormat.RGB10_A2:
                case TextureInternalFormat.RGB10_A2UI:
                case TextureInternalFormat.RGBA12:
                case TextureInternalFormat.RGBA16:
                case TextureInternalFormat.RGBA16_SNORM:
                case TextureInternalFormat.SRGB8:
                case TextureInternalFormat.SRGB8_ALPHA8:
                case TextureInternalFormat.R16F:
                case TextureInternalFormat.RG16F:
                case TextureInternalFormat.RGB16F:
                case TextureInternalFormat.RGBA16F:
                case TextureInternalFormat.R32F:
                case TextureInternalFormat.RG32F:
                case TextureInternalFormat.RGBA32F:
                case TextureInternalFormat.R11F_G11F_B10F:
                case TextureInternalFormat.RGB9_E5:
                    return false;
                case TextureInternalFormat.R8I:
                case TextureInternalFormat.R16I:
                case TextureInternalFormat.R32I:
                case TextureInternalFormat.RG8I:
                case TextureInternalFormat.RG16I:
                case TextureInternalFormat.RG32I:
                case TextureInternalFormat.RGB8I:
                case TextureInternalFormat.RGB16I:
                case TextureInternalFormat.RGB32I:
                case TextureInternalFormat.RGBA8I:
                case TextureInternalFormat.RGBA16I:
                case TextureInternalFormat.RGBA32I:
                    return true;
                case TextureInternalFormat.R8UI:
                case TextureInternalFormat.R16UI:
                case TextureInternalFormat.R32UI:
                case TextureInternalFormat.RG8UI:
                case TextureInternalFormat.RG16UI:
                case TextureInternalFormat.RG32UI:
                case TextureInternalFormat.RGB8UI:
                case TextureInternalFormat.RGB16UI:
                case TextureInternalFormat.RGB32UI:
                case TextureInternalFormat.RGBA8UI:
                case TextureInternalFormat.RGBA16UI:
                case TextureInternalFormat.RGBA32UI:
                case TextureInternalFormat.COMPRESSED_RED:
                case TextureInternalFormat.COMPRESSED_RG:
                case TextureInternalFormat.COMPRESSED_RGB:
                case TextureInternalFormat.COMPRESSED_RGBA:
                case TextureInternalFormat.COMPRESSED_SRGB:
                case TextureInternalFormat.COMPRESSED_RED_RGTC1:
                case TextureInternalFormat.COMPRESSED_SIGNED_RED_RGTC1:
                case TextureInternalFormat.COMPRESSED_RG_RGTC2:
                case TextureInternalFormat.COMPRESSED_SIGNED_RG_RGTC2:
                case TextureInternalFormat.COMPRESSED_RGBA_BPTC_UNORM_ARB:
                case TextureInternalFormat.COMPRESSED_SRGB_ALPHA_BPTC_UNORM_ARB:
                case TextureInternalFormat.COMPRESSED_RGB_BPTC_SIGNED_FLOAT_ARB:
                case TextureInternalFormat.COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT_ARB:
                case TextureInternalFormat.COMPRESSED_RGB_S3TC_DXT1_EXT:
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT1_EXT:
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT3_EXT:
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT5_EXT:
                    return false;
                default:
                    Debug.Assert(false);
                    return false;
            }
        }

        private bool isUnsignedIntegerTexture(Texture t)
        {
            switch (t.getInternalFormat())
            {
                case TextureInternalFormat.R8:
                case TextureInternalFormat.R8_SNORM:
                case TextureInternalFormat.R16:
                case TextureInternalFormat.R16_SNORM:
                case TextureInternalFormat.RG8:
                case TextureInternalFormat.RG8_SNORM:
                case TextureInternalFormat.RG16:
                case TextureInternalFormat.RG16_SNORM:
                case TextureInternalFormat.R3_G3_B2:
                case TextureInternalFormat.RGB4:
                case TextureInternalFormat.RGB5:
                case TextureInternalFormat.RGB8:
                case TextureInternalFormat.RGB8_SNORM:
                case TextureInternalFormat.RGB10:
                case TextureInternalFormat.RGB12:
                case TextureInternalFormat.RGB16:
                case TextureInternalFormat.RGB16_SNORM:
                case TextureInternalFormat.RGBA2:
                case TextureInternalFormat.RGBA4:
                case TextureInternalFormat.RGB5_A1:
                case TextureInternalFormat.RGBA8:
                case TextureInternalFormat.RGBA8_SNORM:
                case TextureInternalFormat.RGB10_A2:
                case TextureInternalFormat.RGB10_A2UI:
                case TextureInternalFormat.RGBA12:
                case TextureInternalFormat.RGBA16:
                case TextureInternalFormat.RGBA16_SNORM:
                case TextureInternalFormat.SRGB8:
                case TextureInternalFormat.SRGB8_ALPHA8:
                case TextureInternalFormat.R16F:
                case TextureInternalFormat.RG16F:
                case TextureInternalFormat.RGB16F:
                case TextureInternalFormat.RGBA16F:
                case TextureInternalFormat.R32F:
                case TextureInternalFormat.RG32F:
                case TextureInternalFormat.RGBA32F:
                case TextureInternalFormat.R11F_G11F_B10F:
                case TextureInternalFormat.RGB9_E5:
                case TextureInternalFormat.R8I:
                case TextureInternalFormat.R16I:
                case TextureInternalFormat.R32I:
                case TextureInternalFormat.RG8I:
                case TextureInternalFormat.RG16I:
                case TextureInternalFormat.RG32I:
                case TextureInternalFormat.RGB8I:
                case TextureInternalFormat.RGB16I:
                case TextureInternalFormat.RGB32I:
                case TextureInternalFormat.RGBA8I:
                case TextureInternalFormat.RGBA16I:
                case TextureInternalFormat.RGBA32I:
                    return false;
                case TextureInternalFormat.R8UI:
                case TextureInternalFormat.R16UI:
                case TextureInternalFormat.R32UI:
                case TextureInternalFormat.RG8UI:
                case TextureInternalFormat.RG16UI:
                case TextureInternalFormat.RG32UI:
                case TextureInternalFormat.RGB8UI:
                case TextureInternalFormat.RGB16UI:
                case TextureInternalFormat.RGB32UI:
                case TextureInternalFormat.RGBA8UI:
                case TextureInternalFormat.RGBA16UI:
                case TextureInternalFormat.RGBA32UI:
                    return true;
                case TextureInternalFormat.COMPRESSED_RED:
                case TextureInternalFormat.COMPRESSED_RG:
                case TextureInternalFormat.COMPRESSED_RGB:
                case TextureInternalFormat.COMPRESSED_RGBA:
                case TextureInternalFormat.COMPRESSED_SRGB:
                case TextureInternalFormat.COMPRESSED_RED_RGTC1:
                case TextureInternalFormat.COMPRESSED_SIGNED_RED_RGTC1:
                case TextureInternalFormat.COMPRESSED_RG_RGTC2:
                case TextureInternalFormat.COMPRESSED_SIGNED_RG_RGTC2:
                case TextureInternalFormat.COMPRESSED_RGBA_BPTC_UNORM_ARB:
                case TextureInternalFormat.COMPRESSED_SRGB_ALPHA_BPTC_UNORM_ARB:
                case TextureInternalFormat.COMPRESSED_RGB_BPTC_SIGNED_FLOAT_ARB:
                case TextureInternalFormat.COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT_ARB:
                case TextureInternalFormat.COMPRESSED_RGB_S3TC_DXT1_EXT:
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT1_EXT:
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT3_EXT:
                case TextureInternalFormat.COMPRESSED_RGBA_S3TC_DXT5_EXT:
                    return false;
                default:
                    Debug.Assert(false);
                    return false;
            }
        }
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    }
}
