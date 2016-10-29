using Akomi.InformationModel.Skills.Setup;
using Akomi.InformationModel.Skills.SkillCatalogue.DetectPresence;

namespace Tapako.TestClasses
{
    /// <summary>
    /// Derivated Class for Skill <see cref="SkillDetectPresenceBase"/>: Image recognition system
    /// </summary>
    public class SkillCheckPresenceDummy : SkillDetectPresenceDefault
    {
        public sealed override string IID { get; set; }

        public sealed override object Container { get; set; }

        public sealed override SetupStep CurrentSetupStep { get; set; }

        #region Private Fields
        private string _pathToTestImage;
        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        public SkillCheckPresenceDummy(string pathToTestImage, object container)
        {
            // Initialize ##################
            UserConstraints = new DetectPresenceInputParam();
            DeviceConstraints = new DetectPresenceInputParam();
            DefaultDeviceValues = new DetectPresenceInputParam();
            OutputParam = new DetectPresenceOutputParam();
            CurrentSetupStep = new SetupStep("CurrentSetupStep");
            IID = "none";
            Container = container;
            //##############################

            _pathToTestImage = pathToTestImage;
            Container = container;
        }

        public sealed override DetectPresenceOutputParam OutputParam
        {
            get { return base.OutputParam; }
            set { base.OutputParam = value; }
        }

        public sealed override DetectPresenceInputParam DefaultDeviceValues
        {
            get { return base.DefaultDeviceValues; }
            set { base.DefaultDeviceValues = value; }
        }

        public sealed override DetectPresenceInputParam DeviceConstraints
        {
            get { return base.DeviceConstraints; }
            set { base.DeviceConstraints = value; }
        }

        public sealed override DetectPresenceInputParam UserConstraints
        {
            get { return base.UserConstraints; }
            set { base.UserConstraints = value; }
        }

        public override void Calculate()
        {

            //TODO Calculate Values from given User Constraints            
            // Write Results in DeviceConstraints
            DeviceConstraints.DetectionCertainty.Value = 99;


            OutputParam.Status = "ready";
        }


        public override void Execute()
        {
            // Based on IID use specific DB for HdevCall
            // hdev.ProcCall(dbs("IID"))

            //Calculate
            OutputParam.Result = true;
        }

    }
} 
    