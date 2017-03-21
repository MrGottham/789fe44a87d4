using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Document which are associated to a table.
    /// </summary>
    public class TableDocument : DocumentBase, ITableDocument
    {
        #region Private variables

        private IField _field;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a document which are associated to a table.
        /// </summary>
        /// <param name="id">Unique ID for the document.</param>
        /// <param name="nameSource">Document name in the source repository.</param>
        /// <param name="nameTarget">Document name in the target repository.</param>
        /// <param name="reference">Reference to the document.</param>
        public TableDocument(int id, string nameSource, string nameTarget, string reference)
            : this(id, nameSource, nameTarget, reference, null)
        {
        }

        /// <summary>
        /// Creates a document which are associated to a table.
        /// </summary>
        /// <param name="id">Unique ID for the document.</param>
        /// <param name="nameSource">Document name in the source repository.</param>
        /// <param name="nameTarget">Document name in the target repository.</param>
        /// <param name="reference">Reference to the document.</param>
        /// <param name="description">Description.</param>
        public TableDocument(int id, string nameSource, string nameTarget, string reference, string description)
            : base(id, nameSource, nameTarget, reference, description)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Field which the documentet are associated with.
        /// </summary>
        public virtual IField Field
        {
            get
            {
                return _field;
            }
            set
            {
                if (_field == value)
                {
                    return;
                }
                _field = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        #endregion
    }
}
