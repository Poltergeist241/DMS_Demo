using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var geocodingServiceUrl = builder.Configuration["GEOCODING_SERVICE_URL"] ?? "http://geocodingservice:8080";
var weatherServiceUrl = builder.Configuration["WEATHER_SERVICE_URL"] ?? "http://weatherservice:8080";

builder.Services.AddHttpClient("geocoding", client =>
{
    client.BaseAddress = new Uri(geocodingServiceUrl);
});

builder.Services.AddHttpClient("weather", client =>
{
    client.BaseAddress = new Uri(weatherServiceUrl);
});

var app = builder.Build();

app.MapGet("/weather", async (string city, IHttpClientFactory factory) =>
{
    var geocodingClient = factory.CreateClient("geocoding");
    var geocodeResponse = await geocodingClient.GetStringAsync($"/geocode?city={Uri.EscapeDataString(city)}");

    using var geocodeJson = JsonDocument.Parse(geocodeResponse);
    var results = geocodeJson.RootElement;

    if (results.ValueKind != JsonValueKind.Array || results.GetArrayLength() == 0)
    {
        return Results.NotFound("City not found.");
    }

    var first = results[0];
    var lat = first.GetProperty("lat").GetString();
    var lon = first.GetProperty("lon").GetString();

    if (string.IsNullOrWhiteSpace(lat) || string.IsNullOrWhiteSpace(lon))
    {
        return Results.Problem("Invalid geocoding response.");
    }

    var weatherClient = factory.CreateClient("weather");
    var weatherJson = await weatherClient.GetStringAsync($"/weather?lat={Uri.EscapeDataString(lat)}&lon={Uri.EscapeDataString(lon)}");

    return Results.Content(weatherJson, "application/json");
});

app.Run();
