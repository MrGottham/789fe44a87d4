using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Interfaces.Data
{
    /// <summary>
    /// Interface to a data object for a document.
    /// </summary>
    public interface IDocumentData : IDataObjectBase
    {
        /// <summary>
        /// Source document.
        /// </summary>
        IDocument Document
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
        }
    }
}
