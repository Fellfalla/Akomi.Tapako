using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using Akomi.InformationModel.Enums;
using Akomi.InformationModel.ExtensionMethods;

namespace Tapako.Framework
{
    public class ProgressReporter : IProgress<Tuple<ProgressStep, ProgressState>>
    {
        public ProgressReporter()
        {
            
        }

        public void Report(Tuple<ProgressStep, ProgressState> value)
        {
            TapakoProgress.SetProgressStep(value.Item1, value.Item2);
            //TapakoProgress.Steps.AddOrUpdate(value.Item1.Description(), value.Item2, (s, old) => value.Item2);
        }
    }

    public static class TapakoProgress
    {

        private static Func<ProgressStep, ProgressState, ProgressStepReverter> _createProcessStepConverter;

        public interface IProgressStepReverter : IDisposable
        {
            void SetDisposedState(ProgressState state);
        }

        /// <summary>
        /// Use this class for setting and resetting a given <see cref="ProgressStep"/>
        /// at creation or dispose
        /// </summary>
        private class ProgressStepReverter : IProgressStepReverter
        {
            // todo: handle state changes after creation of this class
            private ProgressState _releaseState;
            private ProgressStep _step;

            static ProgressStepReverter()
            {
                _createProcessStepConverter = GetFactoryMethod();
            }

            internal static Func<ProgressStep, ProgressState, ProgressStepReverter> GetFactoryMethod()
            {
                return (step, state) => new ProgressStepReverter(step, state);
            }

            private ProgressStepReverter(ProgressStep step, ProgressState oldState)
            {
                _step = step;
                _releaseState = oldState;
            }

            public void Dispose()
            {
                TapakoProgress.SetProgressStep(_step, _releaseState);
            }

            public void SetDisposedState(ProgressState state)
            {
                _releaseState = state;
            }
        }

        //public static event PropertyChangedEventHandler PropertyChanged;

        //public static void OnPropertyChanged(string name)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(null, new PropertyChangedEventArgs(name));
        //    }

        //}

        //private static TapakoProgress _instance;

        private static readonly Dispatcher ProgressDispatcher;

        private static string _nextGenerationDeviceScanString = ProgressStep.NextGenerationScan.Description();
        private static string _fieldDeviceScanString = ProgressStep.FieldScan.Description();
        private static string _primitiveDeviceScanString = ProgressStep.PrimitveScan.Description();
        private static string _basicDeviceScanString = ProgressStep.BasicScan.Description();
        private static string _standardDeviceScanString = ProgressStep.StandardScan.Description();
        private static string _publishInformationModelString = ProgressStep.Publication.Description();
        private static string _parametrizeDevicesString = ProgressStep.Parametrization.Description();
        private static string _configureDevicesString = ProgressStep.Configuration.Description();

        static TapakoProgress()
        {
            //ProgressDispatcher = Dispatcher.CurrentDispatcher;
            _createProcessStepConverter = ProgressStepReverter.GetFactoryMethod();
            Steps = new ConcurrentDictionary<string, ProgressState>();
            ProgressDispatcher = Dispatcher.CurrentDispatcher;

            Steps[_nextGenerationDeviceScanString] = ProgressState.Outstanding;
            Steps[_fieldDeviceScanString] = ProgressState.Outstanding;
            Steps[_standardDeviceScanString] = ProgressState.Outstanding;
            Steps[_primitiveDeviceScanString] = ProgressState.Outstanding;
            Steps[_basicDeviceScanString] = ProgressState.Outstanding;
            Steps[_publishInformationModelString] = ProgressState.Outstanding;
            Steps[_parametrizeDevicesString] = ProgressState.Outstanding;
            Steps[_configureDevicesString] = ProgressState.Outstanding;
        }

        /// <summary>
        /// </summary>
        /// <param name="key">Should be the same as the Description of the EnumValue of <see cref="ProgressStep"/></param>
        /// <param name="newState"></param>
        public static IProgressStepReverter SetProgressStep(string key, ProgressState newState)
        {
            if (Steps[key] != newState)
            {
                ProgressStep currentStep;
                if (!Enum.TryParse(key, true, out currentStep)) // if not sucessful, try parse through desciption
                {
                    currentStep = Enum.GetValues(typeof (ProgressStep))
                        .OfType<ProgressStep>()
                        .FirstOrDefault(step => step.Description().Equals(key));
                }

                var eventArgs =  new ProgressChangedEventArgs(newState, Steps[key], key, currentStep);
                return SetProgressStep(eventArgs);
            }
            return null;
        }

        public static IProgressStepReverter SetProgressStep(ProgressStep step, ProgressState newState)
        {
            string key = step.Description();
            if (Steps[key] != newState)
            {
                var eventArgs =  new ProgressChangedEventArgs(newState, Steps[key], key, step);
                return SetProgressStep(eventArgs);
            }
            return null;
        }

        public static ProgressState GetProgressStepState(ProgressStep step)
        {
            string key = step.Description();
            return Steps[key];
        }

        private static IProgressStepReverter SetProgressStep(ProgressChangedEventArgs eventArgs)
        {
            Steps[eventArgs.ProgressStepKey] = eventArgs.NewState;
            if (ProgressChanged != null) ProgressChanged(null, eventArgs);
            return _createProcessStepConverter(eventArgs.ProgressStep, ProgressState.Failed);
        }

        /// <summary>
        /// e.g. PLC
        /// </summary>
        public static ProgressState NextGenerationDeviceScan
        {
            get { return Steps[_nextGenerationDeviceScanString]; }
            set
            {
                SetProgressStep(ProgressStep.NextGenerationScan, value);
            }
        }

        /// <summary>
        /// e.g. hydraulics
        /// </summary>
        public static ProgressState PrimitiveDeviceScan
        {
            get { return Steps[_primitiveDeviceScanString]; }
            set
            {
                SetProgressStep(ProgressStep.PrimitveScan, value);
            }
        }

        /// <summary>
        /// e.g. IoLink
        /// </summary>
        public static ProgressState BasicDeviceScan
        {
            get { return Steps[_basicDeviceScanString]; }
            set
            {
                SetProgressStep(ProgressStep.BasicScan, value);
            }
        }

        /// <summary>
        /// e. g. EtherCat, Fieldbus
        /// </summary>
        public static ProgressState FieldDeviceScan
        {
            get { return Steps[_fieldDeviceScanString]; }
            set
            {
                SetProgressStep(ProgressStep.FieldScan, value);
            }
        }

        /// <summary>
        /// e.g. Ethernet HMI
        /// </summary>
        public static ProgressState StandardDeviceScan
        {
            get { return Steps[_standardDeviceScanString]; }
            set
            {
                SetProgressStep(ProgressStep.StandardScan, value);
            }
        }

        /// <summary>
        /// e.g. Start Opc Ua Server
        /// </summary>
        public static ProgressState PublishInformationModel
        {
            get { return Steps[_publishInformationModelString]; }
            set
            {
                SetProgressStep(ProgressStep.Publication, value);
            }
        }

        /// <summary>
        /// e.g. Setting trigger point
        /// </summary>
        public static ProgressState ParametrizeDevices
        {
            get { return Steps[_parametrizeDevicesString]; }
            set
            {
                SetProgressStep(ProgressStep.Parametrization, value);
            }
        }

        // todo: Make Observable Dictionary, welches seine reihenfolde nicht ändert. Vorlage kommt von Dr.Wpf
        public static ConcurrentDictionary<string, ProgressState> Steps { get; set; }

        public static event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        public static event PropertyChangedEventHandler PropertyChanged;

        public static void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            // todo: eines von den beiden entfernen wenn das endlich mal funktioniert
            if (handler != null) handler(null, new PropertyChangedEventArgs(propertyName));
        }

        private static void SetDictionaryKeyFromDispatcher(string key, ProgressState value)
        {
            Action<string, ProgressState> action = (keyArg, valueArg) => Steps[keyArg] = valueArg;

            ProgressDispatcher.BeginInvoke(action, DispatcherPriority.DataBind, new object[] {key, value});
        }
    }

    public class ProgressChangedEventArgs : EventArgs
    {
        public ProgressChangedEventArgs()
        {
            
        }

        public ProgressChangedEventArgs(ProgressState newState, ProgressState oldState, string progressStepKey, ProgressStep progressStep)
        {
            NewState = newState;
            OldState = oldState;
            ProgressStepKey = progressStepKey;
            ProgressStep = progressStep;
        }

        public ProgressState NewState { get; set; }

        public ProgressState OldState { get; set; }

        public string ProgressStepKey { get; set; }

        public ProgressStep ProgressStep { get; set; }
    }
}