using System.Collections;
using Akomi.InformationModel.Device;

namespace Tapako.DeviceInformationManagement.InformationSources
{
    /// <summary>
    /// Implement this factory in the class of your driver, which is reponsible for creating the correct device object.
    /// </summary>
    public interface IDeviceCompletement
    {
        IDevice CompleteDeviceDriver(ref IDevice deviceRoot);
    }

}

