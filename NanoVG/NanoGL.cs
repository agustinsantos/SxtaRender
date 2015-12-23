using Sxta.Math;
using Sxta.Render;
using System;
using NVGcolor = Sxta.Math.Vector4f;

namespace NanoVG
{
    // Create flags
    [Flags]
    public enum NVGcreateFlags
    {
        NVG_NONE = 0,
        // Flag indicating if geometry based anti-aliasing is used (may not be needed when using MSAA).
        NVG_ANTIALIAS = 1 << 0,
        // Flag indicating if strokes should be drawn using stencil buffer. The rendering will be a little
        // slower, but path overlaps (i.e. self-intersecting or sharp turns) will be drawn just once.
        NVG_STENCIL_STROKES = 1 << 1,
        // Flag indicating that additional debug checks are done.
        NVG_DEBUG = 1 << 2,
    }

    public enum GLNVGshaderType
    {
        NSVG_SHADER_FILLGRAD,
        NSVG_SHADER_FILLIMG,
        NSVG_SHADER_SIMPLE,
        NSVG_SHADER_IMG
    }


    public struct GLNVGtexture
    {
        public int id;
        public uint tex;
        public int width, height;
        public NVGtexture type;
        public NVGimageFlags flags;
    }

    public enum GLNVGcallType
    {
        GLNVG_NONE = 0,
        GLNVG_FILL,
        GLNVG_CONVEXFILL,
        GLNVG_STROKE,
        GLNVG_TRIANGLES,
    }

    public struct GLNVGcall
    {
        public GLNVGcallType type;
        public int image;
        public int pathOffset;
        public int pathCount;
        public int triangleOffset;
        public int triangleCount;
        public int uniformOffset;
    }

    public struct GLNVGpath
    {
        public int fillOffset;
        public int fillCount;
        public int strokeOffset;
        public int strokeCount;
    }

    public class GLNVGcontext : IDisposable
    {
        internal readonly FrameBuffer fb;
        internal GLVGshader Shader;
        GLNVGtexture[] textures;
        Vector2f view;
        int ntextures;
        int ctextures;
        int textureId;
        uint vertBuf;
        uint vertArr;
        uint fragBuf;
        int fragSize;
        public NVGcreateFlags Flags { get; set; }

        // Per frame buffers
        GLNVGcall[] calls;
        int ccalls;
        int ncalls;
        GLNVGpath[] paths;
        int cpaths;
        int npaths;
        NVGvertex[] verts;
        int cverts;
        int nverts;
        GLNVGfragUniforms[] uniforms;
        int cuniforms;
        int nuniforms;
        Mesh<NVGvertex, uint> data; //TODO


        #region public methods
        public GLNVGcontext(FrameBuffer fb)
        {
            this.fb = fb;
            this.fragSize = 1;
        }

        public void CreateShaderProgram()
        {
            Shader = new GLVGshader();
            Program p = new Program(new Module(NvgOpenGL.ShaderVersion, NvgOpenGL.NvgOpenGLShader));

            Shader.ViewSizeUniform = p.getUniform2f("uViewSize");
            Shader.TexUniform = p.getUniformSampler("tex");
            Shader.FragUniform = p.getUniformBlock("frag");
            Shader.Prog = p;
        }

        public void RenderTriangles(NVGpaint paint, NVGscissor scissor, NVGvertex[] verts, int nverts)
        {
            GLNVGcall call = AllocCall();
            call.type = GLNVGcallType.GLNVG_TRIANGLES;
            call.image = paint.image;

            // Allocate vertices for all the paths.
            call.triangleOffset = AllocVerts(nverts);
            call.triangleCount = nverts;
            verts.CopyTo(this.verts, call.triangleOffset);


            // Fill shader
            call.uniformOffset = AllocFragUniforms(1);
            GLNVGfragUniforms frag = this.uniforms[call.uniformOffset];
            ConvertPaint(ref frag, paint, scissor, 1.0f, 1.0f, -1.0f);
            frag.type = GLNVGshaderType.NSVG_SHADER_IMG;
        }

        public void RenderCancel()
        {
            this.nverts = 0;
            this.npaths = 0;
            this.ncalls = 0;
            this.nuniforms = 0;
        }

        public void RenderStroke(NVGpaint paint, NVGscissor scissor, float fringe, float strokeWidth, NVGpath[] paths)
        {
            GLNVGcall call = new GLNVGcall();
            int i, maxverts, offset;
            int npaths = paths.Length;
            call.type = GLNVGcallType.GLNVG_STROKE;
            call.pathOffset = this.paths.Length;
            call.pathCount = npaths;
            call.image = paint.image;

            // Allocate vertices for all the paths.
            maxverts = MaxVertCount(paths, npaths);
            offset = AllocVerts(maxverts);

            for (i = 0; i < npaths; i++)
            {
                GLNVGpath copy = new GLNVGpath();
                NVGpath path = paths[i];
                if (path.nstroke != 0)
                {
                    copy.strokeOffset = offset;
                    copy.strokeCount = path.nstroke;
                    Array.Copy(this.verts, offset, path.stroke, 0, path.nstroke);
                    offset += path.nstroke;
                }
                this.paths[call.pathOffset + i] = copy;
            }

            if (this.Flags.HasFlag(NVGcreateFlags.NVG_STENCIL_STROKES))
            {
                // Fill shader
                call.uniformOffset = AllocFragUniforms(2);

                ConvertPaint(ref this.uniforms[call.uniformOffset], paint, scissor, strokeWidth, fringe, -1.0f);
                ConvertPaint(ref this.uniforms[call.uniformOffset + fragSize], paint, scissor, strokeWidth, fringe, 1.0f - 0.5f / 255.0f);
            }
            else
            {
                // Fill shader
                call.uniformOffset = AllocFragUniforms(1);
                ConvertPaint(ref this.uniforms[call.uniformOffset], paint, scissor, strokeWidth, fringe, -1.0f);
            }
            this.calls[this.ncalls - 1] = call;
        }

        public void RenderFill(NVGpaint paint, NVGscissor scissor, float fringe, Bound bounds, NVGpath[] paths, int npaths)
        {
            GLNVGcall call = AllocCall();

            int i, maxverts, offset;

            call.type = GLNVGcallType.GLNVG_FILL;
            call.pathOffset = AllocPaths(npaths);
            call.pathCount = npaths;
            call.image = paint.image;

            if (npaths == 1 && paths[0].convex)
                call.type = GLNVGcallType.GLNVG_CONVEXFILL;

            // Allocate vertices for all the paths.
            maxverts = MaxVertCount(paths, npaths) + 6;
            offset = AllocVerts(maxverts);

            for (i = 0; i < npaths; i++)
            {
                GLNVGpath copy = this.paths[call.pathOffset + i];
                NVGpath path = paths[i];
                // memset(copy, 0, sizeof(GLNVGpath));
                if (path.nfill > 0)
                {
                    copy.fillOffset = offset;
                    copy.fillCount = path.nfill;
                    Array.Copy(path.fill, 0, this.verts, offset, path.nfill);
                    offset += path.nfill;
                }
                if (path.nstroke > 0)
                {
                    copy.strokeOffset = offset;
                    copy.strokeCount = path.nstroke;
                    Array.Copy(path.stroke, 0, this.verts, offset, path.nstroke);
                    offset += path.nstroke;
                }
                this.paths[call.pathOffset + i] = copy;
            }

            // Quad
            call.triangleOffset = offset;
            call.triangleCount = 6;
            this.verts[call.triangleOffset].Set(bounds.xmin, bounds.ymax, 0.5f, 1.0f);
            this.verts[call.triangleOffset + 1].Set(bounds.xmax, bounds.ymax, 0.5f, 1.0f);
            this.verts[call.triangleOffset + 2].Set(bounds.xmax, bounds.ymin, 0.5f, 1.0f);

            this.verts[call.triangleOffset + 3].Set(bounds.xmin, bounds.ymax, 0.5f, 1.0f);
            this.verts[call.triangleOffset + 4].Set(bounds.xmax, bounds.ymin, 0.5f, 1.0f);
            this.verts[call.triangleOffset + 5].Set(bounds.xmin, bounds.ymin, 0.5f, 1.0f);

            // Setup uniforms for draw calls
            if (call.type == GLNVGcallType.GLNVG_FILL)
            {
                call.uniformOffset = AllocFragUniforms(2);
                // Simple shader for stencil
                this.uniforms[call.uniformOffset].strokeThr = -1.0f;
                this.uniforms[call.uniformOffset].type = GLNVGshaderType.NSVG_SHADER_SIMPLE;
                // Fill shader
                ConvertPaint(ref this.uniforms[call.uniformOffset + this.fragSize], paint, scissor, fringe, fringe, -1.0f);
            }
            else
            {
                call.uniformOffset = AllocFragUniforms(1);
                // Fill shader
                ConvertPaint(ref this.uniforms[call.uniformOffset], paint, scissor, fringe, fringe, -1.0f);
            }
            this.calls[this.ncalls - 1] = call;
        }


        public void RenderFlush()
        {
            int i;

            if (this.ncalls > 0)
            {
                //glUseProgram(this.Shader.Prog);

                // Setup require GL state.
                //glBlendFunc(GL_ONE, GL_ONE_MINUS_SRC_ALPHA);
                fb.setBlend(true, BlendEquation.ADD, BlendArgument.ONE, BlendArgument.ONE_MINUS_CONSTANT_ALPHA);

                //glEnable(GL_CULL_FACE);
                //glCullFace(GL_BACK);
                fb.setPolygonMode(PolygonMode.FILL, PolygonMode.CULL);

                //glFrontFace(GL_CCW);
                fb.setFrontFaceCW(false);

                //glEnable(GL_BLEND);
                fb.setBlend(true);

                //glDisable(GL_DEPTH_TEST);
                fb.setDepthTest(false);

                //glDisable(GL_SCISSOR_TEST);
                fb.setScissorTest(false);

                //glColorMask(GL_TRUE, GL_TRUE, GL_TRUE, GL_TRUE);
                fb.setColorMask(true, true, true, true);

                //glStencilMask(0xffffffff);
                //glStencilOp(GL_KEEP, GL_KEEP, GL_KEEP);
                //glStencilFunc(GL_ALWAYS, 0, 0xffffffff);
                fb.setStencilMask(0xffffffff);
                fb.setStencilTest(true, Function.ALWAYS, 0, 0xffffffff, StencilOperation.KEEP, StencilOperation.KEEP, StencilOperation.KEEP);

#if TODO
                glActiveTexture(GL_TEXTURE0);
                glBindTexture(GL_TEXTURE_2D, 0);

                this.boundTexture = 0;
                this.stencilMask = 0xffffffff;
                this.stencilFunc = GL_ALWAYS;
                this.stencilFuncRef = 0;
                this.stencilFuncMask = 0xffffffff;
#endif
                if (data == null)
                {
                    data = new Mesh<NVGvertex, uint>(NVGvertex.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STREAM);
                    data.addAttributeType(0, 2, AttributeType.A32F, false);
                    data.addAttributeType(1, 2, AttributeType.A32F, false);
                }
                else
                {
                    data.clear();
                }
                data.addVertices(this.verts, this.nverts);
                // Set view and texture just once per frame.
                this.Shader.TexUniform.set(null);
                this.Shader.ViewSizeUniform.set(this.view);
#if TODO
                // Upload ubo for frag shaders
                glBindBuffer(GL_UNIFORM_BUFFER, gl.fragBuf);
                glBufferData(GL_UNIFORM_BUFFER, gl.nuniforms * gl.fragSize, gl.uniforms, GL_STREAM_DRAW);

                // Upload vertex data
                glBindVertexArray(gl.vertArr);
                glBindBuffer(GL_ARRAY_BUFFER, gl.vertBuf);
                glBufferData(GL_ARRAY_BUFFER, gl.nverts * sizeof(NVGvertex), gl.verts, GL_STREAM_DRAW);
                glEnableVertexAttribArray(0);
                glEnableVertexAttribArray(1);
                glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, sizeof(NVGvertex), (const GLvoid*)(size_t)0);
                glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, sizeof(NVGvertex), (const GLvoid*)(0 + 2 * sizeof(float)));

                // Set view and texture just once per frame.
                glUniform1i(gl.shader.loc[GLNVG_LOC_TEX], 0);
                glUniform2fv(gl.shader.loc[GLNVG_LOC_VIEWSIZE], 1, gl.view);

                glBindBuffer(GL_UNIFORM_BUFFER, gl.fragBuf);
                
                for (i = 0; i < this.ncalls; i++)
                {
                    GLNVGcall call = this.calls[i];
                    if (call.type == GLNVG_FILL)
                        DrawFill( call);
                    else if (call.type == GLNVG_CONVEXFILL)
                        DrawConvexFill( call);
                    else if (call.type == GLNVG_STROKE)
                        DrawStroke( call);
                    else if (call.type == GLNVG_TRIANGLES)
                        DrawTriangles( call);
                }
#endif

                for (i = 0; i < this.ncalls; i++)
                {
                    GLNVGcall call = this.calls[i];
                    switch (call.type)
                    {
                        case GLNVGcallType.GLNVG_FILL:
                            DrawFill(call);
                            break;
                        case GLNVGcallType.GLNVG_CONVEXFILL:
                            DrawConvexFill(call);
                            break;
                        case GLNVGcallType.GLNVG_STROKE:
                            DrawStroke(call);
                            break;
                        case GLNVGcallType.GLNVG_TRIANGLES:
                            DrawTriangles(call);
                            break;
                    }
                }
#if TODO

                glDisableVertexAttribArray(0);
                glDisableVertexAttribArray(1);
                glBindVertexArray(0);
                glDisable(GL_CULL_FACE);
                glBindBuffer(GL_ARRAY_BUFFER, 0);
                glUseProgram(0);
                glnvg__bindTexture(gl, 0);
#endif
            }

            // Reset calls
            this.nverts = 0;
            this.npaths = 0;
            this.ncalls = 0;
            this.nuniforms = 0;
        }

        public void RenderGetTextureSize(int image, out int w, out int h)
        {
            int t = FindTexture(image);
            if (t == -1)
            {
                w = 0;
                h = 0;
                return;
            }
            GLNVGtexture tex = this.textures[t];
            w = tex.width;
            h = tex.height;
            return;
        }
        public void RenderDeleteTexture(int image)
        {
            DeleteTexture(image);
        }
        public Vector2f ViewPort
        {
            set { view = value; }
        }

        #endregion

        private void DrawFill(GLNVGcall call)
        {
            /*
            	GLNVGpath* paths = &gl->paths[call->pathOffset];
	            int i, npaths = call->pathCount;

	            // Draw shapes
	            glEnable(GL_STENCIL_TEST);
	            glnvg__stencilMask(gl, 0xff);
	            glnvg__stencilFunc(gl, GL_ALWAYS, 0, 0xff);
	            glColorMask(GL_FALSE, GL_FALSE, GL_FALSE, GL_FALSE);

	            // set bindpoint for solid loc
	            glnvg__setUniforms(gl, call->uniformOffset, 0);
	            glnvg__checkError(gl, "fill simple");

	            glStencilOpSeparate(GL_FRONT, GL_KEEP, GL_KEEP, GL_INCR_WRAP);
	            glStencilOpSeparate(GL_BACK, GL_KEEP, GL_KEEP, GL_DECR_WRAP);
	            glDisable(GL_CULL_FACE);
	            for (i = 0; i < npaths; i++)
		            glDrawArrays(GL_TRIANGLE_FAN, paths[i].fillOffset, paths[i].fillCount);
	            glEnable(GL_CULL_FACE);

	            // Draw anti-aliased pixels
	            glColorMask(GL_TRUE, GL_TRUE, GL_TRUE, GL_TRUE);

	            glnvg__setUniforms(gl, call->uniformOffset + gl->fragSize, call->image);
	            glnvg__checkError(gl, "fill fill");

	            if (gl->flags & NVG_ANTIALIAS) {
		            glnvg__stencilFunc(gl, GL_EQUAL, 0x00, 0xff);
		            glStencilOp(GL_KEEP, GL_KEEP, GL_KEEP);
		            // Draw fringes
		            for (i = 0; i < npaths; i++)
			            glDrawArrays(GL_TRIANGLE_STRIP, paths[i].strokeOffset, paths[i].strokeCount);
	            }

	            // Draw fill
	            glnvg__stencilFunc(gl, GL_NOTEQUAL, 0x0, 0xff);
	            glStencilOp(GL_ZERO, GL_ZERO, GL_ZERO);
	            glDrawArrays(GL_TRIANGLES, call->triangleOffset, call->triangleCount);

	            glDisable(GL_STENCIL_TEST);
            */

            GLNVGpath paths = this.paths[call.pathOffset];
            int i, npaths = call.pathCount;

            // Draw shapes
            //glEnable(GL_STENCIL_TEST);
            //glnvg__stencilMask(gl, 0xff);
            //glnvg__stencilFunc(gl, GL_ALWAYS, 0, 0xff);
            //glStencilOpSeparate(GL_FRONT, GL_KEEP, GL_KEEP, GL_INCR_WRAP);
            //glStencilOpSeparate(GL_BACK, GL_KEEP, GL_KEEP, GL_DECR_WRAP);
            this.fb.setStencilMask(0xff);
            this.fb.setStencilTest(true, Function.ALWAYS, 0, 0xff, StencilOperation.KEEP, StencilOperation.KEEP, StencilOperation.INCR_WRAP,
                                         Function.ALWAYS, 0, 0xff, StencilOperation.KEEP, StencilOperation.KEEP, StencilOperation.DECR_WRAP);
            //glColorMask(GL_FALSE, GL_FALSE, GL_FALSE, GL_FALSE);
            this.fb.setColorMask(false, false, false, false);
            SetUniforms(call.uniformOffset, 0);
            fb.setPolygonMode(PolygonMode.FILL, PolygonMode.FILL);
            for (i = 0; i < npaths; i++)
                this.fb.draw(this.Shader.Prog, data.getBuffers(), MeshMode.TRIANGLE_FAN, this.paths[i].fillOffset, this.paths[i].fillCount);
            fb.setPolygonMode(PolygonMode.FILL, PolygonMode.CULL);

            // Draw anti-aliased pixels
            this.fb.setColorMask(true, true, true, true);

            SetUniforms(call.uniformOffset + this.fragSize, call.image);
            if (this.Flags.HasFlag(NVGcreateFlags.NVG_ANTIALIAS))
            {
                //glnvg__stencilFunc(gl, GL_EQUAL, 0x00, 0xff);
                //glStencilOp(GL_KEEP, GL_KEEP, GL_KEEP);
                fb.setStencilTest(true, Function.EQUAL, 0x00, 0xff, StencilOperation.KEEP, StencilOperation.KEEP, StencilOperation.KEEP);
                // Draw fringes
                for (i = 0; i < npaths; i++)
                    this.fb.draw(this.Shader.Prog, data.getBuffers(), MeshMode.TRIANGLE_STRIP, this.paths[i].strokeOffset, this.paths[i].strokeCount);
            }

            // Draw fill
            //glnvg__stencilFunc(gl, GL_NOTEQUAL, 0x0, 0xff);
            //glStencilOp(GL_ZERO, GL_ZERO, GL_ZERO);
            fb.setStencilTest(true, Function.NOTEQUAL, 0x00, 0xff, StencilOperation.RESET, StencilOperation.RESET, StencilOperation.RESET);
            this.fb.draw(this.Shader.Prog, data.getBuffers(), MeshMode.TRIANGLES, call.triangleOffset, call.triangleCount);

            fb.setStencilTest(false);
        }
        private void DrawConvexFill(GLNVGcall call)
        {
            throw new NotImplementedException();
        }
        private void DrawStroke(GLNVGcall call)
        {
            throw new NotImplementedException();
        }
        private void DrawTriangles(GLNVGcall call)
        {
            SetUniforms(call.uniformOffset, call.image);

            this.fb.draw(this.Shader.Prog, data.getBuffers(), MeshMode.TRIANGLES, call.triangleOffset, call.triangleCount);
        }

        private void SetUniforms(int uniformOffset, int image)
        {
            this.Shader.FragUniform.set(this.uniforms[uniformOffset], BufferUsage.STREAM_DRAW);
            /*
                        glBindBufferRange(GL_UNIFORM_BUFFER, GLNVG_FRAG_BINDING, gl->fragBuf, uniformOffset, sizeof(GLNVGfragUniforms));

                        if (image != 0)
                        {
                            GLNVGtexture* tex = glnvg__findTexture(gl, image);
                            glnvg__bindTexture(gl, tex != NULL ? tex->tex : 0);
                            glnvg__checkError(gl, "tex paint tex");
                        }
                        else
                        {
                            glnvg__bindTexture(gl, 0);
                        }
            */
        }

        private NVGcolor PremulColor(NVGcolor c)
        {
            c.X *= c.W;
            c.Y *= c.W;
            c.Z *= c.W;
            return c;
        }

        private void XformToMat3(ref Matrix3x4f m3, float[] t)
        {
            m3[0] = t[0];
            m3[1] = t[1];
            m3[2] = 0.0f;
            m3[3] = 0.0f;
            m3[4] = t[2];
            m3[5] = t[3];
            m3[6] = 0.0f;
            m3[7] = 0.0f;
            m3[8] = t[4];
            m3[9] = t[5];
            m3[10] = 1.0f;
            m3[11] = 0.0f;
        }

        private void ConvertPaint(ref GLNVGfragUniforms frag, NVGpaint paint, NVGscissor scissor, float width, float fringe, float strokeThr)
        {
            GLNVGtexture tex;
            float[] invxform = new float[6];
            frag.innerCol = PremulColor(paint.innerColor);
            frag.outerCol = PremulColor(paint.outerColor);

            if (scissor.extent[0] < -0.5f || scissor.extent[1] < -0.5f)
            {
                frag.scissorMat = new Matrix3x4f();
                frag.scissorExt.X = 1.0f;
                frag.scissorExt.Y = 1.0f;
                frag.scissorScale.X = 1.0f;
                frag.scissorScale.Y = 1.0f;
            }
            else
            {
                Nvg.TransformInverse(invxform, scissor.xform);
                XformToMat3(ref frag.scissorMat, invxform);
                frag.scissorExt.X = scissor.extent[0];
                frag.scissorExt.Y = scissor.extent[1];
                frag.scissorScale.X = (float)Math.Sqrt(scissor.xform[0] * scissor.xform[0] + scissor.xform[2] * scissor.xform[2]) / fringe;
                frag.scissorScale.Y = (float)Math.Sqrt(scissor.xform[1] * scissor.xform[1] + scissor.xform[3] * scissor.xform[3]) / fringe;
            }
            frag.extent = new Vector2f(paint.extent[0], paint.extent[1]);
            frag.strokeMult = (width * 0.5f + fringe * 0.5f) / fringe;
            frag.strokeThr = strokeThr;

            if (paint.image != 0)
            {
                int pos = FindTexture(paint.image);
                if (pos == -1) return;
                tex = this.textures[pos];
                if (tex.flags.HasFlag(NVGimageFlags.NVG_IMAGE_FLIPY))
                {
                    float[] flipped = new float[6];
                    Nvg.TransformScale(flipped, 1.0f, -1.0f);
                    Nvg.TransformMultiply(flipped, paint.xform);
                    Nvg.TransformInverse(invxform, flipped);
                }
                else
                {
                    Nvg.TransformInverse(invxform, paint.xform);
                }
                frag.type = GLNVGshaderType.NSVG_SHADER_FILLIMG;

                if (tex.type == NVGtexture.NVG_TEXTURE_RGBA)
                    frag.texType = tex.flags.HasFlag(NVGimageFlags.NVG_IMAGE_PREMULTIPLIED) ? 0 : 1;
                else
                    frag.texType = 2;
            }
            else
            {
                frag.type = GLNVGshaderType.NSVG_SHADER_FILLGRAD;
                frag.radius = paint.radius;
                frag.feather = paint.feather;
                Nvg.TransformInverse(invxform, paint.xform);
            }

            XformToMat3(ref frag.paintMat, invxform);
        }
        private GLNVGcall AllocCall()
        {
            if (this.ncalls + 1 > this.ccalls)
            {
                int ccalls = Math.Max(this.ncalls + 1, 128) + this.ccalls / 2; // 1.5x Overallocate
                this.calls = NVGcontext.ResizeArray(this.calls, ccalls);
                this.ccalls = ccalls;
            }
            GLNVGcall ret = new GLNVGcall();
            this.calls[this.ncalls++] = ret;
            return ret;
        }
        private int AllocPaths(int n)
        {
            int ret = 0;
            if (this.npaths + n > this.cpaths)
            {
                int cpaths = Math.Max(this.npaths + n, 128) + this.cpaths / 2; // 1.5x Overallocate
                this.paths = NVGcontext.ResizeArray(this.paths, cpaths);
                this.cpaths = cpaths;
            }
            ret = this.npaths;
            this.npaths += n;
            return ret;
        }

        private int AllocVerts(int n)
        {
            int ret = 0;
            if (this.nverts + n > this.cverts)
            {
                int cverts = Math.Max(this.nverts + n, 4096) + this.cverts / 2; // 1.5x Overallocate
                this.verts = NVGcontext.ResizeArray(this.verts, cverts);
                this.cverts = cverts;
            }
            ret = this.nverts;
            this.nverts += n;
            return ret;
        }
        private int AllocFragUniforms(int n)
        {
            int ret = 0, structSize = this.fragSize;
            if (this.nuniforms + n > this.cuniforms)
            {
                int cuniforms = Math.Max(this.nuniforms + n, 128) + this.cuniforms / 2; // 1.5x Overallocate
                this.uniforms = NVGcontext.ResizeArray(this.uniforms, cuniforms); ;
                this.cuniforms = cuniforms;
            }
            ret = this.nuniforms * structSize;
            this.nuniforms += n;
            return ret;
        }
        private int MaxVertCount(NVGpath[] paths, int npaths)
        {
            int i, count = 0;
            for (i = 0; i < npaths; i++)
            {
                count += paths[i].nfill;
                count += paths[i].nstroke;
            }
            return count;
        }
        private int FindTexture(int id)
        {
            int i;
            for (i = 0; i < this.ntextures; i++)
                if (this.textures[i].id == id)
                    return i;
            return -1;
        }
        private void DeleteTexture(int id)
        {
            int i;
            for (i = 0; i < this.ntextures; i++)
            {
                if (this.textures[i].id == id)
                {
                    throw new NotImplementedException();
#if TODO
                    if (this.textures[i].tex != 0 && (this.textures[i].flags & NVG_IMAGE_NODELETE) == 0)
                        glDeleteTextures(1, &this.textures[i].tex);
                    memset(&this.textures[i], 0, sizeof(this.textures[i]));
                    return;
#endif
                }
            }
            return;
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Shader != null)
                        Shader.Dispose();
                    if (data != null)
                        data.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~GLNVGcontext() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        // cached state
#if NANOVG_GL_USE_STATE_FILTER
	GLuint boundTexture;
	GLuint stencilMask;
	GLenum stencilFunc;
	GLint stencilFuncRef;
	GLuint stencilFuncMask;
#endif
    }



    public static class NvgOpenGL
    {
        internal const int ShaderVersion = 330;
        internal const string NvgOpenGLShader =
    @"#ifdef _VERTEX_
        layout (location = 0) in vec2 aPosition;
        layout (location = 1) in vec2 aTexCoord;
        out vec2 ftcoord;
        out vec2 fpos;
        uniform vec2 uViewSize;

        void main(void)
        {
            ftcoord = aTexCoord;
            fpos = aPosition;
            gl_Position = vec4(2.0 * aPosition.x / uViewSize.x - 1.0, 1.0 - 2.0 * aPosition.y / uViewSize.y, 0, 1);
        }
#endif
# ifdef _FRAGMENT_
            precision mediump float;
            in vec2 ftcoord; 
            in vec2 fpos; 
            out vec4 outColor; 
            uniform sampler2D tex; 
            layout(std140) uniform frag {
				mat3 scissorMat;
				mat3 paintMat;
				vec4 innerCol;
				vec4 outerCol;
				vec2 scissorExt;
				vec2 scissorScale;
				vec2 extent;
				float radius;
				float feather;
				float strokeMult;
				float strokeThr;
				int texType;
				int type;
			};
		float sdroundrect(vec2 pt, vec2 ext, float rad) {
			vec2 ext2 = ext - vec2(rad,rad);
			vec2 d = abs(pt) - ext2;
			return min(max(d.x,d.y),0.0) + length(max(d,0.0)) - rad;
		}
		
		// Scissoring
		float scissorMask(vec2 p) {
			vec2 sc = (abs((scissorMat * vec3(p,1.0)).xy) - scissorExt);
			sc = vec2(0.5,0.5) - sc * scissorScale;
			return clamp(sc.x,0.0,1.0) * clamp(sc.y,0.0,1.0);
		}
		#ifdef EDGE_AA
		// Stroke - from [0..1] to clipped pyramid, where the slope is 1px.
		float strokeMask() {
			return min(1.0, (1.0-abs(ftcoord.x*2.0-1.0))*strokeMult) * min(1.0, ftcoord.y);
		}
		#endif
		
		void main(void) {
		   vec4 result;
			float scissor = scissorMask(fpos);
		#ifdef EDGE_AA
			float strokeAlpha = strokeMask();
		#else
			float strokeAlpha = 1.0;
		#endif
			if (type == 0) {			// Gradient
				// Calculate gradient color using box gradient
				vec2 pt = (paintMat * vec3(fpos,1.0)).xy;
				float d = clamp((sdroundrect(pt, extent, radius) + feather*0.5) / feather, 0.0, 1.0);
				vec4 color = mix(innerCol,outerCol,d);
				// Combine alpha
				color *= strokeAlpha * scissor;
				result = color;
			} else if (type == 1) {		// Image
				// Calculate color fron texture
				vec2 pt = (paintMat * vec3(fpos,1.0)).xy / extent;
				vec4 color = texture(tex, pt);
				if (texType == 1) color = vec4(color.xyz*color.w,color.w);
				if (texType == 2) color = vec4(color.x);
				// Apply color tint and alpha.
				color *= innerCol;
				// Combine alpha
				color *= strokeAlpha * scissor;
				result = color;
			} else if (type == 2) {		// Stencil fill
				result = vec4(1,1,1,1);
			} else if (type == 3) {		// Textured tris
				vec4 color = texture(tex, ftcoord);
				if (texType == 1) color = vec4(color.xyz*color.w,color.w);
				if (texType == 2) color = vec4(color.x);
				color *= scissor;
				result = color * innerCol;
			}
		#ifdef EDGE_AA
			if (strokeAlpha < strokeThr) discard;
		#endif
			outColor = result;
		}
#endif";



        public static GLNVGcontext CreateContext(FrameBuffer fb)
        {
            GLNVGcontext context = new GLNVGcontext(fb);
            context.CreateShaderProgram();
            return context;
        }

    }
}
