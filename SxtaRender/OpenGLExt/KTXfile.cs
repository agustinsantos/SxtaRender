using OpenTK.Graphics.OpenGL;
using Sxta.Render;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Sxta.Render.OpenGLExt
{
    /// <summary>
    /// libktx is a small library of functions for creating KTX (Khronos
    /// TeXture) files and instantiating OpenGL textures from them.
    /// 
    /// KTX is a format for storing textures for OpenGL and OpenGL ES applications.
    /// It is distinguished by the simplicity of the loader required to instantiate 
    /// a GL texture object from the file contents.
    /// libktx is a small library of functions for creating KTX (Khronos
    /// TeXture) files and instantiating OpenGL textures from them.
    /// 
    /// For information about the KTX format see the
    /// <a href="http://www.khronos.org/opengles/sdk/tools/KTX/file_format_spec/">
    /// formal specification.</a>
    /// 
    /// The library is open source software. Most of the code is licensed under a
    /// modified BSD license. The code for unpacking ETC1, ETC2 and EAC compressed
    /// textures has a separate license that restricts it to uses associated with
    /// Khronos Group APIs.
    /// </summary>
    public static class LibKTX
    {

        /// <summary>
        /// Error codes returned by library functions.
        /// </summary>
        public enum KTX_Error_Code
        {
            KTX_SUCCESS = 0,		 /*  Operation was successful. */
            KTX_FILE_OPEN_FAILED,	 /*  The target file could not be opened. */
            KTX_FILE_WRITE_ERROR,    /*  An error occurred while writing to the file. */
            KTX_GL_ERROR,            /*  GL operations resulted in an error. */
            KTX_INVALID_OPERATION,   /*  The operation is not allowed in the current state. */
            KTX_INVALID_VALUE,	     /* A parameter value was not valid */
            KTX_NOT_FOUND,			 /*  Requested key was not found */
            KTX_OUT_OF_MEMORY,       /*  Not enough memory to complete the operation. */
            KTX_UNEXPECTED_END_OF_FILE, /*  The file did not contain enough data */
            KTX_UNKNOWN_FILE_FORMAT, /* The file not a KTX file */
            KTX_UNSUPPORTED_TEXTURE_TYPE, /*  The KTX file specifies an unsupported texture type. */
        }



        /// <summary>
        /// structure used to pass information about the texture to WriteKTX
        /// </summary>
        public struct KTX_texture_info
        {
            /// <summary>
            /// The type of the image data.
            /// 
            ///  Values are the same as in the type parameter of
            ///  glTexImage*D. Must be 0 for compressed images.
            /// </summary>
            public uint glType;

            /// <summary>
            /// The data type size to be used in case of endianness conversion.
            /// 
            ///  This value is used in the event conversion is required when the
            ///  KTX file is loaded. It should be the size in bytes corresponding
            ///  to glType. Must be 1 for compressed images.
            /// </summary>
            public uint glTypeSize;

            /// <summary>
            /// The format of the image(s).
            /// 
            /// Values are the same as in the format parameter
            /// of glTexImage*D. Must be 0 for compressed images.
            /// </summary>
            public uint glFormat;

            /// <summary>
            /// The internalformat of the image(s).
            /// 
            /// Values are the same as for the internalformat parameter of
            /// glTexImage*2D. Note: it will not be used when a KTX file
            /// containing an uncompressed texture is loaded into OpenGL ES.
            /// </summary>
            public uint glInternalFormat;

            /// <summary>
            /// The base internalformat of the image(s)
            ///
            /// For non-compressed textures, should be the same as glFormat.
            /// For compressed textures specifies the base internal, e.g.
            /// GL_RGB, GL_RGBA.
            /// </summary>
            public uint glBaseInternalFormat;

            /// <summary>
            /// Width of the image for texture level 0, in pixels.
            /// </summary>
            public uint pixelWidth;

            /// <summary>
            /// Height of the texture image for level 0, in pixels.
            ///
            /// Must be 0 for 1D textures.
            /// </summary>
            public uint pixelHeight;

            /// <summary>
            /// Depth of the texture image for level 0, in pixels.
            ///
            /// Must be 0 for 1D, 2D and cube textures.
            /// </summary>
            public uint pixelDepth;

            /// <summary>
            /// The number of array elements.
            ///
            /// Must be 0 if not an array texture.
            /// </summary>
            public uint numberOfArrayElements;

            /// <summary>
            /// The number of cubemap faces.
            ///
            /// Must be 6 for cubemaps and cubemap arrays, 1 otherwise. Cubemap
            /// faces must be provided in the order: +X, -X, +Y, -Y, +Z, -Z.
            /// </summary>
            public uint numberOfFaces;

            /// <summary>
            /// The number of mipmap levels.
            ///
            /// 1 for non-mipmapped texture. 0 indicates that a full mipmap pyramid should
            /// be generated from level 0 at load time (this is usually not allowed for
            /// compressed formats). Mipmaps must be provided in order from largest size to
            /// smallest size. The first mipmap level is always level 0.
            /// </summary>
            public uint numberOfMipmapLevels;
        }


        /// <summary>
        /// Structure used to pass image data to ktxWriteKTX.
        /// </summary>
        public struct KTX_image_info
        {
            public int size;	/* Size of the image data in bytes. */
            public byte[] data;  /* Pointer to the image data. */
        }

        /// <summary>
        /// Structure used to return texture dimensions
        /// </summary>
        public struct KTX_dimensions
        {
            public int width;
            public int height;
            public int depth;
        }


        #region KTX textures read
        /// <summary>
        /// Load a GL texture object from a Stream.
        /// 
        ///  This function will unpack compressed GL_ETC1_RGB8_OES and GL_ETC2_* format
        ///  textures in software when the format is not supported by the GL context,
        ///  provided the library has been compiled with SUPPORT_SOFTWARE_ETC_UNPACK
        ///  defined as 1.
        ///  
        ///  It will also convert textures with legacy formats to their modern equivalents
        ///  when the format is not supported by the GL context, provided that the library
        ///  has been compiled with SUPPORT_LEGACY_FORMAT_CONVERSION defined as 1. 
        /// </summary>
        /// <param name="stream">the  Stream from which to load.</param>
        /// <param name="pTexture">	name of the GL texture to load. If pTexture == 0
        ///                         the function will generate a texture name.
        ///                         The function binds either the
        ///                         generated name or the name given in pTexture
        /// 						to the texture target returned in pTarget,
        /// 						before loading the texture data. If pTexture
        ///                         is not null and a name was generated, the generated
        ///                         name will be returned in pTexture.</param>
        /// <param name="pTarget">pTarget is set to the texture target used. The
        /// 					  target is chosen based on the file contents.</param>
        /// <param name="pDimensions">the width, height and depth of the texture's base level are returned in the
        ///                          fields of the KTX_dimensions structure.</param>
        /// <param name="pIsMipmapped">pIsMipmapped is set to TRUE if the KTX texture is mipmapped, DALSE
        ///                           otherwise.</param>
        /// <param name="pGlerror">pGlerror is set to the value returned by
        ///                          glGetError when this function returns the error
        ///                          KTX_GL_ERROR. </param>
        /// <param name="pKvdLen"> pKvdLen is set to the number of bytes
        ///                        of key-value data pointed at by ppKvd. Must not be
        ///                        null, ifppKvd is not null.</param>
        /// <param name="ppKvd">If not null,ppKvd is set to the point to a block of
        ///                     memory containing key-value data read from the file.
        ///                     The application is responsible for freeing the memory.</param>
        /// <returns>KTX_SUCCESS on success, other KTX_Error_Code enum values on error.</returns>
        public static KTX_Error_Code LoadTexture(Stream stream, ref uint pTexture, out TextureTarget pTarget,
                                                    out KTX_dimensions pDimensions, out bool pIsMipmapped,
                                                    out  ErrorCode pGlerror, ref  int pKvdLen, ref byte[] ppKvd)
        {
            pTarget = TextureTarget.Texture2D;
            pGlerror = ErrorCode.NoError;
            pDimensions = new KTX_dimensions();
            pIsMipmapped = false;

            int previousUnpackAlignment;
            KTX_header header;
            KTX_texinfo texinfo = new KTX_texinfo();
            byte[] data = null;
            uint dataSize = 0;
            uint texname;
            int texnameUser;
            uint faceLodSize;
            uint faceLodSizeRounded;
            uint level;
            uint face;
            PixelFormat glFormat;
            PixelInternalFormat glInternalFormat;
            KTX_Error_Code errorCode = KTX_Error_Code.KTX_SUCCESS;
            ErrorCode errorTmp;

            if (pGlerror != ErrorCode.NoError)
                pGlerror = ErrorCode.NoError;

            if (ppKvd != null)
            {
                ppKvd = null;
            }

            if (stream == null || !stream.CanRead || !stream.CanSeek)
            {
                return KTX_Error_Code.KTX_INVALID_VALUE;
            }

            header = KTX_header.ReadHeader(stream);

            errorCode = KTX_header.CheckHeader(header, ref texinfo);
            if (errorCode != KTX_Error_Code.KTX_SUCCESS)
            {
                return errorCode;
            }

            if (ppKvd != null)
            {
                if (pKvdLen == 0)
                    return KTX_Error_Code.KTX_INVALID_OPERATION;

                pKvdLen = (int)header.bytesOfKeyValueData;
                if (pKvdLen != 0)
                {
                    ppKvd = new byte[pKvdLen];
                    if (ppKvd == null)
                        return KTX_Error_Code.KTX_OUT_OF_MEMORY;
                    if (stream.Read(ppKvd, 0, pKvdLen) == 0)
                    {
                        //free(*ppKvd);
                        ppKvd = null;

                        return KTX_Error_Code.KTX_UNEXPECTED_END_OF_FILE;
                    }
                }
            }
            else
            {
                /* skip key/value metadata */
                stream.Position += header.bytesOfKeyValueData;
                if (stream.Position > stream.Length)
                {
                    return KTX_Error_Code.KTX_UNEXPECTED_END_OF_FILE;
                }
            }

            if (contextProfile == 0)
                DiscoverContextCapabilities();

            /* KTX files require an unpack alignment of 4 */
            GL.GetInteger(GetPName.UnpackAlignment, out previousUnpackAlignment);
            if (previousUnpackAlignment != KTX_GL_UNPACK_ALIGNMENT)
            {
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, KTX_GL_UNPACK_ALIGNMENT);
            }


            texnameUser = (int)pTexture;
            if (texnameUser != 0)
            {
                texname = pTexture;
            }
            else
            {
                GL.GenTextures(1, out texname);
            }
            GL.BindTexture(texinfo.glTarget, texname);

            //OpenTK supports GenerateMipmap for OpenGL >= 3.0
            const bool SupportedGenerateMipmap = true;

            // Prefer glGenerateMipmaps over GL_GENERATE_MIPMAP
            if (texinfo.generateMipmaps && (SupportedGenerateMipmap))
            {
                GL.TexParameter(texinfo.glTarget, TextureParameterName.GenerateMipmap, 1);
            }

            if (texinfo.glTarget == TextureTarget.TextureCubeMap)
            {
                texinfo.glTarget = TextureTarget.TextureCubeMapPositiveX;
            }

            glInternalFormat = (PixelInternalFormat)header.glInternalFormat;
            glFormat = (PixelFormat)header.glFormat;
            if (!texinfo.compressed)
            {
#if SUPPORT_LEGACY_FORMAT_CONVERSION
		// If sized legacy formats are supported there is no need to convert.
		// If only unsized formats are supported, there is no point in converting
		// as the modern formats aren't supported either.
		if (sizedFormats == _NON_LEGACY_FORMATS && supportsSwizzle) {
			convertFormat(texinfo.glTarget, &glFormat, &glInternalFormat);
			errorTmp = glGetError();
		} else if (sizedFormats == _NO_SIZED_FORMATS)
			glInternalFormat = header.glBaseInternalFormat;
#else
                /*
                 * These defines are needed to compile the KTX library.
                 * When these things are not available in the GL version in
                 * use at runtime, the library either provides its own support
                 * or handles the expected errors.
                 */
                const int GL_ALPHA = 0x1906;
                const int GL_LUMINANCE = 0x1909;
                const int GL_LUMINANCE_ALPHA = 0x190A;
                const int GL_INTENSITY = 0x8049;

                // With only unsized formats must change internal format.
                if (sizedFormats == _NO_SIZED_FORMATS
                    || ((sizedFormats & _LEGACY_FORMATS) == 0 && (header.glBaseInternalFormat == GL_ALPHA
                    || header.glBaseInternalFormat == GL_LUMINANCE
                    || header.glBaseInternalFormat == GL_LUMINANCE_ALPHA
                    || header.glBaseInternalFormat == GL_INTENSITY)))
                {
                    glInternalFormat = (PixelInternalFormat)header.glBaseInternalFormat;
                }
#endif
            }

            for (level = 0; level < header.numberOfMipmapLevels; ++level)
            {
                int pixelWidth = System.Math.Max(1, (int)header.pixelWidth >> (int)level);
                int pixelHeight = System.Math.Max(1, (int)header.pixelHeight >> (int)level);
                int pixelDepth = System.Math.Max(1, (int)header.pixelDepth >> (int)level);

                faceLodSize = ReadUInt(stream, header.endianness);

                faceLodSizeRounded = (faceLodSize + 3) & ~(uint)3;
                if (data == null)
                {
                    /* allocate memory sufficient for the first level */
                    data = new byte[faceLodSizeRounded];
                    if (data == null)
                    {
                        errorCode = KTX_Error_Code.KTX_OUT_OF_MEMORY;
                        goto cleanup;
                    }
                    dataSize = faceLodSizeRounded;
                }
                else if (dataSize < faceLodSizeRounded)
                {
                    /* subsequent levels cannot be larger than the first level */
                    errorCode = KTX_Error_Code.KTX_INVALID_VALUE;
                    goto cleanup;
                }

                for (face = 0; face < header.numberOfFaces; ++face)
                {
                    stream.Read(data, 0, (int)faceLodSizeRounded);

                    /* Perform endianness conversion on texture data */
                    if (header.endianness == KTX_header.KTX_ENDIAN_REF_REV && header.glTypeSize == 2)
                    {
                        SwapEndianArr16(data, faceLodSize / 2);
                    }
                    else if (header.endianness == KTX_header.KTX_ENDIAN_REF_REV && header.glTypeSize == 4)
                    {
                        SwapEndianArr32(data, faceLodSize / 4);
                    }

                    if (texinfo.textureDimensions == 1)
                    {
                        if (texinfo.compressed)
                        {
                            GL.CompressedTexImage1D((TextureTarget)((int)texinfo.glTarget + face), (int)level,
                                                    glInternalFormat, pixelWidth, 0, (int)faceLodSize, data);
                        }
                        else
                        {
                            GL.TexImage1D((TextureTarget)((int)texinfo.glTarget + face), (int)level,
                                        glInternalFormat, pixelWidth, 0, glFormat, (OpenTK.Graphics.OpenGL.PixelType)header.glType, data);
                        }
                    }
                    else if (texinfo.textureDimensions == 2)
                    {
                        if (header.numberOfArrayElements != 0)
                        {
                            pixelHeight = (int)header.numberOfArrayElements;
                        }
                        if (texinfo.compressed)
                        {
                            // It is simpler to just attempt to load the format, rather than divine which
                            // formats are supported by the implementation. In the event of an error,
                            // software unpacking can be attempted.
                            GL.CompressedTexImage2D((TextureTarget)((int)texinfo.glTarget + face), (int)level,
                                                    glInternalFormat, pixelWidth, pixelHeight, 0, (int)faceLodSize, data);
                        }
                        else
                        {
                            GL.TexImage2D((TextureTarget)((int)texinfo.glTarget + face), (int)level,
                                            glInternalFormat, pixelWidth, pixelHeight, 0, glFormat, (OpenTK.Graphics.OpenGL.PixelType)header.glType, data);
                        }
                    }
                    else if (texinfo.textureDimensions == 3)
                    {
                        if (header.numberOfArrayElements != 0)
                        {
                            pixelDepth = (int)header.numberOfArrayElements;
                        }
                        if (texinfo.compressed)
                        {
                            GL.CompressedTexImage3D((TextureTarget)((int)texinfo.glTarget + face), (int)level,
                                                    glInternalFormat, pixelWidth, pixelHeight, pixelDepth, 0, (int)header.glType, data);
                        }
                        else
                        {
                            GL.TexImage3D((TextureTarget)((int)texinfo.glTarget + face), (int)level,
                                            glInternalFormat, pixelWidth, pixelHeight, pixelDepth, 0,
                                            glFormat, (OpenTK.Graphics.OpenGL.PixelType)header.glType, data);
                        }
                    }

                    errorTmp = GL.GetError();

#if SUPPORT_SOFTWARE_ETC_UNPACK
			        // Renderion is returning INVALID_VALUE. Oops!!
			        if ((errorTmp == GL_INVALID_ENUM || errorTmp == GL_INVALID_VALUE)
				        && texinfo.compressed
				        && texinfo.textureDimensions == 2
				        && (glInternalFormat == GL_ETC1_RGB8_OES || (glInternalFormat >= GL_COMPRESSED_R11_EAC && glInternalFormat <= GL_COMPRESSED_SRGB8_ALPHA8_ETC2_EAC)))
			            {
				        GLubyte* unpacked;
				        GLenum format, internalFormat, type;

				        errorCode = _ktxUnpackETC((GLubyte*)data, glInternalFormat, pixelWidth, pixelHeight,
					                              &unpacked, &format, &internalFormat, &type,
										          R16Formats, supportsSRGB);
				        if (errorCode != KTX_SUCCESS) {
					        goto cleanup;
				        }
				        if (!sizedFormats & _NON_LEGACY_FORMATS) {
					        if (internalFormat == GL_RGB8)
						        internalFormat = GL_RGB;
					        else if (internalFormat == GL_RGBA8)
						        internalFormat = GL_RGBA;
				        }
				        glTexImage2D(texinfo.glTarget + face, level, 
							         internalFormat, pixelWidth, pixelHeight, 0, 
							         format, type, unpacked);

				        free(unpacked);
				        errorTmp = glGetError();
			        }
#endif
                    if (errorTmp != ErrorCode.NoError)
                    {
                        if (pGlerror != 0)
                            pGlerror = errorTmp;
                        errorCode = KTX_Error_Code.KTX_GL_ERROR;
                        goto cleanup;
                    }

                }

            }


        cleanup:
            data = null;

            /* restore previous GL state */
            if (previousUnpackAlignment != KTX_GL_UNPACK_ALIGNMENT)
            {
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, previousUnpackAlignment);
            }

            if (errorCode == KTX_Error_Code.KTX_SUCCESS)
            {
                if (texinfo.generateMipmaps && SupportedGenerateMipmap)
                {
                    GL.GenerateMipmap((GenerateMipmapTarget)texinfo.glTarget);
                }
                pTarget = texinfo.glTarget;
                if (pTexture != 0)
                {
                    pTexture = texname;
                }
                pDimensions.width = (int)header.pixelWidth;
                pDimensions.height = (int)header.pixelHeight;
                pDimensions.depth = (int)header.pixelDepth;
                if (pIsMipmapped)
                {
                    if (texinfo.generateMipmaps || header.numberOfMipmapLevels > 1)
                        pIsMipmapped = true;
                    else
                        pIsMipmapped = false;
                }
            }
            else
            {
                if (ppKvd != null)
                {
                    ppKvd = null;
                }

                if (texnameUser == 0)
                {
                    GL.DeleteTextures(1, ref texname);
                }
            }

            return errorCode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename">A string that contains the path of the file to load.</param>
        /// <param name="pTexture">	name of the GL texture to load. If pTexture == 0
        ///                         the function will generate a texture name.
        ///                         The function binds either the
        ///                         generated name or the name given in pTexture
        /// 						to the texture target returned in pTarget,
        /// 						before loading the texture data. If pTexture
        ///                         is not null and a name was generated, the generated
        ///                         name will be returned in pTexture.</param>
        /// <param name="pTarget">pTarget is set to the texture target used. The
        /// 					  target is chosen based on the file contents.</param>
        /// <param name="pDimensions">the width, height and depth of the texture's base level are returned in the
        ///                          fields of the KTX_dimensions structure.</param>
        /// <param name="pIsMipmapped">pIsMipmapped is set to TRUE if the KTX texture is mipmapped, DALSE
        ///                           otherwise.</param>
        /// <param name="pGlerror">pGlerror is set to the value returned by
        ///                          glGetError when this function returns the error
        ///                          KTX_GL_ERROR. </param>
        /// <param name="pKvdLen"> pKvdLen is set to the number of bytes
        ///                        of key-value data pointed at by ppKvd. Must not be
        ///                        null, ifppKvd is not null.</param>
        /// <param name="ppKvd">If not null,ppKvd is set to the point to a block of
        ///                     memory containing key-value data read from the file.
        ///                     The application is responsible for freeing the memory.</param>
        /// <returns>KTX_SUCCESS on success, other KTX_Error_Code enum values on error.</returns>
        public static KTX_Error_Code LoadTexture(string filename, ref uint pTexture, out TextureTarget pTarget,
                                            out KTX_dimensions pDimensions, out bool pIsMipmapped,
                                            out  ErrorCode pGlerror, ref  int pKvdLen, ref byte[] ppKvd)
        {
            KTX_Error_Code errorCode;
            using (FileStream stream = new FileStream("Media/rgb-reference.ktx", FileMode.Open))
            {
                errorCode = LoadTexture(stream, ref pTexture, out pTarget, out pDimensions,
                                          out pIsMipmapped, out pGlerror, ref pKvdLen, ref ppKvd);
            }
            return errorCode;
        }

        #endregion

        #region private or internal
        /// <summary>
        /// Required unpack alignment
        /// </summary>
        public const int KTX_GL_UNPACK_ALIGNMENT = 4;
        /// <summary>
        /// Additional contextProfile bit indicating an OpenGL ES context.
        /// 
        /// This is the same value NVIDIA returns when using an OpenGL ES profile
        /// of their desktop drivers. However it is not specified in any official
        /// specification as OpenGL ES does not support the GL_CONTEXT_PROFILE_MASK
        /// query.
        /// </summary>
        private const int _CONTEXT_ES_PROFILE_BIT = 0x4;

        /// <summary>
        /// These values describe values that may be used with the sizedFormats
        ///  variable.
        /// </summary>
        private const int _NON_LEGACY_FORMATS = 0x1; /*Non-legacy sized formats are supported. */
        private const int _LEGACY_FORMATS = 0x2;  /*Legacy sized formats are supported. */

        /// <summary>
        /// All sized formats are supported
        /// </summary>
        private const int _ALL_SIZED_FORMATS = (_NON_LEGACY_FORMATS | _LEGACY_FORMATS);
        private const int _NO_SIZED_FORMATS = 0; /*No sized formats are supported. */

        /// <summary>
        /// used to pass GL context capabilites to subroutines.
        /// </summary>
        private const int _KTX_NO_R16_FORMATS = 0x0;
        private const int _KTX_R16_FORMATS_NORM = 0x1;
        private const int _KTX_R16_FORMATS_SNORM = 0x2;
        private const int _KTX_ALL_R16_FORMATS = (_KTX_R16_FORMATS_NORM | _KTX_R16_FORMATS_SNORM);

        /// <summary>
        /// indicates the profile of the current context.
        /// </summary>
        private static int contextProfile = 0;

        /// <summary>
        /// indicates what sized texture formats are supported
        /// by the current context.
        /// </summary>
        private static int sizedFormats = _ALL_SIZED_FORMATS;
        private static bool supportsSwizzle = true;

        /// <summary>
        /// indicates which R16 & RG16 formats are supported by the current context.
        /// </summary>
        private static int R16Formats = _KTX_ALL_R16_FORMATS;

        /// <summary>
        /// indicates if the current context supports sRGB textures.
        /// </summary>
        private static bool supportsSRGB = true;

        /// <summary>
        /// Discover the capabilities of the current GL context.
        /// Queries the context and sets several the following internal variables indicating
        /// the capabilities of the context:
        /// </summary>
        private static void DiscoverContextCapabilities()
        {
            int majorVersion = 1;
            int minorVersion = 0;
            string extensions = GL.GetString(StringName.Extensions);

            if (GL.GetString(StringName.Version).StartsWith("GL ES"))
                contextProfile = _CONTEXT_ES_PROFILE_BIT;
            // MAJOR & MINOR only introduced in GL {,ES} 3.0
            GL.GetInteger(GetPName.MajorVersion, out majorVersion);
            GL.GetInteger(GetPName.MinorVersion, out minorVersion);
            if (GL.GetError() != ErrorCode.NoError)
            {
#if TODO
                // < v3.0; resort to the old-fashioned way.
                if (contextProfile & _CONTEXT_ES_PROFILE_BIT)
                    sscanf(GL.GetString(StringName.Version), "OpenGL ES %d.%d ", &majorVersion, &minorVersion);
                else
                    sscanf(GL.GetString(StringName.Version), "OpenGL %d.%d ", &majorVersion, &minorVersion);
#endif
                throw new NotSupportedException("Only support a OpenGL version higher than 3.0");
            }
            if ((contextProfile & _CONTEXT_ES_PROFILE_BIT) != 0)
            {
                if (majorVersion < 3)
                {
                    supportsSwizzle = false;
                    sizedFormats = _NO_SIZED_FORMATS;
                    R16Formats = _KTX_NO_R16_FORMATS;
                    supportsSRGB = false;
                }
                else
                {
                    sizedFormats = _NON_LEGACY_FORMATS;
                }
                if (extensions.Contains("GL_OES_required_internalformat"))
                {
                    sizedFormats |= _ALL_SIZED_FORMATS;
                }
                // There are no OES extensions for sRGB textures or R16 formats.
            }
            else
            {
                // TODO. OpenTK does not have defined this constants :-/
                const int GL_CONTEXT_PROFILE_MASK = 0x9126;
                const int GL_CONTEXT_CORE_PROFILE_BIT = 0x00000001;
                const int GL_CONTEXT_COMPATIBILITY_PROFILE_BIT = 0x00000002;
                // PROFILE_MASK was introduced in OpenGL 3.2.
                // Profiles: CONTEXT_CORE_PROFILE_BIT 0x1, CONTEXT_COMPATIBILITY_PROFILE_BIT 0x2.
                GL.GetInteger((GetPName)GL_CONTEXT_PROFILE_MASK, out contextProfile);

                if (GL.GetError() == ErrorCode.NoError)
                {
                    // >= 3.2
                    if (majorVersion == 3 && minorVersion < 3)
                        supportsSwizzle = false;
                    if ((contextProfile & GL_CONTEXT_CORE_PROFILE_BIT) != 0)
                        sizedFormats &= ~_LEGACY_FORMATS;
                }
                else
                {
                    // < 3.2
                    contextProfile = GL_CONTEXT_COMPATIBILITY_PROFILE_BIT;
                    supportsSwizzle = false;
                    // sRGB textures introduced in 2.0
                    if (majorVersion < 2 && !extensions.Contains("GL_EXT_texture_sRGB"))
                    {
                        supportsSRGB = false;
                    }
                    // R{,G]16 introduced in 3.0; R{,G}16_SNORM introduced in 3.1.
                    if (majorVersion == 3)
                    {
                        if (minorVersion == 0)
                            R16Formats &= ~_KTX_R16_FORMATS_SNORM;
                    }
                    else if (extensions.Contains("GL_ARB_texture_rg"))
                    {
                        R16Formats &= ~_KTX_R16_FORMATS_SNORM;
                    }
                    else
                    {
                        R16Formats = _KTX_NO_R16_FORMATS;
                    }
                }
            }
        }

        private static uint ReadUInt(Stream stream, uint endianness)
        {
            byte[] buff = new byte[sizeof(uint)];
            stream.Read(buff, 0, sizeof(uint));
            uint val = BitConverter.ToUInt32(buff, 0);
            if (endianness == KTX_header.KTX_ENDIAN_REF_REV)
            {
                uint x = val;
                val = (x << 24) | ((x & 0xFF00) << 8) | ((x & 0xFF0000) >> 8) | (x >> 24);
            }

            return val;
        }

        private static void SwapEndianArr16(byte[] data, uint len)
        {
            int cnt = 0;
            for (uint i = 0; i < len; i++)
            {
                byte b0 = data[cnt];
                byte b1 = data[cnt + 1];
                data[cnt] = b1;
                data[cnt + 1] = b0;
                cnt += 2;
            }
        }

        private static void SwapEndianArr32(byte[] data, uint len)
        {
            int cnt = 0;
            for (uint i = 0; i < len; i++)
            {
                byte b0 = data[cnt];
                byte b1 = data[cnt + 1];
                byte b2 = data[cnt + 2];
                byte b3 = data[cnt + 3];
                data[cnt] = b3;
                data[cnt + 1] = b2;
                data[cnt + 2] = b1;
                data[cnt + 3] = b0;
                cnt += 4;
            }
        }
        #endregion

        /*
 * @~English
 * @brief Write image(s) in a KTX-formatted stdio FILE stream.
 *
 * @param [in] dst		    pointer to the FILE stream to write to.
 * @param [in] textureInfo  pointer to a KTX_image_info structure providing
 *                          information about the images to be included in
 *                          the KTX file.
 * @param [in] bytesOfKeyValueData
 *                          specifies the number of bytes of key-value data.
 * @param [in] keyValueData a pointer to the keyValue data.
 * @param [in] numImages    number of images in the following array
 * @param [in] images       array of KTX_image_info providing image size and
 *                          data.
 *
 * @return	KTX_SUCCESS on success, other KTX_* enum values on error.
 *
 * @exception KTX_INVALID_VALUE @p dst or @p target are @c null
 * @exception KTX_INVALID_VALUE @c glTypeSize in @p textureInfo is not 1, 2, or 4 or
 *                              is different from the size of the type specified
 *                              in @c glType.
 * @exception KTX_INVALID_VALUE @c pixelWidth in @p textureInfo is 0 or pixelDepth != 0
 *                              && pixelHeight == 0.
 * @exception KTX_INVALID_VALUE @c numberOfFaces != 1 || numberOfFaces != 6 or
 *                              numberOfArrayElements or numberOfMipmapLevels are < 0.
 * @exception KTX_INVALID_OPERATION
 *                              numberOfFaces == 6 and images are either not 2D or
 *                              are not square.
 * @exception KTX_INVALID_OPERATION
 *							    number of images is insufficient for the specified
 *                              number of mipmap levels and faces.
 * @exception KTX_INVALID_OPERATION
 *                              the size of a provided image is different than that
 *                              required for the specified width, height or depth
 *                              or for the mipmap level being processed.
 * @exception KTX_FILE_WRITE_ERROR a system error occurred while writing the file.
 */
        public static KTX_Error_Code WriteKTX(Stream dst, KTX_texture_info textureInfo,
                                               uint bytesOfKeyValueData, byte[] keyValueData,
                                               int numImages, KTX_image_info[] images)
        {
            KTX_header header = new KTX_header();
            uint i, level, dimension, cubemap = 0;
            uint numMipmapLevels, numArrayElements;
            byte[] pad = new byte[4] { 0, 0, 0, 0 };
            KTX_Error_Code errorCode = KTX_Error_Code.KTX_SUCCESS;
            bool compressed = false;

            if (dst == null)
            {
                return KTX_Error_Code.KTX_INVALID_VALUE;
            }

            //endianess int.. if this comes out reversed, all of the other ints will too.
            header.endianness = KTX_header.KTX_ENDIAN_REF;
            header.glType = textureInfo.glType;
            header.glTypeSize = textureInfo.glTypeSize;
            header.glFormat = textureInfo.glFormat;
            header.glInternalFormat = textureInfo.glInternalFormat;
            header.glBaseInternalFormat = textureInfo.glBaseInternalFormat;
            header.pixelWidth = textureInfo.pixelWidth;
            header.pixelHeight = textureInfo.pixelHeight;
            header.pixelDepth = textureInfo.pixelDepth;
            header.numberOfArrayElements = textureInfo.numberOfArrayElements;
            header.numberOfFaces = textureInfo.numberOfFaces;
            header.numberOfMipmapLevels = textureInfo.numberOfMipmapLevels;
            header.bytesOfKeyValueData = bytesOfKeyValueData;

            /* Do some sanity checking */
            if (header.glTypeSize != 1 &&
                header.glTypeSize != 2 &&
                header.glTypeSize != 4)
            {
                /* Only 8, 16, and 32-bit types supported so far */
                return KTX_Error_Code.KTX_INVALID_VALUE;
            }
            if (header.glTypeSize != SizeofGLtype((OpenTK.Graphics.OpenGL.PixelType)header.glType))
                return KTX_Error_Code.KTX_INVALID_VALUE;

            if (header.glType == 0 || header.glFormat == 0)
            {
                if (header.glType + header.glFormat != 0)
                {
                    /* either both or none of glType & glFormat must be zero */
                    return KTX_Error_Code.KTX_INVALID_VALUE;
                }
                else
                    compressed = true;

            }

            /* Check texture dimensions. KTX files can store 8 types of textures:
             * 1D, 2D, 3D, cube, and array variants of these. There is currently
             * no GL extension that would accept 3D array or cube array textures
             * but we'll let such files be created.
             */
            if ((header.pixelWidth == 0) ||
                (header.pixelDepth > 0 && header.pixelHeight == 0))
            {
                /* texture must have width */
                /* texture must have height if it has depth */
                return KTX_Error_Code.KTX_INVALID_VALUE;
            }
            if (header.pixelHeight > 0 && header.pixelDepth > 0)
                dimension = 3;
            else if (header.pixelHeight > 0)
                dimension = 2;
            else
                dimension = 1;

            if (header.numberOfFaces == 6)
            {
                if (dimension != 2)
                {
                    /* cube map needs 2D faces */
                    return KTX_Error_Code.KTX_INVALID_OPERATION;
                }
                if (header.pixelWidth != header.pixelHeight)
                {
                    /* cube maps require square images */
                    return KTX_Error_Code.KTX_INVALID_OPERATION;
                }
            }
            else if (header.numberOfFaces != 1)
            {
                /* numberOfFaces must be either 1 or 6 */
                return KTX_Error_Code.KTX_INVALID_VALUE;
            }

            if (header.numberOfArrayElements < 0 || header.numberOfMipmapLevels < 0)
                return KTX_Error_Code.KTX_INVALID_VALUE;

            if (header.numberOfArrayElements == 0)
            {
                numArrayElements = 1;
                if (header.numberOfFaces == 6)
                    cubemap = 1;
            }
            else
                numArrayElements = header.numberOfArrayElements;

            /* Check number of mipmap levels */
            if (header.numberOfMipmapLevels == 0)
            {
                numMipmapLevels = 1;
            }
            else
                numMipmapLevels = header.numberOfMipmapLevels;
            if (numMipmapLevels > 1)
            {
                uint max_dim = System.Math.Max(System.Math.Max(header.pixelWidth, header.pixelHeight), header.pixelDepth);
                if (max_dim < (1 << (int)(header.numberOfMipmapLevels - 1)))
                {
                    /* Can't have more mip levels than 1 + log2(max(width, height, depth)) */
                    return KTX_Error_Code.KTX_INVALID_VALUE;
                }
            }

            if (numImages < numMipmapLevels * header.numberOfFaces)
            {
                /* Not enough images */
                return KTX_Error_Code.KTX_INVALID_OPERATION;
            }

            //write header
            KTX_header.WriteHeader(dst, header);

            //write keyValueData
            if (bytesOfKeyValueData != 0)
            {
                if (keyValueData == null)
                    return KTX_Error_Code.KTX_INVALID_OPERATION;

                dst.Write(keyValueData, 0, (int)bytesOfKeyValueData);
            }

            /* Write the image data */
            for (level = 0, i = 0; level < numMipmapLevels; ++level)
            {
                int elementBytes;
                int expectedFaceSize;
                uint face, faceLodSize, faceLodRounding;
                int groupBytes = GroupSize((PixelFormat)header.glFormat,
                                              (OpenTK.Graphics.OpenGL.PixelType)header.glType,
                                              out elementBytes);
                uint pixelWidth, pixelHeight, pixelDepth;
                uint packedRowBytes = 0, rowBytes = 0, rowRounding = 0;


                pixelWidth = System.Math.Max(1, header.pixelWidth >> (int)level);
                pixelHeight = System.Math.Max(1, header.pixelHeight >> (int)level);
                pixelDepth = System.Math.Max(1, header.pixelDepth >> (int)level);

                /* Calculate face sizes for this LoD based on glType, glFormat, width & height */
                expectedFaceSize = (int)(groupBytes
                                           * pixelWidth
                                           * pixelHeight
                                           * pixelDepth
                                           * numArrayElements);

                rowRounding = 0;
                packedRowBytes = (uint)(groupBytes * pixelWidth);
                /* KTX format specifies UNPACK_ALIGNMENT==4 */
                if (!compressed && elementBytes < KTX_GL_UNPACK_ALIGNMENT)
                {
                    rowBytes = (uint)(KTX_GL_UNPACK_ALIGNMENT / elementBytes);
                    /* The following statement is equivalent to:
                    /*     packedRowBytes *= ceil((groupBytes * width) / KTX_GL_UNPACK_ALIGNMENT);
                     */
                    rowBytes *= (uint)(((groupBytes * pixelWidth) + (KTX_GL_UNPACK_ALIGNMENT - 1)) / KTX_GL_UNPACK_ALIGNMENT);
                    rowRounding = rowBytes - packedRowBytes;
                }

                if (rowRounding == 0)
                {
                    faceLodSize = (uint)(images[i].size);
                }
                else
                {
                    /* Need to pad the rows to meet the required UNPACK_ALIGNMENT */
                    faceLodSize = rowBytes * pixelHeight * pixelDepth * numArrayElements;
                }
                faceLodRounding = 3 - ((faceLodSize + 3) % 4);

                dst.Write(BitConverter.GetBytes(faceLodSize), 0, sizeof(uint));

                for (face = 0; face < header.numberOfFaces; ++face, ++i)
                {
                    if (!compressed)
                    {
                        /* Sanity check. */
                        if (images[i].size != expectedFaceSize)
                        {
                            errorCode = KTX_Error_Code.KTX_INVALID_OPERATION;
                            goto cleanup;
                        }
                    }
                    if (rowRounding == 0)
                    {
                        /* Can write whole face at once */
                        dst.Write(images[i].data, 0, (int)faceLodSize);
                    }
                    else
                    {
                        /* Write the rows individually, padding each one */
                        uint row;
                        uint numRows = pixelHeight
                                       * pixelDepth
                                       * numArrayElements;
                        for (row = 0; row < numRows; row++)
                        {
                            dst.Write(images[i].data, (int)(row * packedRowBytes), (int)packedRowBytes);
                            dst.Write(pad, 0, (int)rowRounding);
                        }
                    }
                    if (faceLodRounding != 0)
                    {
                        dst.Write(pad, 0, (int)faceLodRounding);
                    }
                }
            }

        cleanup:
            return errorCode;
        }

        /*
         * @~English
         * @brief Write image(s) to a KTX file on disk.
         *
         * @param [in] dstname		pointer to a C string that contains the path of
         * 							the file to load.
         * @param [in] textureInfo  pointer to a KTX_image_info structure providing
         *                          information about the images to be included in
         *                          the KTX file.
         * @param [in] bytesOfKeyValueData
         *                          specifies the number of bytes of key-value data.
         * @param [in] keyValueData a pointer to the keyValue data.
         * @param [in] numImages    number of images in the following array.
         * @param [in] images       array of KTX_image_info providing image size and
         *                          data.
         *
         * @return	KTX_SUCCESS on success, other KTX_* enum values on error.
         *
         * @exception KTX_FILE_OPEN_FAILED unable to open the specified file for
         *                                 writing.
         *
         * For other exceptions, see ktxWriteKTXF().
         */
        /// <summary>
        /// Write image(s) to a KTX file on disk.
        /// </summary>
        /// <param name="dstname">string that contains the path of
        /// 					  the file to load.</param>
        /// <param name="textureInfo"></param>
        /// <param name="bytesOfKeyValueData"></param>
        /// <param name="keyValueData"></param>
        /// <param name="numImages"></param>
        /// <param name="images"></param>
        /// <returns></returns>
        public static KTX_Error_Code WriteKTX(string dstname, KTX_texture_info textureInfo,
                                               uint bytesOfKeyValueData, byte[] keyValueData,
                                               int numImages, KTX_image_info[] images)
        {
            KTX_Error_Code errorCode = KTX_Error_Code.KTX_SUCCESS;
            using (FileStream dst = new FileStream(dstname, FileMode.OpenOrCreate))
            {
                errorCode = WriteKTX(dst, textureInfo, bytesOfKeyValueData, keyValueData,
                                         numImages, images);

                return errorCode;
            }
        }

        /// <summary>
        /// Return the size of the group of elements constituting a pixel.
        /// </summary>
        /// <param name="format">the format of the image data</param>
        /// <param name="type">the type of the image data</param>
        /// <param name="elementBytes">the size in bytes or < 0 if the type, format or combination
        ///          is invalid.</param>
        /// <returns></returns>
        private static int GroupSize(PixelFormat format, OpenTK.Graphics.OpenGL.PixelType type, out int elementBytes)
        {
            elementBytes = -1;
            switch (format)
            {
                case PixelFormat.Alpha:
                case PixelFormat.Red:
                case PixelFormat.Green:
                case PixelFormat.Blue:
                case PixelFormat.Luminance:
                    elementBytes = SizeofGLtype(type);
                    return elementBytes;
                case PixelFormat.LuminanceAlpha:
                case PixelFormat.Rg:
                    elementBytes = SizeofGLtype(type);
                    return elementBytes * 2;
                case PixelFormat.Rgb:
                case PixelFormat.Bgr:
                    if (type == OpenTK.Graphics.OpenGL.PixelType.UnsignedShort565)
                    {
                        elementBytes = 2;
                        return elementBytes;
                    }
                    else
                    {
                        elementBytes = SizeofGLtype(type);
                        return elementBytes * 3;
                    }
                case PixelFormat.Bgra:
                case PixelFormat.Rgba:
                    if (type == OpenTK.Graphics.OpenGL.PixelType.UnsignedShort4444 ||
                        type == OpenTK.Graphics.OpenGL.PixelType.UnsignedShort5551)
                    {
                        elementBytes = 2;
                        return elementBytes;
                    }
                    else
                    {
                        elementBytes = SizeofGLtype(type);
                        return elementBytes * 4;
                    }
                default:
                    break;
            }

            return elementBytes;
        }


        /// <summary>
        /// Return the sizeof the GL type in basic machine units
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static int SizeofGLtype(OpenTK.Graphics.OpenGL.PixelType type)
        {
            OpenTK.Graphics.OpenGL.PixelType pixelType = (OpenTK.Graphics.OpenGL.PixelType)type;
            switch (pixelType)
            {
                case OpenTK.Graphics.OpenGL.PixelType.Byte:
                case OpenTK.Graphics.OpenGL.PixelType.UnsignedByte:
                    return sizeof(byte);
                case OpenTK.Graphics.OpenGL.PixelType.Short:
                    return sizeof(short);
                case OpenTK.Graphics.OpenGL.PixelType.UnsignedShort:
                    return sizeof(ushort);
                case OpenTK.Graphics.OpenGL.PixelType.Int:
                    return sizeof(int);
                case OpenTK.Graphics.OpenGL.PixelType.UnsignedInt:
                    return sizeof(uint);
                //case OpenTK.Graphics.OpenGL.PixelType.HalfFloat:
                //    return sizeof(GLhalf);
                case OpenTK.Graphics.OpenGL.PixelType.Float:
                    return sizeof(float);
            }
            return -1;
        }

        /// <summary>
        /// KTX file header. See the KTX specification for descriptions
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct KTX_header
        {
            //public fixed byte identifier[16];
            public byte identifier0;
            public byte identifier1;
            public byte identifier2;
            public byte identifier3;
            public byte identifier4;
            public byte identifier5;
            public byte identifier6;
            public byte identifier7;
            public byte identifier8;
            public byte identifier9;
            public byte identifier10;
            public byte identifier11;

            public uint endianness;
            public uint glType;
            public uint glTypeSize;
            public uint glFormat;
            public uint glInternalFormat;
            public uint glBaseInternalFormat;
            public uint pixelWidth;
            public uint pixelHeight;
            public uint pixelDepth;
            public uint numberOfArrayElements;
            public uint numberOfFaces;
            public uint numberOfMipmapLevels;
            public uint bytesOfKeyValueData;

            private const int KTX_HEADER_SIZE = 64;

            public const int KTX_ENDIAN_REF = 0x04030201;
            public const int KTX_ENDIAN_REF_REV = 0x01020304;


            /// <summary>
            /// Read and check a KTX header
            /// </summary>
            /// <param name="header"></param>
            /// <param name="stream"></param>
            /// <returns></returns>
            public static KTX_header ReadHeader(Stream stream)
            {

                byte[] buff = new byte[KTX_HEADER_SIZE];
                stream.Read(buff, 0, KTX_HEADER_SIZE);
                KTX_header header = ReadUsingPointer(buff);
                return header;
            }

            public static void WriteHeader(Stream stream, KTX_header header)
            {
                byte[] buff = new byte[KTX_HEADER_SIZE];
                fixed (byte* pbuff = buff)
                {
                    *(KTX_header*)pbuff = *((KTX_header*)&header);
                    stream.Write(buff, 0, KTX_HEADER_SIZE);
                }
            }

            /// <summary>
            /// This method reads unmanaged data into a header structure 
            /// This uses a technique described in 
            /// http://www.codeproject.com/Articles/25896/Reading-Unmanaged-Data-Into-Structures
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            private static unsafe KTX_header ReadUsingPointer(byte[] data)
            {
                fixed (byte* header = &data[0])
                {
                    return *(KTX_header*)header;
                }
            }

            /// <summary>
            /// Check a KTX file header.
            /// As well as checking that the header identifies a KTX file, the function
            /// sanity checks the values and returns information about the texture in a
            /// KTX_texinfo structure.
            /// </summary>
            /// <param name="header">KTX header to check</param>
            /// <param name="texinfo">KTX_texinfo structure in which to return information about the texture.</param>
            /// <returns></returns>
            public static KTX_Error_Code CheckHeader(KTX_header header, ref KTX_texinfo texinfo)
            {
                //TODO Check if 1D texture is supported
                bool SupportedCompressedTexImage1D = true;
                bool SupportedTexImage1D = true;

                // TODO Check if texture is supported
                bool SupportedCompressedTexImage3D = true;
                bool SupportedTexImage3D = true;


                uint max_dim;

                /* Compare identifier, is this a KTX file? */
                //private readonly byte[] KTX_IDENTIFIER_REF = new byte[12] { 0xAB, 0x4B, 0x54, 0x58, 0x20, 0x31, 0x31, 0xBB, 0x0D, 0x0A, 0x1A, 0x0A };

                if (header.identifier0 != 0xAB || header.identifier1 != 0x4B || header.identifier2 != 0x54 || header.identifier3 != 0x58 ||
                    header.identifier4 != 0x20 || header.identifier5 != 0x31 || header.identifier6 != 0x31 || header.identifier7 != 0xBB ||
                    header.identifier8 != 0x0D || header.identifier9 != 0x0A || header.identifier10 != 0x1A || header.identifier11 != 0x0A)
                {
                    return KTX_Error_Code.KTX_UNKNOWN_FILE_FORMAT;
                }
                if (header.endianness == KTX_ENDIAN_REF_REV)
                {
                    /* Convert endianness of header fields if necessary */
                    header.SwapEndian();

                    if (header.glTypeSize != 1 ||
                        header.glTypeSize != 2 ||
                        header.glTypeSize != 4)
                    {
                        /* Only 8, 16, and 32-bit types supported so far */
                        return KTX_Error_Code.KTX_INVALID_VALUE;
                    }
                }
                else if (header.endianness != KTX_ENDIAN_REF)
                {
                    return KTX_Error_Code.KTX_INVALID_VALUE;
                }
                /* Check glType and glFormat */
                texinfo.compressed = false;
                if (header.glType == 0 || header.glFormat == 0)
                {
                    if (header.glType + header.glFormat != 0)
                    {
                        /* either both or none of glType, glFormat must be zero */
                        return KTX_Error_Code.KTX_INVALID_VALUE;
                    }
                    texinfo.compressed = true;
                }

                /* Check texture dimensions. KTX files can store 8 types of textures:
                   1D, 2D, 3D, cube, and array variants of these. There is currently
                   no GL extension that would accept 3D array or cube array textures. */
                if ((header.pixelWidth == 0) ||
                    (header.pixelDepth > 0 && header.pixelHeight == 0))
                {
                    /* texture must have width */
                    /* texture must have height if it has depth */
                    return KTX_Error_Code.KTX_INVALID_VALUE;
                }

                texinfo.textureDimensions = 1;
                texinfo.glTarget = TextureTarget.Texture1D;
                texinfo.generateMipmaps = false;
                if (header.pixelHeight > 0)
                {
                    texinfo.textureDimensions = 2;
                    texinfo.glTarget = TextureTarget.Texture2D;
                }
                if (header.pixelDepth > 0)
                {
                    texinfo.textureDimensions = 3;
                    texinfo.glTarget = TextureTarget.Texture3D;
                }

                if (header.numberOfFaces == 6)
                {
                    if (texinfo.textureDimensions == 2)
                    {
                        texinfo.glTarget = TextureTarget.TextureCubeMap;
                    }
                    else
                    {
                        /* cube map needs 2D faces */
                        return KTX_Error_Code.KTX_INVALID_VALUE;
                    }
                }
                else if (header.numberOfFaces != 1)
                {
                    /* numberOfFaces must be either 1 or 6 */
                    return KTX_Error_Code.KTX_INVALID_VALUE;
                }

                /* load as 2D texture if 1D textures are not supported */
                if (texinfo.textureDimensions == 1 &&
                    ((texinfo.compressed && (!SupportedCompressedTexImage1D)) ||
                     (!texinfo.compressed && (!SupportedTexImage1D))))
                {
                    texinfo.textureDimensions = 2;
                    texinfo.glTarget = TextureTarget.Texture2D;
                    header.pixelHeight = 1;
                }

                if (header.numberOfArrayElements > 0)
                {
                    if (texinfo.glTarget == TextureTarget.Texture1D)
                    {
                        texinfo.glTarget = TextureTarget.Texture1DArray;
                    }
                    else if (texinfo.glTarget == TextureTarget.Texture2D)
                    {
                        texinfo.glTarget = TextureTarget.Texture2DArray;
                    }
                    else
                    {
                        /* No API for 3D and cube arrays yet */
                        return KTX_Error_Code.KTX_UNSUPPORTED_TEXTURE_TYPE;
                    }
                    texinfo.textureDimensions++;
                }

                /* reject 3D texture if unsupported */
                if (texinfo.textureDimensions == 3 &&
                    ((texinfo.compressed && (!SupportedCompressedTexImage3D)) ||
                     (!texinfo.compressed && (!SupportedTexImage3D))))
                {
                    return KTX_Error_Code.KTX_UNSUPPORTED_TEXTURE_TYPE;
                }

                /* Check number of mipmap levels */
                if (header.numberOfMipmapLevels == 0)
                {
                    texinfo.generateMipmaps = true;
                    header.numberOfMipmapLevels = 1;
                }

                max_dim = System.Math.Max(System.Math.Max(header.pixelWidth, header.pixelHeight), header.pixelDepth);
                if (max_dim < (uint)(1 << (int)(header.numberOfMipmapLevels - 1)))
                {
                    /* Can't have more mip levels than 1 + log2(max(width, height, depth)) */
                    return KTX_Error_Code.KTX_INVALID_VALUE;
                }

                return KTX_Error_Code.KTX_SUCCESS;
            }

            private void SwapEndian()
            {
                uint x = this.glType;
                this.glType = (x << 24) | ((x & 0xFF00) << 8) | ((x & 0xFF0000) >> 8) | (x >> 24);
                x = this.glTypeSize;
                this.glTypeSize = (x << 24) | ((x & 0xFF00) << 8) | ((x & 0xFF0000) >> 8) | (x >> 24);
                x = this.glFormat;
                this.glFormat = (x << 24) | ((x & 0xFF00) << 8) | ((x & 0xFF0000) >> 8) | (x >> 24);
                x = this.glInternalFormat;
                this.glInternalFormat = (x << 24) | ((x & 0xFF00) << 8) | ((x & 0xFF0000) >> 8) | (x >> 24);
                x = this.glBaseInternalFormat;
                this.glBaseInternalFormat = (x << 24) | ((x & 0xFF00) << 8) | ((x & 0xFF0000) >> 8) | (x >> 24);
                x = this.pixelWidth;
                this.pixelWidth = (x << 24) | ((x & 0xFF00) << 8) | ((x & 0xFF0000) >> 8) | (x >> 24);
                x = this.pixelHeight;
                this.pixelHeight = (x << 24) | ((x & 0xFF00) << 8) | ((x & 0xFF0000) >> 8) | (x >> 24);
                x = this.pixelDepth;
                this.pixelDepth = (x << 24) | ((x & 0xFF00) << 8) | ((x & 0xFF0000) >> 8) | (x >> 24);
                x = this.numberOfArrayElements;
                this.numberOfArrayElements = (x << 24) | ((x & 0xFF00) << 8) | ((x & 0xFF0000) >> 8) | (x >> 24);
                x = this.numberOfFaces;
                this.numberOfFaces = (x << 24) | ((x & 0xFF00) << 8) | ((x & 0xFF0000) >> 8) | (x >> 24);
                x = this.numberOfMipmapLevels;
                this.numberOfMipmapLevels = (x << 24) | ((x & 0xFF00) << 8) | ((x & 0xFF0000) >> 8) | (x >> 24);
                x = this.bytesOfKeyValueData;
                this.bytesOfKeyValueData = (x << 24) | ((x & 0xFF00) << 8) | ((x & 0xFF0000) >> 8) | (x >> 24);
            }

        }

    }


    /// <summary>
    /// CheckHeader returns texture information in this structure
    /// </summary>
    internal struct KTX_texinfo
    {
        /* Data filled in by CheckHeader() */
        public uint textureDimensions;
        public TextureTarget glTarget;
        public bool compressed;
        public bool generateMipmaps;
    }
}

