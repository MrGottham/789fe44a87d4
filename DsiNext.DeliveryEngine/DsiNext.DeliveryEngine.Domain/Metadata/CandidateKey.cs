using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    public class CandidateKey : KeyBase, ICandidateKey
    {
        public CandidateKey(string nameSource, string nameTarget) : base(nameSource, nameTarget)
        {
        }

        public CandidateKey(string nameSource, string nameTarget, string description) : base(nameSource, nameTarget, description)
        {
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
