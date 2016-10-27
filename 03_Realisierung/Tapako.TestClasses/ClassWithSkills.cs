using System.ComponentModel;
using Akomi.InformationModel;
using Akomi.InformationModel.Attributes;
using Akomi.InformationModel.Component.HmiImage;
using Akomi.InformationModel.Component.PhysicalDescription;
using Akomi.InformationModel.Skills;
using IComponent = Akomi.InformationModel.Component.IComponent;

namespace Tapako.TestClasses
{
    [IsMergeable(true)]
    public class ClassWithSkills : IComponent
    {

        public ClassWithSkills()
        {
            Skills = new SkillList();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public IHmiImage HmiImage { get; set; }
        public IPhysicalDescription PhysicalDescription { get; set; }
        public IConnectionList Connections { get; private set; }

        //[DuplicatesAllowed(false)]
        //[IsMergeable(true)]

        public ISkillList Skills { get; private set; }
    }
}
