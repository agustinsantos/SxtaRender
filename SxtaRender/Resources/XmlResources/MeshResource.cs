using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sxta.Render.Resources.XmlResources
{
    public class MeshResource : ResourceTemplate<MeshBuffers>
    {
        public static MeshResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new MeshResource(manager, name, desc, e);
        }

        public MeshResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(0, manager, name, desc)
        {
            try
            {
                using (StreamReader reader = new StreamReader(new MemoryStream((byte[])desc.getData())))
                {
                    this.valueC = new MeshBuffers();
                    string line;
                    line = reader.ReadLine();
                    string[] numbers = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    int xmin = Convert.ToInt32(numbers[0]);
                    int xmax = Convert.ToInt32(numbers[1]);
                    int ymin = Convert.ToInt32(numbers[2]);
                    int ymax = Convert.ToInt32(numbers[3]);
                    int zmin = Convert.ToInt32(numbers[4]);
                    int zmax = Convert.ToInt32(numbers[5]);
                    this.valueC.bounds = new Math.Box3f(xmin, xmax, ymin, ymax, zmin, zmax);

                    line = reader.ReadLine().Trim();
                    switch (line)
                    {
                        case "points":
                            this.valueC.mode = MeshMode.POINTS;
                            break;
                        case "lines":
                            this.valueC.mode = MeshMode.LINES;
                            break;
                        case "linesadjacency":
                            this.valueC.mode = MeshMode.LINES_ADJACENCY;
                            break;
                        case "linestrip":
                            this.valueC.mode = MeshMode.LINE_STRIP;
                            break;
                        case "linestripadjacency":
                            this.valueC.mode = MeshMode.LINE_STRIP_ADJACENCY;
                            break;
                        case "triangles":
                            this.valueC.mode = MeshMode.TRIANGLES;
                            break;
                        case "trianglesadjacency":
                            this.valueC.mode = MeshMode.TRIANGLES_ADJACENCY;
                            break;
                        case "trianglestrip":
                            this.valueC.mode = MeshMode.TRIANGLE_STRIP;
                            break;
                        case "trianglefan":
                            this.valueC.mode = MeshMode.TRIANGLE_FAN;
                            break;
                        default:
                            throw new Exception("Invalid mesh topology '" + line + "'");
                    }
                    line = reader.ReadLine();
                    int attributeCount = Convert.ToInt32(line);
                    int vertexSize = 0;
                    int[] attributeIds = new int[attributeCount];
                    int[] attributeComponents = new int[attributeCount];
                    AttributeType[] attributeTypes = new AttributeType[attributeCount];
                    bool[] attributeNorms = new bool[attributeCount];

                    for (int i = 0; i < attributeCount; ++i)
                    {
                        line = reader.ReadLine();
                        string[] attrs = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        attributeIds[i] = Convert.ToInt32(attrs[0]);
                        attributeComponents[i] = Convert.ToInt32(attrs[1]);
                        switch (attrs[2])
                        {
                            case "byte":
                                attributeTypes[i] = AttributeType.A8I;
                                vertexSize += attributeComponents[i] * 1;
                                break;
                            case "ubyte":
                                attributeTypes[i] = AttributeType.A8UI;
                                vertexSize += attributeComponents[i] * 1;
                                break;
                            case "short":
                                attributeTypes[i] = AttributeType.A16I;
                                vertexSize += attributeComponents[i] * 2;
                                break;
                            case "ushort":
                                attributeTypes[i] = AttributeType.A16UI;
                                vertexSize += attributeComponents[i] * 2;
                                break;
                            case "int":
                                attributeTypes[i] = AttributeType.A32I;
                                vertexSize += attributeComponents[i] * 4;
                                break;
                            case "uint":
                                attributeTypes[i] = AttributeType.A32UI;
                                vertexSize += attributeComponents[i] * 4;
                                break;
                            case "float":
                                attributeTypes[i] = AttributeType.A32F;
                                vertexSize += attributeComponents[i] * 4;
                                break;
                            case "double":
                                attributeTypes[i] = AttributeType.A64F;
                                vertexSize += attributeComponents[i] * 8;
                                break;
                            default:
                                throw new Exception("Invalid mesh vertex component type '" + attrs[2] + "'");
                        }
                        switch (attrs[3])
                        {
                            case "true":
                                attributeNorms[i] = true;
                                break;
                            case "false":
                                attributeNorms[i] = false;
                                break;
                            default:
                                throw new Exception("Invalid mesh vertex normalization '" + attrs[3] + "'");
                        }
                    }
                    for (int i = 0; i < attributeCount; ++i)
                    {
                        this.valueC.addAttributeBuffer(attributeIds[i], attributeComponents[i],
                                                    vertexSize, attributeTypes[i], attributeNorms[i]);
                    }
                    int vertexCount;
                    line = reader.ReadLine();
                    vertexCount = Convert.ToInt32(line);
                    this.valueC.nvertices = vertexCount;
                    byte[] vertexBuffer = new byte[vertexCount * vertexSize];
                    int offset = 0;
                    for (int i = 0; i < vertexCount; ++i)
                    {
                        line = reader.ReadLine();
                        string[] vals = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        int cnt = 0;
                        for (int j = 0; j < attributeCount; ++j)
                        {
                            AttributeBuffer ab = this.valueC.getAttributeBuffer(j);
                            for (int k = 0; k < ab.getSize(); ++k)
                            {
                                switch (ab.getType())
                                {
                                    case AttributeType.A8I:
                                    case AttributeType.A8UI:
                                        {
                                            byte ic;
                                            ic = Byte.Parse(vals[cnt]);
                                            vertexBuffer[offset] = ic;
                                            offset += sizeof(byte);
                                            break;
                                        }
                                    case AttributeType.A16I:
                                        {
                                            short s;
                                            s = short.Parse(vals[cnt]);
                                            Array.Copy(BitConverter.GetBytes(s), 0, vertexBuffer, offset, sizeof(short));
                                            offset += sizeof(short);
                                            break;
                                        }
                                    case AttributeType.A16UI:
                                        {
                                            ushort us;
                                            us = ushort.Parse(vals[cnt]);
                                            Array.Copy(BitConverter.GetBytes(us), 0, vertexBuffer, offset, sizeof(ushort));
                                            offset += sizeof(ushort);
                                            break;
                                        }
                                    case AttributeType.A32I:
                                        {
                                            int si;
                                            si = int.Parse(vals[cnt]);
                                            Array.Copy(BitConverter.GetBytes(si), 0, vertexBuffer, offset, sizeof(int));
                                            offset += sizeof(int);
                                            break;
                                        }
                                    case AttributeType.A32UI:
                                        {
                                            uint ui;
                                            ui = uint.Parse(vals[cnt]);
                                            Array.Copy(BitConverter.GetBytes(ui), 0, vertexBuffer, offset, sizeof(uint));
                                            offset += sizeof(uint);
                                            break;
                                        }
                                    case AttributeType.A32F:
                                        {
                                            float f;
                                            f = float.Parse(vals[cnt], CultureInfo.InvariantCulture);
                                            Array.Copy(BitConverter.GetBytes(f), 0, vertexBuffer, offset, sizeof(float));
                                            offset += sizeof(float);
                                            break;
                                        }
                                    case AttributeType.A64F:
                                        {
                                            double d;
                                            d = double.Parse(vals[cnt], CultureInfo.InvariantCulture);
                                            Array.Copy(BitConverter.GetBytes(d), 0, vertexBuffer, offset, sizeof(double));
                                            offset += sizeof(double);
                                            break;
                                        }

                                    // not handled (don't know why)
                                    case AttributeType.A16F:
                                    case AttributeType.A32I_2_10_10_10_REV:
                                    case AttributeType.A32UI_2_10_10_10_REV:
                                    case AttributeType.A32I_FIXED:
                                    default:
                                        {
                                            Debug.Assert(false); // unsupported
                                            break;
                                        }
                                }
                                cnt++;
                            }
                        }
                    }
                    gpub = new GPUBuffer();
                    gpub.setData(vertexCount * vertexSize, vertexBuffer, BufferUsage.STATIC_DRAW);
                    for (int i = 0; i < this.valueC.getAttributeCount(); ++i)
                    {
                        this.valueC.getAttributeBuffer(i).setBuffer(gpub);
                    }
                    int indiceCount;
                    line = reader.ReadLine();
                    indiceCount = int.Parse(line);
                    this.valueC.nindices = indiceCount;

                    if (this.valueC.nindices > 0)
                    {
                        int indiceSize;
                        AttributeType type;
                        if (vertexCount < 256)
                        {
                            indiceSize = 1; //sizeof(byte)
                            type = AttributeType.A8UI;
                        }
                        else if (vertexCount < 65536)
                        {
                            indiceSize = 2; // sizeof(ushort)
                            type = AttributeType.A16UI;
                        }
                        else
                        {
                            indiceSize = 4; // sizeof(uint)
                            type = AttributeType.A32UI;
                        }

                        gpubindex = new GPUBuffer();
                        offset = 0;

                        line = reader.ReadLine();
                        string[] vals = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        int cnt = 0;

                        if (indiceSize == 1)
                        {
                            byte[] indiceBuffer = new byte[indiceCount];
                            for (int i = 0; i < indiceCount; ++i)
                            {
                                while (cnt > vals.Length - 1)
                                {
                                    line = reader.ReadLine();
                                    vals = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    cnt = 0;
                                }
                                indiceBuffer[i] = byte.Parse(vals[cnt++], CultureInfo.InvariantCulture);
                            }
                            gpubindex.setData(indiceCount * indiceSize, indiceBuffer, BufferUsage.STATIC_DRAW);
                        }
                        else if (indiceSize == 2)
                        {
                            ushort[] indiceBuffer = new ushort[indiceCount];
                            for (int i = 0; i < indiceCount; ++i)
                            {
                                while (cnt > vals.Length - 1)
                                {
                                    line = reader.ReadLine();
                                    vals = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    cnt = 0;
                                }
                                indiceBuffer[i] = ushort.Parse(vals[cnt++], CultureInfo.InvariantCulture);
                            }
                            gpubindex.setData(indiceCount * indiceSize, indiceBuffer, BufferUsage.STATIC_DRAW);
                        }
                        else
                        {
                            uint[] indiceBuffer = new uint[indiceCount];
                            for (int i = 0; i < indiceCount; ++i)
                            {
                                while (cnt > vals.Length - 1)
                                {
                                    line = reader.ReadLine();
                                    vals = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    cnt = 0;
                                }
                                indiceBuffer[i] = uint.Parse(vals[cnt++], CultureInfo.InvariantCulture);
                            }
                            gpubindex.setData(indiceCount * indiceSize, indiceBuffer, BufferUsage.STATIC_DRAW);
                        }
                        this.valueC.setIndicesBuffer(new AttributeBuffer(0, 1, type, false, gpubindex));
                    }
                }
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
            if (gpub != null)
                gpub.Dispose();
            if (gpubindex != null)
                gpubindex.Dispose();
        }
        GPUBuffer gpub;
        GPUBuffer gpubindex;
    }
}
