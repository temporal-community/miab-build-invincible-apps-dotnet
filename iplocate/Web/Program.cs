using IPLocate.Workflow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Temporalio.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services for static files
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

// Create a client to localhost on "default" namespace
var client = await TemporalClient.ConnectAsync(new("localhost:7233")
{
    LoggerFactory = LoggerFactory.Create(builder =>
        builder
            .AddSimpleConsole(options => options.TimestampFormat = "[HH:mm:ss] ")
            .SetMinimumLevel(LogLevel.Information)),
});

// Enable static files and set default files
app.UseDefaultFiles();
app.UseStaticFiles();

// Map the root to serve index.html
app.MapGet("/", () => Results.File("index.html", "text/html"));

// Add demo-options
app.MapGet("/demo-options", () => WorkflowConstants.DemoOptions);

app.MapPost("/greet", async (HttpContext context) =>
{
    var form = context.Request.Form;
    var name = form["name"].ToString();
    int seconds = 0;
    if (!string.IsNullOrEmpty(form["sleep_duration"].ToString()))
    {
        seconds = int.Parse(form["sleep_duration"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
    }

    var input = new GetAddressFromIPWorkflow.WorkflowInput(name, seconds);

    var options = new WorkflowOptions(
        id: "greeting-workflow-" + DateTime.Now.ToString("HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
        taskQueue: WorkflowConstants.TaskQueueName);

    // Run workflow
    var result = await client.ExecuteWorkflowAsync(
        (GetAddressFromIPWorkflow wf) => wf.RunAsync(input),
        options);

    return $"Hello, {name}!<br> Your IP Address is <code>{result.IP}</code>.<br> You are in {result.Location}";
});

await app.RunAsync();