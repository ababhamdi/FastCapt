using System.Windows;
using System.Windows.Data;

namespace FastCapt.Controls
{
    internal static class BindingHelpers
    {
        internal static void SetBinding(this DependencyObject target,
            DependencyProperty targetProperty,
            object source,
            string sourcePropertyPath,
            BindingMode bindingMode = BindingMode.TwoWay)
        {
            var binding = new Binding
            {
                Source = source,
                Path = new PropertyPath(sourcePropertyPath),
                Mode = bindingMode
            };
            BindingOperations.SetBinding(target, targetProperty, binding);
        }
    }
}