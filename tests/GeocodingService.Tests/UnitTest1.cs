using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GeocodingService.Tests
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UnitTest1(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddHttpClient("geocoding")
                        .ConfigurePrimaryHttpMessageHandler(() => new FakeGeocodingHandler());
                });
            });
        }

        [Fact]
        public async Task GeocodeEndpoint_ReturnsJson()
        {
            using var client = _factory.CreateClient();

            var response = await client.GetAsync("/geocode?city=Berlin");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        }

        private sealed class FakeGeocodingHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[{\"lat\":\"52.52\",\"lon\":\"13.405\"}]")
                };

                return Task.FromResult(response);
            }
        }
    }
}
