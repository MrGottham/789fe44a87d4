using System;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Informations about a FORM classification.
    /// </summary>
    [Serializable]
    public class FormClass : NamedObject, IFormClass
    {
        #region Constructors

        /// <summary>
        /// Default constructor to support serialization.
        /// </summary>
        protected FormClass()
        {
        }

        /// <summary>
        /// Creates Informations about an FORM classification.
        /// </summary>
        /// <param name="nameSource">FORM classification in the source directory.</param>
        /// <param name="nameTarget">FORM classification in the target directory.</param>
        /// <param name="description">Text for FORM classification.</param>
        public FormClass(string nameSource, string nameTarget, string description)
            : base(nameSource, nameTarget, description)
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException("description");
            }
        }

        #endregion

        #region IFormClass Members

        /// <summary>
        /// FORM classification.
        /// </summary>
        public virtual string FormClassName
        {
            get
            {
                return NameTarget;
            }
        }

        /// <summary>
        /// Text for FORM classification.
        /// </summary>
        public virtual string FormClassText
        {
            get
            {
                return Description;
            }
        }

        #endregion
    }
}
