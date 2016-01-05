using Sxta.Math;
using Sxta.Render;
using System;
using System.Collections.Generic;
using NVGcolor = Sxta.Math.Vector4f;

namespace NanoVG
{
    internal enum NVGcommands
    {
        NVG_MOVETO = 0,
        NVG_LINETO = 1,
        NVG_BEZIERTO = 2,
        NVG_CLOSE = 3,
        NVG_WINDING = 4,
    }

    [Flags]
    internal enum NVGpointFlags : byte
    {
        NVG_PT_CORNER = 0x01,
        NVG_PT_LEFT = 0x02,
        NVG_PT_BEVEL = 0x04,
        NVG_PR_INNERBEVEL = 0x08,
    }

    public class NVGstate
    {
        public NVGpaint fill;
        public NVGpaint stroke;
        public float strokeWidth;
        public float miterLimit;
        public NVGlineJoin lineJoin;
        public NVGlineCap lineCap;
        public float alpha;
        public float[] xform = new float[6] { 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f };
        public NVGscissor scissor = new NVGscissor();
        public float fontSize;
        public float letterSpacing;
        public float lineHeight;
        public float fontBlur;
        public NVGalign textAlign;
        public int fontId;

        public NVGstate Clone()
        {
            NVGstate newState = (NVGstate)this.MemberwiseClone();
            newState.xform = (float[])this.xform.Clone();
            newState.scissor = this.scissor.Clone();
            return newState;
        }
    }

    internal struct NVGpoint
    {
        public float x, y;
        public float dx, dy;
        public float len;
        public float dmx, dmy;
        public NVGpointFlags flags;

        public static float Normalize(ref float x, ref float y)
        {
            float d = (float)Math.Sqrt((x * x) + (y * y));
            if (d > 1e-6f)
            {
                float id = 1.0f / d;
                x *= id;
                y *= id;
            }
            return d;
        }

        public static bool Equals(float x1, float y1, float x2, float y2, float tol)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            return dx * dx + dy * dy < tol * tol;
        }

        public static float DistPtSeg(float x, float y, float px, float py, float qx, float qy)
        {
            float pqx, pqy, dx, dy, d, t;
            pqx = qx - px;
            pqy = qy - py;
            dx = x - px;
            dy = y - py;
            d = pqx * pqx + pqy * pqy;
            t = pqx * dx + pqy * dy;
            if (d > 0) t /= d;
            if (t < 0) t = 0;
            else if (t > 1) t = 1;
            dx = px + t * pqx - x;
            dy = py + t * pqy - y;
            return dx * dx + dy * dy;
        }
    }

    internal struct NVGpathCache
    {
        public NVGpoint[] points;
        public int npoints;
        public int cpoints;
        public NVGpath[] paths;
        public int npaths;
        public int cpaths;
        public NVGvertex[] verts;
        public int nverts;
        public int cverts;
        public Bound bounds;

        private const int NVG_INIT_POINTS_SIZE = 128;
        private const int NVG_INIT_PATHS_SIZE = 16;
        private const int NVG_INIT_VERTS_SIZE = 256;

        public static NVGpathCache AllocPathCache()
        {
            NVGpathCache c = new NVGpathCache();

            c.points = new NVGpoint[NVG_INIT_POINTS_SIZE];
            c.npoints = 0;
            c.cpoints = NVG_INIT_POINTS_SIZE;

            c.paths = new NVGpath[NVG_INIT_PATHS_SIZE];
            c.npaths = 0;
            c.cpaths = NVG_INIT_PATHS_SIZE;

            c.verts = new NVGvertex[NVG_INIT_VERTS_SIZE];
            c.nverts = 0;
            c.cverts = NVG_INIT_VERTS_SIZE;

            return c;
        }
    }


    public class NVGcontext : IDisposable
    {
        public const int NVG_MAX_STATES = 32;
        private const int NVG_INIT_COMMANDS_SIZE = 256;
        internal const int NVG_MAX_FONTIMAGES = 4;

        internal NVGparams params_;
        internal GLNVGcontext glctx;
        internal float[] commands = new float[NVG_INIT_COMMANDS_SIZE];
        internal int ccommands = 0;
        internal int ncommands = NVG_INIT_COMMANDS_SIZE;
        internal float commandx, commandy;
        internal NVGstate[] states = new NVGstate[NVG_MAX_STATES];
        internal int nstates;
        internal NVGpathCache cache = new NVGpathCache();
        internal float tessTol;
        internal float distTol;
        internal float fringeWidth;
        internal float devicePxRatio;
        internal FONScontext fs;
        internal int[] fontImages = new int[NVG_MAX_FONTIMAGES];
        internal int fontImageIdx;
        internal int drawCallCount;
        internal int fillTriCount;
        internal int strokeTriCount;
        internal int textTriCount;

        public NVGcontext(FrameBuffer fb, NVGcreateFlags flags, NVGparams params_)
        {
            glctx = NvgOpenGL.CreateContext(fb);
            glctx.Flags = flags;
            this.params_ = params_;
            this.cache = NVGpathCache.AllocPathCache();

            this.SaveState();
            this.Reset();
            /*
            FONSparams fontParams = new FONSparam();
            fontParams.width = NVG_INIT_FONTIMAGE_SIZE;
	        fontParams.height = NVG_INIT_FONTIMAGE_SIZE;
	        fontParams.flags = FONS_ZERO_TOPLEFT;
	        fontParams.renderCreate = NULL;
	        fontParams.renderUpdate = NULL;
	        fontParams.renderDraw = NULL;
	        fontParams.renderDelete = NULL;
	        fontParams.userPtr = NULL;
	        ctx.fs = fonsCreateInternal(&fontParams);
	        if (ctx.fs == NULL) goto error;

	        // Create font texture
	        ctx.fontImages[0] = ctx.params.renderCreateTexture(ctx.params.userPtr, NVG_TEXTURE_ALPHA, fontParams.width, fontParams.height, 0, NULL);
	        if (ctx.fontImages[0] == 0) goto error;
	        ctx.fontImageIdx = 0;
            */
        }

        public NVGstate GetState()
        {
            return this.states[this.nstates - 1];
        }

        public void SetState(NVGstate state)
        {
            this.states[this.nstates - 1] = state;
        }

        /// <summary>
        /// Pushes and saves the current render state into a state stack.
        /// A matching nvgRestore() must be used to restore the state.
        /// </summary>
        public void SaveState()
        {
            if (this.nstates >= NVG_MAX_STATES)
                return;
            if (this.nstates > 0)
            {
                NVGstate newstate = GetState().Clone();
                this.states[this.nstates] = newstate;
            }
            else
            {
                this.states[0] = new NVGstate();
            }
            this.nstates++;
        }

        /// <summary>
        /// Pops and restores current render state.
        /// </summary>
        public void Restore()
        {
            if (this.nstates <= 1)
                return;
            this.nstates--;
        }
        
        /// <summary>
        /// Resets current render state to default values. Does not affect the render state stack.
        /// </summary>
        /// <param name="ctx"></param>
        public void Reset()
        {
            NVGstate state = this.GetState();
            
            state.fill.SetPaintColor(new NVGcolor(255, 255, 255, 255));
            state.stroke.SetPaintColor(new NVGcolor(0, 0, 0, 255));
            state.strokeWidth = 1.0f;
            state.miterLimit = 10.0f;
            state.lineCap = NVGlineCap.NVG_BUTT;
            state.lineJoin = NVGlineJoin.NVG_MITER;
            state.alpha = 1.0f;
            state.xform = new float[6] { 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f }; //nvgTransformIdentity(state.xform);

            state.scissor = new NVGscissor();
            state.scissor.extent[0] = -1.0f;
            state.scissor.extent[1] = -1.0f;

            state.fontSize = 16.0f;
            state.letterSpacing = 0.0f;
            state.lineHeight = 1.0f;
            state.fontBlur = 0.0f;
            state.textAlign = NVGalign.NVG_ALIGN_LEFT | NVGalign.NVG_ALIGN_BASELINE;
            state.fontId = 0;

            this.SetState(state);
        }
        public void ClearPathCache()
        {
            this.cache.npoints = 0;
            this.cache.npaths = 0;
        }

        public void RenderViewport(int windowWidth, int windowHeight)
        {
            this.glctx.ViewPort = new Vector2f(windowWidth, windowHeight);
        }

        public void AddPath()
        {
            if (this.cache.npaths + 1 > this.cache.cpaths)
            {
                int cpaths = this.cache.npaths + 1 + this.cache.cpaths / 2;
                this.cache.paths = ResizeArray(this.cache.paths, cpaths);
                this.cache.cpaths = cpaths;
            }
            NVGpath path = new NVGpath();
            path.first = this.cache.npoints;
            path.winding = NVGwinding.NVG_CCW;
            this.cache.paths[this.cache.npaths] = path;

            this.cache.npaths++;
        }
        private NVGpoint LastPoint()
        {
            if (this.cache.npoints == 0)
                throw new Exception("No points");
            return this.cache.points[this.cache.npoints - 1];
        }
        private NVGpath LastPath()
        {
            if (this.cache.npaths == 0)
                throw new Exception("No paths");
            return this.cache.paths[this.cache.npaths - 1];
        }
        private void AddPoint(float x, float y, NVGpointFlags flags)
        {
            NVGpath path = LastPath();
            NVGpoint pt;

            if (path.count > 0 && this.cache.npoints > 0)
            {
                pt = LastPoint();
                if (NVGpoint.Equals(pt.x, pt.y, x, y, this.distTol))
                {
                    pt.flags |= flags;
                    this.cache.points[this.cache.npoints - 1] = pt;
                    return;
                }
            }

            if (this.cache.npoints + 1 > this.cache.cpoints)
            {
                int cpoints = this.cache.npoints + 1 + this.cache.cpoints / 2;
                this.cache.points = ResizeArray(this.cache.points, cpoints);
                this.cache.cpoints = cpoints;
            }

            pt = new NVGpoint();
            pt.x = x;
            pt.y = y;
            pt.flags = flags;
            this.cache.points[this.cache.npoints] = pt;
            this.cache.npoints++;
            path.count++;

            this.cache.paths[this.cache.npaths - 1] = path;
        }

        private void ClosePath()
        {
            this.cache.paths[this.cache.npaths - 1].closed = true;
        }

        private void PathWinding(NVGwinding winding)
        {
            this.cache.paths[this.cache.npaths - 1].winding = winding;
        }



        public void FlattenPaths()
        {
            NVGpoint last;
            int p0;
            int p1;
            NVGpath path;
            int i, j;
            float area;
            int p;

            if (cache.npaths > 0)
                return;

            // Flatten
            i = 0;
            while (i < this.ncommands)
            {
                int cmd = (int)this.commands[i];
                switch ((NVGcommands)cmd)
                {
                    case NVGcommands.NVG_MOVETO:
                        AddPath();
                        p = i + 1;
                        AddPoint(this.commands[p], this.commands[p + 1], NVGpointFlags.NVG_PT_CORNER);
                        i += 3;
                        break;
                    case NVGcommands.NVG_LINETO:
                        p = i + 1;
                        AddPoint(this.commands[p], this.commands[p + 1], NVGpointFlags.NVG_PT_CORNER);
                        i += 3;
                        break;
                    case NVGcommands.NVG_BEZIERTO:

                        if (this.cache.npoints > 0)
                        {
                            last = LastPoint();
                            int cp1 = i + 1;
                            int cp2 = i + 3;
                            p = i + 5;
                            TesselateBezier(last.x, last.y, this.commands[cp1], this.commands[cp1 + 1], this.commands[cp2], this.commands[cp2 + 1], this.commands[p], this.commands[p + 1], 0, NVGpointFlags.NVG_PT_CORNER);
                        }
                        i += 7;
                        break;
                    case NVGcommands.NVG_CLOSE:
                        ClosePath();
                        i++;
                        break;
                    case NVGcommands.NVG_WINDING:
                        PathWinding((NVGwinding)this.commands[i + 1]);
                        i += 2;
                        break;
                    default:
                        i++;
                        break;
                }
            }

            this.cache.bounds.xmin = this.cache.bounds.ymin = float.MaxValue;
            this.cache.bounds.xmax = this.cache.bounds.ymax = float.MinValue;

            // Calculate the direction and length of line segments.
            for (j = 0; j < this.cache.npaths; j++)
            {
                path = this.cache.paths[j];
                int pts = path.first;

                // If the first and last points are the same, remove the last, mark as closed path.
                p0 = pts + path.count - 1;
                p1 = pts; // this.cache.points[pts];
                if (NVGpoint.Equals(this.cache.points[p0].x, this.cache.points[p0].y, this.cache.points[p1].x, this.cache.points[p1].y, this.distTol))
                {
                    path.count--;
                    p0 = pts + path.count - 1;
                    path.closed = true;
                }

                // Enforce winding.
                if (path.count > 2)
                {
                    area = PolyArea(this.cache.points, pts, path.count);
                    if (path.winding == NVGwinding.NVG_CCW && area < 0.0f)
                        PolyReverse(this.cache.points, pts, path.count);
                    if (path.winding == NVGwinding.NVG_CW && area > 0.0f)
                        PolyReverse(this.cache.points, pts, path.count);
                }
                for (i = 0; i < path.count; i++)
                {
                    // Calculate segment direction and length
                    this.cache.points[p0].dx = this.cache.points[p1].x - this.cache.points[p0].x;
                    this.cache.points[p0].dy = this.cache.points[p1].y - this.cache.points[p0].y;
                    this.cache.points[p0].len = NVGpoint.Normalize(ref this.cache.points[p0].dx, ref this.cache.points[p0].dy);
                    // Update bounds
                    this.cache.bounds.xmin = Math.Min(this.cache.bounds.xmin, this.cache.points[p0].x);
                    this.cache.bounds.ymin = Math.Min(this.cache.bounds.ymin, this.cache.points[p0].y);
                    this.cache.bounds.xmax = Math.Max(this.cache.bounds.xmax, this.cache.points[p0].x);
                    this.cache.bounds.ymax = Math.Max(this.cache.bounds.ymax, this.cache.points[p0].y);
                    // Advance
                    p0 = pts + i;
                    p1 = pts + i + 1;
                }
            }
        }

        public void ExpandFill(float w, NVGlineJoin lineJoin, float miterLimit)
        {
            int verts;
            int dst;
            int cverts, i, j;
            float aa = this.fringeWidth;
            bool fringe = w > 0.0f;
            bool convex;
            CalculateJoins(w, lineJoin, miterLimit);

            // Calculate max vertex usage.
            cverts = 0;
            for (i = 0; i < this.cache.npaths; i++)
            {
                NVGpath path = this.cache.paths[i];
                cverts += path.count + path.nbevel + 1;
                if (fringe)
                    cverts += (path.count + path.nbevel * 5 + 1) * 2; // plus one for loop
            }

            verts = AllocTempVerts(cverts);
             
            convex = this.cache.npaths == 1 && this.cache.paths[0].convex;

            for (i = 0; i < this.cache.npaths; i++)
            {
                NVGpath path = this.cache.paths[i];
                int pts = path.first;
                int p0;
                int p1;
                float rw, lw, woff;
                float ru, lu;

                // Calculate shape vertices.
                woff = 0.5f * aa;
                dst = verts;

                if (fringe)
                {
                    // Looping
                    p0 = pts + path.count - 1;
                    p1 = pts;
                    for (j = 0; j < path.count; ++j)
                    {
                        if (this.cache.points[p1].flags.HasFlag(NVGpointFlags.NVG_PT_BEVEL))
                        {
                            float dlx0 = this.cache.points[p0].dy;
                            float dly0 = -this.cache.points[p0].dx;
                            float dlx1 = this.cache.points[p1].dy;
                            float dly1 = -this.cache.points[p1].dx;
                            if (this.cache.points[p1].flags.HasFlag(NVGpointFlags.NVG_PT_LEFT))
                            {
                                float lx = this.cache.points[p1].x + this.cache.points[p1].dmx * woff;
                                float ly = this.cache.points[p1].y + this.cache.points[p1].dmy * woff;
                                this.cache.verts[dst].Set(lx, ly, 0.5f, 1); dst++;
                            }
                            else
                            {
                                float lx0 = this.cache.points[p1].x + dlx0 * woff;
                                float ly0 = this.cache.points[p1].y + dly0 * woff;
                                float lx1 = this.cache.points[p1].x + dlx1 * woff;
                                float ly1 = this.cache.points[p1].y + dly1 * woff;
                                this.cache.verts[dst].Set(lx0, ly0, 0.5f, 1); dst++;
                                this.cache.verts[dst].Set(lx1, ly1, 0.5f, 1); dst++;
                            }
                        }
                        else
                        {
                            this.cache.verts[dst].Set(this.cache.points[p1].x + (this.cache.points[p1].dmx * woff), this.cache.points[p1].y + (this.cache.points[p1].dmy * woff), 0.5f, 1); dst++;
                        }
                        p0 = pts + j;
                        p1 = pts + j + 1;
                    }
                }
                else
                {
                    for (j = 0; j < path.count; ++j)
                    {
                        this.cache.verts[dst].Set(cache.points[pts + j].x, cache.points[pts + j].y, 0.5f, 1);
                        dst++;
                    }
                }

                path.nfill = (int)(dst - verts);
                path.fill = new NVGvertex[path.nfill];
                Array.Copy(this.cache.verts, verts, path.fill, 0, path.nfill);
                verts = dst;

                // Calculate fringe
                if (fringe)
                {
                    lw = w + woff;
                    rw = w - woff;
                    lu = 0;
                    ru = 1;
                    dst = verts;

                    // Create only half a fringe for convex shapes so that
                    // the shape can be rendered without stenciling.
                    if (convex)
                    {
                        lw = woff;  // This should generate the same vertex as fill inset above.
                        lu = 0.5f;  // Set outline fade at middle.
                    }

                    // Looping
                    p0 = pts + path.count - 1;
                    p1 = pts;

                    for (j = 0; j < path.count; ++j)
                    {
                        if (this.cache.points[p1].flags.HasFlag(NVGpointFlags.NVG_PT_BEVEL) || this.cache.points[p1].flags.HasFlag(NVGpointFlags.NVG_PR_INNERBEVEL))
                        {
                            dst = BevelJoin(this.cache.verts, dst, this.cache.points[p0], this.cache.points[p1], lw, rw, lu, ru, this.fringeWidth);
                        }
                        else
                        {
                            this.cache.verts[dst].Set(this.cache.points[p1].x + (this.cache.points[p1].dmx * lw), this.cache.points[p1].y + (this.cache.points[p1].dmy * lw), lu, 1); dst++;
                            this.cache.verts[dst].Set(this.cache.points[p1].x - (this.cache.points[p1].dmx * rw), this.cache.points[p1].y - (this.cache.points[p1].dmy * rw), ru, 1); dst++;
                        }
                        p0 = pts + j;
                        p1 = pts + j + 1;
                    }

                    // Loop it
                    this.cache.verts[dst].Set(this.cache.verts[verts].Position.X, this.cache.verts[verts].Position.Y, lu, 1); dst++;
                    this.cache.verts[dst].Set(this.cache.verts[verts + 1].Position.X, this.cache.verts[verts + 1].Position.Y, ru, 1); dst++;

                    path.nstroke = (int)(dst - verts);
                    path.stroke = new NVGvertex[path.nstroke];
                    Array.Copy(this.cache.verts, verts, path.stroke, 0, path.nstroke);
                    verts = dst;
                }
                else
                {
                    path.nstroke = 0;
                }
                this.cache.paths[i] = path;
            }
        }
        public void ExpandStroke(float w, NVGlineCap lineCap, NVGlineJoin lineJoin, float miterLimit)
        {
            NVGpathCache cache = this.cache;
            int verts;
            int dst;
            int cverts, i, j;
            float aa = this.fringeWidth;
            int ncap = CurveDivs(w, Math.PI, this.tessTol); // Calculate divisions per half circle.

            CalculateJoins(w, lineJoin, miterLimit);

            // Calculate max vertex usage.
            cverts = 0;
            for (i = 0; i < cache.npaths; i++)
            {
                NVGpath path = cache.paths[i];
                bool loop = path.closed;
                if (lineJoin == NVGlineJoin.NVG_ROUND)
                    cverts += (path.count + path.nbevel * (ncap + 2) + 1) * 2; // plus one for loop
                else
                    cverts += (path.count + path.nbevel * 5 + 1) * 2; // plus one for loop
                if (!loop)
                {
                    // space for caps
                    if (lineCap == NVGlineCap.NVG_ROUND)
                    {
                        cverts += (ncap * 2 + 2) * 2;
                    }
                    else
                    {
                        cverts += (3 + 3) * 2;
                    }
                }
            }

            verts = AllocTempVerts(cverts);

            for (i = 0; i < cache.npaths; i++)
            {
                NVGpath path = cache.paths[i];
                int pts = path.first;
                int p0;
                int p1;
                int s, e;
                bool loop;
                float dx, dy;

                path.fill = null;
                path.nfill = 0;

                // Calculate fringe or stroke
                loop =  path.closed;
                dst = verts;

                if (loop)
                {
                    // Looping
                    p0 = pts + path.count - 1;
                    p1 = pts;
                    s = 0;
                    e = path.count;
                }
                else
                {
                    // Add cap
                    p0 = pts;
                    p1 = pts+1;
                    s = 1;
                    e = path.count - 1;
                }

                if (!loop)
                {
                    // Add cap
                    dx = this.cache.points[p1].x - this.cache.points[p0].x;
                    dy = this.cache.points[p1].y - this.cache.points[p0].y;
                    NVGpoint.Normalize(ref dx, ref dy);
                    if (lineCap == NVGlineCap.NVG_BUTT)
                        dst = ButtCapStart(this.cache.verts, dst, this.cache.points[p0], dx, dy, w, -aa * 0.5f, aa);
                    else if (lineCap == NVGlineCap.NVG_BUTT || lineCap == NVGlineCap.NVG_SQUARE)
                        dst = ButtCapStart(this.cache.verts, dst, this.cache.points[p0], dx, dy, w, w - aa, aa);
                    else if (lineCap == NVGlineCap.NVG_ROUND)
                        dst = RoundCapStart(this.cache.verts, dst, this.cache.points[p0], dx, dy, w, ncap, aa);
                }

                for (j = s; j < e; ++j)
                {
                    if ((this.cache.points[p1].flags.HasFlag(NVGpointFlags.NVG_PT_BEVEL) || this.cache.points[p1].flags.HasFlag(NVGpointFlags.NVG_PR_INNERBEVEL)))
                    {
                        if (lineJoin == NVGlineJoin.NVG_ROUND)
                        {
                            dst = RoundJoin(this.cache.verts, dst, this.cache.points[p0], this.cache.points[p1], w, w, 0, 1, ncap, aa);
                        }
                        else
                        {
                            dst = BevelJoin(this.cache.verts, dst, this.cache.points[p0], this.cache.points[p1], w, w, 0, 1, aa);
                        }
                    }
                    else
                    {
                        this.cache.verts[dst].Set(this.cache.points[p1].x + (this.cache.points[p1].dmx * w), this.cache.points[p1].y + (this.cache.points[p1].dmy * w), 0, 1); dst++;
                        this.cache.verts[dst].Set(this.cache.points[p1].x - (this.cache.points[p1].dmx * w), this.cache.points[p1].y - (this.cache.points[p1].dmy * w), 1, 1); dst++;
                    }
                    p0 = p1++;
                }

                if (loop)
                {
                    // Loop it
                    this.cache.verts[dst].Set(this.cache.verts[verts].Position.X, this.cache.verts[verts].Position.Y, 0, 1); dst++;
                    this.cache.verts[dst].Set(this.cache.verts[verts+1].Position.X, this.cache.verts[verts+1].Position.Y, 1, 1); dst++;
                }
                else
                {
                    // Add cap
                    dx = this.cache.points[p1].x - this.cache.points[p0].x;
                    dy = this.cache.points[p1].y - this.cache.points[p0].y;
                    NVGpoint.Normalize(ref dx, ref dy);
                    if (lineCap == NVGlineCap.NVG_BUTT)
                        dst = ButtCapEnd(this.cache.verts, dst, this.cache.points[p1], dx, dy, w, -aa * 0.5f, aa);
                    else if (lineCap == NVGlineCap.NVG_BUTT || lineCap == NVGlineCap.NVG_SQUARE)
                        dst = ButtCapEnd(this.cache.verts, dst, this.cache.points[p1], dx, dy, w, w - aa, aa);
                    else if (lineCap == NVGlineCap.NVG_ROUND)
                        dst = RoundCapEnd(this.cache.verts, dst, this.cache.points[p1], dx, dy, w, ncap, aa);
                }

                path.nstroke = (int)(dst - verts);
                //path.stroke = dst;
                path.stroke = new NVGvertex[path.nstroke];
                Array.Copy(this.cache.verts, verts, path.stroke, 0, path.nstroke);

                verts = dst;
                cache.paths[i] = path;
            }
        }

        private static int BevelJoin(NVGvertex[] arr, int dst, NVGpoint p0, NVGpoint p1,
                              float lw, float rw, float lu, float ru, float fringe)
        {
            float rx0, ry0, rx1, ry1;
            float lx0, ly0, lx1, ly1;
            float dlx0 = p0.dy;
            float dly0 = -p0.dx;
            float dlx1 = p1.dy;
            float dly1 = -p1.dx;

            if (p1.flags.HasFlag(NVGpointFlags.NVG_PT_LEFT))
            {
                ChooseBevel(p1.flags.HasFlag(NVGpointFlags.NVG_PR_INNERBEVEL), p0, p1, lw, out lx0, out ly0, out lx1, out ly1);

                arr[dst].Set(lx0, ly0, lu, 1); dst++;
                arr[dst].Set(p1.x - dlx0 * rw, p1.y - dly0 * rw, ru, 1); dst++;

                if (p1.flags.HasFlag(NVGpointFlags.NVG_PT_BEVEL))
                {
                    arr[dst].Set(lx0, ly0, lu, 1); dst++;
                    arr[dst].Set(p1.x - dlx0 * rw, p1.y - dly0 * rw, ru, 1); dst++;

                    arr[dst].Set(lx1, ly1, lu, 1); dst++;
                    arr[dst].Set(p1.x - dlx1 * rw, p1.y - dly1 * rw, ru, 1); dst++;
                }
                else
                {
                    rx0 = p1.x - p1.dmx * rw;
                    ry0 = p1.y - p1.dmy * rw;

                    arr[dst].Set(p1.x, p1.y, 0.5f, 1); dst++;
                    arr[dst].Set(p1.x - dlx0 * rw, p1.y - dly0 * rw, ru, 1); dst++;

                    arr[dst].Set(rx0, ry0, ru, 1); dst++;
                    arr[dst].Set(rx0, ry0, ru, 1); dst++;

                    arr[dst].Set(p1.x, p1.y, 0.5f, 1); dst++;
                    arr[dst].Set(p1.x - dlx1 * rw, p1.y - dly1 * rw, ru, 1); dst++;
                }

                arr[dst].Set(lx1, ly1, lu, 1); dst++;
                arr[dst].Set(p1.x - dlx1 * rw, p1.y - dly1 * rw, ru, 1); dst++;

            }
            else
            {
                ChooseBevel(p1.flags.HasFlag(NVGpointFlags.NVG_PR_INNERBEVEL), p0, p1, -rw, out rx0, out ry0, out rx1, out ry1);

                arr[dst].Set(p1.x + dlx0 * lw, p1.y + dly0 * lw, lu, 1); dst++;
                arr[dst].Set(rx0, ry0, ru, 1); dst++;

                if (p1.flags.HasFlag(NVGpointFlags.NVG_PT_BEVEL))
                {
                    arr[dst].Set(p1.x + dlx0 * lw, p1.y + dly0 * lw, lu, 1); dst++;
                    arr[dst].Set(rx0, ry0, ru, 1); dst++;

                    arr[dst].Set(p1.x + dlx1 * lw, p1.y + dly1 * lw, lu, 1); dst++;
                    arr[dst].Set(rx1, ry1, ru, 1); dst++;
                }
                else
                {
                    lx0 = p1.x + p1.dmx * lw;
                    ly0 = p1.y + p1.dmy * lw;

                    arr[dst].Set(p1.x + dlx0 * lw, p1.y + dly0 * lw, lu, 1); dst++;
                    arr[dst].Set(p1.x, p1.y, 0.5f, 1); dst++;

                    arr[dst].Set(lx0, ly0, lu, 1); dst++;
                    arr[dst].Set(lx0, ly0, lu, 1); dst++;

                    arr[dst].Set(p1.x + dlx1 * lw, p1.y + dly1 * lw, lu, 1); dst++;
                    arr[dst].Set(p1.x, p1.y, 0.5f, 1); dst++;
                }

                arr[dst].Set(p1.x + dlx1 * lw, p1.y + dly1 * lw, lu, 1); dst++;
                arr[dst].Set(rx1, ry1, ru, 1); dst++;
            }

            return dst;
        }
        private  static int RoundJoin(NVGvertex[] arr, int dst, NVGpoint  p0, NVGpoint  p1,
                                      float lw, float rw, float lu, float ru, int ncap, float fringe)
        {
            int i, n;
            float dlx0 = p0.dy;
            float dly0 = -p0.dx;
            float dlx1 = p1.dy;
            float dly1 = -p1.dx;

            if (p1.flags.HasFlag(NVGpointFlags.NVG_PT_LEFT))
            {
                float lx0, ly0, lx1, ly1, a0, a1;
                ChooseBevel(p1.flags.HasFlag(NVGpointFlags.NVG_PR_INNERBEVEL), p0, p1, lw, out lx0, out ly0, out lx1, out ly1);
                a0 = (float)Math.Atan2(-dly0, -dlx0);
                a1 = (float)Math.Atan2(-dly1, -dlx1);
                if (a1 > a0) a1 -= (float)Math.PI * 2;

                arr[dst].Set(lx0, ly0, lu, 1); dst++;
                arr[dst].Set(p1.x - dlx0 * rw, p1.y - dly0 * rw, ru, 1); dst++;

                n = (int)MathHelper.Clamp(Math.Ceiling(((a0 - a1) / (float)Math.PI) * ncap), 2, ncap);
                for (i = 0; i < n; i++)
                {
                    float u = i / (float)(n - 1);
                    float a = a0 + u * (a1 - a0);
                    float rx = p1.x + (float)Math.Cos(a) * rw;
                    float ry = p1.y + (float)Math.Sin(a) * rw;
                    arr[dst].Set(p1.x, p1.y, 0.5f, 1); dst++;
                    arr[dst].Set(rx, ry, ru, 1); dst++;
                }

                arr[dst].Set(lx1, ly1, lu, 1); dst++;
                arr[dst].Set(p1.x - dlx1 * rw, p1.y - dly1 * rw, ru, 1); dst++;

            }
            else
            {
                float rx0, ry0, rx1, ry1, a0, a1;
                ChooseBevel(p1.flags.HasFlag(NVGpointFlags.NVG_PR_INNERBEVEL), p0, p1, -rw, out rx0, out ry0, out rx1, out ry1);
                a0 = (float)Math.Atan2(dly0, dlx0);
                a1 = (float)Math.Atan2(dly1, dlx1);
                if (a1 < a0) a1 += (float)Math.PI * 2;

                arr[dst].Set(p1.x + dlx0 * rw, p1.y + dly0 * rw, lu, 1); dst++;
                arr[dst].Set(rx0, ry0, ru, 1); dst++;

                n = (int)MathHelper.Clamp( Math.Ceiling(((a1 - a0) / (float)Math.PI) * ncap), 2, ncap);
                for (i = 0; i < n; i++)
                {
                    float u = i / (float)(n - 1);
                    float a = a0 + u * (a1 - a0);
                    float lx = p1.x + (float)Math.Cos(a) * lw;
                    float ly = p1.y + (float)Math.Sin(a) * lw;
                    arr[dst].Set(lx, ly, lu, 1); dst++;
                    arr[dst].Set(p1.x, p1.y, 0.5f, 1); dst++;
                }

                arr[dst].Set(p1.x + dlx1 * rw, p1.y + dly1 * rw, lu, 1); dst++;
                arr[dst].Set(rx1, ry1, ru, 1); dst++;

            }
            return dst;
        }

        private void TesselateBezier(float x1, float y1, float x2, float y2,
                                  float x3, float y3, float x4, float y4,
                                  int level, NVGpointFlags type)
        {
            float x12, y12, x23, y23, x34, y34, x123, y123, x234, y234, x1234, y1234;
            float dx, dy, d2, d3;

            if (level > 10) return;

            x12 = (x1 + x2) * 0.5f;
            y12 = (y1 + y2) * 0.5f;
            x23 = (x2 + x3) * 0.5f;
            y23 = (y2 + y3) * 0.5f;
            x34 = (x3 + x4) * 0.5f;
            y34 = (y3 + y4) * 0.5f;
            x123 = (x12 + x23) * 0.5f;
            y123 = (y12 + y23) * 0.5f;

            dx = x4 - x1;
            dy = y4 - y1;
            d2 = (float)Math.Abs(((x2 - x4) * dy - (y2 - y4) * dx));
            d3 = (float)Math.Abs(((x3 - x4) * dy - (y3 - y4) * dx));

            if ((d2 + d3) * (d2 + d3) < this.tessTol * (dx * dx + dy * dy))
            {
                AddPoint(x4, y4, type);
                return;
            }

            /*	if (nvg__absf(x1+x3-x2-x2) + nvg__absf(y1+y3-y2-y2) + nvg__absf(x2+x4-x3-x3) + nvg__absf(y2+y4-y3-y3) < ctx.tessTol) {
                    nvg__addPoint(ctx, x4, y4, type);
                    return;
                }*/

            x234 = (x23 + x34) * 0.5f;
            y234 = (y23 + y34) * 0.5f;
            x1234 = (x123 + x234) * 0.5f;
            y1234 = (y123 + y234) * 0.5f;

            TesselateBezier(x1, y1, x12, y12, x123, y123, x1234, y1234, level + 1, 0);
            TesselateBezier(x1234, y1234, x234, y234, x34, y34, x4, y4, level + 1, type);
        }

        private float PolyArea(NVGpoint[] pts, int start, int npts)
        {
            int i;
            float area = 0;
            for (i = 2; i < npts; i++)
            {
                NVGpoint a = pts[start + 0];
                NVGpoint b = pts[start + i - 1];
                NVGpoint c = pts[start + i];
                area += Triarea2(a.x, a.y, b.x, b.y, c.x, c.y);
            }
            return area * 0.5f;
        }
        private static float Triarea2(float ax, float ay, float bx, float by, float cx, float cy)
        {
            float abx = bx - ax;
            float aby = by - ay;
            float acx = cx - ax;
            float acy = cy - ay;
            return acx * aby - abx * acy;
        }

        private void PolyReverse(NVGpoint[] pts, int start, int npts)
        {
            NVGpoint tmp;
            int i = start + 0, j = start + npts - 1;
            while (i < j)
            {
                tmp = pts[i];
                pts[i] = pts[j];
                pts[j] = tmp;
                i++;
                j--;
            }
        }

        public void CalculateJoins(float w, NVGlineJoin lineJoin, float miterLimit)
        {
            int i, j;
            float iw = 0.0f;

            if (w > 0.0f) iw = 1.0f / w;

            // Calculate which joins needs extra vertices to append, and gather vertex count.
            for (i = 0; i < this.cache.npaths; i++)
            {
                NVGpath path = this.cache.paths[i];
                int pts = path.first;
                int p0 = pts + path.count - 1;
                int p1 = pts;
                int nleft = 0;

                path.nbevel = 0;

                for (j = 0; j < path.count; j++)
                {
                    float dlx0, dly0, dlx1, dly1, dmr2, cross, limit;
                    dlx0 = this.cache.points[p0].dy;
                    dly0 = -this.cache.points[p0].dx;
                    dlx1 = this.cache.points[p1].dy;
                    dly1 = -this.cache.points[p1].dx;
                    // Calculate extrusions
                    this.cache.points[p1].dmx = (dlx0 + dlx1) * 0.5f;
                    this.cache.points[p1].dmy = (dly0 + dly1) * 0.5f;
                    dmr2 = this.cache.points[p1].dmx * this.cache.points[p1].dmx + this.cache.points[p1].dmy * this.cache.points[p1].dmy;
                    if (dmr2 > 0.000001f)
                    {
                        float scale = 1.0f / dmr2;
                        if (scale > 600.0f)
                        {
                            scale = 600.0f;
                        }
                        this.cache.points[p1].dmx *= scale;
                        this.cache.points[p1].dmy *= scale;
                    }

                    // Clear flags, but keep the corner.
                    this.cache.points[p1].flags = (this.cache.points[p1].flags.HasFlag(NVGpointFlags.NVG_PT_CORNER)) ? NVGpointFlags.NVG_PT_CORNER : 0;

                    // Keep track of left turns.
                    cross = this.cache.points[p1].dx * this.cache.points[p0].dy - this.cache.points[p0].dx * this.cache.points[p1].dy;
                    if (cross > 0.0f)
                    {
                        nleft++;
                        this.cache.points[p1].flags |= NVGpointFlags.NVG_PT_LEFT;
                    }

                    // Calculate if we should use bevel or miter for inner join.
                    limit = Math.Max(1.01f, Math.Min(this.cache.points[p0].len, this.cache.points[p1].len) * iw);
                    if ((dmr2 * limit * limit) < 1.0f)
                        this.cache.points[p1].flags |= NVGpointFlags.NVG_PR_INNERBEVEL;

                    // Check to see if the corner needs to be beveled.
                    if (this.cache.points[p1].flags.HasFlag(NVGpointFlags.NVG_PT_CORNER))
                    {
                        if ((dmr2 * miterLimit * miterLimit) < 1.0f || lineJoin == NVGlineJoin.NVG_BEVEL || lineJoin == NVGlineJoin.NVG_ROUND)
                        {
                            this.cache.points[p1].flags |= NVGpointFlags.NVG_PT_BEVEL;
                        }
                    }

                    if (this.cache.points[p1].flags.HasFlag(NVGpointFlags.NVG_PT_BEVEL) || this.cache.points[p1].flags.HasFlag(NVGpointFlags.NVG_PR_INNERBEVEL))
                        path.nbevel++;
                    p0 = pts + j;
                    p1 = pts + j + 1;
                }

                path.convex = (nleft == path.count);
            }
        }

        private int AllocTempVerts(int nverts)
        {
            if (nverts > this.cache.cverts)
            {
                int cverts = (nverts + 0xff) & ~0xff; // Round up to prevent allocations when things change just slightly.
                this.cache.verts = ResizeArray(this.cache.verts, cverts);
                this.cache.cverts = cverts;
            }

            return this.cache.nverts;
        }

        private static int CurveDivs(float r, double arc, float tol)
        {
            float da = (float)Math.Acos(r / (r + tol)) * 2.0f;
            return Math.Max(2, (int)Math.Ceiling(arc / da));
        }

        private static int ButtCapStart(NVGvertex[] arr, int dst, NVGpoint p,
                                        float dx, float dy, float w, float d, float aa)
        {
            float px = p.x - dx * d;
            float py = p.y - dy * d;
            float dlx = dy;
            float dly = -dx;
            arr[dst].Set(px + dlx * w - dx * aa, py + dly * w - dy * aa, 0, 0); dst++;
            arr[dst].Set(px - dlx * w - dx * aa, py - dly * w - dy * aa, 1, 0); dst++;
            arr[dst].Set(px + dlx * w, py + dly * w, 0, 1); dst++;
            arr[dst].Set(px - dlx * w, py - dly * w, 1, 1); dst++;
            return dst;
        }

        private static int ButtCapEnd(NVGvertex[] arr, int dst, NVGpoint p,
                                      float dx, float dy, float w, float d, float aa)
        {
            float px = p.x + dx * d;
            float py = p.y + dy * d;
            float dlx = dy;
            float dly = -dx;
            arr[dst].Set(px + dlx * w, py + dly * w, 0, 1); dst++;
            arr[dst].Set(px - dlx * w, py - dly * w, 1, 1); dst++;
            arr[dst].Set(px + dlx * w + dx * aa, py + dly * w + dy * aa, 0, 0); dst++;
            arr[dst].Set(px - dlx * w + dx * aa, py - dly * w + dy * aa, 1, 0); dst++;
            return dst;
        }

        private static int RoundCapStart(NVGvertex[] arr, int dst, NVGpoint p,
                                            float dx, float dy, float w, int ncap, float aa)
        {
             int i;
            float px = p.x;
            float py = p.y;
            float dlx = dy;
            float dly = -dx;
            for (i = 0; i < ncap; i++)
            {
                float a = (float)(i / (float)(ncap - 1) * Math.PI);
                float ax = (float)(Math.Cos(a) * w), ay = (float)(Math.Sin(a) * w);
                arr[dst].Set(px - dlx * ax - dx * ay, py - dly * ax - dy * ay, 0, 1); dst++;
                arr[dst].Set(px, py, 0.5f, 1); dst++;
            }
            arr[dst].Set(px + dlx * w, py + dly * w, 0, 1); dst++;
            arr[dst].Set(px - dlx * w, py - dly * w, 1, 1); dst++;
            return dst;
         }

        private static int RoundCapEnd(NVGvertex[] arr, int dst, NVGpoint p,
                                       float dx, float dy, float w, int ncap, float aa)
        {
            int i;
            float px = p.x;
            float py = p.y;
            float dlx = dy;
            float dly = -dx;
            arr[dst].Set(px + dlx * w, py + dly * w, 0, 1); dst++;
            arr[dst].Set(px - dlx * w, py - dly * w, 1, 1); dst++;
            for (i = 0; i < ncap; i++)
            {
                float a = (float)(i / (float)(ncap - 1) * Math.PI);
                float ax = (float)(Math.Cos(a) * w), ay = (float)(Math.Sin(a) * w);
                arr[dst].Set(px, py, 0.5f, 1); dst++;
                arr[dst].Set(px - dlx * ax + dx * ay, py - dly * ax + dy * ay, 0, 1); dst++;
            }
            return dst;
        }
        private static void ChooseBevel(bool bevel, NVGpoint p0, NVGpoint p1, float w,
                                        out float x0, out float y0, out float x1, out float y1)
        {
            if (bevel)
            {
                x0 = p1.x + p0.dy * w;
                y0 = p1.y - p0.dx * w;
                x1 = p1.x + p1.dy * w;
                y1 = p1.y - p1.dx * w;
            }
            else
            {
                x0 = p1.x + p1.dmx * w;
                y0 = p1.y + p1.dmy * w;
                x1 = p1.x + p1.dmx * w;
                y1 = p1.y + p1.dmy * w;
            }
        }
        // TODO.Put this in some utils class 
        // Reallocates an array with a new size, and copies the contents
        // of the old array to the new array.
        // Arguments:
        //   oldArray  the old array, to be reallocated.
        //   newSize   the new array size.
        // Returns     A new array with the same contents.
        public static T[] ResizeArray<T>(T[] oldArray, int newSize) where T : struct
        {
            int oldSize = oldArray == null ? 0 : oldArray.Length;
            T[] newArray = new T[newSize];
            int preserveLength = System.Math.Min(oldSize, newSize);
            if (preserveLength > 0)
                System.Array.Copy(oldArray, newArray, preserveLength);
            return newArray;
        }

#region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (glctx != null)
                        glctx.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~NVGcontext() {
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
    }

    public enum NVGtexture
    {
        NVG_TEXTURE_ALPHA = 0x01,
        NVG_TEXTURE_RGBA = 0x02,
    }

    public class NVGscissor
    {
        public float[] xform = new float[6];
        public float[] extent = new float[2];

        public NVGscissor Clone()
        {
            NVGscissor newcissor = (NVGscissor)this.MemberwiseClone();
            newcissor.xform = (float[])this.xform.Clone();
            newcissor.extent = (float[])this.extent.Clone();
            return newcissor;
        }
    }

    public struct NVGpath
    {
        public int first;
        public int count;
        public bool closed;
        public int nbevel;
        public NVGvertex[] fill;
        public int nfill;
        public NVGvertex[] stroke;
        public int nstroke;
        public NVGwinding winding;
        public bool convex;
    }

    public struct NVGparams
    {
        public object UserPtr { get; set; }
        public bool EdgeAntiAlias { get; set; }

        public delegate void RenderCreateDelegate(object uptr);
        public delegate void RenderCreateTextureDelegate(object uptr, int type, int w, int h, NVGimageFlags imageFlags, byte[] data);
        public delegate void RenderDeleteTextureDelegate(object uptr, int type);
        public delegate void RenderUpdateTextureDelegate(object uptr, int image, int x, int y, int w, int h, byte[] data);
        public delegate void RenderGetTextureSizeDelegate(object uptr, int image, out int x, out int y);
        public delegate void RenderViewportDelegate(object uptr, int width, int height);
        public delegate void RenderCancelDelegate(object uptr);
        public delegate void RenderFlushDelegate(object uptr);
        public delegate void RenderFillDelegate(object uptr);
        public delegate void RenderStrokeDelegate(object uptr);
        public delegate void RenderTrianglesDelegate(object uptr);
        public delegate void RenderDeleteDelegate(object uptr);


        public RenderCreateDelegate RenderCreate;
        public RenderCreateTextureDelegate RenderCreateTexture;
        public RenderViewportDelegate RenderViewport;
        public RenderCancelDelegate RenderCancel;
        public RenderFlushDelegate RenderFlush;
#if TODO
	int (*renderCreate)(void* uptr);
	int (*renderCreateTexture)(void* uptr, int type, int w, int h, int imageFlags, const unsigned char* data);
	int (*renderDeleteTexture)(void* uptr, int image);
	int (*renderUpdateTexture)(void* uptr, int image, int x, int y, int w, int h, const unsigned char* data);
	int (*renderGetTextureSize)(void* uptr, int image, int* w, int* h);
	void (*renderViewport)(void* uptr, int width, int height);
	void (*renderCancel)(void* uptr);
	void (*renderFlush)(void* uptr);
	void (*renderFill)(void* uptr, NVGpaint* paint, NVGscissor* scissor, float fringe, const float* bounds, const NVGpath* paths, int npaths);
	void (*renderStroke)(void* uptr, NVGpaint* paint, NVGscissor* scissor, float fringe, float strokeWidth, const NVGpath* paths, int npaths);
	void (*renderTriangles)(void* uptr, NVGpaint* paint, NVGscissor* scissor, const NVGvertex* verts, int nverts);
	void (*renderDelete)(void* uptr);
#endif
    }


    internal struct FONScontext { }
}
