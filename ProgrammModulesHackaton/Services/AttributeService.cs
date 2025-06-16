using Microsoft.Data.Sqlite;
using ProgrammModulesHackaton.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Generic;

namespace ProgrammModulesHackaton.Services
{
    public class AttributeService
    {
        private readonly string _connectionString;

        public AttributeService()
        {
            _connectionString = AppConfig.ConnectionString;
        }

        public void AddAttribute(string name)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand("INSERT INTO Attributes (Name) VALUES (@name);", conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.ExecuteNonQuery();
        }

        public void DeleteAttribute(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand("DELETE FROM Attributes WHERE Id = @id;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public List<Models.Attribute> GetAllAttributes()
        {
            var list = new List<Models.Attribute>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand("SELECT Id, Name FROM Attributes;", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Models.Attribute
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }

            return list;
        }

        public Models.Attribute? GetAttributeById(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand("SELECT Id, Name FROM Attributes WHERE Id = @id;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Models.Attribute
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                };
            }

            return null;
        }

        public List<ObjectAttribute> GetAttributesByObjectId(int objectId)
        {
            var result = new List<ObjectAttribute>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = new SqliteCommand(@"
            SELECT Id, ObjectId, AttributeId
            FROM ObjectAttributes
            WHERE ObjectId = @objectId;", conn);

            cmd.Parameters.AddWithValue("@objectId", objectId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new ObjectAttribute
                {
                    Id = reader.GetInt32(0),
                    ObjectId = reader.GetInt32(1),
                    AttributeId = reader.GetInt32(2),
                });
            }

            return result;
        }

    }
}
