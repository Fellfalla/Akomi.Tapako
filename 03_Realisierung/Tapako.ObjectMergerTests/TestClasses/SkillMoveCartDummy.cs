using Akomi.InformationModel.Component;
using Akomi.InformationModel.Skills.Setup;
using Akomi.InformationModel.Skills.SkillCatalogue;

namespace Tapako.TestClasses
{
    /// <summary />
    public class SkillMoveCartDummy : SkillMoveCartBase
    {
        public SkillMoveCartDummy() : base(null)
        {
        }
        public SkillMoveCartDummy(IComponent context) : base(context)
        {
        }

        /// <summary>
        /// Skill output container for holding the skill result
        /// </summary>
        public override MoveCartOutputParam OutputParam { get; set; }

        public override void Calculate()
        {
            throw new System.NotImplementedException();
        }

        public override void Execute()
        {
            throw new System.NotImplementedException();
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

        public override object Container { get; set; }
    }
}
