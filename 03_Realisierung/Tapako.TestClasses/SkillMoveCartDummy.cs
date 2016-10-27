using Akomi.InformationModel.Component;
using Akomi.InformationModel.Skills.Setup;
using Akomi.InformationModel.Skills.SkillCatalogue.MoveCart;

namespace Tapako.TestClasses
{
    /// <summary />
    public class SkillMoveCartDummy : SkillMoveCartDefault
    {
        public SkillMoveCartDummy() : base(null)
        {
        }
        public SkillMoveCartDummy(IComponent context) : base(context)
        {
        }

        public override string IID { get; set; }
        public override SetupStep CurrentSetupStep { get; set; }
        public override MoveCartInputParam UserConstraints { get; set; }
        public override MoveCartInputParam DeviceConstraints { get; set; }
        public override MoveCartInputParam DefaultDeviceValues { get; set; }

        /// <summary>
        /// Skill output container for holding the skill result
        /// </summary>
        public override MoveCartOutputParam OutputParam { get; set; }
        public override object Container { get; set; }
    }
}
