namespace MiaBDurableVsNormalExecution.Workflow;

using Microsoft.Extensions.Logging;
using Temporalio.Workflows;

[Workflow]
public class CountingWorkflow
{
    [WorkflowRun]
    public async Task<string> RunAsync()
    {
        var logger = Workflow.Logger;

        logger.LogInformation("***Counting to 10***");

        var activityOptions = new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(60) };

        int i = 1;
        while (i <= 10)
        {
            logger.LogInformation("{Number}", i);
            i = await Workflow.ExecuteActivityAsync(
                (Activities act) => act.AddOne(i),
                activityOptions);
            await Workflow.DelayAsync(TimeSpan.FromSeconds(1));
        }

        logger.LogInformation("***Finished counting to 10***");

        return "Completed";
    }
}