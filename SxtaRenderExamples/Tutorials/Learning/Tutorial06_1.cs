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
    /// Demonstrates how Debuging Normals
    /// </summary>
    [Example("Example 6.1: Debuging Normals", ExampleCategory.Learning, "6. Lighting", 1, Source = "Tutorial06_1", Documentation = "Tutorial06_1")]
    public class TutorialLearning06_1 : GameWindow
    {
        public TutorialLearning06_1()
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
            // position the camera 
            camera = new FirstPersonCamera(this);
            camera.Position = new Vector3f(0, 0, -5);

            // fovy, aspect, zNear, zFar
            Matrix4f projection = Matrix4f.CreatePerspectiveFieldOfView((float)MathHelper.ToRadians(60), (float)this.Width / (float)this.Height, 0.01f, 100.0f);
            uPMatrix.set(projection);

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

        /// <summary>
        /// Add your game logic here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            camera.Update((float)e.Time);
            angle += 0.01f;
            mat = Matrix4f.CreateRotationZ(-angle) * camera.Matrix;
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
        private FirstPersonCamera camera;
        float angle = 0;

        const string FRAGMENT_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec3 aNormal;

        uniform mat4 uMVMatrix;
        uniform mat4 uPMatrix;

        out vec3 vColor;

        void main()
        {
            gl_Position = uPMatrix * uMVMatrix * vec4(aPosition, 1.0);

            // We use normal vectors as (fake) color.
            // in later examples we will use normals for proper ilumination
            vColor = normalize(aNormal);
        }
#endif
#ifdef _FRAGMENT_
        in vec3 vColor;
        out vec3 FragColor;
 
        void main()
        {
            FragColor =  vColor; 
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
            using (TutorialLearning06_1 example = new TutorialLearning06_1())
            {
                example.Run(30.0, 10.0);
            }
        }

        #endregion
    }


}
