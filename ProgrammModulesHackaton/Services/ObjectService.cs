using Microsoft.Data.Sqlite;
using ProgrammModulesHackaton;
using ProgrammModulesHackaton.Models;
using System.Collections.Generic;

public class ObjectService
{
    private readonly string _connectionString = AppConfig.ConnectionString;

    public List<ControlObject> GetAll()
    {
        var list = new List<ControlObject>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = new SqliteCommand("SELECT Id, Name, Address, Description, CreatedAt FROM ControlObjects", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new ControlObject
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Address = reader.GetString(2),
                Description = reader.GetString(3),
                CreatedAt = DateTime.Parse(reader.GetString(4))
            });
        }

        return list;
    }

    public void Add(ControlObject newObj)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = new SqliteCommand(@"
                INSERT INTO ControlObjects (Name, Address, Description, CreatedAt)
                VALUES (@name, @address, @desc, @createdAt)", conn);

        cmd.Parameters.AddWithValue("@name", newObj.Name);
        cmd.Parameters.AddWithValue("@address", newObj.Address);
        cmd.Parameters.AddWithValue("@desc", newObj.Description);
        cmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("s"));
        cmd.ExecuteNonQuery();
    }

    public ControlObject? GetById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = new SqliteCommand("SELECT Id, Name, Address, Description, CreatedAt FROM ControlObjects WHERE Id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new ControlObject
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Address = reader.GetString(2),
                Description = reader.GetString(3),
                CreatedAt = DateTime.Parse(reader.GetString(4))
            };
        }

        return null;
    }

    public void Update(int id, string newName)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = new SqliteCommand("UPDATE ControlObjects SET Name = @name WHERE Id = @id", conn);
        cmd.Parameters.AddWithValue("@name", newName);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = new SqliteCommand("DELETE FROM ControlObjects WHERE Id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public List<ControlObject> FindByAttribute(string attrName, string value)
    {
        var result = new List<ControlObject>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var sql = @"
                SELECT co.Id, co.Name, co.Address, co.Description, co.CreatedAt
                FROM ControlObjects co
                JOIN ObjectAttributes oa ON co.Id = oa.ObjectId
                JOIN Attributes a ON oa.AttributeId = a.Id
                WHERE a.Name = @attrName AND oa.Value = @value";

        using var cmd = new SqliteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@attrName", attrName);
        cmd.Parameters.AddWithValue("@value", value);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new ControlObject
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Address = reader.GetString(2),
                Description = reader.GetString(3),
                CreatedAt = DateTime.Parse(reader.GetString(4))
            });
        }

        return result;
    }
}
