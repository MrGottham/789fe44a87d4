namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    /// <summary>
    /// Interface for a document author.
    /// </summary>
    public interface IDocumentAuthor : IMetadataObjectBase
    {
        /// <summary>
        /// Name of author.
        /// </summary>
        string Author
        {
            get;
            set;
        }

        /// <summary>
        /// Institution of author.
        /// </summary>
        string Institution
        {
            get;
            set;
        }
    }
}
