using System;
using Akomi.InformationModel.Component;
using Akomi.InformationModel.Skills;

namespace Tapako.TestClasses
{
    public class InputDummy : InputParamBase { }
    public class OutputDummy : OutputParamBase { }

    public class SkillEmptyDummy : SkillBase<InputDummy, OutputDummy>
    {
        private string _name;
        private string _usid;

        public SkillEmptyDummy(IComponent context, string usid = "1", string name = "test") : base (context)
        {
            _usid = usid;
            _name = name;
        }

        public override string Name
        {
            get { return _name; }
        }

        public override string USID
        {
            get { return _usid; }
        }

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

        //public override SkillType SkillType
        //{
        //    get { throw new System.NotImplementedException(); }
        //}

        public override string SkillType
        {
            get { throw new NotImplementedException(); }
        }
    }
}