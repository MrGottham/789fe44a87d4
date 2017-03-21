using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    public class Table : NamedObject, ITable
    {
        private bool _master;
        private readonly ObservableCollection<IFilter> _recordFilters = new ObservableCollection<IFilter>();
        private readonly ObservableCollection<IFilter> _fieldFilters = new ObservableCollection<IFilter>();
        private readonly ObservableCollection<IField> _fields = new ObservableCollection<IField>();
        private readonly ObservableCollection<ITableDocument> _documents = new ObservableCollection<ITableDocument>();
        private readonly ObservableCollection<IForeignKey> _foreignKeys = new ObservableCollection<IForeignKey>();
        private readonly ObservableCollection<ICandidateKey> _candidateKeys = new ObservableCollection<ICandidateKey>();

        public Table(string nameSource, string nameTarget) : base(nameSource, nameTarget)
        {}

        public Table(string nameSource, string nameTarget, string description) : base(nameSource, nameTarget, description)
        {
        }

        public virtual bool Master
        {
            get { return _master; }
            set
            {
                if (_master == value) return;

                _master = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public ICandidateKey PrimaryKey
        { 
            get
            {
                return CandidateKeys.Count > 0
                           ? CandidateKeys[0]
                           : null;
            }
        }

        public ReadOnlyObservableCollection<IFilter> RecordFilters
        {
            get { return new ReadOnlyObservableCollection<IFilter>(_recordFilters); }
        }

        public ReadOnlyObservableCollection<IFilter> FieldFilters
        {
            get { return new ReadOnlyObservableCollection<IFilter>(_fieldFilters); }
        }

        public ReadOnlyObservableCollection<IField> Fields
        {
            get { return new ReadOnlyObservableCollection<IField>(_fields); }
        }

        public ReadOnlyObservableCollection<ITableDocument> Documents
        {
            get { return new ReadOnlyObservableCollection<ITableDocument>(_documents); }
        }

        public ReadOnlyObservableCollection<IForeignKey> ForeignKeys
        {
            get { return new ReadOnlyObservableCollection<IForeignKey>(_foreignKeys); }
        }

        public ReadOnlyObservableCollection<ICandidateKey> CandidateKeys
        {
            get { return new ReadOnlyObservableCollection<ICandidateKey>(_candidateKeys); }
        }

        public virtual void AddRecordFilter(IFilter filter)
        {
            if (filter == null) throw new ArgumentNullException("filter");

            _recordFilters.Add(filter);
        }

        public virtual void AddFieldFilter(IFilter filter)
        {
            if (filter == null) throw new ArgumentNullException("filter");

            _fieldFilters.Add(filter);
        }

        public void AddFilter(IField field)
        {
            throw new NotImplementedException();
        }

        public void AddDocument(ITableDocument document)
        {
            if (document == null) throw new ArgumentNullException("document");

            _documents.Add(document);
        }

        public void AddField(IField field)
        {
            if (field == null) throw new ArgumentNullException("field");

            _fields.Add(field);
        }

        public void AddCandidateKey(ICandidateKey key)
        {
            if (key == null) throw new ArgumentNullException("key");

            _candidateKeys.Add(key);
        }

        public void AddForeignKey(IForeignKey key)
        {
            if (key == null) throw new ArgumentNullException("key");

            _foreignKeys.Add(key);
        }

        /// <summary>
        /// Creates an empty row for the table.
        /// </summary>
        /// <returns>Empty row for the table.</returns>
        public virtual IEnumerable<IDataObjectBase> CreateRow()
        {
            return Fields.Select(m => m.CreateDataObject(null)).ToList();
        }

        /// <summary>
        /// Clone the table.
        /// </summary>
        /// <returns>Cloned table.</returns>
        public virtual object Clone()
        {
            var clonedTable = new Table(NameSource, NameTarget, Description)
                {
                    Master = Master
                };
            foreach (var field in Fields)
            {
                var clonedField = new Field(field.NameSource, field.NameTarget, field.Description, field.LengthOfSource, field.LengthOfTarget, field.DatatypeOfSource, field.DatatypeOfTarget, clonedTable)
                    {
                        Nullable = field.Nullable,
                        ColumnId = field.ColumnId,
                        DefaultValue = field.DefaultValue,
                        OriginalDatatype = field.OriginalDatatype
                    };
                /*
                if (field.Map != null)
                {
                    clonedField.Map = field.Map;
                }
                foreach (var functionality in field.Functionality)
                {
                    clonedField.AddFunctionality(functionality);
                }
                */
                clonedTable.AddField(clonedField);
            }
            foreach (var candidateKey in CandidateKeys)
            {
                var clonedCandidateKey = new CandidateKey(candidateKey.NameSource, candidateKey.NameTarget, candidateKey.Description)
                    {
                        Table = clonedTable
                    };
                foreach (var clonedkeyField in candidateKey.Fields.Select(m => new KeyValuePair<IField, IMap>(GetClonedField(clonedTable, m.Key), null)))
                {
                    if (clonedkeyField.Value == null)
                    {
                        clonedCandidateKey.AddField(clonedkeyField.Key);
                        continue;
                    }
                    clonedCandidateKey.AddField(clonedkeyField.Key, clonedkeyField.Value);
                }
                clonedTable.AddCandidateKey(candidateKey);
            }
            foreach (var foreignKey in ForeignKeys)
            {
                var clonedForeignKey = new ForeignKey(foreignKey.CandidateKey, foreignKey.NameSource, foreignKey.NameTarget, foreignKey.Description, foreignKey.Cardinality)
                    {
                        Table = clonedTable
                    };
                foreach (var clonedkeyField in foreignKey.Fields.Select(m => new KeyValuePair<IField, IMap>(GetClonedField(clonedTable, m.Key), null)))
                {
                    if (clonedkeyField.Value == null)
                    {
                        clonedForeignKey.AddField(clonedkeyField.Key);
                        continue;
                    }
                    clonedForeignKey.AddField(clonedkeyField.Key, clonedkeyField.Value);
                }
                clonedTable.AddForeignKey(clonedForeignKey);
            }
            foreach (var recordFilter in RecordFilters)
            {
                var clonedRecordFilter = new Filter();
                foreach (var criteria in recordFilter.Criterias)
                {
                    var equalCriteria = criteria as IEqualCriteria;
                    if (equalCriteria != null)
                    {
                        var equalToField = equalCriteria.GetType().GetField("_equalTo", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (equalToField == null)
                        {
                            throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.FieldNotFoundOnType, "_equalTo", equalCriteria.GetType().Name));
                        }
                        var clonedEqualCriteria = Activator.CreateInstance(equalCriteria.GetType(), new[] {GetClonedField(clonedTable, equalCriteria.Field), equalToField.GetValue(equalCriteria)}) as IEqualCriteria;
                        clonedRecordFilter.AddCriteria(clonedEqualCriteria);
                        continue;
                    }
                    var poolCriteria = criteria as IPoolCriteria;
                    if (poolCriteria != null)
                    {
                        var poolValuesField = poolCriteria.GetType().GetField("_poolValues", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (poolValuesField == null)
                        {
                            throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.FieldNotFoundOnType, "_poolValues", poolCriteria.GetType().Name));
                        }
                        var clonedPoolCriteria = Activator.CreateInstance(poolCriteria.GetType(), new[] {GetClonedField(clonedTable, poolCriteria.Field), poolValuesField.GetValue(poolCriteria)}) as IPoolCriteria;
                        clonedRecordFilter.AddCriteria(clonedPoolCriteria);
                        continue;
                    }
                    var intervalCriteria = criteria as IIntervalCriteria;
                    if (intervalCriteria != null)
                    {
                        var fromValueField = intervalCriteria.GetType().GetField("_fromValue", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (fromValueField == null)
                        {
                            throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.FieldNotFoundOnType, "_fromValue", intervalCriteria.GetType().Name));
                        }
                        var toValueField = intervalCriteria.GetType().GetField("_toValue", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (toValueField == null)
                        {
                            throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.FieldNotFoundOnType, "_toValue", intervalCriteria.GetType().Name));
                        }
                        var clonedCriteria = Activator.CreateInstance(intervalCriteria.GetType(), new[] {GetClonedField(clonedTable, intervalCriteria.Field), fromValueField.GetValue(intervalCriteria), toValueField.GetValue(intervalCriteria)}) as IIntervalCriteria;
                        clonedRecordFilter.AddCriteria(clonedCriteria);
                        continue;
                    }
                    var notNullCriteria = criteria as INotNullCriteria;
                    if (notNullCriteria != null)
                    {
                        var clonedNotCriteria = Activator.CreateInstance(notNullCriteria.GetType(), new object[] {GetClonedField(clonedTable, notNullCriteria.Field)}) as INotNullCriteria;
                        clonedRecordFilter.AddCriteria(clonedNotCriteria);
                        continue;
                    }
                    throw new NotSupportedException(criteria.GetType().Name);
                }
                clonedTable.AddRecordFilter(clonedRecordFilter);
            }
            foreach (var fieldFilter in FieldFilters)
            {
                var clonedFieldFilter = new Filter();
                foreach (var criteria in fieldFilter.Criterias)
                {
                    var excludeFieldCriteria = criteria as IExcludeFieldCriteria;
                    if (excludeFieldCriteria != null)
                    {
                        var clonedExcludeFieldCriteria = new ExcludeFieldCriteria(excludeFieldCriteria.Field);
                        clonedFieldFilter.AddCriteria(clonedExcludeFieldCriteria);
                        continue;
                    }
                    throw new NotSupportedException(criteria.GetType().Name);
                }
                clonedTable.AddFieldFilter(clonedFieldFilter);
            }
            foreach (var document in Documents)
            {
                throw new NotSupportedException(document.GetType().Name);
            }
            return clonedTable;
        }

        private static IField GetClonedField(ITable clonedTable, IField field)
        {
            return clonedTable.Fields.Single(m => Equals(m.NameSource, field.NameSource) && Equals(m.NameTarget, field.NameTarget));
        }
    }
}
