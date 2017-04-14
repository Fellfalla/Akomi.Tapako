using System;
using System.Collections.Generic;
using Akomi.InformationModel;
using Akomi.InformationModel.Component;
using Akomi.InformationModel.Component.Documentation;
using Akomi.InformationModel.Component.Identification;
using Akomi.InformationModel.Component.PhysicalDescription;
using Akomi.InformationModel.Component.Presentation;
using Akomi.InformationModel.Component.ManufacturingData;
using Akomi.InformationModel.Component.State;
using Akomi.InformationModel.Component.TradingData;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Device.Description;
using Akomi.InformationModel.Device.Logic;
using Akomi.InformationModel.Device.Safety;
using Akomi.InformationModel.Device.Security;
using Akomi.Logger;
using Akomi.InformationModel.Skills;
using Akomi.InformationModel.Component.ComponentPort;
using Akomi.InformationModel.Device.Parametrization;

namespace Tapako.DeviceInformationManagement.InformationSources
{
    public abstract class DeviceCompletementBase : IDeviceCompletement
    {
        delegate void CompletionInvocation(ref IDevice device);

        public virtual IDevice CompleteDeviceDriver(ref IDevice deviceRoot)
        {
            var a = new CompletionInvocation[] {
                (ref IDevice device) => CompleteSkills                      (ref device),
                (ref IDevice device) => CompleteDescription                 (ref device),
                (ref IDevice device) => CompleteIdentification              (ref device),
                (ref IDevice device) => CompleteSecurity                    (ref device),
                (ref IDevice device) => CompletePorts                       (ref device),
                (ref IDevice device) => CompletePresentationData            (ref device),
                (ref IDevice device) => CompleteDocumentation               (ref device),
                (ref IDevice device) => CompletePhysicalDescription         (ref device),
                (ref IDevice device) => CompleteSafety                      (ref device),
                (ref IDevice device) => CompleteState                       (ref device),
                (ref IDevice device) => CompleteSubdevices                  (ref device),
                (ref IDevice device) => CompleteLogic                       (ref device),
                (ref IDevice device) => CompleteManufacturingData           (ref device),
                (ref IDevice device) => CompleteTradingData                 (ref device),
                (ref IDevice device) => CompleteParametrization             (ref device)
            };

            foreach (var action in a)
            {
                try
                {
                    action.Invoke(ref deviceRoot);
                }
                catch (NotImplementedException e)
                {
                    Logger.Warning("Completion not implemented in {0}:\n{1}", deviceRoot, e.ToString());
                }
            }

            return deviceRoot;
        }

        private TIn InstaciateIfNull<TIn, TCreate>(TIn type)
            where TCreate : TIn, new()
        {

            if (type == null)
            {
                type = Activator.CreateInstance<TCreate>();
            }

            return type;
        }


        protected void CompleteIdentification        (ref IDevice device)
        {
            device.Identification = InstaciateIfNull<IIdentification, Identification>(device.Identification);
            CompleteIdentification      (device.Identification       );
        }
        protected void CompleteSkills                (ref IDevice device)
        {
            //device.Skills = InstaciateIfNull<ISkillList, SkillList>(device.Skills);
            CompleteSkills(device.Skills, device);
        }
        protected void CompletePresentationData      (ref IDevice device)
        {
            device.PresentationData = InstaciateIfNull<IPresentationData, PresentationData>(device.PresentationData);
            CompletePresentationData(device.PresentationData     );
        }
        protected void CompletePhysicalDescription   (ref IDevice device)
        {
            device.PhysicalDescription = InstaciateIfNull<IPhysicalDescription, PhysicalDescription>(device.PhysicalDescription);
            CompletePhysicalDescription(device.PhysicalDescription  );
        }
        protected void CompleteLogic                 (ref IDevice device)
        {
            CompleteLogic(device                      );
        }
        protected void CompleteTradingData           (ref IDevice device)
        {
            device.TradingData = InstaciateIfNull<ITradingData, TradingData>(device.TradingData);
            CompleteTradingData(device.TradingData          );
        }
        protected void CompleteDescription           (ref IDevice device)
        {
            device.Description = InstaciateIfNull<IDeviceDescription, DeviceDescription>(device.Description);
            CompleteDescription(device.Description          );
        }

        protected void CompleteDocumentation         (ref IDevice device)
        {
            device.Documentation = InstaciateIfNull<IDocumentation, Documentation>(device.Documentation);
            CompleteDocumentation(device.Documentation        );
        }
        protected void CompleteSecurity              (ref IDevice device)
        {
            device.Security= InstaciateIfNull<ISecurity, Security>(device.Security);
            CompleteSecurity(device.Security             );
        }
        protected void CompletePorts                 (ref IDevice device)
        {
            //device.Ports= InstaciateIfNull<IPortList, PortList>(device.Security);
            CompletePorts(device.Ports          );
        }
        protected void CompleteSafety                (ref IDevice device)
        {
            device.Safety = InstaciateIfNull<ISafety, Safety>(device.Safety);
            CompleteSafety(device.Safety);
        }
        protected void CompleteState                 (ref IDevice device)
        {
            device.State= InstaciateIfNull<IState, State>(device.State);
            CompleteState(device.State);
        }
        protected void CompleteSubdevices            (ref IDevice device)
        {
            device.SubDevices = InstaciateIfNull<IList<IDevice>, List<IDevice>>(device.SubDevices);
            CompleteSubdevices(device.SubDevices);
        }

        protected void CompleteManufacturingData     (ref IDevice device)
        {
            device.ManufacturingData= InstaciateIfNull<IManufacturingData, ManufacturingData>(device.ManufacturingData);
            CompleteManufacturingData(device.ManufacturingData);
        }
        protected void CompleteParametrization       (ref IDevice device)
        {
            device.Parametrization = InstaciateIfNull<Parametrization, Parametrization>(device.Parametrization);
            CompleteParametrization(device);}


        protected abstract void CompleteIdentification          (IIdentification        identification      );
        protected abstract void CompleteSkills                  (ISkillList             skillList           , IComponent context);
        protected abstract void CompletePresentationData        (IPresentationData      presentationData    );
        protected abstract void CompletePhysicalDescription     (IPhysicalDescription   physicalDescription );
        protected abstract void CompleteLogic                   (ILogicContainer        logicContainer      );
        protected abstract void CompleteTradingData             (ITradingData           tradingData         );
        protected abstract void CompleteDescription             (IDeviceDescription     deviceDescription   );
        protected abstract void CompleteDocumentation           (IDocumentation         documentation       );
        protected abstract void CompleteSecurity                (ISecurity              security            );
        protected abstract void CompletePorts                   (IPortList              portList            );
        protected abstract void CompleteSafety                  (ISafety                safety              );
        protected abstract void CompleteState                   (IState                 state               );
        protected abstract void CompleteSubdevices              (IList<IDevice>         subdevices          );
        protected abstract void CompleteManufacturingData          (IManufacturingData     productionData      );
        protected abstract void CompleteParametrization         (IDevice                device              );

    }

}

