using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System;
using System.IO;
using ProgrammModulesHackaton.Models;

namespace ProgrammModulesHackaton.Services
{
    public class XmlImportService
    {
        private readonly string _connectionString;

        public XmlImportService()
        {
            _connectionString = AppConfig.ConnectionString;
        }

        /// <summary>
        /// Импортирует из XML-файла объекты ControlObject и связанные с ними решения и атрибуты.
        /// Ожидаемая структура XML:
        /// <Objects>
        ///   <ControlObject>
        ///     <Name>...</Name>
        ///     <Address>...</Address>
        ///     <Description>...</Description>
        ///     <Attributes>
        ///       <Attribute Name="..." Value="..." />
        ///       ...
        ///     </Attributes>
        ///     <Decisions>
        ///       <Decision>
        ///         <Text>...</Text>
        ///         <DueDate>yyyy-MM-dd</DueDate>
        ///         <Status>...</Status>
        ///         <Responsible>...</Responsible>
        ///       </Decision>
        ///       ...
        ///     </Decisions>
        ///   </ControlObject>
        ///   ...
        /// </Objects>
        /// </summary>
        public void ImportFromXml(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("XML-файл не найден", filePath);

            var doc = XDocument.Load(filePath);
            var root = doc.Root;
            if (root == null || root.Name != "Objects")
                throw new InvalidDataException("Некорректная структура XML: отсутствует корневой <Objects>");

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            foreach (var xo in root.Elements("ControlObject"))
            {
                // Читаем поля объекта
                var name = xo.Element("Name")?.Value?.Trim() ?? throw new InvalidDataException("<Name> обязательна");
                var address = xo.Element("Address")?.Value?.Trim() ?? "";
                var description = xo.Element("Description")?.Value?.Trim() ?? "";

                // Вставляем ControlObject
                long objectId;
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = @"
                        INSERT INTO ControlObjects (Name, Address, Description, CreatedAt)
                        VALUES (@name, @address, @desc, @now);
                        SELECT last_insert_rowid();";
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@address", address);
                    cmd.Parameters.AddWithValue("@desc", description);
                    cmd.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("s"));
                    objectId = (long)cmd.ExecuteScalar()!;
                }

                // Атрибуты (если есть)
                var attrs = xo.Element("Attributes");
                if (attrs != null)
                {
                    foreach (var xa in attrs.Elements("Attribute"))
                    {
                        var attrName = xa.Attribute("Name")?.Value?.Trim();
                        if (string.IsNullOrEmpty(attrName))
                            continue;
                        using var cmd = conn.CreateCommand();
                        cmd.Transaction = tx;
                        cmd.CommandText = @"
                            INSERT INTO ObjectAttributes (ObjectId, AttributeId)
                            SELECT @objId, Id FROM Attributes WHERE Name = @name
                            ;
                            ";
                        cmd.Parameters.AddWithValue("@objId", objectId);
                        cmd.Parameters.AddWithValue("@name", attrName);
                        cmd.ExecuteNonQuery();
                        // если нужно сохранять Value, добавьте соответствующий столбец и параметр
                    }
                }

                // Решения (если есть)
                var decs = xo.Element("Decisions");
                if (decs != null)
                {
                    foreach (var xd in decs.Elements("Decision"))
                    {
                        var text = xd.Element("Text")?.Value?.Trim() ?? "";
                        var dueStr = xd.Element("DueDate")?.Value?.Trim() ?? "";
                        if (!DateTime.TryParse(dueStr, out var dueDate))
                            dueDate = DateTime.UtcNow.AddDays(7);
                        var status = xd.Element("Status")?.Value?.Trim() ?? "Ожидает";
                        var resp = xd.Element("Responsible")?.Value?.Trim() ?? "";

                        using var cmd = conn.CreateCommand();
                        cmd.Transaction = tx;
                        cmd.CommandText = @"
                            INSERT INTO Decisions 
                              (ControlObjectId, Text, DueDate, Status, Responsible)
                            VALUES
                              (@objId, @text, @due, @status, @resp);";
                        cmd.Parameters.AddWithValue("@objId", objectId);
                        cmd.Parameters.AddWithValue("@text", text);
                        cmd.Parameters.AddWithValue("@due", dueDate.ToString("s"));
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@resp", resp);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            tx.Commit();
        }
    }
}
