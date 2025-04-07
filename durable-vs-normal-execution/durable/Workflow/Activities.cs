namespace MiaBDurableVsNormalExecution.Workflow;

using Temporalio.Activities;

public class Activities
{
    [Activity]
    public int AddOne(int num)
    {
        return num + 1;
    }
}