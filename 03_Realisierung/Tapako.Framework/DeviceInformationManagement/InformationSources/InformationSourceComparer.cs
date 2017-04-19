using System.Collections.Generic;

namespace Tapako.DeviceInformationManagement.InformationSources
{
    /// <summary>
    /// Comparer, um die Information Sources in einer Liste der Priorität nach ordnen zu können
    /// </summary>
    public class InformationSourceComparer : IComparer<IInformationSource>
    {
        /// <summary>
        /// Compares the priority of two information sources.
        /// </summary>
        /// <param name="x">first source</param>
        /// <param name="y">second source</param>
        /// <returns>
        /// -1 if y has bigger priority, otherwise 1.
        /// If they are equally prior 0 will be returned.
        /// </returns>
        public int Compare(IInformationSource x, IInformationSource y)
        {
            return y.SourcePriority - x.SourcePriority;
        }
    }
}