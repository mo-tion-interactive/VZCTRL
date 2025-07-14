using System.Windows;
using System.Windows.Controls;
using Ventuz.Models;

namespace Ventuz.Views
{
    public class ControlTypeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? ButtonTemplate { get; set; }
        public DataTemplate? SliderTemplate { get; set; }
        public DataTemplate? SwitchTemplate { get; set; }
        public DataTemplate? LabelTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is not ControlElement element)
                return base.SelectTemplate(item, container);

            return element.Type?.ToLower() switch
            {
                "button" => ButtonTemplate,
                "slider" => SliderTemplate,
                "switch" => SwitchTemplate,
                "label" => LabelTemplate,
                _ => base.SelectTemplate(item, container)
            };
        }
    }
}