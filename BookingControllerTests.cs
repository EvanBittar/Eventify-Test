using System.Net.WebSockets;
using System.Security.Claims;
using Eventify_High_Performance_Event_Management_API.Controller;
using Eventify_High_Performance_Event_Management_API.Dtos;
using Eventify_High_Performance_Event_Management_API.Models;
using Eventify_High_Performance_Event_Management_API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Eventify.Tests
{
    public class BookingControllerTests
    {
        private readonly Mock<IBookingRepository> _mockRepo;
        private readonly BookingController _controller;

        public BookingControllerTests()
        {
            _mockRepo = new Mock<IBookingRepository>();
            _controller = new BookingController(_mockRepo.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
        [Fact]
        public async Task CreateBooking_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            int eventId = 3;
            _mockRepo.Setup(repo => repo.CreateBookingAsync(It.IsAny<BookingDto>()))
                     .ReturnsAsync("Success");
            // Act
            var result = await _controller.CreateBooking(eventId);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            dynamic data = okResult.Value;
            string message = data.GetType().GetProperty("Message").GetValue(data, null);

            Assert.Equal("Booking created successfully.", message);
        }
        [Fact]
        public async Task CreateBooking_ShouldReturnBadRequest_WhenEventIsFull()
        {
            // Arrange
            int eventId= 3;
            _mockRepo.Setup(repo => repo.CreateBookingAsync(It.IsAny<BookingDto>()))
                     .ReturnsAsync("Event is full");
            // Act
            var result = await _controller.CreateBooking(eventId);
            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task CancelBooking_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            int bookingid = 3;
            _mockRepo.Setup(repo => repo.CancelBookingAsync(bookingid))
                     .ReturnsAsync(true);
            // Act
            var result = await _controller.CancelBooking(bookingid);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic data = okResult.Value;
            string message = data.GetType().GetProperty("Message").GetValue(data, null);

            Assert.Equal("Booking cancelled successfully.", message);
        }
        [Fact]
        public async Task CancelBooking_ShouldReturnBadRequest_WhenFails()
        {
            // Arrange
            int bookingid = 3;
            _mockRepo.Setup(repo => repo.CancelBookingAsync(bookingid))
                     .ReturnsAsync(false);
            // Act
            var result = await _controller.CancelBooking(bookingid);
            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            dynamic data = badRequestResult.Value;
            string message = data.GetType().GetProperty("Message").GetValue(data, null);

            Assert.Equal("Could not cancel booking or booking not found.", message);

        }
    }
}