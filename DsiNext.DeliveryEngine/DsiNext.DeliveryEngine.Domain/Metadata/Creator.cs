using System;
using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Creator of the archive.
    /// </summary>
    public class Creator : NamedObject, ICreator
    {
        #region Private variables

        private DateTime _periodStart;
        private DateTime _periodEnd;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a creator of the archive.
        /// </summary>
        /// <param name="nameSource">Name of the creator in the source repository.</param>
        /// <param name="nameTarget">Name of the creator in the target repository.</param>
        /// <param name="periodStart">Timestamp for the start of the period.</param>
        /// <param name="periodEnd">Timestamp for the end of the period.</param>
        public Creator(string nameSource, string nameTarget, DateTime periodStart, DateTime periodEnd)
            : this(nameSource, nameTarget, null, periodStart, periodEnd)
        {
        }

        /// <summary>
        /// Creates a creator of the archive.
        /// </summary>
        /// <param name="nameSource">Name of the creator in the source repository.</param>
        /// <param name="nameTarget">Name of the creator in the target repository.</param>
        /// <param name="description">Descriptio for the creator.</param>
        /// <param name="periodStart">Timestamp for the start of the period.</param>
        /// <param name="periodEnd">Timestamp for the end of the period.</param>
        public Creator(string nameSource, string nameTarget, string description, DateTime periodStart, DateTime periodEnd)
            : base(nameSource, nameTarget, description)
        {
            _periodStart = periodStart;
            _periodEnd = periodEnd;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Timestamp for the start of the period.
        /// </summary>
        public virtual DateTime PeriodStart
        {
            get
            {
                return _periodStart;
            }
            set
            {
                if (_periodStart == value)
                {
                    return;
                }
                _periodStart = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Timestamp for the end of the period.
        /// </summary>
        public virtual DateTime PeriodEnd
        {
            get
            {
                return _periodEnd;
            }
            set
            {
                if (_periodEnd == value)
                {
                    return;
                }
                _periodEnd = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        #endregion
    }
}
