using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Tapako.DeviceInformationManagement.InformationSources;
using Akomi.InformationModel;
using Akomi.InformationModel.Component;
using Akomi.InformationModel.Component.Documentation;
using Akomi.InformationModel.Component.Identification;
using Akomi.InformationModel.Component.ManufacturingData;
using Akomi.InformationModel.Component.PhysicalDescription;
using Akomi.InformationModel.Component.Presentation;
using Akomi.InformationModel.Component.State;
using Akomi.InformationModel.Component.TradingData;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Device.Description;
using Akomi.InformationModel.Device.Logic;
using Akomi.InformationModel.Device.Safety;
using Akomi.InformationModel.Device.Security;
using System.Collections.Generic;

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

        public class DummyDeviceCompletement : DeviceCompletementBase
        {
            
            protected override void CompletePorts(IPortList portList)
            {
                throw new NotImplementedException();
            }

            protected override void CompleteDescription(IDeviceDescription deviceDescription)
            {
                throw new NotImplementedException();
            }

            protected override void CompleteDocumentation(IDocumentation documentation)
            {
                throw new NotImplementedException();
            }

            protected override void CompleteIdentification(IIdentification identification)
            {
                throw new NotImplementedException();
            }

            protected override void CompleteLogic(ILogicContainer logicContainer)
            {
                throw new NotImplementedException();
            }

            protected override void CompleteManufacturingData(IManufacturingData productionData)
            {
                throw new NotImplementedException();
            }

            protected override void CompleteParametrization(IDevice device)
            {
                throw new NotImplementedException();
            }

            protected override void CompletePhysicalDescription(IPhysicalDescription physicalDescription)
            {
                throw new NotImplementedException();
            }

            protected override void CompletePresentationData(IPresentationData presentationData)
            {
                throw new NotImplementedException();
            }

            protected override void CompleteSafety(ISafety safety)
            {
                throw new NotImplementedException();
            }

            protected override void CompleteSecurity(ISecurity security)
            {
                throw new NotImplementedException();
            }

            protected override void CompleteSkills(ISkillList skillList, IComponent context)
            {
                throw new NotImplementedException();
            }

            protected override void CompleteState(IState state)
            {
                throw new NotImplementedException();
            }

            protected override void CompleteSubdevices(IList<IDevice> subdevices)
            {
                throw new NotImplementedException();
            }

            protected override void CompleteTradingData(ITradingData tradingData)
            {
                throw new NotImplementedException();
            }

           
        }
    }
}
