using Microsoft.Data.Sqlite;
using ProgrammModulesHackaton.Models;
using ProgrammModulesHackaton.Services;
using System;
using System.Linq;
using Xunit;

namespace ProgrammModulesHackaton.Tests
{
    public class AttributeServiceTests
    {
        private readonly AttributeService _service;

        public AttributeServiceTests()
        {
            // Убедись, что AppConfig.ConnectionString указывает на тестовую БД
            _service = new AttributeService();
        }

        [Fact]
        public void AddAttribute_ShouldInsertNewAttribute()
        {
            // Arrange
            var name = "Test Attribute";

            // Act
            _service.AddAttribute(name);
            var all = _service.GetAllAttributes();
            var added = all.FirstOrDefault(a => a.Name == name);

            // Assert
            Assert.NotNull(added);

            // Cleanup
            _service.DeleteAttribute(added.Id);
        }

        [Fact]
        public void GetAttributeById_ShouldReturnCorrectAttribute()
        {
            // Arrange
            var name = "Findable Attribute";
            _service.AddAttribute(name);
            var attr = _service.GetAllAttributes().FirstOrDefault(a => a.Name == name);

            // Act
            var found = _service.GetAttributeById(attr.Id);

            // Assert
            Assert.NotNull(found);
            Assert.Equal(attr.Name, found.Name);

            // Cleanup
            _service.DeleteAttribute(attr.Id);
        }

        [Fact]
        public void DeleteAttribute_ShouldRemoveAttribute()
        {
            // Arrange
            var name = "To Delete";
            _service.AddAttribute(name);
            var attr = _service.GetAllAttributes().FirstOrDefault(a => a.Name == name);

            // Act
            _service.DeleteAttribute(attr.Id);
            var deleted = _service.GetAttributeById(attr.Id);

            // Assert
            Assert.Null(deleted);
        }

        [Fact]
        public void GetAllAttributes_ShouldReturnAllInserted()
        {
            // Arrange
            var name1 = "Attr1_" + Guid.NewGuid();
            var name2 = "Attr2_" + Guid.NewGuid();
            _service.AddAttribute(name1);
            _service.AddAttribute(name2);

            // Act
            var list = _service.GetAllAttributes();

            // Assert
            Assert.Contains(list, a => a.Name == name1);
            Assert.Contains(list, a => a.Name == name2);

            // Cleanup
            var toDelete = list.Where(a => a.Name == name1 || a.Name == name2).ToList();
            foreach (var a in toDelete)
                _service.DeleteAttribute(a.Id);
        }
    }
}
