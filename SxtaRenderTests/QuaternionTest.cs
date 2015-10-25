using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sxta.Math;

namespace SxtaRenderTests
{
    [TestClass]
    public class QuaternionTest
    {
        const double epsilonError = 0.00001;

        #region Testing Identity
        [TestMethod]
        public void TestIdentity01()
        {
            Quaternion q = Quaternion.Identity;

            Assert.AreEqual(0, q.X);
            Assert.AreEqual(0, q.Y);
            Assert.AreEqual(0, q.Z);
            Assert.AreEqual(1, q.W);
        }

        [TestMethod]
        public void TestIdentity02()
        {
            Quaternion q1 = Quaternion.Identity;

            q1.X = 1;
            q1.Y = 2;
            q1.Z = 3;
            q1.W = 4;
            Assert.AreEqual(1, q1.X);
            Assert.AreEqual(2, q1.Y);
            Assert.AreEqual(3, q1.Z);
            Assert.AreEqual(4, q1.W);

            Quaternion q2 = Quaternion.Identity;

            Assert.AreEqual(0, q2.X);
            Assert.AreEqual(0, q2.Y);
            Assert.AreEqual(0, q2.Z);
            Assert.AreEqual(1, q2.W);
        }
        #endregion

        #region Testing Constructors
        [TestMethod]
        public void TestConstructor01()
        {
            Quaternion q = new Quaternion();

            Assert.AreEqual(0, q.X);
            Assert.AreEqual(0, q.Y);
            Assert.AreEqual(0, q.Z);
            Assert.AreEqual(0, q.W);
        }

        [TestMethod]
        public void TestConstructor02()
        {
            Vector3f v = new Vector3f(1, 2, 3);

            Quaternion q = new Quaternion(v, 4);

            Assert.AreEqual(1, q.X);
            Assert.AreEqual(2, q.Y);
            Assert.AreEqual(3, q.Z);
            Assert.AreEqual(4, q.W);

            // Modifying the original vector 
            // doesnt alter our quaternion
            v.X = 5;
            v.Y = 6;
            v.Z = 7;
            Assert.AreEqual(1, q.X);
            Assert.AreEqual(2, q.Y);
            Assert.AreEqual(3, q.Z);
            Assert.AreEqual(4, q.W);
        }

        [TestMethod]
        public void TestConstructor03()
        {
            Quaternion q = new Quaternion(1, 2, 3, 4);

            Assert.AreEqual(1, q.X);
            Assert.AreEqual(2, q.Y);
            Assert.AreEqual(3, q.Z);
            Assert.AreEqual(4, q.W);
        }

        [TestMethod]
        public void TestConstructor04()
        {
            Quaternion src = new Quaternion(1, 2, 3, 4);
            Quaternion dst = new Quaternion(src);

            Assert.AreEqual(1, dst.X);
            Assert.AreEqual(2, dst.Y);
            Assert.AreEqual(3, dst.Z);
            Assert.AreEqual(4, dst.W);

            // Modifying the original vector 
            // doesnt alter our quaternion
            dst.X = 5;
            dst.Y = 6;
            dst.Z = 7;
            Assert.AreEqual(1, src.X);
            Assert.AreEqual(2, src.Y);
            Assert.AreEqual(3, src.Z);
            Assert.AreEqual(4, src.W);

            src.X = 8;
            src.Y = 9;
            src.Z = 10;
            Assert.AreEqual(5, dst.X);
            Assert.AreEqual(6, dst.Y);
            Assert.AreEqual(7, dst.Z);
            Assert.AreEqual(4, src.W);
        }

        [TestMethod]
        public void TestConstructor05()
        {
            Vector4f v = new Vector4f(1, 2, 3, 4);

            Quaternion q = new Quaternion(v);

            Assert.AreEqual(1, q.X);
            Assert.AreEqual(2, q.Y);
            Assert.AreEqual(3, q.Z);
            Assert.AreEqual(4, q.W);

            // Modifying the original vector 
            // doesnt alter our quaternion
            v.X = 5;
            v.Y = 6;
            v.Z = 7;
            v.W = 8;
            Assert.AreEqual(1, q.X);
            Assert.AreEqual(2, q.Y);
            Assert.AreEqual(3, q.Z);
            Assert.AreEqual(4, q.W);
        }

        [TestMethod]
        public void TestConstructor06()
        {
            Vector3f axis = new Vector3f(1f, 1f, 1f);
            double angle = MathHelper.ToRadians(120);

            Quaternion q = new Quaternion(angle, axis);

            Assert.AreEqual(0.5, q.X, epsilonError);
            Assert.AreEqual(0.5, q.Y, epsilonError);
            Assert.AreEqual(0.5, q.Z, epsilonError);
            Assert.AreEqual(0.5, q.W, epsilonError);
        }

        [TestMethod]
        public void TestConstructor07()
        {
            Vector3d axis = new Vector3d(1, 1, 1);
            double angle = MathHelper.ToRadians(120);

            Quaternion q = new Quaternion(angle, axis);

            Assert.AreEqual(0.5, q.X, epsilonError);
            Assert.AreEqual(0.5, q.Y, epsilonError);
            Assert.AreEqual(0.5, q.Z, epsilonError);
            Assert.AreEqual(0.5, q.W, epsilonError);
        }
        #endregion

        #region Set Methods
        [TestMethod]
        public void TestSet01()
        {
            Quaternion q = new Quaternion();
            q.Set(1, 2, 3, 4);

            Assert.AreEqual(1, q.X);
            Assert.AreEqual(2, q.Y);
            Assert.AreEqual(3, q.Z);
            Assert.AreEqual(4, q.W);
        }

        [TestMethod]
        public void TestSet02()
        {
            Quaternion q = new Quaternion();
            q.Set(new Vector4f(1, 2, 3, 4));

            Assert.AreEqual(1, q.X);
            Assert.AreEqual(2, q.Y);
            Assert.AreEqual(3, q.Z);
            Assert.AreEqual(4, q.W);
        }

        [TestMethod]
        public void TestSet03()
        {
            Quaternion q = new Quaternion();
            q.Set(new Vector4d(1, 2, 3, 4));

            Assert.AreEqual(1, q.X);
            Assert.AreEqual(2, q.Y);
            Assert.AreEqual(3, q.Z);
            Assert.AreEqual(4, q.W);
        }
        #endregion

        #region To From and To Axis/Angle

        [TestMethod]
        public void TestFromAxisAngle01()
        {
            double angle;

            angle = MathHelper.ToRadians(0);
            Quaternion q1 = new Quaternion(angle, Vector3f.UnitX);

            Assert.AreEqual(0, q1.X, epsilonError);
            Assert.AreEqual(0, q1.Y, epsilonError);
            Assert.AreEqual(0, q1.Z, epsilonError);
            Assert.AreEqual(1, q1.W, epsilonError);

            angle = MathHelper.ToRadians(90);
            Quaternion q2 = new Quaternion(angle, Vector3f.UnitX);

            Assert.AreEqual(0.7071067811865476, q2.X, epsilonError);
            Assert.AreEqual(0, q2.Y, epsilonError);
            Assert.AreEqual(0, q2.Z, epsilonError);
            Assert.AreEqual(0.7071067811865475, q2.W, epsilonError);


            angle = MathHelper.ToRadians(180);
            Quaternion q3 = new Quaternion(angle, Vector3f.UnitX);

            Assert.AreEqual(1, q3.X, epsilonError);
            Assert.AreEqual(0, q3.Y, epsilonError);
            Assert.AreEqual(0, q3.Z, epsilonError);
            Assert.AreEqual(0, q3.W, epsilonError);

            angle = MathHelper.ToRadians(270);
            Quaternion q4 = new Quaternion(angle, Vector3f.UnitX);

            Assert.AreEqual(0.7071067811865476, q4.X, epsilonError);
            Assert.AreEqual(0, q4.Y, epsilonError);
            Assert.AreEqual(0, q4.Z, epsilonError);
            Assert.AreEqual(-0.7071067811865475, q4.W, epsilonError);
        }

        [TestMethod]
        public void TestFromAxisAngle02()
        {
            double angle;

            angle = MathHelper.ToRadians(0);
            Quaternion q1 = new Quaternion(angle, Vector3f.UnitY);

            Assert.AreEqual(0, q1.X, epsilonError);
            Assert.AreEqual(0, q1.Y, epsilonError);
            Assert.AreEqual(0, q1.Z, epsilonError);
            Assert.AreEqual(1, q1.W, epsilonError);

            angle = MathHelper.ToRadians(90);
            Quaternion q2 = new Quaternion(angle, Vector3f.UnitY);

            Assert.AreEqual(0, q2.X, epsilonError);
            Assert.AreEqual(0.7071067811865476, q2.Y, epsilonError);
            Assert.AreEqual(0, q2.Z, epsilonError);
            Assert.AreEqual(0.7071067811865475, q2.W, epsilonError);


            angle = MathHelper.ToRadians(180);
            Quaternion q3 = new Quaternion(angle, Vector3f.UnitY);

            Assert.AreEqual(0, q3.X, epsilonError);
            Assert.AreEqual(1, q3.Y, epsilonError);
            Assert.AreEqual(0, q3.Z, epsilonError);
            Assert.AreEqual(0, q3.W, epsilonError);

            angle = MathHelper.ToRadians(270);
            Quaternion q4 = new Quaternion(angle, Vector3f.UnitY);

            Assert.AreEqual(0, q4.X, epsilonError);
            Assert.AreEqual(0.7071067811865476, q4.Y, epsilonError);
            Assert.AreEqual(0, q4.Z, epsilonError);
            Assert.AreEqual(-0.7071067811865475, q4.W, epsilonError);
        }

        [TestMethod]
        public void TestFromAxisAngle03()
        {
            double angle;

            angle = MathHelper.ToRadians(0);
            Quaternion q1 = new Quaternion(angle, Vector3f.UnitZ);

            Assert.AreEqual(0, q1.X, epsilonError);
            Assert.AreEqual(0, q1.Y, epsilonError);
            Assert.AreEqual(0, q1.Z, epsilonError);
            Assert.AreEqual(1, q1.W, epsilonError);

            angle = MathHelper.ToRadians(90);
            Quaternion q2 = new Quaternion(angle, Vector3f.UnitZ);

            Assert.AreEqual(0, q2.X, epsilonError);
            Assert.AreEqual(0, q2.Y, epsilonError);
            Assert.AreEqual(0.7071067811865476, q2.Z, epsilonError);
            Assert.AreEqual(0.7071067811865475, q2.W, epsilonError);


            angle = MathHelper.ToRadians(180);
            Quaternion q3 = new Quaternion(angle, Vector3f.UnitZ);

            Assert.AreEqual(0, q3.X, epsilonError);
            Assert.AreEqual(0, q3.Y, epsilonError);
            Assert.AreEqual(1, q3.Z, epsilonError);
            Assert.AreEqual(0, q3.W, epsilonError);

            angle = MathHelper.ToRadians(270);
            Quaternion q4 = new Quaternion(angle, Vector3f.UnitZ);

            Assert.AreEqual(0, q4.X, epsilonError);
            Assert.AreEqual(0, q4.Y, epsilonError);
            Assert.AreEqual(0.7071067811865476, q4.Z, epsilonError);
            Assert.AreEqual(-0.7071067811865475, q4.W, epsilonError);
        }

        [TestMethod]
        public void TestFromAxisAngle04()
        {
            double angle = MathHelper.ToRadians(0);
            Quaternion q1 = new Quaternion(angle, new Vector3f(0, 0, 0));

            Assert.AreEqual(0, q1.X, epsilonError);
            Assert.AreEqual(0, q1.Y, epsilonError);
            Assert.AreEqual(0, q1.Z, epsilonError);
            Assert.AreEqual(1, q1.W, epsilonError);
        }

        [TestMethod]
        public void TestToAxisAngle01()
        {
            Quaternion q1 = new Quaternion(0, 0, 0, 1);
            float angle1;
            Vector3f axis1;
            q1.ToAxisAngle(out axis1, out angle1);

            Assert.AreEqual(MathHelper.ToRadians(0), angle1, epsilonError);
            Assert.AreEqual(Vector3f.UnitX, axis1);

            Quaternion q2 = new Quaternion(0.7071067811865476f, 0, 0, 0.7071067811865475f);
            float angle2;
            Vector3f axis2;
            q2.ToAxisAngle(out axis2, out angle2);

            Assert.AreEqual(MathHelper.ToRadians(90), angle2, epsilonError);
            Assert.AreEqual(Vector3f.UnitX, axis2);

            Quaternion q3 = new Quaternion(1, 0, 0, 0);
            float angle3;
            Vector3f axis3;
            q3.ToAxisAngle(out axis3, out angle3);

            Assert.AreEqual(MathHelper.ToRadians(180), angle3, epsilonError);
            Assert.AreEqual(Vector3f.UnitX, axis3);

            Quaternion q4 = new Quaternion(0.7071067811865476f, 0, 0, -0.7071067811865475f);
            float angle4;
            Vector3f axis4;
            q4.ToAxisAngle(out axis4, out angle4);

            Assert.AreEqual(MathHelper.ToRadians(270), angle4, epsilonError);
            Assert.AreEqual(Vector3f.UnitX, axis4);
        }

        [TestMethod]
        public void TestToAxisAngle02()
        {
            Quaternion q2 = new Quaternion(0, 0.7071067811865476f, 0, 0.7071067811865475f);
            float angle2;
            Vector3f axis2;
            q2.ToAxisAngle(out axis2, out angle2);

            Assert.AreEqual(MathHelper.ToRadians(90), angle2, epsilonError);
            Assert.AreEqual(Vector3f.UnitY, axis2);

            Quaternion q3 = new Quaternion(0, 1, 0, 0);
            float angle3;
            Vector3f axis3;
            q3.ToAxisAngle(out axis3, out angle3);

            Assert.AreEqual(MathHelper.ToRadians(180), angle3, epsilonError);
            Assert.AreEqual(Vector3f.UnitY, axis3);

            Quaternion q4 = new Quaternion(0, 0.7071067811865476f, 0, -0.7071067811865475f);
            float angle4;
            Vector3f axis4;
            q4.ToAxisAngle(out axis4, out angle4);

            Assert.AreEqual(MathHelper.ToRadians(270), angle4, epsilonError);
            Assert.AreEqual(Vector3f.UnitY, axis4);
        }

        [TestMethod]
        public void TestToAxisAngle03()
        {
            Quaternion q2 = new Quaternion(0, 0, 0.7071067811865476f, 0.7071067811865475f);
            float angle2;
            Vector3f axis2;
            q2.ToAxisAngle(out axis2, out angle2);

            Assert.AreEqual(MathHelper.ToRadians(90), angle2, epsilonError);
            Assert.AreEqual(Vector3f.UnitZ, axis2);

            Quaternion q3 = new Quaternion(0, 0, 1, 0);
            float angle3;
            Vector3f axis3;
            q3.ToAxisAngle(out axis3, out angle3);

            Assert.AreEqual(MathHelper.ToRadians(180), angle3, epsilonError);
            Assert.AreEqual(Vector3f.UnitZ, axis3);

            Quaternion q4 = new Quaternion(0, 0, 0.7071067811865476f, -0.7071067811865475f);
            float angle4;
            Vector3f axis4;
            q4.ToAxisAngle(out axis4, out angle4);

            Assert.AreEqual(MathHelper.ToRadians(270), angle4, epsilonError);
            Assert.AreEqual(Vector3f.UnitZ, axis4);
        }
        #endregion

        #region Length
        [TestMethod]
        public void TestLength01()
        {
            Quaternion q1 = new Quaternion(0, 0, 0, 1);

            Assert.AreEqual(1, q1.Length, epsilonError);

            Quaternion q2 = new Quaternion(1, 0, 0, 0);

            Assert.AreEqual(1, q2.Length, epsilonError);

            Quaternion q3 = new Quaternion(0, 1, 0, 0);

            Assert.AreEqual(1, q3.Length, epsilonError);

            Quaternion q4 = new Quaternion(0, 0, 1, 0);

            Assert.AreEqual(1, q4.Length, epsilonError);
        }

        [TestMethod]
        public void TestLength02()
        {
            Quaternion q1 = new Quaternion(1, 1, 1, 1);

            Assert.AreEqual(2, q1.Length, epsilonError);
        }

        [TestMethod]
        public void TestLengthSquared01()
        {
            Quaternion q1 = new Quaternion(0, 0, 0, 1);

            Assert.AreEqual(1, q1.LengthSquared, epsilonError);

            Quaternion q2 = new Quaternion(1, 0, 0, 0);

            Assert.AreEqual(1, q2.LengthSquared, epsilonError);

            Quaternion q3 = new Quaternion(0, 1, 0, 0);

            Assert.AreEqual(1, q3.LengthSquared, epsilonError);

            Quaternion q4 = new Quaternion(0, 0, 1, 0);

            Assert.AreEqual(1, q4.LengthSquared, epsilonError);
        }

        [TestMethod]
        public void TestLengthSquared02()
        {
            Quaternion q1 = new Quaternion(1, 1, 1, 1);

            Assert.AreEqual(4, q1.LengthSquared, epsilonError);
        }

        [TestMethod]
        public void TestIsZeroRotation()
        {
            Quaternion q1 = new Quaternion(1, 1, 1, 1);
            Quaternion q2 = new Quaternion(0, 0, 0, 1);

            Assert.IsFalse(q1.IsZeroRotation);
            Assert.IsTrue(q2.IsZeroRotation);
        }
        #endregion

        #region Normalize
        [TestMethod]
        public void TestInstanceNormalize01()
        {
            Quaternion q1 = new Quaternion(0, 0, 0, 1);
            q1.Normalize();

            Assert.AreEqual(1, q1.Length, epsilonError);
            Assert.AreEqual(0, q1.X, epsilonError);
            Assert.AreEqual(0, q1.Y, epsilonError);
            Assert.AreEqual(0, q1.Z, epsilonError);
            Assert.AreEqual(1, q1.W, epsilonError);

            Quaternion q2 = new Quaternion(1, 0, 0, 0);
            q2.Normalize();

            Assert.AreEqual(1, q2.Length, epsilonError);
            Assert.AreEqual(1, q2.X, epsilonError);
            Assert.AreEqual(0, q2.Y, epsilonError);
            Assert.AreEqual(0, q2.Z, epsilonError);
            Assert.AreEqual(0, q2.W, epsilonError);

            Quaternion q3 = new Quaternion(0, 1, 0, 0);
            q3.Normalize();

            Assert.AreEqual(1, q3.Length, epsilonError);
            Assert.AreEqual(0, q3.X, epsilonError);
            Assert.AreEqual(1, q3.Y, epsilonError);
            Assert.AreEqual(0, q3.Z, epsilonError);
            Assert.AreEqual(0, q3.W, epsilonError);

            Quaternion q4 = new Quaternion(0, 0, 1, 0);
            q4.Normalize();

            Assert.AreEqual(1, q4.Length, epsilonError);
            Assert.AreEqual(0, q4.X, epsilonError);
            Assert.AreEqual(0, q4.Y, epsilonError);
            Assert.AreEqual(1, q4.Z, epsilonError);
            Assert.AreEqual(0, q4.W, epsilonError);
        }

        [TestMethod]
        public void TestInstanceNormalize02()
        {
            Quaternion q1 = new Quaternion(0, 0, 0, 100);
            q1.Normalize();

            Assert.AreEqual(1, q1.Length, epsilonError);
            Assert.AreEqual(0, q1.X, epsilonError);
            Assert.AreEqual(0, q1.Y, epsilonError);
            Assert.AreEqual(0, q1.Z, epsilonError);
            Assert.AreEqual(1, q1.W, epsilonError);

            Quaternion q2 = new Quaternion(100, 0, 0, 0);
            q2.Normalize();

            Assert.AreEqual(1, q2.Length, epsilonError);
            Assert.AreEqual(1, q2.X, epsilonError);
            Assert.AreEqual(0, q2.Y, epsilonError);
            Assert.AreEqual(0, q2.Z, epsilonError);
            Assert.AreEqual(0, q2.W, epsilonError);

            Quaternion q3 = new Quaternion(0, 100, 0, 0);
            q3.Normalize();

            Assert.AreEqual(1, q3.Length, epsilonError);
            Assert.AreEqual(0, q3.X, epsilonError);
            Assert.AreEqual(1, q3.Y, epsilonError);
            Assert.AreEqual(0, q3.Z, epsilonError);
            Assert.AreEqual(0, q3.W, epsilonError);

            Quaternion q4 = new Quaternion(0, 0, 100, 0);
            q4.Normalize();

            Assert.AreEqual(1, q4.Length, epsilonError);
            Assert.AreEqual(0, q4.X, epsilonError);
            Assert.AreEqual(0, q4.Y, epsilonError);
            Assert.AreEqual(1, q4.Z, epsilonError);
            Assert.AreEqual(0, q4.W, epsilonError);
        }

        [TestMethod]
        public void TestInstanceNormalize03()
        {
            Quaternion q1 = new Quaternion(0, 0, 0, 0);
            q1.Normalize();

            Assert.AreEqual(float.NaN, q1.X);
            Assert.AreEqual(float.NaN, q1.Y);
            Assert.AreEqual(float.NaN, q1.Z);
            Assert.AreEqual(float.NaN, q1.W);

        }

        public void TestStaticNormalize01()
        {
            Quaternion org1 = new Quaternion(0, 0, 0, 1);
            Quaternion q1 = Quaternion.Normalize(org1);

            Assert.AreEqual(1, q1.Length, epsilonError);
            Assert.AreEqual(0, q1.X, epsilonError);
            Assert.AreEqual(0, q1.Y, epsilonError);
            Assert.AreEqual(0, q1.Z, epsilonError);
            Assert.AreEqual(1, q1.W, epsilonError);

            Quaternion org2 = new Quaternion(1, 0, 0, 0);
            Quaternion q2 = Quaternion.Normalize(org2);

            Assert.AreEqual(1, q2.Length, epsilonError);
            Assert.AreEqual(1, q2.X, epsilonError);
            Assert.AreEqual(0, q2.Y, epsilonError);
            Assert.AreEqual(0, q2.Z, epsilonError);
            Assert.AreEqual(0, q2.W, epsilonError);

            Quaternion org3 = new Quaternion(0, 1, 0, 0);
            Quaternion q3 = Quaternion.Normalize(org3);

            Assert.AreEqual(1, q3.Length, epsilonError);
            Assert.AreEqual(0, q3.X, epsilonError);
            Assert.AreEqual(1, q3.Y, epsilonError);
            Assert.AreEqual(0, q3.Z, epsilonError);
            Assert.AreEqual(0, q3.W, epsilonError);

            Quaternion org4 = new Quaternion(0, 0, 1, 0);
            Quaternion q4 = Quaternion.Normalize(org4);

            Assert.AreEqual(1, q4.Length, epsilonError);
            Assert.AreEqual(0, q4.X, epsilonError);
            Assert.AreEqual(0, q4.Y, epsilonError);
            Assert.AreEqual(1, q4.Z, epsilonError);
            Assert.AreEqual(0, q4.W, epsilonError);
        }

        [TestMethod]
        public void TestStaticNormalize02()
        {
            Quaternion org1 = new Quaternion(0, 0, 0, 100);
            Quaternion q1 = Quaternion.Normalize(org1);

            Assert.AreEqual(1, q1.Length, epsilonError);
            Assert.AreEqual(0, q1.X, epsilonError);
            Assert.AreEqual(0, q1.Y, epsilonError);
            Assert.AreEqual(0, q1.Z, epsilonError);
            Assert.AreEqual(1, q1.W, epsilonError);

            Quaternion org2 = new Quaternion(100, 0, 0, 0);
            Quaternion q2 = Quaternion.Normalize(org2);

            Assert.AreEqual(1, q2.Length, epsilonError);
            Assert.AreEqual(1, q2.X, epsilonError);
            Assert.AreEqual(0, q2.Y, epsilonError);
            Assert.AreEqual(0, q2.Z, epsilonError);
            Assert.AreEqual(0, q2.W, epsilonError);

            Quaternion org3 = new Quaternion(0, 100, 0, 0);
            Quaternion q3 = Quaternion.Normalize(org3);

            Assert.AreEqual(1, q3.Length, epsilonError);
            Assert.AreEqual(0, q3.X, epsilonError);
            Assert.AreEqual(1, q3.Y, epsilonError);
            Assert.AreEqual(0, q3.Z, epsilonError);
            Assert.AreEqual(0, q3.W, epsilonError);

            Quaternion org4 = new Quaternion(0, 0, 100, 0);
            Quaternion q4 = Quaternion.Normalize(org4);

            Assert.AreEqual(1, q4.Length, epsilonError);
            Assert.AreEqual(0, q4.X, epsilonError);
            Assert.AreEqual(0, q4.Y, epsilonError);
            Assert.AreEqual(1, q4.Z, epsilonError);
            Assert.AreEqual(0, q4.W, epsilonError);
        }

        [TestMethod]
        public void TestStaticNormalize03()
        {
            Quaternion org1 = new Quaternion(0, 0, 0, 0);
            Quaternion q1 = Quaternion.Normalize(org1);

            Assert.AreEqual(float.NaN, q1.X);
            Assert.AreEqual(float.NaN, q1.Y);
            Assert.AreEqual(float.NaN, q1.Z);
            Assert.AreEqual(float.NaN, q1.W);

        }
        
        #endregion

        #region Conjugate
        [TestMethod]
        public void TestInstanceConjugate01()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            q1.Conjugate();

            Assert.AreEqual(-1, q1.X);
            Assert.AreEqual(-2, q1.Y);
            Assert.AreEqual(-3, q1.Z);
            Assert.AreEqual(4, q1.W);

        }

        [TestMethod]
        public void TestStaticConjugate01()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            Quaternion rst = Quaternion.Conjugate(q1);

            Assert.AreEqual(-1, rst.X);
            Assert.AreEqual(-2, rst.Y);
            Assert.AreEqual(-3, rst.Z);
            Assert.AreEqual(4, rst.W);

            Assert.AreEqual(1, q1.X);
            Assert.AreEqual(2, q1.Y);
            Assert.AreEqual(3, q1.Z);
            Assert.AreEqual(4, q1.W);
        }

        [TestMethod]
        public void TestStaticConjugate02()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            Quaternion rst;
            Quaternion.Conjugate(ref q1, out rst);

            Assert.AreEqual(-1, rst.X);
            Assert.AreEqual(-2, rst.Y);
            Assert.AreEqual(-3, rst.Z);
            Assert.AreEqual(4, rst.W);

            Assert.AreEqual(1, q1.X);
            Assert.AreEqual(2, q1.Y);
            Assert.AreEqual(3, q1.Z);
            Assert.AreEqual(4, q1.W);
        }
        #endregion

        #region Invert
        [TestMethod]
        public void TestInstanceInvert01()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            q1.Invert();

            Assert.AreEqual(-1.0f / 30.0f, q1.X, epsilonError);
            Assert.AreEqual(-1.0f / 15.0f, q1.Y, epsilonError);
            Assert.AreEqual(-1.0f / 10.0f, q1.Z, epsilonError);
            Assert.AreEqual(2.0f / 15.0f, q1.W, epsilonError);
        }

        [TestMethod]
        public void TestStaticInvert01()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            Quaternion rst = Quaternion.Invert(q1);

            Assert.AreEqual(-1.0f / 30.0f, rst.X, epsilonError);
            Assert.AreEqual(-1.0f / 15.0f, rst.Y, epsilonError);
            Assert.AreEqual(-1.0f / 10.0f, rst.Z, epsilonError);
            Assert.AreEqual(2.0f / 15.0f, rst.W, epsilonError);

            Assert.AreEqual(1, q1.X);
            Assert.AreEqual(2, q1.Y);
            Assert.AreEqual(3, q1.Z);
            Assert.AreEqual(4, q1.W);
        }

        [TestMethod]
        public void TestStaticInvert02()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            Quaternion rst;
            Quaternion.Invert(ref q1, out rst);

            Assert.AreEqual(-1.0f / 30.0f, rst.X, epsilonError);
            Assert.AreEqual(-1.0f / 15.0f, rst.Y, epsilonError);
            Assert.AreEqual(-1.0f / 10.0f, rst.Z, epsilonError);
            Assert.AreEqual(2.0f / 15.0f, rst.W, epsilonError);

            Assert.AreEqual(1, q1.X);
            Assert.AreEqual(2, q1.Y);
            Assert.AreEqual(3, q1.Z);
            Assert.AreEqual(4, q1.W);
        }
        #endregion

        #region Equals, Comparable
        [TestMethod]
        public void TestEqual01()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            Quaternion q2 = new Quaternion(1, 2, 3, 4);
            Quaternion q3 = new Quaternion(3, 2, 1, 4);

            Assert.AreEqual(q1, q2);
            Assert.IsTrue(q1.Equals(q2));
            Assert.IsTrue(q2.Equals(q1));
            Assert.IsTrue(q1 == q2);
            Assert.IsFalse(q1 != q2);

            Assert.AreNotEqual(q1, q3);
            Assert.IsFalse(q1.Equals(q3));
            Assert.IsFalse(q3.Equals(q1));
            Assert.IsFalse(q1 == q3);
            Assert.IsTrue(q1 != q3);
        }

        [TestMethod]
        public void TestEqual02()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            Object dummy = new object();
            Assert.AreNotEqual(q1, dummy);
            Assert.IsFalse(q1.Equals(dummy));
            Assert.IsFalse(dummy.Equals(q1));
        }

        [TestMethod]
        public void TestGetHashCode01()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            Quaternion q2 = new Quaternion(1, 2, 3, 4);
            Quaternion q3 = new Quaternion(5, 6, 7, 8);

            Assert.AreEqual(q1.GetHashCode(), q2.GetHashCode());
            Assert.AreNotEqual(q1.GetHashCode(), q3.GetHashCode());
        }

        [TestMethod]
        public void TestCompare01()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            Quaternion q2 = new Quaternion(1, 2, 3, 4);
            Quaternion q3 = new Quaternion(5, 6, 7, 8);

            Assert.IsFalse(q1 > q2);
            Assert.IsFalse(q2 > q1);
            Assert.IsFalse(q1 > q3);
            Assert.IsTrue(q3 > q1);
            Assert.IsTrue(q2 >= q1);
            Assert.IsTrue(q1 >= q2);

            Assert.IsTrue(q1 <= q2);
            Assert.IsTrue(q2 <= q1);
            Assert.IsTrue(q1 <= q3);
            Assert.IsFalse(q3 <= q1);
            Assert.IsFalse(q2 < q1);
            Assert.IsFalse(q1 < q2);

        }
        #endregion

        #region Add
        [TestMethod]
        public void TestAddStaticOperator()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            Quaternion q2 = new Quaternion(4, 3, 2, 1);
            Quaternion q3 = new Quaternion(5, 6, 7, 8);

            Quaternion rst1 = q1 + q2;
            Quaternion rst2 = q1 + q3;
            Quaternion rst3 = q2 + q3;

            Assert.AreEqual(5, rst1.X, epsilonError);
            Assert.AreEqual(5, rst1.Y, epsilonError);
            Assert.AreEqual(5, rst1.Z, epsilonError);
            Assert.AreEqual(5, rst1.W, epsilonError);

            Assert.AreEqual(6, rst2.X, epsilonError);
            Assert.AreEqual(8, rst2.Y, epsilonError);
            Assert.AreEqual(10, rst2.Z, epsilonError);
            Assert.AreEqual(12, rst2.W, epsilonError);

            Assert.AreEqual(9, rst3.X, epsilonError);
            Assert.AreEqual(9, rst3.Y, epsilonError);
            Assert.AreEqual(9, rst3.Z, epsilonError);
            Assert.AreEqual(9, rst3.W, epsilonError);

            Assert.AreEqual(1, q1.X, epsilonError);
            Assert.AreEqual(2, q1.Y, epsilonError);
            Assert.AreEqual(3, q1.Z, epsilonError);
            Assert.AreEqual(4, q1.W, epsilonError);

            Assert.AreEqual(4, q2.X, epsilonError);
            Assert.AreEqual(3, q2.Y, epsilonError);
            Assert.AreEqual(2, q2.Z, epsilonError);
            Assert.AreEqual(1, q2.W, epsilonError);

            Assert.AreEqual(5, q3.X, epsilonError);
            Assert.AreEqual(6, q3.Y, epsilonError);
            Assert.AreEqual(7, q3.Z, epsilonError);
            Assert.AreEqual(8, q3.W, epsilonError);
        }

        [TestMethod]
        public void TestAddStaticMethod01()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            Quaternion q2 = new Quaternion(4, 3, 2, 1);
            Quaternion q3 = new Quaternion(5, 6, 7, 8);

            Quaternion rst1 = Quaternion.Add(q1, q2);
            Quaternion rst2 = Quaternion.Add(q1, q3);
            Quaternion rst3 = Quaternion.Add(q2, q3);

            Assert.AreEqual(5, rst1.X, epsilonError);
            Assert.AreEqual(5, rst1.Y, epsilonError);
            Assert.AreEqual(5, rst1.Z, epsilonError);
            Assert.AreEqual(5, rst1.W, epsilonError);

            Assert.AreEqual(6, rst2.X, epsilonError);
            Assert.AreEqual(8, rst2.Y, epsilonError);
            Assert.AreEqual(10, rst2.Z, epsilonError);
            Assert.AreEqual(12, rst2.W, epsilonError);

            Assert.AreEqual(9, rst3.X, epsilonError);
            Assert.AreEqual(9, rst3.Y, epsilonError);
            Assert.AreEqual(9, rst3.Z, epsilonError);
            Assert.AreEqual(9, rst3.W, epsilonError);

            Assert.AreEqual(1, q1.X, epsilonError);
            Assert.AreEqual(2, q1.Y, epsilonError);
            Assert.AreEqual(3, q1.Z, epsilonError);
            Assert.AreEqual(4, q1.W, epsilonError);

            Assert.AreEqual(4, q2.X, epsilonError);
            Assert.AreEqual(3, q2.Y, epsilonError);
            Assert.AreEqual(2, q2.Z, epsilonError);
            Assert.AreEqual(1, q2.W, epsilonError);

            Assert.AreEqual(5, q3.X, epsilonError);
            Assert.AreEqual(6, q3.Y, epsilonError);
            Assert.AreEqual(7, q3.Z, epsilonError);
            Assert.AreEqual(8, q3.W, epsilonError);
        }

        [TestMethod]
        public void TestAddStaticMethod02()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            Quaternion q2 = new Quaternion(4, 3, 2, 1);
            Quaternion q3 = new Quaternion(5, 6, 7, 8);

            Quaternion rst1, rst2, rst3;
            Quaternion.Add(ref q1, ref q2, out rst1);
            Quaternion.Add(ref q1, ref q3, out rst2);
            Quaternion.Add(ref q2, ref q3, out rst3);

            Assert.AreEqual(5, rst1.X, epsilonError);
            Assert.AreEqual(5, rst1.Y, epsilonError);
            Assert.AreEqual(5, rst1.Z, epsilonError);
            Assert.AreEqual(5, rst1.W, epsilonError);

            Assert.AreEqual(6, rst2.X, epsilonError);
            Assert.AreEqual(8, rst2.Y, epsilonError);
            Assert.AreEqual(10, rst2.Z, epsilonError);
            Assert.AreEqual(12, rst2.W, epsilonError);

            Assert.AreEqual(9, rst3.X, epsilonError);
            Assert.AreEqual(9, rst3.Y, epsilonError);
            Assert.AreEqual(9, rst3.Z, epsilonError);
            Assert.AreEqual(9, rst3.W, epsilonError);

            Assert.AreEqual(1, q1.X, epsilonError);
            Assert.AreEqual(2, q1.Y, epsilonError);
            Assert.AreEqual(3, q1.Z, epsilonError);
            Assert.AreEqual(4, q1.W, epsilonError);

            Assert.AreEqual(4, q2.X, epsilonError);
            Assert.AreEqual(3, q2.Y, epsilonError);
            Assert.AreEqual(2, q2.Z, epsilonError);
            Assert.AreEqual(1, q2.W, epsilonError);

            Assert.AreEqual(5, q3.X, epsilonError);
            Assert.AreEqual(6, q3.Y, epsilonError);
            Assert.AreEqual(7, q3.Z, epsilonError);
            Assert.AreEqual(8, q3.W, epsilonError);
        }

        [TestMethod]
        public void TestAddInstanceMethod()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            Quaternion q2 = new Quaternion(4, 3, 2, 1);
            Quaternion q3 = new Quaternion(5, 6, 7, 8);

            q1.Add(q2);

            Assert.AreEqual(4, q2.X, epsilonError);
            Assert.AreEqual(3, q2.Y, epsilonError);
            Assert.AreEqual(2, q2.Z, epsilonError);
            Assert.AreEqual(1, q2.W, epsilonError);

            q2.Add(q3);

            Assert.AreEqual(5, q1.X, epsilonError);
            Assert.AreEqual(5, q1.Y, epsilonError);
            Assert.AreEqual(5, q1.Z, epsilonError);
            Assert.AreEqual(5, q1.W, epsilonError);

            Assert.AreEqual(9, q2.X, epsilonError);
            Assert.AreEqual(9, q2.Y, epsilonError);
            Assert.AreEqual(9, q2.Z, epsilonError);
            Assert.AreEqual(9, q2.W, epsilonError);

            Assert.AreEqual(5, q3.X, epsilonError);
            Assert.AreEqual(6, q3.Y, epsilonError);
            Assert.AreEqual(7, q3.Z, epsilonError);
            Assert.AreEqual(8, q3.W, epsilonError);
        }
        #endregion

        #region Sub
        [TestMethod]
        public void TestSubStaticOperator()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            Quaternion q2 = new Quaternion(4, 3, 2, 1);
            Quaternion q3 = new Quaternion(5, 6, 7, 8);

            Quaternion rst1 = q1 - q2;
            Quaternion rst2 = q1 - q3;
            Quaternion rst3 = q2 - q3;

            Assert.AreEqual(-3, rst1.X, epsilonError);
            Assert.AreEqual(-1, rst1.Y, epsilonError);
            Assert.AreEqual(1, rst1.Z, epsilonError);
            Assert.AreEqual(3, rst1.W, epsilonError);

            Assert.AreEqual(-4, rst2.X, epsilonError);
            Assert.AreEqual(-4, rst2.Y, epsilonError);
            Assert.AreEqual(-4, rst2.Z, epsilonError);
            Assert.AreEqual(-4, rst2.W, epsilonError);

            Assert.AreEqual(-1, rst3.X, epsilonError);
            Assert.AreEqual(-3, rst3.Y, epsilonError);
            Assert.AreEqual(-5, rst3.Z, epsilonError);
            Assert.AreEqual(-7, rst3.W, epsilonError);

            Assert.AreEqual(1, q1.X, epsilonError);
            Assert.AreEqual(2, q1.Y, epsilonError);
            Assert.AreEqual(3, q1.Z, epsilonError);
            Assert.AreEqual(4, q1.W, epsilonError);

            Assert.AreEqual(4, q2.X, epsilonError);
            Assert.AreEqual(3, q2.Y, epsilonError);
            Assert.AreEqual(2, q2.Z, epsilonError);
            Assert.AreEqual(1, q2.W, epsilonError);

            Assert.AreEqual(5, q3.X, epsilonError);
            Assert.AreEqual(6, q3.Y, epsilonError);
            Assert.AreEqual(7, q3.Z, epsilonError);
            Assert.AreEqual(8, q3.W, epsilonError);
        }

        [TestMethod]
        public void TestSubStaticMethod01()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            Quaternion q2 = new Quaternion(4, 3, 2, 1);
            Quaternion q3 = new Quaternion(5, 6, 7, 8);

            Quaternion rst1 = Quaternion.Sub(q1, q2);
            Quaternion rst2 = Quaternion.Sub(q1, q3);
            Quaternion rst3 = Quaternion.Sub(q2, q3);

            Assert.AreEqual(-3, rst1.X, epsilonError);
            Assert.AreEqual(-1, rst1.Y, epsilonError);
            Assert.AreEqual(1, rst1.Z, epsilonError);
            Assert.AreEqual(3, rst1.W, epsilonError);

            Assert.AreEqual(-4, rst2.X, epsilonError);
            Assert.AreEqual(-4, rst2.Y, epsilonError);
            Assert.AreEqual(-4, rst2.Z, epsilonError);
            Assert.AreEqual(-4, rst2.W, epsilonError);

            Assert.AreEqual(-1, rst3.X, epsilonError);
            Assert.AreEqual(-3, rst3.Y, epsilonError);
            Assert.AreEqual(-5, rst3.Z, epsilonError);
            Assert.AreEqual(-7, rst3.W, epsilonError);

            Assert.AreEqual(1, q1.X, epsilonError);
            Assert.AreEqual(2, q1.Y, epsilonError);
            Assert.AreEqual(3, q1.Z, epsilonError);
            Assert.AreEqual(4, q1.W, epsilonError);

            Assert.AreEqual(4, q2.X, epsilonError);
            Assert.AreEqual(3, q2.Y, epsilonError);
            Assert.AreEqual(2, q2.Z, epsilonError);
            Assert.AreEqual(1, q2.W, epsilonError);

            Assert.AreEqual(5, q3.X, epsilonError);
            Assert.AreEqual(6, q3.Y, epsilonError);
            Assert.AreEqual(7, q3.Z, epsilonError);
            Assert.AreEqual(8, q3.W, epsilonError);
        }

        [TestMethod]
        public void TestSubStaticMethod02()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            Quaternion q2 = new Quaternion(4, 3, 2, 1);
            Quaternion q3 = new Quaternion(5, 6, 7, 8);

            Quaternion rst1, rst2, rst3;
            Quaternion.Sub(ref q1, ref q2, out rst1);
            Quaternion.Sub(ref q1, ref q3, out rst2);
            Quaternion.Sub(ref q2, ref q3, out rst3);


            Assert.AreEqual(-3, rst1.X, epsilonError);
            Assert.AreEqual(-1, rst1.Y, epsilonError);
            Assert.AreEqual(1, rst1.Z, epsilonError);
            Assert.AreEqual(3, rst1.W, epsilonError);

            Assert.AreEqual(-4, rst2.X, epsilonError);
            Assert.AreEqual(-4, rst2.Y, epsilonError);
            Assert.AreEqual(-4, rst2.Z, epsilonError);
            Assert.AreEqual(-4, rst2.W, epsilonError);

            Assert.AreEqual(-1, rst3.X, epsilonError);
            Assert.AreEqual(-3, rst3.Y, epsilonError);
            Assert.AreEqual(-5, rst3.Z, epsilonError);
            Assert.AreEqual(-7, rst3.W, epsilonError);

            Assert.AreEqual(1, q1.X, epsilonError);
            Assert.AreEqual(2, q1.Y, epsilonError);
            Assert.AreEqual(3, q1.Z, epsilonError);
            Assert.AreEqual(4, q1.W, epsilonError);

            Assert.AreEqual(4, q2.X, epsilonError);
            Assert.AreEqual(3, q2.Y, epsilonError);
            Assert.AreEqual(2, q2.Z, epsilonError);
            Assert.AreEqual(1, q2.W, epsilonError);

            Assert.AreEqual(5, q3.X, epsilonError);
            Assert.AreEqual(6, q3.Y, epsilonError);
            Assert.AreEqual(7, q3.Z, epsilonError);
            Assert.AreEqual(8, q3.W, epsilonError);
        }

        [TestMethod]
        public void TestSubInstanceMethod()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            Quaternion q2 = new Quaternion(4, 3, 2, 1);
            Quaternion q3 = new Quaternion(5, 6, 7, 8);

            q1.Sub(q2);

            Assert.AreEqual(4, q2.X, epsilonError);
            Assert.AreEqual(3, q2.Y, epsilonError);
            Assert.AreEqual(2, q2.Z, epsilonError);
            Assert.AreEqual(1, q2.W, epsilonError);

            q2.Sub(q3);

            Assert.AreEqual(-3, q1.X, epsilonError);
            Assert.AreEqual(-1, q1.Y, epsilonError);
            Assert.AreEqual(1, q1.Z, epsilonError);
            Assert.AreEqual(3, q1.W, epsilonError);

            Assert.AreEqual(-1, q2.X, epsilonError);
            Assert.AreEqual(-3, q2.Y, epsilonError);
            Assert.AreEqual(-5, q2.Z, epsilonError);
            Assert.AreEqual(-7, q2.W, epsilonError);

            Assert.AreEqual(5, q3.X, epsilonError);
            Assert.AreEqual(6, q3.Y, epsilonError);
            Assert.AreEqual(7, q3.Z, epsilonError);
            Assert.AreEqual(8, q3.W, epsilonError);
        }
        #endregion

        #region Mul
        [TestMethod]
        public void TestMulStaticOperator()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);

            Quaternion rst1 = q1 * 2;
            Quaternion rst2 = 2 * q1;

            Assert.AreEqual(2, rst1.X, epsilonError);
            Assert.AreEqual(4, rst1.Y, epsilonError);
            Assert.AreEqual(6, rst1.Z, epsilonError);
            Assert.AreEqual(8, rst1.W, epsilonError);

            Assert.AreEqual(2, rst2.X, epsilonError);
            Assert.AreEqual(4, rst2.Y, epsilonError);
            Assert.AreEqual(6, rst2.Z, epsilonError);
            Assert.AreEqual(8, rst2.W, epsilonError);

            Assert.AreEqual(1, q1.X, epsilonError);
            Assert.AreEqual(2, q1.Y, epsilonError);
            Assert.AreEqual(3, q1.Z, epsilonError);
            Assert.AreEqual(4, q1.W, epsilonError);
        }

        [TestMethod]
        public void TestMulStaticMethod01()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);

            Quaternion rst1 = Quaternion.Multiply(q1, 2);

            Assert.AreEqual(2, rst1.X, epsilonError);
            Assert.AreEqual(4, rst1.Y, epsilonError);
            Assert.AreEqual(6, rst1.Z, epsilonError);
            Assert.AreEqual(8, rst1.W, epsilonError);

            Assert.AreEqual(1, q1.X, epsilonError);
            Assert.AreEqual(2, q1.Y, epsilonError);
            Assert.AreEqual(3, q1.Z, epsilonError);
            Assert.AreEqual(4, q1.W, epsilonError);
        }

        [TestMethod]
        public void TestMulStaticMethod02()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);

            Quaternion rst1;
            Quaternion.Multiply(ref q1, 2, out rst1);

            Assert.AreEqual(2, rst1.X, epsilonError);
            Assert.AreEqual(4, rst1.Y, epsilonError);
            Assert.AreEqual(6, rst1.Z, epsilonError);
            Assert.AreEqual(8, rst1.W, epsilonError);

            Assert.AreEqual(1, q1.X, epsilonError);
            Assert.AreEqual(2, q1.Y, epsilonError);
            Assert.AreEqual(3, q1.Z, epsilonError);
            Assert.AreEqual(4, q1.W, epsilonError);
        }

        [TestMethod]
        public void TestMulInstanceMethod()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);

            q1.Multiply(2);

            Assert.AreEqual(2, q1.X, epsilonError);
            Assert.AreEqual(4, q1.Y, epsilonError);
            Assert.AreEqual(6, q1.Z, epsilonError);
            Assert.AreEqual(8, q1.W, epsilonError);
        }
        #endregion

        #region From and To Matrix
        [TestMethod]
        public void TestFromMatrix01()
        {
            Quaternion rst = new Quaternion(1, 2, 3, 4);
            Matrix3f m = new Matrix3f(2.0f / 15.0f, -2.0f / 3.0f, 11.0f / 15.0f,
                                     14.0f / 15.0f, 1.0f / 3.0f, 2.0f / 15.0f,
                                     -1.0f / 3.0f, 2.0f / 3.0f, 2.0f / 3.0f);
            Quaternion q1 = Quaternion.FromRotateMatrix(m);
            rst.Normalize();

            Assert.AreEqual(rst.X, q1.X, epsilonError);
            Assert.AreEqual(rst.Y, q1.Y, epsilonError);
            Assert.AreEqual(rst.Z, q1.Z, epsilonError);
            Assert.AreEqual(rst.W, q1.W, epsilonError);
        }

        [TestMethod]
        public void TestToMatrix01()
        {
            Matrix3f m = new Matrix3f(2.0f / 15.0f, -2.0f / 3.0f, 11.0f / 15.0f,
                                     14.0f / 15.0f, 1.0f / 3.0f, 2.0f / 15.0f,
                                     -1.0f / 3.0f, 2.0f / 3.0f, 2.0f / 3.0f);
            Quaternion q = new Quaternion(1, 2, 3, 4);
            Matrix3f rst = Quaternion.ToMatrix3f(q);

            Assert.AreEqual(m.R0C0, rst.R0C0, epsilonError);
            Assert.AreEqual(m.R0C1, rst.R0C1, epsilonError);
            Assert.AreEqual(m.R0C2, rst.R0C2, epsilonError);

            Assert.AreEqual(m.R1C0, rst.R1C0, epsilonError);
            Assert.AreEqual(m.R1C1, rst.R1C1, epsilonError);
            Assert.AreEqual(m.R1C2, rst.R1C2, epsilonError);

            Assert.AreEqual(m.R2C0, rst.R2C0, epsilonError);
            Assert.AreEqual(m.R2C1, rst.R2C1, epsilonError);
            Assert.AreEqual(m.R2C2, rst.R2C2, epsilonError);
        }

        [TestMethod]
        public void TestToMatrix02()
        {
            Matrix4f m = new Matrix4f(2.0f / 15.0f, -2.0f / 3.0f, 11.0f / 15.0f, 0,
                                     14.0f / 15.0f, 1.0f / 3.0f, 2.0f / 15.0f, 0,
                                     -1.0f / 3.0f, 2.0f / 3.0f, 2.0f / 3.0f, 0,
                                     0, 0, 0, 1);
            Quaternion q = new Quaternion(1, 2, 3, 4);
            Matrix4f rst = Quaternion.ToMatrix4f(q);

            Assert.AreEqual(m.R0C0, rst.R0C0, epsilonError);
            Assert.AreEqual(m.R0C1, rst.R0C1, epsilonError);
            Assert.AreEqual(m.R0C2, rst.R0C2, epsilonError);
            Assert.AreEqual(m.R0C3, rst.R0C3, epsilonError);

            Assert.AreEqual(m.R1C0, rst.R1C0, epsilonError);
            Assert.AreEqual(m.R1C1, rst.R1C1, epsilonError);
            Assert.AreEqual(m.R1C2, rst.R1C2, epsilonError);
            Assert.AreEqual(m.R1C3, rst.R1C3, epsilonError);

            Assert.AreEqual(m.R2C0, rst.R2C0, epsilonError);
            Assert.AreEqual(m.R2C1, rst.R2C1, epsilonError);
            Assert.AreEqual(m.R2C2, rst.R2C2, epsilonError);
            Assert.AreEqual(m.R2C3, rst.R2C3, epsilonError);

            Assert.AreEqual(m.R3C0, rst.R3C0, epsilonError);
            Assert.AreEqual(m.R3C1, rst.R3C1, epsilonError);
            Assert.AreEqual(m.R3C2, rst.R3C2, epsilonError);
            Assert.AreEqual(m.R3C3, rst.R3C3, epsilonError);
        }
        #endregion

        #region Misc
        [TestMethod]
        public void TestToString()
        {
            Quaternion q1 = new Quaternion(1, 2, 3, 4);
            string str = q1.ToString();
            Assert.AreEqual("V: (1, 2, 3), W: 4", str);
        }
        #endregion
    }
}
