using System;
using System.Collections.Specialized;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.Configuration
{
    /// <summary>
    /// Configuration repository to be used by the delivery engine.
    /// </summary>
    public class ConfigurationRepository : IConfigurationRepository
    {
        #region Constructor

        /// <summary>
        /// Creates an instance of the <see cref="ConfigurationRepository"/>.
        /// </summary>
        /// <param name="appSettingCollection">The collection of application settings.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="appSettingCollection"/> is null.</exception>
        public ConfigurationRepository(NameValueCollection appSettingCollection)
        {
            if (appSettingCollection == null)
            {
                throw new ArgumentNullException(nameof(appSettingCollection));
            }

            if (string.IsNullOrWhiteSpace(appSettingCollection["IncludeEmptyTables"]) || bool.TryParse(appSettingCollection["IncludeEmptyTables"], out var includeEmptyTables) == false)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.ApplicationSettingMissing, "IncludeEmptyTables"));
            }

            IncludeEmptyTables = includeEmptyTables;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether empty tables should be included in the delivery.
        /// </summary>
        public virtual bool IncludeEmptyTables { get; }

        #endregion
    }
}
