using System;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Marker functionality to a metadata object.
    /// </summary>
    public class MarkFunctionality : FunctionalityBase<string>, IMarkFunctionality
    {
        #region Constructor

        /// <summary>
        /// Creates a marker functionality to a metadata object.
        /// </summary>
        /// <param name="markerValue">Marker value for the functionality.</param>
        public MarkFunctionality(string markerValue)
            : base("MarkFunctionality", markerValue)
        {
            if (string.IsNullOrEmpty(markerValue))
            {
                throw new ArgumentNullException("markerValue");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Marker value for the functionality.
        /// </summary>
        public virtual new string Functionality
        {
            get
            {
                return base.Value;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                base.Value = value;
            }
        }

        /// <summary>
        /// The XML metadata value for the funtionality.
        /// </summary>
        public override string XmlValue
        {
            get
            {
                return Functionality;
            }
        }

        #endregion
    }
}
