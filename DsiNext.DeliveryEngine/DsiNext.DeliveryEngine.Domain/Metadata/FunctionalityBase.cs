using System;
using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Basic functionality to a metadata object.
    /// </summary>
    /// <typeparam name="T">Type of the value for functionality.</typeparam>
    public abstract class FunctionalityBase<T> : MetadataObjectBase, IFunctionality where T : class 
    {
        #region Private variables

        private string _name;
        private T _functionality;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a functionality to a metadata object.
        /// </summary>
        /// <param name="name">Name of functionality.</param>
        /// <param name="functionality">Value for functionality.</param>
        protected FunctionalityBase(string name, T functionality)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            if (functionality == null)
            {
                throw new ArgumentNullException("functionality");
            }
            _name = name;
            _functionality = functionality;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name of functionality.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                if (_name == value)
                {
                    return;
                }
                _name = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Value.
        /// </summary>
        public virtual T Value
        {
            get
            {
                return _functionality;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (value.Equals(_functionality))
                {
                    return;
                }
                _functionality = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
                RaisePropertyChanged(this, "Functionality");
            }
        }

        /// <summary>
        /// Value for functionality.
        /// </summary>
        public virtual object Functionality
        {
            get
            {
                return Value;
            }
            set
            {
                Value = value as T;
            }
        }

        /// <summary>
        /// The XML metadata value for the funtionality.
        /// </summary>
        public virtual string XmlValue
        {
            get
            {
                return typeof(T).Name;
            }
        }

        #endregion
    }
}
