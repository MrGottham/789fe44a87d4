using System;
using System.Collections.Generic;

namespace DsiNext.DeliveryEngine.Repositories.Interfaces.Comparers
{
    /// <summary>
    /// Class for comparing key values made by methods in DataRepositoryHelper.
    /// </summary>
    public class KeyValueComparer : IEqualityComparer<string>
    {
        #region IEqualityComparer<string> Members

        /// <summary>
        /// Determines whether the specified key values are equal.
        /// </summary>
        /// <param name="x">Key value.</param>
        /// <param name="y">Key value.</param>
        /// <returns>Whether the specified key values are equal.</returns>
        public virtual bool Equals(string x, string y)
        {
            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Returns a hash code for the key value.
        /// </summary>
        /// <param name="keyValue">Key value.</param>
        /// <returns>Hash code for the key value object.</returns>
        public virtual int GetHashCode(string keyValue)
        {
            return keyValue == null ? 0 : keyValue.GetHashCode();
        }

        #endregion
    }
}
