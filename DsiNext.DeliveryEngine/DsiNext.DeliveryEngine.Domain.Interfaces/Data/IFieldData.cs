namespace DsiNext.DeliveryEngine.Domain.Interfaces.Data
{
    /// <summary>
    /// Interface to a data object for a field.
    /// </summary>
    /// <typeparam name="TSourceValue">Type of the source value.</typeparam>
    /// <typeparam name="TTargetValue">Type of the target value.</typeparam>
    public interface IFieldData<TSourceValue, out TTargetValue> : IDataObjectBase
    {
        /// <summary>
        /// Source value.
        /// </summary>
        TSourceValue SourceValue
        {
            get;
            set;
        }

        /// <summary>
        /// Target value.
        /// </summary>
        TTargetValue TargetValue
        {
            get;
        }

        /// <summary>
        /// Indicates if the value is mapped.
        /// </summary>
        bool Mapping
        {
            get;
        }
    }
}
