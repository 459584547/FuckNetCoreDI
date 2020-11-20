using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplicationTest.Services;

namespace WebApplicationTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public ILog _logger { set; get; }

        public TestService testService { set; get; }


        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            //log4net.Appender.ColoredConsoleAppender
           
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet("str")]
        public ClassBean GetStr(int s)
        {
            _logger.Info("log4net");
            return new ClassBean()
            {
                C = 1,
                o = testService.x(),
                weatherForecast = new WeatherForecast() { TemperatureC = s }
            };
        }
    }
}
