using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProgrammModulesHackaton.Services
{
    public class XmlImportService
    {
        private readonly string _connectionString;

        public XmlImportService()
        {
            _connectionString = AppConfig.ConnectionString;
        }

        public void ImportFromXml(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден.");
                return;
            }

            var doc = XDocument.Load(filePath);
            var objects = doc.Root?.Elements("ControlObject");
            if (objects == null)
            {
                Console.WriteLine("Некорректная структура XML.");
                return;
            }

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var tx = conn.BeginTransaction();

            foreach (var obj in objects)
            {
                string address = obj.Element("Address")?.Value ?? "Не указан";
                string description = obj.Element("Description")?.Value ?? "";

                // Вставка объекта
                var cmdObj = new SqliteCommand("INSERT INTO ControlObjects (Address, Description) VALUES (@address, @description); SELECT last_insert_rowid();", conn, tx);
                cmdObj.Parameters.AddWithValue("@address", address);
                cmdObj.Parameters.AddWithValue("@description", description);
                var controlObjectId = (long)cmdObj.ExecuteScalar();

                // Импорт атрибутов
                var attrs = obj.Element("Attributes")?.Elements("Attribute");
                if (attrs != null)
                {
                    foreach (var attr in attrs)
                    {
                        string name = attr.Element("Name")?.Value ?? "";
                        string value = attr.Element("Value")?.Value ?? "";

                        var cmdAttr = new SqliteCommand("INSERT INTO ObjectAttributes (ControlObjectId, AttributeName, AttributeValue) VALUES (@id, @name, @value);", conn, tx);
                        cmdAttr.Parameters.AddWithValue("@id", controlObjectId);
                        cmdAttr.Parameters.AddWithValue("@name", name);
                        cmdAttr.Parameters.AddWithValue("@value", value);
                        cmdAttr.ExecuteNonQuery();
                    }
                }

                // Импорт решений
                var decisions = obj.Element("Decisions")?.Elements("Decision");
                if (decisions != null)
                {
                    foreach (var dec in decisions)
                    {
                        string text = dec.Element("Text")?.Value ?? "";
                        string status = dec.Element("Status")?.Value ?? "Новое";
                        string responsible = dec.Element("Responsible")?.Value ?? "Не указан";
                        string dueStr = dec.Element("DueDate")?.Value ?? "";
                        if (!DateTime.TryParse(dueStr, out var dueDate))
                            dueDate = DateTime.Now.AddDays(7);

                        var cmdDec = new SqliteCommand("INSERT INTO Decisions (ControlObjectId, Text, DueDate, Status, Responsible) VALUES (@id, @text, @due, @status, @resp);", conn, tx);
                        cmdDec.Parameters.AddWithValue("@id", controlObjectId);
                        cmdDec.Parameters.AddWithValue("@text", text);
                        cmdDec.Parameters.AddWithValue("@due", dueDate);
                        cmdDec.Parameters.AddWithValue("@status", status);
                        cmdDec.Parameters.AddWithValue("@resp", responsible);
                        cmdDec.ExecuteNonQuery();
                    }
                }
            }

            tx.Commit();
            Console.WriteLine("Импорт из XML завершён успешно.");
        }
    }
}
