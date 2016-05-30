/*
 * Proland: a procedural landscape rendering library.
 * Website : http://proland.inrialpes.fr/
 * Copyright (c) 2008-2015 INRIA - LJK (CNRS - Grenoble University)
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 * 
 * 2. Redistributions in binary form must reproduce the above copyright notice, 
 * this list of conditions and the following disclaimer in the documentation 
 * and/or other materials provided with the distribution.
 * 
 * 3. Neither the name of the copyright holder nor the names of its contributors 
 * may be used to endorse or promote products derived from this software without 
 * specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE 
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 */
/*
 * Proland is distributed under the Berkeley Software Distribution 3 Licence. 
 * For any assistance, feedback and enquiries about training programs, you can check out the 
 * contact page on our website : 
 * http://proland.inrialpes.fr/
 */
/*
 * Main authors: Eric Bruneton, Antoine Begault, Guillaume Piolat.
* Modified and ported to C# and Sxta Engine by Agustin Santos and Daniel Olmedo 2015-2016
*/

using System.IO;
using System;
using System.Diagnostics;

namespace Sxta.Proland.Terrain
{
    public delegate void ProjectionFunction(int x, int y, int w, out double sx, out double sy, out double sz);
    public delegate float ColorTransformDelegate(float p);


    public class Preprocess
    {
        public enum MODE { HEIGHT, COLOR };

        private void CreateDir(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        /* 
 		* Preprocess an elevation map into a file that can be used with a Proland.ResidualProducer.
 		* 
 		* param src the map to be preprocessed. Only the x channel is used.
 		* param dstMinTileSize the size of the root tile (without borders). E.g., a size
 		*     of 24 corresponds to a full size of 24+5=29.
 		* param dstTileSize the maximum size of the tiles (without borders). E.g., a size
 		*     of 192 corresponds to a full size of 192+5=197. dstTileSize must be a
 		*     multiple of dstMinTileSize.
 		* param dstMaxLevel the maximum quadtree level to compute. This level is computed
 		*     from the first tile having the maximum size. E.g., if dstMinTileSize is 24,
 		*     dstTileSize is 192, and maxLevel is 2, this means that the last computed
 		*     level will be 5 (level 0 has size 24, level 1 48, level 2 96, level 3 192,
 		*     level 4 2*192 and level 5 4*192).
 		* param dstFolder where the precomputed file must be saved.
 		* param tmpFolder where temporary files must be saved.
 		*/
        public void PreprocessDem(InputMap src, int dstMinTileSize, int dstTileSize, int dstMaxLevel,
                                  string dstFolder, string tmpFolder, float residualScale)
        {
            Debug.Assert(dstTileSize % dstMinTileSize == 0);

            //if (dstTileSize % dstMinTileSize != 0)
            //    throw new ArgumentException("dstTileSize must be a multiple of dstMinTileSize");

            CreateDir(tmpFolder);
            CreateDir(dstFolder);
            int dstSize = dstTileSize << dstMaxLevel;
            HeightFunction hf = new PlaneHeightFunction(src, dstSize);
            HeightMipmap hm = new HeightMipmap(hf, dstMinTileSize, dstSize, dstTileSize, residualScale, tmpFolder);
            hm.Compute1();
            while (true)
            {
                if (!hm.Compute2())
                {
                    break;
                }
            }
            hm.Generate(0, 0, 0, residualScale, dstFolder + "/DEM.dat");

        }

        /* 
         * Preprocess a spherical elevation map into six files that can be used with six
         * proland::ResidualProducer to form a planet.
         * @ingroup preprocess
         * @author Eric Bruneton
         *
         * @param src the spherical map to be preprocessed. The x coordinate corresponds to
         *     longitudes, and the y coordinate to latitudes (i.e. using the
         *     equirectangular projection, aka the equidirectional projection,
         *     equidistant cylindrical projection, geographic projection, or plate
         *     carrée projection). Only the x channel is used.
         * @param dstMinTileSize the size of the root tile (without borders). E.g., a size
         *     of 24 corresponds to a full size of 24+5=29.
         * @param dstTileSize the maximum size of the tiles (without borders). E.g., a size
         *     of 192 corresponds to a full size of 192+5=197. dstTileSize must be a
         *     multiple of dstMinTileSize.
         * @param dstMaxLevel the maximum quadtree level to compute. This level is computed
         *     from the first tile having the maximum size. E.g., if dstMinTileSize is 24,
         *     dstTileSize is 192, and maxLevel is 2, this means that the last computed
         *     level will be 5 (level 0 has size 24, level 1 48, level 2 96, level 3 192,
         *     level 4 2*192 and level 5 4*192).
         * @param dstFolder where the precomputed file must be saved.
         * @param tmpFolder where temporary files must be saved.
         * @param residualScale the scale factor to use to quantify the residuals in
         *     short values. Residuals are divided by this factor before beeing quantified.
         *     A small value gives better precision, but can lead to overflows. If you get
         *     overflows during the precomputations (i.e. if the maximum residual, indicated
         *     in the standard ouput is larger than 65535), retry with a larger value.
         */
        public void preprocessSphericalDem(InputMap src, int dstMinTileSize, int dstTileSize, int dstMaxLevel,
                                          string dstFolder, string tmpFolder, float residualScale)
        {
            Debug.Assert(dstTileSize % dstMinTileSize == 0);
            if (File.Exists(dstFolder + "/DEM1.dat") && File.Exists(dstFolder + "/DEM2.dat") && File.Exists(dstFolder + "/DEM3.dat") &&
               File.Exists(dstFolder + "/DEM4.dat") && File.Exists(dstFolder + "/DEM5.dat") && File.Exists(dstFolder + "/DEM6.dat"))
            {
                return;
            }
            CreateDir(tmpFolder + "1");
            CreateDir(tmpFolder + "2");
            CreateDir(tmpFolder + "3");
            CreateDir(tmpFolder + "4");
            CreateDir(tmpFolder + "5");
            CreateDir(tmpFolder + "6");
            CreateDir(dstFolder);
            int dstSize = dstTileSize << dstMaxLevel;
            HeightFunction hf1 = new SphericalHeightFunction(src, projection1, dstSize);
            HeightFunction hf2 = new SphericalHeightFunction(src, projection2, dstSize);
            HeightFunction hf3 = new SphericalHeightFunction(src, projection3, dstSize);
            HeightFunction hf4 = new SphericalHeightFunction(src, projection4, dstSize);
            HeightFunction hf5 = new SphericalHeightFunction(src, projection5, dstSize);
            HeightFunction hf6 = new SphericalHeightFunction(src, projection6, dstSize);
            HeightMipmap hm1 = new HeightMipmap(hf1, dstMinTileSize, dstSize, dstTileSize, residualScale, tmpFolder + "1");
            HeightMipmap hm2 = new HeightMipmap(hf2, dstMinTileSize, dstSize, dstTileSize, residualScale, tmpFolder + "2");
            HeightMipmap hm3 = new HeightMipmap(hf3, dstMinTileSize, dstSize, dstTileSize, residualScale, tmpFolder + "3");
            HeightMipmap hm4 = new HeightMipmap(hf4, dstMinTileSize, dstSize, dstTileSize, residualScale, tmpFolder + "4");
            HeightMipmap hm5 = new HeightMipmap(hf5, dstMinTileSize, dstSize, dstTileSize, residualScale, tmpFolder + "5");
            HeightMipmap hm6 = new HeightMipmap(hf6, dstMinTileSize, dstSize, dstTileSize, residualScale, tmpFolder + "6");
            HeightMipmap.setCube(hm1, hm2, hm3, hm4, hm5, hm6);
            hm1.Compute1();
            hm2.Compute1();
            hm3.Compute1();
            hm4.Compute1();
            hm5.Compute1();
            hm6.Compute1();
            while (true)
            {
                hm1.Compute2();
                hm2.Compute2();
                hm3.Compute2();
                hm4.Compute2();
                hm5.Compute2();
                if (!hm6.Compute2())
                {
                    break;
                }
            }
            hm1.Generate(0, 0, 0, residualScale, dstFolder + "/DEM1.dat");
            hm2.Generate(0, 0, 0, residualScale, dstFolder + "/DEM2.dat");
            hm3.Generate(0, 0, 0, residualScale, dstFolder + "/DEM3.dat");
            hm4.Generate(0, 0, 0, residualScale, dstFolder + "/DEM4.dat");
            hm5.Generate(0, 0, 0, residualScale, dstFolder + "/DEM5.dat");
            hm6.Generate(0, 0, 0, residualScale, dstFolder + "/DEM6.dat");
        }

        /* 
         * Preprocess a spherical elevation map into six files that can be used with six
         * proland::OrthoCPUProducer to compute terrain shadows with "Ambient Aperture
         * Lighting", Christopher Oat, Pedro V. Sander, I3D 2007. <b>WARNING</b> the
         * current implementation of this method only works when 'maxLevel' is such that
         * a tile at this level can be "reasonably" considered as "flat". In practice,
         * maxLevel should be at least 10.
         * @ingroup preprocess
         * @author Eric Bruneton
         *
         * @param src the spherical map to be preprocessed. The x coordinate corresponds to
         *     longitudes, and the y coordinate to latitudes (i.e. using the
         *     equirectangular projection, aka the equidirectional projection,
         *     equidistant cylindrical projection, geographic projection, or plate
         *     carrée projection). Only the x channel is used.
         * @param srcFolder where the preprocessed elevation map files are stored (these
         *     files are supposed to have been Generated with #preprocessSphericalDem).
         * @param minLevel the minimum quadtree level used to sample visibility around
         *     a point. This level is computed from the first tile having the maximum
         *     size (see proland::preprocessSphericalDem).
         * @param maxLevel the maximum quadtree level use to sample visibility around a
         *     point. This is also the maximum quadtree level of the produced quadtree.
         *     This level is computed from the first tile having the maximum size (see
         *     proland::preprocessSphericalDem).
         * @param samples the number of samples per direction and per level, used to
         *     sample visibility around a point.
         * @param dstFolder where the precomputed file must be saved.
         * @param tmpFolder where temporary files must be saved.
         */
        public void preprocessSphericalAperture(string srcFolder, int minLevel, int maxLevel, int samples,
                                                string dstFolder, string tmpFolder)
        {
            if (File.Exists(dstFolder + "/APERTURE1.dat") && File.Exists(dstFolder + "/APERTURE2.dat") && File.Exists(dstFolder + "/APERTURE3.dat") &&
                File.Exists(dstFolder + "/APERTURE4.dat") && File.Exists(dstFolder + "/APERTURE5.dat") && File.Exists(dstFolder + "/APERTURE6.dat"))
            {
                return;
            }
            CreateDir(tmpFolder + "1");
            CreateDir(tmpFolder + "2");
            CreateDir(tmpFolder + "3");
            CreateDir(tmpFolder + "4");
            CreateDir(tmpFolder + "5");
            CreateDir(tmpFolder + "6");
            CreateDir(dstFolder);
            DemTileCache r1 = new DemTileCache(srcFolder + "/DEM1.dat", 60);
            DemTileCache r2 = new DemTileCache(srcFolder + "/DEM2.dat", 60);
            DemTileCache r3 = new DemTileCache(srcFolder + "/DEM3.dat", 60);
            DemTileCache r4 = new DemTileCache(srcFolder + "/DEM4.dat", 60);
            DemTileCache r5 = new DemTileCache(srcFolder + "/DEM5.dat", 60);
            DemTileCache r6 = new DemTileCache(srcFolder + "/DEM6.dat", 60);
            ElevationTileCache z1 = new ElevationTileCache(r1, 40);
            ElevationTileCache z2 = new ElevationTileCache(r2, 40);
            ElevationTileCache z3 = new ElevationTileCache(r3, 40);
            ElevationTileCache z4 = new ElevationTileCache(r4, 40);
            ElevationTileCache z5 = new ElevationTileCache(r5, 40);
            ElevationTileCache z6 = new ElevationTileCache(r6, 40);
            ElevationTileCache[] faces = new ElevationTileCache[6];
            faces[0] = z1;
            faces[1] = z2;
            faces[2] = z3;
            faces[3] = z4;
            faces[4] = z5;
            faces[5] = z6;
            PlanetElevationTileCache pz = new PlanetElevationTileCache(faces, maxLevel);
            ApertureMipmap a1 = new ApertureMipmap(pz, projection1f, 1.0, maxLevel, minLevel, samples);
            a1.build(tmpFolder + "1");
            a1.Generate(tmpFolder + "1", dstFolder + "/APERTURE1.dat");
            ApertureMipmap a2 = new ApertureMipmap(pz, projection2f, 1.0, maxLevel, minLevel, samples);
            a2.build(tmpFolder + "2");
            a2.Generate(tmpFolder + "2", dstFolder + "/APERTURE2.dat");
            ApertureMipmap a3 = new ApertureMipmap(pz, projection3f, 1.0, maxLevel, minLevel, samples);
            a3.build(tmpFolder + "3");
            a3.Generate(tmpFolder + "3", dstFolder + "/APERTURE3.dat");
            ApertureMipmap a4 = new ApertureMipmap(pz, projection4f, 1.0, maxLevel, minLevel, samples);
            a4.build(tmpFolder + "4");
            a4.Generate(tmpFolder + "4", dstFolder + "/APERTURE4.dat");
            ApertureMipmap a5 = new ApertureMipmap(pz, projection5f, 1.0, maxLevel, minLevel, samples);
            a5.build(tmpFolder + "5");
            a5.Generate(tmpFolder + "5", dstFolder + "/APERTURE5.dat");
            ApertureMipmap a6 = new ApertureMipmap(pz, projection6f, 1.0, maxLevel, minLevel, samples);
            a6.build(tmpFolder + "6");
            a6.Generate(tmpFolder + "6", dstFolder + "/APERTURE6.dat");
        }

        /* 
         * Preprocess a map into files that can be used with a proland.OrthoCPUProducer
         *
         * param src the map to be preprocessed.
         * param dstTileSize the size of the Generated tiles (without borders). E.g., a size
         *     of 192 corresponds to a full size of 192+4=196.
         * param dstChannels the number of components per pixel in the Generated files.
         * param dstMaxLevel the maximum quadtree level to compute.
         * param dstFolder where the precomputed file must be saved.
         * param tmpFolder where temporary files must be saved.
         */
        public void PreprocessOrtho(InputMap src, int dstTileSize, int dstChannels, int dstMaxLevel,
          string dstFolder, string tmpFolder, ColorTransformDelegate rgbToLinear = null, ColorTransformDelegate linearToRgb = null)
        {
            if (File.Exists(dstFolder + "/RGB.dat") && File.Exists(dstFolder + "/dxt/RGB.dat") && File.Exists(dstFolder + "/residuals/RGB.dat"))
            {
                return;
            }
            CreateDir(tmpFolder);
            CreateDir(dstFolder);
            CreateDir(dstFolder + "/dxt");
            CreateDir(dstFolder + "/residuals");
            int dstSize = dstTileSize << dstMaxLevel;
            ColorFunction cf = new PlaneColorFunction(src, dstSize);
            ColorMipmap cm = new ColorMipmap(cf, dstSize, dstTileSize, 2, dstChannels,
                rgbToLinear == null ? id : rgbToLinear, linearToRgb == null ? id : linearToRgb, tmpFolder);
            cm.Compute();
            cm.Generate(0, 0, 0, false, true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/RGB.dat");
            cm.Generate(0, 0, 0, true, true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/dxt/RGB.dat");
            cm.GenerateResiduals(true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/RGB.dat", tmpFolder + "/residuals/RGB.dat");
            cm.ReorderResiduals(tmpFolder + "/RGB.dat", dstFolder + "/residuals/RGB.dat");
        }


        /* 
         * Preprocess a spherical map into files that can be used with a
         * proland::OrthoCPUProducer (and either a proland::OrthoGPUProducer or a
         * proland::OrthoProducer).
         * @ingroup preprocess
         * @author Eric Bruneton
         *
         * @param src the spherical map to be preprocessed.
         * @param dstTileSize the size of the Generated tiles (without borders). E.g., a size
         *     of 192 corresponds to a full size of 192+4=196.
         * @param dstChannels the number of components per pixel in the Generated files.
         * @param dstMaxLevel the maximum quadtree level to compute.
         * @param dstFolder where the precomputed file must be saved.
         * @param tmpFolder where temporary files must be saved.
         * @param rgbToLinear an optional transformation to transform the colors of the
         *     input map into a "linear" space where they can be safely interpolated.
         *     For instance, if the input map contains the square root of a reflectance (to
         *     get better precision for low values), this function should be the square
         *     function. A null value indicates the identity function.
         * @param linearToRgb an optional transformation, which must be the inverse of
         *     'rgbToLinear'. A null value indicates the identity function.
         */
        public void preprocessSphericalOrtho(InputMap src, int dstTileSize, int dstChannels, int dstMaxLevel,
          string dstFolder, string tmpFolder, ColorTransformDelegate rgbToLinear = null, ColorTransformDelegate linearToRgb = null)
        {
            if (File.Exists(dstFolder + "/RGB1.dat") && File.Exists(dstFolder + "/dxt/RGB1.dat") && File.Exists(dstFolder + "/residuals/RGB1.dat") &&
                File.Exists(dstFolder + "/RGB2.dat") && File.Exists(dstFolder + "/dxt/RGB2.dat") && File.Exists(dstFolder + "/residuals/RGB2.dat") &&
                File.Exists(dstFolder + "/RGB3.dat") && File.Exists(dstFolder + "/dxt/RGB3.dat") && File.Exists(dstFolder + "/residuals/RGB3.dat") &&
                File.Exists(dstFolder + "/RGB4.dat") && File.Exists(dstFolder + "/dxt/RGB4.dat") && File.Exists(dstFolder + "/residuals/RGB4.dat") &&
                File.Exists(dstFolder + "/RGB5.dat") && File.Exists(dstFolder + "/dxt/RGB5.dat") && File.Exists(dstFolder + "/residuals/RGB5.dat") &&
                File.Exists(dstFolder + "/RGB6.dat") && File.Exists(dstFolder + "/dxt/RGB6.dat") && File.Exists(dstFolder + "/residuals/RGB6.dat"))
            {
                return;
            }
            CreateDir(tmpFolder + "1");
            CreateDir(tmpFolder + "2");
            CreateDir(tmpFolder + "3");
            CreateDir(tmpFolder + "4");
            CreateDir(tmpFolder + "5");
            CreateDir(tmpFolder + "6");
            CreateDir(dstFolder);
            CreateDir(dstFolder + "/dxt");
            CreateDir(dstFolder + "/residuals");
            int dstSize = dstTileSize << dstMaxLevel;
            ColorFunction cf1 = new SphericalColorFunction(src, projection1h, dstSize);
            ColorFunction cf2 = new SphericalColorFunction(src, projection2h, dstSize);
            ColorFunction cf3 = new SphericalColorFunction(src, projection3h, dstSize);
            ColorFunction cf4 = new SphericalColorFunction(src, projection4h, dstSize);
            ColorFunction cf5 = new SphericalColorFunction(src, projection5h, dstSize);
            ColorFunction cf6 = new SphericalColorFunction(src, projection6h, dstSize);
            ColorMipmap cm1 = new ColorMipmap(cf1, dstSize, dstTileSize, 2, dstChannels,
                rgbToLinear == null ? id : rgbToLinear, linearToRgb == null ? id : linearToRgb, tmpFolder + "1");
            ColorMipmap cm2 = new ColorMipmap(cf2, dstSize, dstTileSize, 2, dstChannels,
                rgbToLinear == null ? id : rgbToLinear, linearToRgb == null ? id : linearToRgb, tmpFolder + "2");
            ColorMipmap cm3 = new ColorMipmap(cf3, dstSize, dstTileSize, 2, dstChannels,
                rgbToLinear == null ? id : rgbToLinear, linearToRgb == null ? id : linearToRgb, tmpFolder + "3");
            ColorMipmap cm4 = new ColorMipmap(cf4, dstSize, dstTileSize, 2, dstChannels,
                rgbToLinear == null ? id : rgbToLinear, linearToRgb == null ? id : linearToRgb, tmpFolder + "4");
            ColorMipmap cm5 = new ColorMipmap(cf5, dstSize, dstTileSize, 2, dstChannels,
                rgbToLinear == null ? id : rgbToLinear, linearToRgb == null ? id : linearToRgb, tmpFolder + "5");
            ColorMipmap cm6 = new ColorMipmap(cf6, dstSize, dstTileSize, 2, dstChannels,
                rgbToLinear == null ? id : rgbToLinear, linearToRgb == null ? id : linearToRgb, tmpFolder + "6");
            ColorMipmap.SetCube(cm1, cm2, cm3, cm4, cm5, cm6);
            cm1.Compute();
            cm2.Compute();
            cm3.Compute();
            cm4.Compute();
            cm5.Compute();
            cm6.Compute();
            cm1.Generate(0, 0, 0, false, true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/RGB1.dat");
            cm2.Generate(0, 0, 0, false, true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/RGB2.dat");
            cm3.Generate(0, 0, 0, false, true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/RGB3.dat");
            cm4.Generate(0, 0, 0, false, true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/RGB4.dat");
            cm5.Generate(0, 0, 0, false, true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/RGB5.dat");
            cm6.Generate(0, 0, 0, false, true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/RGB6.dat");
            cm1.Generate(0, 0, 0, true, true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/dxt/RGB1.dat");
            cm2.Generate(0, 0, 0, true, true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/dxt/RGB2.dat");
            cm3.Generate(0, 0, 0, true, true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/dxt/RGB3.dat");
            cm4.Generate(0, 0, 0, true, true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/dxt/RGB4.dat");
            cm5.Generate(0, 0, 0, true, true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/dxt/RGB5.dat");
            cm6.Generate(0, 0, 0, true, true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/dxt/RGB6.dat");
            cm1.GenerateResiduals(true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/RGB1.dat", tmpFolder + "1/RGB.dat");
            cm1.ReorderResiduals(tmpFolder + "1/RGB.dat", dstFolder + "/residuals/RGB1.dat");
            cm2.GenerateResiduals(true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/RGB2.dat", tmpFolder + "2/RGB.dat");
            cm2.ReorderResiduals(tmpFolder + "2/RGB.dat", dstFolder + "/residuals/RGB2.dat");
            cm3.GenerateResiduals(true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/RGB3.dat", tmpFolder + "3/RGB.dat");
            cm3.ReorderResiduals(tmpFolder + "3/RGB.dat", dstFolder + "/residuals/RGB3.dat");
            cm4.GenerateResiduals(true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/RGB4.dat", tmpFolder + "4/RGB.dat");
            cm4.ReorderResiduals(tmpFolder + "4/RGB.dat", dstFolder + "/residuals/RGB4.dat");
            cm5.GenerateResiduals(true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/RGB5.dat", tmpFolder + "5/RGB.dat");
            cm5.ReorderResiduals(tmpFolder + "5/RGB.dat", dstFolder + "/residuals/RGB5.dat");
            cm6.GenerateResiduals(true, ColorMipmap.RGB_JPEG_QUALITY, dstFolder + "/RGB6.dat", tmpFolder + "6/RGB.dat");
            cm6.ReorderResiduals(tmpFolder + "6/RGB.dat", dstFolder + "/residuals/RGB6.dat");
        }


        private void projection1(int x, int y, int w, out double sx, out double sy, out double sz) // north pole
        {
            double xl = (System.Math.Max(System.Math.Min(x, w), 0.0)) / w * 2.0 - 1.0;
            double yl = (System.Math.Max(System.Math.Min(y, w), 0.0)) / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = xl / l;
            sy = yl / l;
            sz = 1.0 / l;
        }

        private void projection2(int x, int y, int w, out double sx, out double sy, out double sz) // face 1
        {
            double xl = (System.Math.Max(System.Math.Min(x, w), 0.0)) / w * 2.0 - 1.0;
            double yl = (System.Math.Max(System.Math.Min(y, w), 0.0)) / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = xl / l;
            sy = -1.0 / l;
            sz = yl / l;
        }

        private void projection3(int x, int y, int w, out double sx, out double sy, out double sz) // face 2
        {
            double xl = (System.Math.Max(System.Math.Min(x, w), 0.0)) / w * 2.0 - 1.0;
            double yl = (System.Math.Max(System.Math.Min(y, w), 0.0)) / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = 1.0 / l;
            sy = xl / l;
            sz = yl / l;
        }

        private void projection4(int x, int y, int w, out double sx, out double sy, out double sz) // face 3
        {
            double xl = (System.Math.Max(System.Math.Min(x, w), 0.0)) / w * 2.0 - 1.0;
            double yl = (System.Math.Max(System.Math.Min(y, w), 0.0)) / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = -xl / l;
            sy = 1.0 / l;
            sz = yl / l;
        }

        private void projection5(int x, int y, int w, out double sx, out double sy, out double sz) // face 4
        {
            double xl = (System.Math.Max(System.Math.Min(x, w), 0.0)) / w * 2.0 - 1.0;
            double yl = (System.Math.Max(System.Math.Min(y, w), 0.0)) / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = -1.0 / l;
            sy = -xl / l;
            sz = yl / l;
        }

        private void projection6(int x, int y, int w, out double sx, out double sy, out double sz) // south pole
        {
            double xl = (System.Math.Max(System.Math.Min(x, w), 0.0)) / w * 2.0 - 1.0;
            double yl = (System.Math.Max(System.Math.Min(y, w), 0.0)) / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = xl / l;
            sy = -yl / l;
            sz = -1.0 / l;
        }

        private void projection1f(double x, double y, double w, out double sx, out double sy, out double sz) // north pole
        {
            double xl = x / w * 2.0 - 1.0;
            double yl = y / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = xl / l;
            sy = yl / l;
            sz = 1.0 / l;
        }

        private void projection2f(double x, double y, double w, out double sx, out double sy, out double sz) // face 1
        {
            double xl = x / w * 2.0 - 1.0;
            double yl = y / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = xl / l;
            sy = -1.0 / l;
            sz = yl / l;
        }

        private void projection3f(double x, double y, double w, out double sx, out double sy, out double sz) // face 2
        {
            double xl = x / w * 2.0 - 1.0;
            double yl = y / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = 1.0 / l;
            sy = xl / l;
            sz = yl / l;
        }

        private void projection4f(double x, double y, double w, out double sx, out double sy, out double sz) // face 3
        {
            double xl = x / w * 2.0 - 1.0;
            double yl = y / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = -xl / l;
            sy = 1.0 / l;
            sz = yl / l;
        }

        private void projection5f(double x, double y, double w, out double sx, out double sy, out double sz) // face 4
        {
            double xl = x / w * 2.0 - 1.0;
            double yl = y / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = -1.0 / l;
            sy = -xl / l;
            sz = yl / l;
        }

        private void projection6f(double x, double y, double w, out double sx, out double sy, out double sz) // south pole
        {
            double xl = x / w * 2.0 - 1.0;
            double yl = y / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = xl / l;
            sy = -yl / l;
            sz = -1.0 / l;
        }

        private void projection1h(int x, int y, int w, out double sx, out double sy, out double sz) // north pole
        {
            double xl = (System.Math.Max(System.Math.Min(x, w - 1), 0) + 0.5) / w * 2.0 - 1.0;
            double yl = (System.Math.Max(System.Math.Min(y, w - 1), 0) + 0.5) / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = xl / l;
            sy = yl / l;
            sz = 1.0 / l;
        }

        private void projection2h(int x, int y, int w, out double sx, out double sy, out double sz) // face 1
        {
            double xl = (System.Math.Max(System.Math.Min(x, w - 1), 0) + 0.5) / w * 2.0 - 1.0;
            double yl = (System.Math.Max(System.Math.Min(y, w - 1), 0) + 0.5) / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = xl / l;
            sy = -1.0 / l;
            sz = yl / l;
        }

        private void projection3h(int x, int y, int w, out double sx, out double sy, out double sz) // face 2
        {
            double xl = (System.Math.Max(System.Math.Min(x, w - 1), 0) + 0.5) / w * 2.0 - 1.0;
            double yl = (System.Math.Max(System.Math.Min(y, w - 1), 0) + 0.5) / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = 1.0 / l;
            sy = xl / l;
            sz = yl / l;
        }

        private void projection4h(int x, int y, int w, out double sx, out double sy, out double sz) // face 3
        {
            double xl = (System.Math.Max(System.Math.Min(x, w - 1), 0) + 0.5) / w * 2.0 - 1.0;
            double yl = (System.Math.Max(System.Math.Min(y, w - 1), 0) + 0.5) / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = -xl / l;
            sy = 1.0 / l;
            sz = yl / l;
        }

        private void projection5h(int x, int y, int w, out double sx, out double sy, out double sz) // face 4
        {
            double xl = (System.Math.Max(System.Math.Min(x, w - 1), 0) + 0.5) / w * 2.0 - 1.0;
            double yl = (System.Math.Max(System.Math.Min(y, w - 1), 0) + 0.5) / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = -1.0 / l;
            sy = -xl / l;
            sz = yl / l;
        }

        private void projection6h(int x, int y, int w, out double sx, out double sy, out double sz) // south pole
        {
            double xl = (System.Math.Max(System.Math.Min(x, w - 1), 0) + 0.5) / w * 2.0 - 1.0;
            double yl = (System.Math.Max(System.Math.Min(y, w - 1), 0) + 0.5) / w * 2.0 - 1.0;
            double l = System.Math.Sqrt(xl * xl + yl * yl + 1.0);
            sx = xl / l;
            sy = -yl / l;
            sz = -1.0 / l;
        }

        private float id(float x)
        {
            return x;
        }

    }
}






















