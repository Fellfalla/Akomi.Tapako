using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Akomi.InformationModel.Device.Parametrization;
using Akomi.Logger;
using Microsoft.Practices.Unity;
using Prism.Logging;
using Prism.Regions;
using Prism.Unity;
using Tapako.Framework;
using Tapako.Services;
using Tapako.Utilities.UniversalHostSearch;
using Tapako.View;
using Tapako.ViewModel;

namespace Tapako.Startup
{
    /// <summary>
    /// Bootstrapper für Tapako.
    /// Diese Klasse übernimmt das starten des Programms
    /// </summary>
    public class Bootstrapper : UnityBootstrapper
    {

        private void GlobalSettingConfiguration()
        {
            //ParametrizationHandlerBase.DefaultProgressReporter = Container.Resolve<ProgressReporter>();
            ParametrizationHandlerBase.DefaultProgressReporter = new ProgressReporter();
        }

        protected override DependencyObject CreateShell()
        {
            //return new MainWindow(new TapakoViewModel()); Das wird jetzt von dem Container übernommen
            GlobalSettingConfiguration();
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            var mainWindow = (Window) Shell; // Hier muss das Fenster initialisiert werden, damit es angezeigt wird

            // Registriere die View auf die unterschiedlichen Regionen
            var regionManager = Container.Resolve<IRegionManager>();
            //regionManager.RegisterViewWithRegion(RegionNames.MvvmTest, typeof(HostSearchView));
            regionManager.RegisterViewWithRegion(RegionNames.HostSearchView, typeof(HostSearchView));
            regionManager.RegisterViewWithRegion(RegionNames.AnalysisView, typeof(AnalysisView));
            regionManager.RegisterViewWithRegion(RegionNames.LoggerView, typeof(LoggerView));
            regionManager.RegisterViewWithRegion(RegionNames.OpcUaServerControl, typeof(OpcUaServerControlView));
            regionManager.RegisterViewWithRegion(RegionNames.ProgressView, typeof(ProgressView));
            regionManager.RegisterViewWithRegion(RegionNames.DeviceView, typeof(DeviceView));
            regionManager.RegisterViewWithRegion(RegionNames.UniversalHostSearchView, typeof(UniversalHostSearchView));
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();



            #region Registriere alle Views
            Container.RegisterType<AnalysisView>();
            Container.RegisterType<HostSearchView>();
            Container.RegisterType<ProgressView>();
            Container.RegisterType<MainWindow>();
            Container.RegisterType<LoggerView>();
            Container.RegisterType<DeviceView>();
            Container.RegisterType<UniversalHostSearchView>();
            #endregion
            
            #region Sonstige Registriereungen
            Container.RegisterType<UserControl>();
            Container.RegisterType<ProgressReporter>();
            Container.RegisterType<INotifyPropertyChanged>();
            Container.RegisterType<DeviceTapakoViewModel>();
            Container.RegisterType<IDeviceTapakoViewModel, DeviceTapakoViewModel>();
            #endregion

            #region Registriere Klassen, die es nur 1 mal geben soll
            //TapakoViewModel viewModel = new TapakoViewModel();
            //Container.RegisterInstance<ITapakoViewModel>(viewModel);
            //Container.RegisterInstance(viewModel);

            Container.RegisterInstance(new LoggerFacade());

            // Register once instanziated ViewModels
            // todo: Bootstrapper besser verstehen um das was jetzt kommt vermeiden zu können
            var universalHostSearchViewModel = new UniversalHostSearchViewModel();
            var hostSearchViewModel = new HostSearchViewModel(universalHostSearchViewModel);
            var tapakoViewModel = new TapakoViewModel(hostSearchViewModel);
            Container.RegisterInstance(universalHostSearchViewModel);
            Container.RegisterInstance<IHostSearchViewModel>(hostSearchViewModel);
            Container.RegisterInstance<ITapakoViewModel>(tapakoViewModel);

            #endregion
        }

        protected override void ConfigureServiceLocator()
        {
            base.ConfigureServiceLocator();



        }

        /// <summary>
        /// Gibt eine Instanz auf den eigenen Logger zurück.
        /// Dieser Logger wird dann für Diverse meldungen verwendet
        /// </summary>
        /// <returns></returns>
        protected override ILoggerFacade CreateLogger()
        {
        return new LoggerFacade();
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            var mappings = base.ConfigureRegionAdapterMappings();
            // Hier habe ich versucht, einen Behaviour Adapter zu einem bestimmten Region-Typ hinzuzufügen
            // Allerdings können die geforderten Klassen der Dependency injection nicht vom Container aufgelöst werden
            //mappings.RegisterMapping(typeof(UserControl), Container.Resolve<UniformGridRegionAdapter>());
            return mappings;
        }


    }
}