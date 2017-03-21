namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    /// <summary>
    /// Interface for a document which are associated to a table.
    /// </summary>
    public interface ITableDocument : IDocument
    {
        /// <summary>
        /// Field which the documentet are associated with.
        /// </summary>
        IField Field
        {
            get;
            set;
        }
    }
}
