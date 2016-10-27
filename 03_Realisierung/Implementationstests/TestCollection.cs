using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable IsExpressionAlwaysTrue
#pragma warning disable 184

namespace Implementationstests
{
    [TestClass]
    public class TestCollection
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsTrue(EnumTest.Three > EnumTest.One);
            Assert.IsTrue(EnumTest.Three > EnumTest.Two);


            Assert.IsTrue(EnumTest.Z > EnumTest.A);
            Assert.IsTrue(EnumTest.B > EnumTest.Z);
        }

        [TestMethod]
        public void TestSortList()
        {
            List<EnumTest> a = new List<EnumTest>() {EnumTest.Three, EnumTest.One, EnumTest.Two};
            List<EnumTest> expected = new List<EnumTest>() {EnumTest.One, EnumTest.Two, EnumTest.Three};
            List<EnumTest> notExpected = new List<EnumTest>() {EnumTest.Three, EnumTest.Two, EnumTest.One};

            a.Sort();
            Assert.IsTrue(expected.SequenceEqual(a));
            Assert.IsFalse(notExpected.SequenceEqual(a));
        }

        [TestMethod]
        public void ArrayIsEnumerable()
        {
            int[] a = new[] {1, 2, 3};
            int b = 5;

            Assert.IsTrue(a is IEnumerable);
            Assert.IsFalse(b is IEnumerable);
        }


    }


    class ComparerTest : IComparer<EnumTest>
    {
        public int Compare(EnumTest x, EnumTest y)
        {
            if (x < y)
            {
                return -1; // x ist kleiner als y
            }
            else if (x == y)
            {
                return 0; // x ist gleich y
            }
            else
            {
                return 1; // x ist größer als y
            }
        }
    }

    public enum EnumTest
    {
        One = 1,
        Two = 2,
        Three = 3,
        Z = 5,
        A = 3,
        B = 7
    }
}
