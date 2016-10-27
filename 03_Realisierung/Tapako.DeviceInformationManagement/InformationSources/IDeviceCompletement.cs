using System;
using System.Collections;
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

namespace Tapako.DeviceInformationManagement.InformationSources
{
    /// <summary>
    /// Implement this factory in the class of your driver, which is reponsible for creating the correct device object.
    /// </summary>
    public interface IDeviceCompletement
    {
        IDevice CompleteDeviceDriver(ref IDevice deviceRoot);
    }

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
                (ref IDevice device) => CompleteConnections                 (ref device),
                (ref IDevice device) => CompletePresentationData            (ref device),
                (ref IDevice device) => CompleteDocumentation               (ref device),
                (ref IDevice device) => CompletePhysicalDescription         (ref device),
                (ref IDevice device) => CompleteSafety                      (ref device),
                (ref IDevice device) => CompleteState                       (ref device),
                (ref IDevice device) => CompleteSubdevices                  (ref device),
                (ref IDevice device) => CompleteLogic                       (ref device),
                (ref IDevice device) => CompleteManufacturingData              (ref device),
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

        protected void CompleteIdentification        (ref IDevice device) {CompleteIdentification      (device.Identification       );}
        protected void CompleteSkills                (ref IDevice device) {CompleteSkills              (device.Skills, device       );}
        protected void CompletePresentationData      (ref IDevice device) {CompletePresentationData    (device.PresentationData     );}
        protected void CompletePhysicalDescription   (ref IDevice device) {CompletePhysicalDescription (device.PhysicalDescription  );}
        protected void CompleteLogic                 (ref IDevice device) {CompleteLogic               (device                      );}
        protected void CompleteTradingData           (ref IDevice device) {CompleteTradingData         (device.TradingData          );}
        protected void CompleteDescription           (ref IDevice device) {CompleteDescription         (device.Description          );}
        protected void CompleteDocumentation         (ref IDevice device) {CompleteDocumentation       (device.Documentation        );}
        protected void CompleteSecurity              (ref IDevice device) {CompleteSecurity            (device.Security             );}
        protected void CompleteConnections           (ref IDevice device) {CompleteConnections         (device.Connections          );}
        protected void CompleteSafety                (ref IDevice device) {CompleteSafety              (device.Safety               );}
        protected void CompleteState                 (ref IDevice device) {CompleteState               (device.State                );}
        protected void CompleteSubdevices            (ref IDevice device) {CompleteSubdevices          (device.SubDevices           );}
        protected void CompleteManufacturingData        (ref IDevice device) {CompleteManufacturingData      (device.ManufacturingData       );}
        protected void CompleteParametrization       (ref IDevice device) {CompleteParametrization     (device                      );}


        protected abstract void CompleteIdentification          (IIdentification        identification      );
        protected abstract void CompleteSkills                  (ISkillList             skillList           , IComponent context);
        protected abstract void CompletePresentationData        (IPresentationData      presentationData    );
        protected abstract void CompletePhysicalDescription     (IPhysicalDescription   physicalDescription );
        protected abstract void CompleteLogic                   (ILogicContainer        logicContainer      );
        protected abstract void CompleteTradingData             (ITradingData           tradingData         );
        protected abstract void CompleteDescription             (IDeviceDescription     deviceDescription   );
        protected abstract void CompleteDocumentation           (IDocumentation         documentation       );
        protected abstract void CompleteSecurity                (ISecurity              security            );
        protected abstract void CompleteConnections             (IConnectionList        connectionList      );
        protected abstract void CompleteSafety                  (ISafety                safety              );
        protected abstract void CompleteState                   (IState                 state               );
        protected abstract void CompleteSubdevices              (IList<IDevice>         subdevices          );
        protected abstract void CompleteManufacturingData          (IManufacturingData     productionData      );
        protected abstract void CompleteParametrization         (IDevice                device              );

    }

}

