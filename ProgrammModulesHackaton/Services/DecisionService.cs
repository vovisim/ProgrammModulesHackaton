using Microsoft.Data.Sqlite;
using ProgrammModulesHackaton.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammModulesHackaton.Services
{
    public class DecisionService
    {
        private readonly string _connectionString;

        public DecisionService()
        {
            _connectionString = AppConfig.ConnectionString;
        }

        public Decision? GetById(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand("SELECT Id, ControlObjectId, Text, DueDate, Status, Responsible FROM Decisions WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Decision
                {
                    Id = reader.GetInt32(0),
                    ControlObjectId = reader.GetInt32(1),
                    Text = reader.GetString(2),
                    DueDate = reader.GetDateTime(3),
                    Status = reader.GetString(4),
                    Responsible = reader.GetString(5)
                };
            }

            return null;
        }

        public List<Decision> GetAll()
        {
            var list = new List<Decision>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand("SELECT Id, ControlObjectId, Text, DueDate, Status, Responsible FROM Decisions", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Decision
                {
                    Id = reader.GetInt32(0),
                    ControlObjectId = reader.GetInt32(1),
                    Text = reader.GetString(2),
                    DueDate = reader.GetDateTime(3),
                    Status = reader.GetString(4),
                    Responsible = reader.GetString(5)
                });
            }

            return list;
        }


        public int AddDecision(Decision decision)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand(@"
                INSERT INTO Decisions (ControlObjectId, Text, DueDate, Status, Responsible)
                VALUES (@objectId, @text, @dueDate, @status, @responsible);
                SELECT last_insert_rowid();", conn);

            cmd.Parameters.AddWithValue("@objectId", decision.ControlObjectId);
            cmd.Parameters.AddWithValue("@text", decision.Text);
            cmd.Parameters.AddWithValue("@dueDate", decision.DueDate);
            cmd.Parameters.AddWithValue("@status", decision.Status);
            cmd.Parameters.AddWithValue("@responsible", decision.Responsible);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<Decision> GetDecisionsForObject(int controlObjectId)
        {
            var list = new List<Decision>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand("SELECT Id, ControlObjectId, Text, DueDate, Status, Responsible FROM Decisions WHERE ControlObjectId = @id", conn);
            cmd.Parameters.AddWithValue("@id", controlObjectId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Decision
                {
                    Id = reader.GetInt32(0),
                    ControlObjectId = reader.GetInt32(1),
                    Text = reader.GetString(2),
                    DueDate = reader.GetDateTime(3),
                    Status = reader.GetString(4),
                    Responsible = reader.GetString(5)
                });
            }

            return list;
        }

        public void UpdateDecision(Decision decision)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand(@"
                UPDATE Decisions
                SET Text = @text, DueDate = @dueDate, Status = @status, Responsible = @responsible
                WHERE Id = @id", conn);

            cmd.Parameters.AddWithValue("@text", decision.Text);
            cmd.Parameters.AddWithValue("@dueDate", decision.DueDate);
            cmd.Parameters.AddWithValue("@status", decision.Status);
            cmd.Parameters.AddWithValue("@responsible", decision.Responsible);
            cmd.Parameters.AddWithValue("@id", decision.Id);

            cmd.ExecuteNonQuery();
        }

        public void DeleteDecision(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand("DELETE FROM Decisions WHERE Id = @id;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public List<Decision> GetOverdueDecisions()
        {
            var list = new List<Decision>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand(@"
                SELECT Id, ControlObjectId, Text, DueDate, Status, Responsible
                FROM Decisions
                WHERE DueDate < CURRENT_TIMESTAMP AND Status != 'Выполнено';", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Decision
                {
                    Id = reader.GetInt32(0),
                    ControlObjectId = reader.GetInt32(1),
                    Text = reader.GetString(2),
                    DueDate = reader.GetDateTime(3),
                    Status = reader.GetString(4),
                    Responsible = reader.GetString(5)
                });
            }

            return list;
        }
    }
}
