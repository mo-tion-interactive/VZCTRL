using System.Collections.ObjectModel;
using Ventuz.Models;

namespace Ventuz.ViewModels
{
    public class EditorViewModel : ViewModelBase
    {
        private string? _targetIP;
        private ControlElement? _selectedControl;

        public ObservableCollection<ControlElement> Controls { get; set; } = new();

        public string? TargetIP
        {
            get => _targetIP;
            set { _targetIP = value; OnPropertyChanged(); }
        }

        public ControlElement? SelectedControl
        {
            get => _selectedControl;
            set { _selectedControl = value; OnPropertyChanged(); }
        }

        public string ProjectName { get; set; } = "";

        private string _connectionStatus = "Nicht verbunden";
        public string ConnectionStatus
        {
            get => _connectionStatus;
            set { _connectionStatus = value; OnPropertyChanged(); }
        }

        private string? _debugOutput;
        public string DebugOutput
        {
#pragma warning disable CS8603 // Mögliche Nullverweisrückgabe.
            get => _debugOutput;
#pragma warning restore CS8603 // Mögliche Nullverweisrückgabe.
            set
            {
                _debugOutput = value;
                OnPropertyChanged(nameof(DebugOutput));
            }
        }
    }
}