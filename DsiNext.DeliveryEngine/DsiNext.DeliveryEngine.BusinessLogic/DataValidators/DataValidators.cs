using System;
using System.Collections.ObjectModel;
using Domstolene.JFS.CommonLibrary.IoC.Interfaces;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.DataValidators;

namespace DsiNext.DeliveryEngine.BusinessLogic.DataValidators
{
    /// <summary>
    /// Collection of data validators for the delivery engine.
    /// </summary>
    public class DataValidators : Collection<IDataValidator>, IDataValidators
    {
        #region Constructor

        /// <summary>
        /// Creates a collection of data validators for the delivery engine.
        /// </summary>
        /// <param name="container">Container for Inversion of Control.</param>
        public DataValidators(IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            foreach (var dataValidator in container.ResolveAll<IDataValidator>())
            {
                Add(dataValidator);
            }
        }

        #endregion
    }
}
