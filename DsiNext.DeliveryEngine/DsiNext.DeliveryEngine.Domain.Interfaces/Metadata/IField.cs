using System;
using System.Collections.ObjectModel;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;

namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    public interface IField : INamedObject
    {
        int LengthOfSource { get; set; }

        int LengthOfTarget { get; set; }

        Type DatatypeOfSource { get; set; }

        Type DatatypeOfTarget { get; set; }

        ITable Table { get; }

        IMap Map { get; set; }

        bool Nullable { get; set; }

        ReadOnlyObservableCollection<IFunctionality> Functionality { get; }

        string ColumnId { get; set; }

        string DefaultValue { get; set; }

        string OriginalDatatype { get; set; }

        void AddFunctionality(IFunctionality functionality);

        /// <summary>
        /// Creates a data object for the field.
        /// </summary>
        /// <param name="value">Value to the data object.</param>
        /// <returns>Data object for the field.</returns>
        IDataObjectBase CreateDataObject(object value);
    }
}
