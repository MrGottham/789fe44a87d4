using System.Collections.Generic;

namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    public interface IStaticMap<TSource, TTarget> : IMap
    {
        IDictionary<TSource, TTarget> Rules { get; }

        void AddRule(TSource source, TTarget target);
    }
}
