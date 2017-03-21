using System;
using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests marker functionality to a metadata object.
    /// </summary>
    [TestFixture]
    public class MarkFunctionalityTests
    {
        /// <summary>
        /// Test that the constructor initialize a mark funcitonality.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeAMarkFunctionality()
        {
            var fixture = new Fixture();
            var markValue = fixture.CreateAnonymous<string>();

            var markFunctionality = new MarkFunctionality(markValue);
            Assert.That(markFunctionality, Is.Not.Null);
            Assert.That(markFunctionality.Name, Is.Not.Null);
            Assert.That(markFunctionality.Name, Is.Not.Empty);
            Assert.That(markFunctionality.Name, Is.EqualTo("MarkFunctionality"));
            Assert.That(markFunctionality.Functionality, Is.Not.Null);
            Assert.That(markFunctionality.Functionality, Is.Not.Empty);
            Assert.That(markFunctionality.Functionality, Is.EqualTo(markValue));
            Assert.That(markFunctionality.XmlValue, Is.Not.Null);
            Assert.That(markFunctionality.XmlValue, Is.Not.Empty);
            Assert.That(markFunctionality.XmlValue, Is.EqualTo(markFunctionality.Functionality));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the mark value is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsAnArgumentNullExceptionIfMarkValueIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new MarkFunctionality(null));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the mark value is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsAnArgumentNullExceptionIfMarkValueIsEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => new MarkFunctionality(string.Empty));
        }

        /// <summary>
        /// Test that the setter for Functionality raise the PropertyChanged event when value is changed.
        /// </summary>
        [Test]
        public void TestThatFunctionalitySetterRaisePropertyChangedIfValueIsChanged()
        {
            var fixture = new Fixture();

            var markFunctionality = new MarkFunctionality(fixture.CreateAnonymous<string>());
            Assert.That(markFunctionality, Is.Not.Null);

            var eventCalled = false;
            markFunctionality.PropertyChanged += ((s, e) =>
                                                      {
                                                          Assert.That(s, Is.Not.Null);
                                                          Assert.That(e, Is.Not.Null);
                                                          Assert.That(e.PropertyName, Is.Not.Null);
                                                          Assert.That(e.PropertyName, Is.Not.Empty);
                                                          if (e.PropertyName.Equals("Functionality"))
                                                          {
                                                              eventCalled = true;
                                                          }
                                                      });

            var newValue = fixture.CreateAnonymous<string>();
            markFunctionality.Functionality = newValue;
            Assert.That(markFunctionality.Functionality, Is.EqualTo(newValue));
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter for Functionality throws an ArgumentNullException if the value is null.
        /// </summary>
        [Test]
        public void TestThatFunctionalitySetterThrowsAnArgumentNullExceptionIfValueIsNull()
        {
            var fixture = new Fixture();

            var markFunctionality = new MarkFunctionality(fixture.CreateAnonymous<string>());
            Assert.That(markFunctionality, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => markFunctionality.Functionality = null);
        }

        /// <summary>
        /// Test that the setter for Functionality throws an ArgumentNullException if the value is empty.
        /// </summary>
        [Test]
        public void TestThatFunctionalitySetterThrowsAnArgumentNullExceptionIfValueIsEmpty()
        {
            var fixture = new Fixture();

            var markFunctionality = new MarkFunctionality(fixture.CreateAnonymous<string>());
            Assert.That(markFunctionality, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => markFunctionality.Functionality = string.Empty);
        }
    }
}
