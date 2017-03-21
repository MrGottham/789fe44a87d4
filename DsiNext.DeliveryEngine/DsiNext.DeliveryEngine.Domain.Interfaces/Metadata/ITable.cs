using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;

namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    public interface ITable : INamedObject, ICloneable
    {
        bool Master { get; set; }

        ICandidateKey PrimaryKey { get; }

        ReadOnlyObservableCollection<IFilter> RecordFilters { get; }

        ReadOnlyObservableCollection<IFilter> FieldFilters { get; }

        ReadOnlyObservableCollection<IField> Fields { get; }

        ReadOnlyObservableCollection<ITableDocument> Documents { get; }

        ReadOnlyObservableCollection<IForeignKey> ForeignKeys { get; }

        ReadOnlyObservableCollection<ICandidateKey> CandidateKeys { get; }

        void AddRecordFilter(IFilter filter);

        void AddFieldFilter(IFilter filter);

        void AddFilter(IField field);

        void AddDocument(ITableDocument document);

        void AddField(IField field);

        void AddCandidateKey(ICandidateKey key);

        void AddForeignKey(IForeignKey key);

        /// <summary>
        /// Creates an empty row for the table.
        /// </summary>
        /// <returns>Empty row for the table.</returns>
        IEnumerable<IDataObjectBase> CreateRow();
    }
}
