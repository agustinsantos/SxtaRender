// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates the GameWindow class.
    /// </summary>
    [Example("Example 6.1: Basic Texture Mapping", ExampleCategory.CoreUsage, "6. Textures", 1, Source = "Tutorial06_1", Documentation = "Tutorial-TODO")]
    public class Tutorial06_1 : GameWindow
    {
        public Tutorial06_1()
            : base(600, 600)
        {
            Keyboard.KeyDown += Keyboard_KeyDown;
        }

        #region Keyboard_KeyDown

        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        /// <param name="sender">The KeyboardDevice which generated this event.</param>
        /// <param name="e">The key that was pressed.</param>
        void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Exit();

            if (e.Key == Key.F11)
                if (this.WindowState == WindowState.Fullscreen)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Fullscreen;
        }

        #endregion

        #region OnLoad

        /// <summary>
        /// Setup OpenGL and load resources here.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            fb = new FrameBuffer(true);
            p = new Program(new Module(330, EXAMPLE_SHADER));
            Bitmap texture = new Bitmap("Resources/Texture-512x512-RGBA.bmp");
            t = CreateTexture(texture);
            p.getUniformSampler("gSampler").set(t);
            quad = new Mesh<Vertex_V3T2f, uint>(Vertex_V3T2f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC, 4, 6);
            quad.addAttributeType(0, 3, AttributeType.A32F, false);
            quad.addAttributeType(1, 2, AttributeType.A32F, false);
            quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(-1, -1, 0), TexCoord = new Vector2f(0, 0) });
            quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(1, -1, 0), TexCoord = new Vector2f(1, 0) });
            quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(-1, 1, 0), TexCoord = new Vector2f(0, 1) });
            quad.addVertex(new Vertex_V3T2f() { Position = new Vector3f(1, 1, 0), TexCoord = new Vector2f(1, 1) });
            quad.addIndice(0);
            quad.addIndice(1);
            quad.addIndice(2);
            quad.addIndice(2);
            quad.addIndice(1);
            quad.addIndice(3);
            m = quad.getBuffers();
        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            if (t != null)
                t.Dispose();
            if (buff != null)
                buff.Dispose();
            if (p != null)
                p.Dispose();
            if (quad != null)
                quad.Dispose();
            if (fb != null)
                fb.Dispose();
            base.OnUnload(e);
        }

        #endregion

        #region OnResize

        /// <summary>
        /// Respond to resize events here.
        /// </summary>
        /// <param name="e">Contains information on the new GameWindow size.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnResize(EventArgs e)
        {
            // GL.Viewport(0, 0, Width, Height);
            fb.setViewport(new Vector4i(0, 0, Width, Height));

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
        }

        #endregion

        #region OnUpdateFrame

        /// <summary>
        /// Add your game logic here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Nothing to do!
        }

        #endregion

        #region OnRenderFrame

        /// <summary>
        /// Add your game rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            fb.clear(true, true, true);
            fb.draw(p, m, MeshMode.TRIANGLES, 0, 6);
            this.SwapBuffers();
        }

        #endregion

        #region Fields
        FrameBuffer fb;
        Program p;
        Mesh<Vertex_V3T2f, uint> quad;
        MeshBuffers m;
        Texture t;
        GPUBuffer buff;

        const string EXAMPLE_SHADER =
@"#ifdef _VERTEX_
        layout (location = 0) in vec3 Position;
        layout (location = 1) in vec2 TexCoord;

        out vec2 TexCoord0;

        void main()
        {
            gl_Position = vec4(Position*0.8, 1.0);
            TexCoord0 = TexCoord;
        }
#endif
#ifdef _FRAGMENT_
        in vec2 TexCoord0;
        out vec4 FragColor;
        uniform sampler2D gSampler;
 
        void main()
        {
            FragColor =  texture2D(gSampler, TexCoord0.xy);
        }
#endif";


        #endregion

        public Texture CreateTexture(Bitmap img)
        {
            TextureInternalFormat pif;
            TextureFormat pf;
            Sxta.Render.PixelType pt;
            int size;
            EnumConversion.ConvertPixelFormat(img.PixelFormat, out pif, out pf, out pt, out size);
            img.RotateFlip(RotateFlipType.RotateNoneFlipY);
            BitmapData Data = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            buff = new GPUBuffer();
            buff.setData(Data.Width * Data.Height * size, Data.Scan0, BufferUsage.STATIC_DRAW);
            img.UnlockBits(Data);
            Texture.Parameters @params = new Texture.Parameters();
            Sxta.Render.Buffer.Parameters s = new Sxta.Render.Buffer.Parameters();
            Texture texture = new Texture2D(img.Width, img.Height, pif, pf, pt, @params, s, buff);
            return texture;
        }

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (Tutorial06_1 example = new Tutorial06_1())
            {
                example.Run(30.0, 0.0);
            }
        }

        #endregion



        private struct Vertex_V3T2f
        {
            public Vector3f Position;
            public Vector2f TexCoord;
            public static int SizeInBytes
            {
                get { return Vector2f.SizeInBytes + Vector3f.SizeInBytes; }
            }
        }
    }
}
