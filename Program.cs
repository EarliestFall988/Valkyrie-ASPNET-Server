using ValkyrieFSMCore;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using System.Text.Json;

using Valkyrie_Server;
using ValkyrieFSMCore.WM;

var builder = WebApplication.CreateBuilder(args);

var env = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

string linkURI = env["VALK_DASHBOARD_LINK"];

string version = env["VERSION"];

var splash = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n    <title>It Works!!</title>\r\n    <link rel=\"preconnect\" href=\"https://fonts.googleapis.com\">\r\n    <link rel=\"preconnect\" href=\"https://fonts.gstatic.com\" crossorigin>\r\n    <link href=\"https://fonts.googleapis.com/css2?family=Roboto&display=swap\" rel=\"stylesheet\">\r\n</head>\r\n<script>\r\n\r\n    getURI = () => {\r\n        document.getElementById(\"link-to-paste\").innerHTML = window.location.href + \"api/v1/sync\";\r\n    }\r\n\r\n</script>\r\n<style>\r\n    .body {\r\n        background-color: #171717;\r\n        width: 90vw;\r\n        height: 90vh;\r\n        color: #ffffff;\r\n        padding: 3rem;\r\n        font-family: 'Roboto', sans-serif;\r\n        display: block;\r\n        box-sizing: border-box;\r\n    }\r\n\r\n    h1 {\r\n        font-size: 3rem;\r\n    }\r\n\r\n    h1, h2 {\r\n        user-select: none;\r\n    }\r\n\r\n    h2 {\r\n        font-size: 1.5rem;\r\n        font-weight: 200;\r\n    }\r\n\r\n    p {\r\n        font-size: 1.25rem;\r\n    }\r\n\r\n    a {\r\n        font-size: 1.25rem;\r\n        color: #ffff;\r\n        text-decoration: none;\r\n        background-color: #1d4ed8;\r\n        padding: 0.25rem;\r\n        border-radius: 0.25rem;\r\n    }\r\n\r\n    .sync-link {\r\n        color: lightblue;\r\n        text-decoration: underline;\r\n        text-underline-offset: 0.25rem;\r\n        font-size: 1.5rem;\r\n    }\r\n\r\n    .no-select {\r\n        user-select: none;\r\n    }\r\n</style>\r\n<body onload=\"getURI()\" class=\"body\">\r\n " +
    $"<h1>It Works!🎉🎉</h1>\r\n    <h2>Copy the link below and paste it back in the Valkyrie Connection Page</h2>\r\n    <p> <span class=\"no-select\">👉 </span><span id=\"link-to-paste\" class=\"sync-link\"></span></p>\r\n    <p class=\"no-select\"><a href=\"{linkURI ?? "https://google.com"}\"> Go To Dashboard </a></p> <p class=\"no-select\">v{version}</p>\r\n</body>\r\n</html>\r\n";
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var apiKey = env["API_KEY"];
var valkApiKey = "some key";

var projectTest = new GetProjects();
projectTest.Parameters.Add("out", VariableDefinition<List<Project>>.CreateCustom("out", "projects", new()));
var result = projectTest.Function();

Debug.WriteLine("projects ");
foreach (var x in projectTest.Get<List<Project>>("out"))
{
    Debug.WriteLine(x.Name);
}

Debug.WriteLine("\n\n\n\n");

var project = projectTest.Get<List<Project>>("out")[0];

var split = new SplitProject();
split.Parameters.Add("project", new VariableDefinition<Project>("project", project, "project"));
split.Parameters.Add("name", VariableDefinition<string>.CreateString("name", ""));
split.Parameters.Add("id", VariableDefinition<string>.CreateString("id", ""));


var splitResult = split.Function();

Debug.WriteLine("split result: " + splitResult);

Debug.WriteLine("name: " + split.Get<string>("name"));


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

app.MapGet("/api/v1/sync", async (HttpContext ctx) =>
{

    string instructionId = ctx.Request.Headers["id"];
    string key = ctx.Request.Headers["apikey"];

    ctx.Response.Headers.Add("Content-Type", "application/json");

    if (string.IsNullOrEmpty(key))
    {
        ctx.Response.StatusCode = 401;
        return JsonSerializer.Serialize(new message("No API key provided"));
    }

    if (apiKey != key)
    {
        ctx.Response.StatusCode = 401;
        return JsonSerializer.Serialize(new message("incorrect api key"));
    }

    if (string.IsNullOrEmpty(instructionId))
    {
        ctx.Response.StatusCode = 400;
        return JsonSerializer.Serialize(new message("No instruction ID provided"));
    }

    Debug.WriteLine("test 1");

    ValkyrieServerController valkyrieServerController = new ValkyrieServerController()
    {
        ValkyrieAPIKey = valkApiKey
    };


    Debug.WriteLine("test 2");


    var res = await valkyrieServerController.UpdateInstructionFunctionDefinitions(instructionId);
    if (res.statusCode.ToLower() == "ok")
    {
        ctx.Response.StatusCode = 200;
        return JsonSerializer.Serialize(new message("Synced!"));
    }

    ctx.Response.StatusCode = 500;

    return JsonSerializer.Serialize(new errResult(res.response, res.statusCode));
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


    try
    {
        context.Response.Headers.Add("Content-Type", "application/json");
        context.Response.StatusCode = 200;
        return DiscoverFunctionsHandler.GetFunctionDefinitionsJSON();
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
internal record FunctionListItem(string Name, string Description, ExpectedParameter[] Parameters);

internal record message(string content);
internal record errResult(string error, string reasonPhrase);



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
