// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using System;
using System.Drawing;
using MathHelper = Sxta.Math.MathHelper;

namespace Examples.Tutorials
{
    /// <summary>
    /// Demonstrates how to draw a simple figure (cube) in 3D using multiple buffers
    /// </summary>
    [Example("Example 4.05: Drawing in 3D (Several Buffers)", ExampleCategory.Learning, "4. Drawing", 2, Source = "Tutorial04_5", Documentation = "Tutorial04_5")]
    public class TutorialLearning04_5 : GameWindow
    {
        public TutorialLearning04_5()
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

            Vector3f[] positions = new Vector3f[] {
                    new Vector3f(-1, -1, -1),
                    new Vector3f(1, -1, -1),
                    new Vector3f(1, 1, -1),
                    new Vector3f(-1, 1, -1),

                    new Vector3f(-1, -1, 1),
                    new Vector3f(1, -1, 1),
                    new Vector3f(1, 1, 1),
                    new Vector3f(-1, 1, 1),

                    new Vector3f(-1, -1, -1),
                    new Vector3f(-1, 1, -1),
                    new Vector3f(-1, 1, 1),
                    new Vector3f(-1, -1, 1),

                    new Vector3f(1, -1, -1),
                    new Vector3f(1, 1, -1),
                    new Vector3f(1, 1, 1),
                    new Vector3f(1, -1, 1),

                    new Vector3f(-1, -1, -1),
                    new Vector3f(-1, -1, 1),
                    new Vector3f(1, -1, 1),
                    new Vector3f(1, -1, -1),

                    new Vector3f(-1, 1, -1),
                    new Vector3f(-1, 1, 1),
                    new Vector3f(1, 1, 1),
                    new Vector3f(1, 1, -1),
            };
            meshBuff = new MeshBuffers();
            meshBuff.addAttributeBuffer(0, 3, AttributeType.A32F, false);
            AttributeBuffer positionBuff = meshBuff.getAttributeBuffer(0);
            posbuff = new GPUBuffer();
            posbuff.setData(sizeof(float) * 3 * positions.Length, positions, BufferUsage.STATIC_DRAW);
            positionBuff.setBuffer(posbuff);

            Vector3f[] colors = new Vector3f[] {
                new Vector3f(1, 1, 0),
                new Vector3f(1, 1, 0),
                new Vector3f(1, 1, 0),
                new Vector3f(1, 1, 0),

                new Vector3f(0, 0, 1),
                new Vector3f(0, 0, 1),
                new Vector3f(0, 0, 1),
                new Vector3f(0, 0, 1),

                new Vector3f(0, 1, 1),
                new Vector3f(0, 1, 1),
                new Vector3f(0, 1, 1),
                new Vector3f(0, 1, 1),

                new Vector3f(1, 0, 0),
                new Vector3f(1, 0, 0),
                new Vector3f(1, 0, 0),
                new Vector3f(1, 0, 0),

                new Vector3f(1, 0, 1),
                new Vector3f(1, 0, 1),
                new Vector3f(1, 0, 1),
                new Vector3f(1, 0, 1),

                new Vector3f(0, 1, 0),
                new Vector3f(0, 1, 0),
                new Vector3f(0, 1, 0),
                new Vector3f(0, 1, 0)
                };
            meshBuff.addAttributeBuffer(1, 3, AttributeType.A32F, false);
            AttributeBuffer colorBuff = meshBuff.getAttributeBuffer(1);
            colbuff = new GPUBuffer();
            colbuff.setData(sizeof(float) * 3 * colors.Length, colors, BufferUsage.STATIC_DRAW);
            colorBuff.setBuffer(colbuff);


            elembuff = new GPUBuffer();
            uint[] elements = new uint[] {
                               0,1,2,
                               0,2,3,

                               4,5,6,
                               4,6,7,

                               8,9,10,
                               8,10,11,

                               12,13,14,
                               12,14,15,

                               16,17,18,
                               16,18,19,

                               20,21,22,
                               20,22,23 };

            elembuff.setData(sizeof(uint) * 1 * elements.Length, elements, BufferUsage.STATIC_DRAW);
            AttributeBuffer elemAttrBuf = new AttributeBuffer(0, elements.Length, AttributeType.A32UI, false, elembuff);
            meshBuff.setIndicesBuffer(elemAttrBuf);

            fb.setClearColor(Color.White);
        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            if (p != null)
                p.Dispose();
            if (fb != null)
                fb.Dispose();
            if (posbuff != null)
                posbuff.Dispose();
            if (colbuff != null)
                colbuff.Dispose();
            if (elembuff != null)
                elembuff.Dispose();
            if (meshBuff != null)
                meshBuff.Dispose();

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
            angle += 0.01f;
            mat = Matrix4f.CreateTranslation(0.0f, 0.0f, -6.0f) * Matrix4f.CreateRotation(angle, 0.0f, 1.0f, 0.5f);
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
            fb.draw(p, meshBuff, MeshMode.TRIANGLES, 0, meshBuff.getIndiceBuffer().getSize());
            this.SwapBuffers();
        }

        #endregion

        #region Fields
        FrameBuffer fb;
        Program p;
        Matrix4f mat;
        UniformMatrix4f uMVMatrix;
        float angle = 0;
        GPUBuffer posbuff;
        GPUBuffer colbuff;
        GPUBuffer elembuff;
        MeshBuffers meshBuff;

        const string FRAGMENT_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec3 aColor;

        uniform mat4 uMVMatrix;
        uniform mat4 uPMatrix;

        out vec3 vColor;

        void main()
        {
            gl_Position = uPMatrix * uMVMatrix * vec4(aPosition, 1.0);
            vColor = aColor;
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
            using (TutorialLearning04_5 example = new TutorialLearning04_5())
            {
                example.Run(30.0, 10.0);
            }
        }

        #endregion
    }


}
