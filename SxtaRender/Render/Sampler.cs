using OpenTK.Graphics.OpenGL;
using Sxta.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Sxta.Render
{
    /// <summary>
    /// A texture sampler. A texture sampler can be used in a UniformSampler to
    /// change the default sampling parameters of a texture.
    /// </summary>
    public class Sampler
    {
    
		/// <summary>
    	/// A set of texture sampling parameters.
    	/// </summary>
        public class Parameters
        {

          
			/// <summary>
			/// Creates a new set of texture sampling parameters with default values.
			/// Initializes a new instance of the <see cref="Sxta.Render.Sampler+Parameters"/> class.
			/// </summary>
            public Parameters()
            {
                _wrapS = TextureWrap.CLAMP_TO_EDGE;
                _wrapT = TextureWrap.CLAMP_TO_EDGE;
                _wrapR = TextureWrap.CLAMP_TO_EDGE;
                _min = TextureFilter.NEAREST;
                _mag = TextureFilter.LINEAR;
                _borderType = 0;
                _lodMin = -1000.0f;
                _lodMax = 1000.0f;
                _lodBias = 0.0f;
                _maxAnisotropy = 1.0f;
                _compareFunc = Function.ALWAYS;

                _border.i0 = 0;
                _border.i1 = 0;
                _border.i2 = 0;
                _border.i3 = 0;
            }


            /*
             * Deletes these texture sampling parameters.
             */
            //~Parameters(){}

            
			/// <summary>
			/// Returns the wrap parameter for texture coordinate s. Determines the behavior of the sampler
            /// when sampling out of the borders of the texture.
			/// </summary>
			/// <returns>
			/// The s.
			/// </returns>
            public TextureWrap wrapS()
            {
                return _wrapS;
            }

            
			/// <summary>
			/// Returns the wrap parameter for texture coordinate t. Determines the behavior of the sampler
            /// when sampling out of the borders of the texture.
			/// </summary>
			/// <returns>
			/// The t.
			/// </returns>
            public TextureWrap wrapT()
            {
                return _wrapT;
            }

           
			/// <summary>
			/// Returns the wrap parameter for texture coordinate r (only for 3D textures or arrays). Determines
            /// the behavior of the sampler when sampling out of the borders of the texture.
			/// </summary>
			/// <returns>
			/// The r.
			/// </returns>
            public TextureWrap wrapR()
            {
                return _wrapR;
            }

            
			/// <summary>
			/// Returns the minifying function used whenever the pixel being textured maps to an area
            /// greated than one texture element.
			/// </summary>
            public TextureFilter min()
            {
                return _min;
            }

            
			/// <summary>
			/// Returns the magnifying function used whenever the pixel being textured maps to an area
            /// less than or equal to one texture element.
			/// </summary>
            public TextureFilter mag()
            {
                return _mag;
            }

           
			/// <summary>
			/// Returns the type of data used to set the border color.
            /// Can be int, float, Iint, or uint.
			/// </summary>
			/// <returns>
			/// The type.
			/// </returns>
            public uint borderType()
            {
                return _borderType;
            }

            
			/// <summary>
			/// Returns the border color as integers.
			/// </summary>
            public int[] borderi()
            {
                return new int[] { _border.i0, _border.i1, _border.i2, _border.i3 };
            }

            
			/// <summary>
			/// Returns the border color as floats.
			/// </summary>
            public float[] borderf()
            {
                return new float[] { _border.f0, _border.f1, _border.f2, _border.f3 };
            }

            
			/// <summary>
			/// Returns the border color as Iintegers.
			/// </summary>
			/// <returns>
			/// The ii.
			/// </returns>
            public int[] borderIi()
            {
                return new int[] { _border.i0, _border.i1, _border.i2, _border.i3 };
            }

            
			/// <summary>
			/// Returns the border color as uintegers.
			/// </summary>
			/// <returns>
			/// The iui.
			/// </returns>
            public uint[] borderIui()
            {
                return new uint[] { _border.ui0, _border.ui1, _border.ui2, _border.ui3 };
            }

            
			/// <summary>
			/// LReturns the minimum level of detail used in this sampler.
			/// </summary>
			/// <returns>
			/// The minimum.
			/// </returns>
            public float lodMin()
            {
                return _lodMin;
            }

           
			/// <summary>
			/// Returns the maximum level of detail used in this sampler.
			/// </summary>
			/// <returns>
			/// The max.
			/// </returns>
            public float lodMax()
            {
                return _lodMax;
            }


			/// <summary>
			/// Returns the bias applied to the level of details. The bias is the starting valueC
            /// when looking inside a texture, and will be added to the computed level displayed.
			/// </summary>
			/// <returns>
			/// The bias.
			/// </returns>
            public float lodBias()
            {
                return _lodBias;
            }

            
			/// <summary>
			/// Returns the comparison operator used for depth tests.
			/// </summary>
			/// <returns>
			/// The func.
			/// </returns>
            public Function compareFunc()
            {
                return _compareFunc;
            }

            
			/// <summary>
			/// MReturns the anisotropic maximum valueC parameter.
			/// </summary>
			/// <returns>
			/// The anisotropy EX.
			/// </returns>
            public float maxAnisotropyEXT()
            {
                return _maxAnisotropy;
            }

            
			/// <summary>
			/// Sets the wrap parameter for texture coordinate s. Determines the behavior of the sampler
            /// when sampling out of the borders of the texture.
			/// </summary>
			/// <returns>
			/// The s.
			/// </returns>
			/// <param name='wrapS'>
			/// Wrap s the wrap parameter for coordinate s.
			/// </param>
            public virtual Parameters wrapS(TextureWrap wrapS)
            {
                _wrapS = wrapS;
                return this;
            }


			/// <summary>
			/// Sets the wrap parameter for texture coordinate t. Determines the behavior of the sampler
            /// when sampling out of the borders of the texture.
			/// </summary>
			/// <returns>
			/// The t.
			/// </returns>
			/// <param name='wrapT'>
			/// Wrap t the wrap parameter for coordinate t.
			/// </param>
            public virtual Parameters wrapT(TextureWrap wrapT)
            {
                _wrapT = wrapT;
                return this;
            }

            
			/// <summary>
			/// Sets the wrap parameter for texture coordinate r. Determines the behavior of the sampler
            /// when sampling out of the borders of the texture.
			/// </summary>
			/// <returns>
			/// The r.
			/// </returns>
			/// <param name='wrapR'>
			/// Wrap r the wrap parameter for coordinate r.
			/// </param>
            public virtual Parameters wrapR(TextureWrap wrapR)
            {
                _wrapR = wrapR;
                return this;
            }

			/// <summary>
			/// Sets the minifying function used whenever the pixel being textured maps to an area
            /// greater than one texture element.
			/// </summary>
			/// <param name='min'>
			/// Minimum min the minifying function.
			/// </param>
            public virtual Parameters min(TextureFilter min)
            {
                _min = min;
                return this;
            }

			/// <summary>
			/// Sets the magnifying function used whenever the pixel being textured maps to an area
            /// less than or equal to one texture element.
			/// </summary>
			/// <param name='mag'>
			/// Mag the magnifying function.
			/// </param>
            public virtual Parameters mag(TextureFilter mag)
            {
                _mag = mag;
                return this;
            }

        
			/// <summary>
			/// Sets the border color as integers.
			/// Borderi the specified r, g, b and a.
			/// </summary>
			/// <param name='r'>
			/// R. red component.
			/// </param>
			/// <param name='g'>
			/// G. green component.
			/// </param>
			/// <param name='b'>
			/// B. blue component.
			/// </param>
			/// <param name='a'>
			/// A. alpha component.
			/// </param>
            public virtual Parameters borderi(int r, int g, int b, int a = 0)
            {
                _border.i0 = r;
                _border.i1 = g;
                _border.i2 = b;
                _border.i3 = a;
                _borderType = 0;
                return this;
            }

			/// <summary>
			/// Sets the border color as floats.
			/// Borderi the specified r, g, b and a.
			/// </summary>
			/// <param name='r'>
			/// R. red component.
			/// </param>
			/// <param name='g'>
			/// G. green component.
			/// </param>
			/// <param name='b'>
			/// B. blue component.
			/// </param>
			/// <param name='a'>
			/// A. alpha component.
			/// </param>
            public virtual Parameters borderf(float r, float g, float b, float a = 0.0f)
            {
                _border.f0 = r;
                _border.f1 = g;
                _border.f2 = b;
                _border.f3 = a;
                _borderType = 1;
                return this;
            }

			/// <summary>
			/// Sets the border color as Lintegers.
			/// Borderi the specified r, g, b and a.
			/// </summary>
			/// <param name='r'>
			/// R. red component.
			/// </param>
			/// <param name='g'>
			/// G. green component.
			/// </param>
			/// <param name='b'>
			/// B. blue component.
			/// </param>
			/// <param name='a'>
			/// A. alpha component.
			/// </param>
            public virtual Parameters borderIi(int r, int g, int b, int a = 0)
            {
                _border.i0 = r;
                _border.i1 = g;
                _border.i2 = b;
                _border.i3 = a;
                _borderType = 2;
                return this;
            }


			/// <summary>
			/// Sets the border color as uintegers.
			/// Borderi the specified r, g, b and a.
			/// </summary>
			/// <param name='r'>
			/// R. red component.
			/// </param>
			/// <param name='g'>
			/// G. green component.
			/// </param>
			/// <param name='b'>
			/// B. blue component.
			/// </param>
			/// <param name='a'>
			/// A. alpha component.
			/// </param>
            public virtual Parameters borderIui(uint r, uint g, uint b, uint a = 0)
            {
                _border.ui0 = r;
                _border.ui1 = g;
                _border.ui2 = b;
                _border.ui3 = a;
                _borderType = 2;
                return this;
            }

       
			/// <summary>
			/// Sets the minimum level of detail used in this sampler.
			/// </summary>
			/// <returns>
			/// The minimum.
			/// </returns>
			/// <param name='lodMin'>
			/// Lod minimum the minimum level of detail.
			/// </param>
            public virtual Parameters lodMin(float lodMin)
            {
                _lodMin = lodMin;
                return this;
            }

        
			/// <summary>
			/// Sets the maximum level of detail used in this sampler.
			/// </summary>
			/// <returns>
			/// The max.
			/// </returns>
			/// <param name='lodMax'>
			/// Lod max the maximum level of detail.
			/// </param>
            public virtual Parameters lodMax(float lodMax)
            {
                _lodMax = lodMax;
                return this;
            }

       
			/// <summary>
			/// Sets the bias applied to the level of details. The bias is the starting valueC
            /// when looking inside a texture, and will be added to the computed level displayed.
			/// </summary>
			/// <returns>
			/// The bias.
			/// </returns>
			/// <param name='lodBias'>
			/// Lod bias. the bias applied to every lod access.
			/// </param>
            public virtual Parameters lodBias(float lodBias)
            {
                _lodBias = lodBias;
                return this;
            }

        
			/// <summary>
			/// Sets the comparison operator used for depth tests.
			/// </summary>
			/// <returns>
			/// The func.
			/// </returns>
			/// <param name='compareFunc'>
			/// Compare func. the comparison operator.
			/// </param>
            public virtual Parameters compareFunc(Function compareFunc)
            {
                _compareFunc = compareFunc;
                return this;
            }

        
			/// <summary>
			/// Sets the anisotropic maximum valueC parameter.
			/// </summary>
			/// <returns>
			/// The anisotropy EX.
			/// </returns>
			/// <param name='maxAnisotropy'>
			/// Max anisotropy the anisotropy maximum valueC.
			/// </param>
            public virtual Parameters maxAnisotropyEXT(float maxAnisotropy)
            {
                _maxAnisotropy = maxAnisotropy;
                return this;
            }

            
			/// <param name='l'>
			/// Less Operator for Sampler::Parameters. Mostly here to determine if the operators are
            /// not the same.
			/// </param>
			/// <param name='r'>
			/// R.
			/// </param>
            public static bool operator <(Parameters l, Parameters r)
            {
                if (l._wrapS != r._wrapS) return l._wrapS < r._wrapS;
                if (l._wrapT != r._wrapT) return l._wrapT < r._wrapT;
                if (l._wrapR != r._wrapR) return l._wrapR < r._wrapR;
                if (l._min != r._min) return l._min < r._min;
                if (l._mag != r._mag) return l._mag < r._mag;
                if (l._borderType != r._borderType) return l._borderType < r._borderType;
                switch (l._borderType)
                {
                    case 0: // i
                    case 2:
                        { // Ii
                            Vector4i ub = new Vector4i(l._border.i0, l._border.i1, l._border.i2, l._border.i3);
                            Vector4i vb = new Vector4i(r._border.i0, r._border.i1, r._border.i2, r._border.i3);
                            if (ub < vb)
                            {
                                return true;
                            }
                            if (vb < ub)
                            {
                                return false;
                            }
                            break;
                        }
                    case 3:
                        { // Iui
                            Vector4ui ub = new Vector4ui(l._border.ui0, l._border.ui1, l._border.ui2, l._border.ui3);
                            Vector4ui vb = new Vector4ui(r._border.ui0, r._border.ui1, r._border.ui2, r._border.ui3);
                            if (ub < vb)
                            {
                                return true;
                            }
                            if (vb < ub)
                            {
                                return false;
                            }
                            break;
                        }
                    case 1:
                        { // f
                            Vector4f ub = new Vector4f(l._border.f0, l._border.f1, l._border.f2, l._border.f3);
                            Vector4f vb = new Vector4f(r._border.f0, r._border.f1, r._border.f2, r._border.f3);
                            if (ub < vb)
                            {
                                return true;
                            }
                            if (vb < ub)
                            {
                                return false;
                            }
                            break;
                        }
                    default:
                        Debug.Assert(false);
                        break;
                }
                if (l._lodMin != r._lodMin) return l._lodMin < r._lodMin;
                if (l._lodMax != r._lodMax) return l._lodMax < r._lodMax;
                if (l._lodBias != r._lodBias) return l._lodBias < r._lodBias;
                if (l._maxAnisotropy != r._maxAnisotropy) return l._maxAnisotropy < r._maxAnisotropy;
                return l._compareFunc < r._compareFunc;
            }

            public static bool operator >(Parameters l, Parameters r)
            {
                if (l._wrapS != r._wrapS) return l._wrapS > r._wrapS;
                if (l._wrapT != r._wrapT) return l._wrapT > r._wrapT;
                if (l._wrapR != r._wrapR) return l._wrapR > r._wrapR;
                if (l._min != r._min) return l._min > r._min;
                if (l._mag != r._mag) return l._mag > r._mag;
                if (l._borderType != r._borderType) return l._borderType > r._borderType;
                switch (l._borderType)
                {
                    case 0: // i
                    case 2:
                        { // Ii
                            Vector4i ub = new Vector4i(l._border.i0, l._border.i1, l._border.i2, l._border.i3);
                            Vector4i vb = new Vector4i(r._border.i0, r._border.i1, r._border.i2, r._border.i3);
                            if (ub > vb)
                            {
                                return true;
                            }
                            if (vb > ub)
                            {
                                return false;
                            }
                            break;
                        }
                    case 3:
                        { // Iui
                            Vector4ui ub = new Vector4ui(l._border.ui0, l._border.ui1, l._border.ui2, l._border.ui3);
                            Vector4ui vb = new Vector4ui(r._border.ui0, r._border.ui1, r._border.ui2, r._border.ui3);
                            if (ub > vb)
                            {
                                return true;
                            }
                            if (vb > ub)
                            {
                                return false;
                            }
                            break;
                        }
                    case 1:
                        { // f
                            Vector4f ub = new Vector4f(l._border.f0, l._border.f1, l._border.f2, l._border.f3);
                            Vector4f vb = new Vector4f(r._border.f0, r._border.f1, r._border.f2, r._border.f3);
                            if (ub > vb)
                            {
                                return true;
                            }
                            if (vb > ub)
                            {
                                return false;
                            }
                            break;
                        }
                    default:
                        Debug.Assert(false);
                        break;
                }
                if (l._lodMin != r._lodMin) return l._lodMin > r._lodMin;
                if (l._lodMax != r._lodMax) return l._lodMax > r._lodMax;
                if (l._lodBias != r._lodBias) return l._lodBias > r._lodBias;
                if (l._maxAnisotropy != r._maxAnisotropy) return l._maxAnisotropy > r._maxAnisotropy;
                return l._compareFunc > r._compareFunc;
            }

            private TextureWrap _wrapS;

            private TextureWrap _wrapT;

            private TextureWrap _wrapR;

            private TextureFilter _min;

            private TextureFilter _mag;

            [StructLayout(LayoutKind.Explicit)]
            private struct Border
            {
                [FieldOffset(0)]
                public int i0;
                [FieldOffset(4)]
                public int i1;
                [FieldOffset(8)]
                public int i2;
                [FieldOffset(12)]
                public int i3;
                [FieldOffset(0)]
                public uint ui0;
                [FieldOffset(4)]
                public uint ui1;
                [FieldOffset(8)]
                public uint ui2;
                [FieldOffset(12)]
                public uint ui3;
                [FieldOffset(0)]
                public float f0;
                [FieldOffset(4)]
                public float f1;
                [FieldOffset(8)]
                public float f2;
                [FieldOffset(12)]
                public float f3;
            }

            private Border _border;

            private uint _borderType;

            private float _lodMin;

            private float _lodMax;

            private float _lodBias;

            private float _maxAnisotropy;

            private Function _compareFunc;
        }

      
		/// <summary>
		/// Creates a new sampler with the given parameters.
		/// Initializes a new instance of the <see cref="Sxta.Render.Sampler"/> class.
		/// </summary>
		/// <param name='params_'>
		/// Params_. valueC the sampler parameters.
		/// </param>
        public Sampler(Parameters params_)
        {
            Tuple<uint, uint> val;
            if (!INSTANCES.TryGetValue(params_, out val))
            {
#if OPENTK
                GL.GenSamplers(1, out samplerId);
                Debug.Assert(samplerId > 0);
                GL.SamplerParameter(samplerId, SamplerParameter.TextureWrapS, EnumConversion.getTextureWrap(params_.wrapS()));
                GL.SamplerParameter(samplerId, SamplerParameter.TextureWrapT, EnumConversion.getTextureWrap(params_.wrapT()));
                GL.SamplerParameter(samplerId, SamplerParameter.TextureWrapR, EnumConversion.getTextureWrap(params_.wrapR()));
                GL.SamplerParameter(samplerId, SamplerParameter.TextureMinFilter, EnumConversion.getTextureMinFilter(params_.min()));
                GL.SamplerParameter(samplerId, SamplerParameter.TextureMagFilter, EnumConversion.getTextureMagFilter(params_.mag()));
                switch (params_.borderType())
                {
                    case 0: // i
                        GL.SamplerParameter(samplerId, SamplerParameter.TextureBorderColor, params_.borderi());
                        break;
                    case 1: // f
                        GL.SamplerParameter(samplerId, SamplerParameter.TextureBorderColor, params_.borderf());
                        break;
#if TODO                
                    case 2: // Ii
                        GL.SamplerParameterI(samplerId, SamplerParameter.TextureBorderColor, params_.borderIi());
                        break;
                    case 3: // Iui
                        GL.SamplerParameterI(samplerId, SamplerParameter.TextureBorderColor, params_.borderIui());
                        break;
#endif
                    default:
                        Debug.Assert(false);
                        break;
                }
                GL.SamplerParameter(samplerId, SamplerParameter.TextureMinLod, params_.lodMin());
                GL.SamplerParameter(samplerId, SamplerParameter.TextureMaxLod, params_.lodMax());
                GL.SamplerParameter(samplerId, SamplerParameter.TextureLodBias, params_.lodBias());
                if (params_.compareFunc() != Function.ALWAYS)
                {
#if TODO
                    GL.SamplerParameter(samplerId, SamplerParameter.TextureCompareMode, GL_COMPARE_REF_TO_TEXTURE);
                    GL.SamplerParameter(samplerId, SamplerParameter.TextureCompareFunc, EnumConversion.getFunction(params_.compareFunc()));
#endif
                    throw new NotImplementedException();
                }
                GL.SamplerParameter(samplerId, SamplerParameter.TextureMaxAnisotropyExt, params_.maxAnisotropyEXT());
#else
                glGenSamplers(1, &samplerId);
                Debug.Assert(samplerId > 0);
                glSamplerParameteri(samplerId, GL_TEXTURE_WRAP_S, getTextureWrap(params_.wrapS()));
                glSamplerParameteri(samplerId, GL_TEXTURE_WRAP_T, getTextureWrap(params_.wrapT()));
                glSamplerParameteri(samplerId, GL_TEXTURE_WRAP_R, getTextureWrap(params_.wrapR()));
                glSamplerParameteri(samplerId, GL_TEXTURE_MIN_FILTER, getTextureFilter(params_.min()));
                glSamplerParameteri(samplerId, GL_TEXTURE_MAG_FILTER, getTextureFilter(params_.mag()));
                switch (params_.borderType())
                {
                    case 0: // i
                        glSamplerParameteriv(samplerId, GL_TEXTURE_BORDER_COLOR, params_.borderi());
                        break;
                    case 1: // f
                        glSamplerParameterfv(samplerId, GL_TEXTURE_BORDER_COLOR, params_.borderf());
                        break;
                    case 2: // Ii
                        glSamplerParameterIiv(samplerId, GL_TEXTURE_BORDER_COLOR, params_.borderIi());
                        break;
                    case 3: // Iui
                        glSamplerParameterIuiv(samplerId, GL_TEXTURE_BORDER_COLOR, params_.borderIui());
                        break;
                    default:
                        Debug.Assert(false);
                }
                glSamplerParameterf(samplerId, GL_TEXTURE_MIN_LOD, params_.lodMin());
                glSamplerParameterf(samplerId, GL_TEXTURE_MAX_LOD, params_.lodMax());
                glSamplerParameterf(samplerId, GL_TEXTURE_LOD_BIAS, params_.lodBias());
                if (params_.compareFunc() != ALWAYS)
                {
                    glSamplerParameteri(samplerId, GL_TEXTURE_COMPARE_MODE, GL_COMPARE_REF_TO_TEXTURE);
                    glSamplerParameteri(samplerId, GL_TEXTURE_COMPARE_FUNC, getFunction(params_.compareFunc()));
                }
                glSamplerParameterf(samplerId, GL_TEXTURE_MAX_ANISOTROPY_EXT, params_.maxAnisotropyEXT());
#endif
                INSTANCES[params_] = new Tuple<uint, uint>(samplerId, 1);
            }
            else
            {
                samplerId = val.Item1;
                INSTANCES[params_] = new Tuple<uint, uint>(samplerId, val.Item2 + 1);
            }
        }

      
		/// <summary>
		/// Deletes this sampler.
		/// Releases unmanaged resources and performs other cleanup operations before the <see cref="Sxta.Render.Sampler"/> is
		/// reclaimed by garbage collection.
		/// </summary>
        ~Sampler()
        {
            Texture.unbindSampler(this);
            Tuple<uint, uint> val;
            bool found = INSTANCES.TryGetValue(params_, out val);

            Debug.Assert(found); //i != INSTANCES.end());
            Debug.Assert(val.Item1 == samplerId);
            Debug.Assert(val.Item2 >= 1);
            if (val.Item2 == 1)
            {
#if OPENTK
                GL.DeleteSamplers(1, ref samplerId);
#else
                glDeleteSamplers(1, &samplerId);
#endif
                INSTANCES.Remove(params_);
            }
            else
            {
                INSTANCES[params_] = new Tuple<uint, uint>(samplerId, val.Item2 - 1);
            }
        }

       
		/// <summary>
		/// Returns the id of this sampler.
		/// </summary>
		/// <returns>
		/// The identifier.
		/// </returns>
        public uint getId()
        {
            return samplerId;
        }


       
		/// <summary>
		/// The id of this sampler.
		/// </summary>
        private uint samplerId;

      
		/// <summary>
		/// TThe parameters of this sampler.
		/// </summary>
        private Parameters params_;

        
		/// <summary>
		/// The shared texture sampler instances. All the sampler objects with
        /// the same options are represented with the same OpenGL instance. This
        /// map associates the shared sampler id and the corresponding number of
        /// Sampler object to each possible valueC for the sampler parameters.
		/// </summary>
        private static Dictionary<Sampler.Parameters, Tuple<uint, uint>> INSTANCES;
    }
}
