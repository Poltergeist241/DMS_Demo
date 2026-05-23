var builder = WebApplication.CreateBuilder(args);
var bffUrl = builder.Configuration["BFF_URL"] ?? "http://backendforfrontend:8080";

builder.Services.AddHttpClient("bff", client =>
{
    client.BaseAddress = new Uri(bffUrl);
});
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/weather", async (string city, IHttpClientFactory factory) =>
{
    var client = factory.CreateClient("bff");
    var response = await client.GetAsync($"/weather?city={Uri.EscapeDataString(city)}");
    var content = await response.Content.ReadAsStringAsync();

    return Results.Content(content, response.Content.Headers.ContentType?.ToString() ?? "application/json", statusCode: (int)response.StatusCode);
});

app.Run();
