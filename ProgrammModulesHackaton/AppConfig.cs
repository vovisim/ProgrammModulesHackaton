using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammModulesHackaton
{
    public static class AppConfig
    {
        public static readonly string ProjectDir =
            Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName!;

        public static readonly string DataDir = Path.Combine(ProjectDir, "Data");

        public static readonly string DbPath = Path.Combine(DataDir, "database.db");

        public static readonly string ConnectionString = $"Data Source={DbPath};";
    }
}
