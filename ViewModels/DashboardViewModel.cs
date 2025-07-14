using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Ventuz.Models;
using Ventuz.Views;

namespace Ventuz.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly Manager _manager = new();

        public ObservableCollection<string> AllProjects { get; } = new();
        public ObservableCollection<string> RecentProjects { get; } = new();

        private string? _selectedProject;
        public string? SelectedProject
        {
            get => _selectedProject;
            set
            {
                if (_selectedProject != value)
                {
                    _selectedProject = value;
                    OnPropertyChanged();
                    ((RelayCommand)OpenProjectCommand).CanExecuteChangedInvoke();
                }
            }
        }

        public ICommand OpenProjectCommand { get; }
        public ICommand NewProjectCommand { get; }

        public DashboardViewModel()
        {
            LoadProjects();

            OpenProjectCommand = new RelayCommand(_ => OpenProject(), _ => SelectedProject != null);
            NewProjectCommand = new RelayCommand(_ => CreateNewProject());
        }

        private void LoadProjects()
        {
            AllProjects.Clear();
            foreach (var p in _manager.LoadAllProjects())
                AllProjects.Add(p);

            RecentProjects.Clear();
            foreach (var p in _manager.LoadRecentProjects())
                RecentProjects.Add(p);
        }
        private void OpenProject()
        {
            if (string.IsNullOrWhiteSpace(SelectedProject))
                return;

            var projectPath = _manager.GetProjectPath(SelectedProject);
            if (!Directory.Exists(projectPath))
            {
                MessageBox.Show($"Das Projekt '{SelectedProject}' wurde nicht gefunden.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadProjects();
                return;
            }

            try
            {
                var editor = new Editor(projectPath, _manager);
                editor.Show();

                _manager.AddRecentProject(SelectedProject);
                LoadProjects();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Öffnen des Projekts: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateNewProject()
        {
            var dialog = new InputDialog();
            if (dialog.ShowDialog() != true)
                return;

            string inputName = dialog.InputText;

            if (_manager.LoadAllProjects().Contains(inputName))
            {
                MessageBox.Show("Ein Projekt mit diesem Namen existiert bereits.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _manager.CreateNewProject(inputName);
                LoadProjects();

                SelectedProject = inputName;
                OpenProject();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Anlegen des Projekts: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}