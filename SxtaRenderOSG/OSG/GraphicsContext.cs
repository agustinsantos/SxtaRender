using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Render.OSG
{
    public class ScreenIdentifier
    {
        public ScreenIdentifier()
        {
            displayNum = 0;
            screenNum = 0;
            hostName = "";
        }

        public ScreenIdentifier(int in_screenNum)
        {
            displayNum = 0;
            screenNum = in_screenNum;
            hostName = "";
        }

        public ScreenIdentifier(string in_hostName, int in_displayNum, int in_screenNum)
        {
            displayNum = in_displayNum;
            screenNum = in_screenNum;
            hostName = in_hostName;
        }

        /** Return the display name in the form hostName::displayNum:screenNum. */
        public string DisplayName
        {
            get { return hostName + ":" + displayNum + "." + screenNum; }
        }

        /** Read the DISPLAY environmental variable, and set the ScreenIdentifier accordingly.
          * Note, if either of displayNum or screenNum are not defined then -1 is set respectively to
          * signify that this parameter has not been set. When parameters are undefined one can call
          * call setUndefinedScreenDetailsToDefaultScreen() after readDISPLAY() to ensure valid values. */
        public void ReadDISPLAY() { throw new NotImplementedException(); }

        /** Set the screenIndentifier from the displayName string.
          * Note, if either of displayNum or screenNum are not defined then -1 is set to
          * signify that this parameter has not been set. When parameters are undefined one can call
          * call setUndefinedScreenDetailsToDefaultScreen() after readDISPLAY() to ensure valid values. */
        public void SetScreenIdentifier(string displayName) { throw new NotImplementedException(); }

        /** Set any undefined displayNum or screenNum values (i.e. -1) to the default display & screen of 0 respectively.*/
        public void SetUndefinedScreenDetailsToDefaultScreen()
        {
            if (displayNum < 0) displayNum = 0;
            if (screenNum < 0) screenNum = 0;
        }

        private string hostName;
        private int displayNum;
        private int screenNum;
    }

    public class Traits : ScreenIdentifier
    {
        public Traits(DisplaySettings ds = null)
        {
            x = 0;
            y = 0;
            width = 0;
            height = 0;
            windowDecoration = false;
            supportsResize = true;
            red = 8;
            blue = 8;
            green = 8;
            alpha = 0;
            depth = 24;
            stencil = 0;
            sampleBuffers = 0;
            samples = 0;
            pbuffer = false;
            quadBufferStereo = false;
            doubleBuffer = false;
            target = 0;
            format = 0;
            level = 0;
            face = 0;
            mipMapGeneration = false;
            vsync = true;
            swapGroupEnabled = false;
            swapGroup = 0;
            swapBarrier = 0;
            useMultiThreadedOpenGLEngine = false;
            useCursor = true;
            glContextVersion = "1.0";
            glContextFlags = 0;
            glContextProfileMask = 0;
            sharedContext = null;
            setInheritedWindowPixelFormat = false;
            overrideRedirect = false;
            swapMethod = SwapMethod.SWAP_DEFAULT;

            if (ds != null)
            {
                alpha = ds.MinimumNumAlphaBits;
                stencil = ds.MinimumNumStencilBits;
                if (ds.NumMultiSamples != 0) sampleBuffers = 1;
                samples = ds.NumMultiSamples;
                if (ds.Stereo)
                {
                    switch (ds.StereoMode)
                    {
                        case (StereoMode.QUAD_BUFFER): quadBufferStereo = true; break;
                        case (StereoMode.VERTICAL_INTERLACE):
                        case (StereoMode.CHECKERBOARD):
                        case (StereoMode.HORIZONTAL_INTERLACE): stencil = 8; break;
                        default: break;
                    }
                }

                glContextVersion = ds.GLContextVersion;
                glContextFlags = ds.GLContextFlags;
                glContextProfileMask = ds.GLContextProfileMask;

                swapMethod = ds.SwapMethod;
            }
        }


        // graphics context original and size
        public int x;
        public int y;
        public int width;
        public int height;

        // window decoration and behaviour
        public string windowName;
        public bool windowDecoration;
        public bool supportsResize;

        // buffer depths, 0 equals off.
        public int red;
        public int blue;
        public int green;
        public int alpha;
        public int depth;
        public int stencil;

        // multi sample parameters
        public int sampleBuffers;
        public int samples;

        // buffer configuration
        public bool pbuffer;
        public bool quadBufferStereo;
        public bool doubleBuffer;

        // render to texture
        public int target;
        public int format;
        public int level;
        public int face;
        public bool mipMapGeneration;

        // V-sync
        public bool vsync;

        // Swap Group
        public bool swapGroupEnabled;
        public uint swapGroup;
        public uint swapBarrier;

        // use multithreaded OpenGL-engine (OS X only)
        public bool useMultiThreadedOpenGLEngine;

        // enable cursor
        public bool useCursor;

        // settings used in set up of graphics context, only presently used by GL3 build of OSG.
        public string glContextVersion;
        public int glContextFlags;
        public int glContextProfileMask;

        /** return true if glContextVersion is set in the form major.minor, and assign the appropriate major and minor values to the associated parameters.*/
        public bool getContextVersion(out int major, out int minor)
        {
            major = minor = 0;
            if (string.IsNullOrWhiteSpace(glContextVersion)) return false;
            string[] parts = glContextVersion.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            major = int.Parse(parts[0]);
            minor = int.Parse(parts[1]);
            return true;
        }

        // shared context
        public GraphicsContext sharedContext;

        public object inheritedWindowData;

        // ask the GraphicsWindow implementation to set the pixel format of an inherited window
        public bool setInheritedWindowPixelFormat;

        // X11 hint whether to override the window managers window size/position redirection
        public bool overrideRedirect;

        public SwapMethod swapMethod;
    }
    public class ScreenSettings
    {
        public ScreenSettings()
        {
            width = 0;
            height = 0;
            refreshRate = 0;
            colorDepth = 0;
        }
        public ScreenSettings(int w, int h, double rRate = 0, int cDepth = 0)
        {
            this.width = w;
            this.height = h;
            this.refreshRate = rRate;
            this.colorDepth = cDepth;
        }

        public int width;
        public int height;
        public double refreshRate;         ///< Screen refresh rate, in Hz.
        public int colorDepth;    ///< RGB(A) color buffer depth.
    }

    /// <summary>
    /// Callback to be implemented to provide access to Windowing API's ability to create Windows/pbuffers.
    /// </summary>
    public abstract class WindowingSystemInterface
    {
        public abstract int GeNumScreens(ScreenIdentifier screenIdentifier /*= ScreenIdentifier()*/);

        public abstract void GetScreenSettings(ScreenIdentifier screenIdentifier, out ScreenSettings resolution);

        public virtual bool SetScreenSettings(ScreenIdentifier screenIdentifier, ScreenSettings resolution) { return false; }

        public abstract void EnumerateScreenSettings(ScreenIdentifier screenIdentifier, List<ScreenSettings> resolutionList);

        public abstract GraphicsContext CreateGraphicsContext(Traits traits);

        /** Gets screen resolution without using the ScreenResolution structure.
          * \deprecated Provided only for backward compatibility. */
        public void GetScreenResolution(ScreenIdentifier screenIdentifier, out int width, out  int height)
        {
            ScreenSettings settings;
            GetScreenSettings(screenIdentifier, out settings);
            width = settings.width;
            height = settings.height;
        }

        /** Sets screen resolution without using the ScreenSettings structure.
          * \deprecated Provided only for backward compatibility. */
        public bool SetScreenResolution(ScreenIdentifier screenIdentifier, int width, int height)
        {
            return SetScreenSettings(screenIdentifier, new ScreenSettings(width, height));
        }

        /** \deprecated Provided only for backward compatibility. */
        public bool setScreenRefreshRate(ScreenIdentifier screenIdentifier, double refreshRate)
        {
            ScreenSettings settings;
            GetScreenSettings(screenIdentifier, out settings);
            settings.refreshRate = refreshRate;
            return SetScreenSettings(screenIdentifier, settings);
        }
    }

    /// <summary>
    ///  Base class for providing Windowing API agnostic access to creating and managing graphics context.
    /// </summary>
    public class GraphicsContext
    {
        /// <summary>
        /// Set the query the windowing system for screens and create graphics context - this functor should be supplied by the windows toolkit.
        /// </summary>
        /// <param name="wsInterface"></param>
        public static void SetWindowingSystemInterface(WindowingSystemInterface wsInterface)
        {
            WindowingSystemInterface = wsInterface;
        }


        /** Create a graphics context for a specified set of traits.*/
        public static GraphicsContext CreateGraphicsContext(Traits traits)
        {
            WindowingSystemInterface wsref = GraphicsContext.WindowingSystemInterface;
            if (wsref != null)
            {
                // catch any undefined values.
                if (traits != null) traits.SetUndefinedScreenDetailsToDefaultScreen();

                return wsref.CreateGraphicsContext(traits);
            }
            else
                return null;
        }

        private static WindowingSystemInterface s_WindowingSystemInterface;
        // Use a static reference to hold the window system interface.
        // Wrap this within a function, in order to control the order in which
        // the static pointer's constructor is executed.
        private static WindowingSystemInterface WindowingSystemInterface
        {
            get
            {
                return s_WindowingSystemInterface;
            }
            set
            {
                s_WindowingSystemInterface = value;
            }
        }
    }
}
