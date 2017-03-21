using System.Text.RegularExpressions;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Enums;

namespace DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators
{
    /// <summary>
    /// Interface for a regular expression replacer which can set, change or clear a field value.
    /// </summary>
    public interface IRegularExpressionReplacer : IDataManipulator
    {
        /// <summary>
        /// Regular expression for the replacer.
        /// </summary>
        Regex RegularExpression
        {
            get;
        }

        /// <summary>
        /// Descripes when the regular expression should set, change or clear the field value.
        /// </summary>
        RegularExpressionApplyOn ApplyOn
        {
            get;
        }

        /// <summary>
        /// Name of the field on which to set or change data.
        /// </summary>
        string FieldName
        {
            get;
        }

        /// <summary>
        /// The new field value.
        /// </summary>
        object FieldValue
        {
            get;
        }
    }
}
