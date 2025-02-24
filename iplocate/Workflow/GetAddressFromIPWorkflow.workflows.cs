namespace IPLocate.Workflow;

using Microsoft.Extensions.Logging;
using Temporalio.Workflows;

[Workflow]
public class GetAddressFromIPWorkflow
{
    public record WorkflowInput(string Name, int Seconds);

    public record WorkflowOutput(string IP, string Location);

    [WorkflowRun]
    public async Task<WorkflowOutput> RunAsync(WorkflowInput input)
    {
        var activityOptions = new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(60) };

        var ip = await Workflow.ExecuteActivityAsync(
           (Activities act) => act.GetIPAsync(),
           activityOptions);

        if (input.Seconds > 0)
        {
            await Workflow.DelayAsync(TimeSpan.FromSeconds(input.Seconds));
        }

        var location = await Workflow.ExecuteActivityAsync(
            (Activities act) => act.GetLocationInfoAsync(ip),
            activityOptions);

        return new WorkflowOutput(ip, location);
    }
}