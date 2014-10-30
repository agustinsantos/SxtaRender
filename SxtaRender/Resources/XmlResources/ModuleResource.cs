using log4net;
using Sxta.Math;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sxta.Render.Resources.XmlResources
{
    public class ModuleResource : ResourceTemplate<Module>
    {
        public static ModuleResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new ModuleResource(manager, name, desc, e);
        }

        //20
        public ModuleResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(20, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            try
            {
                checkParameters(desc, e, "name,version, source,vertex,tessControl,tessEvaluation,geometry,fragment,feedback,varyings,options,");


                int version;
                getIntParameter(desc, e, "version", out version);
                this.valueC = new Module();
                this.valueC.Name = name;

                XmlNode n = e.FirstChild;
                while (n != null)
                {
                    XmlElement f = n as XmlElement;
                    if (f != null)
                    {
                        string na = f.GetAttribute("name");

                        string type = f.Name;
                        Value v = null;
                        if (type.StartsWith("uniform1"))
                        {
                            checkParameters(desc, f, "name,x,");
                            float x = 0.0f;
                            getFloatParameter(desc, f, "x", out x);
                            switch (type[8])
                            {
                                case 'f':
                                    v = new Value1f(na, (float)x);
                                    break;
                                case 'd':
                                    v = new Value1d(na, (double)x);
                                    break;
                                case 'i':
                                    v = new Value1i(na, (int)x);
                                    break;
                                case 'b':
                                    v = new Value1b(na, x != 0);
                                    break;
                                case 'v':
                                    if (type[9] == 'i')
                                        v = new Value1ui(na, (uint)x);
                                    break;
                                default:
                                    log.Error("Invalid type specifier '" + type + "'");
                                    throw new Exception();

                            }
                        }
                        else if (type.StartsWith("uniform2"))
                        {
                            checkParameters(desc, f, "name,x,y,");
                            float x = 0.0f;
                            float y = 0.0f;
                            getFloatParameter(desc, f, "x", out x);
                            getFloatParameter(desc, f, "y", out y);
                            switch (type[8])
                            {
                                case 'f':
                                    v = new Value2f(na, new Vector2f(x, y));
                                    break;
                                case 'd':
                                    v = new Value2d(na, new Vector2d(x, y));
                                    break;
                                case 'i':
                                    v = new Value2i(na, new Vector2i((int)x, (int)y));
                                    break;
                                case 'b':
                                    v = new Value2b(na, new Vector2b(x != 0, y != 0));
                                    break;
                                case 'v':
                                    if (type[9] == 'i')
                                        v = new Value2ui(na, new Vector2ui((uint)x, (uint)y));
                                    break;
                                default:
                                    log.Error("Invalid type specifier '" + type + "'");
                                    throw new Exception();

                            }
                        }
                        else if (type.StartsWith("uniform3"))
                        {
                            checkParameters(desc, f, "name,x,y,z,");
                            float x = 0.0f;
                            float y = 0.0f;
                            float z = 0.0f;
                            getFloatParameter(desc, f, "x", out x);
                            getFloatParameter(desc, f, "y", out y);
                            getFloatParameter(desc, f, "z", out z);
                            switch (type[8])
                            {
                                case 'f':
                                    v = new Value3f(na, new Vector3f(x, y, z));
                                    break;
                                case 'd':
                                    v = new Value3d(na, new Vector3d(x, y, z));
                                    break;
                                case 'i':
                                    v = new Value3i(na, new Vector3i((int)x, (int)y, (int)z));
                                    break;
                                case 'b':
                                    v = new Value3b(na, new Vector3b(x != 0, y != 0, z != 0));
                                    break;
                                case 'v':
                                    if (type[9] == 'i')
                                        v = new Value3ui(na, new Vector3ui((uint)(x), (uint)(y), (uint)(z)));
                                    break;
                                default:
                                    log.Error("Invalid type specifier '" + type + "'");
                                    throw new Exception();

                            }
                        }
                        else if (type.StartsWith("uniform4"))
                        {
                            checkParameters(desc, f, "name,x,y,z,w,");
                            float x = 0.0f;
                            float y = 0.0f;
                            float z = 0.0f;
                            float w = 0.0f;
                            getFloatParameter(desc, f, "x", out x);
                            getFloatParameter(desc, f, "y", out y);
                            getFloatParameter(desc, f, "z", out z);
                            getFloatParameter(desc, f, "w", out w);
                            switch (type[8])
                            {
                                case 'f':
                                    v = new Value4f(na, new Vector4f(x, y, z, w));
                                    break;
                                case 'd':
                                    v = new Value4d(na, new Vector4d(x, y, z, w));
                                    break;
                                case 'i':
                                    v = new Value4i(na, new Vector4i((int)x, (int)y, (int)z, (int)w));
                                    break;
                                case 'b':
                                    v = new Value4b(na, new Vector4b(x != 0, y != 0, z != 0, w != 0));
                                    break;
                                case 'v':
                                    if (type[9] == 'i')
                                        v = new Value4ui(na, new Vector4ui((uint)(x), (uint)(y), (uint)(z), (uint)(w)));
                                    break;
                                default:
                                    log.Error("Invalid type specifier '" + type + "'");
                                    throw new Exception();

                            }
                        }
                        else if (type.StartsWith("uniformMatrix"))
                        {
                            checkParameters(desc, f, "name,valueC,");
                            string value = f.GetAttribute("valueC");
                            if (value == "identity")
                            {
                                if (type.StartsWith("uniformMatrix2"))
                                {
                                    switch (type[14])
                                    {
                                        case 'd':
                                            {
                                                double[] m = new double[] { 1, 0, 0, 1 };
                                                v = new ValueMatrix2d(na, m);
                                                break;
                                            }
                                        case 'f':
                                            {
                                                float[] m = new float[] { 1.0f, 0.0f, 0.0f, 1.0f };
                                                v = new ValueMatrix2f(na, m);
                                                break;
                                            }
                                        default:
                                            log.Error("Unsupported matrix type '" + type + "'");
                                            throw new Exception();
                                    }
                                }
                                else if (type.StartsWith("uniformMatrix3"))
                                {
                                    switch (type[14])
                                    {
                                        case 'd':
                                            v = new ValueMatrix3d(na, Matrix3d.Identity);
                                            break;
                                        case 'f':
                                            v = new ValueMatrix3f(na, Matrix3f.Identity);
                                            break;
                                        default:
                                            log.Error("Unsupported matrix type '" + type + "'");
                                            throw new Exception();
                                    }
                                }
                                else if (type.StartsWith("uniformMatrix4"))
                                {
                                    switch (type[14])
                                    {
                                        case 'd':
                                            v = new ValueMatrix4d(na, Matrix4d.Identity);
                                            break;
                                        case 'f':
                                            v = new ValueMatrix4f(na, Matrix4f.Identity);
                                            break;
                                        default:
                                            log.Error("Unsupported matrix type '" + type + "'");
                                            throw new Exception();
                                    }
                                }
                                else
                                {
                                    log.Error("Unsupported matrix type or invalid valueC'" + type + "'");
                                    throw new Exception();
                                }
                            }
                            else
                            {
                                float[] valuesF = null;
                                double[] valuesD = null; // to avoid copying the buffer if type is double instead of float;
                                int nValues = -1;
                                bool isFloat = true;
                                if (type.StartsWith("uniformMatrix2x3"))
                                {
                                    nValues = 6;
                                    isFloat = type[16] != 'd';
                                }
                                else if (type.StartsWith("uniformMatrix2x4"))
                                {
                                    nValues = 8;
                                    isFloat = type[16] != 'd';
                                }
                                else if (type.StartsWith("uniformMatrix3x2"))
                                {
                                    nValues = 6;
                                    isFloat = type[16] != 'd';
                                }
                                else if (type.StartsWith("uniformMatrix3x4"))
                                {
                                    nValues = 12;
                                    isFloat = type[16] != 'd';
                                }
                                else if (type.StartsWith("uniformMatrix4x2"))
                                {
                                    nValues = 8;
                                    isFloat = type[16] != 'd';
                                }
                                else if (type.StartsWith("uniformMatrix4x3"))
                                {
                                    nValues = 12;
                                    isFloat = type[16] != 'd';
                                }
                                else if (type.StartsWith("uniformMatrix2"))
                                {
                                    nValues = 4;
                                    isFloat = type[14] != 'd';
                                }
                                else if (type.StartsWith("uniformMatrix3"))
                                {
                                    nValues = 9;
                                    isFloat = type[14] != 'd';
                                }
                                else if (type.StartsWith("uniformMatrix4"))
                                {
                                    nValues = 16;
                                    isFloat = type[14] != 'd';
                                }
                                else
                                {
                                    log.Error("Unsupported matrix type '" + type + "'");
                                    throw new Exception();
                                }

                                if (isFloat)
                                {
                                    valuesF = new float[nValues];
                                }
                                else
                                {
                                    valuesD = new double[nValues];
                                }
                                if (value == "zero")
                                {
                                    for (int i = 0; i < nValues; ++i)
                                    {
                                        if (isFloat)
                                        {
                                            valuesF[i] = 0.0f;
                                        }
                                        else
                                        {
                                            valuesD[i] = 0;
                                        }
                                    }
                                }
                                else
                                {
                                    int cpt = 0;
                                    if (!string.IsNullOrWhiteSpace(value))
                                        foreach (string val in value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                        {
                                            if (isFloat)
                                            {
                                                valuesF[cpt] = float.Parse(val, CultureInfo.InvariantCulture);
                                            }
                                            else
                                            {
                                                valuesD[cpt] = double.Parse(val, CultureInfo.InvariantCulture);
                                            }
                                            ++cpt;
                                        }
                                    if (cpt != nValues)
                                    {
                                        log.Error("Invalid matrix format '" + value + "'");
                                        throw new Exception();
                                    }
                                }

                                if (type.StartsWith("uniformMatrix2x3"))
                                {
                                    if (isFloat)
                                    {
                                        v = new ValueMatrix2x3f(na, valuesF);
                                    }
                                    else
                                    {
                                        v = new ValueMatrix2x3d(na, valuesD);
                                    }
                                }
                                else if (type.StartsWith("uniformMatrix2x4"))
                                {
                                    if (isFloat)
                                    {
                                        v = new ValueMatrix2x4f(na, valuesF);
                                    }
                                    else
                                    {
                                        v = new ValueMatrix2x4d(na, valuesD);
                                    }
                                }
                                else if (type.StartsWith("uniformMatrix3x2"))
                                {
                                    if (isFloat)
                                    {
                                        v = new ValueMatrix3x2f(na, valuesF);
                                    }
                                    else
                                    {
                                        v = new ValueMatrix3x2d(na, valuesD);
                                    }
                                }
                                else if (type.StartsWith("uniformMatrix3x4"))
                                {
                                    if (isFloat)
                                    {
                                        v = new ValueMatrix3x4f(na, valuesF);
                                    }
                                    else
                                    {
                                        v = new ValueMatrix3x4d(na, valuesD);
                                    }
                                }
                                else if (type.StartsWith("uniformMatrix4x2"))
                                {
                                    if (isFloat)
                                    {
                                        v = new ValueMatrix4x2f(na, valuesF);
                                    }
                                    else
                                    {
                                        v = new ValueMatrix4x2d(na, valuesD);
                                    }
                                }
                                else if (type.StartsWith("uniformMatrix4x3"))
                                {
                                    if (isFloat)
                                    {
                                        v = new ValueMatrix4x3f(na, valuesF);
                                    }
                                    else
                                    {
                                        v = new ValueMatrix4x3d(na, valuesD);
                                    }
                                }
                                else if (type.StartsWith("uniformMatrix2"))
                                {
                                    if (isFloat)
                                    {
                                        v = new ValueMatrix2f(na, valuesF);
                                    }
                                    else
                                    {
                                        v = new ValueMatrix3d(na, valuesD);
                                    }
                                }
                                else if (type.StartsWith("uniformMatrix3"))
                                {
                                    if (isFloat)
                                    {
                                        v = new ValueMatrix3f(na, valuesF);
                                    }
                                    else
                                    {
                                        v = new ValueMatrix3d(na, valuesD);
                                    }
                                }
                                else if (type.StartsWith("uniformMatrix4"))
                                {
                                    if (isFloat)
                                    {
                                        v = new ValueMatrix4f(na, valuesF);
                                    }
                                    else
                                    {
                                        v = new ValueMatrix4d(na, valuesD);
                                    }
                                }
                            }
                        }
                        else if (type == "uniformSampler")
                        {
                            checkParameters(desc, f, "name,texture,");
                            Texture t = (Texture)manager.loadResource(f.GetAttribute("texture")).get();
                            if (t == null)
                            {
                                log.Error("Cannot find '" + f.GetAttribute("texture") + "' texture");
                                throw new Exception();
                            }
                            v = new ValueSampler(UniformType.SAMPLER_2D, na, t);
                        }
                        else
                        {
                            log.Error("Unsupported type specifier '" + type + "'");
                            throw new Exception();
                        }
                        valueC.addInitialValue(v);
                    }
                    n = n.NextSibling;
                }

                string header = "";
                if (!string.IsNullOrWhiteSpace(e.GetAttribute("options")))
                {
                    string[] options = e.GetAttribute("options").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string option in options)
                    {
                        header = header + "#define " + option + "\n";
                    }

                }
                if (((string[])desc.getData()).Length > 1)
                {
                    string[] parts = (string[])desc.getData();
                    string head = header;
                    string vertex = string.IsNullOrWhiteSpace(parts[0]) ? null : parts[0];
                    string tessControl = string.IsNullOrWhiteSpace(parts[1]) ? null : parts[1];
                    string tessEval = string.IsNullOrWhiteSpace(parts[2]) ? null : parts[2];
                    string geometry = string.IsNullOrWhiteSpace(parts[3]) ? null : parts[3];
                    string fragment = string.IsNullOrWhiteSpace(parts[4]) ? null : parts[4];
                    this.valueC.init(version, head, vertex, head, tessControl, head, tessEval, head, geometry, head, fragment);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(e.GetAttribute("options")))
                    {
                        header += ((string[])desc.getData())[0];
                        this.valueC.init(version, header);
                    }
                    else
                    {
                        this.valueC.init(version, ((string[])desc.getData())[0]);
                    }
                }
                if (!string.IsNullOrWhiteSpace(e.GetAttribute("feedback")))
                {
                    bool interleaved;
                    if (e.GetAttribute("feedback") == "interleaved")
                    {
                        interleaved = true;
                    }
                    else
                    {
                        interleaved = false;
                    }
                    valueC.setFeedbackMode(interleaved);

                    string varyings = e.GetAttribute("varyings");
                    if (!string.IsNullOrWhiteSpace(varyings))
                        foreach (var varying in varyings.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            valueC.addFeedbackVarying(varying);
                        }

                }

                desc.clearData();
                log.Info("Compiled module '" + name + "'");
            }
            catch (Exception ex)
            {
                desc.clearData();
                throw ex;
            }
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
