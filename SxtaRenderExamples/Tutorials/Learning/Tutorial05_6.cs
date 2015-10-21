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
using MathHelper = Sxta.Math.MathHelper;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates how to mix several textures
    /// </summary>
    [Example("Example 5.6: Mix Textures", ExampleCategory.Learning, "5. Textures", 1, Source = "Tutorial05_6", Documentation = "Tutorial05_6")]
    public class TutorialLearning05_6 : GameWindow
    {
        public TutorialLearning05_6()
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
            fb.setDepthTest(true);

            p = new Program(new Module(330, FRAGMENT_SHADER));
            uMVMatrix = p.getUniformMatrix4f("uMVMatrix");

            UniformMatrix4f uPMatrix = p.getUniformMatrix4f("uPMatrix");
            // fovy, aspect, zNear, zFar
            Matrix4f projection = Matrix4f.CreatePerspectiveFieldOfView((float)MathHelper.ToRadians(60), (float)this.Width / (float)this.Height, 0.01f, 100.0f);
            uPMatrix.set(projection);

            Bitmap texture1 = new Bitmap("Resources/Textures/baserock.jpg");
            t1 = CreateTexture(texture1);
            Bitmap texture2 = new Bitmap("Resources/Textures/nicegrass.jpg");
            t2 = CreateTexture(texture2);
            Bitmap texture3 = new Bitmap("Resources/Textures/darkrockalpha.png");
            t3 = CreateTexture(texture3);
            uSampler1 = p.getUniformSampler("uSampler1");
            uSampler2 = p.getUniformSampler("uSampler2");
            uSampler3 = p.getUniformSampler("uSampler3");
            uSampler1.set(t1);
            uSampler2.set(t2);
            uSampler3.set(t3);


            mesh1 = new Mesh<Vertex_V3T2f, uint>(Vertex_V3T2f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLE_STRIP, MeshUsage.GPU_STATIC);
            mesh1.addAttributeType(0, 3, AttributeType.A32F, false);
            mesh1.addAttributeType(1, 2, AttributeType.A32F, false);

            // Front
            mesh1.addVertex(new Vertex_V3T2f() { Position = new Vector3f(-1, -1, 0), TexCoord = new Vector2f(0, 0) });
            mesh1.addVertex(new Vertex_V3T2f() { Position = new Vector3f(1, -1, 0), TexCoord = new Vector2f(1, 0) });
            mesh1.addVertex(new Vertex_V3T2f() { Position = new Vector3f(-1, 1, 0), TexCoord = new Vector2f(0, 1) });
            mesh1.addVertex(new Vertex_V3T2f() { Position = new Vector3f(1, 1, 0), TexCoord = new Vector2f(1, 1) });
            //mesh1.addIndices(new uint[] {0, 1, 2, 2, 1, 3 });

            fb.setClearColor(Color.White);
        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            if (p != null)
                p.Dispose();
            if (mesh1 != null)
                mesh1.Dispose();
            if (t1 != null)
                t1.Dispose();
            if (t2 != null)
                t2.Dispose();
            if (t3 != null)
                t3.Dispose();
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
            fb.setViewport(new Vector4i(0, 0, Width, Height));
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
            fb.clear(true, false, true);

            Matrix4f camera = Matrix4f.CreateTranslation(0.0f, 0.0f, -3.0f);

            mat = /*Matrix4f.CreateTranslation(0.0f, 0.0f, 0.0f) */ camera;
            uMVMatrix.set(mat);
            fb.draw(p, mesh1);

            this.SwapBuffers();
        }

        #endregion

        public Texture CreateTexture(Bitmap img, TextureFilter texFilter = TextureFilter.LINEAR)
        {
            TextureInternalFormat pif;
            TextureFormat pf;
            Sxta.Render.PixelType pt;
            int size;
            EnumConversion.ConvertPixelFormat(img.PixelFormat, out pif, out pf, out pt, out size);
            img.RotateFlip(RotateFlipType.RotateNoneFlipY);
            BitmapData Data = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            using (GPUBuffer texbuff = new GPUBuffer())
            {
                texbuff.setData(Data.Width * Data.Height * size, Data.Scan0, BufferUsage.STATIC_DRAW);
                img.UnlockBits(Data);
                Texture.Parameters texParams = new Texture.Parameters();
                texParams.min(texFilter);
                texParams.mag(texFilter);
                Sxta.Render.Buffer.Parameters s = new Sxta.Render.Buffer.Parameters();
                Texture texture = new Texture2D(img.Width, img.Height, pif, pf, pt, texParams, s, texbuff);
                return texture;
            }
        }

        #region Fields
        FrameBuffer fb;
        Program p;
        Mesh<Vertex_V3T2f, uint> mesh1;
        Matrix4f mat;
        UniformMatrix4f uMVMatrix;
        Texture t1, t2, t3;
        UniformSampler uSampler1, uSampler2, uSampler3;

        const string FRAGMENT_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec2 aTexCoord;

        uniform mat4 uMVMatrix;
        uniform mat4 uPMatrix;

        out vec2 TexCoord;

        void main()
        {
            gl_Position = uPMatrix * uMVMatrix * vec4(aPosition, 1.0);
            TexCoord = aTexCoord;
        }
#endif
#ifdef _FRAGMENT_
        in vec2 TexCoord;
        uniform sampler2D uSampler1;
        uniform sampler2D uSampler2;
        uniform sampler2D uSampler3;

        out vec4 FragColor;

        void main()
        {
            vec4 texColor1 = texture(uSampler1, TexCoord);
            vec4 texColor2 = texture(uSampler2, TexCoord);
            vec4 texColor3 = texture(uSampler3, TexCoord);
            FragColor = mix(texColor1, texColor2, 1.0 - texColor3.r);
        }
#endif";

        #endregion

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (TutorialLearning05_6 example = new TutorialLearning05_6())
            {
                example.Run(30.0, 10.0);
            }
        }

        #endregion

        private struct Vertex_V3T2f
        {
            public Vector3f Position;
            public Vector2f TexCoord;
            public static int SizeInBytes
            {
                get { return Vector3f.SizeInBytes + Vector2f.SizeInBytes; }
            }
        }
    }


}
