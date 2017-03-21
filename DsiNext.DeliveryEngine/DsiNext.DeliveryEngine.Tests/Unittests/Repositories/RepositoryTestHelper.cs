using System;
using System.Configuration;
using System.IO;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories
{
    /// <summary>
    /// Helper class for testing repositories.
    /// </summary>
    public static class RepositoryTestHelper
    {
        /// <summary>
        /// Gets the source test path for an old delivery format.
        /// </summary>
        /// <returns>Source test path for an old delivery format.</returns>
        public static DirectoryInfo GetSourcePathForTest()
        {
            return new DirectoryInfo(Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["SourcePath"]));
        }

        /// <summary>
        /// Gets the source test path for an old delivery format to the Oracle test database.
        /// </summary>
        /// <returns>Source test path for an old delivery format to the Oracle test database.</returns>
        public static DirectoryInfo GetSourcePathForOracleTest()
        {
            return new DirectoryInfo(Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["SourcePathForOracle"]));
        }
    }
}
