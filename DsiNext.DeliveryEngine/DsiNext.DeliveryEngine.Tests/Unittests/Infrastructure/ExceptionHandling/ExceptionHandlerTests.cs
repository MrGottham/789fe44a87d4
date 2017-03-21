using System;
using DsiNext.DeliveryEngine.Infrastructure.ExceptionHandling;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.ExceptionHandling;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Infrastructure.ExceptionHandling
{
    /// <summary>
    /// Tests the exception handler used by the delivery engine.
    /// </summary>
    [TestFixture]
    public class ExceptionHandlerTests
    {
        /// <summary>
        /// Test that the counstructor initialize the exception handler.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeExceptionHandler()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);
        }

        /// <summary>
        /// Test that the counstructor throws an ArgumentNullException if the exception logger is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfExceptionLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionHandler(null));
        }

        /// <summary>
        /// Test that the exception handler log an DeliveryEngineRepositoryException to the exception logger.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerLogDeliveryEngineRepositoryExceptionToExceptionLogger()
        {
            var fixture = new Fixture();

            var exceptionLoggerMock = MockRepository.GenerateMock<IExceptionLogger>();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => exceptionLoggerMock));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineRepositoryException>());

            exceptionLoggerMock.AssertWasCalled(m => m.LogException(Arg<DeliveryEngineRepositoryException>.Is.NotNull));
        }

        /// <summary>
        /// Test that the exception handler call the eventhandler which handle an DeliveryEngineRepositoryException.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerCallOnExceptionForDeliveryEngineRepositoryException()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            var eventCalled = false;
            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   Assert.That(eventArgs.Message, Is.Not.Null);
                                                   Assert.That(eventArgs.Message, Is.Not.Empty);
                                                   Assert.That(eventArgs.Exception, Is.Not.Null);
                                                   Assert.That(eventArgs.Exception, Is.TypeOf(typeof (DeliveryEngineRepositoryException)));
                                                   Assert.That(eventArgs.ExceptionObject, Is.Null);
                                                   Assert.That(eventArgs.CanContinue, Is.False);
                                                   eventCalled = true;
                                               };

            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineRepositoryException>());
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to false for DeliveryEngineRepositoryException if OnException is null.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToFalseForDeliveryEngineRepositoryExceptionIfOnExceptionIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineRepositoryException>(), out canContinue);
            Assert.That(canContinue, Is.False);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to false for DeliveryEngineRepositoryException if OnException set canContinue to false.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToFalseForDeliveryEngineRepositoryExceptionIfOnExceptionSetCanContinueToFalse()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   eventArgs.CanContinue = false;
                                               };

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineRepositoryException>(), out canContinue);
            Assert.That(canContinue, Is.False);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to true for DeliveryEngineRepositoryException if OnException set canContinue to true.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToTrueForDeliveryEngineRepositoryExceptionIfOnExceptionSetCanContinueToTrue()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   eventArgs.CanContinue = true;
                                               };

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineRepositoryException>(), out canContinue);
            Assert.That(canContinue, Is.True);
        }

        /// <summary>
        /// Test that the exception handler log an DeliveryEngineSystemException to the exception logger.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerLogDeliveryEngineSystemExceptionToExceptionLogger()
        {
            var fixture = new Fixture();

            var exceptionLoggerMock = MockRepository.GenerateMock<IExceptionLogger>();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => exceptionLoggerMock));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineSystemException>());

            exceptionLoggerMock.AssertWasCalled(m => m.LogException(Arg<DeliveryEngineSystemException>.Is.NotNull));
        }

        /// <summary>
        /// Test that the exception handler call the eventhandler which handle an DeliveryEngineSystemException.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerCallOnExceptionForDeliveryEngineSystemException()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            var eventCalled = false;
            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   Assert.That(eventArgs.Message, Is.Not.Null);
                                                   Assert.That(eventArgs.Message, Is.Not.Empty);
                                                   Assert.That(eventArgs.Exception, Is.Not.Null);
                                                   Assert.That(eventArgs.Exception, Is.TypeOf(typeof (DeliveryEngineSystemException)));
                                                   Assert.That(eventArgs.ExceptionObject, Is.Null);
                                                   Assert.That(eventArgs.CanContinue, Is.False);
                                                   eventCalled = true;
                                               };

            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineSystemException>());
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to false for DeliveryEngineSystemException if OnException is null.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToFalseForDeliveryEngineSystemExceptionIfOnExceptionIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineSystemException>(), out canContinue);
            Assert.That(canContinue, Is.False);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to false for DeliveryEngineSystemException if OnException set canContinue to false.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToFalseForDeliveryEngineSystemExceptionIfOnExceptionSetCanContinueToFalse()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   eventArgs.CanContinue = false;
                                               };

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineSystemException>(), out canContinue);
            Assert.That(canContinue, Is.False);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to true for DeliveryEngineSystemException if OnException set canContinue to true.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToTrueForDeliveryEngineSystemExceptionIfOnExceptionSetCanContinueToTrue()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   eventArgs.CanContinue = true;
                                               };

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineSystemException>(), out canContinue);
            Assert.That(canContinue, Is.True);
        }

        /// <summary>
        /// Test that the exception handler log an DeliveryEngineBusinessException to the exception logger.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerLogDeliveryEngineBusinessExceptionToExceptionLogger()
        {
            var fixture = new Fixture();

            var exceptionLoggerMock = MockRepository.GenerateMock<IExceptionLogger>();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => exceptionLoggerMock));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineBusinessException>());

            exceptionLoggerMock.AssertWasCalled(m => m.LogException(Arg<DeliveryEngineBusinessException>.Is.NotNull));
        }

        /// <summary>
        /// Test that the exception handler call the eventhandler which handle an DeliveryEngineBusinessException.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerCallOnExceptionForDeliveryEngineBusinessException()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            var eventCalled = false;
            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   Assert.That(eventArgs.Message, Is.Not.Null);
                                                   Assert.That(eventArgs.Message, Is.Not.Empty);
                                                   Assert.That(eventArgs.Exception, Is.Not.Null);
                                                   Assert.That(eventArgs.Exception, Is.TypeOf(typeof (DeliveryEngineBusinessException)));
                                                   Assert.That(eventArgs.ExceptionObject, Is.Null);
                                                   Assert.That(eventArgs.CanContinue, Is.False);
                                                   eventCalled = true;
                                               };

            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineBusinessException>());
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to false for DeliveryEngineBusinessException if OnException is null.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToFalseForDeliveryEngineBusinessExceptionIfOnExceptionIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineBusinessException>(), out canContinue);
            Assert.That(canContinue, Is.False);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to false for DeliveryEngineBusinessException if OnException set canContinue to false.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToFalseForDeliveryEngineBusinessExceptionIfOnExceptionSetCanContinueToFalse()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   eventArgs.CanContinue = false;
                                               };

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineBusinessException>(), out canContinue);
            Assert.That(canContinue, Is.False);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to true for DeliveryEngineBusinessException if OnException set canContinue to true.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToTrueForDeliveryEngineBusinessExceptionIfOnExceptionSetCanContinueToTrue()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   eventArgs.CanContinue = true;
                                               };
            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineBusinessException>(), out canContinue);
            Assert.That(canContinue, Is.True);
        }

        /// <summary>
        /// Test that the exception handler log an DeliveryEngineMetadataException to the exception logger.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerLogDeliveryEngineMetadataExceptionToExceptionLogger()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMetadataExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMetadataExceptionInfo>()));

            var exceptionLoggerMock = MockRepository.GenerateMock<IExceptionLogger>();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => exceptionLoggerMock));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineMetadataException>());

            exceptionLoggerMock.AssertWasCalled(m => m.LogException(Arg<DeliveryEngineMetadataException>.Is.NotNull));
        }

        /// <summary>
        /// Test that the exception handler call the eventhandler which handle an DeliveryEngineMetadataException.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerCallOnExceptionForDeliveryEngineMetadataException()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionInfoMock = MockRepository.GenerateMock<IDeliveryEngineMetadataExceptionInfo>();
            exceptionInfoMock.Expect(m => m.MetadataObject)
                .Return(fixture.CreateAnonymous<object>())
                .Repeat.Any();
            fixture.Customize<IDeliveryEngineMetadataExceptionInfo>(e => e.FromFactory(() => exceptionInfoMock));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            var eventCalled = false;
            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   Assert.That(eventArgs.Message, Is.Not.Null);
                                                   Assert.That(eventArgs.Message, Is.Not.Empty);
                                                   Assert.That(eventArgs.Exception, Is.Not.Null);
                                                   Assert.That(eventArgs.Exception, Is.TypeOf(typeof (DeliveryEngineMetadataException)));
                                                   Assert.That(eventArgs.ExceptionObject, Is.Not.Null);
                                                   Assert.That(eventArgs.ExceptionObject, Is.EqualTo(exceptionInfoMock.MetadataObject));
                                                   Assert.That(eventArgs.CanContinue, Is.False);
                                                   eventCalled = true;
                                               };

            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineMetadataException>());
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to false for DeliveryEngineMetadataException if OnException is null.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToFalseForDeliveryEngineMetadataExceptionIfOnExceptionIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMetadataExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMetadataExceptionInfo>()));
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineMetadataException>(), out canContinue);
            Assert.That(canContinue, Is.False);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to false for DeliveryEngineMetadataException if OnException set canContinue to false.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToFalseForDeliveryEngineMetadataExceptionIfOnExceptionSetCanContinueToFalse()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMetadataExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMetadataExceptionInfo>()));
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   eventArgs.CanContinue = false;
                                               };

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineMetadataException>(), out canContinue);
            Assert.That(canContinue, Is.False);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to true for DeliveryEngineMetadataException if OnException set canContinue to true.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToTrueForDeliveryEngineMetadataExceptionIfOnExceptionSetCanContinueToTrue()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMetadataExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMetadataExceptionInfo>()));
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   eventArgs.CanContinue = true;
                                               };

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineMetadataException>(), out canContinue);
            Assert.That(canContinue, Is.True);
        }

        /// <summary>
        /// Test that the exception handler log an DeliveryEngineMappingException to the exception logger.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerLogDeliveryEngineMappingExceptionToExceptionLogger()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMappingExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMappingExceptionInfo>()));

            var exceptionLoggerMock = MockRepository.GenerateMock<IExceptionLogger>();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => exceptionLoggerMock));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineMappingException>());

            exceptionLoggerMock.AssertWasCalled(m => m.LogException(Arg<DeliveryEngineMappingException>.Is.NotNull));
        }

        /// <summary>
        /// Test that the exception handler call the eventhandler which handle an DeliveryEngineMappingException.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerCallOnExceptionForDeliveryEngineMappingException()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionInfoMock = MockRepository.GenerateMock<IDeliveryEngineMappingExceptionInfo>();
            exceptionInfoMock.Expect(m => m.MappingObject)
                .Return(fixture.CreateAnonymous<object>())
                .Repeat.Any();
            exceptionInfoMock.Expect(m => m.MappingObjectData)
                .Return(fixture.CreateAnonymous<object>())
                .Repeat.Any();
            fixture.Customize<IDeliveryEngineMappingExceptionInfo>(e => e.FromFactory(() => exceptionInfoMock));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            var eventCalled = false;
            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   Assert.That(eventArgs.Message, Is.Not.Null);
                                                   Assert.That(eventArgs.Message, Is.Not.Empty);
                                                   Assert.That(eventArgs.Exception, Is.Not.Null);
                                                   Assert.That(eventArgs.Exception, Is.TypeOf(typeof (DeliveryEngineMappingException)));
                                                   Assert.That(eventArgs.ExceptionObject, Is.Not.Null);
                                                   Assert.That(eventArgs.ExceptionObject, Is.EqualTo(exceptionInfoMock.MappingObject));
                                                   Assert.That(eventArgs.ExceptionObjectData, Is.Not.Null);
                                                   Assert.That(eventArgs.ExceptionObjectData, Is.EqualTo(exceptionInfoMock.MappingObjectData));
                                                   Assert.That(eventArgs.CanContinue, Is.False);
                                                   eventCalled = true;
                                               };

            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineMappingException>());
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to false for DeliveryEngineMappingException if OnException is null.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToFalseForDeliveryEngineMappingExceptionIfOnExceptionIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMappingExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMappingExceptionInfo>()));
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineMappingException>(), out canContinue);
            Assert.That(canContinue, Is.False);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to false for DeliveryEngineMappingException if OnException set canContinue to false.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToFalseForDeliveryEngineMappingExceptionIfOnExceptionSetCanContinueToFalse()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMappingExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMappingExceptionInfo>()));
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   eventArgs.CanContinue = false;
                                               };

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineMappingException>(), out canContinue);
            Assert.That(canContinue, Is.False);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to true for DeliveryEngineMappingException if OnException set canContinue to true.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToTrueForDeliveryEngineMappingExceptionIfOnExceptionSetCanContinueToTrue()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMappingExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMappingExceptionInfo>()));
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   eventArgs.CanContinue = true;
                                               };

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineMappingException>(), out canContinue);
            Assert.That(canContinue, Is.True);
        }

        /// <summary>
        /// Test that the exception handler log an DeliveryEngineValidateException to the exception logger.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerLogDeliveryEngineValidateExceptionToExceptionLogger()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineValidateExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineValidateExceptionInfo>()));

            var exceptionLoggerMock = MockRepository.GenerateMock<IExceptionLogger>();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => exceptionLoggerMock));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineValidateException>());

            exceptionLoggerMock.AssertWasCalled(m => m.LogException(Arg<DeliveryEngineValidateException>.Is.NotNull));
        }

        /// <summary>
        /// Test that the exception handler call the eventhandler which handle an DeliveryEngineValidateException.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerCallOnExceptionForDeliveryEngineValidateException()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionInfoMock = MockRepository.GenerateMock<IDeliveryEngineValidateExceptionInfo>();
            exceptionInfoMock.Expect(m => m.ValidateObject)
                .Return(fixture.CreateAnonymous<object>())
                .Repeat.Any();
            exceptionInfoMock.Expect(m => m.ValidateObjectData)
                .Return(fixture.CreateAnonymous<object>())
                .Repeat.Any();
            fixture.Customize<IDeliveryEngineValidateExceptionInfo>(e => e.FromFactory(() => exceptionInfoMock));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            var eventCalled = false;
            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   Assert.That(eventArgs.Message, Is.Not.Null);
                                                   Assert.That(eventArgs.Message, Is.Not.Empty);
                                                   Assert.That(eventArgs.Exception, Is.Not.Null);
                                                   Assert.That(eventArgs.Exception, Is.TypeOf(typeof (DeliveryEngineValidateException)));
                                                   Assert.That(eventArgs.ExceptionObject, Is.Not.Null);
                                                   Assert.That(eventArgs.ExceptionObject, Is.EqualTo(exceptionInfoMock.ValidateObject));
                                                   Assert.That(eventArgs.ExceptionObjectData, Is.Not.Null);
                                                   Assert.That(eventArgs.ExceptionObjectData, Is.EqualTo(exceptionInfoMock.ValidateObjectData));
                                                   Assert.That(eventArgs.CanContinue, Is.False);
                                                   eventCalled = true;
                                               };

            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineValidateException>());
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to false for DeliveryEngineValidateException if OnException is null.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToFalseForDeliveryEngineValidateExceptionIfOnExceptionIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineValidateExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineValidateExceptionInfo>()));
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineValidateException>(), out canContinue);
            Assert.That(canContinue, Is.False);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to false for DeliveryEngineValidateException if OnException set canContinue to false.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToFalseForDeliveryEngineValidateExceptionIfOnExceptionSetCanContinueToFalse()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineValidateExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineValidateExceptionInfo>()));
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   eventArgs.CanContinue = false;
                                               };

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineValidateException>(), out canContinue);
            Assert.That(canContinue, Is.False);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to true for DeliveryEngineValidateException if OnException set canContinue to true.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToTrueForDeliveryEngineValidateExceptionIfOnExceptionSetCanContinueToTrue()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineValidateExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineValidateExceptionInfo>()));
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   eventArgs.CanContinue = true;
                                               };

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineValidateException>(), out canContinue);
            Assert.That(canContinue, Is.True);
        }

        /// <summary>
        /// Test that the exception handler log an DeliveryEngineConvertException to the exception logger.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerLogDeliveryEngineConvertExceptionToExceptionLogger()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineConvertExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineConvertExceptionInfo>()));

            var exceptionLoggerMock = MockRepository.GenerateMock<IExceptionLogger>();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => exceptionLoggerMock));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineConvertException>());

            exceptionLoggerMock.AssertWasCalled(m => m.LogException(Arg<DeliveryEngineConvertException>.Is.NotNull));
        }

        /// <summary>
        /// Test that the exception handler call the eventhandler which handle an DeliveryEngineConvertException.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerCallOnExceptionForDeliveryEngineConvertException()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionInfoMock = MockRepository.GenerateMock<IDeliveryEngineConvertExceptionInfo>();
            exceptionInfoMock.Expect(m => m.ConvertObject)
                .Return(fixture.CreateAnonymous<object>())
                .Repeat.Any();
            exceptionInfoMock.Expect(m => m.ConvertObjectData)
                .Return(fixture.CreateAnonymous<object>())
                .Repeat.Any();
            fixture.Customize<IDeliveryEngineConvertExceptionInfo>(e => e.FromFactory(() => exceptionInfoMock));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            var eventCalled = false;
            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   Assert.That(eventArgs.Message, Is.Not.Null);
                                                   Assert.That(eventArgs.Message, Is.Not.Empty);
                                                   Assert.That(eventArgs.Exception, Is.Not.Null);
                                                   Assert.That(eventArgs.Exception, Is.TypeOf(typeof (DeliveryEngineConvertException)));
                                                   Assert.That(eventArgs.ExceptionObject, Is.Not.Null);
                                                   Assert.That(eventArgs.ExceptionObject, Is.EqualTo(exceptionInfoMock.ConvertObject));
                                                   Assert.That(eventArgs.ExceptionObjectData, Is.Not.Null);
                                                   Assert.That(eventArgs.ExceptionObjectData, Is.EqualTo(exceptionInfoMock.ConvertObjectData));
                                                   Assert.That(eventArgs.CanContinue, Is.False);
                                                   eventCalled = true;
                                               };

            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineConvertException>());
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to false for DeliveryEngineConvertException if OnException is null.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToFalseForDeliveryEngineConvertExceptionIfOnExceptionIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineConvertExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineConvertExceptionInfo>()));
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineConvertException>(), out canContinue);
            Assert.That(canContinue, Is.False);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to false for DeliveryEngineConvertException if OnException set canContinue to false.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToFalseForDeliveryEngineConvertExceptionIfOnExceptionSetCanContinueToFalse()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineConvertExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineConvertExceptionInfo>()));
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   eventArgs.CanContinue = false;
                                               };

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineConvertException>(), out canContinue);
            Assert.That(canContinue, Is.False);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to true for DeliveryEngineConvertException if OnException set canContinue to true.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToTrueForDeliveryEngineConvertExceptionIfOnExceptionSetCanContinueToTrue()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineConvertExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineConvertExceptionInfo>()));
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   eventArgs.CanContinue = true;
                                               };

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<DeliveryEngineConvertException>(), out canContinue);
            Assert.That(canContinue, Is.True);
        }

        /// <summary>
        /// Test that the exception handler log an Exception to the exception logger.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerLogExceptionToExceptionLogger()
        {
            var fixture = new Fixture();

            var exceptionLoggerMock = MockRepository.GenerateMock<IExceptionLogger>();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => exceptionLoggerMock));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.HandleException(fixture.CreateAnonymous<Exception>());

            exceptionLoggerMock.AssertWasCalled(m => m.LogException(Arg<Exception>.Is.NotNull));
        }

        /// <summary>
        /// Test that the exception handler call the eventhandler which handle an Exception.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerCallOnExceptionForException()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            var eventCalled = false;
            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   Assert.That(eventArgs.Message, Is.Not.Null);
                                                   Assert.That(eventArgs.Message, Is.Not.Empty);
                                                   Assert.That(eventArgs.Exception, Is.Not.Null);
                                                   Assert.That(eventArgs.Exception, Is.TypeOf(typeof (Exception)));
                                                   Assert.That(eventArgs.ExceptionObject, Is.Null);
                                                   Assert.That(eventArgs.CanContinue, Is.False);
                                                   eventCalled = true;
                                               };

            exceptionHandle.HandleException(fixture.CreateAnonymous<Exception>());
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to false for Exception if OnException is null.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToFalseForExceptionIfOnExceptionIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<Exception>(), out canContinue);
            Assert.That(canContinue, Is.False);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to false for Exception if OnException set canContinue to false.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToFalseForExceptionIfOnExceptionSetCanContinueToFalse()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   eventArgs.CanContinue = false;
                                               };

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<Exception>(), out canContinue);
            Assert.That(canContinue, Is.False);
        }

        /// <summary>
        /// Test that the exception handler set canContinue to true for Exception if OnException set canContinue to true.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerSetCanContinueToTrueForExceptionIfOnExceptionSetCanContinueToTrue()
        {
            var fixture = new Fixture();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionLogger>()));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            exceptionHandle.OnException += (sender, eventArgs) =>
                                               {
                                                   Assert.That(sender, Is.Not.Null);
                                                   Assert.That(eventArgs, Is.Not.Null);
                                                   eventArgs.CanContinue = true;
                                               };

            bool canContinue;
            exceptionHandle.HandleException(fixture.CreateAnonymous<Exception>(), out canContinue);
            Assert.That(canContinue, Is.True);
        }

        /// <summary>
        /// Test that the exception handler throws an DeliveryEngineSystemException if an error occurs.
        /// </summary>
        [Test]
        public void TestThatExceptionHandlerThrowsDeliveryEngineSystemExceptionIfExceptionOccurs()
        {
            var fixture = new Fixture();

            var exceptionLoggerMock = MockRepository.GenerateMock<IExceptionLogger>();
            exceptionLoggerMock.Expect(m => m.LogException(Arg<Exception>.Is.NotNull))
                .Throw(fixture.CreateAnonymous<Exception>())
                .Repeat.Any();
            fixture.Customize<IExceptionLogger>(e => e.FromFactory(() => exceptionLoggerMock));

            var exceptionHandle = new ExceptionHandler(fixture.CreateAnonymous<IExceptionLogger>());
            Assert.That(exceptionHandle, Is.Not.Null);

            Assert.Throws<DeliveryEngineSystemException>(() => exceptionHandle.HandleException(fixture.CreateAnonymous<Exception>()));

            exceptionLoggerMock.AssertWasCalled(m => m.LogException(Arg<Exception>.Is.NotNull));
        }
    }
}
