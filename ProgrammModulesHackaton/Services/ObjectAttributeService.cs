using Microsoft.Data.Sqlite;
using ProgrammModulesHackaton.Models;

namespace ProgrammModulesHackaton.Services
{
    public class ObjectAttributeService
    {
        private readonly string _connectionString = AppConfig.ConnectionString;

        public void AssignAttribute(ObjectAttribute newObjAttr)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = new SqliteCommand(@"
                INSERT INTO ObjectAttributes (ObjectId, AttributeId, Value)
                VALUES (@objectId, @attrId, @value);", conn);

            cmd.Parameters.AddWithValue("@objectId", newObjAttr.ObjectId);
            cmd.Parameters.AddWithValue("@attrId", newObjAttr.AttributeId);
            cmd.ExecuteNonQuery();
        }

        public void AssignAttributes(List<ObjectAttribute> newObjAttr)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            foreach (var item in newObjAttr)
            {
                using var cmd = new SqliteCommand(@"
            INSERT INTO ObjectAttributes (ObjectId, AttributeId)
            VALUES (@objectId, @attrId);", conn);

                cmd.Parameters.AddWithValue("@objectId", item.ObjectId);
                cmd.Parameters.AddWithValue("@attrId", item.AttributeId);
                cmd.ExecuteNonQuery();
            }
        }


        public List<AttributesName> GetAttributesByObjectId(int objectId)
        {
            var result = new List<AttributesName>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var sql = @"
            SELECT 
                ObjectAttributes.Id,
                ObjectAttributes.ObjectId,
                Attributes.Name
            FROM ObjectAttributes
            INNER JOIN Attributes ON ObjectAttributes.AttributeId = Attributes.Id
            WHERE ObjectAttributes.ObjectId = @objectId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@objectId", objectId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new AttributesName
                {
                    Id = reader.GetInt32(0),
                    ObjectId = reader.GetInt32(1),
                    AttributeName = reader.GetString(2)
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

        public void DeleteAttributesByObjectId(int objectId)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = new SqliteCommand(
                "DELETE FROM ObjectAttributes WHERE ObjectId = @objectId;", conn);

            cmd.Parameters.AddWithValue("@objectId", objectId);
            cmd.ExecuteNonQuery();
        }

    }
}

