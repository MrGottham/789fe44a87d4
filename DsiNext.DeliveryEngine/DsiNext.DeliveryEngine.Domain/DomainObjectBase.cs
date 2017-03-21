using System;
using System.ComponentModel;
using DsiNext.DeliveryEngine.Domain.Interfaces;

namespace DsiNext.DeliveryEngine.Domain
{
    /// <summary>
    /// Basic domain object in the delivery engine.
    /// </summary>
    public abstract class DomainObjectBase : IDomainObjectBase
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Raise an PropertyChanged event.
        /// </summary>
        /// <param name="sender">Object who are raising the event.</param>
        /// <param name="propertyName">Name of the property which are changed.</param>
        protected virtual void RaisePropertyChanged(object sender, string propertyName)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }
            if (PropertyChanged == null)
            {
                return;
            }
            PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
