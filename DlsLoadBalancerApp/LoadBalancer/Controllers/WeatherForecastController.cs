using Microsoft.AspNetCore.Mvc;
using RestSharp;

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
        public IActionResult Get()
        {
            string url = WeatherForecastController.loadBalancer.NextService();
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest();
            var response = client.GetAsync(request);
            response.Wait();
            RestResponse result = response.Result;
            ObjectResult responseObject = new ObjectResult(result.Content);
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