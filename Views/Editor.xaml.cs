using System.Net;
using System.Windows;
using System.Windows.Controls;
using Ventuz.Models;
using Ventuz.ViewModels;
using Ventuz.Remoting4;

namespace Ventuz.Views
{
    /// <summary>
    /// Interaktionslogik für Editor.xaml
    /// </summary>
    public partial class Editor : Window
    {
        private Cluster? _cluster;
        private IID _sceneIID;
        private SceneStatus? _sceneStatus;
        private readonly EditorViewModel _viewModel;
        private readonly Manager _manager;

        public Editor(string projectName, Manager manager)
        {
            InitializeComponent();
            _manager = manager;

            var model = manager.LoadProjectModel(projectName);
            _viewModel = new EditorViewModel
            {
                ProjectName = projectName,
                TargetIP = model.TargetIP,
            };

            _viewModel.Controls.Clear();
            foreach (var control in model.Controls)
            {
                _viewModel.Controls.Add(control);
            }

            DataContext = _viewModel;
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_viewModel.TargetIP))
            {
                AppendDebug($"Keine IP-Adresse angegeben");
                ShowMessage("Bitte IP-Adresse angeben.", isError: true);
                return;
            }

            try
            {
                _cluster = new Cluster();
                _cluster.AddMachine(new IPEndPoint(IPAddress.Parse(_viewModel.TargetIP), 19400));
                _cluster.ClusterStateChanged += Cluster_ClusterStateChanged;
                _cluster.Start();

                _viewModel.ConnectionStatus = "Verbinde...";
                AppendDebug($"Verbindung gestartet zu {_viewModel.TargetIP}");
            }
            catch (Exception ex)
            {
                _viewModel.ConnectionStatus = "Fehler";
                AppendDebug($"Verbindung fehlgeschlagen: {ex.Message}");
                ShowMessage($"Verbindung fehlgeschlagen: {ex.Message}", isError: true);
            }
        }

        private async void Cluster_ClusterStateChanged(object? sender, EventArgs e)
        {
            if (_cluster?.ClusterState == ClusterState.Ok)
            {
                await LoadSceneFromCluster();
                _viewModel.ConnectionStatus = "Verbunden";
                AppendDebug($"Verbunden zu {_viewModel.TargetIP}");
            }
            else
            {
                _viewModel.ConnectionStatus = "Nicht verbunden";
                AppendDebug($"Nicht verbunden zu {_viewModel.TargetIP}");
            }
        }

        private async Task LoadSceneFromCluster()
        {
            var portResult = await _cluster!.PortStatus(0, 0, true, IID.Invalid, null, null);

            if (portResult.IID is not null)
            {
                _sceneIID = portResult.IID.Value;
                _sceneStatus = await _cluster.Status(_sceneIID, null);
            }
            else
            {
                AppendDebug("Fehler beim Laden");
                ShowMessage("Keine Szene geladen oder Fehler beim Laden.", isError: true);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ControlDialog();
            if (dialog.ShowDialog() == true)
                _viewModel.Controls.Add(dialog.Result);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedControl is null)
                return;

            var dialog = new ControlDialog(_viewModel.SelectedControl);
            if (dialog.ShowDialog() == true)
            {
                var index = _viewModel.Controls.IndexOf(_viewModel.SelectedControl);
                _viewModel.Controls[index] = dialog.Result;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedControl is not null)
                _viewModel.Controls.Remove(_viewModel.SelectedControl);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var model = new Model
            {
                Name = _viewModel.ProjectName,
                TargetIP = _viewModel.TargetIP ?? string.Empty,
                Controls = _viewModel.Controls.ToList()
            };

            _manager.SaveProjectModel(_viewModel.ProjectName, model);

            AppendDebug("Projekt gespeichert");
            ShowMessage("Projekt gespeichert.", "Info", MessageBoxImage.Information);
        }

        private async void SendControl_Click(object sender, RoutedEventArgs e)
        {
            if (_cluster is null || _viewModel.Controls is null)
            {
                AppendDebug("Nicht verbunden");
                ShowMessage("Nicht verbunden!", isError: true);
                return;
            }

            if (sender is Button { DataContext: ControlElement element })
            {
                try
                {
                    string valueToSend = element.Value;

                    switch (element.Type.ToLower())
                    {
                        case "button":
                            await _cluster.DataItem(_sceneIID, element.Command, 0, null, null);
                            break;
                        case "switch":
                            await _cluster.DataItem(_sceneIID, element.Command, valueToSend == "1", null, null);
                            break;
                        default:
                            await _cluster.DataItem(_sceneIID, element.Command, valueToSend, null, null);
                            break;
                    }

                    AppendDebug($"Senden erfolgreich: {element.Command} {element.Value}");
                }
                catch (Exception ex)
                {
                    AppendDebug($"Senden fehlgeschlagen: {ex.Message}");
                    ShowMessage($"Fehler beim Senden: {ex.Message}", isError: true);
                }
            }
        }

        private void AppendDebug(string message)
        {
            _viewModel.DebugOutput = $"[{DateTime.Now:T}] {message}";
        }

        private void ShowMessage(string message, string title = "Hinweis", MessageBoxImage icon = MessageBoxImage.Warning, bool isError = false)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        }
    }
}