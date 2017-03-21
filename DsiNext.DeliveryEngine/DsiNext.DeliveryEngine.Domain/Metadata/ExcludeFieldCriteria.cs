using System;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Exclusion criteria on a field.
    /// </summary>
    public class ExcludeFieldCriteria : ExcludeCriteria, IExcludeFieldCriteria
    {
        #region Private variables

        private readonly IField _field;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an exclusion criteria on a field.
        /// </summary>
        /// <param name="field">Field for the exclusion criteria.</param>
        public ExcludeFieldCriteria(IField field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            _field = field;
        }

        #endregion

        #region IFieldCriteria Members

        /// <summary>
        /// Field for the exclusion criteria.
        /// </summary>
        public virtual IField Field
        {
            get
            {
                return _field;
            }
        }

        #endregion
    }
}
