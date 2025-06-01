using Microsoft.Data.Sqlite;
using ProgrammModulesHackaton.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammModulesHackaton.Services
{
    public class AttributeService
    {
        private readonly string _connectionString;

        public AttributeService()
        {
            _connectionString = AppConfig.ConnectionString;
        }

        public void AddAttribute(int controlObjectId, string name, string value)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = new SqliteCommand(@"
                INSERT INTO ObjectAttributes (ControlObjectId, AttributeName, AttributeValue)
                VALUES (@controlObjectId, @name, @value);", conn);

            cmd.Parameters.AddWithValue("@controlObjectId", controlObjectId);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@value", value);
            cmd.ExecuteNonQuery();
        }

        public void UpdateAttribute(int attributeId, string newValue)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = new SqliteCommand(@"
                UPDATE ObjectAttributes
                SET AttributeValue = @value
                WHERE Id = @id;", conn);

            cmd.Parameters.AddWithValue("@value", newValue);
            cmd.Parameters.AddWithValue("@id", attributeId);
            cmd.ExecuteNonQuery();
        }

        public void DeleteAttribute(int attributeId)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = new SqliteCommand("DELETE FROM ObjectAttributes WHERE Id = @id;", conn);
            cmd.Parameters.AddWithValue("@id", attributeId);
            cmd.ExecuteNonQuery();
        }

        public List<ObjectAttribute> GetAttributesByObjectId(int controlObjectId)
        {
            var result = new List<ObjectAttribute>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = new SqliteCommand(@"
                SELECT Id, ControlObjectId, AttributeName, AttributeValue
                FROM ObjectAttributes
                WHERE ControlObjectId = @controlObjectId;", conn);

            cmd.Parameters.AddWithValue("@controlObjectId", controlObjectId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new ObjectAttribute
                {
                    Id = reader.GetInt32(0),
                    ControlObjectId = reader.GetInt32(1),
                    AttributeName = reader.GetString(2),
                    AttributeValue = reader.GetString(3)
                });
            }

            return result;
        }
    }
}
