using log4net;
using OpenTK.Graphics;
using System.Diagnostics;
using System.Reflection;

namespace Sxta.Render.Core
{
    public class TraceOpenTKDisposableObject
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string stactTraceAtConstructor;
        public TraceOpenTKDisposableObject()
        {
            stactTraceAtConstructor = GetPartialStackTrace();
            log.DebugFormat("This object {0} was created{1}.", this.GetHashCode(), stactTraceAtConstructor);
            CheckCurrentContext();
        }

        private static string GetPartialStackTrace()
        {
            // Create a StackTrace that captures stack trace information, skipping two
            // frames (this call and the previous one).
            StackTrace st = new StackTrace(2, true);
            string stackTrace = st.ToString();
            return stackTrace;
        }

        public void CheckCurrentContext()
        {
            // This situation usually occurs when the GraphicsContext is not created or has been disposed and there are still some 
            // OpenGL objects pending to be disposed. Usually the error is due to a wrong disposing policy. Make sure that every OpenGL 
            // object is disposed before you end your main program (close your window and therefore the GraphicsContext associated with it)
            // OpenGL only works with an active graphics context.
            if (GraphicsContext.CurrentContext == null ||
                GraphicsContext.CurrentContext.IsDisposed ||
                !GraphicsContext.CurrentContext.IsCurrent)
            {
                string actualStackTrace = GetPartialStackTrace();
                log.FatalFormat("An object is trying to use OpenGl but there is no active graphics context. Stack Trace ={0}", actualStackTrace);
                log.FatalFormat("This object was created{0}", stactTraceAtConstructor);
                Debugger.Break();
            }
        }


        public void CheckDispose()
        {
            string actualStackTrace = GetPartialStackTrace();
            log.DebugFormat("This object {0} was disposed{1}.", this.GetHashCode(), actualStackTrace);
            CheckCurrentContext();
        }
    }
}
