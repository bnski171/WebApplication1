using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Distributed;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;

        public WeatherController(IConfiguration configuration, IDistributedCache cache)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _cache = cache;
        }

        [HttpGet("{city}")]
        public async Task<IActionResult> GetWeatherByCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest("City name must be provided.");
            }

            string cacheKey = $"WeatherData:{city}";
            WeatherData weatherData = null;

            string weatherDataJson = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(weatherDataJson))
            {
                weatherData = JsonSerializer.Deserialize<WeatherData>(weatherDataJson);
            }
            else
            {
                string apiKey = _configuration["OpenWeatherMap:ApiKey"];
                string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid={apiKey}";

                try
                {
                    HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        weatherData = JsonConvert.DeserializeObject<WeatherData>(content);

                        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(weatherData), new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                        });
                    }
                    else
                    {
                        return BadRequest($"Failed to get weather data for {city}. Error: {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred while fetching weather data: {ex.Message}");
                }
            }

            return Ok(weatherData);
        }
    }

    public class WeatherData
    {
        public Coord coord { get; set; }
        public Weather[] weather { get; set; }
        public string @base { get; set; }
        public Main main { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int timezone { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }

    public class Coord
    {
        public float lon { get; set; }
        public float lat { get; set; }
    }

    public class Main
    {
        public float temp { get; set; }
        public float feels_like { get; set; }
        public float temp_min { get; set; }
        public float temp_max { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Wind
    {
        public float speed { get; set; }
        public int deg { get; set; }
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class Sys
    {
        public int type { get; set; }
        public int id { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }
}

