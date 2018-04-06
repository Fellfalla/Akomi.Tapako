using Akomi.InformationModel.Device;
using Akomi.InformationModel.Skills;

namespace Tapako.Services
{
    public class ReferenceToHostInputParam : SkillParameter
    {
        public IDevice Device { get; set; }
    }
}
