using System;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests the data source.
    /// </summary>
    [TestFixture]
    public class DataSourceTests
    {
        /// <summary>
        /// Test that the constructor initialize a datasource without a description.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDataSourceWithoutDescription()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var archiveInformationPackageId = fixture.CreateAnonymous<string>();
            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var archivePeriodStart = fixture.CreateAnonymous<DateTime>();
            var archivePeriodEnd = fixture.CreateAnonymous<DateTime>().AddDays(7);
            var dataSource = new DataSource(archiveInformationPackageId, nameSource, nameTarget, archivePeriodStart, archivePeriodEnd);
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.ConnectionString, Is.Null);
            Assert.That(dataSource.ArchiveInformationPackageId, Is.Not.Null);
            Assert.That(dataSource.ArchiveInformationPackageId, Is.Not.Empty);
            Assert.That(dataSource.ArchiveInformationPackageId, Is.EqualTo(archiveInformationPackageId));
            Assert.That(dataSource.ArchiveInformationPackageIdPrevious, Is.EqualTo(0));
            Assert.That(dataSource.NameSource, Is.Not.Null);
            Assert.That(dataSource.NameSource, Is.Not.Empty);
            Assert.That(dataSource.NameSource, Is.EqualTo(nameSource));
            Assert.That(dataSource.NameTarget, Is.Not.Null);
            Assert.That(dataSource.NameTarget, Is.Not.Empty);
            Assert.That(dataSource.NameTarget, Is.EqualTo(nameTarget));
            Assert.That(dataSource.Description, Is.Null);
            Assert.That(dataSource.ArchivePeriodStart, Is.EqualTo(archivePeriodStart));
            Assert.That(dataSource.ArchivePeriodEnd, Is.EqualTo(archivePeriodEnd));
            Assert.That(dataSource.ArchiveInformationPacketType, Is.False);
            Assert.That(dataSource.ArchiveType, Is.False);
            Assert.That(dataSource.SystemPurpose, Is.Null);
            Assert.That(dataSource.SystemContent, Is.Null);
            Assert.That(dataSource.RegionNum, Is.False);
            Assert.That(dataSource.KomNum, Is.False);
            Assert.That(dataSource.CprNum, Is.False);
            Assert.That(dataSource.CvrNum, Is.False);
            Assert.That(dataSource.MatrikNum, Is.False);
            Assert.That(dataSource.BbrNum, Is.False);
            Assert.That(dataSource.WhoSygKod, Is.False);
            Assert.That(dataSource.FormVersion, Is.Null);
            Assert.That(dataSource.ContainsDigitalDocuments, Is.False);
            Assert.That(dataSource.SearchRelatedOtherRecords, Is.False);
            Assert.That(dataSource.SystemFileConcept, Is.False);
            Assert.That(dataSource.MultipleDataCollection, Is.False);
            Assert.That(dataSource.PersonalDataRestrictedInfo, Is.False);
            Assert.That(dataSource.OtherAccessTypeRestrictions, Is.False);
            Assert.That(dataSource.ArchiveApproval, Is.Null);
            Assert.That(dataSource.ArchiveRestrictions, Is.Null);
            Assert.That(dataSource.Tables, Is.Not.Null);
            Assert.That(dataSource.Tables.Count, Is.EqualTo(0));
            Assert.That(dataSource.Views, Is.Not.Null);
            Assert.That(dataSource.Views.Count, Is.EqualTo(0));
            Assert.That(dataSource.Creators, Is.Not.Null);
            Assert.That(dataSource.Creators.Count, Is.EqualTo(0));
            Assert.That(dataSource.AlternativeSystemNames, Is.Not.Null);
            Assert.That(dataSource.AlternativeSystemNames.Count, Is.EqualTo(0));
            Assert.That(dataSource.SourceNames, Is.Not.Null);
            Assert.That(dataSource.SourceNames.Count, Is.EqualTo(0));
            Assert.That(dataSource.UserNames, Is.Not.Null);
            Assert.That(dataSource.UserNames.Count, Is.EqualTo(0));
            Assert.That(dataSource.PredecessorNames, Is.Not.Null);
            Assert.That(dataSource.PredecessorNames.Count, Is.EqualTo(0));
            Assert.That(dataSource.FormClasses, Is.Not.Null);
            Assert.That(dataSource.FormClasses.Count, Is.EqualTo(0));
            Assert.That(dataSource.RelatedRecordsNames, Is.Not.Null);
            Assert.That(dataSource.RelatedRecordsNames.Count, Is.EqualTo(0));
            Assert.That(dataSource.ContextDocuments, Is.Not.Null);
            Assert.That(dataSource.ContextDocuments.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Test that the constructor without a description throws an ArgumentNullException if the archive information package id is null.
        /// </summary>
        [Test]
        public void TestThatConstructorWithoutDescriptionThrowsArgumentNullExceptionIfArchiveInformationPackageIdIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var archivePeriodStart = fixture.CreateAnonymous<DateTime>();
            var archivePeriodEnd = fixture.CreateAnonymous<DateTime>().AddDays(7);
            Assert.Throws<ArgumentNullException>(() => new DataSource(null, nameSource, nameTarget, archivePeriodStart, archivePeriodEnd));
        }

        /// <summary>
        /// Test that the constructor without a description throws an ArgumentNullException if the archive information package id is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorWithoutDescriptionThrowsArgumentNullExceptionIfArchiveInformationPackageIdIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var archivePeriodStart = fixture.CreateAnonymous<DateTime>();
            var archivePeriodEnd = fixture.CreateAnonymous<DateTime>().AddDays(7);
            Assert.Throws<ArgumentNullException>(() => new DataSource(string.Empty, nameSource, nameTarget, archivePeriodStart, archivePeriodEnd));
        }

        /// <summary>
        /// Test that the constructor initialize a datasource with a description.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDataSourceWithDescription()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var archiveInformationPackageId = fixture.CreateAnonymous<string>();
            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var description = fixture.CreateAnonymous<string>();
            var archivePeriodStart = fixture.CreateAnonymous<DateTime>();
            var archivePeriodEnd = fixture.CreateAnonymous<DateTime>().AddDays(7);
            var dataSource = new DataSource(archiveInformationPackageId, nameSource, nameTarget, description, archivePeriodStart, archivePeriodEnd);
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.ConnectionString, Is.Null);
            Assert.That(dataSource.ArchiveInformationPackageId, Is.Not.Null);
            Assert.That(dataSource.ArchiveInformationPackageId, Is.Not.Empty);
            Assert.That(dataSource.ArchiveInformationPackageId, Is.EqualTo(archiveInformationPackageId));
            Assert.That(dataSource.ArchiveInformationPackageIdPrevious, Is.EqualTo(0));
            Assert.That(dataSource.NameSource, Is.Not.Null);
            Assert.That(dataSource.NameSource, Is.Not.Empty);
            Assert.That(dataSource.NameSource, Is.EqualTo(nameSource));
            Assert.That(dataSource.NameTarget, Is.Not.Null);
            Assert.That(dataSource.NameTarget, Is.Not.Empty);
            Assert.That(dataSource.NameTarget, Is.EqualTo(nameTarget));
            Assert.That(dataSource.Description, Is.Not.Null);
            Assert.That(dataSource.Description, Is.Not.Empty);
            Assert.That(dataSource.Description, Is.EqualTo(description));
            Assert.That(dataSource.ArchivePeriodStart, Is.EqualTo(archivePeriodStart));
            Assert.That(dataSource.ArchivePeriodEnd, Is.EqualTo(archivePeriodEnd));
            Assert.That(dataSource.ArchiveInformationPacketType, Is.False);
            Assert.That(dataSource.ArchiveType, Is.False);
            Assert.That(dataSource.SystemPurpose, Is.Null);
            Assert.That(dataSource.SystemContent, Is.Null);
            Assert.That(dataSource.RegionNum, Is.False);
            Assert.That(dataSource.KomNum, Is.False);
            Assert.That(dataSource.CprNum, Is.False);
            Assert.That(dataSource.CvrNum, Is.False);
            Assert.That(dataSource.MatrikNum, Is.False);
            Assert.That(dataSource.BbrNum, Is.False);
            Assert.That(dataSource.WhoSygKod, Is.False);
            Assert.That(dataSource.FormVersion, Is.Null);
            Assert.That(dataSource.ContainsDigitalDocuments, Is.False);
            Assert.That(dataSource.SearchRelatedOtherRecords, Is.False);
            Assert.That(dataSource.SystemFileConcept, Is.False);
            Assert.That(dataSource.MultipleDataCollection, Is.False);
            Assert.That(dataSource.PersonalDataRestrictedInfo, Is.False);
            Assert.That(dataSource.OtherAccessTypeRestrictions, Is.False);
            Assert.That(dataSource.ArchiveApproval, Is.Null);
            Assert.That(dataSource.ArchiveRestrictions, Is.Null);
            Assert.That(dataSource.Tables, Is.Not.Null);
            Assert.That(dataSource.Tables.Count, Is.EqualTo(0));
            Assert.That(dataSource.Views, Is.Not.Null);
            Assert.That(dataSource.Views.Count, Is.EqualTo(0));
            Assert.That(dataSource.Creators, Is.Not.Null);
            Assert.That(dataSource.Creators.Count, Is.EqualTo(0));
            Assert.That(dataSource.AlternativeSystemNames, Is.Not.Null);
            Assert.That(dataSource.AlternativeSystemNames.Count, Is.EqualTo(0));
            Assert.That(dataSource.SourceNames, Is.Not.Null);
            Assert.That(dataSource.SourceNames.Count, Is.EqualTo(0));
            Assert.That(dataSource.UserNames, Is.Not.Null);
            Assert.That(dataSource.UserNames.Count, Is.EqualTo(0));
            Assert.That(dataSource.PredecessorNames, Is.Not.Null);
            Assert.That(dataSource.PredecessorNames.Count, Is.EqualTo(0));
            Assert.That(dataSource.FormClasses, Is.Not.Null);
            Assert.That(dataSource.FormClasses.Count, Is.EqualTo(0));
            Assert.That(dataSource.RelatedRecordsNames, Is.Not.Null);
            Assert.That(dataSource.RelatedRecordsNames.Count, Is.EqualTo(0));
            Assert.That(dataSource.ContextDocuments, Is.Not.Null);
            Assert.That(dataSource.ContextDocuments.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Test that the constructor with a description throws an ArgumentNullException if the archive information package id is null.
        /// </summary>
        [Test]
        public void TestThatConstructorWithDescriptionThrowsArgumentNullExceptionIfArchiveInformationPackageIdIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var description = fixture.CreateAnonymous<string>();
            var archivePeriodStart = fixture.CreateAnonymous<DateTime>();
            var archivePeriodEnd = fixture.CreateAnonymous<DateTime>().AddDays(7);
            Assert.Throws<ArgumentNullException>(() => new DataSource(null, nameSource, nameTarget, description, archivePeriodStart, archivePeriodEnd));
        }

        /// <summary>
        /// Test that the constructor with a description throws an ArgumentNullException if the archive information package id is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorWithDescriptionThrowsArgumentNullExceptionIfArchiveInformationPackageIdIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var description = fixture.CreateAnonymous<string>();
            var archivePeriodStart = fixture.CreateAnonymous<DateTime>();
            var archivePeriodEnd = fixture.CreateAnonymous<DateTime>().AddDays(7);
            Assert.Throws<ArgumentNullException>(() => new DataSource(string.Empty, nameSource, nameTarget, description, archivePeriodStart, archivePeriodEnd));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the system in the source repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameSourceIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));
            Assert.Throws<ArgumentNullException>(() => new DataSource(fixture.CreateAnonymous<string>(), null, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7)));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the system in the source repository is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameSourceIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));
            Assert.Throws<ArgumentNullException>(() => new DataSource(fixture.CreateAnonymous<string>(), string.Empty, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7)));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the system in the target repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameTargetIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));
            Assert.Throws<ArgumentNullException>(() => new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), null, fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7)));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the system in the target repository is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameTargetIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));
            Assert.Throws<ArgumentNullException>(() => new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), string.Empty, fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7)));
        }

        /// <summary>
        /// Test that the setter of ConnectionString throws ArgumentNullException if value is null.
        /// </summary>
        [Test]
        public void TestThatConnectionStringSetterThrowsArgumentNullExceptionIfValueIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.ConnectionString = null);
        }

        /// <summary>
        /// Test that the setter of ConnectionString throws ArgumentNullException if value is empty.
        /// </summary>
        [Test]
        public void TestThatConnectionStringSetterThrowsArgumentNullExceptionIfValueIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.ConnectionString = string.Empty);
        }

        /// <summary>
        /// Test that the setter of ConnectionString change the value.
        /// </summary>
        [Test]
        public void TestThatConnectionStringSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<string>();
            dataSource.ConnectionString = newValue;

            Assert.That(dataSource.ConnectionString, Is.Not.Null);
            Assert.That(dataSource.ConnectionString, Is.Not.Empty);
            Assert.That(dataSource.ConnectionString, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of ConnectionString raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatConnectionStringSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7))
                                 {
                                     ConnectionString = fixture.CreateAnonymous<string>()
                                 };
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("ConnectionString"));
                                                  eventCalled = true;
                                              };

            dataSource.ConnectionString = dataSource.ConnectionString;
            Assert.That(eventCalled, Is.False);

            dataSource.ConnectionString = fixture.CreateAnonymous<string>();
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of ArchiveInformationPackageId change the value.
        /// </summary>
        [Test]
        public void TestThatArchiveInformationPackageIdSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<string>();
            dataSource.ArchiveInformationPackageId = newValue;

            Assert.That(dataSource.ArchiveInformationPackageId, Is.Not.Null);
            Assert.That(dataSource.ArchiveInformationPackageId, Is.Not.Empty);
            Assert.That(dataSource.ArchiveInformationPackageId, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of ArchiveInformationPackageId throws an ArgumentNullException if the value is null.
        /// </summary>
        [Test]
        public void TestThatArchiveInformationPackageIdSetterThrowsArgumentNullExceptionIfValueIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.ArchiveInformationPackageId = null);
        }

        /// <summary>
        /// Test that the setter of ArchiveInformationPackageId throws an ArgumentNullException if the value is empty.
        /// </summary>
        [Test]
        public void TestThatArchiveInformationPackageIdSetterThrowsArgumentNullExceptionIfValueIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.ArchiveInformationPackageId = string.Empty);
        }

        /// <summary>
        /// Test that the setter of ArchiveInformationPackageId raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatArchiveInformationPackageIdSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("ArchiveInformationPackageId"));
                                                  eventCalled = true;
                                              };

            dataSource.ArchiveInformationPackageId = dataSource.ArchiveInformationPackageId;
            Assert.That(eventCalled, Is.False);

            dataSource.ArchiveInformationPackageId = fixture.CreateAnonymous<string>();
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of ArchiveInformationPackageIdPrevious change the value.
        /// </summary>
        [Test]
        public void TestThatArchiveInformationPackageIdPreviousSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<int>();
            dataSource.ArchiveInformationPackageIdPrevious = newValue;

            Assert.That(dataSource.ArchiveInformationPackageIdPrevious, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of ArchiveInformationPackageIdPrevious raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatArchiveInformationPackageIdPreviousSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("ArchiveInformationPackageIdPrevious"));
                                                  eventCalled = true;
                                              };

            dataSource.ArchiveInformationPackageIdPrevious = dataSource.ArchiveInformationPackageIdPrevious;
            Assert.That(eventCalled, Is.False);

            dataSource.ArchiveInformationPackageIdPrevious = fixture.CreateAnonymous<int>();
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of ArchivePeriodStart change the value.
        /// </summary>
        [Test]
        public void TestThatArchivePeriodStartSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<DateTime>().AddDays(1);
            dataSource.ArchivePeriodStart = newValue;

            Assert.That(dataSource.ArchivePeriodStart, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of ArchivePeriodStart raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatArchivePeriodStartSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("ArchivePeriodStart"));
                                                  eventCalled = true;
                                              };

            dataSource.ArchivePeriodStart = dataSource.ArchivePeriodStart;
            Assert.That(eventCalled, Is.False);

            dataSource.ArchivePeriodStart = fixture.CreateAnonymous<DateTime>().AddDays(1);
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of ArchivePeriodEnd change the value.
        /// </summary>
        [Test]
        public void TestThatArchivePeriodEndSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<DateTime>().AddDays(1);
            dataSource.ArchivePeriodEnd = newValue;

            Assert.That(dataSource.ArchivePeriodEnd, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of ArchivePeriodEnd raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatArchivePeriodEndSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("ArchivePeriodEnd"));
                                                  eventCalled = true;
                                              };

            dataSource.ArchivePeriodEnd = dataSource.ArchivePeriodEnd;
            Assert.That(eventCalled, Is.False);

            dataSource.ArchivePeriodEnd = fixture.CreateAnonymous<DateTime>().AddDays(1);
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of ArchiveInformationPacketType change the value.
        /// </summary>
        [Test]
        public void TestThatArchiveInformationPacketTypeSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = !dataSource.ArchiveInformationPacketType;
            dataSource.ArchiveInformationPacketType = newValue;

            Assert.That(dataSource.ArchiveInformationPacketType, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of ArchiveInformationPacketType raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatArchiveInformationPacketTypeSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("ArchiveInformationPacketType"));
                                                  eventCalled = true;
                                              };

            dataSource.ArchiveInformationPacketType = dataSource.ArchiveInformationPacketType;
            Assert.That(eventCalled, Is.False);

            dataSource.ArchiveInformationPacketType = !dataSource.ArchiveInformationPacketType;
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of ArchiveType change the value.
        /// </summary>
        [Test]
        public void TestThatArchiveTypeSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = !dataSource.ArchiveType;
            dataSource.ArchiveType = newValue;

            Assert.That(dataSource.ArchiveType, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of ArchiveType raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatArchiveTypeSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("ArchiveType"));
                                                  eventCalled = true;
                                              };

            dataSource.ArchiveType = dataSource.ArchiveType;
            Assert.That(eventCalled, Is.False);

            dataSource.ArchiveType = !dataSource.ArchiveType;
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of SystemPurpose throws ArgumentNullException if value is null.
        /// </summary>
        [Test]
        public void TestThatSystemPurposeSetterThrowsArgumentNullExceptionIfValueIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.SystemPurpose = null);
        }

        /// <summary>
        /// Test that the setter of SystemPurpose throws ArgumentNullException if value is empty.
        /// </summary>
        [Test]
        public void TestThatSystemPurposeSetterThrowsArgumentNullExceptionIfValueIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.SystemPurpose = string.Empty);
        }

        /// <summary>
        /// Test that the setter of SystemPurpose change the value.
        /// </summary>
        [Test]
        public void TestThatSystemPurposeSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<string>();
            dataSource.SystemPurpose = newValue;

            Assert.That(dataSource.SystemPurpose, Is.Not.Null);
            Assert.That(dataSource.SystemPurpose, Is.Not.Empty);
            Assert.That(dataSource.SystemPurpose, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of SystemPurpose raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatSystemPurposeSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7))
                                 {
                                     SystemPurpose = fixture.CreateAnonymous<string>()
                                 };
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("SystemPurpose"));
                                                  eventCalled = true;
                                              };

            dataSource.SystemPurpose = dataSource.SystemPurpose;
            Assert.That(eventCalled, Is.False);

            dataSource.SystemPurpose = fixture.CreateAnonymous<string>();
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of SystemContent throws ArgumentNullException if value is null.
        /// </summary>
        [Test]
        public void TestThatSystemContentSetterThrowsArgumentNullExceptionIfValueIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.SystemContent = null);
        }

        /// <summary>
        /// Test that the setter of SystemContent throws ArgumentNullException if value is empty.
        /// </summary>
        [Test]
        public void TestThatSystemContentSetterThrowsArgumentNullExceptionIfValueIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.SystemContent = string.Empty);
        }

        /// <summary>
        /// Test that the setter of SystemContent change the value.
        /// </summary>
        [Test]
        public void TestThatSystemContentSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<string>();
            dataSource.SystemContent = newValue;

            Assert.That(dataSource.SystemContent, Is.Not.Null);
            Assert.That(dataSource.SystemContent, Is.Not.Empty);
            Assert.That(dataSource.SystemContent, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of SystemContent raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatSystemContentSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7))
                                 {
                                     SystemContent = fixture.CreateAnonymous<string>()
                                 };
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("SystemContent"));
                                                  eventCalled = true;
                                              };

            dataSource.SystemContent = dataSource.SystemContent;
            Assert.That(eventCalled, Is.False);

            dataSource.SystemContent = fixture.CreateAnonymous<string>();
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of RegionNum change the value.
        /// </summary>
        [Test]
        public void TestThatRegionNumSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = !dataSource.RegionNum;
            dataSource.RegionNum = newValue;

            Assert.That(dataSource.RegionNum, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of RegionNum raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatRegionNumSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("RegionNum"));
                                                  eventCalled = true;
                                              };

            dataSource.RegionNum = dataSource.RegionNum;
            Assert.That(eventCalled, Is.False);

            dataSource.RegionNum = !dataSource.RegionNum;
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of KomNum change the value.
        /// </summary>
        [Test]
        public void TestThatKomNumSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = !dataSource.KomNum;
            dataSource.KomNum = newValue;

            Assert.That(dataSource.KomNum, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of KomNum raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatKomNumSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("KomNum"));
                                                  eventCalled = true;
                                              };

            dataSource.KomNum = dataSource.KomNum;
            Assert.That(eventCalled, Is.False);

            dataSource.KomNum = !dataSource.KomNum;
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of CprNum change the value.
        /// </summary>
        [Test]
        public void TestThatCprNumSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = !dataSource.CprNum;
            dataSource.CprNum = newValue;

            Assert.That(dataSource.CprNum, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of CprNum raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatCprNumSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("CprNum"));
                                                  eventCalled = true;
                                              };

            dataSource.CprNum = dataSource.CprNum;
            Assert.That(eventCalled, Is.False);

            dataSource.CprNum = !dataSource.CprNum;
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of CvrNum change the value.
        /// </summary>
        [Test]
        public void TestThatCvrNumSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = !dataSource.CvrNum;
            dataSource.CvrNum = newValue;

            Assert.That(dataSource.CvrNum, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of CvrNum raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatCvrNumSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("CvrNum"));
                                                  eventCalled = true;
                                              };

            dataSource.CvrNum = dataSource.CvrNum;
            Assert.That(eventCalled, Is.False);

            dataSource.CvrNum = !dataSource.CvrNum;
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of MatrikNum change the value.
        /// </summary>
        [Test]
        public void TestThatMatrikNumSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = !dataSource.MatrikNum;
            dataSource.MatrikNum = newValue;

            Assert.That(dataSource.MatrikNum, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of MatrikNum raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatMatrikNumSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("MatrikNum"));
                                                  eventCalled = true;
                                              };

            dataSource.MatrikNum = dataSource.MatrikNum;
            Assert.That(eventCalled, Is.False);

            dataSource.MatrikNum = !dataSource.MatrikNum;
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of BbrNum change the value.
        /// </summary>
        [Test]
        public void TestThatBbrNumSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = !dataSource.BbrNum;
            dataSource.BbrNum = newValue;

            Assert.That(dataSource.BbrNum, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of BbrNum raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatBbrNumSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("BbrNum"));
                                                  eventCalled = true;
                                              };

            dataSource.BbrNum = dataSource.BbrNum;
            Assert.That(eventCalled, Is.False);

            dataSource.BbrNum = !dataSource.BbrNum;
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of WhoSygKod change the value.
        /// </summary>
        [Test]
        public void TestThatWhoSygKodSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = !dataSource.WhoSygKod;
            dataSource.WhoSygKod = newValue;

            Assert.That(dataSource.WhoSygKod, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of WhoSygKod raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatWhoSygKodSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("WhoSygKod"));
                                                  eventCalled = true;
                                              };

            dataSource.WhoSygKod = dataSource.WhoSygKod;
            Assert.That(eventCalled, Is.False);

            dataSource.WhoSygKod = !dataSource.WhoSygKod;
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of FormVersion throws ArgumentNullException if value is null.
        /// </summary>
        [Test]
        public void TestThatFormVersionSetterThrowsArgumentNullExceptionIfValueIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.FormVersion = null);
        }

        /// <summary>
        /// Test that the setter of FormVersion throws ArgumentNullException if value is empty.
        /// </summary>
        [Test]
        public void TestThatFormVersionSetterThrowsArgumentNullExceptionIfValueIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.FormVersion = string.Empty);
        }

        /// <summary>
        /// Test that the setter of FormVersion change the value.
        /// </summary>
        [Test]
        public void TestThatFormVersionSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<string>();
            dataSource.FormVersion = newValue;

            Assert.That(dataSource.FormVersion, Is.Not.Null);
            Assert.That(dataSource.FormVersion, Is.Not.Empty);
            Assert.That(dataSource.FormVersion, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of FormVersion raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatFormVersionSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7))
                                 {
                                     FormVersion = fixture.CreateAnonymous<string>()
                                 };
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("FormVersion"));
                                                  eventCalled = true;
                                              };

            dataSource.FormVersion = dataSource.FormVersion;
            Assert.That(eventCalled, Is.False);

            dataSource.FormVersion = fixture.CreateAnonymous<string>();
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of ContainsDigitalDocuments change the value.
        /// </summary>
        [Test]
        public void TestThatContainsDigitalDocumentsSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = !dataSource.ContainsDigitalDocuments;
            dataSource.ContainsDigitalDocuments = newValue;

            Assert.That(dataSource.ContainsDigitalDocuments, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of ContainsDigitalDocuments raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatContainsDigitalDocumentsSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("ContainsDigitalDocuments"));
                                                  eventCalled = true;
                                              };

            dataSource.ContainsDigitalDocuments = dataSource.ContainsDigitalDocuments;
            Assert.That(eventCalled, Is.False);

            dataSource.ContainsDigitalDocuments = !dataSource.ContainsDigitalDocuments;
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of SearchRelatedOtherRecords change the value.
        /// </summary>
        [Test]
        public void TestThatSearchRelatedOtherRecordsSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = !dataSource.SearchRelatedOtherRecords;
            dataSource.SearchRelatedOtherRecords = newValue;

            Assert.That(dataSource.SearchRelatedOtherRecords, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of SearchRelatedOtherRecords raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatSearchRelatedOtherRecordsSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("SearchRelatedOtherRecords"));
                                                  eventCalled = true;
                                              };

            dataSource.SearchRelatedOtherRecords = dataSource.SearchRelatedOtherRecords;
            Assert.That(eventCalled, Is.False);

            dataSource.SearchRelatedOtherRecords = !dataSource.SearchRelatedOtherRecords;
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of SystemFileConcept change the value.
        /// </summary>
        [Test]
        public void TestThatSystemFileConceptSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = !dataSource.SystemFileConcept;
            dataSource.SystemFileConcept = newValue;

            Assert.That(dataSource.SystemFileConcept, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of SystemFileConcept raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatSystemFileConceptSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("SystemFileConcept"));
                                                  eventCalled = true;
                                              };

            dataSource.SystemFileConcept = dataSource.SystemFileConcept;
            Assert.That(eventCalled, Is.False);

            dataSource.SystemFileConcept = !dataSource.SystemFileConcept;
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of MultipleDataCollection change the value.
        /// </summary>
        [Test]
        public void TestThatMultipleDataCollectionSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = !dataSource.MultipleDataCollection;
            dataSource.MultipleDataCollection = newValue;

            Assert.That(dataSource.MultipleDataCollection, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of MultipleDataCollection raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatMultipleDataCollectionSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("MultipleDataCollection"));
                                                  eventCalled = true;
                                              };

            dataSource.MultipleDataCollection = dataSource.MultipleDataCollection;
            Assert.That(eventCalled, Is.False);

            dataSource.MultipleDataCollection = !dataSource.MultipleDataCollection;
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of PersonalDataRestrictedInfo change the value.
        /// </summary>
        [Test]
        public void TestThatPersonalDataRestrictedInfoSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = !dataSource.PersonalDataRestrictedInfo;
            dataSource.PersonalDataRestrictedInfo = newValue;

            Assert.That(dataSource.PersonalDataRestrictedInfo, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of PersonalDataRestrictedInfo raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatPersonalDataRestrictedInfoSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("PersonalDataRestrictedInfo"));
                                                  eventCalled = true;
                                              };

            dataSource.PersonalDataRestrictedInfo = dataSource.PersonalDataRestrictedInfo;
            Assert.That(eventCalled, Is.False);

            dataSource.PersonalDataRestrictedInfo = !dataSource.PersonalDataRestrictedInfo;
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of OtherAccessTypeRestrictions change the value.
        /// </summary>
        [Test]
        public void TestThatOtherAccessTypeRestrictionsSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = !dataSource.OtherAccessTypeRestrictions;
            dataSource.OtherAccessTypeRestrictions = newValue;

            Assert.That(dataSource.OtherAccessTypeRestrictions, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of OtherAccessTypeRestrictions raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatOtherAccessTypeRestrictionsSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("OtherAccessTypeRestrictions"));
                                                  eventCalled = true;
                                              };

            dataSource.OtherAccessTypeRestrictions = dataSource.OtherAccessTypeRestrictions;
            Assert.That(eventCalled, Is.False);

            dataSource.OtherAccessTypeRestrictions = !dataSource.OtherAccessTypeRestrictions;
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of ArchiveApproval throws ArgumentNullException if value is null.
        /// </summary>
        [Test]
        public void TestThatArchiveApprovalSetterThrowsArgumentNullExceptionIfValueIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.ArchiveApproval = null);
        }

        /// <summary>
        /// Test that the setter of ArchiveApproval throws ArgumentNullException if value is empty.
        /// </summary>
        [Test]
        public void TestThatArchiveApprovalSetterThrowsArgumentNullExceptionIfValueIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.ArchiveApproval = string.Empty);
        }

        /// <summary>
        /// Test that the setter of ArchiveApproval change the value.
        /// </summary>
        [Test]
        public void TestThatArchiveApprovalSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<string>();
            dataSource.ArchiveApproval = newValue;

            Assert.That(dataSource.ArchiveApproval, Is.Not.Null);
            Assert.That(dataSource.ArchiveApproval, Is.Not.Empty);
            Assert.That(dataSource.ArchiveApproval, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of ArchiveApproval raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatArchiveApprovalSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7))
                                 {
                                     ArchiveApproval = fixture.CreateAnonymous<string>()
                                 };
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("ArchiveApproval"));
                                                  eventCalled = true;
                                              };

            dataSource.ArchiveApproval = dataSource.ArchiveApproval;
            Assert.That(eventCalled, Is.False);

            dataSource.ArchiveApproval = fixture.CreateAnonymous<string>();
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of ArchiveRestrictions change the value.
        /// </summary>
        [Test]
        public void TestThatArchiveRestrictionsSetterChangeValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<string>();
            dataSource.ArchiveRestrictions = newValue;

            Assert.That(dataSource.ArchiveRestrictions, Is.Not.Null);
            Assert.That(dataSource.ArchiveRestrictions, Is.Not.Empty);
            Assert.That(dataSource.ArchiveRestrictions, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of ArchiveRestrictions raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatArchiveRestrictionsSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7))
                                 {
                                     ArchiveRestrictions = fixture.CreateAnonymous<string>()
                                 };
            Assert.That(dataSource, Is.Not.Null);

            var eventCalled = false;
            dataSource.PropertyChanged += (s, e) =>
                                              {
                                                  Assert.That(s, Is.Not.Null);
                                                  Assert.That(e, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Null);
                                                  Assert.That(e.PropertyName, Is.Not.Empty);
                                                  Assert.That(e.PropertyName, Is.EqualTo("ArchiveRestrictions"));
                                                  eventCalled = true;
                                              };

            dataSource.ArchiveRestrictions = dataSource.ArchiveRestrictions;
            Assert.That(eventCalled, Is.False);

            dataSource.ArchiveRestrictions = fixture.CreateAnonymous<string>();
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that AddTable throws an ArgumentNullException if the table is null.
        /// </summary>
        [Test]
        public void TestThatAddTableThrowsArgumentNullExceptionIfTableIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.AddTable(null));
        }

        /// <summary>
        /// Test that AddTable adds a information about a view (query).
        /// </summary>
        [Test]
        public void TestThatAddTableAddsTable()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.Tables, Is.Not.Null);
            Assert.That(dataSource.Tables.Count, Is.EqualTo(0));

            dataSource.AddTable(MockRepository.GenerateMock<ITable>());
            Assert.That(dataSource.Tables.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Test that AddView throws an ArgumentNullException if the view is null.
        /// </summary>
        [Test]
        public void TestThatAddViewThrowsArgumentNullExceptionIfViewIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.AddView(null));
        }

        /// <summary>
        /// Test that AddView adds a information about a view (query).
        /// </summary>
        [Test]
        public void TestThatAddViewAddsView()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.Views, Is.Not.Null);
            Assert.That(dataSource.Views.Count, Is.EqualTo(0));

            dataSource.AddView(MockRepository.GenerateMock<IView>());
            Assert.That(dataSource.Views.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Test that AddCreator throws an ArgumentNullException if the creator is null.
        /// </summary>
        [Test]
        public void TestThatAddCreatorThrowsArgumentNullExceptionIfCreatorIsNull()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.AddCreator(null));
        }

        /// <summary>
        /// Test that AddCreator adds a creator.
        /// </summary>
        [Test]
        public void TestThatAddCreatorAddsCreator()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.Creators, Is.Not.Null);
            Assert.That(dataSource.Creators.Count, Is.EqualTo(0));

            dataSource.AddCreator(MockRepository.GenerateMock<ICreator>());
            Assert.That(dataSource.Creators.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Test that AddAlternativeSystemName throws an ArgumentNullException if the alternative system name is null.
        /// </summary>
        [Test]
        public void TestThatAddAlternativeSystemNameThrowsArgumentNullExceptionIfAlternativeSystemNameIsNull()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.AddAlternativeSystemName(null));
        }

        /// <summary>
        /// Test that AddAlternativeSystemName throws an ArgumentNullException if the alternative system name is empty.
        /// </summary>
        [Test]
        public void TestThatAddAlternativeSystemNameThrowsArgumentNullExceptionIfAlternativeSystemNameIsEmpty()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.AddAlternativeSystemName(string.Empty));
        }

        /// <summary>
        /// Test that AddAlternativeSystemName adds an alternative system name.
        /// </summary>
        [Test]
        public void TestThatAddAlternativeSystemNameAddsAlternativeSystemName()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.AlternativeSystemNames, Is.Not.Null);
            Assert.That(dataSource.AlternativeSystemNames.Count, Is.EqualTo(0));

            dataSource.AddAlternativeSystemName(fixture.CreateAnonymous<string>());
            Assert.That(dataSource.AlternativeSystemNames.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Test that AddSourceName throws an ArgumentNullException if name of the other IT system is null.
        /// </summary>
        [Test]
        public void TestThatAddSourceNameThrowsArgumentNullExceptionIfSourceNameIsNull()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.AddSourceName(null));
        }

        /// <summary>
        /// Test that AddSourceName throws an ArgumentNullException if name of the other IT system is empty.
        /// </summary>
        [Test]
        public void TestThatAddSourceNameThrowsArgumentNullExceptionIfSourceNameIsEmpty()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.AddSourceName(string.Empty));
        }

        /// <summary>
        /// Test that AddSourceName adds name of the another IT system.
        /// </summary>
        [Test]
        public void TestThatAddSourceNameAddsSourceName()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.SourceNames, Is.Not.Null);
            Assert.That(dataSource.SourceNames.Count, Is.EqualTo(0));

            dataSource.AddSourceName(fixture.CreateAnonymous<string>());
            Assert.That(dataSource.SourceNames.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Test that AddUserName throws an ArgumentNullException if name of the user is null.
        /// </summary>
        [Test]
        public void TestThatAddUserNameThrowsArgumentNullExceptionIfUserNameIsNull()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.AddUserName(null));
        }

        /// <summary>
        /// Test that AddUserName throws an ArgumentNullException if the name of the user is empty.
        /// </summary>
        [Test]
        public void TestThatAddUserNameThrowsArgumentNullExceptionIfUserNameIsEmpty()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.AddUserName(string.Empty));
        }

        /// <summary>
        /// Test that AddUserName adds name of an user.
        /// </summary>
        [Test]
        public void TestThatAddUserNameAddsUserName()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.UserNames, Is.Not.Null);
            Assert.That(dataSource.UserNames.Count, Is.EqualTo(0));

            dataSource.AddUserName(fixture.CreateAnonymous<string>());
            Assert.That(dataSource.UserNames.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Test that AddPredecessorName throws an ArgumentNullException if name of the earlier system is null.
        /// </summary>
        [Test]
        public void TestThatAddPredecessorNameThrowsArgumentNullExceptionIfPredecessorNameIsNull()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.AddPredecessorName(null));
        }

        /// <summary>
        /// Test that AddPredecessorName throws an ArgumentNullException if the name of the earlier system is empty.
        /// </summary>
        [Test]
        public void TestThatAddPredecessorNameThrowsArgumentNullExceptionIfPredecessorNameIsEmpty()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.AddPredecessorName(string.Empty));
        }

        /// <summary>
        /// Test that AddPredecessorName adds name of the earlier system having the same function.
        /// </summary>
        [Test]
        public void TestThatAddPredecessorNameAddsPredecessorName()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.PredecessorNames, Is.Not.Null);
            Assert.That(dataSource.PredecessorNames.Count, Is.EqualTo(0));

            dataSource.AddPredecessorName(fixture.CreateAnonymous<string>());
            Assert.That(dataSource.PredecessorNames.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Test that AddFormClass throws an ArgumentNullException if the FORM classification is null.
        /// </summary>
        [Test]
        public void TestThatAddFormClassThrowsArgumentNullExceptionIfFormClassIsNull()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.AddFormClass(null));
        }

        /// <summary>
        /// Test that AddFormClass adds information about a FORM classification.
        /// </summary>
        [Test]
        public void TestThatAddFormClassAddsFormClass()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.FormClasses, Is.Not.Null);
            Assert.That(dataSource.FormClasses.Count, Is.EqualTo(0));

            dataSource.AddFormClass(MockRepository.GenerateMock<IFormClass>());
            Assert.That(dataSource.FormClasses.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Test that AddRelatedRecordsName throws an ArgumentNullException if name of the reference is null.
        /// </summary>
        [Test]
        public void TestThatAddRelatedRecordsNameThrowsArgumentNullExceptionIfRelatedRecordsNameIsNull()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.AddRelatedRecordsName(null));
        }

        /// <summary>
        /// Test that AddRelatedRecordsName throws an ArgumentNullException if name of the reference is empty.
        /// </summary>
        [Test]
        public void TestThatAddRelatedRecordsNameThrowsArgumentNullExceptionIfRelatedRecordsNameIsEmpty()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.AddRelatedRecordsName(string.Empty));
        }

        /// <summary>
        /// Test that AddRelatedRecordsName adds a reference to the records this delivery is seeking means to.
        /// </summary>
        [Test]
        public void TestThatAddRelatedRecordsNameAddsRelatedRecordsName()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.RelatedRecordsNames, Is.Not.Null);
            Assert.That(dataSource.RelatedRecordsNames.Count, Is.EqualTo(0));

            dataSource.AddRelatedRecordsName(fixture.CreateAnonymous<string>());
            Assert.That(dataSource.RelatedRecordsNames.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Test that AddContextDocument throws an ArgumentNullException if the context document is null.
        /// </summary>
        [Test]
        public void TestThatAddContextDocumentThrowsArgumentNullExceptionIfContextDocumentIsNull()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSource.AddContextDocument(null));
        }

        /// <summary>
        /// Test that AddContextDocument adds a context document to the context documentation.
        /// </summary>
        [Test]
        public void TestThatAddContextDocumentAddsContextDocument()
        {
            var fixture = new Fixture();

            var dataSource = new DataSource(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.ContextDocuments, Is.Not.Null);
            Assert.That(dataSource.ContextDocuments.Count, Is.EqualTo(0));

            dataSource.AddContextDocument(MockRepository.GenerateMock<IContextDocument>());
            Assert.That(dataSource.ContextDocuments.Count, Is.EqualTo(1));
        }
    }
}
