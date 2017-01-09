using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Akomi.InformationModel.Component.Presentation;
using Prism.Mvvm;
using Tapako.Utilities.WiringTool.View;

namespace Tapako.Utilities.WiringTool.ViewModel
{
    public class WiringToolViewModel : BindableBase
    {
        //private IList<Tuple<object, object>> _wiredConnections;
        private ICollection<object> _childConnections;
        private ICollection<object> _parentConnections;
        private IHmiImage _childHmiImage;
        private IHmiImage _parentHmiImage;
        private string _childName;
        private string _parentName;
        private ObservableCollection<Wiring> _wirings;

        public WiringToolViewModel()
        {
            //WiredConnections = new List<Tuple<object, object>>();
            Wirings = new ObservableCollection<Wiring>();
        }

        public WiringToolViewModel(IEnumerable parentConnections, IEnumerable childConnections, string parentName = "Parent", string childName = "Child") : this()
        {
            ParentConnections = parentConnections.Cast<object>().ToList();
            ChildConnections = childConnections.Cast<object>().ToList();
            //var t = Tuple.Create(new object(), new object());
            
            ParentName = parentName;
            ChildName = childName;
        }

        //public WiringToolDesignViewModel Create<T>(IEnumerable<T> parentConnections, IEnumerable<T> childConnections,
        //    string parentName = "Parent", string childName = "Child")
        //{
        //    return new WiringToolViewModel();
        //}

        public string ParentName
        {
            get { return _parentName; }
            set { SetProperty(ref _parentName, value); }
        }

        public string ChildName
        {
            get { return _childName; }
            set { SetProperty(ref _childName, value); }
        }

        public IHmiImage ParentHmiImage
        {
            get { return _parentHmiImage; }
            set { SetProperty(ref _parentHmiImage, value); }
        }

        public IHmiImage ChildHmiImage
        {
            get { return _childHmiImage; }
            set { SetProperty(ref _childHmiImage, value); }
        }


        public ICollection<object> ParentConnections
        {
            get { return _parentConnections; }
            set { SetProperty(ref _parentConnections, value); }
        }

        public ICollection<object> ChildConnections
        {
            get { return _childConnections; }
            set { SetProperty(ref _childConnections, value); }
        }

        public IEnumerable<LogicalWiring> LogicalConnections
        {
            get { return Wirings.Select(wiring => wiring.Logical); }
        }

        public ObservableCollection<Wiring> Wirings
        {
            get { return _wirings; }
            set { SetProperty( ref _wirings, value); }
        }

        public void Reset()
        {
            Wirings.Clear();
        }
    }
}
