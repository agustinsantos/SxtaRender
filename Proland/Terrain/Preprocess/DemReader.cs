using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Proland.Terrain
{
    public enum GROUND_REF_SYSTEM { Geographic = 0, UTM, StatePlane };
    public enum GROUND_UNIT { Radian = 0, Feet, Meters, ArcSeconds };
    public enum ELEVATION_UNIT { Feet = 1, Meters };
    public enum PROCESS_CODE
    {
        AutocorrelationResampleSimpleBilinear = 1,
        ManualProfileGRIDEMSimpleBilinear = 2,
        DLGHysographyCTOG8DirectionLinear = 3,
        DCASS4DirectionLinearInterpolation = 4,
        DLGHypsographyLINETRACE = 5,
        DLGHypsographyCPSANUDEMGRASSComplexPolynomial = 5,
        ElectronicImaging = 6
    };

    /// <summary>
    /// A DEM reader. Code from https://dem.codeplex.com/SourceControl/latest#Dem/DEM.cs
    /// </summary>
    public class DemReader
    {
        private ARecord _mARecord;
        private BRecord _mBRecord;

        public DemReader()
        {
            _mARecord = null;
            _mBRecord = null;
        }

        /// <summary>
        /// Get the A-Record
        /// </summary>
        public ARecord ARecord { get { return _mARecord; } }
        /// <summary>
        /// Get the B-Record
        /// </summary>
        public BRecord BRecord { get { return _mBRecord; } }

        /// <summary>
        /// Read a *.dem file
        /// </summary>
        /// <param name="filename"></param>
        public void Read(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            char[] buffer = new char[1024]; // size of A Record
            reader.Read(buffer, 0, 1024);

            _mARecord = new ARecord();
            _mARecord.file_name = ParseString(buffer, 0, 40).ToCharArray();
            _mARecord.free_text_format = ParseString(buffer, 40, 40).ToCharArray();
            _mARecord.SE_geographic_corner_S = ParseString(buffer, 109, 13).ToCharArray();
            _mARecord.SE_geographic_corner_E = ParseString(buffer, 122, 13).ToCharArray();
            _mARecord.process_code = buffer[135];
            _mARecord.origin_code = ParseString(buffer, 140, 4).ToCharArray();
            _mARecord.dem_level_code = ParseInt(buffer, 144);
            _mARecord.elevation_pattern = ParseInt(buffer, 150);
            _mARecord.ground_ref_system = ParseInt(buffer, 156);
            _mARecord.ground_ref_zone = ParseInt(buffer, 162);
            for (int i = 0; i < 15; i++)
                _mARecord.projection[0] = ParseDouble(buffer, 168 + i * 24);
            _mARecord.ground_unit = ParseInt(buffer, 528);
            _mARecord.elevation_unit = ParseInt(buffer, 534);
            _mARecord.side_count = ParseInt(buffer, 540);
            _mARecord.sw_coord[0] = (float)ParseDouble(buffer, 546); // UTM grid (measured in meters)
            _mARecord.sw_coord[1] = (float)ParseDouble(buffer, 570);
            _mARecord.nw_coord[0] = (float)ParseDouble(buffer, 594);
            _mARecord.nw_coord[1] = (float)ParseDouble(buffer, 618);
            _mARecord.ne_coord[0] = (float)ParseDouble(buffer, 642);
            _mARecord.ne_coord[1] = (float)ParseDouble(buffer, 666);
            _mARecord.se_coord[0] = (float)ParseDouble(buffer, 690);
            _mARecord.se_coord[1] = (float)ParseDouble(buffer, 714);
            _mARecord.elevation_min = ParseDouble(buffer, 738);
            _mARecord.elevation_max = ParseDouble(buffer, 762);
            _mARecord.ccw_angle = ParseDouble(buffer, 786);
            _mARecord.elevation_accuracy = ParseInt(buffer, 810);
            _mARecord.xyz_resolution[0] = ParseFloat(buffer, 816);
            _mARecord.xyz_resolution[1] = ParseFloat(buffer, 828);
            _mARecord.xyz_resolution[2] = ParseFloat(buffer, 840);
            _mARecord.eastings_cols = ParseInt(buffer, 858);
            _mARecord.northings_rows = ParseInt(buffer, 858); // WARNING. SHOULD NOT USE THIS VALUE. BUT WE ASSUME IS THE SAME WITH eastings SINCE IT IS ALWAYS THE CASE.
            _mARecord.suspect_void = ParseInt(buffer, 886, 2);
            _mARecord.percent_void = ParseInt(buffer, 896);

            // read the rest of the DEM
            StreamTokenizer tokenizer = new StreamTokenizer(reader, new char[] { ' ' });

            _mBRecord = null;
            for (int col = 0; col < _mARecord.eastings_cols; col++)
            {
                tokenizer.Next(); // row id
                tokenizer.Next(); // col id
                _mARecord.northings_rows = ToInt(tokenizer.Next());

                if (_mBRecord == null)
                    _mBRecord = new BRecord(_mARecord.eastings_cols, _mARecord.northings_rows);

                tokenizer.Next(); // skip next six fields
                tokenizer.Next();
                tokenizer.Next();
                tokenizer.Next();
                tokenizer.Next();
                tokenizer.Next();

                // for (int row = record.northings_rows - 1; row >= 0; row--)
                for (int row = 0; row < _mARecord.northings_rows; row++)
                {
                    _mBRecord.elevations[col, row] = ToShort(tokenizer.Next());
                }
            }

            reader.Close();
        }

        #region " Helper Functions "
        string ParseString(char[] buffer, int start, int count)
        {
            String s = new string(buffer, start, count);
            s = s.Trim();
            return s;
        }
        int ParseInt(char[] buffer, int start)
        {
            return ParseInt(buffer, start, 6);
        }
        int ParseInt(char[] buffer, int start, int count)
        {
            string s = new string(buffer, start, count).Replace('D', 'E');
            int i = 0;
            if (!int.TryParse(s.Trim(), out i)) i = -1;
            return i;
        }
        int ToInt(string s)
        {
            int i;
            int.TryParse(s, out i);
            return i;
        }
        short ToShort(string s)
        {
            short i;
            short.TryParse(s, out i);
            return i;
        }
        float ParseFloat(char[] buffer, int start)
        {
            return ParseFloat(buffer, start, 12);
        }
        float ParseFloat(char[] buffer, int start, int count)
        {
            String s = new string(buffer, start, count).Replace('D', 'E');
            float f = 0;
            if (!float.TryParse(s.Trim(), out f)) f = -1;
            return f;
        }
        double ParseDouble(char[] buffer, int start)
        {
            return ParseDouble(buffer, start, 24);
        }
        double ParseDouble(char[] buffer, int start, int count)
        {
            String s = new string(buffer, start, count).Replace('D', 'E');
            double d = 0;
            if (!double.TryParse(s.Trim(), out d)) d = -1;
            return d;

        }
        #endregion

        class StreamTokenizer
        {
            string[] _mTokens;
            int current;

            public StreamTokenizer(TextReader reader, char[] delimiter)
            {
                string s = reader.ReadToEnd().Replace("-32767", " -100");
                _mTokens = s.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                current = 0;
            }

            public string Next()
            {
                if (current < _mTokens.Length)
                    return _mTokens[current++];

                throw new Exception("No More Data!");
            }
        }
    }


    public class ARecord
    {
        /// <summary>
        /// The authorized digital cell name followed by a comma, space, and the two-character State designator(s) separated by hyphens.
        /// Abbreviations for other countries, such as Canada and Mexico, shall not be represented in the DEM header.
        /// </summary>
        public char[] file_name = new char[40];

        /// <summary>
        /// Free format descriptor field, contains useful information related to digital process such as digitizing instrument, photo codes, slot widths, etc.
        /// </summary>
        public char[] free_text_format = new char[40];

        /// <summary>
        /// Southing of the southeast geographic corner
        /// SE geographic quadrangle corner ordered as:
        ///     x = Longitude = SDDDMMSS.SSSS
        ///     y = Latitude = SDDDMMSS.SSSS
        /// (neg sign (S) right justified, no leading zeroes, plus sign (S) implied)
        /// </summary>
        public char[] SE_geographic_corner_S = new char[13];

        /// <summary>
        /// Easting of the southeast geographic corner
        /// SE geographic quadrangle corner ordered as:
        ///     x = Longitude = SDDDMMSS.SSSS
        ///     y = Latitude = SDDDMMSS.SSSS
        /// (neg sign (S) right justified, no leading zeroes, plus sign (S) implied)
        /// </summary>
        public char[] SE_geographic_corner_E = new char[13];

        /// <summary>
        /// 1=Autocorrelation RESAMPLE Simple bilinear
        /// 2=Manual profile GRIDEM Simple bilinear
        /// 3=DLG/hypsography CTOG 8-direction linear
        /// 4=Interpolation from photogrammetric system contours DCASS 4-direction linear
        /// 5=DLG/hypsography LINETRACE, LT4X Complex linear
        /// 6=DLG/hypsography CPS-3, ANUDEM, GRASS Complex polynomial
        /// 7=Electronic imaging (non-photogrametric), active or passive, sensor systems.
        /// </summary>
        public char process_code;

        /// <summary>
        /// Free format Mapping Origin Code. Example: MAC, WMC, MCMC, RMMC, FS, BLM, CONT (contractor), XX (state postal code).
        /// </summary>
        public char[] origin_code = new char[4];

        /// <summary>
        /// Code 1=DEM-1
        /// 2=DEM-2
        /// 3=DEM-3
        /// 4=DEM-4
        /// </summary>
        public int dem_level_code;

        /// <summary>
        /// 1 = regular
        /// 2 = random
        /// </summary>
        public int elevation_pattern;
        public int ground_ref_system;
        public int ground_ref_zone;
        public double[] projection = new double[15];
        public int ground_unit;
        public int elevation_unit;
        public int side_count;
        // pairs of easting-northings
        public float[] sw_coord = new float[2];
        public float[] nw_coord = new float[2];
        public float[] ne_coord = new float[2];
        public float[] se_coord = new float[2];
        public double ccw_angle;
        public double elevation_min;
        public double elevation_max;
        public int elevation_accuracy;
        public float[] xyz_resolution = new float[3];
        public int northings_rows;
        public int eastings_cols;
        public int suspect_void;
        public int percent_void;
    }

    public class BRecord
    {
        public short[,] elevations;

        public BRecord(int cols, int rows)
        {
            elevations = new short[cols, rows];
        }
    }
}
