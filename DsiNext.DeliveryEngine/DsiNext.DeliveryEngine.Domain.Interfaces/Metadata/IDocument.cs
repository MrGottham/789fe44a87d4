namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    /// <summary>
    /// Interface for a basic document.
    /// </summary>
    public interface IDocument : INamedObject
    {
        /// <summary>
        /// Unique ID for the document.
        /// </summary>
        int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Reference to the document.
        /// </summary>
        string Reference
        {
            get;
            set;
        }
    }
}
