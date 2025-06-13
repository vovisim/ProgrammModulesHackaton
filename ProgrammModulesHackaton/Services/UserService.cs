using Microsoft.Data.Sqlite;
using ProgrammModulesHackaton.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammModulesHackaton.Services
{
    public class UserService
    {
        private readonly string _connectionString;

        public UserService()
        {
            _connectionString = AppConfig.ConnectionString;
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = "SELECT Id, Username, PasswordHash, FullName, Role FROM Users;";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    FullName = reader.GetString(3),
                    Role = reader.GetString(4)
                });
            }

            return users;
        }

        public User? GetUserByUsername(string username)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = "SELECT Id, Username, PasswordHash, FullName, Role FROM Users WHERE Username = @username;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    FullName = reader.GetString(3),
                    Role = reader.GetString(4)
                };
            }

            return null;
        }

        

        public void AddUser(User user)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = "INSERT INTO Users (Username, PasswordHash, FullName, Role) VALUES (@username, @hash, @fullname, @role);";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", user.Username);
            cmd.Parameters.AddWithValue("@hash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@fullname", user.FullName);
            cmd.Parameters.AddWithValue("@role", user.Role);

            cmd.ExecuteNonQuery();
        }

        public User? GetUserById(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = "SELECT Id, Username, PasswordHash, FullName, Role FROM Users WHERE Id = @id;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    FullName = reader.GetString(3),
                    Role = reader.GetString(4)
                };
            }

            return null;
        }

        public void UpdateUser(User user)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = "UPDATE Users SET Username = @username, PasswordHash = @hash, FullName = @fullname, Role = @role WHERE Id = @id;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", user.Username);
            cmd.Parameters.AddWithValue("@hash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@fullname", user.FullName);
            cmd.Parameters.AddWithValue("@role", user.Role);
            cmd.Parameters.AddWithValue("@id", user.Id);

            cmd.ExecuteNonQuery();
        }

        public void DeleteUser(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = "DELETE FROM Users WHERE Id = @id;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

    }
}
