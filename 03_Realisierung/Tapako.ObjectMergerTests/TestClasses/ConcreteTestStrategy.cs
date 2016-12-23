using System.Collections.Generic;
using Akomi.InformationModel.Component;
using Akomi.InformationModel.Component.Identification;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Skills.SkillCatalogue;
using Tapako.Framework.ExtensionMethods;

namespace Tapako.ObjectMergerTests.TestClasses
{
    public class ConcreteTestStrategy : SkillSearchForSubdevicesBase //Strategy<IList<IDevice>>
    {
        public static string BrowseNameResult = "ConcreteTestStrategy Run";


        protected IList<IDevice> InnerExecute()
        {
            //IDevice dev = InputArguments[ArgumentKeywords.ParentObject.ToString()] as IDevice;
            IDevice dev = ((IDevice) Context);

            //if (dev.Identification == null)
            //{
            //    dev.Identification = new DeviceIdentification();
            //}
            //dev.Identification.BrowseName = BrowseNameResult;

            if (dev.Identification == null)
            {
                dev.Identification = new Identification();
            }
            dev.SetBrowseName(BrowseNameResult);

            return null;
        }

        public ConcreteTestStrategy(IComponent context) : base(context)
        {
        }

        public override void Calculate()
        {
            throw new System.NotImplementedException();
        }

        public override void Execute()
        {
            InnerExecute();
        }

        public override void SetupNext()
        {
            throw new System.NotImplementedException();
        }

        public override void SetupBack()
        {
            throw new System.NotImplementedException();
        }

        public override void SetupCancel()
        {
            throw new System.NotImplementedException();
        }

        public override void SetupCreateInstance()
        {
            throw new System.NotImplementedException();
        }

        public override void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
    public class ConcreteTestStrategy2 : SkillSearchForSubdevicesBase// Strategy<IList<IDevice>>
    {
        public static string BrowseNameResult = "ConcreteTestStrategy 2 Run";


        protected IList<IDevice> InnerExecute()
        {
            //IDevice dev = InputArguments[ArgumentKeywords.ParentObject.ToString()] as IDevice;
            IDevice dev = (IDevice)Context;
            dev.SetBrowseName(BrowseNameResult);

            return null;
        }

        public ConcreteTestStrategy2(IComponent context) : base(context)
        {
        }

        public override void Calculate()
        {
            throw new System.NotImplementedException();
        }

        public override void Execute()
        {
            InnerExecute();
        }

        public override void SetupNext()
        {
            throw new System.NotImplementedException();
        }

        public override void SetupBack()
        {
            throw new System.NotImplementedException();
        }

        public override void SetupCancel()
        {
            throw new System.NotImplementedException();
        }

        public override void SetupCreateInstance()
        {
            throw new System.NotImplementedException();
        }

        public override void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}
