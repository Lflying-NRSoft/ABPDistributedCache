using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Volo.Abp.Caching;

namespace DistributedCache_GetOrAdd_ErrorDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        protected readonly IDistributedCache<WeatherForecast[]> _distributedCache;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IDistributedCache<WeatherForecast[]> distributedCache)
        {
            _logger = logger;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> GetAsync()
        {
            var key = nameof(WeatherForecast);
            return await _distributedCache.GetOrAddAsync(key, async () => await GetWeatherForecastAsync());
        }

        [HttpGet("Test")]
        public IEnumerable<WeatherForecast> Test()
        {
            var key = nameof(WeatherForecast);
            return _distributedCache.GetOrAdd(key, () =>
            {
                _logger.LogDebug("没有命中缓存");
                var rng = new Random();
                var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
                return result;
            });
        }

        public Task<WeatherForecast[]> GetWeatherForecastAsync()
        {
            _logger.LogDebug("没有命中缓存");
            var rng = new Random();
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            return Task.FromResult(result);
        }
    }
}
