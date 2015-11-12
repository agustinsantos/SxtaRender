using System;
using System.Collections.Generic;

namespace Sxta.Render.OSG
{
    public enum DisplayType
    {
        MONITOR,
        POWERWALL,
        REALITY_CENTER,
        HEAD_MOUNTED_DISPLAY
    }

    public enum StereoMode
    {
        QUAD_BUFFER,
        ANAGLYPHIC,
        HORIZONTAL_SPLIT,
        VERTICAL_SPLIT,
        LEFT_EYE,
        RIGHT_EYE,
        HORIZONTAL_INTERLACE,
        VERTICAL_INTERLACE,
        CHECKERBOARD
    }

    public enum SplitStereoHorizontalEyeMapping
    {
        LEFT_EYE_LEFT_VIEWPORT,
        LEFT_EYE_RIGHT_VIEWPORT
    }

    public enum SplitStereoVerticalEyeMapping
    {
        LEFT_EYE_TOP_VIEWPORT,
        LEFT_EYE_BOTTOM_VIEWPORT
    }

    [Flags]
    public enum ImplicitBufferAttachment
    {
        IMPLICIT_DEPTH_BUFFER_ATTACHMENT = (1 << 0),
        IMPLICIT_STENCIL_BUFFER_ATTACHMENT = (1 << 1),
        IMPLICIT_COLOR_BUFFER_ATTACHMENT = (1 << 2),
        DEFAULT_IMPLICIT_BUFFER_ATTACHMENT = IMPLICIT_COLOR_BUFFER_ATTACHMENT | IMPLICIT_DEPTH_BUFFER_ATTACHMENT
    }

    public enum SwapMethod
    {
        SWAP_DEFAULT,   // Leave swap method at default returned by choose Pixel Format.
        SWAP_EXCHANGE,  // Flip front / back buffer.
        SWAP_COPY,      // Copy back to front buffer.
        SWAP_UNDEFINED  // Move back to front buffer leaving contents of back buffer undefined.
    }

    /// <summary>
    /// DisplaySettings class for encapsulating what visuals are required and
    /// have been set up, and the status of stereo viewing.
    /// </summary>
    public class DisplaySettings
    {
        private static DisplaySettings s_displaySettings = new DisplaySettings();

        /** Maintain a DisplaySettings singleton for objects to query at runtime.*/
        public static DisplaySettings Instance
        {
            get { return s_displaySettings; }
        }

        public DisplaySettings()
        {
            SetDefaults();
        }
        public DisplaySettings(DisplaySettings ds)
        {
            SetDisplaySettings(ds);
        }

        public void SetDisplaySettings(DisplaySettings vs)
        {
            _displayType = vs._displayType;
            _stereo = vs._stereo;
            _stereoMode = vs._stereoMode;
            _eyeSeparation = vs._eyeSeparation;
            _screenWidth = vs._screenWidth;
            _screenHeight = vs._screenHeight;
            _screenDistance = vs._screenDistance;

            _splitStereoHorizontalEyeMapping = vs._splitStereoHorizontalEyeMapping;
            _splitStereoHorizontalSeparation = vs._splitStereoHorizontalSeparation;

            _splitStereoVerticalEyeMapping = vs._splitStereoVerticalEyeMapping;
            _splitStereoVerticalSeparation = vs._splitStereoVerticalSeparation;

            _splitStereoAutoAdjustAspectRatio = vs._splitStereoAutoAdjustAspectRatio;

            _doubleBuffer = vs._doubleBuffer;
            _RGB = vs._RGB;
            _depthBuffer = vs._depthBuffer;
            _minimumNumberAlphaBits = vs._minimumNumberAlphaBits;
            _minimumNumberStencilBits = vs._minimumNumberStencilBits;
            _minimumNumberAccumRedBits = vs._minimumNumberAccumRedBits;
            _minimumNumberAccumGreenBits = vs._minimumNumberAccumGreenBits;
            _minimumNumberAccumBlueBits = vs._minimumNumberAccumBlueBits;
            _minimumNumberAccumAlphaBits = vs._minimumNumberAccumAlphaBits;

            _maxNumOfGraphicsContexts = vs._maxNumOfGraphicsContexts;
            _numMultiSamples = vs._numMultiSamples;

            _compileContextsHint = vs._compileContextsHint;
            _serializeDrawDispatch = vs._serializeDrawDispatch;
            _useSceneViewForStereoHint = vs._useSceneViewForStereoHint;

            _numDatabaseThreadsHint = vs._numDatabaseThreadsHint;
            _numHttpDatabaseThreadsHint = vs._numHttpDatabaseThreadsHint;

            _application = vs._application;

            _maxTexturePoolSize = vs._maxTexturePoolSize;
            _maxBufferObjectPoolSize = vs._maxBufferObjectPoolSize;

            _implicitBufferAttachmentRenderMask = vs._implicitBufferAttachmentRenderMask;
            _implicitBufferAttachmentResolveMask = vs._implicitBufferAttachmentResolveMask;

            _glContextVersion = vs._glContextVersion;
            _glContextFlags = vs._glContextFlags;
            _glContextProfileMask = vs._glContextProfileMask;
            _swapMethod = vs._swapMethod;

            _keystoneHint = vs._keystoneHint;
            _keystoneFileNames = vs._keystoneFileNames;
            _keystones = vs._keystones;
        }

        public void Merge(DisplaySettings vs)
        {
            if (_stereo || vs._stereo) _stereo = true;

            // need to think what to do about merging the stereo mode.

            if (_doubleBuffer || vs._doubleBuffer) _doubleBuffer = true;
            if (_RGB || vs._RGB) _RGB = true;
            if (_depthBuffer || vs._depthBuffer) _depthBuffer = true;

            if (vs._minimumNumberAlphaBits > _minimumNumberAlphaBits) _minimumNumberAlphaBits = vs._minimumNumberAlphaBits;
            if (vs._minimumNumberStencilBits > _minimumNumberStencilBits) _minimumNumberStencilBits = vs._minimumNumberStencilBits;
            if (vs._numMultiSamples > _numMultiSamples) _numMultiSamples = vs._numMultiSamples;

            if (vs._compileContextsHint) _compileContextsHint = vs._compileContextsHint;
            if (vs._serializeDrawDispatch) _serializeDrawDispatch = vs._serializeDrawDispatch;
            if (vs._useSceneViewForStereoHint) _useSceneViewForStereoHint = vs._useSceneViewForStereoHint;

            if (vs._numDatabaseThreadsHint > _numDatabaseThreadsHint) _numDatabaseThreadsHint = vs._numDatabaseThreadsHint;
            if (vs._numHttpDatabaseThreadsHint > _numHttpDatabaseThreadsHint) _numHttpDatabaseThreadsHint = vs._numHttpDatabaseThreadsHint;

            if (string.IsNullOrWhiteSpace(_application)) _application = vs._application;

            if (vs._maxTexturePoolSize > _maxTexturePoolSize) _maxTexturePoolSize = vs._maxTexturePoolSize;
            if (vs._maxBufferObjectPoolSize > _maxBufferObjectPoolSize) _maxBufferObjectPoolSize = vs._maxBufferObjectPoolSize;

            // these are bit masks so merging them is like logical or
            _implicitBufferAttachmentRenderMask |= vs._implicitBufferAttachmentRenderMask;
            _implicitBufferAttachmentResolveMask |= vs._implicitBufferAttachmentResolveMask;

            // merge swap method to higher value
            if (vs._swapMethod > _swapMethod)
                _swapMethod = vs._swapMethod;

            _keystoneHint = _keystoneHint | vs._keystoneHint;

            // insert any unique filenames into the local list
            foreach (string filename in vs._keystoneFileNames)
            {
                if (!_keystoneFileNames.Contains(filename))
                    _keystoneFileNames.Add(filename);
            }

            // insert unique Keystone object into local list
            foreach (Object obj in vs._keystones)
            {
                if (!_keystones.Contains(obj))
                    _keystones.Add(obj);
            }
        }

        public void SetDefaults()
        {
            _displayType = DisplayType.MONITOR;

            _stereo = false;
            _stereoMode = StereoMode.ANAGLYPHIC;
            _eyeSeparation = 0.05f;
            _screenWidth = 0.325f;
            _screenHeight = 0.26f;
            _screenDistance = 0.5f;

            _splitStereoHorizontalEyeMapping = SplitStereoHorizontalEyeMapping.LEFT_EYE_LEFT_VIEWPORT;
            _splitStereoHorizontalSeparation = 0;

            _splitStereoVerticalEyeMapping = SplitStereoVerticalEyeMapping.LEFT_EYE_TOP_VIEWPORT;
            _splitStereoVerticalSeparation = 0;

            _splitStereoAutoAdjustAspectRatio = false;

            _doubleBuffer = true;
            _RGB = true;
            _depthBuffer = true;
            _minimumNumberAlphaBits = 0;
            _minimumNumberStencilBits = 0;
            _minimumNumberAccumRedBits = 0;
            _minimumNumberAccumGreenBits = 0;
            _minimumNumberAccumBlueBits = 0;
            _minimumNumberAccumAlphaBits = 0;

            _maxNumOfGraphicsContexts = 32;
            _numMultiSamples = 0;

#if  __sgi
            // switch on anti-aliasing by default, just in case we have an Onyx :-)
            _numMultiSamples = 4;
#endif

            _compileContextsHint = false;
            _serializeDrawDispatch = false;
            _useSceneViewForStereoHint = true;

            _numDatabaseThreadsHint = 2;
            _numHttpDatabaseThreadsHint = 1;

            _maxTexturePoolSize = 0;
            _maxBufferObjectPoolSize = 0;

            _implicitBufferAttachmentRenderMask = ImplicitBufferAttachment.DEFAULT_IMPLICIT_BUFFER_ATTACHMENT;
            _implicitBufferAttachmentResolveMask = ImplicitBufferAttachment.DEFAULT_IMPLICIT_BUFFER_ATTACHMENT;
            _glContextVersion = "1.0";
            _glContextFlags = 0;
            _glContextProfileMask = 0;

            _swapMethod = SwapMethod.SWAP_DEFAULT;

            _keystoneHint = false;
        }

        public DisplayType DisplayType
        {
            get { return _displayType; }
            set { _displayType = value; }
        }
        public bool Stereo
        {
            get { return _stereo; }
            set { _stereo = value; }
        }
        public StereoMode StereoMode
        {
            get { return _stereoMode; }
            set { _stereoMode = value; }
        }
        public float EyeSeparation
        {
            get { return _eyeSeparation; }
            set { _eyeSeparation = value; }
        }
        public float ScreenWidth
        {
            get { return _screenWidth; }
            set { _screenWidth = value; }
        }
        public float ScreenHeight
        {
            get { return _screenHeight; }
            set { _screenHeight = value; }
        }
        public float ScreenDistance
        {
            get { return _screenDistance; }
            set { _screenDistance = value; }
        }
        public SplitStereoHorizontalEyeMapping SplitStereoHorizontalEyeMapping
        {
            get { return _splitStereoHorizontalEyeMapping; }
            set { _splitStereoHorizontalEyeMapping = value; }
        }
        public int SplitStereoHorizontalSeparation
        {
            get { return _splitStereoHorizontalSeparation; }
            set { _splitStereoHorizontalSeparation = value; }
        }
        public SplitStereoVerticalEyeMapping SplitStereoVerticalEyeMapping
        {
            get { return _splitStereoVerticalEyeMapping; }
            set { _splitStereoVerticalEyeMapping = value; }
        }
        public int SplitStereoVerticalSeparation
        {
            get { return _splitStereoVerticalSeparation; }
            set { _splitStereoVerticalSeparation = value; }
        }
        public bool SplitStereoAutoAdjustAspectRatio
        {
            get { return _splitStereoAutoAdjustAspectRatio; }
            set { _splitStereoAutoAdjustAspectRatio = value; }
        }
        public bool DoubleBuffer
        {
            get { return _doubleBuffer; }
            set { _doubleBuffer = value; }
        }
        public bool DepthBuffer
        {
            get { return _depthBuffer; }
            set { _depthBuffer = value; }
        }
        public int MaxNumberOfGraphicsContexts
        {
            get { return _maxNumOfGraphicsContexts; }
            set { _maxNumOfGraphicsContexts = value; }
        }
        public int NumMultiSamples
        {
            get { return _numMultiSamples; }
            set { _numMultiSamples = value; }
        }
        protected string Application
        {
            get { return _application; }
            set { _application = value; }
        }
        public bool RGB
        {
            get { return _RGB; }
            set { _RGB = value; }
        }
        public void SetImplicitBufferAttachmentMask(ImplicitBufferAttachment renderMask = ImplicitBufferAttachment.DEFAULT_IMPLICIT_BUFFER_ATTACHMENT,
                                                    ImplicitBufferAttachment resolveMask = ImplicitBufferAttachment.DEFAULT_IMPLICIT_BUFFER_ATTACHMENT)
        {
            _implicitBufferAttachmentRenderMask = renderMask;
            _implicitBufferAttachmentResolveMask = resolveMask;
        }

        public void SetImplicitBufferAttachmentRenderMask(ImplicitBufferAttachment implicitBufferAttachmentRenderMask)
        {
            _implicitBufferAttachmentRenderMask = implicitBufferAttachmentRenderMask;
        }
        public int MinimumNumAlphaBits
        {
            get { return _minimumNumberAlphaBits; }
            set { _minimumNumberAlphaBits = value; }
        }
        public int MinimumNumStencilBits
        {
            get { return _minimumNumberStencilBits; }
            set { _minimumNumberStencilBits = value; }
        }
        public string GLContextVersion
        {
            get { return _glContextVersion; }
            set { _glContextVersion = value; }
        }
        public int GLContextFlags
        {
            get { return _glContextFlags; }
            set { _glContextFlags = value; }
        }
        public int GLContextProfileMask
        {
            get { return _glContextProfileMask; }
            set { _glContextProfileMask = value; }
        }
        public SwapMethod SwapMethod
        {
            get { return _swapMethod; }
            set { _swapMethod = value; }
        }

        private bool _stereo;
        private DisplayType _displayType;
        private StereoMode _stereoMode;
        private float _eyeSeparation;
        private float _screenWidth;
        private float _screenHeight;
        private float _screenDistance;
        private SplitStereoHorizontalEyeMapping _splitStereoHorizontalEyeMapping;
        private int _splitStereoHorizontalSeparation;
        private SplitStereoVerticalEyeMapping _splitStereoVerticalEyeMapping;
        private int _splitStereoVerticalSeparation;
        private bool _splitStereoAutoAdjustAspectRatio;
        private bool _doubleBuffer;
        private bool _RGB;

        private bool _depthBuffer;

        private int _minimumNumberAlphaBits;
        private int _minimumNumberStencilBits;


        public int _minimumNumberAccumRedBits;
        public int _minimumNumberAccumGreenBits;
        public int _minimumNumberAccumBlueBits;
        public int _minimumNumberAccumAlphaBits;
        private int _maxNumOfGraphicsContexts;
        private int _numMultiSamples;
        protected bool _compileContextsHint;
        protected bool _serializeDrawDispatch;
        protected bool _useSceneViewForStereoHint;
        protected int _numDatabaseThreadsHint;
        protected int _numHttpDatabaseThreadsHint;
        private string _application;

        protected int _maxTexturePoolSize;
        protected int _maxBufferObjectPoolSize;

        protected ImplicitBufferAttachment _implicitBufferAttachmentRenderMask;
        protected ImplicitBufferAttachment _implicitBufferAttachmentResolveMask;

        private string _glContextVersion;
        private int _glContextFlags;
        private int _glContextProfileMask;

        private SwapMethod _swapMethod;


        public bool _keystoneHint;
        protected List<string> _keystoneFileNames;
        protected List<Object> _keystones;


    }
}
