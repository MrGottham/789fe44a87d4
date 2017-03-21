using System.Collections.ObjectModel;
using System.Collections.Generic;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;

namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    public interface IKey : INamedObject, IDeliveryEngineValidateExceptionInfo
    {
        ITable Table { get; }

        ReadOnlyObservableCollection<KeyValuePair<IField, IMap>> Fields { get; }

        void AddField(IField field);

        void AddField(IField field, IMap map);
    }
}
