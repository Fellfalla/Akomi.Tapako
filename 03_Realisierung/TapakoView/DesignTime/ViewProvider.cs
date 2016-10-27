using Microsoft.Practices.Unity;
using Tapako.Utilities.UniversalHostSearch;
using Tapako.ViewModel;
using Tapako.ViewModel.DesignTime;

namespace Tapako.View.DesignTime
{

    /// <summary>
    /// Source: http://www.codeproject.com/Articles/1059235/Blendability-Adding-design-time-support-for-region#PuttingItTogether
    /// Used to register views at design Time
    /// </summary>
    public class ViewProvider : UnityDesignTimeViewProvider
    {
        protected override void RegisterViewsWithContainer()
        {
            container.RegisterType<AnalysisView>();
            container.RegisterType<HostSearchView>();
            container.RegisterType<ProgressView>();
            container.RegisterType<MainWindow>();
            container.RegisterType<LoggerView>();
            container.RegisterType<DeviceView>();
            container.RegisterType<UniversalHostSearchView>();

            container.RegisterType<IDeviceTapakoViewModel, DeviceTapakoDesignViewModel>();
            container.RegisterType<UniversalHostSearchViewModel>();
            container.RegisterType<IHostSearchViewModel, HostSearchDesignViewModel>();
            container.RegisterType<ITapakoViewModel, TapakoDesignViewModel>();
            container.RegisterType<OpcUaServerControlViewModel>();
        }

        protected override void RegisterViewsWithRegions()
        {
            RegisterViewWithRegion<HostSearchView>(RegionNames.HostSearchView);
            RegisterViewWithRegion<AnalysisView>(RegionNames.AnalysisView);
            RegisterViewWithRegion<LoggerView>(RegionNames.LoggerView);
            RegisterViewWithRegion<OpcUaServerControlView>(RegionNames.OpcUaServerControl);
            RegisterViewWithRegion<ProgressView>(RegionNames.ProgressView);
            RegisterViewWithRegion<DeviceView>(RegionNames.DeviceView);
            RegisterViewWithRegion<UniversalHostSearchView>(RegionNames.UniversalHostSearchView);
        }
    }
}
