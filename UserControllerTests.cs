using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Eventify_High_Performance_Event_Management_API.Controller;
using Eventify_High_Performance_Event_Management_API.Models;
using Eventify_High_Performance_Event_Management_API.Dtos;
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
        [Fact]
        public async Task LoginUser_ShouldReturnOk_WithToken_WhenCredentialsAreValid()
        {
            var email = "test@example.com";
            var password = "password123";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var fakeUser = new User { UserId = 1, Email = email, PasswordHash = passwordHash, IsAdmin = false };

            _mockRepo.Setup(repo => repo.GetUserByEmailAsync(email))
                     .ReturnsAsync(fakeUser);

            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(s => s.Value).Returns("super_secret_key_with_at_least_64_characters_12345678901234567890");
            _mockConfig.Setup(c => c.GetSection("AppSettings:PasswordKey")).Returns(mockSection.Object);

            var LoginDto = new UserToLoginDto { Email = email, Password = password };

            var result = await _controller.Login(LoginDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenPasswordIsIncorrect()
        {
            // Arrange
            var email = "test@test.com";
            var fakeUser = new User { Email = email, PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct_pass") };
            _mockRepo.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(fakeUser);

            var loginDto = new UserToLoginDto { Email = email, Password = "wrong_password" };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}