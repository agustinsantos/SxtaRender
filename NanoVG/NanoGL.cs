using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoVG
{
    public enum GLNVGuniformLoc
    {
        GLNVG_LOC_VIEWSIZE,
        GLNVG_LOC_TEX,
        GLNVG_LOC_FRAG,
        GLNVG_MAX_LOCS
    }

    public enum GLNVGshaderType
    {
        NSVG_SHADER_FILLGRAD,
        NSVG_SHADER_FILLIMG,
        NSVG_SHADER_SIMPLE,
        NSVG_SHADER_IMG
    }

    public struct GLNVGshader
    {
        uint prog;
        uint frag;
        uint vert;
        int[] loc; //[GLNVG_MAX_LOCS];
    }
    public struct GLNVGtexture
    {
        int id;
        uint tex;
        int width, height;
        int type;
        int flags;
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
        int type;
        int image;
        int pathOffset;
        int pathCount;
        int triangleOffset;
        int triangleCount;
        int uniformOffset;
    }

    public struct GLNVGpath
    {
        int fillOffset;
        int fillCount;
        int strokeOffset;
        int strokeCount;
    }

    public struct GLNVGfragUniforms {
 		float[] scissorMat[12]; // matrices are actually 3 vec4s
		float[] paintMat[12];
		  NVGcolor innerCol;
		  NVGcolor outerCol;
		float[] scissorExt[2];
		float[] scissorScale[2];
		float[] extent[2];
		float radius;
		float feather;
		float strokeMult;
		float strokeThr;
		int texType;
		int type;
    }
}
