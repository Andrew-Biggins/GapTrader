using System.ComponentModel;

namespace TradingSharedCore
{
#nullable enable
    public abstract class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Notifies subscribers that a property has been updated.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Usually called from a property setter. Automatically uses the correct property name to raise
        /// PropertyChanged. Only raises the event if the new value is different to the current value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="member">The backing field of the property.</param>
        /// <param name="value">The new value.</param>
        /// <param name="propertyName">The name of the property.</param>
        protected void SetProperty<T>(ref T member, T value, string propertyName = "")
        {
            if (!Equals(member, value))
            {
                member = value;
                PropertyChanged.Raise(this, propertyName);
            }
        }

        /// <summary>
        /// Explicitly raises PropertyChanged with a given property name, regardless of
        /// whether the value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property to raise PropertyChanged with.</param>
        protected void RaisePropertyChanged(string propertyName) => PropertyChanged.Raise(this, propertyName);

        /// <summary>
        /// Can be used to signify that code exists only for the XAML designer.
        /// </summary>
        protected const string XamlDesignerOnly = "To be used by XAML designer only.";
    }
#nullable disable
}
