﻿// This code is in the Public Domain. It is provided "as is"
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
    /// Demonstrates how to compute ADS Lighting Model
    /// </summary>
    [Example("Example 6.4: Ambient, Diffuse and Specular", ExampleCategory.Learning, "6. Lighting", 1, Source = "Tutorial06_4", Documentation = "Tutorial06_4")]
    public class TutorialLearning06_4 : GameWindow
    {
        public TutorialLearning06_4()
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
            uNMatrix = p.getUniformMatrix4f("uNMatrix");

            uPMatrix = p.getUniformMatrix4f("uPMatrix");

            Uniform3f uLightPosition = p.getUniform3f("uLightPosition");
            uKa = p.getUniform3f("uKa");
            uKd = p.getUniform3f("uKd");
            uKs = p.getUniform3f("uKs");
            Uniform3f uLa = p.getUniform3f("uLa");
            Uniform3f uLd = p.getUniform3f("uLd");
            Uniform3f uLs = p.getUniform3f("uLs");
            Uniform1f uShininess = p.getUniform1f("uShininess");
            uLightPosition.set(new Vector3f(-2, -2, -10));
            uLa.set(new Vector3f(0.0f, 0.0f, 0.0f));
            uLd.set(new Vector3f(4.0f, 4.0f, 4.0f));
            uLs.set(new Vector3f(3.0f, 3.0f, 3.0f));
            uShininess.set(18.0f);


            // position the camera 
            camera = new BasicFPCamera(this);
            //camera.LookAt(new Vector3f(0, 0, 7), new Vector3f(0, 0, 0), new Vector3f(0, 1, 0));
            camera.Position = new Vector3f(2, 2, 9);

            // fovy, aspect, zNear, zFar
            Matrix4f projection = Matrix4f.CreatePerspectiveFieldOfView((float)MathHelper.ToRadians(60), (float)this.Width / (float)this.Height, 0.01f, 100.0f);
            uPMatrix.set(projection);

            // Model 1. Sphere 
            mesh1 = MeshUtils.GenerateSolidSphere(1.0f, 40, 40);
            // Model 2. Cone 
            mesh2 = MeshUtils.GenerateSolidCone(1.0, 2.0, 20, 20);
            // Model 3. Cylinder 
            mesh3 = MeshUtils.GenerateSolidCylinder(1.0, 1.0, 20, 20);
            // Model 4. Torus 
            mesh4 = MeshUtils.GenerateSolidTorus(0.5, 1.0, 20, 20);

            // Model 1. Cube
            cube = new Mesh<Vertex_V3C3f, uint>(Vertex_V3C3f.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC);
            cube.addAttributeType(0, 3, AttributeType.A32F, false);
            cube.addAttributeType(1, 3, AttributeType.A32F, false);

            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, -1), Color = new Vector3f(1, 1, 0) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, -1), Color = new Vector3f(1, 1, 0) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, -1), Color = new Vector3f(1, 1, 0) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, -1), Color = new Vector3f(1, 1, 0) });

            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, 1), Color = new Vector3f(0, 0, 1) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, 1), Color = new Vector3f(0, 0, 1) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, 1), Color = new Vector3f(0, 0, 1) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, 1), Color = new Vector3f(0, 0, 1) });

            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, -1), Color = new Vector3f(0, 1, 1) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, -1), Color = new Vector3f(0, 1, 1) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, 1), Color = new Vector3f(0, 1, 1) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, 1), Color = new Vector3f(0, 1, 1) });

            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, -1), Color = new Vector3f(1, 0, 0) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, -1), Color = new Vector3f(1, 0, 0) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, 1), Color = new Vector3f(1, 0, 0) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, 1), Color = new Vector3f(1, 0, 0) });

            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, -1), Color = new Vector3f(1, 0, 1) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, -1, 1), Color = new Vector3f(1, 0, 1) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, 1), Color = new Vector3f(1, 0, 1) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, -1, -1), Color = new Vector3f(1, 0, 1) });

            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, -1), Color = new Vector3f(0, 1, 0) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(-1, 1, 1), Color = new Vector3f(0, 1, 0) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, 1), Color = new Vector3f(0, 1, 0) });
            cube.addVertex(new Vertex_V3C3f() { Position = new Vector3f(1, 1, -1), Color = new Vector3f(0, 1, 0) });
            cube.addIndices(new uint[] {
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
                               20,22,23 });

            fb.setClearColor(Color.White);
        }

        #endregion

        #region OnUnload

        protected override void OnUnload(EventArgs e)
        {
            if (p != null)
                p.Dispose();
            if (cube != null)
                cube.Dispose();
            if (mesh1 != null)
                mesh1.Dispose();
            if (mesh2 != null)
                mesh2.Dispose();
            if (mesh3 != null)
                mesh3.Dispose();
            if (mesh4 != null)
                mesh4.Dispose();
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
            camera.Resize(Width, Height);
            //camera.Update();

            uPMatrix.set(camera.ProjectionMatrix);
        }
        private void SetNormalMatrix(Matrix4f viewMatrix)
        {
            Matrix4f normalMatrix = viewMatrix;
            normalMatrix.Invert();
            normalMatrix.Transpose();
            uNMatrix.set(normalMatrix);
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

            mat = camera.ViewMatrix * Matrix4f.CreateTranslation(0.5f, 3.0f, 0.0f) * Matrix4f.CreateRotation(angle * 3, 0.0f, 1.0f, 0.5f);
            SetNormalMatrix(mat);
            uMVMatrix.set(mat);
            uKa.set(new Vector3f(1.0f, 0.2f, 0.3f));
            uKd.set(new Vector3f(1.0f, 0.2f, 0.3f));
            uKs.set(new Vector3f(1.0f, 0.2f, 0.3f));
            fb.draw(p, mesh1);

            mat = camera.ViewMatrix * Matrix4f.CreateTranslation(5.0f, 3.0f, 0.0f) * Matrix4f.CreateRotation(angle * 3, 0.0f, 1.0f, 0.5f);
            SetNormalMatrix(mat);
            uMVMatrix.set(mat);
            uKa.set(new Vector3f(0.3f, 1.0f, 0.1f));
            uKd.set(new Vector3f(0.3f, 1.0f, 0.1f));
            uKs.set(new Vector3f(0.3f, 1.0f, 0.1f));
            fb.draw(p, mesh2);

            mat = camera.ViewMatrix * Matrix4f.CreateTranslation(0.5f, 0.0f, 0.0f) * Matrix4f.CreateRotation(angle * 3, 0.0f, 1.0f, 0.5f);
            SetNormalMatrix(mat);
            uMVMatrix.set(mat);
            uKa.set(new Vector3f(0.2f, 0.2f, 1.0f));
            uKd.set(new Vector3f(0.2f, 0.2f, 1.0f));
            uKs.set(new Vector3f(0.2f, 0.2f, 1.0f));
            fb.draw(p, mesh3);

            mat = camera.ViewMatrix * Matrix4f.CreateTranslation(5.0f, 0.0f, 0.0f) * Matrix4f.CreateRotation(angle * 3, 0.0f, 1.0f, 0.5f);
            SetNormalMatrix(mat);
            uMVMatrix.set(mat);
            uKa.set(new Vector3f(1.0f, 1.0f, 0.3f));
            uKd.set(new Vector3f(1.0f, 1.0f, 0.3f));
            uKs.set(new Vector3f(1.0f, 1.0f, 0.3f));
            fb.draw(p, mesh4);

            RenderAxes(camera.ViewMatrix);

            this.SwapBuffers();
        }

        #endregion
        public virtual void RenderAxes(Matrix4f camera, int num = 10)
        {
            for (int i = 0; i < num; i++)
            {
                mat = camera * Matrix4f.CreateTranslation(i, 0.0f, 0.0f) * Matrix4f.Scale(0.25f, 0.1f, 0.1f);
                SetNormalMatrix(mat);
                uMVMatrix.set(mat);
                uKa.set(new Vector3f(1.0f, 0.0f, 0.0f));
                uKd.set(new Vector3f(1.0f, 0.0f, 0.0f));
                uKs.set(new Vector3f(1.0f, 0.0f, 0.0f));
                fb.draw(p, cube);
            }
            for (int i = 0; i < num; i++)
            {
                mat = camera * Matrix4f.CreateTranslation(0.0f, i, 0.0f) * Matrix4f.Scale(0.1f, 0.25f, 0.1f);
                SetNormalMatrix(mat);
                uMVMatrix.set(mat);
                uKa.set(new Vector3f(0.0f, 1.0f, 0.0f));
                uKd.set(new Vector3f(0.0f, 1.0f, 0.0f));
                uKs.set(new Vector3f(0.0f, 1.0f, 0.0f));
                fb.draw(p, cube);
            }
            for (int i = 0; i < num; i++)
            {
                mat = camera * Matrix4f.CreateTranslation(0.0f, 0.0f, i) * Matrix4f.Scale(0.1f, 0.1f, 0.25f);
                SetNormalMatrix(mat);
                uMVMatrix.set(mat);
                uKa.set(new Vector3f(0.0f, 0.0f, 1.0f));
                uKd.set(new Vector3f(0.0f, 0.0f, 1.0f));
                uKs.set(new Vector3f(0.0f, 0.0f, 1.0f));
                fb.draw(p, cube);
            }
        }
        #region Fields
        FrameBuffer fb;
        Program p;
        Mesh<Vertex_V3N3T2f, ushort> mesh1, mesh2, mesh3, mesh4;
        Mesh<Vertex_V3C3f, uint> cube;
        Matrix4f mat;
        UniformMatrix4f uMVMatrix;
        UniformMatrix4f uPMatrix;
        UniformMatrix4f uNMatrix;
        Uniform3f uKa, uKd, uKs;
        private BasicFPCamera camera;
        float angle = 0;

        const string FRAGMENT_SHADER = @"
#ifdef _VERTEX_
        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec3 aNormal;

        uniform mat4 uMVMatrix;
        uniform mat4 uPMatrix;
        uniform mat4 uNMatrix;           // The normal matrix is the transpose inverse of the modelview matrix

        uniform vec3 uLightPosition;	 // Light position in eye coords
        uniform vec3 uKa;                // Ambient Reflectivity
        uniform vec3 uKd;                // Diffuse Reflectivity
        uniform vec3 uKs;                // Specular Reflectivity

        uniform vec3 uLa;                // Ambient light intensity
        uniform vec3 uLd;                // Diffuse light intensity
        uniform vec3 uLs;                // Specular light intensity

        uniform float uShininess;        // Specular shininess factor

        out vec3 vColor;

        void main()
        {
            // Convert normal and position to eye coords
            vec4 eyeCoords = uMVMatrix * vec4(aPosition, 1.0);
            vec3 tnorm = vec3(normalize(uNMatrix * vec4(aNormal, 1.0)));
            
            vec3 s = normalize(vec3(uLightPosition - vec3(eyeCoords)));
            vec3 v = normalize(- vec3(eyeCoords));
            vec3 r = reflect(-s, tnorm);
            vec3 ambient = uLa * uKa;
            float sDotN = max(dot(s, tnorm), 0.0);
            vec3 diffuse = uLd * uKd * sDotN;
            vec3 specular = vec3(0.0);
            if(sDotN > 0.0)
            {
                specular = uLs * uKs * pow ( max (dot(r,v), 0.0), uShininess);

            // The diffuse shading equation
            vColor = ambient + diffuse + specular;
             } else
                // The diffuse shading equation
                vColor = s;

            gl_Position = uPMatrix * eyeCoords;
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
            using (TutorialLearning06_4 example = new TutorialLearning06_4())
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
