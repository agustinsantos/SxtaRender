using System;

namespace Mogre.Utils.GluTesselator
{
	/// <summary>
	/// Summary description for GL.
	/// </summary>
	public class GL
	{
		public const int GL_TRIANGLE_STRIP = 1;
		public const int GL_LINE_LOOP = 2;
		public const int GL_TRIANGLE_FAN = 3;
		public const int GL_TRIANGLES = 4;
	}

	public class GLU
	{
		public const int GLU_TESS_BEGIN = 100100;
		public const int GLU_TESS_BEGIN_DATA = 100106;
		public const int GLU_TESS_BOUNDARY_ONLY = 100141;
		public const int GLU_TESS_COMBINE = 100105;
		public const int GLU_TESS_COMBINE_DATA = 100111;
		public const int GLU_TESS_COORD_TOO_LARGE = 100155;
		public const int GLU_TESS_EDGE_FLAG = 100104;
		public const int GLU_TESS_EDGE_FLAG_DATA = 100110;
		public const int GLU_TESS_END = 100102;
		public const int GLU_TESS_END_DATA = 100108;
		public const int GLU_TESS_ERROR = 100103;
		public const int GLU_TESS_ERROR1 = 100151;
		public const int GLU_TESS_ERROR2 = 100152;
		public const int GLU_TESS_ERROR3 = 100153;
		public const int GLU_TESS_ERROR4 = 100154;
		public const int GLU_TESS_ERROR5 = 100155;
		public const int GLU_TESS_ERROR6 = 100156;
		public const int GLU_TESS_ERROR7 = 100157;
		public const int GLU_TESS_ERROR8 = 100158;
		public const int GLU_TESS_ERROR_DATA = 100109;
		public const double GLU_TESS_MAX_COORD = 9.9999999999999998e+149;
		public const int GLU_TESS_MISSING_BEGIN_CONTOUR = 100152;
		public const int GLU_TESS_MISSING_BEGIN_POLYGON = 100151;
		public const int GLU_TESS_MISSING_END_CONTOUR = 100154;
		public const int GLU_TESS_MISSING_END_POLYGON = 100153;
		public const int GLU_TESS_NEED_COMBINE_CALLBACK = 100156;
		public const int GLU_TESS_TOLERANCE = 100142;
		public const int GLU_TESS_VERTEX = 100101;
		public const int GLU_TESS_VERTEX_DATA = 100107;
		public const int GLU_TESS_WINDING_ABS_GEQ_TWO = 100134;
		public const int GLU_TESS_WINDING_NEGATIVE = 100133;
		public const int GLU_TESS_WINDING_NONZERO = 100131;
		public const int GLU_TESS_WINDING_ODD = 100130;
		public const int GLU_TESS_WINDING_POSITIVE = 100132;
		public const int GLU_TESS_WINDING_RULE = 100140;
		
		public const int GLU_INVALID_ENUM = 100900;
		public const int GLU_INVALID_VALUE = 100901;
		public const int GLU_OUT_OF_MEMORY = 100902;
	}
}
