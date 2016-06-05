using log4net;
using log4net.Appender;
using log4net.Core;
using Sxta.Math;
using Sxta.Render.OpenGLExt;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public bool Enabled { get; set; }

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
        public void init(Font f, Program p, float fontHeight, Vector3i pos, int capacity = 10)
        {
            if (!initialized)
            {
                LogBuffer buf = LogBuffer.getInstance(capacity);
                var root = ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root;
                var attachable = root as IAppenderAttachable;
                var appender = CreateLogBufferAppender();
                if (attachable != null)
                {
                    attachable.AddAppender(appender);
                    //((log4net.Repository.Hierarchy.Logger)log.Logger.).AddAppender(CreateLogBufferAppender());
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Logger is connected");
                    }
                }
                initialized = true;
                Enabled = false;
            }
            base.init(f, p, 0, fontHeight, pos);

        }

        protected override void draw(Method context)
        {
            if (!Enabled)
            {
                return;
            }
            LogBuffer buf = LogBuffer.getInstance();

            FrameBuffer fb = SceneManager.getCurrentFrameBuffer();
            fb.setBlend(true, BlendEquation.ADD, BlendArgument.SRC_ALPHA, BlendArgument.ONE_MINUS_SRC_ALPHA, BlendEquation.ADD, BlendArgument.ZERO, BlendArgument.ONE);

            fontMesh.clear();

            Vector4f vp = (Vector4f)fb.getViewport();

            float xs = (float)position.X;
            float ys = (float)((position.Y > 0) ? position.Y : vp.W + position.Y - System.Math.Min(buf.getSize(), position.Z) * fontHeight);
            for (int l = System.Math.Max(0, buf.getSize() - position.Z); l < buf.getSize(); ++l)
            {
                LogBuffer.LogType t = buf.getType(l);
                if (t == LogBuffer.LogType.DEBUG_LOG)
                {
                    drawLine(vp, xs, ys, 0x00800000, buf.getLine(l));
                }
                else if (t == LogBuffer.LogType.INFO_LOG)
                {
                    drawLine(vp, xs, ys, 0x00FF0000, buf.getLine(l));
                }
                else if (t == LogBuffer.LogType.WARN_LOG)
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
        }

        public void swap(ShowLogTask obj)
        {
            throw new NotImplementedException();
        }

        // Create a new LogBuffer appender
        private static LogBufferAppender CreateLogBufferAppender(string name = "LogBuffer")
        {
            LogBufferAppender appender = new LogBufferAppender();
            appender.Name = name;

            log4net.Layout.PatternLayout layout = new log4net.Layout.PatternLayout();
            //"%date [%thread] %-5level %logger - %message%newline"
            layout.ConversionPattern = "%date{HH:mm:ss,fff} %-5level %logger - %message";
            layout.ActivateOptions();

            appender.Layout = layout;
            appender.ActivateOptions();

            return appender;
        }

        static bool initialized = false;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    }
    public class LogBufferAppender : AppenderSkeleton
    {
        /// <summary>
        /// Writes the logging event to a MessageBox
        /// </summary>
        override protected void Append(LoggingEvent loggingEvent)
        {
            LogBuffer.LogType logLevel = LogBuffer.LogType.ERROR_LOG;
            switch (loggingEvent.Level.Name)
            {
                case "DEBUG":
                    logLevel = LogBuffer.LogType.DEBUG_LOG;
                    break;
                case "INFO":
                    logLevel = LogBuffer.LogType.INFO_LOG;
                    break;
                case "WARN":
                    logLevel = LogBuffer.LogType.WARN_LOG;
                    break;
                case "ERROR":
                    logLevel = LogBuffer.LogType.ERROR_LOG;
                    break;
                case "FATAL":
                    logLevel = LogBuffer.LogType.FATAL_LOG;
                    break;
            }
            LogBuffer.getInstance().addLine(logLevel, RenderLoggingEvent(loggingEvent));
        }

        /// <summary>
        /// This appender requires a <see cref="Layout"/> to be set.
        /// </summary>
        override protected bool RequiresLayout
        {
            get { return true; }
        }
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
            WARN_LOG,
            ERROR_LOG,
            FATAL_LOG
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

        //~LogBuffer()
        //{
        //    // delete[] types;
        //    // delete[] lines;
        //    Debugger.Break();
        //}

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
            if (t >= LogType.WARN_LOG)
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
