using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;

namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    public interface IMap : IDeliveryEngineMappingExceptionInfo
    {
        TTarget MapValue<TSource, TTarget>(TSource source);
    }
}
