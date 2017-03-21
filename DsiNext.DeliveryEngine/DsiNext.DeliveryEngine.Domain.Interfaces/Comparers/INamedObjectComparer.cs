using System.Collections.Generic;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Interfaces.Comparers
{
    /// <summary>
    /// Interface for an equality comparer on named objects.
    /// </summary>
    public interface INamedObjectComparer : IEqualityComparer<INamedObject>
    {
    }
}
