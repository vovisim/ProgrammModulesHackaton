using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using ProgrammModulesHackaton.Helpers;
using ProgrammModulesHackaton.Models;


namespace ProgrammModulesHackaton.Services
{
    internal class DatabaseService
    {

        public static void InitializeDatabase()
        {
            // Убедиться, что папка Data/ создана:
            var projectDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
            var dataDir = Path.Combine(projectDir, "Data");
            Console.WriteLine(dataDir);
            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);

            // Путь к файлу БД:
            string dbPath = Path.Combine(dataDir, "database.db");
            bool isNew = !File.Exists(dbPath);
            Console.WriteLine(dbPath);
            Console.WriteLine(isNew);
           
            // Открываем соединение:
            using var conn = new SqliteConnection($"Data Source={dbPath};");
            conn.Open();

            // Включить поддержку внешних ключей:
            using (var pragma = new SqliteCommand("PRAGMA foreign_keys = ON;", conn))
                pragma.ExecuteNonQuery();

            // Выполняем SQL для создания всех таблиц:
            string sql = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    FullName TEXT NOT NULL,
                    Role TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS ControlObjects (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Address TEXT,
                    Description TEXT,
                    CreatedAt TEXT NOT NULL
                );


                CREATE TABLE IF NOT EXISTS Decisions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ControlObjectId INTEGER NOT NULL,
                    Text TEXT NOT NULL,
                    DueDate DATETIME NOT NULL,
                    Status TEXT NOT NULL,
                    Responsible TEXT NOT NULL,
                    FOREIGN KEY(ControlObjectId) REFERENCES ControlObjects(Id) ON DELETE CASCADE
                );

                CREATE TABLE IF NOT EXISTS Attachments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ControlObjectId INTEGER NOT NULL,
                    FileName TEXT NOT NULL,
                    FileType TEXT NOT NULL,
                    Data BLOB NOT NULL,
                    UploadedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY(ControlObjectId) REFERENCES ControlObjects(Id) ON DELETE CASCADE
                );

               CREATE TABLE IF NOT EXISTS Attributes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL
                );


                CREATE TABLE IF NOT EXISTS ObjectAttributes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ObjectId INTEGER NOT NULL,
                    AttributeId INTEGER NOT NULL,
                    FOREIGN KEY (ObjectId) REFERENCES ControlObjects(Id) ON DELETE CASCADE,
                    FOREIGN KEY (AttributeId) REFERENCES Attributes(Id) ON DELETE CASCADE
                );
            ";
            using (var cmd = new SqliteCommand(sql, conn))
                cmd.ExecuteNonQuery();

            // Если база только создана, добавить начальные роли/пользователя Admin:
            if (isNew)
            {
                using var tx = conn.BeginTransaction();
                using var cmdRole = new SqliteCommand("INSERT INTO Users (Username, PasswordHash, FullName, Role) VALUES ('admin', @hash, 'Admin', 'Admin');", conn, tx);
                // Предполагаем, что PasswordHelper.GenerateHash("admin123") вернёт хеш пароля "admin123":
                cmdRole.Parameters.AddWithValue("@hash", PasswordHelper.HashPassword("admin123"));
                cmdRole.ExecuteNonQuery();
                tx.Commit();
                Console.WriteLine("Создан пользователь admin (пароль: admin123), роль: Admin.");
            }

            Console.WriteLine("База данных и таблицы инициализированы.");
        }


    }
}
