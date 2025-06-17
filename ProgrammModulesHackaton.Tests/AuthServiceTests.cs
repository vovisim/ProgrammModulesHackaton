using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using ProgrammModulesHackaton.Helpers;
using ProgrammModulesHackaton.Models;
using ProgrammModulesHackaton.Services;
using Xunit;

namespace ProgrammModulesHackaton.Tests
{
    public class AuthServiceTests
    {
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _authService = new AuthService();
        }

        [Fact]
        public void Authenticate_ShouldReturnUser_WhenCredentialsAreCorrect()
        {
            // Arrange
            string username = "testuser_" + Guid.NewGuid();
            string password = "securepassword";
            string hash = PasswordHelper.HashPassword(password);
            string fullName = "Test User";
            string role = "admin";

            // Вставим тестового пользователя в БД
            using var conn = new SqliteConnection(AppConfig.ConnectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Username, PasswordHash, FullName, Role) VALUES (@username, @hash, @fullName, @role);";
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@hash", hash);
            cmd.Parameters.AddWithValue("@fullName", fullName);
            cmd.Parameters.AddWithValue("@role", role);
            cmd.ExecuteNonQuery();

            // Act
            var user = _authService.Authenticate(username, password);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(username, user.Username);
            Assert.Equal(fullName, user.FullName);
            Assert.Equal(role, user.Role);
            Assert.NotNull(_authService.CurrentUser);

            // Cleanup
            var deleteCmd = conn.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM Users WHERE Username = @username";
            deleteCmd.Parameters.AddWithValue("@username", username);
            deleteCmd.ExecuteNonQuery();
        }

        [Fact]
        public void Authenticate_ShouldReturnNull_WhenPasswordIncorrect()
        {
            // Arrange
            string username = "wrongpass_" + Guid.NewGuid();
            string correctPassword = "correct123";
            string wrongPassword = "incorrect";
            string hash = PasswordHelper.HashPassword(correctPassword);
            string fullName = "Wrong Pass";
            string role = "user";

            using var conn = new SqliteConnection(AppConfig.ConnectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Username, PasswordHash, FullName, Role) VALUES (@username, @hash, @fullName, @role);";
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@hash", hash);
            cmd.Parameters.AddWithValue("@fullName", fullName);
            cmd.Parameters.AddWithValue("@role", role);
            cmd.ExecuteNonQuery();

            // Act
            var result = _authService.Authenticate(username, wrongPassword);

            // Assert
            Assert.Null(result);
            Assert.Null(_authService.CurrentUser);

            // Cleanup
            var deleteCmd = conn.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM Users WHERE Username = @username";
            deleteCmd.Parameters.AddWithValue("@username", username);
            deleteCmd.ExecuteNonQuery();
        }

        [Fact]
        public void Authenticate_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            var result = _authService.Authenticate("nonexistent_user", "any");

            // Assert
            Assert.Null(result);
            Assert.Null(_authService.CurrentUser);
        }
    }
}
