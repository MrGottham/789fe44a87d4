namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.Log
{
    /// <summary>
    /// Interface for an information logger.
    /// </summary>
    public interface IInformationLogger
    {
        /// <summary>
        /// Log information.
        /// </summary>
        /// <param name="information">Information.</param>
        void LogInformation(string information);

        /// <summary>
        /// Log information.
        /// </summary>
        /// <param name="information">Information.</param>
        /// <param name="arguments">Arguments to log with information.</param>
        void LogInformation(string information, params object[] arguments);

        /// <summary>
        /// Log warning.
        /// </summary>
        /// <param name="warning">Warning.</param>
        void LogWarning(string warning);

        /// <summary>
        /// Log warning.
        /// </summary>
        /// <param name="warning">Warning.</param>
        /// <param name="arguments">Arguments to log with warning.</param>
        void LogWarning(string warning, params object[] arguments);
    }
}
