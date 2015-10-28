using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sxta.Math;

namespace SxtaRenderTests
{
    [TestClass]
    public class Vector3fTest
    {
        const double epsilonError = 0.00001;

        #region Testing Constants: Zero, One, UnitX, UnitY, UnitZ
        [TestMethod]
        public void TestZero01()
        {
            Vector3f v = Vector3f.Zero;

            Assert.AreEqual(0, v.X);
            Assert.AreEqual(0, v.Y);
            Assert.AreEqual(0, v.Z);
        }

        [TestMethod]
        public void TestZero02()
        {
            Vector3f v1 = Vector3f.Zero;

            v1.X = 1;
            v1.Y = 2;
            v1.Z = 3;
            Assert.AreEqual(1, v1.X);
            Assert.AreEqual(2, v1.Y);
            Assert.AreEqual(3, v1.Z);

            Vector3f v2 = Vector3f.Zero;

            Assert.AreEqual(0, v2.X);
            Assert.AreEqual(0, v2.Y);
            Assert.AreEqual(0, v2.Z);
        }

        [TestMethod]
        public void TestOne01()
        {
            Vector3f v = Vector3f.One;

            Assert.AreEqual(1, v.X);
            Assert.AreEqual(1, v.Y);
            Assert.AreEqual(1, v.Z);
        }

        [TestMethod]
        public void TestOne02()
        {
            Vector3f v1 = Vector3f.One;

            v1.X = 1;
            v1.Y = 2;
            v1.Z = 3;
            Assert.AreEqual(1, v1.X);
            Assert.AreEqual(2, v1.Y);
            Assert.AreEqual(3, v1.Z);

            Vector3f v2 = Vector3f.One;

            Assert.AreEqual(1, v2.X);
            Assert.AreEqual(1, v2.Y);
            Assert.AreEqual(1, v2.Z);
        }

        [TestMethod]
        public void TestUnitX01()
        {
            Vector3f v = Vector3f.UnitX;

            Assert.AreEqual(1, v.X);
            Assert.AreEqual(0, v.Y);
            Assert.AreEqual(0, v.Z);
        }

        [TestMethod]
        public void TestUnitX02()
        {
            Vector3f v1 = Vector3f.UnitX;

            v1.X = 1;
            v1.Y = 2;
            v1.Z = 3;
            Assert.AreEqual(1, v1.X);
            Assert.AreEqual(2, v1.Y);
            Assert.AreEqual(3, v1.Z);

            Vector3f v2 = Vector3f.UnitX;

            Assert.AreEqual(1, v2.X);
            Assert.AreEqual(0, v2.Y);
            Assert.AreEqual(0, v2.Z);
        }
        [TestMethod]
        public void TestUnitY01()
        {
            Vector3f v = Vector3f.UnitY;

            Assert.AreEqual(0, v.X);
            Assert.AreEqual(1, v.Y);
            Assert.AreEqual(0, v.Z);
        }

        [TestMethod]
        public void TestUnitY02()
        {
            Vector3f v1 = Vector3f.UnitY;

            v1.X = 1;
            v1.Y = 2;
            v1.Z = 3;
            Assert.AreEqual(1, v1.X);
            Assert.AreEqual(2, v1.Y);
            Assert.AreEqual(3, v1.Z);

            Vector3f v2 = Vector3f.UnitY;

            Assert.AreEqual(0, v2.X);
            Assert.AreEqual(1, v2.Y);
            Assert.AreEqual(0, v2.Z);
        }
        [TestMethod]
        public void TestUnitZ01()
        {
            Vector3f v = Vector3f.UnitZ;

            Assert.AreEqual(0, v.X);
            Assert.AreEqual(0, v.Y);
            Assert.AreEqual(1, v.Z);
        }

        [TestMethod]
        public void TestUnitZ02()
        {
            Vector3f v1 = Vector3f.UnitZ;

            v1.X = 1;
            v1.Y = 2;
            v1.Z = 3;
            Assert.AreEqual(1, v1.X);
            Assert.AreEqual(2, v1.Y);
            Assert.AreEqual(3, v1.Z);

            Vector3f v2 = Vector3f.UnitZ;

            Assert.AreEqual(0, v2.X);
            Assert.AreEqual(0, v2.Y);
            Assert.AreEqual(1, v2.Z);
        }
        #endregion

        #region Testing Constructors
        [TestMethod]
        public void TestConstructor01()
        {
            Vector3f v = new Vector3f();

            Assert.AreEqual(0, v.X);
            Assert.AreEqual(0, v.Y);
            Assert.AreEqual(0, v.Z);
        }

        [TestMethod]
        public void TestConstructor02()
        {
            Vector3f v = new Vector3f(1, 2, 3);
            Vector3f q1 = new Vector3f(v);

            Assert.AreEqual(1, q1.X);
            Assert.AreEqual(2, q1.Y);
            Assert.AreEqual(3, q1.Z);

            // Modifying the original vector 
            // doesnt alter our vector
            v.X = 5;
            v.Y = 6;
            v.Z = 7;
            Assert.AreEqual(5, v.X);
            Assert.AreEqual(6, v.Y);
            Assert.AreEqual(7, v.Z);
            Assert.AreEqual(1, q1.X);
            Assert.AreEqual(2, q1.Y);
            Assert.AreEqual(3, q1.Z);

            v = new Vector3f(1, 2, 3);
            Vector3f q2 = v;

            Assert.AreEqual(1, q2.X);
            Assert.AreEqual(2, q2.Y);
            Assert.AreEqual(3, q2.Z);

            // Modifying the original vector 
            // doesnt alter our vector
            v.X = 5;
            v.Y = 6;
            v.Z = 7;
            Assert.AreEqual(5, v.X);
            Assert.AreEqual(6, v.Y);
            Assert.AreEqual(7, v.Z);
            Assert.AreEqual(1, q2.X);
            Assert.AreEqual(2, q2.Y);
            Assert.AreEqual(3, q2.Z);
        }

        [TestMethod]
        public void TestConstructor03()
        {
            Vector3f v = new Vector3f(1, 2, 3);

            Assert.AreEqual(1, v.X);
            Assert.AreEqual(2, v.Y);
            Assert.AreEqual(3, v.Z);
        }

        [TestMethod]
        public void TestConstructor04()
        {
            Vector4f src = new Vector4f(1, 2, 3, 4);
            Vector3f dst = new Vector3f(src);

            Assert.AreEqual(1, dst.X);
            Assert.AreEqual(2, dst.Y);
            Assert.AreEqual(3, dst.Z);

            // Modifying the one vector 
            // doesnt alter the other vector
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
            Vector2f v2 = new Vector2f(1, 2);

            Vector3f v = new Vector3f(v2);

            Assert.AreEqual(1, v.X);
            Assert.AreEqual(2, v.Y);
            Assert.AreEqual(0, v.Z);

            // Modifying the original vector 
            // doesnt alter our quaternion
            v2.X = 5;
            v2.Y = 6;
            Assert.AreEqual(1, v.X);
            Assert.AreEqual(2, v.Y);
            Assert.AreEqual(0, v.Z);
        }

        #endregion

        #region Set Methods
        [TestMethod]
        public void TestSet01()
        {
            Vector3f v = new Vector3f();
            v.Set(1, 2, 3);

            Assert.AreEqual(1, v.X);
            Assert.AreEqual(2, v.Y);
            Assert.AreEqual(3, v.Z);
        }
        #endregion

        #region Length
        [TestMethod]
        public void TestLength01()
        {
            Vector3f v1 = new Vector3f(1, 0, 0);

            Assert.AreEqual(1, v1.Length, epsilonError);

            Vector3f v2 = new Vector3f(0, 1, 0);

            Assert.AreEqual(1, v2.Length, epsilonError);

            Vector3f v3 = new Vector3f(0, 0, 1);

            Assert.AreEqual(1, v3.Length, epsilonError);
        }

        [TestMethod]
        public void TestLength02()
        {
            Vector3f v1 = new Vector3f(1, 1, 1);

            Assert.AreEqual(Math.Sqrt(3), v1.Length, epsilonError);
        }

        [TestMethod]
        public void TestLengthSquared01()
        {

            Vector3f v1 = new Vector3f(1, 0, 0);

            Assert.AreEqual(1, v1.LengthSquared, epsilonError);

            Vector3f v2 = new Vector3f(0, 1, 0);

            Assert.AreEqual(1, v2.LengthSquared, epsilonError);

            Vector3f v3 = new Vector3f(0, 0, 1);

            Assert.AreEqual(1, v3.LengthSquared, epsilonError);

            Vector3f v4 = new Vector3f(2, 0, 0);

            Assert.AreEqual(4, v4.LengthSquared, epsilonError);

            Vector3f v5 = new Vector3f(0, 2, 0);

            Assert.AreEqual(4, v5.LengthSquared, epsilonError);

            Vector3f v6 = new Vector3f(0, 0, 2);

            Assert.AreEqual(4, v6.LengthSquared, epsilonError);
        }

        [TestMethod]
        public void TestLengthSquared02()
        {
            Vector3f v1 = new Vector3f(1, 1, 1);

            Assert.AreEqual(3, v1.LengthSquared, epsilonError);
        }
        #endregion


        #region Equals, Comparable
        [TestMethod]
        public void TestEqual01()
        {
            Vector3f v1 = new Vector3f(1, 2, 3);
            Vector3f v2 = new Vector3f(1, 2, 3);
            Vector3f v3 = new Vector3f(3, 2, 1);

            Assert.AreEqual(v1, v2);
            Assert.IsTrue(v1.Equals(v2));
            Assert.IsTrue(v2.Equals(v1));
            Assert.IsTrue(v1 == v2);
            Assert.IsFalse(v1 != v2);

            Assert.AreNotEqual(v1, v3);
            Assert.IsFalse(v1.Equals(v3));
            Assert.IsFalse(v3.Equals(v1));
            Assert.IsFalse(v1 == v3);
            Assert.IsTrue(v1 != v3);
        }

        [TestMethod]
        public void TestEqual02()
        {
            Vector3f v1 = new Vector3f(1, 2, 3);
            Object dummy = new object();
            Assert.AreNotEqual(v1, dummy);
            Assert.IsFalse(v1.Equals(dummy));
            Assert.IsFalse(dummy.Equals(v1));
        }

        [TestMethod]
        public void TestGetHashCode01()
        {
            Vector3f v1 = new Vector3f(1, 2, 3);
            Vector3f v2 = new Vector3f(1, 2, 3);
            Vector3f v3 = new Vector3f(5, 6, 7);

            Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
            Assert.AreNotEqual(v1.GetHashCode(), v3.GetHashCode());
        }

        [TestMethod]
        public void TestCompare01()
        {
            Vector3f v1 = new Vector3f(1, 2, 3);
            Vector3f v2 = new Vector3f(1, 2, 3);
            Vector3f v3 = new Vector3f(5, 6, 7);

            Assert.IsFalse(v1 > v2);
            Assert.IsFalse(v2 > v1);
            Assert.IsFalse(v1 > v3);
            Assert.IsTrue(v3 > v1);
            Assert.IsTrue(v2 >= v1);
            Assert.IsTrue(v1 >= v2);

            Assert.IsTrue(v1 <= v2);
            Assert.IsTrue(v2 <= v1);
            Assert.IsTrue(v1 <= v3);
            Assert.IsFalse(v3 <= v1);
            Assert.IsFalse(v2 < v1);
            Assert.IsFalse(v1 < v2);
        }
        #endregion

        #region Misc
        [TestMethod]
        public void TestToString()
        {
            Vector3f v1 = new Vector3f(1, 2, 3);
            string str = v1.ToString();
            Assert.AreEqual("(1, 2, 3)", str);
        }
        #endregion

        #region Normalize
        [TestMethod]
        public void TestInstanceNormalize01()
        {
            Vector3f v1 = new Vector3f(1, 0, 0);
            v1.Normalize();

            Assert.AreEqual(1, v1.Length, epsilonError);
            Assert.AreEqual(1, v1.X, epsilonError);
            Assert.AreEqual(0, v1.Y, epsilonError);
            Assert.AreEqual(0, v1.Z, epsilonError);

            Vector3f v2 = new Vector3f(0, 1, 0);
            v2.Normalize();

            Assert.AreEqual(1, v2.Length, epsilonError);
            Assert.AreEqual(0, v2.X, epsilonError);
            Assert.AreEqual(1, v2.Y, epsilonError);
            Assert.AreEqual(0, v2.Z, epsilonError);

            Vector3f v3 = new Vector3f(0, 0, 1);
            v3.Normalize();

            Assert.AreEqual(1, v3.Length, epsilonError);
            Assert.AreEqual(0, v3.X, epsilonError);
            Assert.AreEqual(0, v3.Y, epsilonError);
            Assert.AreEqual(1, v3.Z, epsilonError);
        }

        [TestMethod]
        public void TestInstanceNormalize02()
        {
            Vector3f v1 = new Vector3f(100, 0, 0);
            v1.Normalize();

            Assert.AreEqual(1, v1.Length, epsilonError);
            Assert.AreEqual(1, v1.X, epsilonError);
            Assert.AreEqual(0, v1.Y, epsilonError);
            Assert.AreEqual(0, v1.Z, epsilonError);

            Vector3f v2 = new Vector3f(0, 100, 0);
            v2.Normalize();

            Assert.AreEqual(1, v2.Length, epsilonError);
            Assert.AreEqual(0, v2.X, epsilonError);
            Assert.AreEqual(1, v2.Y, epsilonError);
            Assert.AreEqual(0, v2.Z, epsilonError);

            Vector3f v3 = new Vector3f(0, 0, 100);
            v3.Normalize();

            Assert.AreEqual(1, v3.Length, epsilonError);
            Assert.AreEqual(0, v3.X, epsilonError);
            Assert.AreEqual(0, v3.Y, epsilonError);
            Assert.AreEqual(1, v3.Z, epsilonError);
        }

        [TestMethod]
        public void TestInstanceNormalize03()
        {
            Vector3f v1 = new Vector3f(0, 0, 0);
            v1.Normalize();

            Assert.AreEqual(float.NaN, v1.X);
            Assert.AreEqual(float.NaN, v1.Y);
            Assert.AreEqual(float.NaN, v1.Z);
        }

        public void TestStaticNormalize01()
        {
            Vector3f org1 = new Vector3f(1, 0, 0);
            Vector3f v1 = Vector3f.Normalize(org1);

            Assert.AreEqual(1, v1.Length, epsilonError);
            Assert.AreEqual(1, v1.X, epsilonError);
            Assert.AreEqual(0, v1.Y, epsilonError);
            Assert.AreEqual(0, v1.Z, epsilonError);

            Vector3f org2 = new Vector3f(0, 1, 0);
            Vector3f v2 = Vector3f.Normalize(org2);

            Assert.AreEqual(1, v2.Length, epsilonError);
            Assert.AreEqual(0, v2.X, epsilonError);
            Assert.AreEqual(1, v2.Y, epsilonError);
            Assert.AreEqual(0, v2.Z, epsilonError);

            Vector3f org3 = new Vector3f(0, 0, 1);
            Vector3f v3 = Vector3f.Normalize(org3);

            Assert.AreEqual(1, v3.Length, epsilonError);
            Assert.AreEqual(0, v3.X, epsilonError);
            Assert.AreEqual(0, v3.Y, epsilonError);
            Assert.AreEqual(1, v3.Z, epsilonError);
        }

        [TestMethod]
        public void TestStaticNormalize02()
        {
            Vector3f org1 = new Vector3f(100, 0, 0);
            Vector3f v1 = Vector3f.Normalize(org1);

            Assert.AreEqual(1, v1.Length, epsilonError);
            Assert.AreEqual(1, v1.X, epsilonError);
            Assert.AreEqual(0, v1.Y, epsilonError);
            Assert.AreEqual(0, v1.Z, epsilonError);
            // the original is not modified
            Assert.AreEqual(100, org1.X, epsilonError);
            Assert.AreEqual(0, org1.Y, epsilonError);
            Assert.AreEqual(0, org1.Z, epsilonError);

            Vector3f org2 = new Vector3f(0, 100, 0);
            Vector3f v2 = Vector3f.Normalize(org2);

            Assert.AreEqual(1, v2.Length, epsilonError);
            Assert.AreEqual(0, v2.X, epsilonError);
            Assert.AreEqual(1, v2.Y, epsilonError);
            Assert.AreEqual(0, v2.Z, epsilonError);
            // the original is not modified
            Assert.AreEqual(0, org2.X, epsilonError);
            Assert.AreEqual(100, org2.Y, epsilonError);
            Assert.AreEqual(0, org2.Z, epsilonError);

            Vector3f org3 = new Vector3f(0, 0, 100);
            Vector3f v3 = Vector3f.Normalize(org3);

            Assert.AreEqual(1, v3.Length, epsilonError);
            Assert.AreEqual(0, v3.X, epsilonError);
            Assert.AreEqual(0, v3.Y, epsilonError);
            Assert.AreEqual(1, v3.Z, epsilonError);
            // the original is not modified
            Assert.AreEqual(0, org3.X, epsilonError);
            Assert.AreEqual(0, org3.Y, epsilonError);
            Assert.AreEqual(100, org3.Z, epsilonError);
        }

        [TestMethod]
        public void TestStaticNormalize03()
        {
            Vector3f org1 = new Vector3f(0, 0, 0);
            Vector3f v1 = Vector3f.Normalize(org1);

            Assert.AreEqual(float.NaN, v1.X);
            Assert.AreEqual(float.NaN, v1.Y);
            Assert.AreEqual(float.NaN, v1.Z);
        }

        public void TestStaticNormalize04()
        {
            Vector3f org1 = new Vector3f(1, 0, 0);
            Vector3f v1;
            Vector3f.Normalize(ref org1, out v1);

            Assert.AreEqual(1, v1.Length, epsilonError);
            Assert.AreEqual(1, v1.X, epsilonError);
            Assert.AreEqual(0, v1.Y, epsilonError);
            Assert.AreEqual(0, v1.Z, epsilonError);

            Vector3f org2 = new Vector3f(0, 1, 0);
            Vector3f v2;
            Vector3f.Normalize(ref org2, out v2);

            Assert.AreEqual(1, v2.Length, epsilonError);
            Assert.AreEqual(0, v2.X, epsilonError);
            Assert.AreEqual(1, v2.Y, epsilonError);
            Assert.AreEqual(0, v2.Z, epsilonError);

            Vector3f org3 = new Vector3f(0, 0, 1);
            Vector3f v3;
            Vector3f.Normalize(ref org3, out v3);

            Assert.AreEqual(1, v3.Length, epsilonError);
            Assert.AreEqual(0, v3.X, epsilonError);
            Assert.AreEqual(0, v3.Y, epsilonError);
            Assert.AreEqual(1, v3.Z, epsilonError);
        }

        [TestMethod]
        public void TestStaticNormalize05()
        {
            Vector3f org1 = new Vector3f(100, 0, 0);
            Vector3f v1;
            Vector3f.Normalize(ref org1, out v1);

            Assert.AreEqual(1, v1.Length, epsilonError);
            Assert.AreEqual(1, v1.X, epsilonError);
            Assert.AreEqual(0, v1.Y, epsilonError);
            Assert.AreEqual(0, v1.Z, epsilonError);
            // the original is not modified
            Assert.AreEqual(100, org1.X, epsilonError);
            Assert.AreEqual(0, org1.Y, epsilonError);
            Assert.AreEqual(0, org1.Z, epsilonError);

            Vector3f org2 = new Vector3f(0, 100, 0);
            Vector3f v2;
            Vector3f.Normalize(ref org2, out v2);

            Assert.AreEqual(1, v2.Length, epsilonError);
            Assert.AreEqual(0, v2.X, epsilonError);
            Assert.AreEqual(1, v2.Y, epsilonError);
            Assert.AreEqual(0, v2.Z, epsilonError);
            // the original is not modified
            Assert.AreEqual(0, org2.X, epsilonError);
            Assert.AreEqual(100, org2.Y, epsilonError);
            Assert.AreEqual(0, org2.Z, epsilonError);

            Vector3f org3 = new Vector3f(0, 0, 100);
            Vector3f v3;
            Vector3f.Normalize(ref org3, out v3);

            Assert.AreEqual(1, v3.Length, epsilonError);
            Assert.AreEqual(0, v3.X, epsilonError);
            Assert.AreEqual(0, v3.Y, epsilonError);
            Assert.AreEqual(1, v3.Z, epsilonError);
            // the original is not modified
            Assert.AreEqual(0, org3.X, epsilonError);
            Assert.AreEqual(0, org3.Y, epsilonError);
            Assert.AreEqual(100, org3.Z, epsilonError);
        }

        [TestMethod]
        public void TestStaticNormalize06()
        {
            Vector3f org1 = new Vector3f(0, 0, 0);
            Vector3f v1;
            Vector3f.Normalize(ref org1, out v1);

            Assert.AreEqual(float.NaN, v1.X);
            Assert.AreEqual(float.NaN, v1.Y);
            Assert.AreEqual(float.NaN, v1.Z);
        }
        #endregion

        #region Add
        [TestMethod]
        public void TestAddStaticOperator()
        {
            Vector3f v1 = new Vector3f(1, 2, 3);
            Vector3f v2 = new Vector3f(4, 3, 2);
            Vector3f v3 = new Vector3f(5, 6, 7);

            Vector3f rst1 = v1 + v2;
            Vector3f rst2 = v1 + v3;
            Vector3f rst3 = v2 + v3;

            Assert.AreEqual(5, rst1.X, epsilonError);
            Assert.AreEqual(5, rst1.Y, epsilonError);
            Assert.AreEqual(5, rst1.Z, epsilonError);

            Assert.AreEqual(6, rst2.X, epsilonError);
            Assert.AreEqual(8, rst2.Y, epsilonError);
            Assert.AreEqual(10, rst2.Z, epsilonError);

            Assert.AreEqual(9, rst3.X, epsilonError);
            Assert.AreEqual(9, rst3.Y, epsilonError);
            Assert.AreEqual(9, rst3.Z, epsilonError);

            Assert.AreEqual(1, v1.X, epsilonError);
            Assert.AreEqual(2, v1.Y, epsilonError);
            Assert.AreEqual(3, v1.Z, epsilonError);

            Assert.AreEqual(4, v2.X, epsilonError);
            Assert.AreEqual(3, v2.Y, epsilonError);
            Assert.AreEqual(2, v2.Z, epsilonError);

            Assert.AreEqual(5, v3.X, epsilonError);
            Assert.AreEqual(6, v3.Y, epsilonError);
            Assert.AreEqual(7, v3.Z, epsilonError);
        }

        [TestMethod]
        public void TestAddStaticMethod01()
        {
            Vector3f v1 = new Vector3f(1, 2, 3);
            Vector3f v2 = new Vector3f(4, 3, 2);
            Vector3f v3 = new Vector3f(5, 6, 7);

            Vector3f rst1 = Vector3f.Add(v1, v2);
            Vector3f rst2 = Vector3f.Add(v1, v3);
            Vector3f rst3 = Vector3f.Add(v2, v3);

            Assert.AreEqual(5, rst1.X, epsilonError);
            Assert.AreEqual(5, rst1.Y, epsilonError);
            Assert.AreEqual(5, rst1.Z, epsilonError);

            Assert.AreEqual(6, rst2.X, epsilonError);
            Assert.AreEqual(8, rst2.Y, epsilonError);
            Assert.AreEqual(10, rst2.Z, epsilonError);

            Assert.AreEqual(9, rst3.X, epsilonError);
            Assert.AreEqual(9, rst3.Y, epsilonError);
            Assert.AreEqual(9, rst3.Z, epsilonError);

            Assert.AreEqual(1, v1.X, epsilonError);
            Assert.AreEqual(2, v1.Y, epsilonError);
            Assert.AreEqual(3, v1.Z, epsilonError);

            Assert.AreEqual(4, v2.X, epsilonError);
            Assert.AreEqual(3, v2.Y, epsilonError);
            Assert.AreEqual(2, v2.Z, epsilonError);

            Assert.AreEqual(5, v3.X, epsilonError);
            Assert.AreEqual(6, v3.Y, epsilonError);
            Assert.AreEqual(7, v3.Z, epsilonError);
        }

        [TestMethod]
        public void TestAddStaticMethod02()
        {
            Vector3f v1 = new Vector3f(1, 2, 3);
            Vector3f v2 = new Vector3f(4, 3, 2);
            Vector3f v3 = new Vector3f(5, 6, 7);

            Vector3f rst1, rst2, rst3;
            Vector3f.Add(ref v1, ref v2, out rst1);
            Vector3f.Add(ref v1, ref v3, out rst2);
            Vector3f.Add(ref v2, ref v3, out rst3);

            Assert.AreEqual(5, rst1.X, epsilonError);
            Assert.AreEqual(5, rst1.Y, epsilonError);
            Assert.AreEqual(5, rst1.Z, epsilonError);

            Assert.AreEqual(6, rst2.X, epsilonError);
            Assert.AreEqual(8, rst2.Y, epsilonError);
            Assert.AreEqual(10, rst2.Z, epsilonError);

            Assert.AreEqual(9, rst3.X, epsilonError);
            Assert.AreEqual(9, rst3.Y, epsilonError);
            Assert.AreEqual(9, rst3.Z, epsilonError);

            Assert.AreEqual(1, v1.X, epsilonError);
            Assert.AreEqual(2, v1.Y, epsilonError);
            Assert.AreEqual(3, v1.Z, epsilonError);

            Assert.AreEqual(4, v2.X, epsilonError);
            Assert.AreEqual(3, v2.Y, epsilonError);
            Assert.AreEqual(2, v2.Z, epsilonError);

            Assert.AreEqual(5, v3.X, epsilonError);
            Assert.AreEqual(6, v3.Y, epsilonError);
            Assert.AreEqual(7, v3.Z, epsilonError);
        }

        [TestMethod]
        public void TestAddInstanceMethod01()
        {
            Vector3f v1 = new Vector3f(1, 2, 3);
            Vector3f v2 = new Vector3f(4, 3, 2);
            Vector3f v3 = new Vector3f(5, 6, 7);

            v1.Add(v2);

            Assert.AreEqual(4, v2.X, epsilonError);
            Assert.AreEqual(3, v2.Y, epsilonError);
            Assert.AreEqual(2, v2.Z, epsilonError);

            v2.Add(v3);

            Assert.AreEqual(5, v1.X, epsilonError);
            Assert.AreEqual(5, v1.Y, epsilonError);
            Assert.AreEqual(5, v1.Z, epsilonError);

            Assert.AreEqual(9, v2.X, epsilonError);
            Assert.AreEqual(9, v2.Y, epsilonError);
            Assert.AreEqual(9, v2.Z, epsilonError);

            Assert.AreEqual(5, v3.X, epsilonError);
            Assert.AreEqual(6, v3.Y, epsilonError);
            Assert.AreEqual(7, v3.Z, epsilonError);
        }
        [TestMethod]
        public void TestAddInstanceMethod02()
        {
            Vector3f v1 = new Vector3f(1, 2, 3);
            Vector3f v2 = new Vector3f(4, 3, 2);
            Vector3f v3 = new Vector3f(5, 6, 7);

            v1.Add(ref v2);

            Assert.AreEqual(4, v2.X, epsilonError);
            Assert.AreEqual(3, v2.Y, epsilonError);
            Assert.AreEqual(2, v2.Z, epsilonError);

            v2.Add(ref v3);

            Assert.AreEqual(5, v1.X, epsilonError);
            Assert.AreEqual(5, v1.Y, epsilonError);
            Assert.AreEqual(5, v1.Z, epsilonError);

            Assert.AreEqual(9, v2.X, epsilonError);
            Assert.AreEqual(9, v2.Y, epsilonError);
            Assert.AreEqual(9, v2.Z, epsilonError);

            Assert.AreEqual(5, v3.X, epsilonError);
            Assert.AreEqual(6, v3.Y, epsilonError);
            Assert.AreEqual(7, v3.Z, epsilonError);
        }
        #endregion

        #region Sub
        [TestMethod]
        public void TestSubStaticOperator()
        {
            Vector3f v1 = new Vector3f(1, 2, 3);
            Vector3f v2 = new Vector3f(4, 3, 2);
            Vector3f v3 = new Vector3f(5, 6, 7);

            Vector3f rst1 = v1 - v2;
            Vector3f rst2 = v1 - v3;
            Vector3f rst3 = v2 - v3;

            Assert.AreEqual(-3, rst1.X, epsilonError);
            Assert.AreEqual(-1, rst1.Y, epsilonError);
            Assert.AreEqual(1, rst1.Z, epsilonError);

            Assert.AreEqual(-4, rst2.X, epsilonError);
            Assert.AreEqual(-4, rst2.Y, epsilonError);
            Assert.AreEqual(-4, rst2.Z, epsilonError);

            Assert.AreEqual(-1, rst3.X, epsilonError);
            Assert.AreEqual(-3, rst3.Y, epsilonError);
            Assert.AreEqual(-5, rst3.Z, epsilonError);

            Assert.AreEqual(1, v1.X, epsilonError);
            Assert.AreEqual(2, v1.Y, epsilonError);
            Assert.AreEqual(3, v1.Z, epsilonError);

            Assert.AreEqual(4, v2.X, epsilonError);
            Assert.AreEqual(3, v2.Y, epsilonError);
            Assert.AreEqual(2, v2.Z, epsilonError);

            Assert.AreEqual(5, v3.X, epsilonError);
            Assert.AreEqual(6, v3.Y, epsilonError);
            Assert.AreEqual(7, v3.Z, epsilonError);
        }

        [TestMethod]
        public void TestSubStaticMethod01()
        {
            Vector3f v1 = new Vector3f(1, 2, 3);
            Vector3f v2 = new Vector3f(4, 3, 2);
            Vector3f v3 = new Vector3f(5, 6, 7);

            Vector3f rst1 = Vector3f.Sub(v1, v2);
            Vector3f rst2 = Vector3f.Sub(v1, v3);
            Vector3f rst3 = Vector3f.Sub(v2, v3);

            Assert.AreEqual(-3, rst1.X, epsilonError);
            Assert.AreEqual(-1, rst1.Y, epsilonError);
            Assert.AreEqual(1, rst1.Z, epsilonError);

            Assert.AreEqual(-4, rst2.X, epsilonError);
            Assert.AreEqual(-4, rst2.Y, epsilonError);
            Assert.AreEqual(-4, rst2.Z, epsilonError);

            Assert.AreEqual(-1, rst3.X, epsilonError);
            Assert.AreEqual(-3, rst3.Y, epsilonError);
            Assert.AreEqual(-5, rst3.Z, epsilonError);

            Assert.AreEqual(1, v1.X, epsilonError);
            Assert.AreEqual(2, v1.Y, epsilonError);
            Assert.AreEqual(3, v1.Z, epsilonError);

            Assert.AreEqual(4, v2.X, epsilonError);
            Assert.AreEqual(3, v2.Y, epsilonError);
            Assert.AreEqual(2, v2.Z, epsilonError);

            Assert.AreEqual(5, v3.X, epsilonError);
            Assert.AreEqual(6, v3.Y, epsilonError);
            Assert.AreEqual(7, v3.Z, epsilonError);
        }

        [TestMethod]
        public void TestSubStaticMethod02()
        {
            Vector3f v1 = new Vector3f(1, 2, 3);
            Vector3f v2 = new Vector3f(4, 3, 2);
            Vector3f v3 = new Vector3f(5, 6, 7);

            Vector3f rst1, rst2, rst3;
            Vector3f.Sub(ref v1, ref v2, out rst1);
            Vector3f.Sub(ref v1, ref v3, out rst2);
            Vector3f.Sub(ref v2, ref v3, out rst3);


            Assert.AreEqual(-3, rst1.X, epsilonError);
            Assert.AreEqual(-1, rst1.Y, epsilonError);
            Assert.AreEqual(1, rst1.Z, epsilonError);

            Assert.AreEqual(-4, rst2.X, epsilonError);
            Assert.AreEqual(-4, rst2.Y, epsilonError);
            Assert.AreEqual(-4, rst2.Z, epsilonError);

            Assert.AreEqual(-1, rst3.X, epsilonError);
            Assert.AreEqual(-3, rst3.Y, epsilonError);
            Assert.AreEqual(-5, rst3.Z, epsilonError);

            Assert.AreEqual(1, v1.X, epsilonError);
            Assert.AreEqual(2, v1.Y, epsilonError);
            Assert.AreEqual(3, v1.Z, epsilonError);

            Assert.AreEqual(4, v2.X, epsilonError);
            Assert.AreEqual(3, v2.Y, epsilonError);
            Assert.AreEqual(2, v2.Z, epsilonError);

            Assert.AreEqual(5, v3.X, epsilonError);
            Assert.AreEqual(6, v3.Y, epsilonError);
            Assert.AreEqual(7, v3.Z, epsilonError);
        }

        [TestMethod]
        public void TestSubInstanceMethod()
        {
            Vector3f q1 = new Vector3f(1, 2, 3);
            Vector3f q2 = new Vector3f(4, 3, 2);
            Vector3f q3 = new Vector3f(5, 6, 7);

            q1.Sub(q2);

            Assert.AreEqual(4, q2.X, epsilonError);
            Assert.AreEqual(3, q2.Y, epsilonError);
            Assert.AreEqual(2, q2.Z, epsilonError);

            q2.Sub(q3);

            Assert.AreEqual(-3, q1.X, epsilonError);
            Assert.AreEqual(-1, q1.Y, epsilonError);
            Assert.AreEqual(1, q1.Z, epsilonError);

            Assert.AreEqual(-1, q2.X, epsilonError);
            Assert.AreEqual(-3, q2.Y, epsilonError);
            Assert.AreEqual(-5, q2.Z, epsilonError);

            Assert.AreEqual(5, q3.X, epsilonError);
            Assert.AreEqual(6, q3.Y, epsilonError);
            Assert.AreEqual(7, q3.Z, epsilonError);
        }
        #endregion

        #region Mul
        [TestMethod]
        public void TestMulStaticOperator()
        {
            Vector3f v1 = new Vector3f(1, 2, 3);

            Vector3f rst1 = v1 * 2;
            Vector3f rst2 = 2 * v1;

            Assert.AreEqual(2, rst1.X, epsilonError);
            Assert.AreEqual(4, rst1.Y, epsilonError);
            Assert.AreEqual(6, rst1.Z, epsilonError);

            Assert.AreEqual(2, rst2.X, epsilonError);
            Assert.AreEqual(4, rst2.Y, epsilonError);
            Assert.AreEqual(6, rst2.Z, epsilonError);

            Assert.AreEqual(1, v1.X, epsilonError);
            Assert.AreEqual(2, v1.Y, epsilonError);
            Assert.AreEqual(3, v1.Z, epsilonError);
        }

        [TestMethod]
        public void TestMulStaticMethod01()
        {
            Vector3f q1 = new Vector3f(1, 2, 3);

            Vector3f rst1 = Vector3f.Multiply(q1, 2);

            Assert.AreEqual(2, rst1.X, epsilonError);
            Assert.AreEqual(4, rst1.Y, epsilonError);
            Assert.AreEqual(6, rst1.Z, epsilonError);

            Assert.AreEqual(1, q1.X, epsilonError);
            Assert.AreEqual(2, q1.Y, epsilonError);
            Assert.AreEqual(3, q1.Z, epsilonError);
        }

        [TestMethod]
        public void TestMulStaticMethod02()
        {
            Vector3f q1 = new Vector3f(1, 2, 3);

            Vector3f rst1;
            Vector3f.Multiply(ref q1, 2, out rst1);

            Assert.AreEqual(2, rst1.X, epsilonError);
            Assert.AreEqual(4, rst1.Y, epsilonError);
            Assert.AreEqual(6, rst1.Z, epsilonError);

            Assert.AreEqual(1, q1.X, epsilonError);
            Assert.AreEqual(2, q1.Y, epsilonError);
            Assert.AreEqual(3, q1.Z, epsilonError);
        }

        #endregion

        #region Mix Vector and Matrix
        [TestMethod]
        public void TestLookAt()
        {
            Vector3f pos = new Vector3f(1, 2, 3);
            Vector3f dir = new Vector3f(0, 0, 0);
            Vector3f up = new Vector3f(0, 1, 0);

            Vector3f forward = dir - pos;

            Assert.AreEqual(1, pos.X, epsilonError);
            Assert.AreEqual(2, pos.Y, epsilonError);
            Assert.AreEqual(3, pos.Z, epsilonError);

            Assert.AreEqual(0, dir.X, epsilonError);
            Assert.AreEqual(0, dir.Y, epsilonError);
            Assert.AreEqual(0, dir.Z, epsilonError);

            Assert.AreEqual(-1, forward.X, epsilonError);
            Assert.AreEqual(-2, forward.Y, epsilonError);
            Assert.AreEqual(-3, forward.Z, epsilonError);

            forward.Normalize();
            Matrix4f viewMatrix = Matrix4f.LookAt(pos, dir, up);

            float angle = (float)Vector3f.CalculateAngle(forward, up);

            float dot = Vector3f.Dot(Vector3f.UnitZ, forward);

            Quaternion rst2;

            if (Math.Abs(dot - (-1.0f)) < 0.000001f)
            {
                rst2 = new Quaternion(Vector3f.UnitY.X, Vector3f.UnitY.Y, Vector3f.UnitY.Z, 3.1415926535897932f);
            }
            if (Math.Abs(dot - (1.0f)) < 0.000001f)
            {
                rst2 = Quaternion.Identity;
            }

            float rotAngle = (float)Math.Acos(dot);
            Vector3f rotAxis = Vector3f.Cross(Vector3f.UnitZ, forward);
            rotAxis = Vector3f.Normalize(rotAxis);

            rst2 = Quaternion.FromAxisAngle(rotAxis, rotAngle);

            Matrix4f viewMatrix2 = Quaternion.ToMatrix4f(rst2);
        }
        #endregion
    }
    }
