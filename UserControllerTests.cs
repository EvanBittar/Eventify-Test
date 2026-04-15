using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Eventify_High_Performance_Event_Management_API.Controller;
using Eventify_High_Performance_Event_Management_API.Models;
using Eventify_High_Performance_Event_Management_API.Repository.Interfaces;

namespace Eventify.Tests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly UserController _controller;

        // هذا يجب أن يكون الـ Constructor الوحيد في الملف
        public UserControllerTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _mockConfig = new Mock<IConfiguration>();

            // نمرر الـ Mocks للكنترولر
            _controller = new UserController(_mockRepo.Object, _mockConfig.Object);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnOk_WhenUserExists()
        {
            // Arrange
            var fakeUser = new User { UserId = 2, FirstName = "Evan" };

            _mockRepo.Setup(repo => repo.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(fakeUser);

            // Act
            var result = await _controller.GetUserById(2);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}