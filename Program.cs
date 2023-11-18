using Avalon;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using System.Text.Json;

using Valkyrie_Server;

var builder = WebApplication.CreateBuilder(args);

var appsetting = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var apiKey = appsetting["API_KEY"];
var valkApiKey = "some key";

Debug.WriteLine("\napi key: " + apiKey + "\n");


long minuteCountWaitTime = 5;
StateMachinesController? stateMachinesController = new StateMachinesController(minuteCountWaitTime);



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

app.MapGet("/", (HttpContext ctx) =>
{
    ctx.Response.Headers.Add("Content-Type", "application/json");
    ctx.Response.StatusCode = 200;
    return JsonSerializer.Serialize(new message("Hello World!"));
});


app.MapPost("/api/v1/instruction/{id}", async (HttpContext context) =>
{

    context.RequestAborted.ThrowIfCancellationRequested();

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

    stateMachinesController.AddMachine(id, response.content);

    (bool complete, string result) status = (false, "Not completed - state machine took too long.");

    long tick = 0;


    var totalTime = (60000 / 2) * minuteCountWaitTime;

    while (!status.complete && tick < totalTime && !context.RequestAborted.IsCancellationRequested)
    {

        Debug.WriteLine("status: " + status.complete + " " + tick);

        status = stateMachinesController.HandleStatus(id);

        Thread.Sleep(0); // wait for the machine to complete
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

    if (context.RequestAborted.IsCancellationRequested)
    {
        Debug.WriteLine("request aborted");
        stateMachinesController.KillStateMachineProcess(id);
        context.Response.StatusCode = 408;
        return "";
    }

    if (status.complete)
    {
        context.Response.StatusCode = 200;
        context.Response.Headers.Add("Content-Type", "application/json");
        return status.result;
    }
    else
    {
        context.Response.StatusCode = 500;
        return status.result;
    }
});


app.MapGet("/api/v1/functions", (HttpContext context) =>
{

    string key = context.Request.Headers["apikey"];

    if (key == null)
    {
        context.Response.StatusCode = 401;
        return "No API key provided";
    }

    if (apiKey != key)
    {
        context.Response.StatusCode = 401;
        return "incorrect api key";
    }

    var lib = new FunctionLibrary();

    try
    {

        var result = JsonSerializer.Serialize(lib.ImportedFunctions.Select(x =>
        {
            return new FunctionListItem(x.Key, x.Value.Description, x.Value.ExpectedParameters.Select(x => x.Value).ToArray());
        }));


        context.Response.Headers.Add("Content-Type", "application/json");
        context.Response.StatusCode = 200;
        return result;
    }
    catch (Exception e)
    {
        context.Response.StatusCode = 500;
        return e.Message;
    }
});

app.Run();


/// <summary>
/// The content of the request to the Valkyrie server.
/// </summary>
/// <param name="Name">The name of the function</param>
/// <param name="Description">The description of the function</param>
/// <param name="Parameters">the parameters of the function</param>
internal record FunctionListItem(string Name, string Description, ReferenceTuple[] Parameters);

internal record message(string content);



#region stuff

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateTime.Now.AddDays(index),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");


//internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}

#endregion
