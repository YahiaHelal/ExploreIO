using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // app.UseRouting() sees this to know route which to which
    public class WeatherforecastController : ControllerBase
    {
        private readonly ILogger<WeatherforecastController> _logger;
         private static readonly string[] Summaries = new[]
    {
        "Freezing", "Cold", "Cool", "Mild", "Warm", "Hot", "Scorching", "asd"
    };

        public WeatherforecastController(ILogger<WeatherforecastController> logger)
        {
            _logger = logger;
        }

       [HttpGet] // remember app.UseEndpoints: map endpoint get, post, put, delete methods to your implementation here
       public IEnumerable<Weatherforecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new Weatherforecast
        {
            Date = DateTime.UtcNow.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        });
    }
    }
}

// request flow
/*
Client Request
↓
Middleware 1 (Exception handling)
↓
Middleware 2 (HTTPS redirect)
↓
Routing middleware → determines endpoint
↓
Authorization middleware
↓
Endpoint execution (controller action)
↓
Response flows backward through middleware chain
*/