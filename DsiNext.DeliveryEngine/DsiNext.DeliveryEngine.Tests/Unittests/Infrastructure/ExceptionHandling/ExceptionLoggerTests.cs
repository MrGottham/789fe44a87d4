using System;
using System.IO;
using System.Reflection;
using DsiNext.DeliveryEngine.Infrastructure.ExceptionHandling;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Infrastructure.ExceptionHandling
{
    /// <summary>
    /// Tests the exception logger.
    /// </summary>
    public class ExceptionLoggerTests
    {
        #region Private constants

        private const string LogFileNamePattern = "DeliveryEngine.Exceptions.*.svclog";

        #endregion

        /// <summary>
        /// Test that the constructor initialize the exception logger.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeExceptionLogger()
        {
            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                exceptionLogger.Dispose();
            }
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the path for log files is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfPathIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionLogger(null));
        }

        /// <summary>
        /// Test that the constructor throws an DeliveryEngineRepositoryException if the path for log files does not exist.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsDeliveryEngineRepositoryExceptionIfPathDoesNotExist()
        {
            var fixture = new Fixture();
            Assert.Throws<DeliveryEngineRepositoryException>(() => new ExceptionLogger(new DirectoryInfo(fixture.CreateAnonymous<string>())));
        }

        /// <summary>
        /// Test that Dispose releases internal resources.
        /// </summary>
        [Test]
        public void TestDisposeReleasesInternalResources()
        {
            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                exceptionLogger.Dispose();

                var field = exceptionLogger.GetType().GetField("_disposed", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic);
                Assert.That(field, Is.Not.Null);
// ReSharper disable PossibleNullReferenceException
                Assert.That(field.GetValue(exceptionLogger), Is.True);
// ReSharper restore PossibleNullReferenceException
            }
        }

        /// <summary>
        /// Test that Dispose can be called more than once.
        /// </summary>
        [Test]
        public void TestDisposeCanBeCalledMoreThanOnce()
        {
            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                exceptionLogger.Dispose();
                exceptionLogger.Dispose();
            }
        }

        /// <summary>
        /// Test that LogException log an DeliveryEngineRepositoryException.
        /// </summary>
        [Test]
        public void TestThatLogExceptionLogDeliveryEngineRepositoryException()
        {
            var fixture = new Fixture();

            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                exceptionLogger.LogException(fixture.CreateAnonymous<DeliveryEngineRepositoryException>());
                exceptionLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(1));
        }

        /// <summary>
        /// Test that LogException throws an ArgumentNullException if DeliveryEngineRepositoryException is null.
        /// </summary>
        [Test]
        public void TestThatLogExceptionThrowsArgumentNullExceptionIfDeliveryEngineRepositoryExceptionIsNull()
        {
            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                DeliveryEngineRepositoryException exception = null;
                // ReSharper disable ExpressionIsAlwaysNull
                Assert.Throws<ArgumentNullException>(() => exceptionLogger.LogException(exception));
                // ReSharper restore ExpressionIsAlwaysNull
                exceptionLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(0));
        }

        /// <summary>
        /// Test that LogException log an DeliveryEngineSystemException.
        /// </summary>
        [Test]
        public void TestThatLogExceptionLogDeliveryEngineSystemExceptionException()
        {
            var fixture = new Fixture();

            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                exceptionLogger.LogException(fixture.CreateAnonymous<DeliveryEngineSystemException>());
                exceptionLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(1));
        }

        /// <summary>
        /// Test that LogException throws an ArgumentNullException if DeliveryEngineSystemException is null.
        /// </summary>
        [Test]
        public void TestThatLogExceptionThrowsArgumentNullExceptionIfDeliveryEngineSystemExceptionIsNull()
        {
            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                DeliveryEngineSystemException exception = null;
                // ReSharper disable ExpressionIsAlwaysNull
                Assert.Throws<ArgumentNullException>(() => exceptionLogger.LogException(exception));
                // ReSharper restore ExpressionIsAlwaysNull
                exceptionLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(0));
        }

        /// <summary>
        /// Test that LogException log an DeliveryEngineBusinessException.
        /// </summary>
        [Test]
        public void TestThatLogExceptionLogDeliveryEngineBusinessException()
        {
            var fixture = new Fixture();

            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                exceptionLogger.LogException(fixture.CreateAnonymous<DeliveryEngineBusinessException>());
                exceptionLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(1));
        }

        /// <summary>
        /// Test that LogException throws an ArgumentNullException if DeliveryEngineBusinessException is null.
        /// </summary>
        [Test]
        public void TestThatLogExceptionThrowsArgumentNullExceptionIfDeliveryEngineBusinessExceptionIsNull()
        {
            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                DeliveryEngineBusinessException exception = null;
                // ReSharper disable ExpressionIsAlwaysNull
                Assert.Throws<ArgumentNullException>(() => exceptionLogger.LogException(exception));
                // ReSharper restore ExpressionIsAlwaysNull
                exceptionLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(0));
        }

        /// <summary>
        /// Test that LogException log an DeliveryEngineMetadataException.
        /// </summary>
        [Test]
        public void TestThatLogExceptionLogDeliveryEngineMetadataException()
        {
            var fixture = new Fixture();

            var exceptionInformationMock = MockRepository.GenerateMock<IDeliveryEngineMetadataExceptionInfo>();
            exceptionInformationMock.Expect(m => m.ExceptionInfo)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();
            fixture.Customize<IDeliveryEngineMetadataExceptionInfo>(e => e.FromFactory(() => exceptionInformationMock));

            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                exceptionLogger.LogException(fixture.CreateAnonymous<DeliveryEngineMetadataException>());
                exceptionLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(1));
        }

        /// <summary>
        /// Test that LogException throws an ArgumentNullException if DeliveryEngineMetadataException is null.
        /// </summary>
        [Test]
        public void TestThatLogExceptionThrowsArgumentNullExceptionIfDeliveryEngineMetadataExceptionIsNull()
        {
            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                DeliveryEngineMetadataException exception = null;
                // ReSharper disable ExpressionIsAlwaysNull
                Assert.Throws<ArgumentNullException>(() => exceptionLogger.LogException(exception));
                // ReSharper restore ExpressionIsAlwaysNull
                exceptionLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(0));
        }

        /// <summary>
        /// Test that LogException log an DeliveryEngineMappingException.
        /// </summary>
        [Test]
        public void TestThatLogExceptionLogDeliveryEngineMappingException()
        {
            var fixture = new Fixture();

            var exceptionInformationMock = MockRepository.GenerateMock<IDeliveryEngineMappingExceptionInfo>();
            exceptionInformationMock.Expect(m => m.ExceptionInfo)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();
            fixture.Customize<IDeliveryEngineMappingExceptionInfo>(e => e.FromFactory(() => exceptionInformationMock));

            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                exceptionLogger.LogException(fixture.CreateAnonymous<DeliveryEngineMappingException>());
                exceptionLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(1));
        }

        /// <summary>
        /// Test that LogException throws an ArgumentNullException if DeliveryEngineMappingException is null.
        /// </summary>
        [Test]
        public void TestThatLogExceptionThrowsArgumentNullExceptionIfDeliveryEngineMappingExceptionIsNull()
        {
            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                DeliveryEngineMappingException exception = null;
                // ReSharper disable ExpressionIsAlwaysNull
                Assert.Throws<ArgumentNullException>(() => exceptionLogger.LogException(exception));
                // ReSharper restore ExpressionIsAlwaysNull
                exceptionLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(0));
        }

        /// <summary>
        /// Test that LogException log an DeliveryEngineConvertException.
        /// </summary>
        [Test]
        public void TestThatLogExceptionLogDeliveryEngineConvertException()
        {
            var fixture = new Fixture();

            var exceptionInformationMock = MockRepository.GenerateMock<IDeliveryEngineConvertExceptionInfo>();
            exceptionInformationMock.Expect(m => m.ExceptionInfo)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();
            fixture.Customize<IDeliveryEngineConvertExceptionInfo>(e => e.FromFactory(() => exceptionInformationMock));

            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                exceptionLogger.LogException(fixture.CreateAnonymous<DeliveryEngineConvertException>());
                exceptionLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(1));
        }

        /// <summary>
        /// Test that LogException throws an ArgumentNullException if DeliveryEngineConvertException is null.
        /// </summary>
        [Test]
        public void TestThatLogExceptionThrowsArgumentNullExceptionIfDeliveryEngineConvertExceptionIsNull()
        {
            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                DeliveryEngineConvertException exception = null;
                // ReSharper disable ExpressionIsAlwaysNull
                Assert.Throws<ArgumentNullException>(() => exceptionLogger.LogException(exception));
                // ReSharper restore ExpressionIsAlwaysNull
                exceptionLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(0));
        }

        /// <summary>
        /// Test that LogException log an DeliveryEngineValidateException.
        /// </summary>
        [Test]
        public void TestThatLogExceptionLogDeliveryDeliveryEngineValidateException()
        {
            var fixture = new Fixture();

            var exceptionInformationMock = MockRepository.GenerateMock<IDeliveryEngineValidateExceptionInfo>();
            exceptionInformationMock.Expect(m => m.ExceptionInfo)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();
            fixture.Customize<IDeliveryEngineValidateExceptionInfo>(e => e.FromFactory(() => exceptionInformationMock));

            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                exceptionLogger.LogException(fixture.CreateAnonymous<DeliveryEngineValidateException>());
                exceptionLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(1));
        }

        /// <summary>
        /// Test that LogException throws an ArgumentNullException if DeliveryEngineValidateException is null.
        /// </summary>
        [Test]
        public void TestThatLogExceptionThrowsArgumentNullExceptionIfDeliveryEngineValidateExceptionIsNull()
        {
            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                DeliveryEngineValidateException exception = null;
                // ReSharper disable ExpressionIsAlwaysNull
                Assert.Throws<ArgumentNullException>(() => exceptionLogger.LogException(exception));
                // ReSharper restore ExpressionIsAlwaysNull
                exceptionLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(0));
        }

        /// <summary>
        /// Test that LogException log an Exception.
        /// </summary>
        [Test]
        public void TestThatLogExceptionLogException()
        {
            var fixture = new Fixture();

            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                exceptionLogger.LogException(fixture.CreateAnonymous<Exception>());
                exceptionLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(1));
        }

        /// <summary>
        /// Test that LogException throws an ArgumentNullException if Exception is null.
        /// </summary>
        [Test]
        public void TestThatLogExceptionThrowsArgumentNullExceptionIfExceptionIsNull()
        {
            using (var exceptionLogger = new ExceptionLogger(GetPathForExceptionLogger()))
            {
                Assert.That(exceptionLogger, Is.Not.Null);

                Exception exception = null;
                // ReSharper disable ExpressionIsAlwaysNull
                Assert.Throws<ArgumentNullException>(() => exceptionLogger.LogException(exception));
                // ReSharper restore ExpressionIsAlwaysNull
                exceptionLogger.Dispose();
            }
            Assert.That(GetNumberOfLogFiles(), Is.EqualTo(0));
        }

        /// <summary>
        /// Gets the path for the exception logger.
        /// </summary>
        /// <returns>Path for the exception logger.</returns>
        private static DirectoryInfo GetPathForExceptionLogger()
        {
            var exceptionLoggerPath = new DirectoryInfo(Path.GetTempPath());
            Assert.That(exceptionLoggerPath, Is.Not.Null);
            Assert.That(exceptionLoggerPath.Exists, Is.True);

            foreach (var logFile in exceptionLoggerPath.GetFiles(LogFileNamePattern))
            {
                logFile.Delete();
            }
            Assert.That(exceptionLoggerPath.GetFiles(LogFileNamePattern).Length, Is.EqualTo(0));

            return exceptionLoggerPath;
        }

        /// <summary>
        /// Get the number of log files for the exception logger.
        /// </summary>
        /// <returns></returns>
        private static int GetNumberOfLogFiles()
        {
            var exceptionLoggerPath = new DirectoryInfo(Path.GetTempPath());
            Assert.That(exceptionLoggerPath, Is.Not.Null);
            Assert.That(exceptionLoggerPath.Exists, Is.True);

            return exceptionLoggerPath.GetFiles(LogFileNamePattern).Length;
        }
    }
}
