using System;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Interfaces.Data
{
    /// <summary>
    ///  Interface for an basic data object in the delivery engine.
    /// </summary>
    public interface IDataObjectBase : IDomainObjectBase, ICloneable
    {
        /// <summary>
        /// Field reference for the data object.
        /// </summary>
        IField Field
        {
            get;
        }

        /// <summary>
        /// Get the source value for the data object.
        /// </summary>
        /// <typeparam name="TSourceValue">Type of the source value.</typeparam>
        /// <returns>Source value.</returns>
        TSourceValue GetSourceValue<TSourceValue>();

        /// <summary>
        /// Get the target (mapped) value for the data object.
        /// </summary>
        /// <typeparam name="TTargetValue">Type of the target value.</typeparam>
        /// <returns>Target value.</returns>
        TTargetValue GetTargetValue<TTargetValue>();

        /// <summary>
        /// Get an mapped target value for the data object.
        /// </summary>
        /// <typeparam name="TTargetValue">Type of the target value.</typeparam>
        /// <param name="mapper">Mapper used to map the target value.</param>
        /// <returns>Mapped target value.</returns>
        TTargetValue GetTargetValue<TTargetValue>(IMap mapper);

        /// <summary>
        /// Updates the source value on the data object.
        /// </summary>
        /// <typeparam name="TSourceValue">Type of the source value.</typeparam>
        /// <param name="sourceValue">New source value.</param>
        void UpdateSourceValue<TSourceValue>(TSourceValue sourceValue);
    }
}
