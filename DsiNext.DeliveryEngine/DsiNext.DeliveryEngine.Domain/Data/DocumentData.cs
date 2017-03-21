using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Domain.Data
{
    /// <summary>
    /// Data object for a document.
    /// </summary>
    public class DocumentData : DataObjectBase, IDocumentData
    {
        #region Private variables

        private IDocument _document;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates data object for a document.
        /// </summary>
        /// <param name="field">Field reference for the data object.</param>
        /// <param name="document">Source document.</param>
        public DocumentData(IField field, IDocument document)
            : base(field)
        {
            _document = document;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Source document.
        /// </summary>
        public virtual IDocument Document
        {
            get
            {
                return _document;
            }
            set
            {
                if (_document == value)
                {
                    return;
                }
                _document = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
                RaisePropertyChanged(this, "Reference");
            }
        }

        /// <summary>
        /// Reference to the document.
        /// </summary>
        public virtual string Reference
        {
            get
            {
                return Document == null ? null : Document.Reference;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the source document.
        /// </summary>
        /// <typeparam name="TValue">Type of the source document.</typeparam>
        /// <returns>Source document.</returns>
        public override TValue GetSourceValue<TValue>()
        {
            if (typeof (TValue) != typeof (IDocument))
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.TypeMismatch, typeof (TValue), typeof (IDocument), MethodBase.GetCurrentMethod().Name));
            }
            var property = GetType().GetProperty("Document");
            return (TValue) property.GetValue(this, null);
        }

        /// <summary>
        /// Get the target document.
        /// </summary>
        /// <typeparam name="TValue">Type of the target document.</typeparam>
        /// <returns>Target document.</returns>
        public override TValue GetTargetValue<TValue>()
        {
            if (typeof(TValue) != typeof(IDocument))
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.TypeMismatch, typeof (TValue), typeof (IDocument), MethodBase.GetCurrentMethod().Name));
            }
            var property = GetType().GetProperty("Document");
            return (TValue) property.GetValue(this, null);
        }

        /// <summary>
        /// Updates the source document.
        /// </summary>
        /// <typeparam name="TValue">Type of the source document.</typeparam>
        /// <param name="sourceValue">New source document.</param>
        public override void UpdateSourceValue<TValue>(TValue sourceValue)
        {
            var property = GetType().GetProperty("Document");
            if (Equals(sourceValue, null))
            {
                property.SetValue(this, null, null);
                return;
            }
            if (typeof (TValue) == typeof (IDocument))
            {
                property.SetValue(this, sourceValue, null);
                return;
            }
            throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.TypeMismatch, typeof (TValue), typeof (IDocument), MethodBase.GetCurrentMethod().Name));
        }

        /// <summary>
        /// Clone the data object for the document.
        /// </summary>
        /// <returns>Cloned data object for the document.</returns>
        public override object Clone()
        {
            return new DocumentData(Field, Document);
        }

        #endregion
    }
}
