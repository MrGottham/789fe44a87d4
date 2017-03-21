using System;
using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    public class ForeignKey : KeyBase, IForeignKey
    {
        private ICandidateKey _candidateKey;
        private Cardinality _cardinality;

        public ForeignKey(ICandidateKey candidateKey, string nameSource, string nameTarget, Cardinality cardinality) : base(nameSource, nameTarget)
        {
            if (candidateKey == null) throw new ArgumentNullException("candidateKey");

            _candidateKey = candidateKey;
            _cardinality = cardinality;
        }

        public ForeignKey(ICandidateKey candidateKey, string nameSource, string nameTarget, string description, Cardinality cardinality)
            : base(nameSource, nameTarget, description)
        {
            if (candidateKey == null) throw new ArgumentNullException("candidateKey");

            _candidateKey = candidateKey;
            _cardinality = cardinality;
        }

        public virtual ICandidateKey CandidateKey
        {
            get { return _candidateKey; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (value == _candidateKey) return;

                _candidateKey = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public Cardinality Cardinality
        {
            get { return _cardinality; }
            set
            {
                if (_cardinality == value) return;

                _cardinality = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// The validate object.
        /// </summary>
        public override object ValidateObject
        {
            get
            {
                return this;
            }
        }
    }
}
