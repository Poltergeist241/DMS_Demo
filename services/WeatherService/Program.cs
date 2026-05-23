using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("weather");

var app = builder.Build();

app.MapGet("/weather", async (double lat, double lon, IHttpClientFactory factory) =>
{
    var client = factory.CreateClient("weather");

    var url =
        $"https://api.open-meteo.com/v1/forecast" +
        $"?latitude={lat}&longitude={lon}" +
        // $"&hourly=temperature_2m,rain" +
        $"&current=temperature_2m,rain";

    var json = await client.GetStringAsync(url);

    return Results.Content(json, "application/json");
});

app.Run();
