using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using Xunit;
using WebApplication1.Controllers;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace WebApplication1.Tests
{
    public class WeatherControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IDistributedCache> _mockCache;
        private readonly Mock<AppDbContext> _mockDbContext;
        private readonly WeatherController _controller;

        public WeatherControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockCache = new Mock<IDistributedCache>();
            _mockDbContext = new Mock<AppDbContext>(new DbContextOptionsBuilder<AppDbContext>().Options);
            _controller = new WeatherController(_mockConfiguration.Object, _mockCache.Object, _mockDbContext.Object);
        }

        [Fact]
        public async Task CreateTemperatureRecord_Returns_BadRequest_For_Null()
        {
            var result = await _controller.CreateTemperatureRecord(null);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateTemperatureRecord_Returns_Created_For_Valid_TemperatureRecord()
        {
            var temperatureRecord = new TemperatureRecord
            {
                Id = 1,
                City = "Moscow",
                Temperature = 20
            };

            _mockDbContext.Setup(db => db.TemperatureRecords.Add(temperatureRecord));

            var result = await _controller.CreateTemperatureRecord(temperatureRecord);

            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status201Created, (result as StatusCodeResult).StatusCode);
        }

        [Fact]
        public async Task UpdateTemperatureRecord_Returns_BadRequest_For_Null()
        {
            var result = await _controller.UpdateTemperatureRecord(1, null);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteTemperatureRecord_Returns_NotFound_For_Invalid_Id()
        {
            _mockDbContext.Setup(db => db.TemperatureRecords.FindAsync(1)).ReturnsAsync((TemperatureRecord)null);
            var result = await _controller.DeleteTemperatureRecord(1);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetWeatherByCity_Returns_BadRequest_For_Empty_City()
        {
            var result = await _controller.GetWeatherByCity(string.Empty);
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
