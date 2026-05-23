var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("geocoding");

var app = builder.Build();

app.MapGet("/geocode", async (string city, IHttpClientFactory factory) =>
{
    var client = factory.CreateClient("geocoding");

    var url = $"https://nominatim.openstreetmap.org/search" +
              $"?q={Uri.EscapeDataString(city)}" +
              $"&format=json&limit=1";

    client.DefaultRequestHeaders.UserAgent.ParseAdd("WeatherApp/1.0");

    var json = await client.GetStringAsync(url);

    return Results.Content(json, "application/json");
});

app.Run();