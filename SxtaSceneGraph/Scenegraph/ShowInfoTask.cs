using log4net;
using Sxta.Core;
using Sxta.Math;
using Sxta.Render.OpenGLExt;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Render.Scenegraph
{
    /**
     * An AbstractTask to display the framerate and other information.
     * @ingroup scenegraph
     */
    public class ShowInfoTask : AbstractTask, ISwappable<ShowInfoTask>
    {

        /**
         * Creates a new ShowInfoTask.
         *
         * @param font the Font used to display Text.
         * @param p the program to be used to draw characters.
         * @param color the font color in RGBA8 format.
         * @param size the font height.
         * @param pos x,y position and maximum number of lines of text to display.
         */
        public ShowInfoTask(Font font, Program p, int color, float size, Vector3i pos)
            : base("ShowInfoTask")
        {
            init(font, p, color, size, pos);
        }



        /**
         * Deletes this ShowInfoTask.
         */
        // ~ShowInfoTask();

        public override Task getTask(Object context)
        {
            return new Impl((Method)context, this);
        }


        /**
         * Adds an information to display. The information has a topic and replaces
         * the previous information in this topic. All the topics are cleared after
         * each frame (you have to set them at each frame if you want them to
         * persist on screen).
         *
         * @param topic the topic of the information.
         * @param info an information message.
         */
        static void setInfo(string topic, string info)
        {
            infos[topic] = info;
        }


        /**
         * The mesh used to draw character quads, in order to display text.
         */
        protected static Mesh<Font.Vertex, uint> fontMesh;

        /**
         * The current information messages, associated with their topic.
         */
        protected static IDictionary<string, string> infos = new Dictionary<string, string>();

        /**
         * The program use to draw characters.
         */
        protected Program fontProgram;

        /**
         * The uniform in #fontProgram used to control the font texture.
         */
        protected UniformSampler fontU;

        /**
         * The Font used to display Text.
         */
        protected Font font;

        /**
         * The font color in RGBA8 format.
         */
        protected int fontColor;

        /**
         * The used font height.
         */
        protected float fontHeight;

        /**
         * The x,y position and the maximum number of lines of text to be displayed.
         */
        protected Vector3i position;

        /**
         * Creates an uninitialized ShowInfoTask.
         */
        public ShowInfoTask()
            : base("ShowInfoTask")
        {
        }

        /**
         * Initializes this ShowInfoTask.
         *
         * @param font the Font used to display Text.
         * @param p the program to be used to draw characters.
         * @param color the font color in RGBA8 format.
         * @param size the font height.
         * @param pos x,y position and maximum number of lines of text to display.
         */
        public virtual void init(Font font, Program p, int color, float size, Vector3i pos)
        {
            this.fps = 0;
            this.frames = 0;
            this.start = 0.0;
            this.fontProgram = p;
            this.fontU = p.getUniformSampler("font");
            this.font = font;
            this.fontColor = color;
            this.position = pos;
            this.fontHeight = size;
            if (fontMesh == null)
            {
                fontMesh = new Mesh<Font.Vertex, uint>(Font.Vertex.SizeInBytes, MeshMode.TRIANGLES, MeshUsage.CPU);
                fontMesh.addAttributeType(0, 4, AttributeType.A16F, false);
                fontMesh.addAttributeType(1, 4, AttributeType.A8UI, true);
            }
        }

        /**
         * Swaps this ShowInfoTask with another one.
         *
         * @param t a ShowInfoTask.
         */
        public virtual void swap(ShowInfoTask t)
        {
            Std.Swap(ref fontProgram, ref  t.fontProgram);
            Std.Swap(ref fontU, ref t.fontU);
            Std.Swap(ref font, ref t.font);
            Std.Swap(ref fontColor, ref t.fontColor);
            Std.Swap(ref fontHeight, ref t.fontHeight);
            Std.Swap(ref  position, ref t.position);
            Std.Swap(ref fps, ref t.fps);
            Std.Swap(ref frames, ref t.frames);
            Std.Swap(ref start, ref t.start);
        }

        /**
         * Draws a line of text.
         *
         * @param vp the framebuffer viewport, in pixels.
         * @param xs the x coordinate of the first character to display.
         * @param ys the y coordinate of the first character to display.
         * @param color the color of this line of text, in RGBA8 format.
         * @param s the line of text to display.
         */
        protected virtual void drawLine(Vector4f vp, float xs, float ys, int color, string s)
        {
            font.addLine(vp, xs, ys, s, fontHeight, color, fontMesh);
        }
        protected virtual void drawLine(Vector4i vp, float xs, float ys, int color, string s)
        {
            font.addLine(vp, xs, ys, s, fontHeight, color, fontMesh);
        }
        /**
         * Draws the framerate and the information messages.
         *
         * @param context the method to which this task belongs.
         */
        protected virtual void draw(Method context)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("ShowInfo");
            }

            FrameBuffer fb = SceneManager.getCurrentFrameBuffer();
            fb.setBlend(true, BlendEquation.ADD, BlendArgument.SRC_ALPHA, BlendArgument.ONE_MINUS_SRC_ALPHA, BlendEquation.ADD, BlendArgument.ZERO, BlendArgument.ONE);

            Vector4i vp = fb.getViewport();
            float xs = (float)position.X;
            float ys = position.Y > 0 ? position.Y : vp.W + position.Y - position.Z * fontHeight;

            ++frames;
            double current = context.getOwner().getOwner().getTime();
            double delay = (current - start) * 1e-6;
            if (delay > 1.0)
            {
                fps = (int)(frames / delay);
                frames = 0;
                start = current;
            }
            else if (delay < 0.0)
            {
                // happens when replaying recorded events
                fps = 0;
                frames = 0;
                start = current;
            }
            string os = "";
            string val;
            if (!infos.TryGetValue("FPS", out val))
            {
                os += fps + " FPS";
            }
            else
            {
                os += val + " FPS";
            }

            fontMesh.clear();
            drawLine(vp, xs, ys, fontColor, os);
            ys += fontHeight;

            foreach (var info in infos)
            {
                if (info.Key != "FPS" && !string.IsNullOrWhiteSpace(info.Value))
                {
                    drawLine(vp, xs, ys, fontColor, info.Value);
                    ys += fontHeight;
                }
            }
            infos.Clear();

            fontU.set(font.getImage());
            fb.draw(fontProgram, fontMesh);

            fb.setBlend(false);

        }


        /**
         * The current framerate.
         */
        private int fps;

        /**
         * The number of frames displayed since #start. This counter is periodically
         * reset to 0.
         */
        private int frames;

        /**
         * The time at which the #frames counter was reset to 0.
         */
        private double start;

        /**
         * A Task to display the framerate and other information.
         */
        private class Impl : Task
        {

            /**
             * Creates a new ShowInfoTask::Impl task.
             *
             * @param context the method to which 'source' belongs.
             * @param source the ShowInfoTask that created this task.
             */
            public Impl(Method context, ShowInfoTask source)
                : base("ShowInfo", true, 0)
            {
                this.context = context;
                this.source = source;
            }


            /**
             * Deletes this ShowInfoTask::Impl.
             */
            // ~Impl();

            public override bool run()
            {
                source.draw(context);
                return true;
            }


            protected override Type getTypeInfo()
            {
                return source.GetType();
            }


            /**
             * The method to which #source belongs.
             */
            private Method context;

            /**
             * The ShowInfoTask that created this task.
             */
            private ShowInfoTask source;
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
