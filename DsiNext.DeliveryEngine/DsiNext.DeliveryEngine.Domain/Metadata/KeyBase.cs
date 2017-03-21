using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    public abstract class KeyBase : NamedObject, IKey
    {
        protected KeyBase(string nameSource, string nameTarget) : base(nameSource, nameTarget)
        {
            Map = new ObservableCollection<KeyValuePair<IField, IMap>>();
        }

        protected KeyBase(string nameSource, string nameTarget, string description) : base(nameSource, nameTarget, description)
        {
            Map = new ObservableCollection<KeyValuePair<IField, IMap>>();
        }

        protected ObservableCollection<KeyValuePair<IField, IMap>> Map { get; set; }

        public virtual ITable Table { get; set; }

        public ReadOnlyObservableCollection<KeyValuePair<IField, IMap>> Fields
        {
            get { return new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(Map); }
        }

        public void AddField(IField field)
        {
            if (field == null) throw new ArgumentNullException("field");

            if (field.Table != Table)
                throw new DeliveryEngineMetadataException(
                    Resource.GetExceptionMessage(ExceptionMessage.FieldTableMismatch, field.Table.NameTarget, Table.NameTarget), this);

            Map.Add(new KeyValuePair<IField, IMap>(field, null));
        }

        public void AddField(IField field, IMap map)
        {
            if (field == null) throw new ArgumentNullException("field");
            if (map == null) throw new ArgumentNullException("map");

            if (field.Table != Table)
                throw new DeliveryEngineMetadataException(
                    Resource.GetExceptionMessage(ExceptionMessage.FieldTableMismatch, field.Table.NameTarget, Table.NameTarget), this);

            Map.Add(new KeyValuePair<IField, IMap>(field, map));
        }

        /// <summary>
        /// The validate object.
        /// </summary>
        public virtual object ValidateObject
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Data for the validating object.
        /// </summary>
        public virtual object ValidateObjectData
        {
            get;
            set;
        }
    }
}
