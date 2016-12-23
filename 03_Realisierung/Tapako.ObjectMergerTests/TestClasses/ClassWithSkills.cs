using System.ComponentModel;
using Akomi.InformationModel;
using Akomi.InformationModel.Attributes;
using Akomi.InformationModel.Component.Connection;
using Akomi.InformationModel.Component.Identification;
using Akomi.InformationModel.Component.PhysicalDescription;
using Akomi.InformationModel.Component.Presentation;
using Akomi.InformationModel.Component.ManufacturingData;
using Akomi.InformationModel.Component.State;
using Akomi.InformationModel.Component.TradingData;
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
            Connections = new ConnectionList();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public IHmiImage HmiImage { get; set; }
        public IIdentification Identification { get; set; }
        public IManufacturingData ManufacturingData { get; set; }
        public ITradingData TradingData { get; set; }
        public IPresentationData PresentationData { get; set; }
        public IPhysicalDescription PhysicalDescription { get; set; }

        //[DuplicatesAllowed(false)]
        //[IsMergeable(true)]

        public IConnectionList Connections { get; private set; }
        public ISkillList Skills { get; private set; }
        public IState State { get; set; }
    }
}
