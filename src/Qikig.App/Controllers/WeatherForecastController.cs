using FreeRedis;
using Microsoft.AspNetCore.Mvc;
using Qikig.App.Extension;
using Qikig.Service;

namespace Qikig.App.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly IUserService _userService;
        private RedisClient  _redisClient;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IUserService userService, FreeRedisService redisClient)
        {
            _logger = logger;
            _userService = userService;
            _redisClient = redisClient.Instance;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            var redisinfo =  _redisClient.Get("key");//redis使用
            var u= _userService.GetUserName(2);//读取数据库mysql

            _logger.LogInformation("User Name: {UserName}", u);
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
