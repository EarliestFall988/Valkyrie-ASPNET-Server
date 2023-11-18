using Avalon;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using System.Text.Json;

using Valkyrie_Server;

var builder = WebApplication.CreateBuilder(args);

var env = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var splash = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n    <title>It Works!!</title>\r\n    <link rel=\"preconnect\" href=\"https://fonts.googleapis.com\">\r\n    <link rel=\"preconnect\" href=\"https://fonts.gstatic.com\" crossorigin>\r\n    <link href=\"https://fonts.googleapis.com/css2?family=Roboto&display=swap\" rel=\"stylesheet\">\r\n</head>\r\n<style>\r\n    .body {\r\n        background-color: #171717;\r\n        width: 90vw;\r\n        height: 90vh;\r\n        color: #ffffff;\r\n        padding: 3rem;\r\n        font-family: 'Roboto', sans-serif;\r\n        display: block;\r\n        box-sizing: border-box;\r\n    }\r\n\r\n    h1 {\r\n        font-size: 3rem;\r\n    }\r\n\r\n    p {\r\n        font-size: 1.25rem;\r\n    }\r\n\r\n    a {\r\n        font-size: 1.25rem;\r\n        color: #ffff;\r\n        text-decoration: none;\r\n        background-color: #1d4ed8;\r\n        padding: 0.25rem;\r\n        border-radius: 0.25rem;\r\n    }\r\n\r\n</style>\r\n<body class=\"body\">\r\n    <h1>It Works!</h1>\r\n    <p>Copy the link in your browser and paste it back in the Valkyrie Connection Page</p>\r\n    <p>or</p>\r\n    <a href=\"/api/v1/sync\">Sync Manually</a>\r\n</body>\r\n</html>";

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var apiKey = env["API_KEY"];
var valkApiKey = "some key";


long minuteCountWaitTime = 5;
StateMachinesController? stateMachinesController = new StateMachinesController(minuteCountWaitTime);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

app.MapGet("/", (HttpContext ctx) =>
{
    ctx.Response.Headers.Add("Content-Type", "text/html");
    ctx.Response.StatusCode = 200;
    return splash;
});

app.MapGet("/api/v1/sync", (HttpContext ctx) =>
{
    ctx.Response.Headers.Add("Content-Type", "application/json");
    ctx.Response.StatusCode = 200;
    return JsonSerializer.Serialize(new message("Syncing"));
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
