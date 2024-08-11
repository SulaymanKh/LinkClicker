using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using LinkClicker_API.Controllers;
using LinkClicker_API.Models.Admin;
using LinkClicker_API.Models.Generic;
using LinkClicker_API.Database;
using LinkClicker_API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LinkClicker_API.Models.LinkClickerDatabase;

namespace LinkClicker_API.Tests.Tests
{
    public class AdminControllerTests
    {
        private readonly LinkDbContext _context;
        private readonly Mock<LinkDbContext> _mockContext;
        private readonly Mock<ILogger<AdminController>> _mockLogger;
        private readonly Mock<IBackgroundTaskQueue> _mockTaskQueue;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            var options = new DbContextOptionsBuilder<LinkDbContext>()
                  .UseInMemoryDatabase(databaseName: "LinkClickerTestDb")
                  .EnableSensitiveDataLogging()
                  .Options;

            _context = new LinkDbContext(options);
            _mockContext = new Mock<LinkDbContext>(new DbContextOptions<LinkDbContext>());
            _mockLogger = new Mock<ILogger<AdminController>>();
            _mockTaskQueue = new Mock<IBackgroundTaskQueue>();
            _controller = new AdminController(_context, _mockLogger.Object, _mockTaskQueue.Object);
           
        }

        [Fact]
        public async Task CreateLink_ShouldReturnSuccess_WhenLinkCanBeCreated()
        {
            //Arrange
            var request = new CreateLinkRequestModel
            {
                Url = "www.test.co.uk/",
                ClicksPerLink = 1,
                ExpiryInMinutes = 1,
                Username = "TestUser",
                NumberOfLinks = 1
            };

            var link = new Link
            {
                Id = Guid.NewGuid(),
                Status = LinkStatus.Active,
                Username = request.Username,
                Url = request.Url,
                ClickCount = 0,
                MaxClicks = request.ClicksPerLink,
                ExpiryTime = DateTime.UtcNow.AddMinutes((double)request.ExpiryInMinutes)
            };

            _context.Links.Add(link);
            await _context.SaveChangesAsync();

            //Act
            var result = await _controller.CreateLink(request) as OkObjectResult;
            var response = result?.Value as ResponseWrapper<List<LinkInfoModel>>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Success", response.Information);
            Assert.False(response.IsError);
        }

        [Fact]
        public async Task CreateLink_ShouldReturnError_When200kLinksAlreadyExist()
        {
            //Arrange
            var request = new CreateLinkRequestModel
            {
                Url = "www.test.co.uk/",
                ClicksPerLink = 1,
                ExpiryInMinutes = 1,
                Username = "TestUser",
                NumberOfLinks = 1
            };

            for (int i = 0; i < 200000; i++)
            {
                var link = new Link
                {
                    Id = Guid.NewGuid(),
                    Status = LinkStatus.Active,
                    Username = $"User{i}",
                    Url = $"www.test{i}.co.uk",
                    ClickCount = 0,
                    MaxClicks = request.ClicksPerLink,
                    ExpiryTime = DateTime.UtcNow.AddMinutes((double)request.ExpiryInMinutes)
                };

                _context.Links.Add(link);
            }
            await _context.SaveChangesAsync();

            //Act
            var result = await _controller.CreateLink(request) as OkObjectResult;
            var response = result?.Value as ResponseWrapper<List<LinkInfoModel>>;

            //Assert
            Assert.NotNull(result);
            Assert.True(response.IsError);
            Assert.Equal("The system has reached the maximum number of links (200,000). No more requests can be accepted.", response.Information);
        }

        [Fact]
        public async Task DeleteLinks_ShouldReturnSuccess_WhenExpiredByClicksRequestReceived()
        {
            //Arrange
            var request = new DeleteLinksRequestModel
            {
                DeleteAll = true,
                Statuses = new List<LinkStatus> { LinkStatus.ExpiredByClicks }
            };

            var expiredLink = new Link
            {
                Id = Guid.NewGuid(),
                Status = LinkStatus.ExpiredByClicks, 
                Username = "TestUser",
                Url = "www.test.co.uk",
                ClickCount = 5,
                MaxClicks = 10,
                ExpiryTime = DateTime.UtcNow.AddMinutes(50) 
            };

            _context.Links.Add(expiredLink);
            await _context.SaveChangesAsync();

            //Act
            var result = await _controller.DeleteLinks(request) as OkObjectResult;
            var response = result.Value as ResponseWrapper<string>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Links have been deleted based on the provided criteria.", response.Data);
            Assert.False(response.IsError);
        }

        [Fact]
        public async Task DeleteLinks_ShouldReturnSuccess_WhenExpiredByTimeRequestReceived()
        {
            //Arrange
            var request = new DeleteLinksRequestModel
            {
                DeleteAll = true,
                Statuses = new List<LinkStatus> { LinkStatus.ExpiredByClicks }
            };

            var expiredLink = new Link
            {
                Id = Guid.NewGuid(),
                Status = LinkStatus.ExpiredByTime,
                Username = "TestUser",
                Url = "www.test.co.uk",
                ClickCount = 5,
                MaxClicks = 10,
                ExpiryTime = DateTime.UtcNow.AddMinutes(50)
            };

            _context.Links.Add(expiredLink);
            await _context.SaveChangesAsync();

            //Act
            var result = await _controller.DeleteLinks(request) as OkObjectResult;
            var response = result.Value as ResponseWrapper<string>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Links have been deleted based on the provided criteria.", response.Data);
            Assert.False(response.IsError);
        }

        [Fact]
        public async Task GetLinkDetails_ShouldReturnLinkDetails_WhenLinkExists()
        {
            //Arrange
            var id = Guid.NewGuid();
            var link = new Link
            {
                Id = id,
                Status = LinkStatus.Active,
                Username = "TestUser",
                Url = "http://testurl.com",
                ExpiryTime = DateTime.UtcNow.AddHours(1),
                MaxClicks = 10,
                ClickCount = 0
            };

            _context.Links.Add(link);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetLinkDetails(id) as OkObjectResult;
            var response = result?.Value as ResponseWrapper<GetLinkDetailsModel>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal($"You have found the secret, {link.Username}!.", response?.Information);
            Assert.False(response.IsError);
            Assert.NotNull(response.Data);
        }
    }
}
