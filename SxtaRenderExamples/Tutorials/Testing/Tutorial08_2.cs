using Sxta.Render.Resources;
using Sxta.Render.Scenegraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using log4net;
using System.Reflection;
using Sxta.Render;
using OpenTK.Input;
using proland;
using Examples;
using Sxta.Math;
using Sxta.Render.Resources.XmlResources;
using OpenTK.Graphics.OpenGL;

namespace Examples.Tutorials
{
    /// <summary>
    /// Drawing a plane using Scenegraphs
    /// </summary>
    [Example("Example 8.02: Drawing a plane using Scenegraphs", ExampleCategory.Testing, "8. Proland", 1, Source = "Tutorial08_2", Documentation = "Tutorial-TODO")]
    public class Tutorial08_2 : GameWindow
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        SceneManager manager;
        //public TerrainViewController controller;
        public static int mouseX, mouseY;
        public static bool rotate;

        public Tutorial08_2() : base(600, 600) //: GlutWindow(Window.Parameters().size(1024, 768))
        {
#if TODO
            FileLogger.File*out = new FileLogger.File("log.html");
            Logger.INFO_LOGGER = new FileLogger("INFO", out, Logger.INFO_LOGGER);
            Logger.WARNING_LOGGER = new FileLogger("WARNING", out, Logger.WARNING_LOGGER);
            Logger.ERROR_LOGGER = new FileLogger("ERROR", out, Logger.ERROR_LOGGER);
            Keyboard.KeyDown += Keyboard_KeyDown;
            XMLResourceLoader resLoader = new XMLResourceLoader();
            resLoader.addPath(".");
            resLoader.addArchive("helloworld.xml");

            ResourceManager resManager = new ResourceManager(resLoader, 8);

            manager = new SceneManager();
            manager.setResourceManager(resManager);

            manager.setScheduler((Scheduler)(resManager.loadResource("defaultScheduler").get()));
            manager.setRoot((SceneNode)(resManager.loadResource("scene").get()));
            manager.setCameraNode("camera");
            manager.setCameraMethod("draw");

            //controller = new TerrainViewController(manager.getCameraNode(), 2500.0);
#endif
            Keyboard.KeyDown += Keyboard_KeyDown;
        }


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

        protected override void OnLoad(EventArgs e)
        {
            //TERRAIN PARAMETERS
            float size = 500;
            float zmin = -1;
            float zmax = 1;
            Deformation deform = new Deformation();
            float splitFactor = 2;
            int maxLevel = 7;

            //SceneNode
            SceneNode scene = new SceneNode();

            //Camera children
            camera = new SceneNode();
            scene.addChild(camera);
            //Terrain children
            root = new TerrainQuad(null, null, 0, 0, -size, -size, 2.0 * size, zmin, zmax);
            myTerrain = new TerrainNode(deform, root, splitFactor, maxLevel);

            //MESH LOAD
            resLoader = new XMLResourceLoader();
            resLoader.addPath(dir + "/Resources/Meshes");
            resManager = new ResourceManager(resLoader);
            res = (MeshResource)resManager.loadResource("quad2.mesh");
            mesh = (MeshBuffers)res.get();

            //FRAMEBUFFER INIT
            fb = new FrameBuffer(true);
            fb.setDepthTest(true);
            fb.setDepthRange(0.0001f, 100.0f);
            //fb.setFrontFaceCW(false);
            //fb.setPolygonMode(Sxta.Render.PolygonMode.LINE, Sxta.Render.PolygonMode.FILL);

            //PROGRAM INIT
            p = new Program(new Sxta.Render.Module(330, EXAMPLE_SHADER));

            //UNIFORMS INIT
            uMVMatrix = p.getUniformMatrix4f("uMVMatrix");
            uOffset = p.getUniform4f("offset");
            UniformMatrix4f uPMatrix = p.getUniformMatrix4f("uPMatrix");
            // fovy, aspect, zNear, zFar
            Matrix4f projection = Matrix4f.CreatePerspectiveFieldOfView((float)Sxta.Math.MathHelper.ToRadians(60), (float)this.Width / (float)this.Height, 0.01f, 100.0f);
            //Matrix4f projection = Matrix4f.Identity;
            uPMatrix.set(projection);

            fb.setClearColor(System.Drawing.Color.Black);
        }

        protected override void OnUnload(EventArgs e)
        {
            resManager.releaseResource(res);
            if (resManager != null)
                resManager.Dispose();

            if (p != null)
                p.Dispose();
            if (mesh != null)
                mesh.Dispose();
            if (fb != null)
                fb.Dispose();
            base.OnUnload(e);
        }

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


        /// <summary>
        /// Add your game logic here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            myTerrain.update(camera);
            //myTerrain.
            //mat = Matrix4f.CreateTranslation(-2.0f, -2.0f, -6.0f) * Matrix4f.CreateRotation((float)(-Math.PI / 4), 7.0f, -7.0f, 0.5f);
            //uMVMatrix.set(mat);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            fb.clear(true, true, true);
            fb.draw(p, mesh, mesh.mode, 0, mesh.nvertices, 1, mesh.nindices);
            this.SwapBuffers();
        }

        TerrainNode myTerrain;
        TerrainQuad root;
        SceneNode camera;

        string dir = ".";
        XMLResourceLoader resLoader;
        ResourceManager resManager;
        FrameBuffer fb;
        Program p;
        Uniform1f timer;
        float time = 0;

        Matrix4f mat;
        UniformMatrix4f uMVMatrix;
        Uniform4f uOffset;

        MeshResource res;
        MeshBuffers mesh;

        //TUTORIAL08_1 SHADER
        const string EXAMPLE_SHADER = @"
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

        [STAThread]
        public static void Main()
        {
            using (Tutorial08_2 example = new Tutorial08_2())
            {
                example.Run(30.0, 10.0);
            }
        }
#if TODO
        public virtual void redisplay(double t, double dt)
        {
            controller.update();
            controller.setProjection();

            FrameBuffer fb = FrameBuffer.getDefault();
            fb.clear(true, false, true);

            manager.update(t, dt);
            manager.draw();

            GlutWindow.redisplay(t, dt);

            if (Logger.ERROR_LOGGER != NULL)
        {
            Logger.ERROR_LOGGER.flush();
        }

    }

    public virtual void reshape(int x, int y)
        {
            FrameBuffer fb = FrameBuffer.getDefault();
            fb.setDepthTest(true, Function.LESS);
            fb.setViewport(vec4<GLint>(0, 0, x, y));
            GlutWindow.reshape(x, y);
        }

        public virtual void idle(bool damaged)
        {
            GlutWindow.idle(damaged);
            if (damaged)
            {
                manager.getResourceManager().updateResources();
            }
        }

        public virtual bool mouseClick(button b, state s, modifier m, int x, int y)
        {
            mouseX = x;
            mouseY = y;
            rotate = (m & CTRL) != 0;
            return true;
        }

        public virtual bool mouseMotion(int x, int y)
        {
            if (rotate)
            {
                controller.phi += (mouseX - x) / 500.0;
                controller.theta += (mouseY - y) / 500.0;
            }
            else
            {
                Vector3d oldp = manager.getWorldCoordinates(mouseX, mouseY);
                Vector3d p = manager.getWorldCoordinates(x, y);
                if (valid(oldp) && valid(p))
                {
                    controller.move(oldp, p);
                }
            }
            mouseX = x;
            mouseY = y;
            return true;
        }

        public virtual bool mouseWheel(wheel b, modifier m, int x, int y)
        {
            if (b == WHEEL_DOWN)
            {
                controller.d *= 1.1;
            }
            if (b == WHEEL_UP)
            {
                controller.d /= 1.1;
            }
            return true;
        }
#if TODO
        public virtual bool keyTyped(unsigned char c, modifier m, int x, int y)
    {
        if (c == 27)
        {
            .exit(0);
        }
        return true;
    }
#endif
        public virtual bool specialKey(KeyboardKeyEventArgs k, modifier m, int x, int y)
        {
            if (k.Key == Key.F5)
                manager.getResourceManager().updateResources();
            return true;
        }

        public bool valid(Vector3d p)
        {
            return Math.Abs(p.X) < 1000.0 && Math.Abs(p.Y) < 1000.0 && Math.Abs(p.Z) < 1000.0;
        }
        /// <summary>
        /// Entry point of this example.
        /// </summary>
#endif

    }
}


