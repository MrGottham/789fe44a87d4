using DsiNext.DeliveryEngine.BusinessLogic.Commands;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.BusinessLogic.Commands
{
    /// <summary>
    /// Tests the command executing the delivering engine.
    /// </summary>
    [TestFixture]
    public class DeliveryEngineExecuteCommandTests
    {
        /// <summary>
        /// Test that the constructor initialize the command executing the delivery engine.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeCommand()
        {
            var command = new DeliveryEngineExecuteCommand();
            Assert.That(command, Is.Not.Null);
            Assert.That(command.OverrideArchiveInformationPackageId, Is.Null);
            Assert.That(command.ValidationOnly, Is.False);
            Assert.That(command.Table, Is.Null);
            Assert.That(command.TablesHandledSimultaneity, Is.EqualTo(5));
            Assert.That(command.RemoveMissingRelationshipsOnForeignKeys, Is.False);
            Assert.That(command.NumberOfForeignTablesToCache, Is.EqualTo(10));
        }

        /// <summary>
        /// Test that the setter of OverrideArchiveInformationPackageId change the value.
        /// </summary>
        [Test]
        public void TestThatOverrideArchiveInformationPackageIdSetterChangeValue()
        {
            var fixture = new Fixture();

            var command = new DeliveryEngineExecuteCommand();
            Assert.That(command, Is.Not.Null);
            Assert.That(command.OverrideArchiveInformationPackageId, Is.Null);

            var newValue = fixture.CreateAnonymous<string>();
            command.OverrideArchiveInformationPackageId = newValue;
            Assert.That(command.OverrideArchiveInformationPackageId, Is.Not.Null);
            Assert.That(command.OverrideArchiveInformationPackageId, Is.Not.Empty);
            Assert.That(command.OverrideArchiveInformationPackageId, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of ValidationOnly change the value.
        /// </summary>
        [Test]
        public void TestThatValidationOnlySetterChangeValue()
        {
            var command = new DeliveryEngineExecuteCommand();
            Assert.That(command, Is.Not.Null);
            Assert.That(command.ValidationOnly, Is.False);

            command.ValidationOnly = true;
            Assert.That(command.ValidationOnly, Is.True);
        }

        /// <summary>
        /// Test that the setter of Table change the value.
        /// </summary>
        [Test]
        public void TestThatTableSetterChangeValue()
        {
            var fixture = new Fixture();

            var command = new DeliveryEngineExecuteCommand();
            Assert.That(command, Is.Not.Null);
            Assert.That(command.Table, Is.Null);

            var newValue = fixture.CreateAnonymous<string>();
            command.Table = newValue;
            Assert.That(command.Table, Is.Not.Null);
            Assert.That(command.Table, Is.Not.Empty);
            Assert.That(command.Table, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of TablesHandledSimultaneity change the value.
        /// </summary>
        [Test]
        public void TestThatTablesHandledSimultaneitySetterChangeValue()
        {
            var fixture = new Fixture();

            var command = new DeliveryEngineExecuteCommand();
            Assert.That(command, Is.Not.Null);
            Assert.That(command.TablesHandledSimultaneity, Is.EqualTo(5));

            var newValue = command.TablesHandledSimultaneity + fixture.CreateAnonymous<int>();
            command.TablesHandledSimultaneity = newValue;
            Assert.That(command.TablesHandledSimultaneity, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of RemoveMissingRelationshipsOnForeignKeys change the value.
        /// </summary>
        [Test]
        public void TestThatRemoveMissingRelationshipsOnForeignKeysSetterChangeValue()
        {
            var command = new DeliveryEngineExecuteCommand();
            Assert.That(command, Is.Not.Null);
            Assert.That(command.RemoveMissingRelationshipsOnForeignKeys, Is.False);

            command.RemoveMissingRelationshipsOnForeignKeys = true;
            Assert.That(command.RemoveMissingRelationshipsOnForeignKeys, Is.True);
        }

        /// <summary>
        /// Test that the setter of NumberOfForeignTablesToCache change the value.
        /// </summary>
        [Test]
        public void TestThatNumberOfForeignTablesToCacheSetterChangeValue()
        {
            var fixture = new Fixture();

            var command = new DeliveryEngineExecuteCommand();
            Assert.That(command, Is.Not.Null);
            Assert.That(command.NumberOfForeignTablesToCache, Is.EqualTo(10));

            var newValue = command.NumberOfForeignTablesToCache + fixture.CreateAnonymous<int>();
            command.NumberOfForeignTablesToCache = newValue;
            Assert.That(command.NumberOfForeignTablesToCache, Is.EqualTo(newValue));
        }
    }
}
