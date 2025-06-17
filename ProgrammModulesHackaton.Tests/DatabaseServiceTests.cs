using System;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using ProgrammModulesHackaton.Helpers;
using ProgrammModulesHackaton.Services;
using Xunit;

namespace ProgrammModulesHackaton.Tests
{
    public class DatabaseServiceTests
    {
        private readonly string _testDbPath;
        private readonly string _testDataDir;

        public DatabaseServiceTests()
        {
            var testBasePath = Path.Combine(Path.GetTempPath(), "DbServiceTests");
            _testDataDir = Path.Combine(testBasePath, "Data");
            _testDbPath = Path.Combine(_testDataDir, "database.db");
            

            // Очистим перед каждым тестом
            if (Directory.Exists(_testDataDir))
                Directory.Delete(_testDataDir, recursive: true);
        }

        [Fact]
        public void InitializeDatabase_ShouldCreateDatabaseFile()
        {
            // Act
            DatabaseService.InitializeDatabase();

            // Assert
            Assert.False(File.Exists(_testDbPath));
        }

        [Fact]
        public void InitializeDatabase_ShouldCreateTables()
        {
            // Act
            DatabaseService.InitializeDatabase();

            // Assert: Проверим, что таблицы существуют
            using var conn = new SqliteConnection($"Data Source={_testDbPath}");
            conn.Open();

            string[] requiredTables = new[]
            {
                "Users", "ControlObjects", "Decisions",
                "Attachments", "Attributes", "ObjectAttributes"
            };

            foreach (var table in requiredTables)
            {
                using var cmd = new SqliteCommand($"SELECT name FROM sqlite_master WHERE type='table' AND name='{table}';", conn);
                var result = cmd.ExecuteScalar();
                Assert.Equal(table, result);
            }
        }

        [Fact]
        public void InitializeDatabase_ShouldInsertAdminUser_IfDbNew()
        {
            // Act
            DatabaseService.InitializeDatabase();

            // Assert
            using var conn = new SqliteConnection($"Data Source={_testDbPath}");
            conn.Open();

            using var cmd = new SqliteCommand("SELECT Username, FullName, Role FROM Users WHERE Username = 'admin';", conn);
            using var reader = cmd.ExecuteReader();

            Assert.True(reader.Read());
            Assert.Equal("admin", reader.GetString(0));
            Assert.Equal("Admin", reader.GetString(1));
            Assert.Equal("Admin", reader.GetString(2));
        }

        [Fact]
        public void InitializeDatabase_ShouldNotInsertAdminTwice()
        {
            // Act
            DatabaseService.InitializeDatabase();
            DatabaseService.InitializeDatabase(); // Повторно

            using var conn = new SqliteConnection($"Data Source={_testDbPath}");
            conn.Open();

            using var cmd = new SqliteCommand("SELECT COUNT(*) FROM Users WHERE Username = 'admin';", conn);
            long count = (long)cmd.ExecuteScalar();

            Assert.Equal(1, count); // должен быть только один admin
        }
    }
}
