using Microsoft.Data.Sqlite;
using ProgrammModulesHackaton.Models;

namespace ProgrammModulesHackaton.Services
{
    public class ObjectAttributeService
    {
        private readonly string _connectionString = AppConfig.ConnectionString;

        public void AssignAttribute(int objectId, int attributeId, string value)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = new SqliteCommand(@"
                INSERT INTO ObjectAttributes (ObjectId, AttributeId, Value)
                VALUES (@objectId, @attrId, @value);", conn);

            cmd.Parameters.AddWithValue("@objectId", objectId);
            cmd.Parameters.AddWithValue("@attrId", attributeId);
            cmd.Parameters.AddWithValue("@value", value);
            cmd.ExecuteNonQuery();
        }

        public List<ObjectAttribute> GetAttributesByObjectId(int objectId)
        {
            var result = new List<ObjectAttribute>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var sql = "SELECT Id, ObjectId, AttributeId, Value FROM ObjectAttributes WHERE ObjectId = @objectId";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@objectId", objectId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new ObjectAttribute
                {
                    Id = reader.GetInt32(0),
                    ObjectId = reader.GetInt32(1),
                    AttributeId = reader.GetInt32(2),
                    Value = reader.GetString(3)
                });
            }

            return result;
        }

        public void DeleteAttributeAssignment(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = new SqliteCommand("DELETE FROM ObjectAttributes WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void UpdateAttributeValue(int id, string newValue)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = new SqliteCommand("UPDATE ObjectAttributes SET Value = @val WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@val", newValue);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}

