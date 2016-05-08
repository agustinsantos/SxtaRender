using Sxta.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Proland.Atmo
{
    public class AtmoParameters
    {
        public float Rg = 6360.0f;
        public float Rt = 6420.0f;
        public float RL = 6421.0f;
        public int TRANSMITTANCE_W = 256;
        public int TRANSMITTANCE_H = 64;
        public int SKY_W = 64;
        public int SKY_H = 16;
        public int RES_R = 32;
        public int RES_MU = 128;
        public int RES_MU_S = 32;
        public int RES_NU = 8;
        public float AVERAGE_GROUND_REFLECTANCE = 0.1f;
        public float HR = 8.0f;
        public Vector3f betaR = new Vector3f(5.8e-3f, 1.35e-2f, 3.31e-2f);
        public float HM = 1.2f;
        public Vector3f betaMSca = new Vector3f(4e-3f, 4e-3f, 4e-3f);
        public Vector3f betaMEx = new Vector3f(4.44e-3f, 4.44e-3f, 4.44e-3f);
        public float mieG = 0.8f;
    }
}
