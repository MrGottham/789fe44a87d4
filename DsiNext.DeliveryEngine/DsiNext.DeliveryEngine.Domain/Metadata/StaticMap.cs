using System;
using System.Collections.Generic;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    public class StaticMap<TSource, TTarget> : MetadataObjectBase, IStaticMap<TSource, TTarget>
    {
        private readonly IDictionary<TSource, TTarget> _rules = new Dictionary<TSource, TTarget>();

        public virtual TTarget MapValue(TSource value)
        {
            if (_rules.ContainsKey(value) == false)
            {
                MappingObjectData = value;
                throw new DeliveryEngineMappingException(Resource.GetExceptionMessage(ExceptionMessage.MapByDictionaryFailed, value), this);
            }

            return _rules[value];
        }

        public virtual IDictionary<TSource, TTarget> Rules
        {
            get
            {
                return _rules;
            }
        }

        public virtual void AddRule(TSource source, TTarget target)
        {
            if (_rules.ContainsKey(source))
            {
                if (_rules[source].Equals(target) == false)
                {
                    MappingObjectData = source;
                    throw new DeliveryEngineMappingException(Resource.GetExceptionMessage(ExceptionMessage.ExistingMappingRule, source, target), this);
                }
                return;
            }
            _rules.Add(source, target);
        }

        public virtual TTgt MapValue<TSrc, TTgt>(TSrc value)
        {
            if (typeof (TSource) != typeof (TSrc))
            {
                MappingObjectData = value;
                throw new DeliveryEngineMappingException(Resource.GetExceptionMessage(ExceptionMessage.TypeMismatch, typeof (TSource).Name, typeof (TSrc).Name), this);
            }
            if (typeof (TTarget) != typeof (TTgt))
            {
                MappingObjectData = value;
                throw new DeliveryEngineMappingException(Resource.GetExceptionMessage(ExceptionMessage.TypeMismatch, typeof (TTarget).Name, typeof (TTgt).Name), this);
            }

            if (Equals(value, null) && (typeof (TTgt) == typeof (string) || (typeof (TTgt).IsGenericType && typeof (TTgt).GetGenericTypeDefinition() == typeof (Nullable<>))))
            {
                object nullObject = null;
                // ReSharper disable ExpressionIsAlwaysNull
                return (TTgt) nullObject;
                // ReSharper restore ExpressionIsAlwaysNull
            }
            if (Equals(value, string.Empty) && typeof (TTgt) == typeof (string))
            {
                if (_rules.ContainsKey((TSource) (object) value))
                {
                    return (TTgt) (object) _rules[(TSource) (object) value];
                }
                object emptyString = value as string;
                return (TTgt) emptyString;
            }
            if (_rules.ContainsKey((TSource) (object) value) == false)
            {
                MappingObjectData = value;
                throw new DeliveryEngineMappingException(Resource.GetExceptionMessage(ExceptionMessage.MapByDictionaryFailed, value), this);
            }
            return (TTgt) (object) _rules[(TSource) (object) value];
        }

        /// <summary>
        /// Merged information about the mapping object.
        /// </summary>
        public virtual string ExceptionInfo
        {
            get
            {
                var sourceType = typeof (TSource);
                var targetType = typeof (TTarget);
                return string.Format("{0}, TSource={1}, TTarget={2}", GetType().Name, sourceType.Name, targetType.Name);
            }
        }

        /// <summary>
        /// The mapping object.
        /// </summary>
        public virtual object MappingObject
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Data for the mapping object.
        /// </summary>
        public virtual object MappingObjectData
        {
            get;
            set;
        }
    }
}
