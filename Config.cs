using System.IO;

namespace Ventuz
{
    public static class Config
    {
        public static string ProjectName = "project.json";
        public static string ProjectRootFolder = @"C:\Ventuz (Demo)";
        public static string RecentProjectsFile => Path.Combine(ProjectRootFolder, "recent.json");
    }
}