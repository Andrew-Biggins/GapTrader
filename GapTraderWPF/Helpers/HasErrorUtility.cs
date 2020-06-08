using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GapTraderWPF.Helpers
{
    public sealed class HasErrorUtility
    {
        public static readonly DependencyProperty MvvmHasErrorProperty = DependencyProperty.RegisterAttached("MvvmHasError",
                                                                    typeof(bool),
                                                                    typeof(HasErrorUtility),
                                                                    new FrameworkPropertyMetadata(false,
                                                                                                  FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                                  null,
                                                                                                  CoerceMvvmHasError));

        public static bool GetMvvmHasError(DependencyObject d)
        {
            return (bool)d.GetValue(MvvmHasErrorProperty);
        }

        public static void SetMvvmHasError(DependencyObject d, bool value)
        {
            d.SetValue(MvvmHasErrorProperty, value);
        }

        private static object CoerceMvvmHasError(DependencyObject d, Object baseValue)
        {
            var ret = (bool)baseValue;

            if (BindingOperations.IsDataBound(d, MvvmHasErrorProperty))
            {
                if (GetHasErrorDescriptor(d) == null)
                {
                    DependencyPropertyDescriptor desc = DependencyPropertyDescriptor.FromProperty(Validation.HasErrorProperty, d.GetType());
                    desc.AddValueChanged(d, OnHasErrorChanged);
                    SetHasErrorDescriptor(d, desc);
                    ret = Validation.GetHasError(d);
                }
            }
            else
            {
                if (GetHasErrorDescriptor(d) != null)
                {
                    DependencyPropertyDescriptor desc = GetHasErrorDescriptor(d);
                    desc.RemoveValueChanged(d, OnHasErrorChanged);
                    SetHasErrorDescriptor(d, null);
                }
            }

            return ret;
        }

        private static readonly DependencyProperty HasErrorDescriptorProperty = DependencyProperty.RegisterAttached("HasErrorDescriptor",
                                                                                typeof(DependencyPropertyDescriptor),
                                                                                typeof(HasErrorUtility));

        private static DependencyPropertyDescriptor GetHasErrorDescriptor(DependencyObject d)
        {
            var ret = d.GetValue(HasErrorDescriptorProperty);
            return ret as DependencyPropertyDescriptor;
        }

        private static void OnHasErrorChanged(object sender, EventArgs e)
        {
            if (sender is DependencyObject d)
            {
                d.SetValue(MvvmHasErrorProperty, d.GetValue(Validation.HasErrorProperty));
            }
        }

        private static void SetHasErrorDescriptor(DependencyObject d, DependencyPropertyDescriptor value)
        {
            var ret = d.GetValue(HasErrorDescriptorProperty);
            d.SetValue(HasErrorDescriptorProperty, value);
        }
    }
}

