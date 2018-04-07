using Akomi.InformationModel.Device;
using Akomi.InformationModel.Skills;

namespace Tapako.Services
{
    public class ReferenceToHostInputParam : InputParamBase
    {
        public IDevice Device { get; set; }
    }
}
