using System;
using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Document author.
    /// </summary>
    public class DocumentAuthor : MetadataObjectBase, IDocumentAuthor
    {
        #region Private variables

        private string _author;
        private string _institution;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a document author.
        /// </summary>
        /// <param name="author">Name of author.</param>
        /// <param name="institution">Institution of author.</param>
        public DocumentAuthor(string author, string institution)
        {
            if (string.IsNullOrEmpty(author))
            {
                throw new ArgumentNullException("author");
            }
            if (string.IsNullOrEmpty(institution))
            {
                throw new ArgumentNullException("institution");
            }
            _author = author;
            _institution = institution;
        }

        /// <summary>
        /// Creates a document author.
        /// </summary>
        /// <param name="institution">Institution of author.</param>
        public DocumentAuthor(string institution)
        {
            if (string.IsNullOrEmpty(institution))
            {
                throw new ArgumentNullException("institution");
            }
            _institution = institution;
        }

        #endregion

        #region IDocumentAuthor Members

        /// <summary>
        /// Name of author.
        /// </summary>
        public virtual string Author
        {
            get
            {
                return _author;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                if (_author == value)
                {
                    return;
                }
                _author = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Institution of author.
        /// </summary>
        public virtual string Institution
        {
            get
            {
                return _institution;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                if (_institution == value)
                {
                    return;
                }
                _institution = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        #endregion
    }
}
