namespace Tapako.DeviceInformationManagement.InformationSources
{
    /// <summary>
    /// This indicates the priority of the <see cref="IInformationSource"/>. 
    /// The higher the <see cref="SourcePriority"/> the earlier this <see cref="IInformationSource"/> should be called.
    /// </summary>
    public enum SourcePriority
    {
        /// <summary>
        /// High SourcePriority is for elements with lots of Logic
        /// </summary>
        High = 3,

        /// <summary>
        /// Medium SourcePriority is for Information Source without any Logic
        /// </summary>
        Medium = 2,

        /// <summary>
        /// Low SourcePriority is for information sources which need other information sources to be loaded earlier
        /// </summary>
        Low = 1
    }

}
