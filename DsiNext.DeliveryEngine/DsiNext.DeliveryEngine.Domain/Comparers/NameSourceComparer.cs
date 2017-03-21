using System;
using DsiNext.DeliveryEngine.Domain.Interfaces.Comparers;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Domain.Comparers
{
    /// <summary>
    /// Equality comparer for source name on named objects.
    /// </summary>
    public class NameSourceComparer : INamedObjectComparer 
    {
        #region IEqualityComparer<INamedObject> Members

        /// <summary>
        /// Compare source name on the two named objects.
        /// </summary>
        /// <param name="x">Named object.</param>
        /// <param name="y">Named object.</param>
        /// <returns>Whether the source name are eqaul on the two name objects.</returns>
        public virtual bool Equals(INamedObject x, INamedObject y)
        {
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }
            return string.Equals(x.NameSource, y.NameSource);
        }

        /// <summary>
        /// Returns the hash code for the named object.
        /// </summary>
        /// <param name="obj">Named object.</param>
        /// <returns>Hash code.</returns>
        public virtual int GetHashCode(INamedObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (obj.NameSource == null)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, obj.NameSource, "obj.NameSource"));
            }
            return obj.NameSource.GetHashCode();
        }

        #endregion
    }
}
