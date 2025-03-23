using System.Net.Http.Headers;
using System.Net;
using Moq;
using StackExchange.Redis;
using PruebaTecnica.Controllers;
using Microsoft.Extensions.Http;
using Moq.Protected;
using Microsoft.AspNetCore.Mvc.Core;

namespace UnitTest
{
    public class TestControllerTests
    {
        
            [Fact]
            public async Task Get_ReturnsDataFromRedis_WhenCacheIsAvailable()
            {
                // Arrange
                var mockClient = new Mock<HttpClient>();
                var mockMuxer = new Mock<IConnectionMultiplexer>();
                var mockDb = new Mock<IDatabase>();

                // Configurar el mock de Redis para devolver datos cacheados
                string cachedJson = "[{\"name\":{\"common\":\"Cached Country\"},\"capital\":[\"Cached Capital\"],\"latlng\":[1.0,2.0]}]";
                mockDb.Setup(db => db.StringGetAsync("countries", CommandFlags.None))
                      .ReturnsAsync(cachedJson);

                mockMuxer.Setup(muxer => muxer.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                         .Returns(mockDb.Object);

                var controller = new TestController(mockClient.Object, mockMuxer.Object);

                // Act
                var result = await controller.Get(60);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("redis", result.Origin);
                Assert.NotEmpty(result.Forecasts);
                Assert.Equal("Cached Country", result.Forecasts.First().CountryName);
            }

            [Fact]
            public async Task Get_ReturnsDataFromApi_WhenCacheIsExpired()
            {
                // Arrange
                var mockClient = new Mock<HttpClient>();
                var mockMuxer = new Mock<IConnectionMultiplexer>();
                var mockDb = new Mock<IDatabase>();

                // Configurar el mock de Redis para no devolver datos cacheados (cache expirada)
                mockDb.Setup(db => db.StringGetAsync("countries", CommandFlags.None))
                      .ReturnsAsync(string.Empty);

                // Configurar el mock de HttpClient para devolver datos de la API
                string apiResponseJson = "[{\"name\":{\"common\":\"API Country\"},\"capital\":[\"API Capital\"],\"latlng\":[3.0,4.0]}]";
              
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(apiResponseJson)
                    });
                
                mockMuxer.Setup(muxer => muxer.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                         .Returns(mockDb.Object);

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);

            var controller = new TestController(httpClient, mockMuxer.Object);

                // Act
                var result = await controller.Get(60);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("api", result.Origin);
                Assert.NotEmpty(result.Forecasts);
                Assert.Equal("API Country", result.Forecasts.First().CountryName);
            }
        
    }
}