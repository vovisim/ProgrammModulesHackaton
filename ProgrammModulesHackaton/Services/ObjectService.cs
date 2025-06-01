using Microsoft.Data.Sqlite;
using ProgrammModulesHackaton.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammModulesHackaton.Services
{
    public class ObjectService
    {
        private readonly string _connectionString;

        public ObjectService()
        {
            _connectionString = AppConfig.ConnectionString;
        }

        public int AddControlObject(string address, string description)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand(@"
                INSERT INTO ControlObjects (Address, Description)
                VALUES (@address, @description);
                SELECT last_insert_rowid();", conn);

            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@description", description);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<ControlObject> GetAllObjects()
        {
            var result = new List<ControlObject>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand("SELECT Id, Address, Description, CreatedAt FROM ControlObjects;", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new ControlObject
                {
                    Id = reader.GetInt32(0),
                    Address = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3)
                });
            }

            return result;
        }

        public ControlObject GetObjectById(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand("SELECT Id, Address, Description, CreatedAt FROM ControlObjects WHERE Id = @id;", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new ControlObject
                {
                    Id = reader.GetInt32(0),
                    Address = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3)
                };
            }

            return null;
        }

        public void UpdateControlObject(int id, string address, string description)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand(@"
                UPDATE ControlObjects
                SET Address = @address, Description = @description
                WHERE Id = @id;", conn);

            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

        public void DeleteControlObject(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand("DELETE FROM ControlObjects WHERE Id = @id;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
