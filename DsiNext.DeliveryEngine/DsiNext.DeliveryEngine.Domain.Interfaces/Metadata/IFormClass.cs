namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    /// <summary>
    /// Interface for informations about a FORM classification.
    /// </summary>
    public interface IFormClass : INamedObject
    {
        /// <summary>
        /// FORM classification.
        /// </summary>
        string FormClassName
        {
            get;
        }

        /// <summary>
        /// Text for FORM classification.
        /// </summary>
        string FormClassText
        {
            get;
        }
    }
}
