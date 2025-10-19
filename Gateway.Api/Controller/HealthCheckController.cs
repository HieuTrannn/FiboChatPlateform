using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Api.Controller
{
    [Route("api/heathcheck")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HealthCheckController> _logger;

        public HealthCheckController(IHttpClientFactory httpClientFactory, ILogger<HealthCheckController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        [HttpGet("check")]
        public async Task<IActionResult> Check()
        {
            var services = new Dictionary<string, string>
        {
            { "Auth", "http://authentication:5001/api/health/ping" },
            { "Course", "http://course-api:5002/api/health/ping" }
        };

            var results = new List<object>();

            foreach (var s in services)
            {
                try
                {
                    var res = await _httpClient.GetAsync(s.Value);
                    var status = res.IsSuccessStatusCode ? "OK" : "Error";
                    results.Add(new { service = s.Key, status });
                }
                catch (Exception ex)
                {
                    results.Add(new { service = s.Key, status = "Down", error = ex.Message });
                    _logger.LogError(ex, $"{s.Key} service unreachable");
                }
            }

            return Ok(results);
        }
    }
}
