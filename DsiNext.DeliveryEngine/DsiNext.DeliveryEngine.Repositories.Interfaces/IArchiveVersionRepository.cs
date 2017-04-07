using System.Collections.Generic;
using System.IO;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Repositories.Interfaces
{
    /// <summary>
    /// Interface for a repository to write the delivery format.
    /// </summary>
    public interface IArchiveVersionRepository : IRepository
    {
        DirectoryInfo DestinationFolder { get; }

        IDataSource DataSource { get; set; }

        void ArchiveMetaData();

        void ArchiveTableData(IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>> tableData, object syncRoot);
    }
}
