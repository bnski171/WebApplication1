using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplication1.Controllers;
using Xunit;

namespace WebApplication1.Tests
{
    public class WeatherForecastControllerTests
    {
        private readonly Mock<ILogger<WeatherForecastController>> _mockLogger;
        private readonly WeatherForecastController _controller;

        public WeatherForecastControllerTests()
        {
            _mockLogger = new Mock<ILogger<WeatherForecastController>>();
            _controller = new WeatherForecastController(_mockLogger.Object);
        }

        [Fact]
        public void Get_Returns_Correct_Number_Of_WeatherForecasts()
        {
           
            const int expectedCount = 5;

           
            var result = _controller.Get();

            
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public void Get_Returns_WeatherForecasts_With_Valid_Temperature_Ranges()
        {
           
            const int minTemperatureC = -20;
            const int maxTemperatureC = 55;

           
            var result = _controller.Get().ToList();

            
            Assert.All(result, forecast =>
            {
                Assert.InRange(forecast.TemperatureC, minTemperatureC, maxTemperatureC);
            });
        }

        [Fact]
        public void Get_Returns_WeatherForecasts_With_Valid_Summaries()
        {
            
            var result = _controller.Get().ToList();

            
            Assert.All(result, forecast =>
            {
                Assert.Contains(WeatherForecastController.Summaries, summary => summary == forecast.Summary);
            });
        }
    }
}
