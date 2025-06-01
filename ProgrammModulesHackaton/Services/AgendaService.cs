using Microsoft.Data.Sqlite;
using ProgrammModulesHackaton.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammModulesHackaton.Services
{
    public class AgendaService
    {
        private readonly string _connectionString;

        public AgendaService()
        {
            _connectionString = AppConfig.ConnectionString;
        }

        /// <summary>
        /// Объекты с решениями, срок исполнения которых истёк и не завершены
        /// </summary>
        public List<ControlObject> GetObjectsWithOverdueDecisions()
        {
            var result = new List<ControlObject>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT DISTINCT co.Id, co.Address, co.Description, co.CreatedAt
                FROM ControlObjects co
                JOIN Decisions d ON co.Id = d.ControlObjectId
                WHERE d.DueDate < CURRENT_TIMESTAMP AND d.Status != 'Выполнено';
            ";

            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new ControlObject
                {
                    Id = reader.GetInt32(0),
                    Address = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3)
                });
            }

            return result;
        }

        /// <summary>
        /// Объекты, у которых есть хотя бы одно новое (ещё не рассмотренное) решение
        /// </summary>
        public List<ControlObject> GetObjectsWithNewDecisions()
        {
            var result = new List<ControlObject>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT DISTINCT co.Id, co.Address, co.Description, co.CreatedAt
                FROM ControlObjects co
                JOIN Decisions d ON co.Id = d.ControlObjectId
                WHERE d.Status = 'Новое';
            ";

            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new ControlObject
                {
                    Id = reader.GetInt32(0),
                    Address = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3)
                });
            }

            return result;
        }

        /// <summary>
        /// Все объекты с активными поручениями (не выполненными)
        /// </summary>
        public List<ControlObject> GetActiveAgenda()
        {
            var result = new List<ControlObject>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT DISTINCT co.Id, co.Address, co.Description, co.CreatedAt
                FROM ControlObjects co
                JOIN Decisions d ON co.Id = d.ControlObjectId
                WHERE d.Status != 'Выполнено';
            ";

            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new ControlObject
                {
                    Id = reader.GetInt32(0),
                    Address = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3)
                });
            }

            return result;
        }
    }
}
