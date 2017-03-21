namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    /// <summary>
    /// 
    /// </summary>
    public interface IForeignKey : IKey
    {
        /// <summary>
        /// 
        /// </summary>
        ICandidateKey CandidateKey { get; }

        /// <summary>
        /// 
        /// </summary>
        Cardinality Cardinality { get; set; }
    }
}
