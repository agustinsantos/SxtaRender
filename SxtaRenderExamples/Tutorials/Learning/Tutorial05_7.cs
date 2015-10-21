// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using Sxta.Render.OpenGLExt;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using MathHelper = Sxta.Math.MathHelper;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates how to texture a Sphere
    /// </summary>
    [Example("Example 5.6: Texturing a Sphere", ExampleCategory.Learning, "5. Textures", 1, Source = "Tutorial05_7", Documentation = "Tutorial05_7")]
    public class TutorialLearning05_7 : GameWindow
    {
        public TutorialLearning05_7()
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

            Bitmap texture1 = new Bitmap("Resources/Textures/Earthmap720x360_grid.jpg");
            t1 = CreateTexture(texture1);
            uSampler = p.getUniformSampler("uSampler");
            uSampler.set(t1);


            mesh1 = MeshUtils.GenerateSolidSphere(1.0f, 40, 40);

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

        #region OnUpdateFrame

        Matrix4f MV = Matrix4f.CreateRotationX(Math.PI / 2) * Matrix4f.CreateTranslation(0.0f, -0.3f, -3.0f);

        /// <summary>
        /// Add your game logic here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            angle += 0.01f;

            mat = Matrix4f.CreateRotationZ(-angle) * MV;
            uMVMatrix.set(mat);
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
        Mesh<Vertex_V3N3T2f, ushort> mesh1;
        Matrix4f mat;
        UniformMatrix4f uMVMatrix;
        Texture t1;
        UniformSampler uSampler;
        float angle = 0;

        const string FRAGMENT_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition;

        uniform mat4 uMVMatrix;
        uniform mat4 uPMatrix;

        out vec4 TexCoord;

        void main()
        {
            gl_Position = uPMatrix * uMVMatrix * vec4(aPosition, 1.0);
            TexCoord = vec4(aPosition, 1.0);
        }
#endif
#ifdef _FRAGMENT_
        in vec4 TexCoord;
        uniform sampler2D uSampler;

        out vec4 FragColor;

        void main()
        {
            // processing of the texture coordinates;
            // this is unnecessary if correct texture coordinates are specified by the application
            vec2 longitudeLatitude = vec2((atan(TexCoord.y, TexCoord.x) / 3.1415926 + 1.0) * 0.5,
                                          (asin(TexCoord.z) / 3.1415926 + 0.5));

            // look up the color of the texture image specified by the uniform uSampler
            // at the position specified by longitudeLatitude.x and
            // longitudeLatitude.y and return it in FragColor
            FragColor = texture(uSampler, longitudeLatitude);
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
            using (TutorialLearning05_7 example = new TutorialLearning05_7())
            {
                example.Run(30.0, 10.0);
            }
        }

        #endregion
    }


}
