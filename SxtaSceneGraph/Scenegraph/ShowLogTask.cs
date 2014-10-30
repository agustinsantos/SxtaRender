using log4net;
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
     * A ShowInfoTask sub class to display the ork::Logger messages.
     * @ingroup scenegraph
     */
    public class ShowLogTask : ShowInfoTask, ISwappable<ShowLogTask>
    {
        /**
         * True if this task is enabled. When disabled the message logs are not
         * displayed.
         */
        public static bool enabled;

        /**
         * Creates a new ShowLogTask.
         *
         * @param f the Font used to display Text.
         * @param p the program to be used to draw characters.
         * @param fontHeight the used font height.
         * @param pos x,y position and maximum number of lines of text to display.
         */
        public ShowLogTask(Font f, Program p, float fontHeight, Vector3i pos)
            : base(f, p, 0, fontHeight, pos)
        {
        }


        /**
         * Deletes this ShowLogTask.
         */
        // ~ShowLogTask();


        /**
         * Creates an uninitialized ShowLogTask.
         */
        public ShowLogTask()
            : base()
        {
        }

        /**
         * Initializes this ShowLogTask.
         *
         * @param f the Font used to display Text.
         * @param p the program to be used to draw characters.
         * @param fontHeight the used font height.
         * @param pos x,y position and maximum number of lines of text to display.
         */
        public void init(Font f, Program p, float fontHeight, Vector3i pos)
        {
            if (!initialized)
            {
                int capacity = 256;
                LogBuffer buf = LogBuffer.getInstance(capacity);
                //if (log.IsDebugEnabled) {
                //    Logger::DEBUG_LOGGER = new MemLogger("DEBUG_LOGGER", LogBuffer::DEBUG_LOG, buf, Logger::DEBUG_LOGGER);
                //}
                //Logger::INFO_LOGGER = new MemLogger("INFO", LogBuffer::INFO_LOG, buf, Logger::INFO_LOGGER);
                //Logger::WARNING_LOGGER = new MemLogger("WARNING", LogBuffer::WARNING_LOG, buf, Logger::WARNING_LOGGER);
                //Logger::ERROR_LOGGER = new MemLogger("ERROR", LogBuffer::ERROR_LOG, buf, Logger::ERROR_LOGGER);
                initialized = true;
            }
            base.init(f, p, 0, fontHeight, pos);

        }

        protected override void draw(Method context)
        {
#if TODO
            if (log.IsDebugEnabled)
            {
                log.Debug("ShowLog");
            }
            LogBuffer buf = LogBuffer.getInstance();
            if (buf.hasNewErrors)
            {
                enabled = true;
                buf.hasNewErrors = false;
            }
            if (!enabled)
            {
                return;
            }
            FrameBuffer fb = SceneManager.getCurrentFrameBuffer();
            fb.setBlend(true, ADD, SRC_ALPHA, ONE_MINUS_SRC_ALPHA, ADD, ZERO, ONE);

            fontMesh.clear();

            Vector4f vp = fb.getViewport();

            float xs = (float)position.X;
            float ys = (float)((position.Y > 0) ? position.Y : vp.W + position.Y - min(buf.getSize(), position.Z) * fontHeight);
            for (int l = max(0, buf.getSize() - position.Z); l < buf.getSize(); ++l)
            {
                LogBuffer.type t = buf.getType(l);
                if (t == LogBuffer.DEBUG_LOG)
                {
                    drawLine(vp, xs, ys, 0x00800000, buf.getLine(l));
                }
                else if (t == LogBuffer.INFO_LOG)
                {
                    drawLine(vp, xs, ys, 0x00FF0000, buf.getLine(l));
                }
                else if (t == LogBuffer.WARNING_LOG)
                {
                    drawLine(vp, xs, ys, 0xFFCC0000, buf.getLine(l));
                }
                else
                {
                    drawLine(vp, xs, ys, 0xFF000000, buf.getLine(l));
                }
                ys += (float)fontHeight;
            }

            fb.draw(fontProgram, fontMesh);
            fb.setBlend(false);
#endif
            throw new NotImplementedException();
        }

        public void swap(ShowLogTask obj)
        {
            throw new NotImplementedException();
        }

        static bool initialized = false;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    }

    public class LogBuffer
    {

        public static LogBuffer INSTANCE;

        public static LogBuffer getInstance(int capacity = 256)
        {
            if (INSTANCE == null)
            {
                INSTANCE = new LogBuffer(capacity);
            }
            return INSTANCE;
        }

        public enum LogType
        {
            DEBUG_LOG,
            INFO_LOG,
            WARNING_LOG,
            ERROR_LOG
        }

        public bool hasNewErrors;

        public LogBuffer(int capacity)
        {
            hasNewErrors = false;
            this.capacity = capacity;
            first = 0;
            size = 0;
            types = new LogType[capacity];
            lines = new string[capacity];
        }

        ~LogBuffer()
        {
            // delete[] types;
            // delete[] lines;
            throw new NotImplementedException();
        }

        public int getSize()
        {
            return size;
        }

        public LogType getType(int index)
        {
            return types[(first + index) % capacity];
        }

        public string getLine(int index)
        {
            return lines[(first + index) % capacity];
        }

        public void addLine(LogType t, string s)
        {
            if (size < capacity)
            {
                types[size] = t;
                lines[size] = s;
                size += 1;
            }
            else
            {
                types[first] = t;
                lines[first] = s;
                first = (first + 1) % capacity;
            }
            if (t == LogType.WARNING_LOG || t == LogType.ERROR_LOG)
            {
                hasNewErrors = true;
            }
        }

        public void addText(LogType t, string s)
        {
            string line = null;
            for (int i = 0; i < s.Length; ++i)
            {
                char c = s[i];
                if (c == '\n' || c == '\\')
                {
                    addLine(t, line);
                    line = "";
                }
                else
                {
                    line = line + c;
                }
            }
        }


        private LogType[] types;

        private string[] lines;

        private int capacity;

        private int first;

        private int size;
    }
}
