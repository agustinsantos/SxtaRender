using Sxta.Math;
using Sxta.Render;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SxtaRender.Fonts
{
    /// <summary>
    /// in vec2 a_pos;
    /// in vec2 a_offset;
    /// in vec4 a_data1;
    /// in vec4 a_data2;
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SdfVertexStruct
    {
        public Vector2f Position;
        public Vector2f Offset;
        public Vector4f Data1;
        public Vector4f Data2;
        public static int SizeInBytes
        {
            get { return 2 * Vector2f.SizeInBytes + 2 * Vector4f.SizeInBytes; }
        }
        public static SdfVertexStruct NewVertex(Vector2f pos, Vector2f offset, Vector2f tex, float minzoom = 1, float maxzoom = 25, float labelminzoom = 16)
        {
            SdfVertexStruct rst = new SdfVertexStruct()
            {
                Position = pos, /* posx and posy */
                Offset = new Vector2f((float)Math.Round(offset.X * 64), (float)Math.Round(offset.Y * 64)), // use 1/64 pixels for placement
                Data1 = new Vector4f(tex.X, tex.Y, /* tex */
                                     labelminzoom * 10, 0), /* labelminzoom */
                Data2 = new Vector4f(minzoom * 10,  // minzoom, 1/10 zoom levels: z16 == 160.
                                     Math.Min(maxzoom, 25) * 10, // maxzoom, 1/10 zoom levels: z16 == 160.
                                     minzoom * 10, Math.Min(maxzoom, 25) * 10)
            };
            return rst;
        }

        public static SdfVertexStruct NewVertex(float x, float y, float ox, float oy, short tx, short ty, float minzoom = 1, float maxzoom = 25, float labelminzoom = 16)
        {
            SdfVertexStruct rst = new SdfVertexStruct()
            {
                Position = new Vector2f(x, y), /* posx and posy */
                Offset = new Vector2f((float)Math.Round(ox * 64), (float)Math.Round(oy * 64)), // use 1/64 pixels for placement
                //Data1 = new Vector4f(tx / 4, ty / 4, /* tex */
                //                     labelminzoom * 10, 0), /* labelminzoom */
                Data1 = new Vector4f(tx, ty, /* tex */
                                     labelminzoom * 10, 0), /* labelminzoom */
                Data2 = new Vector4f(minzoom * 10,  // minzoom, 1/10 zoom levels: z16 == 160.
                                     Math.Min(maxzoom, 25) * 10, // maxzoom, 1/10 zoom levels: z16 == 160.
                                     minzoom * 10, Math.Min(maxzoom, 25) * 10)
            };
            return rst;
        }

        public static readonly AttributeBuffer[] Attributes = new AttributeBuffer[4] {
                new AttributeBuffer(0, 2, AttributeType.A32F, false, null),
                new AttributeBuffer(1, 2, AttributeType.A32F, false, null),
                new AttributeBuffer(2, 4, AttributeType.A32F, false, null),
                new AttributeBuffer(3, 4, AttributeType.A32F, false, null),
        };

        public static AttributeBuffer[] GetAttributes()
        {
            return Attributes;
        }

    }
    public class SdfUniformStruct
    {
        public Matrix4f Matrix;
        public Matrix4f ExMatrix;
        public float Zoom;
        public float Fadedist;
        public float Minfadezoom;
        public float Maxfadezoom;
        public float Fadezoom;
        public bool Skewed;
        public Vector2f TexSize;
        public int TextureId;
        public Vector4f Color;
        public float Buffer;
        public float Gamma;

        public void Set(SdfShader shader)
        {
            shader.Matrix = Matrix;
            shader.ExMatrix = ExMatrix;
            shader.Zoom = Zoom;
            shader.Fadedist = Fadedist;
            shader.Minfadezoom = Minfadezoom;
            shader.Maxfadezoom = Maxfadezoom;
            shader.Fadezoom = Fadezoom;
            shader.Skewed = Skewed;
            shader.TexSize = TexSize;
            shader.Color = Color;
            shader.Buffer = Buffer;
            shader.Gamma = Gamma;
        }
    }


    public class SdfShader : IDisposable
    {


        public const string DIR_PATH = @"Resources/Shaders";
        public const string VERTEX_FILENAME = @"sdfvertex.glsl";
        public const string FRAGMENT_FILENAME = @"sdffragment.glsl";

        private Program p;
        private UniformMatrix4f uMatrix;
        private UniformMatrix4f uExmatrix;
        private Uniform1f uZoom;
        private Uniform1f uFadedist;
        private Uniform1f uMinfadezoom;
        private Uniform1f uMaxfadezoom;
        private Uniform1f uFadezoom;
        private Uniform1b uSkewed;
        private Uniform2f uTexSize;
        private UniformSampler uTexture;
        private Uniform4f uColor;
        private Uniform1f uBuffer;
        private Uniform1f uGamma;


        public void Init()
        {
            string vertexShader = File.ReadAllText(DIR_PATH + Path.DirectorySeparatorChar + VERTEX_FILENAME);
            string fragmentShader = File.ReadAllText(DIR_PATH + Path.DirectorySeparatorChar + FRAGMENT_FILENAME);
            p = new Program(new Module(330, vertexShader, fragmentShader));
            uMatrix = p.getUniformMatrix4f("u_matrix");
            uExmatrix = p.getUniformMatrix4f("u_exmatrix");
            uZoom = p.getUniform1f("u_zoom");
            uFadedist = p.getUniform1f("u_fadedist");
            uMinfadezoom = p.getUniform1f("u_minfadezoom");
            uMaxfadezoom = p.getUniform1f("u_maxfadezoom");
            uFadezoom = p.getUniform1f("u_fadezoom");
            uSkewed = p.getUniform1b("u_skewed");
            uTexture = p.getUniformSampler("u_texture");
            uTexSize = p.getUniform2f("u_texsize");
            uColor = p.getUniform4f("u_color");
            uBuffer = p.getUniform1f("u_buffer");
            uGamma = p.getUniform1f("u_gamma");
        }
        public Program Program
        {
            get
            {
                return p;
            }
        }

        public Matrix4f Matrix
        {
            set
            {
                uMatrix.set(value);
            }
        }
        public Matrix4f ExMatrix
        {
            set
            {
                uExmatrix.set(value);
            }
        }

        public float Zoom
        {
            set
            {
                uZoom.set(value);
            }
        }
        public float Fadedist
        {
            set
            {
                uFadedist.set(value);
            }
        }
        public float Minfadezoom
        {
            set
            {
                uMinfadezoom.set(value);
            }
        }

        public float Maxfadezoom
        {
            set
            {
                uMaxfadezoom.set(value);
            }
        }

        public float Fadezoom
        {
            set
            {
                uFadezoom.set(value);
            }
        }
        public bool Skewed
        {
            set
            {
                uSkewed.set(value);
            }
        }

        public Vector2f TexSize
        {
            set
            {
                uTexSize.set(value);
            }
        }

        public Texture Texture
        {
            set
            {
                uTexture.set(value);
            }
        }
        public Vector4f Color
        {
            set
            {
                uColor.set(value);
            }
        }
        public float Buffer
        {
            set
            {
                uBuffer.set(value);
            }
        }
        public float Gamma
        {
            set
            {
                uGamma.set(value);
            }
        }
        public void Dispose()
        {
            if (p != null) p.Dispose();
        }
    }
}
