using DsiNext.DeliveryEngine.BusinessLogic.Commands;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Commands;
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
        #region Private variables

        private Fixture _fixture;

        #endregion

        /// <summary>
        /// Setup each unit test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        /// <summary>
        /// Test that the constructor initialize the command executing the delivery engine.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeCommand()
        {
            IDeliveryEngineExecuteCommand command = new DeliveryEngineExecuteCommand();
            Assert.That(command, Is.Not.Null);
            Assert.That(command.OverrideArchiveInformationPackageId, Is.Null);
            Assert.That(command.ValidationOnly, Is.False);
            Assert.That(command.Table, Is.Null);
            Assert.That(command.TablesHandledSimultaneity, Is.EqualTo(5));
            Assert.That(command.RemoveMissingRelationshipsOnForeignKeys, Is.False);
            Assert.That(command.NumberOfForeignTablesToCache, Is.EqualTo(10));
            Assert.That(command.IncludeEmptyTables, Is.False);
        }

        /// <summary>
        /// Test that the setter of OverrideArchiveInformationPackageId change the value.
        /// </summary>
        [Test]
        public void TestThatOverrideArchiveInformationPackageIdSetterChangeValue()
        {
            IDeliveryEngineExecuteCommand command = new DeliveryEngineExecuteCommand();
            Assert.That(command, Is.Not.Null);
            Assert.That(command.OverrideArchiveInformationPackageId, Is.Null);

            string newValue = _fixture.CreateAnonymous<string>();
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
            IDeliveryEngineExecuteCommand command = new DeliveryEngineExecuteCommand();
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
            IDeliveryEngineExecuteCommand command = new DeliveryEngineExecuteCommand();
            Assert.That(command, Is.Not.Null);
            Assert.That(command.Table, Is.Null);

            string newValue = _fixture.CreateAnonymous<string>();
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
            IDeliveryEngineExecuteCommand command = new DeliveryEngineExecuteCommand();
            Assert.That(command, Is.Not.Null);
            Assert.That(command.TablesHandledSimultaneity, Is.EqualTo(5));

            var newValue = command.TablesHandledSimultaneity + _fixture.CreateAnonymous<int>();
            command.TablesHandledSimultaneity = newValue;
            Assert.That(command.TablesHandledSimultaneity, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of RemoveMissingRelationshipsOnForeignKeys change the value.
        /// </summary>
        [Test]
        public void TestThatRemoveMissingRelationshipsOnForeignKeysSetterChangeValue()
        {
            IDeliveryEngineExecuteCommand command = new DeliveryEngineExecuteCommand();
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
            IDeliveryEngineExecuteCommand command = new DeliveryEngineExecuteCommand();
            Assert.That(command, Is.Not.Null);
            Assert.That(command.NumberOfForeignTablesToCache, Is.EqualTo(10));

            var newValue = command.NumberOfForeignTablesToCache + _fixture.CreateAnonymous<int>();
            command.NumberOfForeignTablesToCache = newValue;
            Assert.That(command.NumberOfForeignTablesToCache, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of IncludeEmptyTables change the value.
        /// </summary>
        [Test]
        public void TestThatIncludeEmptyTablesSetterChangeValue()
        {
            IDeliveryEngineExecuteCommand command = new DeliveryEngineExecuteCommand();
            Assert.That(command, Is.Not.Null);
            Assert.That(command.IncludeEmptyTables, Is.False);

            command.IncludeEmptyTables = true;
            Assert.That(command.IncludeEmptyTables, Is.True);
        }
    }
}
