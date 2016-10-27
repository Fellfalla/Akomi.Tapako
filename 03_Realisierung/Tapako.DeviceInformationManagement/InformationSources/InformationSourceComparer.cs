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
            if (x.SourcePriority < y.SourcePriority)
            {
                return -1; // kleiner als 0 rückt das Element weiter nach vorne
            }
            else if (x.SourcePriority == y.SourcePriority)
            {
                return 0; // position bleibt gleich
            }
            else
            {
                return 1;  // größer als 0 rückt das Element weiter nach hinten
            }

        }
    }
}