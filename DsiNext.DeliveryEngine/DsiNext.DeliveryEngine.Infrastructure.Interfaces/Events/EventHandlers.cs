namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events
{
    /// <summary>
    /// Event handler for an delivery engine event.
    /// </summary>
    /// <typeparam name="TEventArgs">Type of arguments to the event.</typeparam>
    /// <param name="sender">Object, which raises the event.</param>
    /// <param name="eventArgs">Arguments to the event.</param>
    public delegate void DeliveryEngineEventHandler<in TEventArgs>(object sender, TEventArgs eventArgs) where TEventArgs : IDeliveryEngineEventArgs;
}
