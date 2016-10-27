using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;
using Akomi.InformationModel.Enums;
using ExtensionMethodsCollection;
using Prism.Mvvm;
using Tapako.Framework;
using ProgressChangedEventArgs = Tapako.Framework.ProgressChangedEventArgs;

namespace Tapako.ViewModel
{
    public class ProgressViewModel : BindableBase
    {
        private ObservableCollection<KeyValuePair<string, ProgressState>> _progressSteps;
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        public ProgressViewModel()
        {
            ProgressSteps = new ObservableCollection<KeyValuePair<string, ProgressState>>(TapakoProgress.Steps.ToArray());
            TapakoProgress.ProgressChanged += (sender, args) => _dispatcher.DoDispatchedAction(() => ProgressChanged(sender, args));
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs args)
        {
            var newValue = new KeyValuePair<string, ProgressState>(args.ProgressStepKey, args.NewState);

            var changedItem = ProgressSteps.IndexOf(ProgressSteps.FirstOrDefault(pair => pair.Key.Equals(newValue.Key)));

            if (changedItem > -1)
            {
                ProgressSteps.RemoveAt(changedItem);
                ProgressSteps.Insert(changedItem, newValue);
            }

            else
            {
                ProgressSteps.Add(newValue);
            }
        }

        public ObservableCollection<KeyValuePair<string, ProgressState>> ProgressSteps
        {
            get { return _progressSteps; }
            set { SetProperty(ref _progressSteps, value); }
            //get
            //{
            //    return new CollectionView(Progress.Steps);
            //    //return new ObservableCollection<KeyValuePair<string, ProgressState>>(Progress.Steps.ToArray());
            //}
        }
    }

    public class ProgressDesignViewModel : ProgressViewModel
    {
        public ProgressDesignViewModel()
        {
            //base.ProgressSteps = new CollectionView(new Dictionary<string, ProgressState>()
            //    {
            //        {"Step1", ProgressState.Finished },
            //        {"Step2", ProgressState.Finished },
            //        {"Step3", ProgressState.InProgress },
            //        {"Step4", ProgressState.InProgress },
            //        {"Step5", ProgressState.Outstanding },
            //        {"Step6", ProgressState.Outstanding },
            //    });
        }

        public new ICollectionView ProgressSteps
        {
            get
            {
                return new CollectionView(new Dictionary<string, ProgressState>()
                {
                    {"Step1", ProgressState.Finished },
                    {"Step2", ProgressState.Finished },
                    {"Step3", ProgressState.InProgress },
                    {"Step4", ProgressState.InProgress },
                    {"Step5", ProgressState.Outstanding },
                    {"Step6", ProgressState.Outstanding },
                });
            }
            private set { }
        }
    }
}