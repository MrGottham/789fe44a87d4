using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    public interface IDynamicMap<in TSource, out TTarget> : IMap
    {
        ReadOnlyObservableCollection<KeyValuePair<Regex, string>> Rules { get; }

        void AddRule(Regex regex, string replacement);
    }
}
