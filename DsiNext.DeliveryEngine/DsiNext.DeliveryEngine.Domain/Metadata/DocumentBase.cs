using System;
using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Basic document.
    /// </summary>
    public abstract class DocumentBase : NamedObject, IDocument
    {
        #region Private variables

        private int _id;
        private string _reference;

        #endregion 

        #region Constructors

        /// <summary>
        /// Creates a basic document.
        /// </summary>
        /// <param name="id">Unique ID for the document.</param>
        /// <param name="nameSource">Document name in the source repository.</param>
        /// <param name="nameTarget">Document name in the target repository.</param>
        /// <param name="reference">Reference to the document.</param>
        protected DocumentBase(int id, string nameSource, string nameTarget, string reference)
            : this(id, nameSource, nameTarget, reference, null)
        {
        }

        /// <summary>
        /// Creates a basic document.
        /// </summary>
        /// <param name="id">Unique ID for the document.</param>
        /// <param name="nameSource">Document name in the source repository.</param>
        /// <param name="nameTarget">Document name in the target repository.</param>
        /// <param name="reference">Reference to the document.</param>
        /// <param name="description">Description.</param>
        protected DocumentBase(int id, string nameSource, string nameTarget, string reference, string description)
            : base(nameSource, nameTarget, description)
        {
            if (string.IsNullOrEmpty(reference))
            {
                throw new ArgumentNullException("reference");
            }
            _id = id;
            _reference = reference;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Unique ID for the document.
        /// </summary>
        public virtual int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id == value)
                {
                    return;
                }
                _id = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Reference to the document.
        /// </summary>
        public virtual string Reference
        {
            get
            {
                return _reference;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                if (_reference == value)
                {
                    return;
                }
                _reference = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        #endregion
    }
}
