using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Diagnostics;

namespace LoadBalancer.Controllers
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
        private static ILoadBalancer loadBalancer;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ILoadBalancer loadBalancer)
        {
            _logger = logger;
            if(WeatherForecastController.loadBalancer == null)
            {
                WeatherForecastController.loadBalancer = loadBalancer;
                WeatherForecastController.loadBalancer.AddService("http://ApiClient/WeatherForecast");
                WeatherForecastController.loadBalancer.AddService("http://ApiClient_1/WeatherForecast");
            }
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get()
        {
            Debug.WriteLine("[" + DateTime.UtcNow.ToString() + "] Load balancer redirects request.");
            string url = WeatherForecastController.loadBalancer.NextService();
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest();
            RestResponse response = await client.GetAsync(request);
            ObjectResult responseObject = new ObjectResult(response.Content);
            responseObject.ContentTypes.Add("application/json");
            return responseObject;
        }

        [HttpGet("list", Name = "ListServices")]
        public IList<string> ListServices()
        {
            return WeatherForecastController.loadBalancer.GetAllServices();
        }

        [HttpGet("add/{url}", Name = "AddService")]
        public IList<string> AddService(string url)
        {
            WeatherForecastController.loadBalancer.AddService(url);
            return WeatherForecastController.loadBalancer.GetAllServices();
        }

        [HttpGet("remove/{url}", Name = "RemoveService")]
        public IList<string> RemoveService(string url)
        {
            WeatherForecastController.loadBalancer.RemoveService(url);
            return WeatherForecastController.loadBalancer.GetAllServices();
        }
    }
}