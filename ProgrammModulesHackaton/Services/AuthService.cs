using Microsoft.Data.Sqlite;
using ProgrammModulesHackaton.Helpers;
using ProgrammModulesHackaton.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgrammModulesHackaton.Services
{
    public class AuthService
    {
        private readonly string _connectionString;

        public AuthService()
        {
            _connectionString = AppConfig.ConnectionString;
        }

        public User? Authenticate(string username, string password)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = new SqliteCommand("SELECT * FROM Users WHERE Username = @username", conn);
            cmd.Parameters.AddWithValue("@username", username);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                var storedHash = reader.GetString(reader.GetOrdinal("PasswordHash"));

                if (PasswordHelper.VerifyPassword(password, storedHash))
                {
                    return new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Username = reader.GetString(reader.GetOrdinal("Username")),
                        PasswordHash = storedHash,
                        FullName = reader.GetString(reader.GetOrdinal("FullName")),
                        Role = reader.GetString(reader.GetOrdinal("Role"))
                    };
                }
            }

            return null;
        }
    }
}
