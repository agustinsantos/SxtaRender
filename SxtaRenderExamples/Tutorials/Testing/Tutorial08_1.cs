// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sxta.Math;
using Sxta.Render;
using System;
using System.Drawing;
using MathHelper = Sxta.Math.MathHelper;

namespace Examples.Tutorials
{
    /// <summary> 
    /// Demonstrates how to draw a simple figure (cube) in 3D
    /// </summary>
    [Example("Example 8.01: Drawing in 3D (Indices)", ExampleCategory.Testing, "08. Proland", 1, Source = "Tutorial08_1", Documentation = "Tutorial-TODO")]
    public class Tutorial08_1 : GameWindow
    {
        public Tutorial08_1()
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
            uOffset = p.getUniform4f("offset");
            UniformMatrix4f uPMatrix = p.getUniformMatrix4f("uPMatrix");
            // fovy, aspect, zNear, zFar
            Matrix4f projection = Matrix4f.CreatePerspectiveFieldOfView((float)MathHelper.ToRadians(60), (float)this.Width / (float)this.Height, 0.01f, 100.0f);
            //Matrix4f projection = Matrix4f.Identity;
            uPMatrix.set(projection);

            mesh = new Mesh<Vertex_V3C3f, uint>(Vertex_V3C3f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC);
            mesh.addAttributeType(0, 3, AttributeType.A32F, false);
            mesh.addAttributeType(1, 3, AttributeType.A32F, false);

            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, -1), Color = new Vector3f(1, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, -1), Color = new Vector3f(1, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, -1), Color = new Vector3f(1, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, -1), Color = new Vector3f(1, 1, 0) });
            /**
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, 1), Color = new Vector3f(0, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, 1), Color = new Vector3f(0, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, 1), Color = new Vector3f(0, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, 1), Color = new Vector3f(0, 0, 1) });

            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, -1), Color = new Vector3f(0, 1, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, -1), Color = new Vector3f(0, 1, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, 1), Color = new Vector3f(0, 1, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, 1), Color = new Vector3f(0, 1, 1) });

            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, -1), Color = new Vector3f(1, 0, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, -1), Color = new Vector3f(1, 0, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, 1), Color = new Vector3f(1, 0, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, 1), Color = new Vector3f(1, 0, 0) });

            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, -1), Color = new Vector3f(1, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, 1), Color = new Vector3f(1, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, 1), Color = new Vector3f(1, 0, 1) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, -1), Color = new Vector3f(1, 0, 1) });

            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, -1), Color = new Vector3f(0, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, 1), Color = new Vector3f(0, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, 1), Color = new Vector3f(0, 1, 0) });
            mesh.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, -1), Color = new Vector3f(0, 1, 0) });
            */
            mesh.addIndices(new uint[] {
                               0,1,2,
                               0,2,3,
                               /**
                               4,5,6,
                               4,6,7,

                               8,9,10,
                               8,10,11,

                               12,13,14,
                               12,14,15,

                               16,17,18,
                               16,18,19,

                               20,21,22,
                               20,22,23*/ });

            fb.setClearColor(Color.White);
        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            if (p != null)
                p.Dispose();
            if (mesh != null)
                mesh.Dispose();
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
            mat = Matrix4f.CreateTranslation(-2.0f, -2.0f, -6.0f) * Matrix4f.CreateRotation((float)(-Math.PI / 4), 7.0f, -7.0f, 0.5f);
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
            for (int j = 0; j < 6; j++)
            {
                for (int i = 0; i < 6; i++)
                {
                    uOffset.set(new Vector4f(j * 2, i * 2, 1, 0));
                    fb.draw(p, mesh);
                }
            }
            this.SwapBuffers();
        }

        #endregion

        #region Fields
        FrameBuffer fb;
        Program p;
        Mesh<Vertex_V3C3f, uint> mesh;
        Matrix4f mat;
        UniformMatrix4f uMVMatrix;
        Uniform4f uOffset;
        float angle = 0;
        /**
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
        **/

        //--------------------------------------ORIGINAL SHADER---------------------------
        /** 
uniform struct {
            vec4 offset;
            vec4 camera;
            vec2 blending;
            mat4 localToScreen;
} deformation;

#ifdef _VERTEX_

layout(location=0) in vec3 vertex;
out vec4 p;

void main() {
            p = vec4(vertex.xy * deformation.offset.z + deformation.offset.xy, 0.0, 1.0);
            gl_Position = deformation.localToScreen * p;
}

#endif

#ifdef _FRAGMENT_

in vec4 p;
layout(location=0) out vec4 data;

void main() {
            data = vec4(vec3(0.2 + 0.2 * sin(0.1 * length(p.xy))), 1.0);
            data.r += mod(dot(floor(deformation.offset.xy / deformation.offset.z + 0.5), vec2(1.0)), 2.0);
}

#endif
*/

        const string FRAGMENT_SHADER = @"
    uniform vec4 offset;
    vec4 camera;
    vec2 blending;
    uniform mat4 uMVMatrix;
    uniform mat4 uPMatrix;

#ifdef _VERTEX_

layout(location=0) in vec3 vertex;
layout(location = 1) in vec3 aColor;
out vec4 p;

void main() {
    p = vec4(vertex.xy * offset.z + offset.xy, vertex.z, 1.0);
    gl_Position = uPMatrix * uMVMatrix * p;
}

#endif

#ifdef _FRAGMENT_

in vec4 p;
layout(location=0) out vec4 data;

void main() {
    data = vec4(vec3(0.2 + 0.3 * sin(10.1 * length(p.xy))), 1.0);
    data.r += mod(dot(floor(offset.xy / offset.z + 0.5), vec2(1.0)), 4);
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
            using (Tutorial08_1 example = new Tutorial08_1())
            {
                example.Run(30.0, 10.0);
            }
        }

        #endregion

        private struct Vertex_V3C3f
        {
            public Vector3f Position;
            public Vector3f Color;
            public static int SizeInBytes
            {
                get { return Vector3f.SizeInBytes + Vector3f.SizeInBytes; }
            }
        }
    }


}

