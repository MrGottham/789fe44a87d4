using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Infrastructure.Log;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Infrastructure.Log
{
    /// <summary>
    /// Tests the information logger.
    /// </summary>
    [TestFixture]
    public class InformationLoggerTests
    {
        #region Private constants

        private const string LogFileNamePattern = "DeliveryEngine.Informations.*.txt";

        #endregion

        /// <summary>
        /// Test that the constructor initialize the information logger.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeInformationLogger()
        {
            using (var informationLogger = new InformationLogger(GetPathForformationLogger()))
            {
                Assert.That(informationLogger, Is.Not.Null);

                informationLogger.Dispose();
            }
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the path for log files is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfPathIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new InformationLogger(null));
        }

        /// <summary>
        /// Test that the constructor throws an DeliveryEngineRepositoryException if the path for log files does not exist.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsDeliveryEngineRepositoryExceptionIfPathDoesNotExist()
        {
            var fixture = new Fixture();
            Assert.Throws<DeliveryEngineRepositoryException>(() => new InformationLogger(new DirectoryInfo(fixture.CreateAnonymous<string>())));
        }

        /// <summary>
        /// Test that Dispose releases internal resources.
        /// </summary>
        [Test]
        public void TestThatDisposeReleasesInternalResources()
        {
            using (var informationLogger = new InformationLogger(GetPathForformationLogger()))
            {
                Assert.That(informationLogger, Is.Not.Null);

                informationLogger.Dispose();

                var field = informationLogger.GetType().GetField("_disposed", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic);
                Assert.That(field, Is.Not.Null);
                // ReSharper disable PossibleNullReferenceException
                Assert.That(field.GetValue(informationLogger), Is.True);
                // ReSharper restore PossibleNullReferenceException
            }
        }

        /// <summary>
        /// Test that Dispose can be called more than once.
        /// </summary>
        [Test]
        public void TestThatDisposeCanBeCalledMoreThanOnce()
        {
            using (var informationLogger = new InformationLogger(GetPathForformationLogger()))
            {
                Assert.That(informationLogger, Is.Not.Null);

                informationLogger.Dispose();
                informationLogger.Dispose();
            }
        }

        /// <summary>
        /// Test that LogInformation throws an ArgumentNullException if information is null.
        /// </summary>
        [Test]
        public void TestThatLogInformationThrowsArgumentNullExceptionIfInformationIsNull()
        {
            var fixture = new Fixture();
            using (var informationLogger = new InformationLogger(GetPathForformationLogger()))
            {
                Assert.Throws<ArgumentNullException>(() => informationLogger.LogInformation(null));
                Assert.Throws<ArgumentNullException>(() => informationLogger.LogInformation(null, fixture.CreateMany<object>(5).ToArray()));
                informationLogger.Dispose();
            }
        }

        /// <summary>
        /// Test that LogInformation throws an ArgumentNullException if information is empty.
        /// </summary>
        [Test]
        public void TestThatLogInformationThrowsArgumentNullExceptionIfInformationIsEmpty()
        {
            var fixture = new Fixture();
            using (var informationLogger = new InformationLogger(GetPathForformationLogger()))
            {
                Assert.Throws<ArgumentNullException>(() => informationLogger.LogInformation(string.Empty));
                Assert.Throws<ArgumentNullException>(() => informationLogger.LogInformation(string.Empty, fixture.CreateMany<object>(5).ToArray()));
                informationLogger.Dispose();
            }
        }

        /// <summary>
        /// Test that LogInformation writes information to the log.
        /// </summary>
        [Test]
        public void TestThatLogInformationWritesInformationToLog()
        {
            var fixture = new Fixture();
            using (var informationLogger = new InformationLogger(GetPathForformationLogger()))
            {
                informationLogger.LogInformation(fixture.CreateAnonymous<string>());
                informationLogger.LogInformation("{0} {1} {2}", fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<object>());
                informationLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(1));
        }

        /// <summary>
        /// Test that LogWarning throws an ArgumentNullException if warning is null.
        /// </summary>
        [Test]
        public void TestThatLogWarningThrowsArgumentNullExceptionIfWarningIsNull()
        {
            var fixture = new Fixture();
            using (var informationLogger = new InformationLogger(GetPathForformationLogger()))
            {
                Assert.Throws<ArgumentNullException>(() => informationLogger.LogWarning(null));
                Assert.Throws<ArgumentNullException>(() => informationLogger.LogWarning(null, fixture.CreateMany<object>(5).ToArray()));
                informationLogger.Dispose();
            }
        }

        /// <summary>
        /// Test that LogWarning throws an ArgumentNullException if warning is empty.
        /// </summary>
        [Test]
        public void TestThatLogWarningThrowsArgumentNullExceptionIfWarningIsEmpty()
        {
            var fixture = new Fixture();
            using (var informationLogger = new InformationLogger(GetPathForformationLogger()))
            {
                Assert.Throws<ArgumentNullException>(() => informationLogger.LogWarning(string.Empty));
                Assert.Throws<ArgumentNullException>(() => informationLogger.LogWarning(string.Empty, fixture.CreateMany<object>(5).ToArray()));
                informationLogger.Dispose();
            }
        }

        /// <summary>
        /// Test that LogWarning writes warning to the log.
        /// </summary>
        [Test]
        public void TestThatLogWarningWritesWarningToLog()
        {
            var fixture = new Fixture();
            using (var informationLogger = new InformationLogger(GetPathForformationLogger()))
            {
                informationLogger.LogWarning(fixture.CreateAnonymous<string>());
                informationLogger.LogWarning("{0} {1} {2}", fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<object>());
                informationLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(1));
        }

        /// <summary>
        /// Gets the path for the information logger.
        /// </summary>
        /// <returns>Path for the information  logger.</returns>
        private static DirectoryInfo GetPathForformationLogger()
        {
            var informationLoggerPath = new DirectoryInfo(Path.GetTempPath());
            Assert.That(informationLoggerPath, Is.Not.Null);
            Assert.That(informationLoggerPath.Exists, Is.True);

            foreach (var logFile in informationLoggerPath.GetFiles(LogFileNamePattern))
            {
                logFile.Delete();
            }
            Assert.That(informationLoggerPath.GetFiles(LogFileNamePattern).Length, Is.EqualTo(0));

            return informationLoggerPath;
        }

        /// <summary>
        /// Get the number of log files for the information logger.
        /// </summary>
        /// <returns></returns>
        private static int GetNumberOfLogFiles()
        {
            var informationLoggerPath = new DirectoryInfo(Path.GetTempPath());
            Assert.That(informationLoggerPath, Is.Not.Null);
            Assert.That(informationLoggerPath.Exists, Is.True);

            return informationLoggerPath.GetFiles(LogFileNamePattern).Length;
        }
    }
}
