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

        /// <summary>
        /// 异步测试方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> GetAsync()
        {
            var key = nameof(WeatherForecast);
            return await _distributedCache.GetOrAddAsync(key, async () =>
            {
                return await GetWeatherForecastAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        [HttpGet("RedisMGetasync")]
        public async Task<string[]> GetRedisMGetAsync()
        {
            var keys = GetKeys();
            var list = await RedisHelper.MGetAsync(keys.ToArray());
            return list;
        }

        [HttpGet("RedisMGet")]
        public string[] GetRedisMGet()
        {
            var keys = GetKeys();
            var list = RedisHelper.MGet(keys.ToArray());

            return list;
        }

        private List<string> GetKeys()
        {
            var list = new List<string>();
            list.Add("Stock_LQCK01_000_ZCPCSP_ZCYS01_ZCCM01");
            list.Add("Stock_LQCK01_000_ZCPCSP_ZCYS01_ZCCM03");
            list.Add("Stock_LQCK01_000_LQ01_1001_1001");
            list.Add("Stock_LQCK01_000_WXSP01_YPYS01_YPCM01");
            list.Add("Stock_LQCK01_000_SP02_1001_1001");
            list.Add("Stock_LQCK01_000_SP1001_YS1001_CM1001");
            list.Add("Stock_LQCK01_000_ZS01_1001_1001");
            list.Add("Stock_LQCK01_000_ZS01_1001_1001");
            list.Add("Stock_LQCK01_000_ZS02_1001_1001");
            list.Add("Stock_LQCK01_000_ZSTX_1001_1001");
            list.Add("Stock_LQSD01_000_ZCPCSP_ZCYS01_ZCCM01");
            list.Add("Stock_LQSD01_000_ZCPCSP_ZCYS01_ZCCM03");
            list.Add("Stock_LQSD01_000_LQ01_1001_1001");
            list.Add("Stock_LQSD01_000_WXSP01_YPYS01_YPCM01");
            list.Add("Stock_LQSD01_000_SP02_1001_1001");
            list.Add("Stock_LQSD01_000_SP1001_YS1001_CM1001");
            list.Add("Stock_LQSD01_000_ZS01_1001_1001");
            list.Add("Stock_LQSD01_000_ZS02_1001_1001");
            list.Add("Stock_LQSD01_000_ZSTX_1001_1001");
            list.Add("Stock_WXHDGS_000_ZCPCSP_ZCYS01_ZCCM01");
            list.Add("Stock_WXHDGS_000_ZCPCSP_ZCYS01_ZCCM03");
            list.Add("Stock_WXHDGS_000_LQ01_1001_1001");
            list.Add("Stock_WXHDGS_000_WXSP01_YPYS01_YPCM01");
            list.Add("Stock_WXHDGS_000_ZS01_1001_1001");
            list.Add("Stock_WXHDGS_000_ZS02_1001_1001");
            list.Add("Stock_WXHDGS_000_ZSTX_1001_1001");
            list.Add("Stock_WYSD01_000_ZCPCSP_ZCYS01_ZCCM01");
            list.Add("Stock_WYSD01_000_ZCPCSP_ZCYS01_ZCCM03");
            list.Add("Stock_WYSD01_000_LQ01_1001_1001");
            list.Add("Stock_WYSD01_000_WXSP01_YPYS01_YPCM01");
            list.Add("Stock_WYSD01_000_SP02_1001_1001");
            list.Add("Stock_WYSD01_000_ZS01_1001_1001");
            list.Add("Stock_WYSD01_000_ZSTX_1001_1001");
            list.Add("Stock_ZS01_000_ZCPCSP_ZCYS01_ZCCM01");
            list.Add("Stock_ZS01_000_ZCPCSP_ZCYS01_ZCCM03");
            list.Add("Stock_ZS01_000_LQ01_1001_1001");
            list.Add("Stock_ZS01_000_WXSP01_YPYS01_YPCM01");
            list.Add("Stock_ZS01_000_SP02_1001_1001");
            list.Add("Stock_ZS01_000_SP1001_YS1001_CM1001");
            list.Add("Stock_ZS01_000_ZS01_1001_1001");
            list.Add("Stock_ZS01_000_ZS02_1001_1001");
            list.Add("Stock_ZS01_000_ZSTX_1001_1001");

            return list.Distinct().ToList();
        }

        [HttpGet("RedisMSet")]
        public bool SetRedisMGet()
        {
            var keys = GetKeys();
            foreach (var key in keys)
            {
                RedisHelper.Set(key, "LQCK01-ZCPCSPZCYS01ZCCM01-14.0000-14.0000-0.0");
            }

            return true;
        }

        /// <summary>
        /// 同步测试方法
        /// </summary>
        /// <returns></returns>
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
