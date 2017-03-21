using System;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Events;

namespace DsiNext.DeliveryEngine.BusinessLogic.Events
{
    /// <summary>
    /// Arguments to the event raised before getting the data source.
    /// </summary>
    public class GetDataSourceEventArgs : EventArgs, IGetDataSourceEventArgs
    {
    }
}
