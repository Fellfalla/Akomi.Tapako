using System.Collections.Generic;
using Akomi.InformationModel.Device;

namespace Tapako.Services
{
    /// <summary>
    /// Basisklasse für Communication Channel Driver
    /// </summary>
    public abstract class CommunicationChannelDriverBase: ICommunicationChannelDriver
    {

        public abstract CommunicationChannelType CommunicationChannelType { get; }

        public abstract List<IDevice> SearchForSubSystems(IDevice device);

    }
}
