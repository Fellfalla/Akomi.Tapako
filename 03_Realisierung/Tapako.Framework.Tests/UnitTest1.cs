using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Tapako.Framework.Tests
{
    [TestClass]
    public class DllLoaderTests
    {
        [TestMethod]
        public void CanLoadInterfaceClass()
        {
            var instance = DllLoader.LoadClass<TestInterface>(Assembly.GetExecutingAssembly());

            Assert.IsNotNull(instance);
            Assert.AreEqual(1, instance.TestValue);
        }


        [TestMethod]
        public void CanLoadInterfaceClassWithAbstractParent()
        {
            var instance = DllLoader.LoadClass<TestInterface2>(Assembly.GetExecutingAssembly());

            Assert.IsNotNull(instance);
            Assert.AreEqual(1, instance.TestValue);
        }


        public interface TestInterface
        {
            int TestValue { get; }
        }

        public class TestClass : TestInterface
        {
            public int TestValue => 1;

        }


        public interface TestInterface2
        {
            int TestValue { get; }
        }

        public abstract class TestClass2Base : TestInterface2
        {
            public abstract int TestValue { get; }
        }

        public class TestClass2 : TestInterface2
        {
            public int TestValue => 1;
        }
    }
}
