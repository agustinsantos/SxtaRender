using Sxta.Math;
using Sxta.Render;
using System;
using System.IO;
using System.Threading;

namespace Sxta.Proland.Atmo
{
    public partial class PreprocessAtmo
    {
        public AtmoParameters params_;
        public string output;
        public Texture2D transmittanceT;
        public Texture2D irradianceT;
        public Texture3D inscatterT;
        public Texture2D deltaET;
        public Texture3D deltaSRT;
        public Texture3D deltaSMT;
        public Texture3D deltaJT;
        public Program copyInscatter1;
        public Program copyInscatterN;
        public Program copyIrradiance;
        public Program inscatter1;
        public Program inscatterN;
        public Program inscatterS;
        public Program irradiance1;
        public Program irradianceN;
        public Program transmittance;
        public FrameBuffer fbo;
        public int order;

        public PreprocessAtmo(AtmoParameters params_, string output)
        {
            this.params_ = params_;
            this.output = output;
        }

        private void Init()
        {
            transmittanceT = new Texture2D(params_.TRANSMITTANCE_W, params_.TRANSMITTANCE_H, TextureInternalFormat.RGB16F,
               TextureFormat.RGB, PixelType.FLOAT, new Texture.Parameters().min(TextureFilter.LINEAR).mag(TextureFilter.LINEAR), new Render.Buffer.Parameters(), new CPUBuffer<byte>());
            irradianceT = new Texture2D(params_.SKY_W, params_.SKY_H, TextureInternalFormat.RGB16F,
                TextureFormat.RGB, PixelType.FLOAT, new Texture.Parameters().min(TextureFilter.LINEAR).mag(TextureFilter.LINEAR), new Render.Buffer.Parameters(), new CPUBuffer<byte>());
            inscatterT = new Texture3D(params_.RES_MU_S * params_.RES_NU, params_.RES_MU, params_.RES_R, TextureInternalFormat.RGBA16F,
                TextureFormat.RGBA, PixelType.FLOAT, new Texture.Parameters().min(TextureFilter.LINEAR).mag(TextureFilter.LINEAR), new Render.Buffer.Parameters(), new CPUBuffer<byte>());
            deltaET = new Texture2D(params_.SKY_W, params_.SKY_H, TextureInternalFormat.RGB16F,
                TextureFormat.RGB, PixelType.FLOAT, new Texture.Parameters().min(TextureFilter.LINEAR).mag(TextureFilter.LINEAR), new Render.Buffer.Parameters(), new CPUBuffer<byte>());
            deltaSRT = new Texture3D(params_.RES_MU_S * params_.RES_NU, params_.RES_MU, params_.RES_R, TextureInternalFormat.RGB16F,
                TextureFormat.RGB, PixelType.FLOAT, new Texture.Parameters().min(TextureFilter.LINEAR).mag(TextureFilter.LINEAR), new Render.Buffer.Parameters(), new CPUBuffer<byte>());
            deltaSMT = new Texture3D(params_.RES_MU_S * params_.RES_NU, params_.RES_MU, params_.RES_R, TextureInternalFormat.RGB16F,
                TextureFormat.RGB, PixelType.FLOAT, new Texture.Parameters().min(TextureFilter.LINEAR).mag(TextureFilter.LINEAR), new Render.Buffer.Parameters(), new CPUBuffer<byte>());
            deltaJT = new Texture3D(params_.RES_MU_S * params_.RES_NU, params_.RES_MU, params_.RES_R, TextureInternalFormat.RGB16F,
                TextureFormat.RGB, PixelType.FLOAT, new Texture.Parameters().min(TextureFilter.LINEAR).mag(TextureFilter.LINEAR), new Render.Buffer.Parameters(), new CPUBuffer<byte>());

            copyInscatter1 = new Program(new Module(330, constantsAtmoShader + copyInscatter1Shader));
            copyInscatterN = new Program(new Module(330, constantsAtmoShader + commonAtmoShader + copyInscatterNShader));
            copyIrradiance = new Program(new Module(330, constantsAtmoShader + copyIrradianceShader));
            inscatter1 = new Program(new Module(330, constantsAtmoShader + commonAtmoShader + inscatter1Shader));
            inscatterN = new Program(new Module(330, constantsAtmoShader + commonAtmoShader + inscatterNShader));
            inscatterS = new Program(new Module(330, constantsAtmoShader + commonAtmoShader + inscatterSShader));
            irradiance1 = new Program(new Module(330, constantsAtmoShader + commonAtmoShader + irradiance1Shader));
            irradianceN = new Program(new Module(330, constantsAtmoShader + commonAtmoShader + irradianceNShader));
            transmittance = new Program(new Module(330, constantsAtmoShader + commonAtmoShader + transmittanceShader));

            SetParameters(copyInscatter1);
            SetParameters(copyInscatterN);
            SetParameters(copyIrradiance);
            SetParameters(inscatter1);
            SetParameters(inscatterN);
            SetParameters(inscatterS);
            SetParameters(irradiance1);
            SetParameters(irradianceN);
            SetParameters(transmittance);

            copyInscatter1.getUniformSampler("deltaSRSampler").set(deltaSRT);
            copyInscatter1.getUniformSampler("deltaSMSampler").set(deltaSMT);
            copyInscatterN.getUniformSampler("deltaSSampler").set(deltaSRT);
            copyIrradiance.getUniformSampler("deltaESampler").set(deltaET);
            inscatter1.getUniformSampler("transmittanceSampler").set(transmittanceT);
            inscatterN.getUniformSampler("transmittanceSampler").set(transmittanceT);
            inscatterN.getUniformSampler("deltaJSampler").set(deltaJT);
            inscatterS.getUniformSampler("transmittanceSampler").set(transmittanceT);
            inscatterS.getUniformSampler("deltaESampler").set(deltaET);
            inscatterS.getUniformSampler("deltaSRSampler").set(deltaSRT);
            inscatterS.getUniformSampler("deltaSMSampler").set(deltaSMT);
            irradiance1.getUniformSampler("transmittanceSampler").set(transmittanceT);
            irradianceN.getUniformSampler("deltaSRSampler").set(deltaSRT);
            irradianceN.getUniformSampler("deltaSMSampler").set(deltaSMT);

            fbo = new FrameBuffer();
            fbo.setReadBuffer(BufferId.COLOR0);
            fbo.setDrawBuffer(BufferId.COLOR0);

            order = 2;
        }

        private void Finish()
        {
            copyInscatter1.Dispose();
            copyInscatterN.Dispose();
            copyIrradiance.Dispose();
            inscatter1.Dispose();
            inscatterN.Dispose();
            inscatterS.Dispose();
            irradiance1.Dispose();
            irradianceN.Dispose();
            transmittance.Dispose();

            transmittanceT.Dispose();
            irradianceT.Dispose();
            inscatterT.Dispose();
            deltaET.Dispose();
            deltaSRT.Dispose();
            deltaSMT.Dispose();
            deltaJT.Dispose();

            fbo.Dispose();
        }

        private void SetParameters(Program p)
        {
            if (p.getUniform1f("Rg") != null)
            {
                p.getUniform1f("Rg").set(params_.Rg);
            }
            if (p.getUniform1f("Rt") != null)
            {
                p.getUniform1f("Rt").set(params_.Rt);
            }
            if (p.getUniform1f("RL") != null)
            {
                p.getUniform1f("RL").set(params_.RL);
            }
            if (p.getUniform1i("TRANSMITTANCE_W") != null)
            {
                p.getUniform1i("TRANSMITTANCE_W").set(params_.TRANSMITTANCE_W);
            }
            if (p.getUniform1i("TRANSMITTANCE_H") != null)
            {
                p.getUniform1i("TRANSMITTANCE_H").set(params_.TRANSMITTANCE_H);
            }
            if (p.getUniform1i("SKY_W") != null)
            {
                p.getUniform1i("SKY_W").set(params_.SKY_W);
            }
            if (p.getUniform1i("SKY_H") != null)
            {
                p.getUniform1i("SKY_H").set(params_.SKY_H);
            }
            if (p.getUniform1i("RES_R") != null)
            {
                p.getUniform1i("RES_R").set(params_.RES_R);
            }
            if (p.getUniform1i("RES_MU") != null)
            {
                p.getUniform1i("RES_MU").set(params_.RES_MU);
            }
            if (p.getUniform1i("RES_MU_S") != null)
            {
                p.getUniform1i("RES_MU_S").set(params_.RES_MU_S);
            }
            if (p.getUniform1i("RES_NU") != null)
            {
                p.getUniform1i("RES_NU").set(params_.RES_NU);
            }
            if (p.getUniform1f("AVERAGE_GROUND_REFLECTANCE") != null)
            {
                p.getUniform1f("AVERAGE_GROUND_REFLECTANCE").set(params_.AVERAGE_GROUND_REFLECTANCE);
            }
            if (p.getUniform1f("HR") != null)
            {
                p.getUniform1f("HR").set(params_.HR);
            }
            if (p.getUniform3f("betaR") != null)
            {
                p.getUniform3f("betaR").set(params_.betaR);
            }
            if (p.getUniform1f("HM") != null)
            {
                p.getUniform1f("HM").set(params_.HM);
            }
            if (p.getUniform3f("betaMSca") != null)
            {
                p.getUniform3f("betaMSca").set(params_.betaMSca);
            }
            if (p.getUniform3f("betaMEx") != null)
            {
                p.getUniform3f("betaMEx").set(params_.betaMEx);
            }
            if (p.getUniform1f("mieG") != null)
            {
                p.getUniform1f("mieG").set(params_.mieG);
            }
        }

        private void SetLayer(Program p, int layer)
        {
            double r = layer / (params_.RES_R - 1.0);
            r = r * r;
            r = System.Math.Sqrt(params_.Rg * params_.Rg + r * (params_.Rt * params_.Rt - params_.Rg * params_.Rg)) + (layer == 0 ? 0.01 : (layer == params_.RES_R - 1 ? -0.001 : 0.0));
            double dmin = params_.Rt - r;
            double dmax = System.Math.Sqrt(r * r - params_.Rg * params_.Rg) + System.Math.Sqrt(params_.Rt * params_.Rt - params_.Rg * params_.Rg);
            double dminp = r - params_.Rg;
            double dmaxp = System.Math.Sqrt(r * r - params_.Rg * params_.Rg);
            if (p.getUniform1f("r") != null)
            {
                p.getUniform1f("r").set((float)r);
            }
            if (p.getUniform4f("dhdH") != null)
            {
                p.getUniform4f("dhdH").set(new Vector4f((float)dmin, (float)dmax, (float)dminp, (float)dmaxp));
            }
            p.getUniform1i("layer").set(layer);
        }

        private void Preprocess(int step)
        {
            switch (step)
            {
                case 0:
                    Init();
                    // computes transmittance texture T (line 1 in algorithm 4.1)
                    fbo.setTextureBuffer(BufferId.COLOR0, transmittanceT, 0);
                    fbo.setViewport(new Vector4i(0, 0, params_.TRANSMITTANCE_W, params_.TRANSMITTANCE_H));
                    fbo.drawQuad(transmittance);
                    break;
                case 1:
                    // computes irradiance texture deltaE (line 2 in algorithm 4.1)
                    fbo.setTextureBuffer(BufferId.COLOR0, deltaET, 0);
                    fbo.setViewport(new Vector4i(0, 0, params_.SKY_W, params_.SKY_H));
                    fbo.drawQuad(irradiance1);
                    break;
                case 2:
                    // computes single scattering texture deltaS (line 3 in algorithm 4.1)
                    // Rayleigh and Mie separated in deltaSR + deltaSM
                    fbo.setTextureBuffer(BufferId.COLOR0, deltaSRT, 0, -1); //TODO Here I have a OpenGL Error (?)
                    fbo.setTextureBuffer(BufferId.COLOR1, deltaSMT, 0, -1);
                    fbo.setDrawBuffers((BufferId)(BufferId.COLOR0 | BufferId.COLOR1));
                    fbo.setViewport(new Vector4i(0, 0, params_.RES_MU_S * params_.RES_NU, params_.RES_MU));
                    for (int layer = 0; layer < params_.RES_R; ++layer)
                    {
                        SetLayer(inscatter1, layer);
                        fbo.drawQuad(inscatter1);
                    }
                    fbo.setTextureBuffer(BufferId.COLOR1, (Texture2D)null, 0);
                    fbo.setDrawBuffer(BufferId.COLOR0);
                    break;
                case 3:
                    // copies deltaE into irradiance texture E (line 4 in algorithm 4.1)
                    fbo.setTextureBuffer(BufferId.COLOR0, irradianceT, 0);
                    fbo.setViewport(new Vector4i(0, 0, params_.SKY_W, params_.SKY_H));
                    copyIrradiance.getUniform1f("k").set(0.0f);
                    fbo.drawQuad(copyIrradiance);
                    break;
                case 4:
                    // copies deltaS into inscatter texture S (line 5 in algorithm 4.1)
                    fbo.setTextureBuffer(BufferId.COLOR0, inscatterT, 0, -1);
                    fbo.setViewport(new Vector4i(0, 0, params_.RES_MU_S * params_.RES_NU, params_.RES_MU));
                    for (int layer = 0; layer < params_.RES_R; ++layer)
                    {
                        SetLayer(copyInscatter1, layer);
                        fbo.drawQuad(copyInscatter1);
                    }
                    break;
                case 5:
                    // computes deltaJ (line 7 in algorithm 4.1)
                    fbo.setTextureBuffer(BufferId.COLOR0, deltaJT, 0, -1);
                    fbo.setViewport(new Vector4i(0, 0, params_.RES_MU_S * params_.RES_NU, params_.RES_MU));
                    inscatterS.getUniform1f("first").set(order == 2 ? 1.0f : 0.0f);
                    for (int layer = 0; layer < params_.RES_R; ++layer)
                    {
                        SetLayer(inscatterS, layer);
                        fbo.drawQuad(inscatterS);
                    }
                    break;
                case 6:
                    // computes deltaE (line 8 in algorithm 4.1)
                    fbo.setTextureBuffer(BufferId.COLOR0, deltaET, 0);
                    fbo.setViewport(new Vector4i(0, 0, params_.SKY_W, params_.SKY_H));
                    irradianceN.getUniform1f("first").set(order == 2 ? 1.0f : 0.0f);
                    fbo.drawQuad(irradianceN);
                    break;
                case 7:
                    // computes deltaS (line 9 in algorithm 4.1)
                    fbo.setTextureBuffer(BufferId.COLOR0, deltaSRT, 0, -1);
                    fbo.setViewport(new Vector4i(0, 0, params_.RES_MU_S * params_.RES_NU, params_.RES_MU));
                    for (int layer = 0; layer < params_.RES_R; ++layer)
                    {
                        SetLayer(inscatterN, layer);
                        fbo.drawQuad(inscatterN);
                    }
                    break;
                case 8:
                    fbo.setBlend(true, BlendEquation.ADD, BlendArgument.ONE, BlendArgument.ONE);
                    // adds deltaE into irradiance texture E (line 10 in algorithm 4.1)
                    fbo.setTextureBuffer(BufferId.COLOR0, irradianceT, 0);
                    fbo.setViewport(new Vector4i(0, 0, params_.SKY_W, params_.SKY_H));
                    copyIrradiance.getUniform1f("k").set(1.0f);
                    fbo.drawQuad(copyIrradiance);
                    break;
                case 9:
                    // adds deltaS into inscatter texture S (line 11 in algorithm 4.1)
                    fbo.setTextureBuffer(BufferId.COLOR0, inscatterT, 0, -1);
                    fbo.setViewport(new Vector4i(0, 0, params_.RES_MU_S * params_.RES_NU, params_.RES_MU));
                    for (int layer = 0; layer < params_.RES_R; ++layer)
                    {
                        SetLayer(copyInscatterN, layer);
                        fbo.drawQuad(copyInscatterN);
                    }
                    fbo.setBlend(false);
                    if (order < 4)
                    {
                        step = 4;
                        order += 1;
                    }
                    break;
                case 10:
                    {
                        FrameBuffer.Finish();

                        uint[] trailerTrans = new uint[5];
                        // A raw file must end with five 32 bits integers, the first one being 0xCAFEBABE, the others the width, height, depth and components per pixel.
                        trailerTrans[0] = 0xCAFEBABE;
                        trailerTrans[1] = (uint)params_.TRANSMITTANCE_W; // width
                        trailerTrans[2] = (uint)params_.TRANSMITTANCE_H; // height
                        trailerTrans[3] = 0; // depth 
                        trailerTrans[4] = 3;  // components per pixel
                        byte[] bufTrans = new byte[3 * params_.TRANSMITTANCE_W * params_.TRANSMITTANCE_H * sizeof(float)];
                        transmittanceT.getImage(0, TextureFormat.RGB, PixelType.FLOAT, bufTrans);
                        string transmittanceFile = Path.Combine(output, "transmittance.raw");
                        using (FileStream fTrans = new FileStream(transmittanceFile, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                        {
                            fTrans.Write(bufTrans, 0, bufTrans.Length);
                            byte[] trailerBuf = new byte[trailerTrans.Length * sizeof(int)];
                            System.Buffer.BlockCopy(trailerTrans, 0, trailerBuf, 0, trailerBuf.Length);
                            fTrans.Write(trailerBuf, 0, 5 * sizeof(int));
                            fTrans.Close();
                        }
                        bufTrans = null;
                    }
                    break;
                case 11:
                    uint[] trailerIrrad = new uint[5];
                    trailerIrrad[0] = 0xCAFEBABE;
                    trailerIrrad[1] = (uint)params_.SKY_W;
                    trailerIrrad[2] = (uint)params_.SKY_H;
                    trailerIrrad[3] = 0;
                    trailerIrrad[4] = 3;
                    byte[] bufIrrad = new byte[3 * params_.SKY_W * params_.SKY_H * sizeof(float)];
                    irradianceT.getImage(0, TextureFormat.RGB, PixelType.FLOAT, bufIrrad);
                    string irradianceFile = Path.Combine(output, "irradiance.raw");
                    using (FileStream fIrrad = new FileStream(irradianceFile, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                    {
                        fIrrad.Write(bufIrrad, 0, bufIrrad.Length);
                        byte[] trailerBuf = new byte[trailerIrrad.Length * sizeof(int)];
                        System.Buffer.BlockCopy(trailerIrrad, 0, trailerBuf, 0, trailerBuf.Length);
                        fIrrad.Write(trailerBuf, 0, 5 * sizeof(int));
                        fIrrad.Close();
                    }
                    bufIrrad = null;
                    break;
                case 12:
                    uint[] trailerInsc = new uint[5];
                    trailerInsc[0] = 0xCAFEBABE;
                    trailerInsc[1] = (uint)(params_.RES_MU_S * params_.RES_NU);
                    trailerInsc[2] = (uint)(params_.RES_MU * params_.RES_R);
                    trailerInsc[3] = (uint)params_.RES_R;
                    trailerInsc[4] = 4;
                    byte[] bufInsc = new byte[4 * params_.RES_MU_S * params_.RES_NU * params_.RES_MU * params_.RES_R * sizeof(float)];
                    inscatterT.getImage(0, TextureFormat.RGBA, PixelType.FLOAT, bufInsc);
                    string inscatterFile = Path.Combine(output, "inscatter.raw");
                    using (FileStream fInsc = new FileStream(inscatterFile, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                    {
                        fInsc.Write(bufInsc, 0, bufInsc.Length);
                        byte[] trailerBuf = new byte[trailerInsc.Length * sizeof(int)];
                        System.Buffer.BlockCopy(trailerInsc, 0, trailerBuf, 0, trailerBuf.Length);
                        fInsc.Write(trailerBuf, 0, 5 * sizeof(int));
                        fInsc.Close();
                    }
                    bufIrrad = null;
                    break;
                case 13:
                    Finish();
                    break;
                default:
                    ///PRECOMPUTATONS DONE. RESTART APPLICATION.
                    throw new ArgumentException("Only steps 0 to 13 are allowed.");
            }
        }

        /// <summary>
        /// Precomputes the tables for the given atmosphere parameters.
        /// </summary>
        /// <param name="params_">the atmosphere parameters.</param>
        /// <param name="outputDir">the folder where to write the generated tables.</param>
        public static void PreprocessAtmosphereParameters(AtmoParameters params_, string outputDir)
        {
            if (File.Exists(Path.Combine(outputDir, "inscatter.raw")))
                return;
            PreprocessAtmo preprocessAtmo = new PreprocessAtmo(params_, outputDir);
            for (int step = 0; step < 14; step++)
            {
                preprocessAtmo.Preprocess(step);
            }
        }
    }
}
