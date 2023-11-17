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

StateMachinesController? stateMachinesController = new StateMachinesController();

long minuteCountWaitTime = 5;

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

    if (response.result == false)
    {
        context.Response.StatusCode = 500;
        return response.content;
    }

    string id = Guid.NewGuid().ToString();

    if (!stateMachinesController.IsTicking)
    {
        stateMachinesController.Boot();
    }


    //Debug.WriteLine(response.content);


    stateMachinesController.AddMachine(id, response.content);

    (bool complete, string result) status = (false, "Not completed - state machine took too long.");

    long tick = 0;


    var totalTime = 100; //(60000 / 2) * minuteCountWaitTime;

    while (!status.complete && tick < totalTime)
    {

       Debug.WriteLine("status: " + status.complete + " " + tick);

        status = stateMachinesController.HandleStatus(id);

        Thread.Sleep(100); // wait for the machine to complete
        tick++;
    }

    if (stateMachinesController.GetMachines.Length == 0 && stateMachinesController.IsTicking)
    {
        stateMachinesController.Kill();
    }

    if (!status.complete && tick >= totalTime)
    {
        context.Response.StatusCode = 408;
        stateMachinesController.KillStateMachineProcess(id); //remove the machine from the list of machines to process

        return status.result;
    }

    if (status.complete)
    {
        context.Response.StatusCode = 200;
        return status.result;
    }
    else
    {
        context.Response.StatusCode = 500;
        return status.result;
    }
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