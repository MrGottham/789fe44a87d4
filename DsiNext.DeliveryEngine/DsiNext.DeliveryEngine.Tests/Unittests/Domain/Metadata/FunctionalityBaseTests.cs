using System;
using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests the basic functionality to a metadata object.
    /// </summary>
    [TestFixture]
    public class FunctionalityBaseTests
    {
        /// <summary>
        /// Private class for testing the basic functionality to a metadata object.
        /// </summary>
        private class MyFunctionality : FunctionalityBase<object>
        {
            /// <summary>
            /// Creates a private class for testing the basic functionality to a metadata object.
            /// </summary>
            /// <param name="name">Name of functionality.</param>
            /// <param name="functionality">Value for functionality.</param>
            public MyFunctionality(string name, object functionality)
                : base(name, functionality)
            {
            }
        }

        /// <summary>
        /// Test that the constructor initialize a functionality to a metadata object.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeAFunctionality()
        {
            var fixture = new Fixture();
            var name = fixture.CreateAnonymous<string>();
            var obj = fixture.CreateAnonymous<object>();

            var functionality = new MyFunctionality(name, obj);
            Assert.That(functionality, Is.Not.Null);
            Assert.That(functionality.Name, Is.Not.Null);
            Assert.That(functionality.Name, Is.Not.Empty);
            Assert.That(functionality.Name, Is.EqualTo(name));
            Assert.That(functionality.Value, Is.Not.Null);
            Assert.That(functionality.Value, Is.EqualTo(obj));
            Assert.That(functionality.Functionality, Is.Not.Null);
            Assert.That(functionality.Functionality, Is.EqualTo(obj));
            Assert.That(functionality.XmlValue, Is.Not.Null);
            Assert.That(functionality.XmlValue, Is.Not.Empty);
            Assert.That(functionality.XmlValue, Is.EqualTo(obj.GetType().Name));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the name for the functionality is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsAnArgumentNullExceptionIfNameIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new MyFunctionality(null, fixture.CreateAnonymous<object>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the name for the functionality is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsAnArgumentNullExceptionIfNameIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new MyFunctionality(string.Empty, fixture.CreateAnonymous<object>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the value for the functionality is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsAnArgumentNullExceptionIfFunctionalityIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new MyFunctionality(fixture.CreateAnonymous<string>(), null));
        }

        /// <summary>
        /// Test that the setter for Name raise the PropertyChanged event when value is changed.
        /// </summary>
        [Test]
        public void TestThatNameSetterRaisePropertyChangedIfValueIsChanged()
        {
            var fixture = new Fixture();

            var functionality = new MyFunctionality(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>());
            Assert.That(functionality, Is.Not.Null);

            var eventCalled = false;
            functionality.PropertyChanged += ((s, e) =>
                                                  {
                                                      Assert.That(s, Is.Not.Null);
                                                      Assert.That(e, Is.Not.Null);
                                                      Assert.That(e.PropertyName, Is.Not.Null);
                                                      Assert.That(e.PropertyName, Is.Not.Empty);
                                                      if (e.PropertyName.Equals("Name"))
                                                      {
                                                          eventCalled = true;
                                                      }
                                                  });

            functionality.Name = functionality.Name;
            Assert.That(eventCalled, Is.False);

            var newValue = fixture.CreateAnonymous<string>();
            functionality.Name = newValue;
            Assert.That(functionality.Name, Is.EqualTo(newValue));
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter for Name throws an ArgumentNullException if the value is null.
        /// </summary>
        [Test]
        public void TestThatNameSetterThrowsAnArgumentNullExceptionIfValueIsNull()
        {
            var fixture = new Fixture();

            var functionality = new MyFunctionality(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>());
            Assert.That(functionality, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => functionality.Name = null);
        }

        /// <summary>
        /// Test that the setter for Name throws an ArgumentNullException if the value is empty.
        /// </summary>
        [Test]
        public void TestThatNameSetterThrowsAnArgumentNullExceptionIfValueIsEmpty()
        {
            var fixture = new Fixture();

            var functionality = new MyFunctionality(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>());
            Assert.That(functionality, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => functionality.Name = string.Empty);
        }

        /// <summary>
        /// Test that the setter for Value raise the PropertyChanged event when value is changed.
        /// </summary>
        [Test]
        public void TestThatValueSetterRaisePropertyChangedIfValueIsChanged()
        {
            var fixture = new Fixture();

            var functionality = new MyFunctionality(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>());
            Assert.That(functionality, Is.Not.Null);

            var eventCalled = false;
            functionality.PropertyChanged += ((s, e) =>
                                                  {
                                                      Assert.That(s, Is.Not.Null);
                                                      Assert.That(e, Is.Not.Null);
                                                      Assert.That(e.PropertyName, Is.Not.Null);
                                                      Assert.That(e.PropertyName, Is.Not.Empty);
                                                      if (e.PropertyName.Equals("Value"))
                                                      {
                                                          eventCalled = true;
                                                      }
                                                  });

            functionality.Value = functionality.Value;
            Assert.That(eventCalled, Is.False);

            var newValue = fixture.CreateAnonymous<object>();
            functionality.Value = newValue;
            Assert.That(functionality.Value, Is.EqualTo(newValue));
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter for Value throws an ArgumentNullException if the value is null.
        /// </summary>
        [Test]
        public void TestThatValueSetterThrowsAnArgumentNullExceptionIfValueIsNull()
        {
            var fixture = new Fixture();

            var functionality = new MyFunctionality(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>());
            Assert.That(functionality, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => functionality.Value = null);
        }

        /// <summary>
        /// Test that the setter for Functionality throws an ArgumentNullException if the value is null.
        /// </summary>
        [Test]
        public void TestThatFunctionalitySetterThrowsAnArgumentNullExceptionIfValueIsNull()
        {
            var fixture = new Fixture();

            var functionality = new MyFunctionality(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>());
            Assert.That(functionality, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => functionality.Functionality = null);
        }

        /// <summary>
        /// Test that the setter for Functionality raise the PropertyChanged event when value is changed.
        /// </summary>
        [Test]
        public void TestThatFunctionalitySetterRaisePropertyChangedIfValueIsChanged()
        {
            var fixture = new Fixture();

            var functionality = new MyFunctionality(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>());
            Assert.That(functionality, Is.Not.Null);

            var eventCalled = false;
            functionality.PropertyChanged += ((s, e) =>
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

            functionality.Functionality = functionality.Functionality;
            Assert.That(eventCalled, Is.False);

            var newValue = fixture.CreateAnonymous<object>();
            functionality.Functionality = newValue;
            Assert.That(functionality.Functionality, Is.EqualTo(newValue));
            Assert.That(eventCalled, Is.True);
        }
    }
}
