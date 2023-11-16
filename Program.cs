using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

using Valkyrie_Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var apiKey = "some api key";
var valkApiKey = "some key";

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapPost("/api/v1/instruction/{id}", async (HttpContext context) =>
{

    var instructionId = context.Request.RouteValues["id"] as string;
    string key = context.Request.Headers["apikey"];

    if (string.IsNullOrEmpty(key))
    {
        context.Response.StatusCode = 401;
        return "No API key provided";
    }

    if (apiKey != key)
    {
        context.Response.StatusCode = 401;
        return "incorrect api key";
    }


    if (string.IsNullOrEmpty(instructionId))
    {
        context.Response.StatusCode = 400;
        return "No instruction ID provided";
    }

    var ValkyireServerController = new ValkyrieServerController()
    {
        ValkyrieAPIKey = valkApiKey
    };

    var response = await ValkyireServerController.TryGetInstructions(instructionId);

    return response;

    //using (StreamReader reader = new StreamReader(context.Request.Body))
    //{
    //    string body = await reader.ReadToEndAsync();
    //    context.Response.StatusCode = 200;
    //    return body;
    //}
});

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}