using System.ComponentModel;

namespace Ventuz.Models
{
    public class ControlElement : INotifyPropertyChanged
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string Command { get; set; } = "";

        private string _value = "";
        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class Model
    {
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? TargetIP { get; set; }

        public List<ControlElement> Controls { get; set; } = new();
    }
}