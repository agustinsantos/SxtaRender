using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sxta.Math;

namespace SxtaRenderTests
{
    [TestClass]
    public class Matrix4fTest
    {
        const double epsilonError = 0.00001;

        #region CreateTranslation
        [TestMethod]
        public void TestCreateTranslation01()
        {
            Matrix4f m = new Matrix4f(1, 0, 0, 1,
                                      0, 1, 0, 2,
                                      0, 0, 1, 3,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix = Matrix4f.CreateTranslation(1, 2, 3);

            Assert.AreEqual(m, viewMatrix);
        }
        #endregion

        #region Scale
        [TestMethod]
        public void TestCreateScale01()
        {
            Matrix4f m = new Matrix4f(1, 0, 0, 0,
                                      0, 2, 0, 0,
                                      0, 0, 3, 0,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix = Matrix4f.Scale(1, 2, 3);

            Assert.AreEqual(m, viewMatrix);
        }
        #endregion

        #region Rotation
        [TestMethod]
        public void TestCreateRotationX01()
        {
            Matrix4f m = new Matrix4f(1, 0, 0, 0,
                                      0, 0.7071067690849304f, -0.7071067690849304f, 0,
                                      0, 0.7071067690849304f, 0.7071067690849304f, 0,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix = Matrix4f.CreateRotationX(Math.PI / 4);

            Assert.AreEqual(m, viewMatrix);
        }
        [TestMethod]
        public void TestCreateRotationX02()
        {
            Matrix4f m = new Matrix4f(1, 0, 0, 0,
                                      0, 6.123032e-17f, 1f, 0,
                                      0, -1f, 6.123032e-17f, 0,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix = Matrix4f.CreateRotationX(-Math.PI / 2);

            Assert.AreEqual(m, viewMatrix);
        }
        [TestMethod]
        public void TestCreateRotationY01()
        {
            Matrix4f m = new Matrix4f(0.7071067690849304f, 0, 0.7071067690849304f, 0,
                                      0, 1, 0, 0,
                                      -0.7071067690849304f, 0, 0.7071067690849304f, 0,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix = Matrix4f.CreateRotationY(Math.PI / 4);

            Assert.AreEqual(m, viewMatrix);
        }
        [TestMethod]
        public void TestCreateRotationY02()
        {
            Matrix4f m = new Matrix4f(6.123032e-17f, 0, -1f, 0,
                                      0, 1, 0, 0,
                                      1f, 0, 6.123032e-17f, 0,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix = Matrix4f.CreateRotationY(-Math.PI / 2);

            Assert.AreEqual(m, viewMatrix);
        }
        [TestMethod]
        public void TestCreateRotationZ01()
        {
            Matrix4f m = new Matrix4f(0.7071067690849304f, -0.7071067690849304f, 0, 0,
                                      0.7071067690849304f, 0.7071067690849304f, 0, 0,
                                      0, 0, 1, 0,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix = Matrix4f.CreateRotationZ(Math.PI / 4);

            Assert.AreEqual(m, viewMatrix);
        }
        [TestMethod]
        public void TestCreateRotationZ02()
        {
            Matrix4f m = new Matrix4f(6.123032e-17f, 1f, 0, 0,
                                      -1f, 6.123032e-17f, 0, 0,
                                      0, 0, 1, 0,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix = Matrix4f.CreateRotationZ(-Math.PI / 2);

            Assert.AreEqual(m, viewMatrix);
        }
        [TestMethod]
        public void TestCreateRotation01()
        {

            Matrix4f m = new Matrix4f(0.7280277013778687f, -0.525104820728302f, 0.4407272934913635f, 0,
                                      0.6087886095046997f, 0.7907905578613281f, -0.06345657259225845f, 0,
                                       -0.31520164012908936f, 0.3145079016685486f, 0.8953952789306641f, 0,
                                       0, 0, 0, 1);

            Matrix4f viewMatrix = Matrix4f.CreateRotation((float)(Math.PI / 4), 1, 2, 3);

            Assert.AreEqual(m.R0C0, viewMatrix.R0C0, epsilonError);
            Assert.AreEqual(m.R0C1, viewMatrix.R0C1, epsilonError);
            Assert.AreEqual(m.R0C2, viewMatrix.R0C2, epsilonError);
            Assert.AreEqual(m.R0C3, viewMatrix.R0C3, epsilonError);
            Assert.AreEqual(m.R1C0, viewMatrix.R1C0, epsilonError);
            Assert.AreEqual(m.R1C1, viewMatrix.R1C1, epsilonError);
            Assert.AreEqual(m.R1C2, viewMatrix.R1C2, epsilonError);
            Assert.AreEqual(m.R1C3, viewMatrix.R1C3, epsilonError);
            Assert.AreEqual(m.R2C0, viewMatrix.R2C0, epsilonError);
            Assert.AreEqual(m.R2C1, viewMatrix.R2C1, epsilonError);
            Assert.AreEqual(m.R2C2, viewMatrix.R2C2, epsilonError);
            Assert.AreEqual(m.R2C3, viewMatrix.R2C3, epsilonError);
            Assert.AreEqual(m.R3C0, viewMatrix.R3C0, epsilonError);
            Assert.AreEqual(m.R3C1, viewMatrix.R3C1, epsilonError);
            Assert.AreEqual(m.R3C2, viewMatrix.R3C2, epsilonError);
            Assert.AreEqual(m.R3C3, viewMatrix.R3C3, epsilonError);
        }

        [TestMethod]
        public void TestCreateRotation02()
        {
            Matrix4f m = new Matrix4f(0.0714285746216774f, 0.9446408748626709f, -0.32023677229881287f, 0,
                                      -0.6589266061782837f, 0.2857142984867096f, 0.6958326697349548f, 0,
                                       0.7488082051277161f, 0.16131018102169037f, 0.6428571343421936f, 0,
                                       0, 0, 0, 1);

            Matrix4f viewMatrix = Matrix4f.CreateRotation((float)(-Math.PI / 2), 1, 2, 3);

            Assert.AreEqual(m.R0C0, viewMatrix.R0C0, epsilonError);
            Assert.AreEqual(m.R0C1, viewMatrix.R0C1, epsilonError);
            Assert.AreEqual(m.R0C2, viewMatrix.R0C2, epsilonError);
            Assert.AreEqual(m.R0C3, viewMatrix.R0C3, epsilonError);
            Assert.AreEqual(m.R1C0, viewMatrix.R1C0, epsilonError);
            Assert.AreEqual(m.R1C1, viewMatrix.R1C1, epsilonError);
            Assert.AreEqual(m.R1C2, viewMatrix.R1C2, epsilonError);
            Assert.AreEqual(m.R1C3, viewMatrix.R1C3, epsilonError);
            Assert.AreEqual(m.R2C0, viewMatrix.R2C0, epsilonError);
            Assert.AreEqual(m.R2C1, viewMatrix.R2C1, epsilonError);
            Assert.AreEqual(m.R2C2, viewMatrix.R2C2, epsilonError);
            Assert.AreEqual(m.R2C3, viewMatrix.R2C3, epsilonError);
            Assert.AreEqual(m.R3C0, viewMatrix.R3C0, epsilonError);
            Assert.AreEqual(m.R3C1, viewMatrix.R3C1, epsilonError);
            Assert.AreEqual(m.R3C2, viewMatrix.R3C2, epsilonError);
            Assert.AreEqual(m.R3C3, viewMatrix.R3C3, epsilonError);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException),
                    "Small length of vector.")]
        public void TestCreateRotation03()
        {
            Matrix4f viewMatrix = Matrix4f.CreateRotation((float)(Math.PI / 2), 0, 0, 0);
        }
        #endregion

        #region Multiply
        [TestMethod]
        public void TestMultiply01()
        {
            Matrix4f m = new Matrix4f(1, 0, 0, 4,
                                      0, 1, 0, 6,
                                      0, 0, 1, 8,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix1 = Matrix4f.CreateTranslation(1, 2, 3);
            Matrix4f viewMatrix2 = Matrix4f.CreateTranslation(3, 4, 5);
            Matrix4f rst = Matrix4f.Mult(viewMatrix1, viewMatrix2);

            Assert.AreEqual(m, rst);
        }

        [TestMethod]
        public void TestMultiply02()
        {
            Matrix4f m = new Matrix4f(3, 0, 0, 0,
                                      0, 8, 0, 0,
                                      0, 0, 15, 0,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix1 = Matrix4f.Scale(1, 2, 3);
            Matrix4f viewMatrix2 = Matrix4f.Scale(3, 4, 5);
            Matrix4f rst = Matrix4f.Mult(viewMatrix1, viewMatrix2);

            Assert.AreEqual(m, rst);
        }

        [TestMethod]
        public void TestMultiply03()
        {
            Matrix4f m = new Matrix4f(3, 0, 0, 1,
                                      0, 4, 0, 2,
                                      0, 0, 5, 3,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix1 = Matrix4f.CreateTranslation(1, 2, 3);
            Matrix4f viewMatrix2 = Matrix4f.Scale(3, 4, 5);
            Matrix4f rst = Matrix4f.Mult(viewMatrix1, viewMatrix2);

            Assert.AreEqual(m, rst);
        }

        [TestMethod]
        public void TestMultiply04()
        {
            Matrix4f m = new Matrix4f(3, 0, 0, 3,
                                      0, 4, 0, 2 * 4,
                                      0, 0, 5, 3 * 5,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix1 = Matrix4f.Scale(3, 4, 5);
            Matrix4f viewMatrix2 = Matrix4f.CreateTranslation(1, 2, 3);
            Matrix4f rst = Matrix4f.Mult(viewMatrix1, viewMatrix2);

            Assert.AreEqual(m, rst);
        }
        #endregion

        #region LookAt
        [TestMethod]
        public void TestLookAt01()
        {
            Vector3f pos = new Vector3f(0, 0, 0);
            Vector3f dir = new Vector3f(0, 0, -1);
            Vector3f up = new Vector3f(0, 1, 0);

            Matrix4f m = new Matrix4f(1, 0, 0, 0,
                                      0, 1, 0, 0,
                                      0, 0, 1, 0,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix = Matrix4f.LookAt(pos, dir, up);

            Assert.AreEqual(m, viewMatrix);
        }

        [TestMethod]
        public void TestLookAt02()
        {
            Vector3f pos = new Vector3f(0, 0, 1);
            Vector3f dir = new Vector3f(0, 0, -1);
            Vector3f up = new Vector3f(0, 1, 0);

            Matrix4f m = new Matrix4f(1, 0, 0, 0,
                                      0, 1, 0, 0,
                                      0, 0, 1, -1,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix = Matrix4f.LookAt(pos, dir, up);

            Assert.AreEqual(m, viewMatrix);
        }

        [TestMethod]
        public void TestLookAt03()
        {
            Vector3f pos = new Vector3f(1, 1, 1);
            Vector3f dir = new Vector3f(1, 1, -1);
            Vector3f up = new Vector3f(0, 1, 0);

            Matrix4f m = new Matrix4f(1, 0, 0, -1,
                                      0, 1, 0, -1,
                                      0, 0, 1, -1,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix = Matrix4f.LookAt(pos, dir, up);

            Assert.AreEqual(m, viewMatrix);
        }

        [TestMethod]
        public void TestLookAt04()
        {
            Vector3f pos = new Vector3f(0, 0, 0);
            Vector3f dir = new Vector3f(0, 0, 1);
            Vector3f up = new Vector3f(0, 1, 0);

            Matrix4f m = new Matrix4f(-1, 0, 0, 0,
                                      0, 1, 0, 0,
                                      0, 0, -1, 0,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix = Matrix4f.LookAt(pos, dir, up);

            Assert.AreEqual(m, viewMatrix);
        }

        [TestMethod]
        public void TestLookAt05()
        {
            Vector3f pos = new Vector3f(2, 2, 2);
            Vector3f dir = new Vector3f(0, 0, 0);
            Vector3f up = new Vector3f(0, 1, 0);

            Matrix4f m = new Matrix4f(0.7071067690849304f, 0, -0.7071067690849304f, 0,
                                      -0.40824830532073975f, 0.8164966106414795f, -0.40824830532073975f, 0,
                                      0.5773502588272095f, 0.5773502588272095f, 0.5773502588272095f, -3.464101552963257f,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix = Matrix4f.LookAt(pos, dir, up);

            Assert.AreEqual(m, viewMatrix);
        }

        [TestMethod]
        public void TestLookAt06()
        {
            Vector3f pos = new Vector3f(0, 0, 0);
            Vector3f dir = new Vector3f(-2, -2, -2);
            Vector3f up = new Vector3f(0, 1, 0);

            Matrix4f m = new Matrix4f(0.7071067690849304f, 0, -0.7071067690849304f, 0,
                                      -0.40824830532073975f, 0.8164966106414795f, -0.40824830532073975f, 0,
                                      0.5773502588272095f, 0.5773502588272095f, 0.5773502588272095f, 0,
                                      0, 0, 0, 1);

            Matrix4f viewMatrix = Matrix4f.LookAt(pos, dir, up);

            Assert.AreEqual(m, viewMatrix);
        }
        #endregion

        #region Perspective
        [TestMethod]
        public void TestCreatePerspectiveFieldOfView01()
        {
            Matrix4f m = new Matrix4f(1.8106601238250732f, 0, 0, 0,
                                      0, 2.4142136573791504f, 0, 0,
                                      0, 0, -1.0020020008087158f, -0.20020020008087158f,
                                      0, 0, -1, 0);

            Matrix4f viewMatrix = Matrix4f.CreatePerspectiveFieldOfView((float)MathHelper.ToRadians(45), 4.0f/3.0f, 0.1f, 100.0f);

            Assert.AreEqual(m.R0C0, viewMatrix.R0C0, epsilonError);
            Assert.AreEqual(m.R0C1, viewMatrix.R0C1, epsilonError);
            Assert.AreEqual(m.R0C2, viewMatrix.R0C2, epsilonError);
            Assert.AreEqual(m.R0C3, viewMatrix.R0C3, epsilonError);
            Assert.AreEqual(m.R1C0, viewMatrix.R1C0, epsilonError);
            Assert.AreEqual(m.R1C1, viewMatrix.R1C1, epsilonError);
            Assert.AreEqual(m.R1C2, viewMatrix.R1C2, epsilonError);
            Assert.AreEqual(m.R1C3, viewMatrix.R1C3, epsilonError);
            Assert.AreEqual(m.R2C0, viewMatrix.R2C0, epsilonError);
            Assert.AreEqual(m.R2C1, viewMatrix.R2C1, epsilonError);
            Assert.AreEqual(m.R2C2, viewMatrix.R2C2, epsilonError);
            Assert.AreEqual(m.R2C3, viewMatrix.R2C3, epsilonError);
            Assert.AreEqual(m.R3C0, viewMatrix.R3C0, epsilonError);
            Assert.AreEqual(m.R3C1, viewMatrix.R3C1, epsilonError);
            Assert.AreEqual(m.R3C2, viewMatrix.R3C2, epsilonError);
            Assert.AreEqual(m.R3C3, viewMatrix.R3C3, epsilonError);
        }
        #endregion
    }
}
