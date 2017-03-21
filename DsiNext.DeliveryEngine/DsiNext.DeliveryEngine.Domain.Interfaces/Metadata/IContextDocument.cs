using System;
using System.Collections.ObjectModel;

namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    /// <summary>
    /// Interface for a context document.
    /// </summary>
    public interface IContextDocument : IDocument
    {
        /// <summary>
        /// Document date.
        /// </summary>
        DateTime? DocumentDate
        {
            get;
            set;
        }

        DateTimePresicion DocumentDatePresicion { get; set; }

        /// <summary>
        /// Document authors.
        /// </summary>
        ReadOnlyObservableCollection<IDocumentAuthor> DocumentAuthors
        {
            get;
        }
            
        /// <summary>
        /// Cateogires for the context document.
        /// </summary>
        ReadOnlyObservableCollection<ContextDocumentCategories> Categories
        {
            get;
        }

        /// <summary>
        /// Add a document author.
        /// </summary>
        /// <param name="documentAuthor">Document author.</param>
        void AddDocumentAuthor(IDocumentAuthor documentAuthor);

        /// <summary>
        /// Add category to the context document.
        /// </summary>
        /// <param name="category">Category for the context document.</param>
        void AddCategory(ContextDocumentCategories category);
    }
}
