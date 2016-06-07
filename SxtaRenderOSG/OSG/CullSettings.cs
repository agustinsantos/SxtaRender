using Sxta.Math;
using System;
using System.IO;

namespace Sxta.OSG
{
    public class CullSettings
    {
        public CullSettings()
        {
            setDefaults();
            readEnvironmentalVariables();
        }

        public CullSettings(ArgumentParser arguments)
        {
            setDefaults();
            readEnvironmentalVariables();
            readCommandLine(arguments);
        }

        public CullSettings(CullSettings cs) { throw new NotImplementedException(); }


        //public CullSettings& operator = (const CullSettings& settings)
        //{
        //    if (this==&settings) return *this;
        //    setCullSettings(settings);
        //    return *this;
        //}


        public virtual void setDefaults() { throw new NotImplementedException(); }

        [Flags]
        public enum VariablesMask : uint
        {
            COMPUTE_NEAR_FAR_MODE = (0x1 << 0),
            CULLING_MODE = (0x1 << 1),
            LOD_SCALE = (0x1 << 2),
            SMALL_FEATURE_CULLING_PIXEL_SIZE = (0x1 << 3),
            CLAMP_PROJECTION_MATRIX_CALLBACK = (0x1 << 4),
            NEAR_FAR_RATIO = (0x1 << 5),
            IMPOSTOR_ACTIVE = (0x1 << 6),
            DEPTH_SORT_IMPOSTOR_SPRITES = (0x1 << 7),
            IMPOSTOR_PIXEL_ERROR_THRESHOLD = (0x1 << 8),
            NUM_FRAMES_TO_KEEP_IMPOSTORS_SPRITES = (0x1 << 9),
            CULL_MASK = (0x1 << 10),
            CULL_MASK_LEFT = (0x1 << 11),
            CULL_MASK_RIGHT = (0x1 << 12),
            CLEAR_COLOR = (0x1 << 13),
            CLEAR_MASK = (0x1 << 14),
            LIGHTING_MODE = (0x1 << 15),
            LIGHT = (0x1 << 16),
            DRAW_BUFFER = (0x1 << 17),
            READ_BUFFER = (0x1 << 18),

            NO_VARIABLES = 0x00000000,
            ALL_VARIABLES = 0x7FFFFFFF
        };

        // typedef int InheritanceMask;

        /** Set the inheritance mask used in inheritCullSettings to control which variables get overwritten by the passed in CullSettings object.*/
        public void setInheritanceMask(uint mask) { _inheritanceMask = mask; }

        /** Get the inheritance mask used in inheritCullSettings to control which variables get overwritten by the passed in CullSettings object.*/
        public uint getInheritanceMask() { return _inheritanceMask; }

        /** Set the local cull settings values from specified CullSettings object.*/
        public void setCullSettings(CullSettings settings) { throw new NotImplementedException(); }

        /** Inherit the local cull settings variable from specified CullSettings object, according to the inheritance mask.*/
        public virtual void inheritCullSettings(CullSettings settings) { inheritCullSettings(settings, _inheritanceMask); }

        /** Inherit the local cull settings variable from specified CullSettings object, according to the inheritance mask.*/
        public virtual void inheritCullSettings(CullSettings settings, uint inheritanceMask) { throw new NotImplementedException(); }

        /** read the environmental variables.*/
        public void readEnvironmentalVariables() { throw new NotImplementedException(); }

        /** read the commandline arguments.*/
        public void readCommandLine(ArgumentParser arguments) { throw new NotImplementedException(); }


        public enum InheritanceMaskActionOnAttributeSetting
        {
            DISABLE_ASSOCIATED_INHERITANCE_MASK_BIT,
            DO_NOT_MODIFY_INHERITANCE_MASK
        };

        public void setInheritanceMaskActionOnAttributeSetting(InheritanceMaskActionOnAttributeSetting action) { _inheritanceMaskActionOnAttributeSetting = action; }
        public InheritanceMaskActionOnAttributeSetting getInheritanceMaskActionOnAttributeSetting() { return _inheritanceMaskActionOnAttributeSetting; }

        /** Apply the action, specified by the InheritanceMaskActionOnAttributeSetting, to apply to the inheritance bit mask.
          * This method is called by CullSettings.set*() parameter methods to ensure that CullSettings inheritance mechanisms doesn't overwrite the local parameter settings.*/
        public void applyMaskAction(VariablesMask maskBit)
        {
            if (_inheritanceMaskActionOnAttributeSetting == InheritanceMaskActionOnAttributeSetting.DISABLE_ASSOCIATED_INHERITANCE_MASK_BIT)
            {
                _inheritanceMask = _inheritanceMask & (~(uint)maskBit);
            }
        }


        /** Switch the creation of Impostors on or off.
          * Setting active to false forces the CullVisitor to use the Impostor
          * LOD children for rendering. Setting active to true forces the
          * CullVisitor to create the appropriate pre-rendering stages which
          * render to the ImpostorSprite's texture.*/
        public void setImpostorsActive(bool active) { _impostorActive = active; applyMaskAction(VariablesMask.IMPOSTOR_ACTIVE); }

        /** Get whether impostors are active or not. */
        public bool getImpostorsActive() { return _impostorActive; }

        /** Set the impostor error threshold.
          * Used in calculation of whether impostors remain valid.*/
        public void setImpostorPixelErrorThreshold(float numPixels) { _impostorPixelErrorThreshold = numPixels; applyMaskAction(VariablesMask.IMPOSTOR_PIXEL_ERROR_THRESHOLD); }

        /** Get the impostor error threshold.*/
        public float getImpostorPixelErrorThreshold() { return _impostorPixelErrorThreshold; }

        /** Set whether ImpostorSprite's should be placed in a depth sorted bin for rendering.*/
        public void setDepthSortImpostorSprites(bool doDepthSort) { _depthSortImpostorSprites = doDepthSort; applyMaskAction(VariablesMask.DEPTH_SORT_IMPOSTOR_SPRITES); }

        /** Get whether ImpostorSprite's are depth sorted bin for rendering.*/
        public bool getDepthSortImpostorSprites() { return _depthSortImpostorSprites; }

        /** Set the number of frames that an ImpostorSprite is kept whilst not being beyond,
          * before being recycled.*/
        public void setNumberOfFrameToKeepImpostorSprites(int numFrames) { _numFramesToKeepImpostorSprites = numFrames; applyMaskAction(VariablesMask.NUM_FRAMES_TO_KEEP_IMPOSTORS_SPRITES); }

        /** Get the number of frames that an ImpostorSprite is kept whilst not being beyond,
          * before being recycled.*/
        public int getNumberOfFrameToKeepImpostorSprites() { return _numFramesToKeepImpostorSprites; }

        public enum ComputeNearFarMode
        {
            DO_NOT_COMPUTE_NEAR_FAR = 0,
            COMPUTE_NEAR_FAR_USING_BOUNDING_VOLUMES,
            COMPUTE_NEAR_FAR_USING_PRIMITIVES,
            COMPUTE_NEAR_USING_PRIMITIVES
        };

        public void setComputeNearFarMode(ComputeNearFarMode cnfm) { _computeNearFar = cnfm; applyMaskAction(VariablesMask.COMPUTE_NEAR_FAR_MODE); }
        public ComputeNearFarMode getComputeNearFarMode() { return _computeNearFar; }

        public void setNearFarRatio(double ratio) { _nearFarRatio = ratio; applyMaskAction(VariablesMask.NEAR_FAR_RATIO); }
        public double getNearFarRatio() { return _nearFarRatio; }

        [Flags]
        public enum CullingModeValues
        {
            NO_CULLING = 0x0,
            VIEW_FRUSTUM_SIDES_CULLING = 0x1,
            NEAR_PLANE_CULLING = 0x2,
            FAR_PLANE_CULLING = 0x4,
            VIEW_FRUSTUM_CULLING = VIEW_FRUSTUM_SIDES_CULLING |
                                          NEAR_PLANE_CULLING |
                                          FAR_PLANE_CULLING,
            SMALL_FEATURE_CULLING = 0x8,
            SHADOW_OCCLUSION_CULLING = 0x10,
            CLUSTER_CULLING = 0x20,
            DEFAULT_CULLING = VIEW_FRUSTUM_SIDES_CULLING |
                                          SMALL_FEATURE_CULLING |
                                          SHADOW_OCCLUSION_CULLING |
                                          CLUSTER_CULLING,
            ENABLE_ALL_CULLING = VIEW_FRUSTUM_CULLING |
                                          SMALL_FEATURE_CULLING |
                                          SHADOW_OCCLUSION_CULLING |
                                          CLUSTER_CULLING
        };

        // typedef int CullingMode;

        /** Set the culling mode for the CullVisitor to use.*/
        public void setCullingMode(int mode) { _cullingMode = mode; applyMaskAction(VariablesMask.CULLING_MODE); }

        /** Returns the current CullingMode.*/
        public int getCullingMode() { return _cullingMode; }


        public void setCullMask(uint nm) { _cullMask = nm; applyMaskAction(VariablesMask.CULL_MASK); }
        public uint getCullMask() { return _cullMask; }

        void setCullMaskLeft(uint nm) { _cullMaskLeft = nm; applyMaskAction(VariablesMask.CULL_MASK_LEFT); }
        public uint getCullMaskLeft() { return _cullMaskLeft; }

        void setCullMaskRight(uint nm) { _cullMaskRight = nm; applyMaskAction(VariablesMask.CULL_MASK_RIGHT); }
        public uint getCullMaskRight() { return _cullMaskRight; }

        /** Set the LOD bias for the CullVisitor to use.*/
        public void setLODScale(float scale) { _LODScale = scale; applyMaskAction(VariablesMask.LOD_SCALE); }

        /** Get the LOD bias.*/
        public float getLODScale() { return _LODScale; }

        /** Threshold at which small features are culled.
            \param value Bounding volume size in screen space. Default is 2.0. */
        public void setSmallFeatureCullingPixelSize(float value) { _smallFeatureCullingPixelSize = value; applyMaskAction(VariablesMask.SMALL_FEATURE_CULLING_PIXEL_SIZE); }

        /** Get the Small Feature Culling Pixel Size.*/
        public float getSmallFeatureCullingPixelSize() { return _smallFeatureCullingPixelSize; }



        /** Callback for overriding the CullVisitor's default clamping of the projection matrix to computed near and far values.
          * Note, both Matrixf and Matrixd versions of clampProjectionMatrixImplementation must be implemented as the CullVisitor
          * can target either Matrix data type, configured at compile time.*/
        public abstract class ClampProjectionMatrixCallback
        {
            public abstract bool clampProjectionMatrixImplementation(Matrix4f projection, out double znear, out double zfar);
            public abstract bool clampProjectionMatrixImplementation(Matrix4d projection, out double znear, out double zfar);
        }

        /** set the ClampProjectionMatrixCallback.*/
        public void setClampProjectionMatrixCallback(ClampProjectionMatrixCallback cpmc) { _clampProjectionMatrixCallback = cpmc; applyMaskAction(VariablesMask.CLAMP_PROJECTION_MATRIX_CALLBACK); }
        /** get the ClampProjectionMatrixCallback.*/
        public ClampProjectionMatrixCallback getClampProjectionMatrixCallback() { return _clampProjectionMatrixCallback; }


        /** Write out internal settings of CullSettings. */
        public void write(StreamWriter sw) { throw new NotImplementedException(); }



        protected uint _inheritanceMask;
        protected InheritanceMaskActionOnAttributeSetting _inheritanceMaskActionOnAttributeSetting;

        protected ComputeNearFarMode _computeNearFar;
        protected int _cullingMode;
        protected float _LODScale;
        protected float _smallFeatureCullingPixelSize;

        protected ClampProjectionMatrixCallback _clampProjectionMatrixCallback;
        protected double _nearFarRatio;
        protected bool _impostorActive;
        protected bool _depthSortImpostorSprites;
        protected float _impostorPixelErrorThreshold;
        protected int _numFramesToKeepImpostorSprites;

        protected uint _cullMask;
        protected uint _cullMaskLeft;
        protected uint _cullMaskRight;
    }
}