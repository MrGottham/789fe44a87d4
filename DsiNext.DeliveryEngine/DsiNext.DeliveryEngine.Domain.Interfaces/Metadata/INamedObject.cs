using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;

namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    /// <summary>
    /// Interface for a named matadata object in the delivery engine.
    /// </summary>
    public interface INamedObject : IMetadataObjectBase, IDeliveryEngineMetadataExceptionInfo
    {
        /// <summary>
        /// Name from the source repository.
        /// </summary>
        string NameSource
        {
            get;
            set;
        }

        /// <summary>
        /// Name in the target repository.
        /// </summary>
        string NameTarget
        {
            get;
            set;
        }

        /// <summary>
        /// Description.
        /// </summary>
        string Description
        {
            get;
            set;
        }
    }
}
