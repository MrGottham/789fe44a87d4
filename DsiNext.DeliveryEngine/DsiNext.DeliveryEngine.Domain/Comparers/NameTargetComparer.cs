using System;
using DsiNext.DeliveryEngine.Domain.Interfaces.Comparers;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Domain.Comparers
{
    /// <summary>
    /// Equality comparer for target name on named objects.
    /// </summary>
    public class NameTargetComparer : INamedObjectComparer
    {
        #region IEqualityComparer<INamedObject> Members

        /// <summary>
        /// Compare target name on the two named objects.
        /// </summary>
        /// <param name="x">Named object.</param>
        /// <param name="y">Named object.</param>
        /// <returns>Whether the target name are eqaul on the two name objects.</returns>
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
            return string.Equals(x.NameTarget, y.NameTarget);
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
            if (obj.NameTarget == null)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, obj.NameTarget, "obj.NameTarget"));
            }
            return obj.NameTarget.GetHashCode();
        }

        #endregion
    }
}
