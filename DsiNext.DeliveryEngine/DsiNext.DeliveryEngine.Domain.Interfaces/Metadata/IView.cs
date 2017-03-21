namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    /// <summary>
    /// Interface to information about a view (query).
    /// </summary>
    public interface IView : INamedObject
    {
        /// <summary>
        /// Original SQL query which define the view (query).
        /// </summary>
        string SqlQuery
        {
            get;
            set;
        }
    }
}
