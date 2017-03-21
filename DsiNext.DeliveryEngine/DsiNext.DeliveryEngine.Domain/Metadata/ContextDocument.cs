using System;
using System.Collections.ObjectModel;
using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Context document.
    /// </summary>
    public class ContextDocument : DocumentBase, IContextDocument
    {
        #region Private variables

        private DateTime? _documentDate;
        private DateTimePresicion _documentDatePresicion;
        private readonly ObservableCollection<IDocumentAuthor> _documentAuthors = new ObservableCollection<IDocumentAuthor>();
        private readonly ObservableCollection<ContextDocumentCategories> _categories = new ObservableCollection<ContextDocumentCategories>();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a context document.
        /// </summary>
        /// <param name="id">Unique ID for the document.</param>
        /// <param name="nameSource">Document name in the source repository.</param>
        /// <param name="nameTarget">Document name in the target repository.</param>
        /// <param name="reference">Reference to the document.</param>
        /// <param name="category">Primary category for the context document.</param>
        public ContextDocument(int id, string nameSource, string nameTarget, string reference, ContextDocumentCategories category)
            : this(id, nameSource, nameTarget, reference, null, category)
        {
        }

        /// <summary>
        /// Creates a context document.
        /// </summary>
        /// <param name="id">Unique ID for the document.</param>
        /// <param name="nameSource">Document name in the source repository.</param>
        /// <param name="nameTarget">Document name in the target repository.</param>
        /// <param name="reference">Reference to the document.</param>
        /// <param name="description">Description.</param>
        /// <param name="category">Primary category for the context document.</param>
        public ContextDocument(int id, string nameSource, string nameTarget, string reference, string description, ContextDocumentCategories category)
            : base(id, nameSource, nameTarget, reference, description)
        {
            _categories.Add(category);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Document date.
        /// </summary>
        public virtual DateTime? DocumentDate
        {
            get
            {
                return _documentDate;
            }
            set
            {
                if (_documentDate == value)
                {
                    return;
                }
                _documentDate = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public DateTimePresicion DocumentDatePresicion
        {
            get { return _documentDatePresicion; }
            set
            {
                if (_documentDatePresicion == value) return;

                _documentDatePresicion = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Document authors.
        /// </summary>
        public virtual ReadOnlyObservableCollection<IDocumentAuthor> DocumentAuthors
        {
            get
            {
                return new ReadOnlyObservableCollection<IDocumentAuthor>(_documentAuthors);
            }
        }

        /// <summary>
        /// Cateogires for the context document.
        /// </summary>
        public virtual ReadOnlyObservableCollection<ContextDocumentCategories> Categories
        {
            get
            {
                return new ReadOnlyObservableCollection<ContextDocumentCategories>(_categories);
            }
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Add a document author.
        /// </summary>
        /// <param name="documentAuthor">Document author.</param>
        public virtual void AddDocumentAuthor(IDocumentAuthor documentAuthor)
        {
            if (documentAuthor == null)
            {
                throw new ArgumentNullException("documentAuthor");
            }
            _documentAuthors.Add(documentAuthor);
        }

        /// <summary>
        /// Add category to the context document.
        /// </summary>
        /// <param name="category">Category for the context document.</param>
        public virtual void AddCategory(ContextDocumentCategories category)
        {
            _categories.Add(category);
        }

        #endregion
    }
}
