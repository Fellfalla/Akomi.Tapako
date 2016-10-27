using System.Collections.Generic;
using Akomi.InformationModel.Device;

namespace Tapako.Services
{
    public interface ICommunicationChannelDriver
    {

        CommunicationChannelType CommunicationChannelType { get; }

        /// <summary>
        /// Searches For SubDevices of the current Device
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        List<IDevice> SearchForSubSystems(IDevice device);



    }

}
