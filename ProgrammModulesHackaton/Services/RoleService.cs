using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammModulesHackaton.Services
{
    public class RoleService
    {
        private readonly string _connectionString;

        public RoleService()
        {
            _connectionString = AppConfig.ConnectionString;
        }

        // Получить все уникальные роли из таблицы Users
        public List<string> GetAllRoles()
        {
            var roles = new List<string>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = "SELECT DISTINCT Role FROM Users;";

            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                roles.Add(reader.GetString(0));
            }

            return roles;
        }

        // Проверить, есть ли роль в таблице
        public bool RoleExists(string roleName)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = "SELECT COUNT(1) FROM Users WHERE Role = @roleName;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@roleName", roleName);

            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
    }
}
