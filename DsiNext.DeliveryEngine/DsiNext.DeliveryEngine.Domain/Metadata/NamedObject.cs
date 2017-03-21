using System;
using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Named matadata object in the delivery engine.
    /// </summary>
    [Serializable]
    public class NamedObject : MetadataObjectBase, INamedObject
    {
        #region Private variables

        private string _nameSource;
        private string _nameTarget;
        private string _description;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor to support serialization.
        /// </summary>
        protected NamedObject()
        {
        }

        /// <summary>
        /// Creates a named matadata object in the delivery engine.
        /// </summary>
        /// <param name="nameSource">Name from the source repository.</param>
        /// <param name="nameTarget">Name in the target repository.</param>
        public NamedObject(string nameSource, string nameTarget)
            : this(nameSource, nameTarget, null)
        {
        }

        /// <summary>
        /// Creates a named matadata object in the delivery engine.
        /// </summary>
        /// <param name="nameSource">Name from the source repository.</param>
        /// <param name="nameTarget">Name in the target repository.</param>
        /// <param name="description">Description.</param>
        public NamedObject(string nameSource, string nameTarget, string description)
        {
            if (string.IsNullOrEmpty(nameSource))
            {
                throw new ArgumentNullException("nameSource");
            }
            if (string.IsNullOrEmpty(nameTarget))
            {
                throw new ArgumentNullException("nameTarget");
            }
            _nameSource = nameSource;
            _nameTarget = nameTarget;
            _description = description;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name from the source repository.
        /// </summary>
        public virtual string NameSource
        {
            get
            {
                return _nameSource;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                if (_nameSource == value)
                {
                    return;
                }
                _nameSource = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Name in the target repository.
        /// </summary>
        public virtual string NameTarget
        {
            get
            {
                return _nameTarget;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                if (_nameTarget == value)
                {
                    return;
                }
                _nameTarget = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Description.
        /// </summary>
        public virtual string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                if (_description == value)
                {
                    return;
                }
                _description = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Merged information about the metedata object.
        /// </summary>
        public virtual string ExceptionInfo
        {
            get
            {
                return string.Format("{0}, NameSource={1}, NameTarget={2}, Description={3}", GetType().Name, NameSource, NameTarget, Description);
            }
        }

        /// <summary>
        /// The metadata object.
        /// </summary>
        public virtual object MetadataObject
        {
            get
            {
                return this;
            }
        }

        #endregion
    }
}
