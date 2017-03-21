using System;
using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Information about a view (query).
    /// </summary>
    public class View : NamedObject, IView
    {
        #region Private variables

        private string _sqlQuery;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates information about a view (query).
        /// </summary>
        /// <param name="nameSource">Name from the source repository.</param>
        /// <param name="nameTarget">Name in the target repository.</param>
        /// <param name="sqlQuery">Original SQL query which define the view (query).</param>
        public View(string nameSource, string nameTarget, string sqlQuery)
            : this(nameSource, nameTarget, sqlQuery, null)
        {
        }

        /// <summary>
        /// Creates information about a view (query).
        /// </summary>
        /// <param name="nameSource">Name from the source repository.</param>
        /// <param name="nameTarget">Name in the target repository.</param>
        /// <param name="sqlQuery">Original SQL query which define the view (query).</param>
        /// <param name="description">Description.</param>
        public View(string nameSource, string nameTarget, string sqlQuery, string description)
            : base(nameSource, nameTarget, description)
        {
            if (string.IsNullOrEmpty(sqlQuery))
            {
                throw new ArgumentNullException("sqlQuery");
            }
            _sqlQuery = sqlQuery;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Original SQL query which define the view (query).
        /// </summary>
        public virtual string SqlQuery
        {
            get
            {
                return _sqlQuery;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                if (_sqlQuery == value)
                {
                    return;
                }
                _sqlQuery = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        #endregion
    }
}
