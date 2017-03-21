using System;
using System.Collections.ObjectModel;
using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    public class Field : NamedObject, IField
    {
        private int _lengthOfSource;
        private int _lengthOfTarget;
        private Type _datatypeOfSource;
        private Type _datatypeOfTarget;
        private readonly ITable _table;
        private IMap _map;
        private bool _nullable;
        private string _columnId;
        private string _defaultValue;
        private string _originalDatatype;

        private readonly ObservableCollection<IFunctionality> _functionality = new ObservableCollection<IFunctionality>();

        public Field(string nameSource, string nameTarget, int lengthOfSource, int lengthOfTarget, Type datatypeOfSource, Type datatypeOfTarget, ITable table) : base(nameSource, nameTarget)
        {
            if (datatypeOfSource == null) throw new ArgumentNullException("datatypeOfSource");
            if (datatypeOfTarget == null) throw new ArgumentNullException("datatypeOfTarget");
            if (table == null) throw new ArgumentNullException("table");

            _lengthOfSource = lengthOfSource;
            _lengthOfTarget = lengthOfTarget;
            _datatypeOfSource = datatypeOfSource;
            _datatypeOfTarget = datatypeOfTarget;
            _table = table;
        }

        public Field(string nameSource, string nameTarget, string description, int lengthOfSource, int lengthOfTarget, Type datatypeOfSource, Type datatypeOfTarget, ITable table)
            : base(nameSource, nameTarget, description)
        {
            if (datatypeOfSource == null) throw new ArgumentNullException("datatypeOfSource");
            if (datatypeOfTarget == null) throw new ArgumentNullException("datatypeOfTarget");
            if (table == null) throw new ArgumentNullException("table");

            _lengthOfSource = lengthOfSource;
            _lengthOfTarget = lengthOfTarget;
            _datatypeOfSource = datatypeOfSource;
            _datatypeOfTarget = datatypeOfTarget;
            _table = table;
        }

        public void CreateStaticMap()
        {}

        public virtual int LengthOfSource
        {
            get { return _lengthOfSource; }
            set
            {
                if (value <= 0)
                    throw new DeliveryEngineMetadataException(Resource.GetExceptionMessage(ExceptionMessage.InvalidFieldLength, value), this);
                if (value == _lengthOfSource) return;

                _lengthOfSource = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public virtual int LengthOfTarget
        {
            get { return _lengthOfTarget; }
            set
            {
                if (value <= 0) 
                    throw new DeliveryEngineMetadataException(Resource.GetExceptionMessage(ExceptionMessage.InvalidFieldLength, value), this);

                if (value == _lengthOfTarget) return;

                _lengthOfTarget = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public virtual Type DatatypeOfSource
        {
            get { return _datatypeOfSource; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (value == _datatypeOfSource) return;

                _datatypeOfSource = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public virtual Type DatatypeOfTarget
        {
            get { return _datatypeOfTarget; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (value == _datatypeOfSource) return;

                _datatypeOfTarget = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public virtual ITable Table
        {
            get { return _table; }
        }

        public IMap Map
        {
            get { return _map; }
            set
            {
                if (value == null) throw new ArgumentException();
                if (value == _map) return;

                //TODO tjek at typer er rigtige
                //if (value.GetType().ReflectedType.)

                _map = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public bool Nullable
        {
            get { return _nullable; }
            set 
            {
                if (value == _nullable) return;

                _nullable = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public string ColumnId
        {
            get { return _columnId; }
            set
            {
                if (value == _columnId) return;

                _columnId = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public string DefaultValue
        {
            get { return _defaultValue; }
            set
            {
                if (value == _defaultValue) return;

                _defaultValue = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public string OriginalDatatype
        {
            get { return _originalDatatype; }
            set
            {
                if (value == _originalDatatype) return;

                _originalDatatype = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public ReadOnlyObservableCollection<IFunctionality> Functionality
        {
            get { return new ReadOnlyObservableCollection<IFunctionality>(_functionality); }
        }

        public void AddFunctionality(IFunctionality functionality)
        {
            if (functionality == null) throw new ArgumentNullException("functionality");

            _functionality.Add(functionality);
        }

        /// <summary>
        /// Creates a data object for the field.
        /// </summary>
        /// <param name="value">Value to the data object.</param>
        /// <returns>Data object for the field.</returns>
        public IDataObjectBase CreateDataObject(object value)
        {
            var fieldDataType = typeof (FieldData<,>);
            if (DatatypeOfSource == typeof (string))
            {
                return (IDataObjectBase) Activator.CreateInstance(fieldDataType.MakeGenericType(new[] {DatatypeOfSource, DatatypeOfTarget}), new[] {this, value});
            }
            if (DatatypeOfSource == typeof (int?))
            {
                return (IDataObjectBase) Activator.CreateInstance(fieldDataType.MakeGenericType(new[] {DatatypeOfSource, DatatypeOfTarget}), new[] {this, value});
            }
            if (DatatypeOfSource == typeof (long?))
            {
                return (IDataObjectBase) Activator.CreateInstance(fieldDataType.MakeGenericType(new[] {DatatypeOfSource, DatatypeOfTarget}), new[] {this, value});
            }
            if (DatatypeOfSource == typeof (decimal?))
            {
                return (IDataObjectBase) Activator.CreateInstance(fieldDataType.MakeGenericType(new[] {DatatypeOfSource, DatatypeOfTarget}), new[] {this, value});
            }
            if (DatatypeOfSource == typeof (DateTime?))
            {
                return (IDataObjectBase) Activator.CreateInstance(fieldDataType.MakeGenericType(new[] {DatatypeOfSource, DatatypeOfTarget}), new[] {this, value});
            }
            if (DatatypeOfSource == typeof (TimeSpan?))
            {
                return (IDataObjectBase) Activator.CreateInstance(fieldDataType.MakeGenericType(new[] {DatatypeOfSource, DatatypeOfTarget}), new[] {this, value});
            }
            throw new DeliveryEngineMetadataException(Resource.GetExceptionMessage(ExceptionMessage.DataTypeNotSupported, DatatypeOfSource), this);
        }
    }
}
