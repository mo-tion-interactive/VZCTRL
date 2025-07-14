using System.IO;
using System.Text.Json;

namespace Ventuz.Models
{
    public class Manager
    {
        private static readonly string ProjectName = Config.ProjectName;

        private static readonly string ProjectRootFolder = Config.ProjectRootFolder;
        private static string RecentProjectsFile => Config.RecentProjectsFile;

        public List<string> LoadAllProjects()
        {
            if (!Directory.Exists(ProjectRootFolder))
                Directory.CreateDirectory(ProjectRootFolder);

            var dirs = Directory.GetDirectories(ProjectRootFolder);
#pragma warning disable CS8619 // Die NULL-Zulässigkeit von Verweistypen im Wert entspricht nicht dem Zieltyp.
            return dirs.Select(selector: Path.GetFileName)
                .OrderBy(n => n)
                .ToList();
#pragma warning restore CS8619 // Die NULL-Zulässigkeit von Verweistypen im Wert entspricht nicht dem Zieltyp.
        }

        public List<string> LoadRecentProjects()
        {
            if (!File.Exists(RecentProjectsFile))
                return new List<string>();

            var json = File.ReadAllText(RecentProjectsFile);
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }

        public void SaveRecentProjects(List<string> recentProjects)
        {
            var json = JsonSerializer.Serialize(recentProjects, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(RecentProjectsFile, json);
        }

        public void AddRecentProject(string projectName)
        {
            var recent = LoadRecentProjects();
            recent.Remove(projectName);
            recent.Insert(0, projectName);
            if (recent.Count > 10)
                recent.RemoveAt(recent.Count - 1);
            SaveRecentProjects(recent);
        }

        public string GetProjectPath(string projectName)
        {
            return Path.Combine(ProjectRootFolder, projectName);
        }

        public void CreateNewProject(string projectName)
        {
            var path = GetProjectPath(projectName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public Model LoadProjectModel(string projectName)
        {
            var path = Path.Combine(GetProjectPath(projectName), ProjectName);

            if (!File.Exists(path))
            {
                return new Model
                {
                    Name = projectName,
                    CreatedAt = DateTime.Now
                };
            }

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Model>(json) ?? new Model { Name = projectName };
        }

        public void SaveProjectModel(string projectName, Model model)
        {
            var path = Path.Combine(GetProjectPath(projectName), ProjectName);

            var json = JsonSerializer.Serialize(model, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json);
        }
    }
}