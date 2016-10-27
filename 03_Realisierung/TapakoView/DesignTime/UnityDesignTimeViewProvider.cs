using System;
using Microsoft.Practices.Unity;

namespace Tapako.View.DesignTime
{
    /// <summary>
    /// Source: http://www.codeproject.com/Articles/1059235/Blendability-Adding-design-time-support-for-region#PuttingItTogether
    /// </summary>
    public abstract class UnityDesignTimeViewProvider : DesignTimeViewProviderBase
    {
        protected readonly IUnityContainer container = new UnityContainer();

        protected override object ResolveView(Type viewType)
        {
            return container.Resolve(viewType);
        }
    }
}
