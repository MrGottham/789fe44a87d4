using System;
using DsiNext.DeliveryEngine.Domain;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain
{
    /// <summary>
    /// Tests the basic domain object in the delivery engine.
    /// </summary>
    [TestFixture]
    public class DomainObjectBaseTests
    {
        /// <summary>
        /// Own class for testing the basic domain object in the delivery engine.
        /// </summary>
        private class MyDomainObject : DomainObjectBase
        {
            /// <summary>
            /// Raise an PropertyChanged event.
            /// </summary>
            /// <param name="sender">Object who are raising the event.</param>
            /// <param name="propertyName">Name of the property which are changed.</param>
            public new void RaisePropertyChanged(object sender, string propertyName)
            {
                base.RaisePropertyChanged(sender, propertyName);
            }
        }

        /// <summary>
        /// Test that the constructor initialize an domain object.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeAnDomainObject()
        {
            var domainObject = new MyDomainObject();
            Assert.That(domainObject, Is.Not.Null);
        }

        /// <summary>
        /// Test that RaisePropertyChanged throws an ArgumentNullException if the sender is null.
        /// </summary>
        [Test]
        public void TestThatRaisePropertyChangedThrowsAnArgumentNullExceptionIfSenderIsNull()
        {
            var fixture = new Fixture();

            var domainObject = new MyDomainObject();
            Assert.That(domainObject, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => domainObject.RaisePropertyChanged(null, fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that RaisePropertyChanged throws an ArgumentNullException if the property name is null.
        /// </summary>
        [Test]
        public void TestThatRaisePropertyChangedThrowsAnArgumentNullExceptionIfPropertyNameIsNull()
        {
            var fixture = new Fixture();

            var domainObject = new MyDomainObject();
            Assert.That(domainObject, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => domainObject.RaisePropertyChanged(fixture.CreateAnonymous<object>(), null));
        }

        /// <summary>
        /// Test that RaisePropertyChanged throws an ArgumentNullException if the property name is empty.
        /// </summary>
        [Test]
        public void TestThatRaisePropertyChangedThrowsAnArgumentNullExceptionIfPropertyNameIsEmpty()
        {
            var fixture = new Fixture();

            var domainObject = new MyDomainObject();
            Assert.That(domainObject, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => domainObject.RaisePropertyChanged(fixture.CreateAnonymous<object>(), string.Empty));
        }

        /// <summary>
        /// Test that RaisePropertyChanged returns if the event handler is not set.
        /// </summary>
        [Test]
        public void TestThatRaisePropertyChangedReturnsIfEventHandlerNotSet()
        {
            var fixture = new Fixture();
            var sender = fixture.CreateAnonymous<object>();
            var propertyName = fixture.CreateAnonymous<string>();

            var domainObject = new MyDomainObject();
            Assert.That(domainObject, Is.Not.Null);

            domainObject.RaisePropertyChanged(sender, propertyName);
        }

        /// <summary>
        /// Test that RaisePropertyChanged calls the event handler if it is set.
        /// </summary>
        [Test]
        public void TestThatRaisePropertyChangedCallEventHandler()
        {
            var fixture = new Fixture();
            var sender = fixture.CreateAnonymous<object>();
            var propertyName = fixture.CreateAnonymous<string>();

            var domainObject = new MyDomainObject();
            Assert.That(domainObject, Is.Not.Null);

            var eventHandlerCalled = false;
            domainObject.PropertyChanged += ((s, e) =>
                                                 {
                                                     Assert.That(s, Is.Not.Null);
                                                     Assert.That(s, Is.EqualTo(sender));
                                                     Assert.That(e, Is.Not.Null);
                                                     Assert.That(e.PropertyName, Is.Not.Null);
                                                     Assert.That(e.PropertyName, Is.Not.Empty);
                                                     Assert.That(e.PropertyName, Is.EqualTo(propertyName));
                                                     eventHandlerCalled = true;
                                                 });

            domainObject.RaisePropertyChanged(sender, propertyName);

            Assert.That(eventHandlerCalled, Is.True);
        }
    }
}
