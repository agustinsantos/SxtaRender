using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    internal enum NVGpointFlags
    {
        NVG_PT_CORNER = 0x01,
        NVG_PT_LEFT = 0x02,
        NVG_PT_BEVEL = 0x04,
        NVG_PR_INNERBEVEL = 0x08,
    }

    internal struct NVGstate
    {
        NVGpaint fill;
        NVGpaint stroke;
        float strokeWidth;
        float miterLimit;
        int lineJoin;
        int lineCap;
        float alpha;
        float[] xform; // = new float[6];
        NVGscissor scissor;
        float fontSize;
        float letterSpacing;
        float lineHeight;
        float fontBlur;
        int textAlign;
        int fontId;
    }

    internal struct NVGpoint
    {
        float x, y;
        float dx, dy;
        float len;
        float dmx, dmy;
        byte flags;
    }

    internal struct NVGpathCache
    {
        NVGpoint points;
        int npoints;
        int cpoints;
        NVGpath paths;
        int npaths;
        int cpaths;
        NVGvertex verts;
        int nverts;
        int cverts;
        Bound bounds;
    }


    public class NVGcontext
    {
        internal NVGparams params_;
        internal float[] commands;
        internal int ccommands;
        internal int ncommands;
        internal float commandx, commandy;
        internal NVGstate[] states;// = new NVGstate[NVG_MAX_STATES];
        internal int nstates;
        internal NVGpathCache cache;
        internal float tessTol;
        internal float distTol;
        internal float fringeWidth;
        internal float devicePxRatio;
        internal FONScontext fs;
        internal int[] fontImages;// = new int[NVG_MAX_FONTIMAGES];
        internal int fontImageIdx;
        internal int drawCallCount;
        internal int fillTriCount;
        internal int strokeTriCount;
        internal int textTriCount;
    }

    public enum NVGtexture
    {
        NVG_TEXTURE_ALPHA = 0x01,
        NVG_TEXTURE_RGBA = 0x02,
    }

    public struct NVGscissor
    {
        float[] xform;// = new float[6];
        float[] extent;// = new float[2];
    }


    public struct NVGvertex
    {
        float x, y, u, v;
    }


    public struct NVGpath
    {
        int first;
        int count;
        bool closed;
        int nbevel;
        NVGvertex fill;
        int nfill;
        NVGvertex stroke;
        int nstroke;
        int winding;
        int convex;
    }

    public struct NVGparams
    {
        public delegate void RenderCreateDelegate(object uptr);
        public delegate void RenderCreateTextureDelegate(object uptr, int type, int w, int h, int imageFlags, byte[] data);
        public delegate void RenderViewportDelegate(object uptr, int width, int height);
        public delegate void RenderCancelDelegate(object uptr);
        public delegate void RenderFlushDelegate(object uptr);
        public object userPtr;
        public int edgeAntiAlias;

        public RenderViewportDelegate renderViewport;
        public RenderCancelDelegate renderCancel;
        public RenderFlushDelegate renderFlush;
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
