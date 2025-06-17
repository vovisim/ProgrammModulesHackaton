using System;
using System.Linq;
using Xunit;
using ProgrammModulesHackaton.Models;
using ProgrammModulesHackaton.Services; // важно: подключи namespace с настоящим сервисом

namespace ProgrammModulesHackaton.Tests
{
    public class ControlObjectServiceTests
    {
        [Fact]
        public void Add_ShouldInsertControlObject()
        {
            // Arrange
            var service = new ObjectService(); // <--- настоящий сервис
            var testObject = new ControlObject
            {
                Name = "Test Object",
                Address = "Test Address",
                Description = "Test Desc",
                CreatedAt = DateTime.Now
            };

            // Act
            service.Add(testObject);
            var inserted = service.GetAll().FirstOrDefault(o => o.Name == "Test Object");

            // Assert
            Assert.NotNull(inserted);
            Assert.Equal("Test Address", inserted.Address);

            // Cleanup
            service.Delete(inserted.Id);
        }
    }
}
