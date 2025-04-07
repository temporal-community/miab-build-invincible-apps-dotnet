// This file is designated to run the Workflow
using MiaBDurableVsNormalExecution.Workflow;
using Microsoft.Extensions.Logging;
using Temporalio.Client;

// Create a client to localhost on "default" namespace
var client = await TemporalClient.ConnectAsync(new("localhost:7233")
{
    LoggerFactory = LoggerFactory.Create(builder =>
        builder
            .AddSimpleConsole(options => options.TimestampFormat = "[HH:mm:ss] ")
            .SetMinimumLevel(LogLevel.Information)),
});

var options = new WorkflowOptions(
            id: "durable-" + DateTime.Now.ToString("HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
            taskQueue: "durable");

// Run workflow
var result = await client.ExecuteWorkflowAsync(
    (CountingWorkflow wf) => wf.RunAsync(),
    options);