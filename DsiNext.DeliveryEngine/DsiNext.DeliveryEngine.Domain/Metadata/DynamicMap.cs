using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    public class DynamicMap<TSource, TTarget> : MetadataObjectBase, IDynamicMap<TSource, TTarget>
    {
        private readonly ObservableCollection<KeyValuePair<Regex, string>> _rules = new ObservableCollection<KeyValuePair<Regex, string>>();

        private readonly MethodBase _reflectionMethod;

        public DynamicMap() {}

        public DynamicMap(MethodBase reflectionMethod)
        {
            _reflectionMethod = reflectionMethod;
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

            var sourceString = value.ToString();
            var targetString = _rules.Aggregate(sourceString, (current, rule) => rule.Key.Replace(current, rule.Value));
            if (_reflectionMethod != null)
            {
                try
                {
                    return (TTgt) _reflectionMethod.Invoke(null, new object[] {targetString});
                }
                catch (TargetInvocationException ex)
                {
                    var deliveryEngineException = ex.InnerException as DeliveryEngineExceptionBase;
                    if (deliveryEngineException != null)
                    {
                        throw deliveryEngineException;
                    }
                    MappingObjectData = targetString;
                    throw new DeliveryEngineMappingException(Resource.GetExceptionMessage(ExceptionMessage.MapByReflectionFailed, _reflectionMethod.DeclaringType == null ? "{null}" : _reflectionMethod.DeclaringType.Name, _reflectionMethod.Name, targetString), this, ex.InnerException);
                }
            }
            try
            {
                return (TTgt) (object) targetString;
            }
            catch (Exception ex)
            {
                MappingObjectData = targetString;
                throw new DeliveryEngineMappingException(Resource.GetExceptionMessage(ExceptionMessage.MapByTypeCastFailed, typeof (TTarget).Name, targetString), this, ex);
            }
        }

        public virtual ReadOnlyObservableCollection<KeyValuePair<Regex, string>> Rules
        {
            get
            {
                return new ReadOnlyObservableCollection<KeyValuePair<Regex, string>>(_rules);
            }
        }

        public virtual void AddRule(Regex regex, string replacement)
        {
            if (regex == null)
            {
                throw new ArgumentNullException("regex");
            }
            if (string.IsNullOrWhiteSpace(replacement))
            {
                throw new ArgumentNullException("replacement");
            }

            var kvp = new KeyValuePair<Regex, string>(regex, replacement);
            if (_rules.Any(rule => rule.Key.ToString().Equals(regex.ToString())))
            {
                MappingObjectData = regex.ToString();
                throw new DeliveryEngineMappingException(Resource.GetExceptionMessage(ExceptionMessage.ExistingMappingRule, regex.ToString(), replacement), this);
            }
            _rules.Add(kvp);
        }

        /// <summary>
        /// Merged information about the mapping object.
        /// </summary>
        public virtual string ExceptionInfo
        {
            get
            {
                var sourceType = typeof(TSource);
                var targetType = typeof(TTarget);
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
