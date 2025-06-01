using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ProgrammModulesHackaton.Models;

namespace ProgrammModulesHackaton.Services
{
    public class FileService
    {
        private readonly string _connectionString;

        public FileService()
        {
            _connectionString = AppConfig.ConnectionString;
        }

        public int UploadFile(int controlObjectId, string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден", filePath);

            string fileName = Path.GetFileName(filePath);
            string fileType = Path.GetExtension(filePath)?.TrimStart('.').ToLower();

            byte[] fileData = File.ReadAllBytes(filePath);

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand(@"
                INSERT INTO Attachments (ControlObjectId, FileName, FileType, Data)
                VALUES (@objectId, @name, @type, @data);
                SELECT last_insert_rowid();", conn);

            cmd.Parameters.AddWithValue("@objectId", controlObjectId);
            cmd.Parameters.AddWithValue("@name", fileName);
            cmd.Parameters.AddWithValue("@type", fileType);
            cmd.Parameters.AddWithValue("@data", fileData);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<Attachment> GetFilesForObject(int controlObjectId)
        {
            var files = new List<Attachment>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand(@"
                SELECT Id, ControlObjectId, FileName, FileType, Data, UploadedAt
                FROM Attachments
                WHERE ControlObjectId = @id;", conn);

            cmd.Parameters.AddWithValue("@id", controlObjectId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                files.Add(new Attachment
                {
                    Id = reader.GetInt32(0),
                    ControlObjectId = reader.GetInt32(1),
                    FileName = reader.GetString(2),
                    FileType = reader.GetString(3),
                    Data = (byte[])reader["Data"],
                    UploadedAt = reader.GetDateTime(5)
                });
            }

            return files;
        }

        public void SaveFileToDisk(Attachment file, string destinationFolder)
        {
            if (!Directory.Exists(destinationFolder))
                Directory.CreateDirectory(destinationFolder);

            string outputPath = Path.Combine(destinationFolder, file.FileName);
            File.WriteAllBytes(outputPath, file.Data);

            Console.WriteLine($"Файл сохранён: {outputPath}");
        }

        public void DeleteFile(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand("DELETE FROM Attachments WHERE Id = @id;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
