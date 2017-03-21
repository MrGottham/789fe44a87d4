using System;

namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    /// <summary>
    /// Interface for a creator of the archive.
    /// </summary>
    public interface ICreator : INamedObject
    {
        /// <summary>
        /// Timestamp for the start of the period.
        /// </summary>
        DateTime PeriodStart
        {
            get;
            set;
        }

        /// <summary>
        /// Timestamp for the end of the period.
        /// </summary>
        DateTime PeriodEnd
        {
            get;
            set;
        }
    }
}
