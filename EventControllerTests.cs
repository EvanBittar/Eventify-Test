using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Eventify_High_Performance_Event_Management_API.Controller;
using Eventify_High_Performance_Event_Management_API.Repository.Interfaces;
using Eventify_High_Performance_Event_Management_API.Models;
using Eventify_High_Performance_Event_Management_API.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Eventify.Tests
{
    public class EventControllerTests
    {
        private readonly Mock<IEventRepository> _mockRepo;
        private readonly EventController _controller;

        public EventControllerTests()
        {
            _mockRepo = new Mock<IEventRepository>();
            _controller = new EventController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllEvents_ShouldReturnOk_WithListOfEvents()
        {
            // Arrange
            var fakeEvents = new List<Event>
            {
                new Event { EventId = 1, Title = "Test Event" }
            };
            _mockRepo.Setup(repo => repo.GetAllEventsAsync()).ReturnsAsync(fakeEvents);

            // Act
            var result = await _controller.GetAllEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEvents = Assert.IsAssignableFrom<IEnumerable<Event>>(okResult.Value);
            Assert.NotNull(returnedEvents);
            Assert.Single(returnedEvents);
        }

        [Fact]
        public async Task AddEvent_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var newEventDto = new EventToAddDto { Title = "New Event", MaxAttendees = 100 };
            _mockRepo.Setup(repo => repo.CreateEventAsync(newEventDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddEvent(newEventDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Event added successfully.", okResult.Value);
        }

        [Fact]
        public async Task UpdateEvent_ShouldReturnNotFound_WhenEventDoesNotExist()
        {
            // Arrange
            int eventId = 999;
            var updateDto = new EventToAddDto { Title = "Updated Title" };
            _mockRepo.Setup(repo => repo.UpdateEventAsync(eventId, updateDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateEvent(eventId, updateDto);

            // Assert
            var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to Updated event.", notFoundResult.Value);
        }
        [Fact]
        public async Task SearchEvents_ShouldReturnOk_WithMatchingEvents()
        {
            // Arrange
            string searchTerm = "Test";
            var fakeEvents = new List<Event>
            {
                new Event { EventId = 1, Title = "Test Event" , CategoryId = 2 }
            };
            _mockRepo.Setup(repo => repo.SearchEvents(searchTerm, null)).ReturnsAsync(fakeEvents);

            // Act
            var result = await _controller.SearchEvents(searchTerm , null);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEvents = Assert.IsAssignableFrom<IEnumerable<dynamic>>(okResult.Value);
            Assert.NotNull(returnedEvents);
        }
        [Fact]
        public async Task SearchEvents_ShouldReturnEmpty_WhenNoMatch()
        {
            // Arrange
            string serachTerm = "none";
            var fakeEvent = new List<Event>();
            _mockRepo.Setup(repo => repo.SearchEvents(serachTerm, null)).ReturnsAsync(new List<dynamic>()); 
            // Act 
            var result = await _controller.SearchEvents(serachTerm, null);
            // Assert 
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEvents = Assert.IsAssignableFrom<IEnumerable<dynamic>>(okResult.Value);
            Assert.Empty(returnedEvents);

        }
    }
}