using ProgrammModulesHackaton.Models;
using ProgrammModulesHackaton.Services;
using System;
using System.Linq;
using Xunit;

namespace ProgrammModulesHackaton.Tests
{
    public class AgendaServiceTests
    {
        private readonly ObjectService _objectService = new();
        private readonly DecisionService _decisionService = new();
        private readonly AgendaService _agendaService = new();

        [Fact]
        public void GetObjectsWithOverdueDecisions_ShouldReturnObject_WhenDecisionIsOverdue()
        {
            // Arrange
            var testObject = new ControlObject
            {
                Name = "Overdue Test Object",
                Address = "Test Address",
                Description = "Test Desc",
                CreatedAt = DateTime.Now
            };
            _objectService.Add(testObject);
            var inserted = _objectService.GetAll().First(o => o.Name == "Overdue Test Object");

            _decisionService.AddDecision(new Decision
            {
                ControlObjectId = inserted.Id,
                Text = "Test Decision",
                DueDate = DateTime.Now.AddDays(-1),
                Status = "В работе",
                Responsible = "Tester"
            });

            // Act
            var result = _agendaService.GetObjectsWithOverdueDecisions();

            // Assert
            Assert.Contains(result, r => r.Id == inserted.Id);

            // Cleanup
            _objectService.Delete(inserted.Id);
        }

        [Fact]
        public void GetObjectsWithNewDecisions_ShouldReturnObject_WhenDecisionIsNew()
        {
            // Arrange
            var testObject = new ControlObject
            {
                Name = "New Decision Test",
                Address = "Somewhere",
                Description = "",
                CreatedAt = DateTime.Now
            };
            _objectService.Add(testObject);
            var inserted = _objectService.GetAll().First(o => o.Name == "New Decision Test");

            _decisionService.AddDecision(new Decision
            {
                ControlObjectId = inserted.Id,
                Text = "New Decision",
                DueDate = DateTime.Now.AddDays(1),
                Status = "Новое",
                Responsible = "Admin"
            });

            // Act
            var result = _agendaService.GetObjectsWithNewDecisions();

            // Assert
            Assert.Contains(result, r => r.Id == inserted.Id);

            // Cleanup
            _objectService.Delete(inserted.Id);
        }

        [Fact]
        public void GetActiveAgenda_ShouldReturnObject_WhenDecisionNotCompleted()
        {
            // Arrange
            var testObject = new ControlObject
            {
                Name = "Active Agenda Test",
                Address = "Nowhere",
                Description = "",
                CreatedAt = DateTime.Now
            };
            _objectService.Add(testObject);
            var inserted = _objectService.GetAll().First(o => o.Name == "Active Agenda Test");

            _decisionService.AddDecision(new Decision
            {
                ControlObjectId = inserted.Id,
                Text = "Active Decision",
                DueDate = DateTime.Now.AddDays(3),
                Status = "В работе",
                Responsible = "Team"
            });

            // Act
            var result = _agendaService.GetActiveAgenda();

            // Assert
            Assert.Contains(result, r => r.Id == inserted.Id);

            // Cleanup
            _objectService.Delete(inserted.Id);
        }
    }
}
