using System.Windows;
using System.Windows.Data;

namespace GapAnalyserWPF.Helpers
{
    public sealed class ValidationWrapper : DependencyObject
    {
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(nameof(Data), typeof(object),
                typeof(ValidationWrapper), new PropertyMetadata(default(object), OnValueChanged));

        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValidationWrapper comparisonValue = (ValidationWrapper)d;
            BindingExpressionBase bindingExpressionBase = BindingOperations.GetBindingExpressionBase(comparisonValue, BindingToTriggerProperty);
            bindingExpressionBase?.UpdateSource();
        }

        public object BindingToTrigger
        {
            get { return GetValue(BindingToTriggerProperty); }
            set { SetValue(BindingToTriggerProperty, value); }
        }

        public static readonly DependencyProperty BindingToTriggerProperty = DependencyProperty.Register(
            nameof(BindingToTrigger),
            typeof(object),
            typeof(ValidationWrapper),
            new FrameworkPropertyMetadata(default(object), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}

