using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PruebaTecnica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly IDatabase _redis;
        public TestController(HttpClient client, IConnectionMultiplexer muxer)
        {
            _client = client;
            _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("weatherCachingApp", "1.0"));
            _redis = muxer.GetDatabase();
        }
        // GET: api/<TestController>
        [HttpGet]
        public async Task<TestResponse> Get()
        {
            string json;
            var watch = Stopwatch.StartNew();
            var keyName = "countries";
            json = await _redis.StringGetAsync(keyName);

            if (string.IsNullOrEmpty(json))
            {

                HttpResponseMessage response = await _client.GetAsync($"https://restcountries.com/v3.1/region/america");
                if (response.IsSuccessStatusCode)
                {
                    json = await response.Content.ReadAsStringAsync();
                }
                
                var setTask = _redis.StringSetAsync(keyName, json);
                var expireTask = _redis.KeyExpireAsync(keyName, TimeSpan.FromSeconds(3600));
                await Task.WhenAll(setTask, expireTask);
            }


            List<CountryResponse> countries = new List<CountryResponse>();

            using (JsonDocument document = JsonDocument.Parse(json))
            {
                if (document.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement countryElement in document.RootElement.EnumerateArray())
                    {
                        try
                        {
                            string countryName = countryElement.GetProperty("name").GetProperty("common").GetString();
                            string capitalName = countryElement.GetProperty("capital")[0].GetString();
                            JsonElement latlngArray = countryElement.GetProperty("latlng");
                            double latitude = latlngArray[0].GetDouble();
                            double longitude = latlngArray[1].GetDouble();

                            CountryResponse country = new CountryResponse
                            {
                                CountryName = countryName,
                                CapitalName = capitalName,
                                LatLong = new 
                                {
                                    Latitude = latitude,
                                    Longitude = longitude
                                }
                            };

                            countries.Add(country);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error al procesar un país: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("La respuesta no es un array JSON.");
                }
            }
            var result = new TestResponse();
            result.ElapsedTime = watch.ElapsedMilliseconds;
            result.Forecasts = countries;
            return result;
        }

        public class CountryResponse()
        {
            public string CountryName { get; set; }
            public string CapitalName { get; set; }
            public dynamic LatLong { get; set; }
          
        }
        public class TestResponse()
        {
            public long ElapsedTime { get; set; }
            public IEnumerable<CountryResponse> Forecasts { get; set; }
        }

    }
}
