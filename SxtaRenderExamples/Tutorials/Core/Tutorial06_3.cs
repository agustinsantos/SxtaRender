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
    [Example("Example 6.3: DDS Texture Formats", ExampleCategory.CoreUsage, "6. Textures", 1, Source = "Tutorial06_3", Documentation = "Tutorial-TODO")]
    public class Tutorial06_3 : GameWindow
    {
        public Tutorial06_3()
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
            // For a complete suite of DDS images, see where??
            Bitmap texture1 = new Bitmap("Resources/Texture-512x512-RGB.dds");
            Bitmap texture2 = new Bitmap("Resources/Texture-512x512-RGBA.dds");
            Bitmap texture3 = new Bitmap("Resources/Texture-512x512-R5G6B5.dds");
            t1 = CreateTexture(texture1);
            t2 = CreateTexture(texture2);
            t3 = CreateTexture(texture3);
            quad = new Mesh<Vertex17_V3T2f, uint>(Vertex17_V3T2f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC, 4, 6);
            quad.addAttributeType(0, 3, AttributeType.A32F, false);
            quad.addAttributeType(1, 2, AttributeType.A32F, false);
            quad.addVertex(new Vertex17_V3T2f() { Position = new Vector3f(-0.9f, -0.9f, 0), TexCoord = new Vector2f(0, 0) });
            quad.addVertex(new Vertex17_V3T2f() { Position = new Vector3f(-0.1f, -0.9f, 0), TexCoord = new Vector2f(1, 0) });
            quad.addVertex(new Vertex17_V3T2f() { Position = new Vector3f(-0.9f, -0.1f, 0), TexCoord = new Vector2f(0, 1) });
            quad.addVertex(new Vertex17_V3T2f() { Position = new Vector3f(-0.1f, -0.1f, 0), TexCoord = new Vector2f(1, 1) });
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
            if (t1 != null)
                t1.Dispose();
            if (t2 != null)
                t2.Dispose();
            if (t3 != null)
                t3.Dispose();
            if (buff != null)
                for (int i = 0; i < 3; i++)
                    buff[i].Dispose();
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
            Uniform3f translation = p.getUniform3f("Translation");
            fb.clear(true, true, true);
            p.getUniformSampler("gSampler").set(t1);
            translation.set(new Vector3f(0, 0, 0));
            fb.draw(p, m, MeshMode.TRIANGLES, 0, 6);

            p.getUniformSampler("gSampler").set(t2);
            translation.set(new Vector3f(0, 1, 0));
            fb.draw(p, m, MeshMode.TRIANGLES, 0, 6);

            p.getUniformSampler("gSampler").set(t3);
            translation.set(new Vector3f(1, 0, 0));
            fb.draw(p, m, MeshMode.TRIANGLES, 0, 6);

            this.SwapBuffers();
        }

        #endregion

        #region Fields
        FrameBuffer fb;
        Program p;
        Mesh<Vertex17_V3T2f, uint> quad;
        MeshBuffers m;
        Texture t1, t2, t3;
        GPUBuffer[] buff = new GPUBuffer[3];

        const string EXAMPLE_SHADER =
@"#ifdef _VERTEX_
        layout (location = 0) in vec3 Position;
        layout (location = 1) in vec2 TexCoord;
        
        uniform vec3 Translation;
        out vec2 TexCoord0;

        void main()
        {
            gl_Position = vec4(Position + Translation, 1.0);
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
        
        private int buffCount = 0;
        public Texture CreateTexture(Bitmap img)
        {
            TextureInternalFormat pif;
            TextureFormat pf;
            Sxta.Render.PixelType pt;
            int size;
            EnumConversion.ConvertPixelFormat(img.PixelFormat, out pif, out pf, out pt, out size);
            img.RotateFlip(RotateFlipType.RotateNoneFlipY);
            BitmapData Data = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            buff[buffCount] = new GPUBuffer();
            buff[buffCount].setData(Data.Width * Data.Height * size, Data.Scan0, BufferUsage.STATIC_DRAW);
            img.UnlockBits(Data);
            Texture.Parameters @params = new Texture.Parameters();
            Sxta.Render.Buffer.Parameters s = new Sxta.Render.Buffer.Parameters();
            Texture texture = new Texture2D(img.Width, img.Height, pif, pf, pt, @params, s, buff[buffCount]);
            buffCount++;
            return texture;
        }

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (Tutorial06_3 example = new Tutorial06_3())
            {
                example.Run(30.0, 0.0);
            }
        }

        #endregion
    }


    public struct Vertex17_V3T2f
    {
        public Vector3f Position;
        public Vector2f TexCoord;
        public static int SizeInBytes
        {
            get { return Vector2f.SizeInBytes + Vector3f.SizeInBytes; }
        }
    }
}
