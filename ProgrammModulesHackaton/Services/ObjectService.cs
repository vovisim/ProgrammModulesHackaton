using Microsoft.Data.Sqlite;
using ProgrammModulesHackaton;
using ProgrammModulesHackaton.Models;
using System.Collections.Generic;

public class ObjectService
{
    private readonly string _connectionString;

    public ObjectService()
    {
        _connectionString = AppConfig.ConnectionString;
    }

    public List<ControlObject> GetAll()
    {
        var list = new List<ControlObject>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = new SqliteCommand("SELECT Id, Name FROM ControlObjects;", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(new ControlObject
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }

        return list;
    }

    public ControlObject? GetById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = new SqliteCommand("SELECT Id, Name FROM ControlObjects WHERE Id = @id;", conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new ControlObject
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            };
        }

        return null;
    }

    public void Add(string name)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = new SqliteCommand("INSERT INTO ControlObjects (Name) VALUES (@name);", conn);
        cmd.Parameters.AddWithValue("@name", name);
        cmd.ExecuteNonQuery();
    }

    public void Update(int id, string newName)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = new SqliteCommand("UPDATE ControlObjects SET Name = @name WHERE Id = @id;", conn);
        cmd.Parameters.AddWithValue("@name", newName);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        // Сначала удалить все атрибуты, связанные с объектом, если есть FK ограничения
        using var cmdAttributes = new SqliteCommand("DELETE FROM ObjectAttributes WHERE ControlObjectId = @id;", conn);
        cmdAttributes.Parameters.AddWithValue("@id", id);
        cmdAttributes.ExecuteNonQuery();

        // Затем удалить сам объект
        using var cmd = new SqliteCommand("DELETE FROM ControlObjects WHERE Id = @id;", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public List<ControlObject> FindByAttribute(string? attributeName, string? attributeValue)
    {
        var list = new List<ControlObject>();

        if (string.IsNullOrWhiteSpace(attributeName) || string.IsNullOrWhiteSpace(attributeValue))
            return list;

        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        // Поиск объектов, у которых есть атрибут с заданным именем и значением
        string sql = @"
            SELECT DISTINCT co.Id, co.Name
            FROM ControlObjects co
            JOIN ObjectAttributes oa ON co.Id = oa.ControlObjectId
            WHERE oa.AttributeName = @attrName AND oa.AttributeValue = @attrValue;
        ";

        using var cmd = new SqliteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@attrName", attributeName);
        cmd.Parameters.AddWithValue("@attrValue", attributeValue);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new ControlObject
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }

        return list;
    }
}
