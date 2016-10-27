using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tapako.Utilities.WiringTool.Extensions;

namespace WiringToolTests
{
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void TestTupleContains()
        {
            TestContains(45.6, "hello");
            TestContains(new Testclass(), new Testclass());
            TestContains(new Testclass(), new object());
            TestContains(new object(), new Testclass());
            TestContains(new object(), new object());
        }

        private void TestContains <T1,T2> (T1 first, T2 second)
        {

            var tuple2 = Tuple.Create(first, second);
            Assert.IsTrue(tuple2.Contains(first, first));
            Assert.IsTrue(tuple2.Contains(second, first));
        }

        class Testclass { }
    }
}
