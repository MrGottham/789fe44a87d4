namespace DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators
{
    /// <summary>
    /// Interface for an data setter which can set or change a field value.
    /// </summary>
    public interface IDataSetter : IDataManipulator, ICriteriaConfigurations
    {
        /// <summary>
        /// Name of the field on which to set or change data.
        /// </summary>
        string FieldName
        {
            get;
        }

        /// <summary>
        /// The new field value.
        /// </summary>
        object FieldValue
        {
            get;
        }
    }
}
