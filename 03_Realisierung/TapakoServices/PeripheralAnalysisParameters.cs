using System.ComponentModel;
using ExtensionMethodsCollection;

namespace Tapako.Services
{
    public class PeripheralAnalysisParameters
    {
        /// <summary>
        /// If this option is set to true, the scan procedure tries to avpoid 
        /// </summary>
        [DefaultValue(false)]
        public bool AllowUserInput = false;

        public PeripheralAnalysisParameters()
        {
            this.AssignDefaultValueAttributes();
        }
    }
}
